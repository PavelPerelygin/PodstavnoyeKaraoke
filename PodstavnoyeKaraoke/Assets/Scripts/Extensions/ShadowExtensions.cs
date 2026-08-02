using UnityEngine.UI;

namespace Extensions
{
    public static class ShadowExtensions
    {
        public static LTDescr LeanAlpha(this Shadow target, float to, float time)
        {
            var currentColor = target.effectColor;

            return LeanTween.value(currentColor.a, to, time).setOnUpdate((float v) =>
            {
                currentColor.a = v;
                target.effectColor = currentColor;
            });
        }
    }
}