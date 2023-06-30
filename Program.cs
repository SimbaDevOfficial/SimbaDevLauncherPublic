using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimbaDevPublicLauncher
{
    internal class Program
    {
        public const string UpdateApiUrl = "https://raw.githubusercontent.com/DRQSuperior/simba-api/main/update.json";
        public const string LastOpenedDiscordTimeFile = "other.simba";
        public const string CurrentVersion = "3.0";

        public static string FortnitePath { get; set; }
        public static string DiscordUrl { get; set; }
        public static string Version { get; set; }
        public static string Response { get; set; }
        public static string[] Args { get; set; }

        private static async Task Main(string[] args)
        {
            Args = args;
            Launcher.Launcher launcher = new Launcher.Launcher();
            await launcher.Run();
        }
    }
}
