using Boards.Base;
using Controllers;
using Dialogs.Base;
using Game;
using Game.Common.Content;
using Game.PlayersPanel;
using Game.SettingsPanel;
using Game.TracksPanel;
using UnityEngine;

namespace Boards
{
    public class MainBoard : Board
    {
        [SerializeField] private TracksPanel _tracksPanel;
        [SerializeField] private PlayersPanel _playersPanel;
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

        public override void Init()
        {
            base.Init();
            
            _tracksPanel.Init(this);
            _playersPanel.Init(this);
            _settingsPanel.Init(this);
        }

        protected override void Show(bool smoothly, float delay)
        {
        }

        protected override void Hide(bool smoothly)
        {
        }
    }
}