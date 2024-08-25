using Comfort.Common;
using EFT;
using System.Collections.Generic;
using HeadshotDarkness.Enums;

namespace HeadshotDarkness.Helpers
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

        public static EDarknessType GetDeathFadeType(EBodyPart lastBodyPart, EDamageType lastDamageType)
        {
            // if headshot
            if (lastBodyPart == EBodyPart.Head)
            {
                return Plugin.DarknessTypeHeadshot.Value;
            }

            // if explosion
            if (lastDamageType == EDamageType.Explosion || lastDamageType == EDamageType.GrenadeFragment || lastDamageType == EDamageType.Landmine) 
            {
                return Plugin.DarknessTypeExplosion.Value;
            }

            // if... something... else?
            return Plugin.DarknessTypeGeneric.Value;
        }

        public static string GetDeathString(EBodyPart lastBodyPart, EDamageType lastDamageType)
        {
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
