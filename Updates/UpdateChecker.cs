using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimbaDevPublicLauncher.Updates
{
    internal class UpdateChecker
    {
        public async Task CheckForUpdates()
        {
            Logging.Logging.Log("Please wait while we check for updates...");

            try
            {
                using (var client = new WebClient())
                {
                    Program.Response = await client.DownloadStringTaskAsync(Program.UpdateApiUrl);
                }

                var launcherInfo = JObject.Parse(Program.Response);

                Program.Version = (string)launcherInfo["version"];
                Program.DiscordUrl = (string)launcherInfo["discord_url"];

                var latestVersion = (string)launcherInfo["version"];
                var updateUrl = (string)launcherInfo["update_url"];
                var downtime = (bool)launcherInfo["downtime"];
                var downtimeReason = (string)launcherInfo["downtime_reason"];

                if (latestVersion != Program.CurrentVersion)
                {
                    var input = Logging.Logging.Input("Launcher is outdated. Would you like to open the update download? (Y/N)");

                    if (input.ToLower() == "y")
                    {
                        Process.Start(updateUrl);
                    }
                    else
                    {
                        Logging.Logging.Log("Please update your launcher to continue.");
                        Console.Read();
                        Environment.Exit(0);
                    }
                }

                if (downtime)
                {
                    Logging.Logging.Error("Simba is currently down for maintenance. Reason: " + downtimeReason);
                    return;
                }
            }
            catch (Exception ex)
            {
                Logging.Logging.Error("Error checking for updates: " + ex.Message);
                Console.ReadKey();
                Environment.Exit(0);
            }

            await Task.Delay(3000);
            Console.Clear();
        }
    }
}
