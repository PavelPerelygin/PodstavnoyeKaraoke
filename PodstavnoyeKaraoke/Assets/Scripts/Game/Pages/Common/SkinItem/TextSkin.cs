using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Common.SkinItem
{
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class TextSkin : MonoBehaviour
    {
        [SerializeField] private string _nameResource = "";
        [SerializeField] private Text _text;
        
        public string GetNameResource()
        {
            return _nameResource;
        }

        public void SetColor(Color color)
        {
            _text.color = color;
        }
        
        private void OnValidate()
        {
#if UNITY_EDITOR
            if (_text == null)
                _text = GetComponent<Text>();
#endif
        }
    }
}