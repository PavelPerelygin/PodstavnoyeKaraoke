using System;
using Extensions;
using UnityEngine;

namespace Game.Pages.Main
{
    public class NameGame : MonoBehaviour
    {
        private readonly float _showHideTime = 0.4f;
        
        private Vector3 _initialPosition;
        
        private LTDescr _moveLtd;

        public void Init()
        {
            _initialPosition = transform.localPosition;
        }
        
        public float Show(bool smoothly,float delay = 0f, Action onComplete = null)
        {
            TryCancelMoveLtd();

            if (smoothly)
            {
                _moveLtd = gameObject.LeanMoveLocal(_initialPosition, _showHideTime).setDelay(delay).setEase(LeanTweenType.easeOutBack)
                    .setOnComplete(
                        () =>
                        {
                            _moveLtd = null;
                            
                            onComplete?.Invoke();
                        });
                
                return _showHideTime + delay;
            }
            else
            {
                transform.localPosition = _initialPosition;
                
                onComplete?.Invoke();
                
                return 0f;
            }
        }

        public float Hide(bool smoothly, Action onComplete = null)
        {
            TryCancelMoveLtd();

            var hidePosition = gameObject.GetPositionOffScreenByDirection(Vector2.up,true);

            if (smoothly)
            {
                _moveLtd = gameObject.LeanMoveLocal(hidePosition, _showHideTime).setEase(LeanTweenType.easeInBack)
                    .setOnComplete(
                        () =>
                        {
                            _moveLtd = null;
                            
                            onComplete?.Invoke();
                        });
                
                return _showHideTime;
            }
            else
            {
                transform.localPosition = hidePosition;
                
                onComplete?.Invoke();
                
                return 0f;
            }
        }

        private void TryCancelMoveLtd()
        {
            if(_moveLtd == null)
                return;
            
            LeanTween.cancel(_moveLtd.id);
            _moveLtd = null;
        }
    }
}