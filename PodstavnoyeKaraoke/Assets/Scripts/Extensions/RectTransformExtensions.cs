using UnityEngine;

namespace Extensions
{
    public static class RectTransformExtensions
    {
        public static LTDescr LeanSizeY(this RectTransform target, float to, float time)
        {
            var currentSize = target.sizeDelta;
            
            return LeanTween.value(currentSize.y, to, time).setOnUpdate((float v) =>
            {
                currentSize.y = v;
                target.sizeDelta = currentSize;
            });
        }
        
        public static LTDescr LeanSizeX(this RectTransform target, float to, float time)
        {
            var currentSize = target.sizeDelta;
            
            return LeanTween.value(currentSize.x, to, time).setOnUpdate((float v) =>
            {
                currentSize.x = v;
                target.sizeDelta = currentSize;
            });
        }

        public static void SetSizeX(this RectTransform target, float x)
        {
            var currentSize = target.sizeDelta;
            currentSize.x = x;
            target.sizeDelta = currentSize;
        }
        
        public static void SetSizeY(this RectTransform target, float y)
        {
            var currentSize = target.sizeDelta;
            currentSize.y = y;
            target.sizeDelta = currentSize;
        }

        public static Vector2 GetRectSize(this RectTransform target)
        {
            Vector2 size = Vector2.zero;

            size.x = target.rect.width;
            size.y = target.rect.height;

            return size;
        }
    }
}