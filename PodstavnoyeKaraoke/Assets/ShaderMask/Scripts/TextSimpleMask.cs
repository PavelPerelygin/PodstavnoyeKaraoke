using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ShaderMask.Scripts
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]  
    [RequireComponent(typeof(Text))]
    public class TextSimpleMask : MonoBehaviour
    {
        [HideInInspector][SerializeField] private Text _text;
        [SerializeField] [Range(0,255)] private int _maskLayer;
        [SerializeField] private float _colorMask = 15;

        private Material _material;
        
        private readonly int _stencilMaskId = Shader.PropertyToID("_StencilMask");
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
                _material = new Material(Shader.Find("ShaderMask/SimpleMask"));

            if (_text == null)
                _text = GetComponent<Text>();

            if(_text != null)
                _text.material = _material;
        }
        
        private void Refresh()
        {
            _material.SetInt(_stencilMaskId,_maskLayer);
            _material.SetFloat(_colorMaskId,_colorMask);
        }
    }
}