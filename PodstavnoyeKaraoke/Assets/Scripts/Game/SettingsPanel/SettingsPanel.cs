using System.Collections.Generic;
using System.Linq;
using Boards;
using Controllers;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.SettingsPanel
{
    public class SettingsPanel : Interactable
    {
        [SerializeField] private Button _changeScreenModeButton;
        [SerializeField] private Toggle _autoRecordToggle;
        [SerializeField] private Dropdown _microphoneNameDropdown;
        [SerializeField] private Slider _sensitivityMicrophoneSlider;

        private MainBoard _mainBoard;
        
        public void Init(MainBoard mainBoard)
        {
            _mainBoard = mainBoard;

            InitButton();
            InitToggle();
            InitSlider();
            InitDropdown();
        }

        private void InitButton()
        {
            _changeScreenModeButton.onClick.AddListener(ButtonPress);
            _changeScreenModeButton.DisableOverDownColors();
            UpdateScreenModeButton();
        }

        private void InitToggle()
        {
            _autoRecordToggle.DisableOverDownColors();
            _autoRecordToggle.isOn = MainController.Instance.LocalSettings.GetAutoRecord();
            _autoRecordToggle.onValueChanged.AddListener(TogglePress);
        }
        
        private void InitSlider()
        {
            _sensitivityMicrophoneSlider.onValueChanged.AddListener(SliderPress);
            _sensitivityMicrophoneSlider.value = MainController.Instance.LocalSettings.GetSensitivityMicrophone();
        }
        
        private void InitDropdown()
        {
            var currentMicrophone = MainController.Instance.LocalSettings.GetMicrophoneName();
            var microphones = MainController.Instance.MicrophoneController.GetAvailableMicrophones().ToList();
            var emptyMicrophoneName = MainController.Instance.TextManager.GetText(524);
            var noMicrophonesText = MainController.Instance.TextManager.GetText(523);
            
            _microphoneNameDropdown.ClearOptions();

            if (microphones.Count > 0)
            {
                microphones.Add(emptyMicrophoneName);
                _microphoneNameDropdown.AddOptions(new List<string>(microphones));
                _microphoneNameDropdown.onValueChanged.AddListener(OnMicrophoneSelected);
                _microphoneNameDropdown.DisableOverDownColors();
            }
            else
            {
                _microphoneNameDropdown.AddOptions(new List<string> { noMicrophonesText });
                _microphoneNameDropdown.value = 0;
                return;
            }

            var selectIndex = string.IsNullOrEmpty(currentMicrophone)
                ? microphones.IndexOf(emptyMicrophoneName)
                : microphones.IndexOf(currentMicrophone);

            if(selectIndex < 0)
                selectIndex = microphones.Count - 1;
            
            _microphoneNameDropdown.value = selectIndex;
        }
        
        private void OnMicrophoneSelected(int index)
        {
            var microphoneName = _microphoneNameDropdown.options[index].text;
            if (microphoneName == MainController.Instance.TextManager.GetText(524))
                microphoneName = "";

            MainController.Instance.LocalSettings.SetMicrophoneName(microphoneName);
        }
        
        private void SetSensitivityMicrophone(float value)
        {
            MainController.Instance.LocalSettings.SetSensitivityMicrophone(value);
        }
        
        private void UpdateScreenModeButton()
        {
            var value = "";
            if (MainController.Instance.ScreensController.IsFullScreen())
                value = MainController.Instance.TextManager.GetText(25);
            else
                value = MainController.Instance.TextManager.GetText(26);
        
            _changeScreenModeButton.SetText(value);
        }
        
        private void OnChangeFullScreen()
        {
            MainController.Instance.ScreensController.ChangeFullScreen();
            UpdateScreenModeButton();
        }

        private void OnChangeAutoRecord()
        {
            MainController.Instance.LocalSettings.SetAutoRecord(_autoRecordToggle.isOn);
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _changeScreenModeButton.gameObject)
            {
                OnChangeFullScreen();
            }
            else if (selectedObj == _autoRecordToggle.gameObject)
            {
                OnChangeAutoRecord();
            }
            else if (selectedObj == _sensitivityMicrophoneSlider.gameObject)
            {
                SetSensitivityMicrophone(_sensitivityMicrophoneSlider.value);
            }
            
            return true;
        }
    }
}