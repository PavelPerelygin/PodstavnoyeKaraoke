using Boards.Base;
using Controllers;
using Dialogs.Base;
using Game;
using Game.Common.Content;
using Game.PlayerPanel;
using Game.PlayersPanel;
using Game.SettingsPanel;
using Game.TracksPanel;
using Managers.Settings.Local;
using UnityEngine;

namespace Boards
{
    public class MainBoard : Board
    {
        [SerializeField] private TracksPanel _tracksPanel;
        [SerializeField] private PlayersPanel _playersPanel;
        [SerializeField] private PlayerPanel _playerPanel;
        [SerializeField] private SettingsPanel _settingsPanel;
        
        protected override void OnEnableBoard()
        {

        }

        protected override void OnDisableBoard()
        {
            
        }

        public void OpenSettingsDialog()
        {
            if(_ignoreTimeLeft > 0)
                return;
            
            if(MainController.Instance.DialogsController.GetCountDialogs() > 0)
                return;
            
            var dialog = MainController.Instance.DialogsController.CreateDialog(TypeDialog.Settings);
            dialog.Init();
            dialog.Show();
        }

        public void OpenPlayersPanel()
        {
            _playerPanel.Hide();
            _playersPanel.Show();
        }

        public void OpenPlayerPanel(PlayerData playerData)
        {
            _playersPanel.Hide();
            _playerPanel.Show(playerData);
        }

        public void RemovePlayer(PlayerData playerData)
        {
            _playersPanel.RemovePlayer(playerData, OpenPlayersPanel);
        }

        public override void Init()
        {
            base.Init();
            
            _tracksPanel.Init(this);
            _playersPanel.Init(this);
            _playersPanel.Show();
            _playerPanel.Init(this);
            _playerPanel.Hide();
            _settingsPanel.Init(this);

            _tracksPanel.OnChangeSelectedTrack += OnChangeSelectedTrack;
            OnChangeSelectedTrack();
        }

        private void OnChangeSelectedTrack()
        {
            _playerPanel.SetCurrentTrack(_tracksPanel.SelectedTrack?.TrackData);
        }

        protected override void Show(bool smoothly, float delay)
        {
        }

        protected override void Hide(bool smoothly)
        {
        }
    }
}
