using System;
using System.Collections.Generic;

namespace Utilities
{
    public class AnimationCounter
    {
        public List<Action> OnCompleted { get; set; } = new List<Action>();
        
        private int _countAnimations = 0;
        
        public void IncrCountAnimation(int delta = 1)
        {
            _countAnimations += delta;
        }
        
        public void DecrCountAnimation()
        {
            _countAnimations--;
            if (_countAnimations < 0)
            {
                Log.Assert("Error in counting animations");
                _countAnimations = 0;
            }
        }
        
        public bool IsCompleted()
        {
            if (_countAnimations > 0)
                return false;

            return true;
        }
    }
}