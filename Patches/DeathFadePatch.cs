using EFT;
using System.Reflection;
using System;
using SPT.Reflection.Patching;
using EFT.UI;
using HeadshotDarkness.Helpers;
using HeadshotDarkness.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HeadshotDarkness.Patches
{
    public class BeginDeathScreenPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(DeathFade).GetMethod(nameof(DeathFade.EnableEffect));
        }

        [PatchPrefix]
        private static bool PatchPrefix(DeathFade __instance)
        {
            if (Plugin.Enabled.Value == false)
            {
                return true;
            }

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
}
