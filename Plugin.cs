using BepInEx;
using UnityEngine;
using HeadshotDarkness.patches;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using HarmonyLib;

namespace HeadshotDarkness
{
    [BepInPlugin("com.pein.headshotdarkness", "HeadshotDarkness", "1.0.5")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> Enabled { get; set; }
        public static ConfigEntry<bool> DebugMode { get; set; }

        public static ConfigEntry<float> ScreenFadeTime { get; set; }
        public static ConfigEntry<bool> UseAlternateDeathFade { get; set; }
        public static ConfigEntry<bool> ExplosionsDoDarkness { get; set; }
        public static ConfigEntry<float> AudioFadeTime { get; set; }
        public static ConfigEntry<bool> DisableUIDeathSound { get; set; }

        public static ConfigEntry<bool> DeathTextEnabled { get; set; }
        public static ConfigEntry<string> DeathTextString { get; set; }
        public static ConfigEntry<bool> DeathTextContextual { get; set; }
        public static ConfigEntry<int> DeathTextFontSize { get; set; }
        public static ConfigEntry<float> DeathTextLifeTime { get; set; }
        public static ConfigEntry<float> DeathTextFadeInTime { get; set; }
        public static ConfigEntry<float> DeathTextFadeOutTime { get; set; }
        public static ConfigEntry<float> DeathTextFadeDelayTime { get; set; }

        public static AnimationCurve enableCurve { get; set; }
        public static AnimationCurve instantCurve { get; set; }

        private void UpdateEnableCurve(object sender, EventArgs args) // weird............
        {
            enableCurve.keys = new Keyframe[]
            {
                new Keyframe(0f, 0f),
                new Keyframe(ScreenFadeTime.Value, 2f)
            };

            instantCurve.keys = new Keyframe[]
            {
                new Keyframe(0f, 0f),
                new Keyframe(0.001f, 2f)
            };
        }

        private void DoConfig()
        {
            string general = "1. General";
            string parameters = "2. Parameters";
            string deathText = "3. DeathText";

            // General
            Enabled = Config.Bind(general, "Enabled", true, new ConfigDescription(
                    "Enable/disable headshot darkness",
                    null,
                    new ConfigurationManagerAttributes { Order = 1000 }
                ));

            UseAlternateDeathFade = Config.Bind(general, "DoScreenFlash", false, new ConfigDescription(
                    "Lights out. Flashes white for a fraction of a second and instantly switches to black. Disables ScreenFadeTime",
                    null,
                    new ConfigurationManagerAttributes { Order = 999 }
                ));

            ExplosionsDoDarkness = Config.Bind(general, "ExplosionsDoDarkness", false, new ConfigDescription(
                    "Set if explosions should cause the headshot effect.",
                    null,
                    new ConfigurationManagerAttributes { Order = 998 }
                ));

            DebugMode = Config.Bind(general, "AlwaysDoDarkness", false, new ConfigDescription(
                    "Was only here to help me debug, if you for some reason want the headshot effect to always occur then enable it.",
                    null,
                    new ConfigurationManagerAttributes { Order = 990 }
                ));

            DisableUIDeathSound = Config.Bind(general, "DisableUIDeathSound", false, new ConfigDescription(
                    "Prevents the UI death sound from ever playing. It will always be disabled if the headshot effect occurs but this lets you disable it for bodyshot deaths.",
                    null,
                    new ConfigurationManagerAttributes { Order = 989 }
                ));

            // Parameters
            ScreenFadeTime = Config.Bind(parameters, "ScreenFadeTime", 0.05f, new ConfigDescription(
                    "Changes screen fade time",
                    new AcceptableValueRange<float>(0.01f, 1f),
                    new ConfigurationManagerAttributes { Order = 980 }
                ));

            AudioFadeTime = Config.Bind(parameters, "AudioFadeTime", 0.3f, new ConfigDescription(
                    "Changes audio fade time",
                    new AcceptableValueRange<float>(0.01f, 1f),
                    new ConfigurationManagerAttributes { Order = 970 }
                ));

            // DeathText Parameters
            DeathTextEnabled = Config.Bind(deathText, "DeathTextEnabled", false, new ConfigDescription(
                    "Enable/disable death text",
                    null,
                    new ConfigurationManagerAttributes { Order = 960 }
                ));

            DeathTextString = Config.Bind(deathText, "DeathTextString", "You are dead.", new ConfigDescription(
                    "Set the headshot text string",
                    null,
                    new ConfigurationManagerAttributes { Order = 950 }
                ));

            DeathTextContextual = Config.Bind(deathText, "DeathTextContextual", false, new ConfigDescription(
                    "Set if death texts should be contextual. Pulls random death strings from deathstrings.json depending on how you died instead of using the DeathTextString variable",
                    null,
                    new ConfigurationManagerAttributes { Order = 949 }
                ));

            DeathTextFontSize = Config.Bind(deathText, "DeathTextFontSize", 24, new ConfigDescription(
                    "Set the headshot text font size",
                    new AcceptableValueRange<int>(1, 64),
                    new ConfigurationManagerAttributes { Order = 930 }
                ));

            DeathTextLifeTime = Config.Bind(deathText, "DeathTextLifeTime", 3f, new ConfigDescription(
                    "Changes how long the death text is visible for",
                    new AcceptableValueRange<float>(0.01f, 5f),
                    new ConfigurationManagerAttributes { Order = 920 }
                ));

            DeathTextFadeInTime = Config.Bind(deathText, "DeathTextFadeInTime", 0.01f, new ConfigDescription(
                    "Changes how long it takes for the death text to fade in",
                    new AcceptableValueRange<float>(0.01f, 1f),
                    new ConfigurationManagerAttributes { Order = 910 }
                ));

            DeathTextFadeOutTime = Config.Bind(deathText, "DeathTextFadeOutTime", 0.5f, new ConfigDescription(
                    "Changes how long it takes for the death text to fade out",
                    new AcceptableValueRange<float>(0.01f, 1f),
                    new ConfigurationManagerAttributes { Order = 900 }
                ));

            DeathTextFadeDelayTime = Config.Bind(deathText, "DeathTextFadeDelayTime", 0.1f, new ConfigDescription(
                    "Changes how long showing death text is delayed for",
                    new AcceptableValueRange<float>(0.01f, 1f),
                    new ConfigurationManagerAttributes { Order = 890 }
                ));


            ScreenFadeTime.SettingChanged += UpdateEnableCurve;
        }

        private void DoPatches()
        {
            new BeginDeathScreenPatch().Enable();
            new EndDeathScreenPatch().Enable();
            new PlayUiSoundPatch().Enable();
        }

        private void Awake()
        {
            DoConfig();
            DoPatches();

            JsonHelper.Initialize();

            enableCurve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(ScreenFadeTime.Value, 2f)
            );
        }
    }
}
