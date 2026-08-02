using System;
using Extensions;
using UnityEngine;

namespace Fades
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Fade : MonoBehaviour
    {
        [SerializeField] private TypeFade _type;
        
        private CanvasGroup _canvasGroup;

        public TypeFade Type => _type;
        public bool IsEnable { get; private set; }
        
        public void Init()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            DisableFade();
        }

        public void EnableFade(float intensity, float time = 0f,float delay = 0f, Action onCompleted = null)
        {
            IsEnable = true;

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            
            gameObject.LeanCancel();

            if (time > 0)
            {
                _canvasGroup.AlphaCanvas(intensity, time).setDelay(delay).setOnComplete(() => { onCompleted?.Invoke(); });
            }
            else
            {
                _canvasGroup.alpha = intensity;
                onCompleted?.Invoke();
            }
        }
        
        public void DisableFade(float time = 0f, float delay = 0f, Action onCompleted = null)
        {
            IsEnable = false;
            
            if (time > 0)
            {
                _canvasGroup.AlphaCanvas(0f, time).setDelay(delay).setOnComplete(() =>
                {
                    gameObject.SetActive(false);
                    onCompleted?.Invoke();
                });
            }
            else
            {
                _canvasGroup.alpha = 0f;
                gameObject.SetActive(false);
                onCompleted?.Invoke();
            }
        }
    }
}