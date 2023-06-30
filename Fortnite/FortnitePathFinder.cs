using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimbaDevPublicLauncher.Fortnite
{
    internal class FortnitePathFinder
    {
        public async Task<string> GetFortPath()
        {
            try
            {
                var path1 = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Epic\\UnrealEngineLauncher\\LauncherInstalled.dat"));
                dynamic json = JsonConvert.DeserializeObject(path1);

                foreach (var installation in json.InstallationList)
                {
                    if (installation.AppName == "Fortnite")
                    {
                        var path = installation.InstallLocation.ToString() + "";
                        Program.FortnitePath = path.Replace('/', '\\');
                    }
                }

                if (string.IsNullOrEmpty(Program.FortnitePath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Logging.Logging.Log("Your Fortnite Path Appears To Be Missing Or Broken. Please verify Fortnite!\nPress Any Key To Close Simba!");
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

            return Program.FortnitePath;
        }
    }
}
