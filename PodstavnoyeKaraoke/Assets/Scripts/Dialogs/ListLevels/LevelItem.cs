using Controllers.Levels;
using Extensions;
using Game.Common.Content;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Dialogs.ListLevels
{
    public class LevelItem : Interactable
    {
        [SerializeField] private Image _background;
        [SerializeField] private Text _nameLevelText;
        [SerializeField] private Button _removeButton;
        
        private ListLevelsDialog _listLevelsDialog;
        
        public ContentsWrapping<LevelData> ContentsWrapping { get; private set; }
        
        public void Init(ListLevelsDialog listLevelsDialog,ContentsWrapping<LevelData> contentsWrapping)
        {
            _listLevelsDialog = listLevelsDialog;
            ContentsWrapping = contentsWrapping;

            _nameLevelText.text = ContentsWrapping.GetContent().GetNameLevel();
            
            InitButton();
            UpdateBackground();
        }

        private void InitButton()
        {
            _removeButton.onClick.AddListener(ButtonPress);
            _removeButton.DisableOverDownColors();
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

        private void RemoveLevel()
        {
            _listLevelsDialog.RemoveLevel(this);
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _removeButton.gameObject)
            {
                RemoveLevel();
            }
            
            return true;
        }
    }
}