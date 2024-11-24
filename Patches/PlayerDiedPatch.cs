using EFT;
using SPT.Reflection.Patching;
using System.Reflection;

namespace HeadshotDarkness.Patches
{
    public class PlayerDiedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod(nameof(Player.OnDead));
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance)
        {
            BeginDeathScreenPatch.SetShouldDoDarkness(true);
        }
    }
}
