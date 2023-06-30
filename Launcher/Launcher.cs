using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimbaDevPublicLauncher.Launcher
{
    internal class Launcher
    {
        public async Task Run()
        {
            try
            {
                Updates.UpdateChecker updateChecker = new Updates.UpdateChecker();
                await updateChecker.CheckForUpdates();

                Discord.DiscordManager discordManager = new Discord.DiscordManager();
                await discordManager.InitiateDiscord();

                Fortnite.FortnitePathFinder pathFinder = new Fortnite.FortnitePathFinder();
                Program.FortnitePath = await pathFinder.GetFortPath();

                Console.Title = "Simba Dev | " + Program.DiscordUrl.Replace("https://", "") + " | Version: " + Program.Version;
                Console.ForegroundColor = ConsoleColor.Magenta;

                Auth.AuthManager authManager = new Auth.AuthManager();
                var authResult = await authManager.GetAuth();

                while (authResult.StartsWith("Error") || authResult == "invalidauth")
                {
                    Console.Clear();
                    Logging.Logging.Log(authResult.StartsWith("Error") ? "An error occurred while authenticating. Error: " + authResult : "Your auth was invalid.");
                    authResult = await authManager.GetAuth();
                }

                Logging.Logging.Log("Launching Simba Dev! Enjoy :D");
                Launch.LaunchDev(Program.FortnitePath, authResult);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}
