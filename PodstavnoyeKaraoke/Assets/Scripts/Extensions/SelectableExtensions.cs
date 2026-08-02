using UnityEngine;
using UnityEngine.UI;

namespace Extensions
{
    public static class SelectableExtensions
    {
        public static void DisableOverDownColors(this Selectable target)
        {
            target.transition = Selectable.Transition.None;
        }
    }
}