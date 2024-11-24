using EFT;
using System.Reflection;
using System;
using SPT.Reflection.Patching;
using HeadshotDarkness.Helpers;
using HeadshotDarkness.Enums;

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

                if (darknessType == EDarknessType.ScreenFlash)
                {
                    ScreenFlash.StartFlash();
                }

                PlayUISoundPatch.SkipSound = true;
                __instance.StartCoroutine(VolumeAdjuster.FadeVolume(0f, Plugin.AudioFadeTime.Value));
            }
            else
            {
                return true;
            }

            if (Plugin.DeathTextEnabled.Value == true)
            {
                string deathText = Util.GetDeathString(lastBodyPart, lastDamageType);
                DeathTextHelper.CreateDeathText(deathText, Plugin.DeathTextFontSize.Value, Plugin.DeathTextLifeTime.Value, Plugin.DeathTextFadeInTime.Value, Plugin.DeathTextFadeOutTime.Value, Plugin.DeathTextFadeDelayTime.Value);
            }

            return false;
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
            __instance.StartCoroutine(VolumeAdjuster.FadeVolume(1f, Plugin.AudioFadeTime.Value));
        }
    }
}
