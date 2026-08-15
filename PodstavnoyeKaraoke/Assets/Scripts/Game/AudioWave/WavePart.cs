using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.AudioWave
{
    public class WavePart : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RawImage _rawImage;

        public void SetTexture(Texture2D texture)
        {
            _rectTransform.SetSizeX(texture.width);
            _rawImage.texture = texture;
        }
        
    }
}