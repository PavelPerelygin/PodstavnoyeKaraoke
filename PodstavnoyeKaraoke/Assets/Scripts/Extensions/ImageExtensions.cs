using UnityEngine;
using UnityEngine.UI;

namespace Extensions
{
    public static class ImageExtensions
    {
        public static LTDescr LeanAlpha(this Image target, float to, float time,LeanTweenType ease = LeanTweenType.linear)
        {
            Image image = target;
        
            float alpha = to;
            if (alpha < 0)
                alpha = 0;
            else if (alpha > 1f)
                alpha = 1f;
        
            Color currentColor = target.color;
            Color needColor = currentColor;
            needColor.a = alpha;


            return LeanTween.value(target.gameObject, currentColor, needColor, time).setOnUpdate((Color color) =>
            {
                image.color = color;
            }).setEase(ease);
        }
    
        public static void SetAlpha(this Image target, float alpha)
        {
            float needAlpha = alpha;
            if (alpha < 0)
                alpha = 0;
            else if (alpha > 1f)
                alpha = 1f;

            Color color = target.color;
            color.a = needAlpha;

            target.color = color;
        }
        
        public static LTDescr LeanColor(this Image target, Color to, float time)
        {
            Image iamge = target;
            Color currentColor = iamge.color;

            return LeanTween.value(0, 1, time).setOnUpdate((float v) =>
            {
                var r = Mathf.Lerp(currentColor.r, to.r, v);
                var g = Mathf.Lerp(currentColor.g, to.g, v);
                var b = Mathf.Lerp(currentColor.b, to.b, v);
                var a = Mathf.Lerp(currentColor.a, to.a, v);

                iamge.color = new Color(r, g, b, a);
            });
        }

        public static LTDescr LeanColor(this Image target, string to, float time)
        {
            var color = ColorExtensions.ConvertHexToColor(to);
            return LeanColor(target, color, time);
        }
        
    }
}