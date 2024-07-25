using EFT;
using System.Reflection;
using Comfort.Common;
using System;
using UnityEngine;
using SPT.Reflection.Patching;
using BepInEx;
using System.Collections.Generic;

namespace HeadshotDarkness.patches
{
    public class BeginDeathScreenPatch : ModulePatch
    {
        public static float _previousVolume { get; private set; }

        static Player GetLocalPlayer()
        {
            GameWorld world = Singleton<GameWorld>.Instance;
            if (world == null || world.MainPlayer == null)
            {
                return null;
            }

            return world.MainPlayer;
        }

        private static bool ShouldDeathFade(EBodyPart lastBodyPart, EDamageType damageType)
        {
            if (Plugin.ExplosionsDoDarkness.Value == true && (damageType == EDamageType.Explosion || damageType == EDamageType.Landmine || damageType == EDamageType.GrenadeFragment))
            {
                return true;
            }

            if (Plugin.DebugMode.Value == true)
            {
                return true;
            }

            if (lastBodyPart == EBodyPart.Head)
            {
                return true;
            }

            return false;
        }

        private static string GetDeathString(EBodyPart lastBodyPart, EDamageType lastDamageType)
        {
            bool debug = Plugin.DebugMode.Value;
            EDeathString stringEnum;
            if (Plugin.DeathTextContextual.Value)
            {
                // explosion
                if (lastDamageType == EDamageType.Explosion || lastDamageType == EDamageType.GrenadeFragment || lastDamageType == EDamageType.Landmine)
                {
                    stringEnum = EDeathString.Explosion;
                }
                // headshot
                else if (lastBodyPart == EBodyPart.Head)
                {
                    stringEnum = EDeathString.Headshot;
                }
                // AlwaysDoDarkness and not headshot or explosion
                else
                {
                    stringEnum = EDeathString.Generic;
                }
            }
            else
            {
                return Plugin.DeathTextString.Value;
            }

            List<string> list = JsonHelper.GetDeathStrings(stringEnum);
            return list.GetRandomItem();
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(DeathFade).GetMethod(nameof(DeathFade.EnableEffect));
        }

        [PatchPrefix]
        private static void PatchPostFix(DeathFade __instance)
        {
            if (Plugin.Enabled.Value == false)
            {
                return;
            }

            Player player = GetLocalPlayer();
            Type deathType = typeof(DeathFade);
            AnimationCurve _enableCurve = Plugin.enableCurve;
            EBodyPart lastBodyPart = player.LastDamagedBodyPart;
            EDamageType lastDamageType = player.LastDamageType;

            if (ShouldDeathFade(lastBodyPart, lastDamageType))
            {
                string deathText = GetDeathString(lastBodyPart, lastDamageType);

                if (Plugin.UseAlternateDeathFade.Value == true)
                {
                    deathType.GetField("_closeEyesValue", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, 1f);
                    deathType.GetField("_fadeValue", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, 1f);
                    ScreenFlash.StartFlash(0.1f, false);
                }

                if (Plugin.DeathTextEnabled.Value == true)
                {
                    DeathTextHelper.CreateDeathText(deathText, Plugin.DeathTextFontSize.Value, Plugin.DeathTextLifeTime.Value, Plugin.DeathTextFadeInTime.Value, Plugin.DeathTextFadeOutTime.Value, Plugin.DeathTextFadeDelayTime.Value);
                }

                deathType.GetField("_enableCurve", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, _enableCurve);
                deathType.GetField("animationCurve_0", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, _enableCurve);
                deathType.GetField("bool_0", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, false);

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
}
