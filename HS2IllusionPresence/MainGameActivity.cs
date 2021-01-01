using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using KKAPI.Studio;
using UnityEngine.SceneManagement;

namespace HS2IllusionPresence
{
    public class MainGameActivity
    {
        public static HScene Instance;
        public static bool IsPatched;
        public static Harmony HarmonyGame;
        private static SPlayerList PlayerList;

        public static Discord.Activity ActivityStructure;
        private static bool FirstTimeLaunch = true;
        private static HSceneState LoadedScene = HSceneState.Single;
        public static bool InHScene = false;

        public void Initialize()
        {
            ActivityStructure = DiscordInstance.DefaultStruct;
            ActivityStructure.State = "Loading...";
            DiscordInstance.ChangePresence();

            if (StudioAPI.InsideStudio)
            {
                new StudioActivity().Initialize();
            }
            else
            {
                SceneManager.sceneLoaded += WhenSceneLoaded;
                HarmonyGame = new Harmony(HS2IllusionPresence.PluginName);
                new ConfigEvent().InitializeAllEvents();
            }
        }

        private void WhenSceneLoaded(Scene scene, LoadSceneMode LSM)
        {
            if (LSM != LoadSceneMode.Single) return;
            HS2IllusionPresence.Log.LogInfo("Loaded Scene: " + scene.name);

            switch (scene.name)
            {
                case "Title":
                    if (FirstTimeLaunch)
                    {
                        bool timestampHScene = HS2IllusionPresence.PluginConfig.TimestampInHScene.Value;
                        if (!timestampHScene) ActivityStructure.Timestamps.Start = Utility.GetTimestamp(DateTime.UtcNow);
                        FirstTimeLaunch = false;
                    }
                    ActivityStructure.Details = "Lobby";
                    ActivityStructure.State = "In title";
                    break;

                case "CharaCustom":
                    ActivityStructure.Details = "Chara Custom";
                    ActivityStructure.State = "Making nice character";
                    break;

                case "Home":
                    ActivityStructure.Details = "Lobby";
                    ActivityStructure.State = "In player room";
                    break;

                case "SpecialTreatmentRoom":
                    ActivityStructure.Details = "Lobby | DX Plan";
                    ActivityStructure.State = "In VIP lobby";
                    break;

                case "LobbyScene":
                    ActivityStructure.Details = "Lobby";
                    ActivityStructure.State = "Calling girl";
                    break;

                case "HScene":
                    HarmonyGame.PatchAll(typeof(MainGameActivity));
                    IsPatched = true;
                    HS2IllusionPresence.Log.LogInfo("Patch successfully!");
                    break;

                default:
                    if (IsPatched)
                    {
                        HarmonyGame.UnpatchAll(nameof(MainGameActivity));
                        IsPatched = false;
                        Instance = null;
                        InHScene = false;

                        PlayerList.Female = null;
                        PlayerList.Male = null;

                        if (HS2IllusionPresence.PluginConfig.TimestampInHScene.Value)
                        {
                            ActivityStructure.Timestamps.Start = DiscordInstance.DefaultStruct.Timestamps.Start;
                        }

                        HS2IllusionPresence.Log.LogInfo("Unpatch successfully!");
                    }
                    break;
            }
            DiscordInstance.ChangePresence();
        }

        private static void ChangeStateHScene()
        {
            if (HS2IllusionPresence.PluginConfig.TimestampInHScene.Value && !InHScene)
            {
                InHScene = true;
                ActivityStructure.Timestamps.Start = Utility.GetTimestamp(DateTime.UtcNow);
            }

            ActivityStructure.Details = "H Scene | ";
            switch (LoadedScene)
            {
                case HSceneState.Lesbian:
                    ActivityStructure.Details += "2P Girls";
                    ActivityStructure.State = string.Format("Watching {0} and {1} play along",
                        PlayerList.Female[0].fileParam.fullname,
                        PlayerList.Female[1].fileParam.fullname);
                    DiscordInstance.ChangePresence();
                    break;
                case HSceneState.SoloWoman:
                    ActivityStructure.Details += "1P Girl";
                    ActivityStructure.State = string.Format("Watching {0} play along",
                        PlayerList.Female.First().fileParam.fullname);
                    DiscordInstance.ChangePresence();
                    break;
                case HSceneState.ManThreesome:
                    ActivityStructure.Details += "3P Man";
                    ActivityStructure.State = string.Format("Dating with {0}",
                        PlayerList.Female.First().fileParam.fullname);
                    DiscordInstance.ChangePresence();
                    break;
                case HSceneState.WomanThreesome:
                    ActivityStructure.Details += "3P Woman";
                    ActivityStructure.State = string.Format("Dating with {0} and {1}",
                        PlayerList.Female[0].fileParam.fullname,
                        PlayerList.Female[1].fileParam.fullname);
                    DiscordInstance.ChangePresence();
                    break;
                case HSceneState.Single:
                    ActivityStructure.Details += "Dating";
                    ActivityStructure.State = string.Format("Dating with {0}",
                        PlayerList.Female.First().fileParam.fullname);
                    DiscordInstance.ChangePresence();
                    break;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HScene), "SetStartVoice")]
        public static void HFlagRaised(HScene __instance)
        {
            HS2IllusionPresence.Log.LogInfo("HFlag | Voice raised!");
            Instance = __instance;

            PlayerList.Female = new List<AIChara.ChaControl>(Instance.GetFemales().Where(chara => chara != null).ToArray());
            PlayerList.Male = new List<AIChara.ChaControl>(Instance.GetMales().Where(chara => chara != null).ToArray());

            ChangeStateHScene();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HScene), "ChangeAnimation")]
        public static void DetectScene(HScene.AnimationListInfo _info)
        {
            HS2IllusionPresence.Log.LogInfo("HFlag | Position changed!");
            bool manThreesome = _info.fileMale != "" && _info.fileMale2 != "" && _info.fileFemale != "" && _info.fileFemale2 == "";
            bool womanThreesome = _info.fileMale != "" && _info.fileMale2 == "" && _info.fileFemale != "" && _info.fileFemale2 != "";
            bool soloWoman = _info.fileMale != "" && _info.fileMale2 != "" && _info.fileFemale == "" && _info.fileFemale2 != "";
            bool lesbian = _info.fileMale != "" && _info.fileMale2 != "" && _info.fileFemale == "" && _info.fileFemale2 == "";

            LoadedScene = soloWoman ? HSceneState.SoloWoman
                : lesbian ? HSceneState.Lesbian
                : manThreesome ? HSceneState.ManThreesome
                : womanThreesome ? HSceneState.WomanThreesome
                : HSceneState.Single;
            HS2IllusionPresence.Log.LogInfo("Detected H Scene: " + LoadedScene.ToString());

            if (Instance == null) return;
            ChangeStateHScene();
        }

        public struct SPlayerList
        {
            public List<AIChara.ChaControl> Female;
            public List<AIChara.ChaControl> Male;
        }

        public enum HSceneState
        {
            ManThreesome,
            WomanThreesome,
            SoloWoman,
            Lesbian,
            Single
        }
    }
}
