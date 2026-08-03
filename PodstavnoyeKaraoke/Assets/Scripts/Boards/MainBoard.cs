using Boards.Base;
using Controllers;
using Dialogs.Base;
using Game;
using Game.Common.Content;
using UnityEngine;

namespace Boards
{
    public class MainBoard : Board
    {
        [SerializeField] private TracksPanel _tracksPanel;
        
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
        }

        protected override void Show(bool smoothly, float delay)
        {
        }

        protected override void Hide(bool smoothly)
        {
        }
    }
}