using Controllers;
using Dialogs.Base;
using Extensions;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Dialogs.SettingsDialog.Pages
{
    public class PageSpecial : BasePage
    {
        public override void Init()
        {
            InitButtons();
            InitInputField();
        }

        private void InitButtons()
        {
        }

        private void InitInputField()
        {
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;

            return true;
        }
    }
}
