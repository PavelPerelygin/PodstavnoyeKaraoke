using Controllers;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Text))]
    public class SetLabel : MonoBehaviour
    {
        [SerializeField] private int _textId;
        [SerializeField] private bool _isUpper;

        private Text _text;
        
        private void Awake()
        {
            GetTextComponent().text = MainController.Instance.TextManager.GetText(_textId, _isUpper);
        }

        private Text GetTextComponent()
        {
            if (_text == null)
                _text = GetComponent<Text>();

            return _text;
        }

        private void OnValidate()
        {
#if !UNITY_EDITOR
            return;
#endif
            if(Application.isPlaying)
                return;
            
            var value = TextManager.Instance().GetText(_textId, _isUpper);
            GetTextComponent().text = value;

        }
    }
}
