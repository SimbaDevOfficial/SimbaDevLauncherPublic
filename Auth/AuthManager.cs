using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimbaDevPublicLauncher.Auth
{
    internal class AuthManager
    {
        public async Task<string> GetAuth()
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

            Logging.Logging.Log("Thank you for using Simba Dev! Launcher made by @conspiracyy on Discord, SSL and backend by DRQSuperior!");
            Logging.Logging.Log("Please Login to continue.");

            var devicecode = Auth.GetDeviceCode(Auth.GetDevicecodetoken());

            var array = devicecode.Split(new char[] { ',' }, 2);
            if (devicecode.Contains("error"))
            {
                Logging.Logging.Error("Error: " + devicecode);
                return "Error: " + devicecode;
            }
            var token = array[0];

            var exchange = Auth.GetExchange(token);

            Logging.Logging.Log("Logging in...");

            Logging.Logging.Log("Welcome! " + AccountInfo.DisplayName.ToString() + " Enjoy Simba Dev! :D");

            Launcher.Launch.LaunchDev(Program.FortnitePath, exchange);

            Logging.Logging.Log("Launching Simba Dev! Enjoy :D");

            return "Success";
        }

        private void CheckForUpdates()
        {
            Updates.UpdateChecker updateChecker = new Updates.UpdateChecker();
            updateChecker.CheckForUpdates().GetAwaiter().GetResult();
        }
    }
}
