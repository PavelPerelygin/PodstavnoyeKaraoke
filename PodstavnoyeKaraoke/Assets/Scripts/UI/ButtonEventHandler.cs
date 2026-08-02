using Extensions;
using Managers.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonEventHandler : MonoBehaviour , IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField] private GameObject _button;
        [SerializeField] private GameObject _text;
        [SerializeField] private float _scaleButton = 0.05f;
        [SerializeField] private float _scaleText = 0.05f;
        [SerializeField] private float _time = 0.1f;

        public bool Enable { get; private set; } = true;
        
        private Vector3 _initScaleButton = Vector3.zero;
        private Vector3 _initScaleText = Vector3.zero;

        private void Awake()
        {
            GetComponent<Button>().DisableOverDownColors();
            
            if(_button != null)
                _initScaleButton = _button.transform.localScale;
            
            if(_text != null)
                _initScaleText = _text.transform.localScale;
        }

        public void SetEnable(bool enable, bool needSetDefaultScale = true)
        {
            if(enable == Enable)
                return;
            
            Enable = enable;

            if (needSetDefaultScale)
            {
                ScaleButtonUp(false);
                ScaleTextUp(false);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(!Enable)
                return;

            ScaleButtonDown();
        }

        private void ScaleButtonDown()
        {
            if(_button == null)
                return;
            
            var scale = _initScaleButton;
            scale.x -= _scaleButton;
            scale.y -= _scaleButton;
            
            LeanTween.scale(_button, scale, _time);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(!Enable)
                return;

            ScaleButtonUp();
        }

        private void ScaleButtonUp (bool smoothly = true)
        {
            if(_button == null)
                return;

            if (smoothly)
                _button.LeanScale(_initScaleButton, _time);
            else
                _button.transform.localScale = _initScaleButton;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!Enable)
                return;

            ScaleTextDown();
        }
        
        private void ScaleTextDown()
        {
            if(_text == null)
                return;
            
            var scale = _initScaleText;
            scale.x += _scaleText;
            scale.y += _scaleText;
            
            LeanTween.scale(_text, scale, _time);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(!Enable)
                return;

            ScaleTextUp();
        }

        private void ScaleTextUp(bool smoothly = true)
        {
            if(_text == null)
                return;

            if (smoothly)
                _text.LeanScale(_initScaleText, _time);
            else
                _text.transform.localScale = _initScaleText;
        }

        public void EmulateButtonPress()
        {
            ScaleButtonDown();
            LeanTween.delayedCall(_time, () => { ScaleButtonUp();});
        }
    }
}