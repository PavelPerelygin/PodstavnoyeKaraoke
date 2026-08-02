using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.Activation
{
    public class Message : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private CanvasGroup _canvasGroup;

        private bool _isShown = true;
        private bool _showComplete = true;
        private LTDescr _alphaLtd;
        
        public void SetMessage(string message)
        {
            if (_isShown)
            {
                Hide(true, () =>
                {
                    _text.text = message;
                    
                    Show(true);
                });
            }
            else
            {
                _text.text = message;
                
                Show(true);
            }
        }

        #region Show / hide

        private void Show(bool smoothly, Action onComplete = null)
        {
            _isShown = true;
            _showComplete = false;
            
            TryCancelAlphaLtd();

            if (smoothly)
            {
                _alphaLtd = _canvasGroup.LeanAlpha(1f, 0.2f).setOnComplete(() =>
                {
                    _alphaLtd = null;

                    _showComplete = true;
                    
                    onComplete?.Invoke();
                });
            }
            else
            {
                _canvasGroup.alpha = 1;

                _showComplete = true;
                
                onComplete?.Invoke();
            }
        }
        
        public void Hide(bool smoothly, Action onComplete = null)
        {
            _isShown = false;
            _showComplete = false;
            
            TryCancelAlphaLtd();

            if (smoothly)
            {
                _alphaLtd = _canvasGroup.LeanAlpha(0f, 0.2f).setOnComplete(() =>
                {
                    _alphaLtd = null;
                    
                    _showComplete = true;
                    
                    onComplete?.Invoke();
                });
            }
            else
            {
                _canvasGroup.alpha = 0;
                
                _showComplete = true;
                
                onComplete?.Invoke();
            }
        }


        private void TryCancelAlphaLtd()
        {
            if(_alphaLtd == null)
                return;
            
            LeanTween.cancel(_alphaLtd.id);
            _alphaLtd = null;
        }

        #endregion

        private void Update()
        {
            if(!_isShown || !_showComplete)
                return;

            if (Input.GetMouseButtonUp(0))
            {
                Hide(true);   
            }
        }
    }
}