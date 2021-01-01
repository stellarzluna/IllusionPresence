using System;
using KKAPI.Studio;
using UnityEngine.SceneManagement;

namespace HS2IllusionPresence
{
    public class StudioActivity
    {
        public void Initialize()
        {
            StudioAPI.StudioLoadedChanged += WhenStudioLoaded;
        }

        private void WhenStudioLoaded(object sender, EventArgs e)
        {
            MainGameActivity.ActivityStructure.Details = "In Studio";
            MainGameActivity.ActivityStructure.State = DiscordInstance.DefaultStruct.State;
            MainGameActivity.ActivityStructure.Timestamps.Start = Utility.GetTimestamp(DateTime.UtcNow);
            DiscordInstance.ChangePresence();
        }
    }
}
