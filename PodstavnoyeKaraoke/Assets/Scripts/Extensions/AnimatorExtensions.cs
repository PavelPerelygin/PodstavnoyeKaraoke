using System.Linq;
using UnityEngine;

namespace Extensions
{
    public static class AnimatorExtensions
    {
        public static float GetLenghtAnimation(this Animator target,params string [] names )
        {
            RuntimeAnimatorController controler = target.runtimeAnimatorController;

            float lenght = 0;

            foreach (AnimationClip clip in controler.animationClips)
            {
                if (names.Contains(clip.name))
                    lenght +=clip.length;
            }

            return lenght;
        }
    }
}