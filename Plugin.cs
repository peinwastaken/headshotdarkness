using BepInEx;
using HeadshotDarkness.Patches;
using BepInEx.Configuration;
using HeadshotDarkness.Helpers;
using HeadshotDarkness.Enums;

namespace HeadshotDarkness
{
    [BepInPlugin("com.pein.headshotdarkness", "HeadshotDarkness", "1.1.2")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> Enabled { get; set; }
        public static ConfigEntry<bool> DisableUIDeathSound { get; set; }

        public static ConfigEntry<float> AudioFadeTime { get; set; }
        public static ConfigEntry<EDarknessType> DarknessTypeHeadshot { get; set; }
        public static ConfigEntry<EDarknessType> DarknessTypeExplosion { get; set; }
        public static ConfigEntry<EDarknessType> DarknessTypeGeneric { get; set; }

        public static ConfigEntry<bool> DeathTextEnabled { get; set; }
        public static ConfigEntry<string> DeathTextString { get; set; }
        public static ConfigEntry<bool> DeathTextContextual { get; set; }
        public static ConfigEntry<int> DeathTextFontSize { get; set; }
        public static ConfigEntry<float> DeathTextLifeTime { get; set; }
        public static ConfigEntry<float> DeathTextFadeInTime { get; set; }
        public static ConfigEntry<float> DeathTextFadeOutTime { get; set; }
        public static ConfigEntry<float> DeathTextFadeDelayTime { get; set; }
        public static ConfigEntry<bool> Debug { get; set; }

        private void DoConfig()
        {
            string general = "1. General";
            string deathFade = "2. Death Fade";
            string deathText = "3. Death Text";
            string debugText = "4. Debug";

            // General
            Enabled = Config.Bind(general, "Enabled", true, new ConfigDescription(
                    "Enable/disable headshot darkness",
                    null,
                    new ConfigurationManagerAttributes { Order = 1000 }
                ));

            DisableUIDeathSound = Config.Bind(general, "Disable UI Death Sound", false, new ConfigDescription(
                    "Prevents the UI death sound from ever playing. It will always be disabled if the headshot effect occurs but this lets you disable it for all deaths.",
                    null,
                    new ConfigurationManagerAttributes { Order = 990 }
                ));

            // Death Fade
            AudioFadeTime = Config.Bind(deathFade, "Audio Fade Time", 0.3f, new ConfigDescription(
                    "Changes audio fade time",
                    new AcceptableValueRange<float>(0.01f, 1f),
                    new ConfigurationManagerAttributes { Order = 970 }
                ));

            DarknessTypeHeadshot = Config.Bind(deathFade, "Headshot Fade Style", EDarknessType.Default, new ConfigDescription(
                    "Changes darkness style for headshot deaths.",
                    null,
                    new ConfigurationManagerAttributes { Order = 969 }
                ));

            DarknessTypeExplosion = Config.Bind(deathFade, "Explosion Fade Style", EDarknessType.Vanilla, new ConfigDescription(
                    "Changes darkness style for explosion deaths.",
                    null,
                    new ConfigurationManagerAttributes { Order = 968 }
                ));

            DarknessTypeGeneric = Config.Bind(deathFade, "Other Fade Style", EDarknessType.Vanilla, new ConfigDescription(
                    "Changes darkness style for other deaths. Mostly a debug feature but if for whatever reason you want the effect to occur after a bodyshot kill then here you go.",
                    null,
                    new ConfigurationManagerAttributes { Order = 967 }
                ));

            // Death Text
            DeathTextEnabled = Config.Bind(deathText, "Enable Death Text", false, new ConfigDescription(
                    "Enable/disable death text. Only shown when the headshot effect occurs",
                    null,
                    new ConfigurationManagerAttributes { Order = 960 }
                ));

            DeathTextString = Config.Bind(deathText, "Death Text String", "You are dead.", new ConfigDescription(
                    "Set the death text string",
                    null,
                    new ConfigurationManagerAttributes { Order = 950 }
                ));

            DeathTextContextual = Config.Bind(deathText, "Contextual Death Text", false, new ConfigDescription(
                    "Set if death texts should be contextual. Pulls random death strings from deathstrings.json depending on how you died instead of using the Death Text String value",
                    null,
                    new ConfigurationManagerAttributes { Order = 949 }
                ));

            DeathTextFontSize = Config.Bind(deathText, "Death Text Size", 24, new ConfigDescription(
                    "Set the death text font size",
                    new AcceptableValueRange<int>(1, 64),
                    new ConfigurationManagerAttributes { Order = 930 }
                ));

            DeathTextLifeTime = Config.Bind(deathText, "Death Text Lifetime", 3f, new ConfigDescription(
                    "Changes how long the death text is visible for",
                    new AcceptableValueRange<float>(0.01f, 5f),
                    new ConfigurationManagerAttributes { Order = 920 }
                ));

            DeathTextFadeInTime = Config.Bind(deathText, "Death Text Fade In Time", 0.01f, new ConfigDescription(
                    "Changes how long it takes for the death text to fade in",
                    new AcceptableValueRange<float>(0.01f, 1f),
                    new ConfigurationManagerAttributes { Order = 910 }
                ));

            DeathTextFadeOutTime = Config.Bind(deathText, "Death Text Fade Out Time", 0.5f, new ConfigDescription(
                    "Changes how long it takes for the death text to fade out",
                    new AcceptableValueRange<float>(0.01f, 1f),
                    new ConfigurationManagerAttributes { Order = 900 }
                ));

            DeathTextFadeDelayTime = Config.Bind(deathText, "Death Text Fade Delay Time", 0.01f, new ConfigDescription(
                    "Changes how long the death text is delayed for",
                    new AcceptableValueRange<float>(0.01f, 1f),
                    new ConfigurationManagerAttributes { Order = 890 }
                ));

            // Debug
            Debug = Config.Bind("Debug", debugText, true, new ConfigDescription(
                    "Enable/disable debug logging in BepInEx console.",
                    null,
                    new ConfigurationManagerAttributes { Order = 880 }
                ));
        }

        private void DoPatches()
        {
            new PlayerDiedPatch().Enable();
            new BeginDeathScreenPatch().Enable();
            new EndDeathScreenPatch().Enable();
            new PlayUISoundPatch().Enable();
        }

        private void DoGameObjects()
        {
            VolumeAdjuster.Create();
            DeathTextManager.Create();
            ScreenFlashManager.Create();
        }

        private void Awake()
        {
            PluginDebug.CreateLogger(Logger);

            DoConfig();
            DoPatches();
            DoGameObjects();

            JsonHelper.LoadDeathStrings();
        }
    }
}
