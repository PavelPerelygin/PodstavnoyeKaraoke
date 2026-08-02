using System.Collections.Generic;
using System.Linq;
using Controllers;
using Dialogs.Base;
using Extensions;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.EditMicrophone
{
    public class EditMicrophoneDialog : Dialog
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Dropdown _dropdown;
        [SerializeField] private Slider _sensitivityMicrophoneSlider;
        
        private AudioClip _audioClip;
        
        public override void Init()
        {
            InitButton();
            InitDropdown();
            InitSlider();
        }

        private void InitButton()
        {
            _closeButton.onClick.AddListener(ButtonPress);
            _closeButton.DisableOverDownColors();
        }

        private void InitDropdown()
        {
            var currentMicrophone = MainController.Instance.LocalSettings.GetMicrophoneName();
            var microphones = MainController.Instance.MicrophoneController.GetAvailableMicrophones().ToList();
            var emptyMicrophoneName = MainController.Instance.TextManager.GetText(524);
            var noMicrophonesText = MainController.Instance.TextManager.GetText(523);
            
            _dropdown.ClearOptions();

            if (microphones.Count > 0)
            {
                microphones.Add(emptyMicrophoneName);
                _dropdown.AddOptions(new List<string>(microphones));
                _dropdown.onValueChanged.AddListener(OnMicrophoneSelected);
                _dropdown.DisableOverDownColors();
            }
            else
            {
                _dropdown.AddOptions(new List<string> { noMicrophonesText });
                _dropdown.value = 0;
                return;
            }

            var selectIndex = string.IsNullOrEmpty(currentMicrophone)
                ? microphones.IndexOf(emptyMicrophoneName)
                : microphones.IndexOf(currentMicrophone);

            if(selectIndex < 0)
                selectIndex = microphones.Count - 1;
            
            _dropdown.value = selectIndex;
        }

        private void InitSlider()
        {
            _sensitivityMicrophoneSlider.onValueChanged.AddListener(SliderPress);
            _sensitivityMicrophoneSlider.value = MainController.Instance.LocalSettings.GetSensitivityMicrophone();
        }

        private void OnMicrophoneSelected(int index)
        {
            var microphoneName = _dropdown.options[index].text;
            if (microphoneName == MainController.Instance.TextManager.GetText(524))
                microphoneName = "";

            MainController.Instance.LocalSettings.SetMicrophoneName(microphoneName);
        }
        
        private void SetSensitivityMicrophone(float value)
        {
            MainController.Instance.LocalSettings.SetSensitivityMicrophone(value);
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;
            
            if (selectedObj == _sensitivityMicrophoneSlider.gameObject)
            {
                SetSensitivityMicrophone(_sensitivityMicrophoneSlider.value);
            }
            else if (selectedObj == _closeButton.gameObject)
            {
                Hide();
            }
            
            return true;
        }
    }
}
