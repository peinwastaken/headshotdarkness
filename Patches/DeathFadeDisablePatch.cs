using HeadshotDarkness.Components;
using SPT.Reflection.Patching;
using System.Reflection;

namespace HeadshotDarkness.Patches
{
    public class DeathFadeDisablePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(DeathFade).GetMethod(nameof(DeathFade.DisableEffect));
        }

        [PatchPostfix]
        private static void PatchPostfix(DeathFade __instance)
        {
            VolumeAdjuster.Instance.FadeVolume(1f, Plugin.AudioFadeTime.Value);
        }
    }
}
