using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Common.SkinItem
{
    [RequireComponent(typeof(Image))]
    public class ImageSkin : MonoBehaviour
    {
        [SerializeField] private string _nameResource = "";
        [SerializeField] private Image _image;

        public string GetNameResource()
        {
            return _nameResource;
        }

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        private void OnValidate()
        {
            return;
#if UNITY_EDITOR
            if (_image == null)
                _image = GetComponent<Image>();
            
            if(_image.sprite == null || _image.sprite.name == _nameResource)
                return;
            
            _nameResource = _image.sprite.name.ToLower();
#endif
        }
    }
}