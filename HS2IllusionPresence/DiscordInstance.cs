using System;

namespace HS2IllusionPresence
{
    public class DiscordInstance
    {
        public static Discord.Discord Instance;
        public static Discord.ActivityManager ActivityManager;
        public static Discord.Activity DefaultStruct;

        public static void Initialize()
        {
            Instance = new Discord.Discord(791235568683581472, (ulong)Discord.CreateFlags.Default);
            ActivityManager = Instance.GetActivityManager();

            DefaultStruct = new Discord.Activity
            {
                Assets =
                {
                    LargeImage = "hs2_largeimagekey",
                    LargeText = HS2IllusionPresence.PluginName + " | " + HS2IllusionPresence.Version
                }
            };
        }

        public static void ChangePresence()
        {
            bool loop = true;

            if (HS2IllusionPresence.PluginConfig.Enable.Value)
            {
                ActivityManager.UpdateActivity(MainGameActivity.ActivityStructure, (res) =>
                {
                    HS2IllusionPresence.Log.LogInfo("Presence updated!");
                    loop = false;
                });
                while (loop) Instance.RunCallbacks();
            }
        }
    }
}
