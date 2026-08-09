using Boards;
using Extensions;
using Managers.Settings.Local;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.PlayerPanel
{
    public class PlayerPanel : Interactable
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private Button _closeButton;
        [SerializeField] private InputField _namePlayerInputField;
        [SerializeField] private Button _removePlayerButton;
        [SerializeField] private RecordsPanel.RecordsPanel _recordsPanel;
        
        private MainBoard _mainBoard;
        private PlayerData _playerData;

        public void Init(MainBoard mainBoard)
        {
            _mainBoard = mainBoard;
            
            InitButtons();
            InitInputField();
        }

        private void InitButtons()
        {
            _closeButton.onClick.AddListener(ButtonPress);
            _closeButton.DisableOverDownColors();
        }

        private void InitInputField()
        {
            _namePlayerInputField.onValueChanged.AddListener((string value) =>
            {
                if(_playerData == null)
                    return;
                
                _playerData.SetNamePlayer(value);
            });
            _namePlayerInputField.DisableOverDownColors();
        }

        private void OpenPlayersPanel()
        {
            _mainBoard.OpenPlayersPanel();
        }

        #region Show / hide

        public void Show(PlayerData playerData)
        {
            _root.gameObject.SetActive(true);
            
            _playerData = playerData;
            
            _namePlayerInputField.text = _playerData.GetNamePlayer();
            
            _recordsPanel.BuildRecords(playerData.GetRecords());
        }
        
        public void Hide()
        {
            _root.gameObject.SetActive(false);
        }

        #endregion

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _closeButton.gameObject)
            {
                OpenPlayersPanel();
            }
            
            return true;
        }
    }
}