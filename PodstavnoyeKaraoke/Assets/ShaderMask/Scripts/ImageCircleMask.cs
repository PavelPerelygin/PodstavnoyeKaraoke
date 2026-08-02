using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ShaderMask.Scripts
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]    
    [RequireComponent(typeof(Image))]
    public class ImageCircleMask : MonoBehaviour
    {
        [HideInInspector][SerializeField] private Image _image;
        [SerializeField] [Range(0,255)] private int _maskLayer;
        [SerializeField] private float _colorMask = 15;
        [SerializeField] [Range(0,1f)] private float _radiusX = 1f;
        [SerializeField] [Range(0,1f)] private float _radiusY = 1f;
        [SerializeField] [Range(0,1f)] private float _scaleX = 1f;
        [SerializeField] [Range(0,1f)] private float _scaleY = 1f;
        [SerializeField] [Range(0,0.999f)] private float _antialiasThreshold = 0.96f;

        private Material _material;
        
        private static readonly int _stencilMaskId = Shader.PropertyToID("_StencilMask");
        private static readonly int _colorMaskId = Shader.PropertyToID("_ColorMask");
        private static readonly int _radiusXId = Shader.PropertyToID("_RadiusX");
        private static readonly int _radiusYId = Shader.PropertyToID("_RadiusY");
        private static readonly int _scaleXId = Shader.PropertyToID("_ScaleX");
        private static readonly int _scaleYId = Shader.PropertyToID("_ScaleY");
        private static readonly int _antialiasThresholdId = Shader.PropertyToID("_AntialiasThreshold");
        
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
                _material = new Material(Shader.Find("ShaderMask/CircleMask"));
            
            if (_image == null)
                _image = GetComponent<Image>();
            
            if(_image != null)
                _image.material = _material;
        }
        
        private void Refresh()
        {
            _material.SetInt(_stencilMaskId,_maskLayer);
            _material.SetFloat(_colorMaskId,_colorMask);
            _material.SetFloat(_radiusXId,_radiusX);
            _material.SetFloat(_radiusYId,_radiusY);
            _material.SetFloat(_scaleXId,_scaleX);
            _material.SetFloat(_scaleYId,_scaleY);
            _material.SetFloat(_antialiasThresholdId,_antialiasThreshold);

            _material.EnableKeyword("UNITY_UI_ALPHACLIP");
        }
    }
}