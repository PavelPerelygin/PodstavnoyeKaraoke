using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Extensions
{
    public static class InputFieldExtensions
    {
        public static void SetPlaceholder(this InputField target, string value = "")
        {
            Transform textTrn = target.transform.Find("Placeholder");
            if (textTrn == null)
            {
                Log.Assert();
                return;
            }

            Text text = textTrn.GetComponent<Text>();
            if (text == null)
            {
                Log.Assert();
                return;
            }

            text.text = value;
        }
    }
}