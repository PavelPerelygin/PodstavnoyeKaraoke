using UnityEngine;
using UnityEngine.UI;

namespace Extensions
{
    public static class TextExtensions
    {
        public static void SetAlpha(this Text target, float alpha)
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
        
        public static LTDescr LeanAlpha(this Text target, float to, float time,LeanTweenType ease = LeanTweenType.linear)
        {
            Text text = target;
        
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
                text.color = color;
            }).setEase(ease);
        }
    }
}