using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace HeadshotDarkness
{
    public enum EDeathString
    {
        Generic = 0,
        Headshot = 1,
        Explosion = 2,
    }

    public class JsonHelper
    {
        public class DeathStrings
        {
            public List<string> genericDeathStrings;
            public List<string> headshotDeathStrings;
            public List<string> explosionDeathStrings;
        }

        private static DeathStrings deathStrings;

        public static void Initialize()
        {
            LoadDeathStrings();
        }

        public static void LoadDeathStrings()
        {
            string dllPath = Assembly.GetExecutingAssembly().Location;
            string jsonPath = Path.Combine(Path.GetDirectoryName(dllPath), "deathstrings.json");

            if (File.Exists(jsonPath))
            {
                string jsonContent = File.ReadAllText(jsonPath);
                deathStrings = JsonConvert.DeserializeObject<DeathStrings>(jsonContent);
            }
        }

        public static List<string> GetDeathStrings(EDeathString deathString)
        {
            switch (deathString)
            {
                case EDeathString.Generic:
                    return deathStrings.genericDeathStrings;
                case EDeathString.Headshot:
                    return deathStrings.headshotDeathStrings;
                case EDeathString.Explosion:
                    return deathStrings.explosionDeathStrings;
            }

            return null;
        }
    }
}
