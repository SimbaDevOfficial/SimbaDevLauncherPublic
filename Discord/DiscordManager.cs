using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimbaDevPublicLauncher.Discord
{
    internal class DiscordManager
    {
        public async Task InitiateDiscord()
        {
            if (!string.IsNullOrEmpty(Program.DiscordUrl) && ShouldOpenDiscord())
            {
                Process.Start(Program.DiscordUrl);
                UpdateLastOpenedDiscordTime();
            }
        }

        private bool ShouldOpenDiscord()
        {
            if (File.Exists(Program.LastOpenedDiscordTimeFile))
            {
                var lastOpenedTime = File.ReadAllText(Program.LastOpenedDiscordTimeFile);

                if (DateTime.TryParse(lastOpenedTime, out var lastOpenedDateTime))
                {
                    var timeSinceLastOpened = DateTime.Now - lastOpenedDateTime;
                    return timeSinceLastOpened.TotalHours >= 1;
                }
            }

            return true;
        }

        private void UpdateLastOpenedDiscordTime()
        {
            File.WriteAllText(Program.LastOpenedDiscordTimeFile, DateTime.Now.ToString());
        }
    }
}
