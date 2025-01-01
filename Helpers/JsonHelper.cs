using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using HeadshotDarkness.Enums;

namespace HeadshotDarkness.Helpers
{
    public class JsonHelper
    {
        public class DeathStrings
        {
            public List<string> genericDeathStrings;
            public List<string> headshotDeathStrings;
            public List<string> explosionDeathStrings;
        }

        private static DeathStrings deathStrings;

        public static void LoadDeathStrings()
        {
            string dllPath = Assembly.GetExecutingAssembly().Location;
            string jsonPath = Path.Combine(Path.GetDirectoryName(dllPath), "deathstrings.json");

            if (File.Exists(jsonPath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(jsonPath);
                    deathStrings = JsonConvert.DeserializeObject<DeathStrings>(jsonContent);
                }
                catch
                {
                    PluginDebug.LogError("Failed to load deathstrings.json, check for errors. Using default strings.");

                    deathStrings = new DeathStrings
                    {
                        headshotDeathStrings = new List<string>
                        {
                            "You have been shot in the head."
                        },
                        explosionDeathStrings = new List<string>
                        {
                            "You have been blown up."
                        },
                        genericDeathStrings = new List<string>
                        {
                            "You are dead."
                        }
                    };
                }
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
