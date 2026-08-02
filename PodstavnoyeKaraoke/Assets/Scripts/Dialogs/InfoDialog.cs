using Dialogs.Base;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs
{
    public class InfoDialog : Dialog
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Text _headerText;
        [SerializeField] private Text _contentText;

        public void Init(string headerText, string contentText)
        {
            _headerText.text = headerText;
            _contentText.text = contentText;
            
            InitButton();
        }

        private void InitButton()
        {
            _closeButton.onClick.AddListener(ButtonPress);
            _closeButton.DisableOverDownColors();
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _closeButton.gameObject)
            {
                Hide();
            }

            return true;
        }
    }
}