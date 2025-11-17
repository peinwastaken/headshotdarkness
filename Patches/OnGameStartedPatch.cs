using EFT;
using HeadshotDarkness.Components;
using SPT.Reflection.Patching;
using System.Reflection;

namespace HeadshotDarkness.Patches
{
    public class OnGameStartedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
        }

        [PatchPostfix]
        public static void PatchPostfix()
        {
            Plugin.CreateGameObjects();
            
            DarknessManager.Instance.OnGameStarted();
        }
    }
}
