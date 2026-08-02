using Dialogs.Base;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs
{
    public class AttentionDialog : Dialog
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Text _messageText;
        

        public void Init(string text)
        {
            _messageText.text = text;
            
            InitButton();
        }

        private void InitButton()
        {
            _continueButton.onClick.AddListener(ButtonPress);
            _continueButton.DisableOverDownColors();
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _continueButton.gameObject)
            {
                Hide();
            }
            
            return true;
        }
    }
}