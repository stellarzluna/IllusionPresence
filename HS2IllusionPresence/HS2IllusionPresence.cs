using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using KKAPI.Studio;

namespace HS2IllusionPresence
{

    [BepInPlugin(GUID, PluginName, Version)]
    [BepInProcess("HoneySelect2")]
    [BepInProcess("HoneySelect2VR")]
    [BepInProcess("StudioNEOV2")]
    public class HS2IllusionPresence : BaseUnityPlugin
    {
        public const string GUID = "com.skymunn.illusionpresence.hs2";
        public const string Version = "1.1.1";
        public const string PluginName = "IllusionPresence";

        public static HS2IllusionPresence Instance;
        public static ManualLogSource Log = new ManualLogSource(PluginName);
        public static Discord.Discord Discord;

        public static ConfigList PluginConfig;
        public static long TimeStart;

        private void Awake()
        {
            Instance = this;
            BepInEx.Logging.Logger.Sources.Add(Log);
            DiscordInstance.Initialize();
            LoadConfig();

            new MainGameActivity().Initialize();
        }

        private void LoadConfig()
        {
            PluginConfig.TimestampInHScene = Config.Bind("Global",
                "TimestampInHScene",
                false,
                "Set true if elapsed time only in H Scene");
            PluginConfig.Enable = Config.Bind("Global",
                "Enable",
                true,
                "Enable Discord Presence");
        }

        public struct ConfigList
        {
            public ConfigEntry<bool> TimestampInHScene;
            public ConfigEntry<bool> Enable;
        }
    }
}
