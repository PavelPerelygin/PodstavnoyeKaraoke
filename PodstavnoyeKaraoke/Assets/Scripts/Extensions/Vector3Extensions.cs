using UnityEngine;

namespace Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 AddDelta(this Vector3 target, float delat)
        {
            return new Vector3(target.x + delat, target.y + delat, target.z + delat);
        }
    }
}