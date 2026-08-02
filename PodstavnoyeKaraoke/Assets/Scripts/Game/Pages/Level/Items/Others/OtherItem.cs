using System;
using UnityEngine;
using Utilities;

namespace Game.Pages.Level.Items.Others
{
    public class OtherItem : GameFieldItem
    {
        [SerializeField] protected RectTransform _root;
        
        protected LtdManager _ltdManager = new LtdManager();
        
        #region Show / hide

        public void Show(bool smoothly, float time = 0f, float delay = 0f, Action onComplete = null)
        {
            _ltdManager.TryCancelLtd("scale_root");

            if (smoothly)
            {
                var scaleTween = _root.LeanScale(Vector3.one, time).setDelay(delay).setEase(LeanTweenType.easeOutBack)
                    .setOnComplete(
                        () =>
                        {
                            _ltdManager.TrySetNullLtd("scale_root");
                            
                            onComplete?.Invoke();
                        });
                
                _ltdManager.AddLtd("scale_root",scaleTween);
            }
            else
            {
                _root.localScale = Vector3.one;
                
                onComplete?.Invoke();
            }
        }
        
        public void Hide(bool smoothly, float time = 0f, float delay = 0f, Action onComplete = null)
        {
            _ltdManager.TryCancelLtd("scale_root");

            if (smoothly)
            {
                var scaleTween = _root.LeanScale(Vector3.zero, time).setDelay(delay).setEase(LeanTweenType.easeInBack)
                    .setOnComplete(
                        () =>
                        {
                            _ltdManager.TrySetNullLtd("scale_root");
                            
                            onComplete?.Invoke();
                        });
                
                _ltdManager.AddLtd("scale_root",scaleTween);
            }
            else
            {
                _root.localScale = Vector3.zero;
                
                onComplete?.Invoke();
            }
        }

        #endregion
    }
}