using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.Pages.Common.SkinItem
{
    public class RawImageSkin : MonoBehaviour
    {
        [SerializeField] private string _nameResource = "";
        [SerializeField] private RawImage _rawImage;

        public string GetNameResource()
        {
            return _nameResource;
        }

        public void SetTexture2D(Texture2D texture)
        {
            _rawImage.texture = texture;
        }
    }
}