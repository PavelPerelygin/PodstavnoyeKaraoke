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
        [SerializeField] private Button _button;
        [SerializeField] private Text _playerNameText;
        
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
            _button.onClick.AddListener(ButtonPress);
            _button.DisableOverDownColors();
        }

        private void OpenEditPlayerPanel()
        {
            _playersPanel.OpenEditPlayerPanel(PlayerData);
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;
            
            if(selectedObj == _button.gameObject)
            {
                OpenEditPlayerPanel();
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
