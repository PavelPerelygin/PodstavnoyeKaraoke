using Controllers;
using Controllers.Levels;
using Controllers.Skins;
using Extensions;
using Game.Common.Content;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Dialogs.ListSkins
{
    public class SkinItem : Interactable
    {
        [SerializeField] private Image _background;
        [SerializeField] private Toggle _enableToggle;
        [SerializeField] private Text _nameSkinText;
        [SerializeField] private Button _removeButton;
        
        private ListSkinsDialog _listSkinsDialog;
        
        public ContentsWrapping<SkinData> ContentsWrapping {get; private set;}

        public void Init(ListSkinsDialog listSkinsDialog, ContentsWrapping<SkinData> contentsWrapping)
        {
            _listSkinsDialog = listSkinsDialog;
            ContentsWrapping = contentsWrapping;
            
            InitText();
            InitToggle();
            InitButton();
            UpdateBackground();
        }

        private void InitText()
        {
            if (ContentsWrapping.GetContent().GetNameSkin() != SkinData.DefaultSkinName)
                _nameSkinText.text = ContentsWrapping.GetContent().GetNameSkin();
            else
                _nameSkinText.text = MainController.Instance.TextManager.GetText(547);

        }

        private void InitToggle()
        {
            var currentSkin = MainController.Instance.LocalSettings.GetSkinName();
            _enableToggle.isOn = ContentsWrapping.GetContent().GetNameSkin() == currentSkin;
            
            _enableToggle.DisableOverDownColors();
            _enableToggle.onValueChanged.AddListener(TogglePress);
        }

        private void InitButton()
        {
            if (ContentsWrapping.GetContent().GetNameSkin() != SkinData.DefaultSkinName)
            {
                _removeButton.gameObject.SetActive(true);
                _removeButton.onClick.AddListener(ButtonPress);
                _removeButton.DisableOverDownColors();   
            }
            else
            {
                _removeButton.gameObject.SetActive(false);
            }
        }
        
        private void UpdateBackground()
        {
            if (ContentsWrapping.IsValid())
            {
                _background.color = Color.white;
            }
            else
            {
                _background.color = ColorExtensions.ConvertHexToColor("#FF0000");
            }
        }

        private void OnEnableSkin()
        {
            var currentSkin = MainController.Instance.LocalSettings.GetSkinName();
            if (currentSkin == ContentsWrapping.GetContent().GetNameSkin())
            {
                _enableToggle.isOn = true;
                return;
            }
            
            _listSkinsDialog.EnableSkin(this);
        }

        public void DisableSkin()
        {
            _enableToggle.isOn = false;
        }

        private void Remove()
        {
            _listSkinsDialog.RemoveSkin(this);
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _enableToggle.gameObject)
            {
                OnEnableSkin();
            }
            else if (selectedObj == _removeButton.gameObject)
            {
                Remove();
            }
            
            return true;
        }
    }
}