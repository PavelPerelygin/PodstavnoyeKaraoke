
using UnityEngine;

namespace Extensions
{
    public static class TransformExtensions
    {
        public static Transform GetChildByName(this Transform target, string name)
        {
            Transform needed = null;

            foreach (Transform child in target)
            {
                if (child.name == name)
                {
                    needed = child;
                    break;
                }
            }

            return needed;
        }
    }
}
