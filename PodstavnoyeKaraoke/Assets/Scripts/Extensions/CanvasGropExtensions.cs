using UnityEngine;

namespace Extensions
{
    public static class CanvasGropExtensions
    {
        public static LTDescr AlphaCanvas(this CanvasGroup target, float to, float time)
        {
            return LeanTween.alphaCanvas(target, to, time);
        }
    }
}