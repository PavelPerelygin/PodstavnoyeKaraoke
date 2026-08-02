using UnityEngine;
using UnityEngine.UI;

namespace Utilities.QR
{
    [RequireComponent(typeof(RawImage))]
    public class QrGenerator : MonoBehaviour
    {
        [SerializeField] private Color _darkColor = Color.black;
        [SerializeField] private Color _lightColor = Color.white;
        
        const int PIXELS_PER_MODULE = 20;

        public void EncodeString(string text)
        {
            Texture2D qrTexture = QrCodeGenerator.GetTexture(text,_darkColor,_lightColor);
            GetComponent<RawImage>().texture = qrTexture;
        }

        public void EncodeString(string text, QrCodeGenerator.ECCLevel errorCorrectionLevel)
        {
            QrCodeGenerator qrGenerator = new QrCodeGenerator();
            QrCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(text, errorCorrectionLevel);

            Texture2D qrTexture = qrCode.GetGraphic(PIXELS_PER_MODULE, _darkColor, _lightColor);
            GetComponent<RawImage>().texture = qrTexture;
        }
    }
}
