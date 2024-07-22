﻿using EFT;
using System.Reflection;
using Comfort.Common;
using System;
using UnityEngine;
using SPT.Reflection.Patching;

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

            if (Plugin.DebugMode.Value == true | player.LastDamagedBodyPart == EBodyPart.Head)
            {
                deathType.GetField("_enableCurve", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, _enableCurve);
                deathType.GetField("animationCurve_0", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, _enableCurve);
                deathType.GetField("bool_0", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, true);

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
