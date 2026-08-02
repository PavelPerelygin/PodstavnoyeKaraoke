using Controllers;
using Dialogs.Base;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Network;

namespace Dialogs.SettingsDialog.Pages
{
	public class PageCabinet : BasePage
	{
		[SerializeField] private Text _versionLabel;
		[SerializeField] private Toggle _gameUpdateToggle;

		public override void Init()
		{
			InitLabels();
			InitToggles();
		}
		
		private void InitLabels()
		{
			_versionLabel.text = Application.version.ToString();
		}
		
		private void InitToggles()
		{
			_gameUpdateToggle.DisableOverDownColors();
			_gameUpdateToggle.isOn = MainController.Instance.UserSettings.GetNeedUpdateGame();
			_gameUpdateToggle.onValueChanged.AddListener(TogglePress);
		}

		protected override bool GameObjectClickHandler(GameObject selectedObj)
		{
			if (!base.GameObjectClickHandler(selectedObj))
				return false;
			
		
			if (selectedObj == _gameUpdateToggle.gameObject)
			{
				MainController.Instance.UserSettings.SetNeedUpdateGame(_gameUpdateToggle.isOn);
			}

			return true;
		}
	}
}
