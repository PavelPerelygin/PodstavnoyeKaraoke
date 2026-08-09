using Dialogs.Base;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class WarningDialog : Dialog
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Text _text;
        
        
        public void Init(string text)
        {
            _text.text = text;

            InitButton();
        }

        private void InitButton()
        {
            _closeButton.onClick.AddListener(ButtonPress);
            _closeButton.DisableOverDownColors();
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _closeButton.gameObject)
            {
                Hide();
            }
            
            return true;
        }
    }
}