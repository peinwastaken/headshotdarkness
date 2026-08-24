using EFT;
using HeadshotDarkness.Enums;
using System.Collections.Generic;
using TMPro;

namespace HeadshotDarkness.Helpers
{
    public static class FontHelper
    {
        public static Dictionary<EDeathStringFont, TMP_FontAsset> LoadedFonts = [];

        public static void LoadFonts()
        {
            IEnumerable<TMP_FontAsset> tarkovFonts = LocalizationManager.Instance._allFonts;

            // this is stupid. idgaf
            foreach (TMP_FontAsset font in tarkovFonts)
            {
                if (font.name.ToLower().Contains("arial - normal"))
                {
                    LoadedFonts.Add(EDeathStringFont.Arial, font);
                    break;
                }
                else if (font.name.ToLower().Contains("bender normal"))
                {
                    LoadedFonts.Add(EDeathStringFont.Bender, font);
                    break;
                }
            }
        }

        public static TMP_FontAsset FindFont(EDeathStringFont fontType)
        {
            LoadedFonts.TryGetValue(fontType, out TMP_FontAsset font);
            return font;
        }
    }
}
