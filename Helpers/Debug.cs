using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadshotDarkness.Helpers
{
    public static class PluginDebug
    {
        public static ManualLogSource Logger { get; set; }

        public static void CreateLogger(ManualLogSource logSource)
        {
            Logger = logSource;
        }

        public static void LogInfo(string message)
        {
            if (!Plugin.Debug.Value) return;
            Logger?.LogInfo(message);
        }

        public static void LogWarning(string message)
        {
            if (!Plugin.Debug.Value) return;
            Logger?.LogWarning(message);
        }

        public static void LogError(string message)
        {
            if (!Plugin.Debug.Value) return;
            Logger?.LogError(message);
        }
    }
}
