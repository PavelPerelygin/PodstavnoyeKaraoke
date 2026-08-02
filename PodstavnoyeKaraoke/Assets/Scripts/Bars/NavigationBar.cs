using Boards;
using Boards.Base;
using Controllers;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Bars
{
    public class NavigationBar : Interactable
    {
        [SerializeField] private Button _openSettingsDialogButton;

        private bool _gameHasBeenLaunched;
        private LTDescr _moveLtd;

        public void Init()
        {
            MainController.Instance.RememberGameObject(gameObject);
            
            InitButtons();
        }
        
        
        private void InitButtons()
        {
            _openSettingsDialogButton.onClick.AddListener(ButtonPress);
            _openSettingsDialogButton.DisableOverDownColors();
        }
        
        private void OpenMainPage()
        {
            var board = MainController.Instance.ActiveScene.ActiveBoard as MainBoard;
            if (board == null)
            {
                Log.Assert();
                return;
            }
        }
        
        private void OpenSettingsDialog()
        {
            var board = MainController.Instance.ActiveScene.ActiveBoard as MainBoard;
            if (board != null)
                board.OpenSettingsDialog();
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;
            
            if (selectedObj == _openSettingsDialogButton.gameObject)
            {
                OpenSettingsDialog();
            }

            return true;
        }
    }
}