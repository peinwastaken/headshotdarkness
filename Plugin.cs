using BepInEx;
using UnityEngine;
using HeadshotDarkness.patches;
using BepInEx.Configuration;

namespace HeadshotDarkness
{
    [BepInPlugin("com.pein.headshotdarkness", "HeadshotDarkness", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> Enabled { get; set; }
        public static ConfigEntry<bool> DebugMode { get; set; }
        public GameObject audioController { get; set; }

        private void DoConfig()
        {
            Enabled = Config.Bind("General", "Enabled", true, "Enable/disable headshot darkness");
            DebugMode = Config.Bind("General", "AlwaysDoDarkness", false, "Was only here to help me debug, if you for some reason want the headshot effect to always occur then enable it");
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
        }
    }
}
