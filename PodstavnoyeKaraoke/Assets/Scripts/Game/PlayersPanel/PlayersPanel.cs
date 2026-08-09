using System;
using System.Collections.Generic;
using Boards;
using Controllers;
using Extensions;
using Managers.Settings.Local;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.PlayersPanel
{
    public class PlayersPanel : Interactable
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _addPlayerButton;
        [SerializeField] private PlayerItem _playerItemPrefab;

        private MainBoard _mainBoard;
        private List<PlayerItem> _players = new List<PlayerItem>();

        public void Init(MainBoard mainBoard)
        {
            _mainBoard = mainBoard;

            InitButtons();

            BuildPlayerItems();
        }

        private void InitButtons()
        {
            _addPlayerButton.onClick.AddListener(ButtonPress);
            _addPlayerButton.DisableOverDownColors();
        }

        private void BuildPlayerItems()
        {
            var players = MainController.Instance.LocalSettings.GetPlayers();

            for (int i = 0; i < players.Count; i++)
            {
                CreatePlayerItem(players[i]);
            }
        }

        private void CreatePlayerItem(PlayerData playerData)
        {
            var item = Instantiate(_playerItemPrefab, _scrollRect.content);
            item.Init(playerData, this);

            item.transform.SetSiblingIndex(_scrollRect.content.childCount - 2);

            _players.Add(item);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
        }

        public void RemovePlayer(PlayerItem playerItem)
        {
            if (playerItem == null)
                return;

            RemovePlayer(playerItem.PlayerData);
        }

        public void RemovePlayer(PlayerData playerData, Action onRemoved = null)
        {
            if (playerData == null)
                return;

            var text = $"{MainController.Instance.TextManager.GetText(1009)} \"{playerData.GetNamePlayer()}\"";

            MainController.Instance.DialogsController.OpenConfirmDialog(text, () =>
            {
                MainController.Instance.LocalSettings.RemovePlayer(playerData);

                var playerItem = GetPlayerItem(playerData);
                if (_players.Contains(playerItem))
                    _players.Remove(playerItem);

                if (playerItem != null)
                    Destroy(playerItem.gameObject);

                onRemoved?.Invoke();
            }, null);
        }

        private PlayerItem GetPlayerItem(PlayerData playerData)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].PlayerData == playerData)
                    return _players[i];
            }

            return null;
        }

        public void OpenEditPlayerPanel(PlayerData playerData)
        {
            _mainBoard.OpenPlayerPanel(playerData);
        }

        private void AddPlayer()
        {
            var data = MainController.Instance.LocalSettings.CreatePlayer();
            CreatePlayerItem(data);

            _scrollRect.verticalNormalizedPosition = 0;
        }

        #region Show / hide

        public void Show()
        {
            _root.gameObject.SetActive(true);
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

            if (selectedObj == _addPlayerButton.gameObject)
            {
                AddPlayer();
            }

            return true;
        }
    }
}
