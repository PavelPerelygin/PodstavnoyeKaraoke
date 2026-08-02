using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Extensions
{
    public static class ButtonExtensions
    {
        public static void SetText(this Button target, string value)
        {
            Transform labelTrn = target.transform.Find("Text");
            if (labelTrn == null)
            {
                Log.Assert();
                return;
            }

            Text text = labelTrn.GetComponent<Text>();
            if (text == null)
            {
                Log.Assert();
                return;
            }

            text.text = value;
        }

        public static void SetInteractable(this Button target, bool value)
        {
            target.interactable = value;
            
            Color color = target.image.color;
            if (value)
                target.image.color = color.SetA(1f);
            else
                target.image.color = color.SetA(0.5f);
        }

        public static void PlayPressAnimation(this Button target)
        {
            LeanTween.scale(target.gameObject, new Vector3(0.9f, 0.9f, 1f), 0.1f).setOnComplete(() =>
            {
                LeanTween.scale(target.gameObject, Vector3.one, 0.1f);
            });
        }
    }
}