using EFT;
using System.Reflection;
using Comfort.Common;
using System;
using UnityEngine;
using SPT.Reflection.Patching;
using BepInEx;
using EFT.UI;

namespace HeadshotDarkness.patches
{
    public class BeginDeathScreenPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(DeathFade).GetMethod(nameof(DeathFade.EnableEffect));
        }

        [PatchPrefix]
        private static void PatchPreFix(DeathFade __instance)
        {
            if (Plugin.Enabled.Value == false)
            {
                return;
            }

            Player player = Util.GetLocalPlayer();
            Type deathType = typeof(DeathFade);
            AnimationCurve _enableCurve = Plugin.enableCurve;
            EBodyPart lastBodyPart = player.LastDamagedBodyPart;
            EDamageType lastDamageType = player.LastDamageType;

            if (Util.ShouldDeathFade(lastBodyPart, lastDamageType))
            {
                string deathText = Util.GetDeathString(lastBodyPart, lastDamageType);

                if (Plugin.UseAlternateDeathFade.Value == true)
                {
                    // set instant curve
                    deathType.GetField("_enableCurve", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, Plugin.instantCurve);
                    deathType.GetField("animationCurve_0", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, Plugin.instantCurve);

                    // set some values...
                    deathType.GetField("_closeEyesValue", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, 1f);
                    deathType.GetField("_fadeValue", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, 1f);
                    deathType.GetField("bool_0", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, false);
                    
                    // AHHH!!!
                    ScreenFlash.StartFlash(0.1f, false);
                }
                else
                {
                    deathType.GetField("_enableCurve", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, _enableCurve);
                    deathType.GetField("animationCurve_0", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, _enableCurve);
                    deathType.GetField("bool_0", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, false);
                }

                if (Plugin.DeathTextEnabled.Value == true)
                {
                    DeathTextHelper.CreateDeathText(deathText, Plugin.DeathTextFontSize.Value, Plugin.DeathTextLifeTime.Value, Plugin.DeathTextFadeInTime.Value, Plugin.DeathTextFadeOutTime.Value, Plugin.DeathTextFadeDelayTime.Value);
                }

                if (VolumeAdjuster.Instance == null)
                {
                    GameObject adjuster = new GameObject("VolumeAdjuster");
                    adjuster.GetOrAddComponent<VolumeAdjuster>();
                }

                VolumeAdjuster.Instance.StartVolumeAdjust(0f, Plugin.AudioFadeTime.Value);
            }
        }
    }

    public class EndDeathScreenPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(DeathFade).GetMethod(nameof(DeathFade.DisableEffect));
        }

        [PatchPostfix]
        private static void PatchPostFix(DeathFade __instance)
        {
            if (VolumeAdjuster.Instance == null)
            {
                GameObject adjuster = new GameObject("VolumeAdjuster");
                adjuster.GetOrAddComponent<VolumeAdjuster>();
            }

            VolumeAdjuster.Instance.StartVolumeAdjust(1f, 0.1f);
        }
    }

    public class PlayUiSoundPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GUISounds).GetMethod(nameof(GUISounds.PlayUISound));
        }

        [PatchPrefix]
        private static bool PatchPreFix(EUISoundType soundType)
        {
            Player player = Util.GetLocalPlayer();
            bool enabled = Plugin.Enabled.Value;

            if (enabled && soundType == EUISoundType.PlayerIsDead)
            {
                EBodyPart lastBodyPart = player.LastDamagedBodyPart;
                EDamageType lastDamageType = player.LastDamageType;

                if (Util.ShouldDeathFade(lastBodyPart, lastDamageType))
                {
                    Logger.LogInfo("Headshot Darkness is enabled, skipping UI death sound");
                    return false;
                };
            }
            
            return true;
        }
    }
}
