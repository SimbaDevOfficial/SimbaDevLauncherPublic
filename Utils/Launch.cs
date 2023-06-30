using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using SimbaDevPublicLauncher.utils;
using Win32;

namespace SimbaDevPublicLauncher.utils
{
    internal class Launch
    {
        public static string currentDir = Directory.GetCurrentDirectory();
        public static string tempPath = Path.GetTempPath();
        public static readonly string apiURL = "https://raw.githubusercontent.com/DRQSuperior/simba-api/main/update.json";

        public static string getDllUrl()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string apiJson = client.DownloadString(apiURL);

                    JObject apiData = JObject.Parse(apiJson);
                    string encodedUrl = (string)apiData["dll_url"];

                    byte[] urlBytes = Convert.FromBase64String(encodedUrl);
                    string firstDecodedUrl = Encoding.UTF8.GetString(urlBytes);
                    byte[] firstDecodedBytes = Convert.FromBase64String(firstDecodedUrl);
                    string decodedUrl = Encoding.UTF8.GetString(firstDecodedBytes);
                    return decodedUrl;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving DLL URL: " + ex.Message);
                return null;
            }
        }

        public static string getInjectorUrl()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string apiJson = client.DownloadString(apiURL);

                    JObject apiData = JObject.Parse(apiJson);
                    string encodedUrl = (string)apiData["injector_url"];

                    byte[] urlBytes = Convert.FromBase64String(encodedUrl);
                    string firstDecodedUrl = Encoding.UTF8.GetString(urlBytes);
                    byte[] firstDecodedBytes = Convert.FromBase64String(firstDecodedUrl);
                    string decodedUrl = Encoding.UTF8.GetString(firstDecodedBytes);
                    return decodedUrl;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving injector URL: " + ex.Message);
                return null;
            }
        }

        public static async void LaunchDev(string path, string exchange)
        {
            // Define the filenames for Fortnite executables
            string[] fileNames = {
                "FortniteClient-Win64-Shipping.exe",
                "FortniteClient-Win64-Shipping_EAC.exe",
                "FortniteClient-Win64-Shipping_EAC_EOS.exe",
                "FortniteClient-Win64-Shipping_BE.exe",
                "FortniteLauncher.exe"
            };

            // Check if all required Fortnite files exist
            foreach (string fileName in fileNames)
            {
                string filePath = Path.Combine(path, $"FortniteGame\\Binaries\\Win64\\{fileName}");
                if (!File.Exists(filePath))
                {
                    ShowFileMissingMessage(fileName);
                    return;
                }
            }

            // Start the Fortnite client process
            string FN = Path.Combine(path, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe");
            Process Fortnite = new Process
            {
                StartInfo = new ProcessStartInfo(FN)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = true,
                    Arguments = "-AUTH_LOGIN=unused AUTH_TYPE=exchangecode -epicapp=Fortnite -epicenv=Prod -nobe -skippatchcheck -fromfl=eac -epicportal -epiclocale=en-us -AUTH_PASSWORD=" + exchange
        }
            };
            Fortnite.Start();

            // Start the Fortnite launcher process and suspend its threads
            string FNL = Path.Combine(path, "FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe");
            Process FNLP = Process.Start(FNL);
            SuspendProcessThreads(FNLP);

            // Start the Fortnite EAC process and suspend its threads
            string EAC = Path.Combine(path, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_EAC.exe");
            Process EACP = Process.Start(EAC, "EAC ARGS");
            SuspendProcessThreads(EACP);

            Thread.Sleep(2000);

            // Download and inject the Simba.dll file
            string dllPath = Path.Combine(currentDir, "Simba.dll");
            if (File.Exists(dllPath))
            {
                // Delete the existing DLL file if it exists
                File.Delete(dllPath);
            }
            DownloadFile(getDllUrl(), dllPath);

            Thread.Sleep(6000);

            // Download the injector.exe file
            string injectorPath = Path.Combine(tempPath, "Injector.exe");
            if (File.Exists(injectorPath))
            {
                // Delete the existing injector file if it exists
                File.Delete(injectorPath);
            }
            if (!DownloadFile(getInjectorUrl(), injectorPath))
            {
                Console.WriteLine("Failed to download injector!");
                return;
            }

            try
            {
                Fortnite.WaitForInputIdle();
            }
            catch
            {
                Console.WriteLine("Fortnite Has unexpectedly crashed. (Probably)");
                // Close the Fortnite launcher and EAC processes
                FNLP.Close();
                EACP.Close();
                Fortnite.Close();

                // Kill any remaining Fortnite-related processes
                foreach (string processName in fileNames)
                {
                    string[] fileNameParts = processName.Split('.');
                    foreach (Process process in Process.GetProcessesByName(fileNameParts[0]))
                    {
                        process.Kill();
                    }
                }
                // if it fails here im going to cry fr
            }

            // Start the injector process with the necessary arguments
            string injectorArgs = string.Format("\"{0}\" \"{1}\"", Fortnite.Id, dllPath);
            Process.Start(injectorPath, injectorArgs);

            // Wait for the Fortnite process to exit
            Fortnite.WaitForExit();

            // Close the Fortnite launcher and EAC processes
            FNLP.Close();
            EACP.Close();
            Fortnite.Close();

            // Kill any remaining Fortnite-related processes
            foreach (string processName in fileNames)
            {
                string[] fileNameParts = processName.Split('.');
                foreach (Process process in Process.GetProcessesByName(fileNameParts[0]))
                {
                    process.Kill();
                }
            }

            Logging.Log("Thanks for using Simba Dev :)");
        }

        private static void ShowFileMissingMessage(string fileName)
        {
            Console.WriteLine(fileName + " does not exist!");
        }

        private static void SuspendProcessThreads(Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                Win32B.SuspendThread(Win32B.OpenThread(2, false, thread.Id));
            }
        }

        private static bool DownloadFile(string url, string filePath)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(url, filePath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}