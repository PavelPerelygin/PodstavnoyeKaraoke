using System;
using Dialogs.Base;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs
{
    public class ConfirmationDialog : Dialog
    {
        [SerializeField] private Text _text;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _rejectButton;

        private Action _onConfirm;
        private Action _onReject;
        
        public void Init(string text, Action onConfirm, Action onReject)
        {
            _text.text = text;
            
            _onConfirm = onConfirm;
            _onReject = onReject;

            InitButtons();
        }

        private void InitButtons()
        {
            _confirmButton.onClick.AddListener(ButtonPress);
            _confirmButton.DisableOverDownColors();
            
            _rejectButton.onClick.AddListener(ButtonPress);
            _rejectButton.DisableOverDownColors();
        }

        private void OnConfirm()
        {
            _onConfirm?.Invoke();
            
            Hide();
        }

        private void OnReject()
        {
            _onReject?.Invoke();
            
            Hide();
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _confirmButton.gameObject)
            {
                OnConfirm();
            }
            else if (selectedObj == _rejectButton.gameObject)
            {
                OnReject();
            }
            
            return true;
        }
    }
}