using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ShaderMask.Scripts
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]  
    [RequireComponent(typeof(Image))]
    public class ImageSimpleMask : MonoBehaviour
    {
        [HideInInspector][SerializeField] private Image _image;
        [SerializeField] [Range(0,255)] private int _maskLayer;
        [SerializeField] private float _colorMask = 15;
        [SerializeField] private Color _maskColor;
        [Range(0,1)][SerializeField] private float _colorTolerance;

        private Material _material;
        private readonly int _stencilMaskId = Shader.PropertyToID("_StencilMask");
        private readonly int _colorMaskId = Shader.PropertyToID("_ColorMask");
        private readonly int _maskColorId = Shader.PropertyToID("_MaskColor");
        private readonly int _colorToleranceId = Shader.PropertyToID("_ColorTolerance");
        
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

            if (_image == null)
                _image = GetComponent<Image>();

            if(_image != null)
                _image.material = _material;
        }
        
        private void Refresh()
        {
            _material.SetInt(_stencilMaskId,_maskLayer);
            _material.SetFloat(_colorMaskId,_colorMask);
            _material.SetColor(_maskColorId,_maskColor);
            _material.SetFloat(_colorToleranceId,_colorTolerance);
        }
    }
}