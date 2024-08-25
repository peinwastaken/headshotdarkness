using EFT.UI;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeadshotDarkness.Patches
{
    public class PlayUISoundPatch : ModulePatch
    {
        public static bool SkipSound = false;

        protected override MethodBase GetTargetMethod()
        {
            return typeof(GUISounds).GetMethod(nameof(GUISounds.PlayUISound));
        }

        [PatchPrefix]
        private static bool PatchPreFix(EUISoundType soundType)
        {
            bool enabled = Plugin.Enabled.Value;
            if (enabled && soundType == EUISoundType.PlayerIsDead)
            {
                if (SkipSound == true)
                {
                    SkipSound = false;
                    return false;
                };
            }

            return true;
        }
    }
}
