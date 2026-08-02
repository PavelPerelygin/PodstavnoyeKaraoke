using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ShaderMask.Scripts
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]  
    [RequireComponent(typeof(Image))]
    public class ImageMasked : MonoBehaviour
    {
        [HideInInspector][SerializeField] private Image _image;
        [SerializeField] [Range(0,255)] private int _maskLayer;
        [SerializeField] private CompareFunction _compareFunction;
        [SerializeField] private float _colorMask = 15;

        private Material _material;
        private readonly int _stencilCompId = Shader.PropertyToID("_StencilComp");
        private readonly int _stencilMask = Shader.PropertyToID("_StencilMask");
        private readonly int _colorMaskId = Shader.PropertyToID("_ColorMask");
        
        private void OnEnable()
        {
            Validate();
            Refresh();
        }

        private void OnValidate()
        {
            Validate();
            Refresh();
        }

        private void Validate()
        {
            if (_material == null)
                _material = new Material(Shader.Find("ShaderMask/Masked"));

            if (_image == null)
                _image = GetComponent<Image>();

            if(_image != null)
                _image.material = _material;
        }
        
        private void Refresh()
        {
            _material.SetInt(_stencilCompId,(int)_compareFunction);
            _material.SetInt(_stencilMask,_maskLayer);
            _material.SetFloat(_colorMaskId,_colorMask);
        }
    }
}