using Boards.Base;
using Controllers;
using Dialogs.Base;
using Game.Common.Content;

namespace Boards
{
    public class MainBoard : Board
    {
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
        }

        protected override void Show(bool smoothly, float delay)
        {
        }

        protected override void Hide(bool smoothly)
        {
        }
    }
}