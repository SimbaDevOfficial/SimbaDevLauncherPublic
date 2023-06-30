using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimbaDevPublicLauncher.utils;

namespace SimbaDevPublicLauncher
{
    // Launcher by @conspiracyy cleaned up by @DRQSuperior

    // this is open source

    // made so ppl stop calling it a rat (Special EDS)
    internal class Program
    {
        private static string FortnitePath { get; set; }
        private static string DiscordUrl { get; set; }
        private static string Version { get; set; }
        private static string Response { get; set; }
        private static string[] Args { get; set; }

        public static async Task<string> GetAuth()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"
  ___ _       _            ___          
 / __(_)_ __ | |__  __ _  |   \ _____ __
 \__ \ | '  \| '_ \/ _` | | |) / -_) V /
 |___/_|_|_|_|_.__/\__,_| |___/\___|\_/ 
");
            Console.WriteLine("_______________________________________________ \n\n");

            Logging.Log("Thank you for using Simba Dev! Launcher made by @conspiracyy on Discord, SSL and backend by DRQSuperior!");
            Logging.Log("Please go to the following link and provide the auth code after code=, or paste the entire response! https://rebrand.ly/authcode\n\n");
            Process.Start(new ProcessStartInfo("https://rebrand.ly/authcode") { UseShellExecute = true });

            var code = Logging.Input("Auth Code");

            if (string.IsNullOrEmpty(code))
            {
                return "invalidauth";
            }

            if (code.Length == 32)
            {
                var exchange = await Auth.GetExchange(code);

                if (exchange == "invalidauth")
                {
                    return "invalidauth";
                }
            }

            Regex regex = new Regex(@"([a-zA-Z0-9]{32})"); // 32 alphanumeric characters
            Match match = regex.Match(code);

            if (match.Success)
            {
                var authCode = match.Groups[1].Value;
                var exchange = await Auth.GetExchange(authCode);

                if (exchange == "invalidauth")
                {
                    return "invalidauth";
                }
            }
            else
            {
                return "invalidauth";
            }

            return "bruh";
        }

        private static void Main(string[] args)
        {
            Args = args;

            try
            {
                string url = "https://raw.githubusercontent.com/DRQSuperior/simba-api/main/update.json";

                using (var client = new WebClient())
                {
                    Response = client.DownloadString(url);
                }

                JObject launcherInfo = JObject.Parse(Response);

                Version = (string)launcherInfo["version"];
                DiscordUrl = (string)launcherInfo["discord_url"];
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Process.GetCurrentProcess().Kill();
            }

            GetFortPath().GetAwaiter().GetResult();

            Console.Title = "Simba Dev | " + DiscordUrl.Replace("https://", "") + " | Version: " + Version;
            Console.ForegroundColor = ConsoleColor.Magenta;


            string authXD = GetAuth().GetAwaiter().GetResult();

            if (authXD == "invalidauth")
            {
                Console.Clear();
                Logging.Log("Your auth was invalid.");
                authXD = GetAuth().GetAwaiter().GetResult();
            }

            if (authXD.StartsWith("Error"))
            {
                Console.Clear();
                Logging.Log("An error occurred while authenticating. Error: " + authXD);
                authXD = GetAuth().GetAwaiter().GetResult();
            }

            Logging.Log("Launching Simba Dev! Enjoy :D");
            Launch.LaunchDev(FortnitePath, authXD);
        }

        private static async Task GetFortPath()
        {
            string path1 = "";
            string version = "1";

            try
            {
                path1 = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Epic\\UnrealEngineLauncher\\LauncherInstalled.dat"));
                dynamic json = JsonConvert.DeserializeObject(path1);

                foreach (var installation in json.InstallationList)
                {
                    if (installation.AppName == "Fortnite")
                    {
                        path1 = installation.InstallLocation.ToString() + "";
                        version = installation.AppVersion.ToString().Split('-')[1];
                        FortnitePath = path1.Replace('/', '\\');
                    }
                }

                if (string.IsNullOrEmpty(FortnitePath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Logging.Log("Your Fortnite Path Appears To Be Missing Or Broken. Please verify Fortnite!\nPress Any Key To Close Simba!");
                    Console.ReadKey();
                    Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while getting Fortnite path. Please verify your game. Error: " + ex);
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}