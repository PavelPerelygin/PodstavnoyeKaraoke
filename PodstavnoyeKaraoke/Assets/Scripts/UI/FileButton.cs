using Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FileButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private Text _text;
        [SerializeField] private Sprite _existSprite;
        [SerializeField] private Sprite _notExistSprite;
        
        public Button Button => _button;

        public void ExistStay()
        {
            _image.sprite = _existSprite;
            _text.text = MainController.Instance.TextManager.GetText(505);
        }

        public void NotExistStay()
        {
            _image.sprite = _notExistSprite;
            _text.text = MainController.Instance.TextManager.GetText(504);
        }
    }
}