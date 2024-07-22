using BepInEx;
using UnityEngine;
using HeadshotDarkness.patches;
using BepInEx.Configuration;
using System;

namespace HeadshotDarkness
{
    [BepInPlugin("com.pein.headshotdarkness", "HeadshotDarkness", "1.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> Enabled { get; set; }
        public static ConfigEntry<bool> DebugMode { get; set; }

        public static ConfigEntry<float> ScreenFadeTime { get; set; }
        public static ConfigEntry<float> AudioFadeTime { get; set; }

        public GameObject audioController { get; set; }
        public static AnimationCurve enableCurve { get; set; }

        private void UpdateEnableCurve(object sender, EventArgs args) // weird............
        {
            enableCurve.keys = new Keyframe[]
            {
                new Keyframe(0f, 0f),
                new Keyframe(ScreenFadeTime.Value, 2f)
            };
        }

        private void DoConfig()
        {
            // General
            Enabled = Config.Bind("General", "Enabled", true, new ConfigDescription(
                    "Enable/disable headshot darkness",
                    null,
                    new ConfigurationManagerAttributes { Order = 2 }
                ));

            DebugMode = Config.Bind("General", "AlwaysDoDarkness", false, new ConfigDescription(
                    "Was only here to help me debug, if you for some reason want the headshot effect to always occur then enable it",
                    null,
                    new ConfigurationManagerAttributes { Order = 1 }
                ));

            // Parameters
            ScreenFadeTime = Config.Bind("Parameters", "ScreenFadeTime", 0.05f, new ConfigDescription(
                    "Changes screen fade time",
                    new AcceptableValueRange<float>(0.01f, 1f),
                    new ConfigurationManagerAttributes { Order = 2 }
                ));

            AudioFadeTime = Config.Bind("Parameters", "AudioFadeTime", 0.3f, new ConfigDescription(
                    "Changes audio fade time",
                    new AcceptableValueRange<float>(0.01f, 1f),
                    new ConfigurationManagerAttributes { Order = 1 }
                ));

            ScreenFadeTime.SettingChanged += UpdateEnableCurve;
        }

        private void DoPatches()
        {
            new BeginDeathScreenPatch().Enable();
            new EndDeathScreenPatch().Enable();
        }

        private void Awake()
        {
            DoConfig();
            DoPatches();

            enableCurve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(ScreenFadeTime.Value, 2f)
            );
        }
    }
}
