using UnityEngine;

namespace Extensions
{
    public static class AudioSourceExtensions
    {
        public static float GetProgress(this AudioSource target)
        {
            if (target.isPlaying)
            {
                var progress = target.time / target.clip.length;
                return progress;
            }
            
            return 0;
        }
    }
}