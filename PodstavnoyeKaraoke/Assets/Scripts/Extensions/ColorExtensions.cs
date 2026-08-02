using UnityEngine;

namespace Extensions
{
    public static class ColorExtensions
    {
        public static Color SetR(this Color target, float r)
        {
            target.r = r;
            return target;
        }
        
        public static Color SetG(this Color target, float g)
        {
            target.g = g;
            return target;
        }
        
        public static Color SetB(this Color target, float b)
        {
            target.b = b;
            return target;
        }
        
        public static Color SetA(this Color target, float a)
        {
            target.a = a;
            return target;
        }

        public static Color Lerp(Color from, Color to, float interpolation)
        {
            var r = Mathf.Lerp(from.r, to.r, interpolation);
            var g = Mathf.Lerp(from.g, to.g, interpolation);
            var b = Mathf.Lerp(from.b, to.b, interpolation);
            var a = Mathf.Lerp(from.a, to.a, interpolation);

            return new Color(r, g, b, a);
        }
        
        public static string ConvertColorToHex(this Color target)
        {
            string hexColor = string.Format(
                "#{0:X2}{1:X2}{2:X2}{3:X2}",
                (int)(target.r * 255),
                (int)(target.g * 255),
                (int)(target.b * 255),
                (int)(target.a * 255)
            );

            return hexColor;
        }

        public static Color ConvertHexToColor(string hex)
        {
            var color = Color.white;
            ColorUtility.TryParseHtmlString(hex, out color);
            return color;
        }
    }
}