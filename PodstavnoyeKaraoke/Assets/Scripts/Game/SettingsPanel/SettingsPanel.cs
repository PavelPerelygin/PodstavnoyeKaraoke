using System.Collections.Generic;
using System.Linq;
using Boards;
using Controllers;
using Extensions;
using Managers.Audio;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.SettingsPanel
{
    public class SettingsPanel : Interactable
    {
        private const float VolumeLevelSmoothSpeed = 12f;

        private static readonly Color VolumeLevelMinColor = new Color(0.1f, 0.85f, 0.2f);
        private static readonly Color VolumeLevelMaxColor = new Color(1f, 0.1f, 0.05f);

        [SerializeField] private Button _changeScreenModeButton;
        [SerializeField] private Toggle _autoRecordToggle;
        [SerializeField] private Image _volumeLevel;
        [SerializeField] private Dropdown _microphoneNameDropdown;
        [SerializeField] private Slider _sensitivityMicrophoneSlider;
        [SerializeField] private Slider _trackVolumeSlider;
        [SerializeField] private Slider _recordVolumeSlider;

        private MainBoard _mainBoard;
        private RectTransform _volumeLevelRectTransform;
        private Vector2 _volumeLevelInitialAnchoredPosition;
        private float _volumeLevelMaxWidth;
        private float _displayedVolumeLevel;
        
        public void Init(MainBoard mainBoard)
        {
            _mainBoard = mainBoard;

            InitVolumeLevel();
            InitButton();
            InitToggle();
            InitSlider();
            InitDropdown();
        }

        protected override void Update()
        {
            base.Update();
            UpdateVolumeLevel();
        }

        private void InitVolumeLevel()
        {
            if (_volumeLevel == null)
                return;

            _volumeLevelRectTransform = _volumeLevel.rectTransform;
            _volumeLevelInitialAnchoredPosition = _volumeLevelRectTransform.anchoredPosition;
            _volumeLevelMaxWidth = 181;
            SetVolumeLevel(0f, true);
        }

        private void UpdateVolumeLevel()
        {
            if (_volumeLevel == null)
                return;

            if (_volumeLevelRectTransform == null || _volumeLevelMaxWidth <= 0f)
                InitVolumeLevel();

            if (MainController.Instance == null || MainController.Instance.MicrophoneController == null)
            {
                SetVolumeLevel(0f, false);
                return;
            }

            SetVolumeLevel(MainController.Instance.MicrophoneController.GetMicrophoneVolume(), false);
        }

        private void SetVolumeLevel(float normalizedVolume, bool instant)
        {
            normalizedVolume = Mathf.Clamp01(normalizedVolume);
            _displayedVolumeLevel = instant
                ? normalizedVolume
                : Mathf.Lerp(_displayedVolumeLevel, normalizedVolume, Mathf.Clamp01(Time.unscaledDeltaTime * VolumeLevelSmoothSpeed));

            SetVolumeLevelWidth(_volumeLevelMaxWidth * _displayedVolumeLevel);
            _volumeLevel.color = GetVolumeLevelColor(_displayedVolumeLevel);
        }

        private void SetVolumeLevelWidth(float width)
        {
            if (_volumeLevelRectTransform == null)
                return;

            var anchoredPosition = _volumeLevelInitialAnchoredPosition;
            anchoredPosition.x += _volumeLevelRectTransform.pivot.x * (width - _volumeLevelMaxWidth);

            _volumeLevelRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            _volumeLevelRectTransform.anchoredPosition = anchoredPosition;
        }

        private Color GetVolumeLevelColor(float normalizedVolume)
        {
            return Color.Lerp(VolumeLevelMinColor, VolumeLevelMaxColor, normalizedVolume);
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
            _sensitivityMicrophoneSlider.minValue = 0f;
            _sensitivityMicrophoneSlider.maxValue = 100f;
            _sensitivityMicrophoneSlider.SetValueWithoutNotify(MainController.Instance.LocalSettings.GetSensitivityMicrophone());
            _sensitivityMicrophoneSlider.onValueChanged.AddListener(SetSensitivityMicrophone);

            _trackVolumeSlider.SetValueWithoutNotify(MainController.Instance.LocalSettings.GetTrackVolume());
            _trackVolumeSlider.onValueChanged.AddListener(SetTrackVolume);

            _recordVolumeSlider.SetValueWithoutNotify(MainController.Instance.LocalSettings.GetRecordVolume());
            _recordVolumeSlider.onValueChanged.AddListener(SetRecordVolume);
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

        private void SetTrackVolume(float value)
        {
            value = Mathf.Clamp01(value);
            MainController.Instance.LocalSettings.SetTrackVolume(value);
            MainController.Instance.AudioManager.SetVolumeAuidoGroup(TypeGroup.Track, value);
        }

        private void SetRecordVolume(float value)
        {
            value = Mathf.Clamp01(value);
            MainController.Instance.LocalSettings.SetRecordVolume(value);
            MainController.Instance.AudioManager.SetVolumeAuidoGroup(TypeGroup.Record, value);
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
            
            return true;
        }
    }
}
