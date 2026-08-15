using Controllers;
using Extensions;
using Managers.Settings.Local;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Files;

namespace Game.PlayerPanel.RecordsPanel
{
    public class RecordItem : Interactable
    {
        [SerializeField] private Text _nameRecordText;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _removeButton;
        [SerializeField] private Button _dowlandButton;
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _playTimeText;

        private RecordsPanel _recordsPanel;
        private bool _isUpdatingSlider;
        private float _duration;
        private float _progress;

        public float Progress => _progress;
        public RecordData RecordData {get; private set;}

        public void Init(RecordData recordData, RecordsPanel recordsPanel)
        {
            RecordData = recordData;
            _recordsPanel = recordsPanel;
            _nameRecordText.text = RecordData.GetRecordName();

            InitButtons();
            InitSlider();
            ResetProgress();
        }

        private void InitButtons()
        {
            _playButton.onClick.AddListener(ButtonPress);
            _playButton.DisableOverDownColors();

            _stopButton.onClick.AddListener(ButtonPress);
            _stopButton.DisableOverDownColors();

            _pauseButton.onClick.AddListener(ButtonPress);
            _pauseButton.DisableOverDownColors();

            _removeButton.onClick.AddListener(ButtonPress);
            _removeButton.DisableOverDownColors();

            _dowlandButton.onClick.AddListener(ButtonPress);
            _dowlandButton.DisableOverDownColors();

            SetPlaybackButtonsState(false);
        }

        private void InitSlider()
        {
            _slider.minValue = 0f;
            _slider.maxValue = 1f;
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        public void SetDuration(float duration)
        {
            _duration = Mathf.Max(0f, duration);
            UpdatePlayTimeText();
        }

        public void SetProgress(float progress)
        {
            _progress = progress;
            SetSliderValue(progress);
            UpdatePlayTimeText();
        }

        public void ResetProgress()
        {
            _progress = 0f;
            SetSliderValue(0f);
            UpdatePlayTimeText();
        }

        public void SetPlaybackButtonsState(bool isPlaying)
        {
            SetButtonAlpha(_playButton, isPlaying ? 0.5f : 1f);
            SetButtonAlpha(_pauseButton, isPlaying ? 1f : 0.5f);
        }

        private void DowlandRecord()
        {
            if (RecordData == null || !RecordData.IsExistRecord())
                return;

            File.SaveFile(TypeContent.Sound, MainController.Instance.TextManager.GetText(538), RecordData.GetPatchToRecord(), _recordsPanel.GetRecordSaveFileName(RecordData));
        }

        private void SetSliderValue(float value)
        {
            _isUpdatingSlider = true;
            _slider.value = value;
            _isUpdatingSlider = false;
        }

        private void SetButtonAlpha(Button button, float alpha)
        {
            if (button == null || button.targetGraphic == null)
                return;

            var color = button.targetGraphic.color;
            color.a = alpha;
            button.targetGraphic.color = color;
        }

        private void UpdatePlayTimeText()
        {
            float currentTime = _duration * _progress;
            _playTimeText.text = $"{FormatTime(currentTime)}/{FormatTime(_duration)}";
        }

        private string FormatTime(float time)
        {
            time = Mathf.Max(0f, time);

            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int milliseconds = Mathf.FloorToInt((time - Mathf.Floor(time)) * 100f);

            return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _playButton.gameObject)
            {
                _recordsPanel.PlayRecord(this);
            }
            else if (selectedObj == _pauseButton.gameObject)
            {
                _recordsPanel.PauseRecord(this);
            }
            else if (selectedObj == _stopButton.gameObject)
            {
                _recordsPanel.StopRecord(this);
            }
            else if (selectedObj == _removeButton.gameObject)
            {
                _recordsPanel.RemoveRecord(this);
            }
            else if (selectedObj == _dowlandButton.gameObject)
            {
                DowlandRecord();
            }

            return true;
        }

        private void OnSliderValueChanged(float value)
        {
            if (_isUpdatingSlider)
                return;

            _progress = value;
            _recordsPanel.SetRecordProgress(this, value);
            UpdatePlayTimeText();
        }
    }
}
