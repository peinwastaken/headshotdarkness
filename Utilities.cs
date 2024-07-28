using Comfort.Common;
using EFT;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeadshotDarkness
{
    public static class Util
    {
        public static Player GetLocalPlayer()
        {
            GameWorld world = Singleton<GameWorld>.Instance;
            if (world == null || world.MainPlayer == null)
            {
                return null;
            }

            return world.MainPlayer;
        }

        public static bool ShouldDeathFade(EBodyPart lastBodyPart, EDamageType damageType)
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

        public static string GetDeathString(EBodyPart lastBodyPart, EDamageType lastDamageType)
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
    }
}
