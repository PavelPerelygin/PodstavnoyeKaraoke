using System;
using UnityEngine;
using UnityEngine.UI;

namespace Blurs
{
    [RequireComponent(typeof(Image))]
    public class Blur : MonoBehaviour
    {
        [SerializeField] private TypeBlur _type;
        
        private Material _material;

        public TypeBlur Type => _type;
        
        public bool IsEnable { get; private set; }

        public void Init()
        {
            var image = gameObject.GetComponent<Image>();
            image.material = GetBlurMaterial();
            _material = image.material;

            DisableBlur(0f);
        }
        
        private Material GetBlurMaterial()
        {
            Material material = new Material(Shader.Find("Custom/Blur"));
            return material;
        }

        public void EnableBlur(float intensity, float time = 0f,float delay = 0f, Action onCompleted = null)
        {
            IsEnable = true;
            
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            
            gameObject.LeanCancel();
            
            if (time > 0)
            {
                float currentIntensity = GetIntensity();
                LeanTween.value(gameObject, currentIntensity, intensity, time).setOnUpdate((float v) =>
                {
                    SetIntensity(v);
                }).setDelay(delay).setOnComplete(() => { onCompleted?.Invoke(); });
            }
            else
            {
                SetIntensity(intensity);
                onCompleted?.Invoke();
            }
        }

        public void DisableBlur(float time = 0f,float delay = 0f, Action onCompleted = null)
        {
            IsEnable = false;
            
            if (time > 0)
            {
                float currentIntensity = GetIntensity();
                LeanTween.value(gameObject, currentIntensity, 0f, time).setOnUpdate((float v) =>
                {
                    SetIntensity(v);
                }).setDelay(delay).setOnComplete(() =>
                {
                    gameObject.SetActive(false);
                    onCompleted?.Invoke();
                });
            }
            else
            {
                SetIntensity(0f);
                gameObject.SetActive(false);
                onCompleted?.Invoke();
            }
        }
        
        private void SetIntensity(float value)
        {
            float intensity = Mathf.Lerp(0f, 20f, value);
            _material.SetFloat("_Size",intensity);
        }
        
        private float GetIntensity()
        {
            float intensity = _material.GetFloat("_Size");
            return Mathf.InverseLerp(0f, 20f, intensity);
        }
    }
}