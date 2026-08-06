using System;
using Extensions;
using Managers.Settings.Local;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.PlayersPanel
{
    public class PlayerItem : Interactable
    {
        [SerializeField] private Text _playerNameText;
        [SerializeField] private Button _editButton;
        [SerializeField] private Button _removeButton;
        
        private PlayersPanel _playersPanel;
        
        public PlayerData PlayerData { get; private set; }

        public void Init(PlayerData playerData, PlayersPanel playersPanel)
        {
            PlayerData = playerData;
            _playersPanel = playersPanel;

            InitButtons();
            
            PlayerData.OnChangeName += OnChangeName;
        }

        private void UpdatePlayerName()
        {
            _playerNameText.text = PlayerData.GetNamePlayer();
        }

        private void InitButtons()
        {
            _editButton.onClick.AddListener(ButtonPress);
            _editButton.DisableOverDownColors();
            
            _removeButton.onClick.AddListener(ButtonPress);
            _removeButton.DisableOverDownColors();
        }

        private void OpenEditPlayerPanel()
        {
            _playersPanel.OpenEditPlayerPanel(PlayerData);
        }

        private void RemovePlayer()
        {
            _playersPanel.RemovePlayer(this);
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;
            
            if(selectedObj == _editButton.gameObject)
            {
                OpenEditPlayerPanel();
            }
            else if(selectedObj == _removeButton.gameObject)
            {
                RemovePlayer();
            }

            return true;
        }

        #region Events

        private void OnChangeName()
        {
            UpdatePlayerName();
        }

        private void OnDestroy()
        {
            PlayerData.OnChangeName -= OnChangeName;
        }

        #endregion
    }
}