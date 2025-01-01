using EFT;
using System.Reflection;
using System;
using SPT.Reflection.Patching;
using HeadshotDarkness.Helpers;
using HeadshotDarkness.Enums;
using Comfort.Common;

namespace HeadshotDarkness.Patches
{
    public class BeginDeathScreenPatch : ModulePatch
    {
        private static bool _shouldDoDarkness = false;

        protected override MethodBase GetTargetMethod()
        {
            return typeof(DeathFade).GetMethod(nameof(DeathFade.EnableEffect));
        }

        public static void SetShouldDoDarkness(bool state)
        {
            PluginDebug.LogInfo($"setting _shouldDoDarkness to {state.ToString()}");
            _shouldDoDarkness = state;
        }

        [PatchPrefix]
        private static bool PatchPrefix(DeathFade __instance)
        {
            if (_shouldDoDarkness == false)
            {
                return true;
            }

            SetShouldDoDarkness(false);

            PluginDebug.LogInfo("doing darkness");

            if (Plugin.DisableUIDeathSound.Value == true)
            {
                PlayUISoundPatch.SkipSound = true;
            }

            Type deathType = typeof(DeathFade);
            Player player = Util.GetLocalPlayer();
            EBodyPart lastBodyPart = player.LastDamagedBodyPart;
            EDamageType lastDamageType = player.LastDamageType;
            EDarknessType darknessType = Util.GetDeathFadeType(lastBodyPart, lastDamageType);

            if (darknessType != EDarknessType.Vanilla)
            {
                deathType.GetField("animationCurve_0", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, Curves.EnableCurve);
                deathType.GetField("bool_0", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, true);
                deathType.GetField("float_3", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, 3f);

                if (darknessType == EDarknessType.ScreenFlash)
                {
                    ScreenFlashManager.Instance?.DoScreenFlash(0.05f);
                }

                if (Plugin.DeathTextEnabled.Value == true)
                {
                    string deathText = Util.GetDeathString(lastBodyPart, lastDamageType);
                    DeathTextManager.Instance?.DoDeathText(
                        deathText,
                        Plugin.DeathTextFontSize.Value,
                        Plugin.DeathTextLifeTime.Value,
                        Plugin.DeathTextFadeInTime.Value,
                        Plugin.DeathTextFadeOutTime.Value,
                        Plugin.DeathTextFadeDelayTime.Value
                    );
                }

                PlayUISoundPatch.SkipSound = true;
                VolumeAdjuster.Instance?.FadeVolume(0f, Plugin.AudioFadeTime.Value);

                return false;
            }

            return true;
        }
    }

    public class EndDeathScreenPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(DeathFade).GetMethod(nameof(DeathFade.DisableEffect));
        }

        [PatchPostfix]
        private static void PatchPostfix(DeathFade __instance)
        {
            VolumeAdjuster.Instance?.FadeVolume(1f, Plugin.AudioFadeTime.Value);
        }
    }
}
