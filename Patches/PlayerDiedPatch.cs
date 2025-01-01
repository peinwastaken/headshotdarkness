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

        [PatchPrefix]
        private static bool PatchPostfix(Player __instance)
        {
            if (__instance.IsYourPlayer) // lol
            {
                BeginDeathScreenPatch.SetShouldDoDarkness(true);
            }

            return true; // lol
        }
    }
}