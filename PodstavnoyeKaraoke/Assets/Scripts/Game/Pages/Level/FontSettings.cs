using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Game.Pages.Level
{
    public static class FontSettings
    {
        public static readonly string  DefaultFontName = "Impact"; 
        private static List<string> _fontNames = new List<string>(){"Impact",
            "FiraSans-Light",
            "FiraSans-Medium",
            "FiraSans-Regular",
            "Montserrat-Light",
            "Montserrat-Medium",
            "Montserrat-Regular",
            "Roboto",
            "Manrope",
            "PublicSans",
            "GothicA1-Bold",
            "GothicA1-Light",
            "GothicA1-Regular",
            "GothicA1-Medium"
        };

        public static List<string> GetFontNames()
        {
            return _fontNames;
        }

        public static Font GetFontByName(string fontName)
        {
            var font = Resources.Load<Font>($"Fonts/{fontName}");
            if (font == null)
            {
                Log.Assert();
                font = Resources.Load<Font>($"Fonts/Impact");
            }
            
            return font;
        }
    }
}