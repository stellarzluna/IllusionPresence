using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HS2IllusionPresence
{
    public class ConfigEvent
    {
        public void InitializeAllEvents()
        {
            HS2IllusionPresence.PluginConfig.Enable.SettingChanged += Enable_SettingChanged;
            HS2IllusionPresence.PluginConfig.TimestampInHScene.SettingChanged += TimestampInHScene_SettingChanged;
        }

        private void TimestampInHScene_SettingChanged(object sender, EventArgs e)
        {
            if (!KKAPI.Studio.StudioAPI.InsideStudio)
            {
                if (HS2IllusionPresence.PluginConfig.TimestampInHScene.Value)
                {
                    if (!MainGameActivity.InHScene)
                        MainGameActivity.ActivityStructure.Timestamps.Start = DiscordInstance.DefaultStruct.Timestamps.Start;
                }
                else
                {
                    MainGameActivity.ActivityStructure.Timestamps.Start = Utility.GetTimestamp(DateTime.UtcNow);
                }

                DiscordInstance.ChangePresence();
            }
        }

        private void Enable_SettingChanged(object sender, EventArgs e)
        {
            if (HS2IllusionPresence.PluginConfig.Enable.Value)
            {
                DiscordInstance.Initialize();
                DiscordInstance.ChangePresence();
            }
            else
            {
                DiscordInstance.Instance.Dispose();
                DiscordInstance.Instance = null;
            }
        }
    }
}
