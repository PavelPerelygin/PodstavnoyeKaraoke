using Controllers;
using Dialogs.Base;
using Extensions;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Dialogs.SettingsDialog.Pages
{
    public class PageSpecial : BasePage
    {
        [SerializeField] private FileButton _mainScreenBackgroundButton;
        [SerializeField] private FileButton _selectScreenBackgroundButton;
        [SerializeField] private FileButton _gameScreenBackgroundButton;
        [SerializeField] private Button _editMicrophoneButton;
        [SerializeField] private Button _openListLevelsButton;
        [SerializeField] private Button _openListSkinsButton;
        
        public override void Init()
        {
            InitButtons();
            InitInputField();
        }

        private void InitButtons()
        {
            _mainScreenBackgroundButton.Button.onClick.AddListener(ButtonPress);
            _mainScreenBackgroundButton.Button.DisableOverDownColors();
            UpdateMainScreenBackgroundButton();
            
            _selectScreenBackgroundButton.Button.onClick.AddListener(ButtonPress);
            _selectScreenBackgroundButton.Button.DisableOverDownColors();
            UpdateSelectScreenBackgroundButton();
            
            _gameScreenBackgroundButton.Button.onClick.AddListener(ButtonPress);
            _gameScreenBackgroundButton.Button.DisableOverDownColors();
            UpdateGameScreenBackgroundButton();
            
            _editMicrophoneButton.onClick.AddListener(ButtonPress);
            _editMicrophoneButton.DisableOverDownColors();
            
            _openListLevelsButton.onClick.AddListener(ButtonPress);
            _openListLevelsButton.DisableOverDownColors();
            
            _openListSkinsButton.onClick.AddListener(ButtonPress);
            _openListSkinsButton.DisableOverDownColors();
        }

        private void InitInputField()
        {
        }

        private void UpdateMainScreenBackgroundButton()
        {
            if(MainController.Instance.LocalSettings.GetMainScreenBackground().IsExistSource())
                _mainScreenBackgroundButton.ExistStay();
            else
                _mainScreenBackgroundButton.NotExistStay();
        }
        
        private void UpdateSelectScreenBackgroundButton()
        {
            if(MainController.Instance.LocalSettings.GetSelectScreenBackground().IsExistSource())
                _selectScreenBackgroundButton.ExistStay();
            else
                _selectScreenBackgroundButton.NotExistStay();
        }
        
        private void UpdateGameScreenBackgroundButton()
        {
            if(MainController.Instance.LocalSettings.GetGameScreenBackground().IsExistSource())
                _gameScreenBackgroundButton.ExistStay();
            else
                _gameScreenBackgroundButton.NotExistStay();
        }

        private void OpenEditMicrophoneDialog()
        {
            MainController.Instance.DialogsController.OpenDialog(TypeDialog.EditMicrophone, 0.4f);
        }

        private void OpenListLevelsDialog()
        {
            MainController.Instance.DialogsController.OpenDialog(TypeDialog.ListLevels, 0.4f);
        }

        private void OpenListSkinsDialog()
        {
            MainController.Instance.DialogsController.OpenDialog(TypeDialog.ListSkins, 0.4f);
        }

        private void ClickByMainScreenBackgroundButton()
        {
            if (MainController.Instance.LocalSettings.GetMainScreenBackground().IsExistSource())
            {
                MainController.Instance.LocalSettings.GetMainScreenBackground().RemoveSource();
                UpdateMainScreenBackgroundButton();
            }
            else
            {
                MainController.Instance.LocalSettings.GetMainScreenBackground().OpenSource(() =>
                {
                    UpdateMainScreenBackgroundButton();
                });
            }
        }
        
        private void ClickBySelectScreenBackgroundButton()
        {
            if (MainController.Instance.LocalSettings.GetSelectScreenBackground().IsExistSource())
            {
                MainController.Instance.LocalSettings.GetSelectScreenBackground().RemoveSource();
                UpdateSelectScreenBackgroundButton();
            }
            else
            {
                MainController.Instance.LocalSettings.GetSelectScreenBackground().OpenSource(() =>
                {
                    UpdateSelectScreenBackgroundButton();
                });
            }
        }
        
        private void ClickByGameScreenBackgroundButton()
        {
            if (MainController.Instance.LocalSettings.GetGameScreenBackground().IsExistSource())
            {
                MainController.Instance.LocalSettings.GetGameScreenBackground().RemoveSource();
                UpdateGameScreenBackgroundButton();
            }
            else
            {
                MainController.Instance.LocalSettings.GetGameScreenBackground().OpenSource(() =>
                {
                    UpdateGameScreenBackgroundButton();
                });
            }
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _editMicrophoneButton.gameObject)
            {
                OpenEditMicrophoneDialog();
            }
            else if (selectedObj == _openListLevelsButton.gameObject)
            {
                OpenListLevelsDialog();
            }
            else if (selectedObj == _openListSkinsButton.gameObject)
            {
                OpenListSkinsDialog();
            }
            else if (selectedObj == _mainScreenBackgroundButton.gameObject)
            {
                ClickByMainScreenBackgroundButton();
            }
            else if (selectedObj == _selectScreenBackgroundButton.gameObject)
            {
                ClickBySelectScreenBackgroundButton();
            }
            else if (selectedObj == _gameScreenBackgroundButton.gameObject)
            {
                ClickByGameScreenBackgroundButton();
            }

            return true;
        }
    }
}
