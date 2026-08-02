using Controllers;
using Controllers.Update;
using Dialogs.Base;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Network;

namespace Dialogs
{
    public class UpdateDialog : Dialog
    {
        [SerializeField] private Text _contentText;
        [SerializeField] private Button _downloadButton;
        [SerializeField] private Button _closeButton;

        private string _downloadURL;
        UpdateInfo _updateInfo;
    
        public void Init(UpdateInfo updateInfo)
        {
            _updateInfo = updateInfo;
            _downloadURL = _updateInfo.Url;

            SetTextContent(MainController.Instance.TextManager.GetText(561));
            InitButtons();
        }

        public void Init(string textContent, string downloadUrl)
        {
            _downloadURL = downloadUrl;
            
            SetTextContent(textContent);
            InitButtons();
        }

        private void SetTextContent(string content)
        {
            _contentText.text = content;
        }
    
        private void InitButtons()
        {
            _closeButton.onClick.AddListener(ButtonPress);
            _closeButton.DisableOverDownColors();

            _downloadButton.onClick.AddListener(ButtonPress);
            _downloadButton.DisableOverDownColors();
        }
    
        protected override bool GameObjectClickHandler(GameObject gameObj)
        {
            if (!base.GameObjectClickHandler(gameObj))
                return false;
        
            if (gameObj == _downloadButton.gameObject)
            {
                Browser.OpenWebsite(_downloadURL);
            }
            else if (gameObj == _closeButton.gameObject)
            {
                Hide();
            }

            return true;
        }
    }
}
