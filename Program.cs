using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimbaDevPublicLauncher.Utils;

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

        static string currentVersion = "3.0";

        static string lastOpenedDiscordTimeFile = "other.simba";

        static void CheckForUpdates()
        {
            Logging.Log("Please wait while we check for updates...");

            string apiURL = "https://raw.githubusercontent.com/DRQSuperior/simba-api/main/update.json";

            try
            {
                WebClient client = new WebClient();
                string apiJson = client.DownloadString(apiURL);

                JObject apiData = JObject.Parse(apiJson);
                string latestVersion = (string)apiData["version"];

                if (latestVersion != currentVersion)
                {
                    string updateURL = (string)apiData["update_url"];
                    string input = Logging.Input("Launcher is outdated. Would you like to open the update download? (Y/N)");
                    if (input.ToLower() == "y")
                    {
                        System.Diagnostics.Process.Start(updateURL);
                    } else
                    {
                        Logging.Log("Please update your launcher to continue.");
                        Console.Read();
                        Environment.Exit(0);
                    }
                }

                bool downtime = (bool)apiData["downtime"];
                if (downtime)
                {
                    string downtimeReason = (string)apiData["downtime_reason"];
                    Logging.Error("Simba is currently down for maintenance. Reason: " + downtimeReason);
                    return;
                }

                string discordURL = (string)apiData["discord_url"];
                if (!string.IsNullOrEmpty(discordURL))
                {
                    if (ShouldOpenDiscord())
                    {
                        System.Diagnostics.Process.Start(discordURL);

                        UpdateLastOpenedDiscordTime();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error("Error checking for updates: " + ex.Message);
                Console.ReadKey();
                Environment.Exit(0);
            }

            Thread.Sleep(3000);
            Console.Clear();
        }

        static bool ShouldOpenDiscord()
        {
            if (File.Exists(lastOpenedDiscordTimeFile))
            {
                string lastOpenedTime = File.ReadAllText(lastOpenedDiscordTimeFile);
                DateTime lastOpenedDateTime;
                if (DateTime.TryParse(lastOpenedTime, out lastOpenedDateTime))
                {
                    TimeSpan timeSinceLastOpened = DateTime.Now - lastOpenedDateTime;
                    return timeSinceLastOpened.TotalHours >= 1; 
                }
            }

            return true; 
        }

        static void UpdateLastOpenedDiscordTime()
        {
            // Update the last opened Discord time to the current time
            File.WriteAllText(lastOpenedDiscordTimeFile, DateTime.Now.ToString());
        }

        public static async Task<string> GetAuth()
        {
            CheckForUpdates();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("_______________________________________________");
            Console.WriteLine(@"
     ___ _       _            ___          
    / __(_)_ __ | |__  __ _  |   \ _____ __
    \__ \ | '  \| '_ \/ _` | | |) / -_) V /
    |___/_|_|_|_|_.__/\__,_| |___/\___|\_/ 
");
            Console.WriteLine("_______________________________________________\n\n");

            Logging.Log("Thank you for using Simba Dev! Launcher made by @conspiracyy on Discord, SSL and backend by DRQSuperior!");
            Logging.Log("Please Login to continue.");

            string devicecode = Auth.Auth.GetDeviceCode(Auth.Auth.GetDevicecodetoken());
            string[] array = devicecode.Split(new char[] { ',' }, 2);
            if (devicecode.Contains("error"))
            {
                Logging.Error("Error: " + devicecode);
                return "Error: " + devicecode;
            }
            string token = array[0];

            string exchange = Auth.Auth.GetExchange(token);

            Logging.Log("Logging in...");

            Logging.Log("Welcome! " + Auth.AccountInfo.DisplayName.ToString() + " Enjoy Simba Dev! :D");

            Launch.LaunchDev(FortnitePath, exchange);

            Logging.Log("Launching Simba Dev! Enjoy :D");

            return "Success";
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