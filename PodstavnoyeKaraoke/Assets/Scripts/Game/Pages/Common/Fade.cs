using System;
using UnityEngine;

namespace Game.Pages.Common
{
    public class Fade : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        private LTDescr _alphaLtd;

        public float SetAlpha(float alpha,bool smoothly, float time, float delay, Action onComplete = null)
        {
            TryCancelAlphaLtd();

            if (smoothly)
            {
                _alphaLtd = _canvasGroup.LeanAlpha(alpha, time).setDelay(delay).setOnComplete(() =>
                {
                    _alphaLtd = null;
                    
                    onComplete?.Invoke();
                });

                return time + delay;
            }
            
            _canvasGroup.alpha = alpha;
                
            onComplete?.Invoke();

            return 0;
        }

        private void TryCancelAlphaLtd()
        {
            if(_alphaLtd == null)
                return;
            
            LeanTween.cancel(_alphaLtd.id);
            _alphaLtd = null;
        }
    }
}