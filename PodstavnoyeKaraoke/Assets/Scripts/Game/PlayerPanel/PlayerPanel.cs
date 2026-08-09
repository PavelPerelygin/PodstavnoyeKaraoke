using Boards;
using Controllers;
using Extensions;
using Managers.Audio;
using Managers.Settings.Local;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.PlayerPanel
{
    public class PlayerPanel : Interactable
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private Button _closeButton;
        [SerializeField] private InputField _namePlayerInputField;
        [SerializeField] private Button _removePlayerButton;
        [SerializeField] private Text _currentTrackName;
        [SerializeField] private Slider _currentTrackSlider;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _stopButtonButton;
        [SerializeField] private Button _recordButtonButton;
        [SerializeField] private Text _playTimeText;
        [SerializeField] private RecordsPanel.RecordsPanel _recordsPanel;
        
        private MainBoard _mainBoard;
        private PlayerData _playerData;
        private TrackData _currentTrackData;
        private AudioInfo _currentTrackAudioInfo;
        private bool _isUpdatingTrackSlider;
        private float _currentTrackProgress;

        public void Init(MainBoard mainBoard)
        {
            _mainBoard = mainBoard;
            
            InitButtons();
            InitInputField();
            InitCurrentTrackSlider();
            ResetCurrentTrackView();
        }

        private void InitButtons()
        {
            _closeButton.onClick.AddListener(ButtonPress);
            _closeButton.DisableOverDownColors();
            
            _playButton.onClick.AddListener(ButtonPress);
            _playButton.DisableOverDownColors();
            
            _pauseButton.onClick.AddListener(ButtonPress);
            _pauseButton.DisableOverDownColors();
            
            _stopButtonButton.onClick.AddListener(ButtonPress);
            _stopButtonButton.DisableOverDownColors();
            
            _recordButtonButton.onClick.AddListener(ButtonPress);
            _recordButtonButton.DisableOverDownColors();
        }

        private void InitInputField()
        {
            _namePlayerInputField.onValueChanged.AddListener((string value) =>
            {
                if(_playerData == null)
                    return;
                
                _playerData.SetNamePlayer(value);
            });
            _namePlayerInputField.DisableOverDownColors();
        }

        private void InitCurrentTrackSlider()
        {
            _currentTrackSlider.minValue = 0f;
            _currentTrackSlider.maxValue = 1f;
            _currentTrackSlider.onValueChanged.AddListener(OnCurrentTrackSliderValueChanged);
        }

        private void SetCurrentTrackName(string trackName)
        {
            _currentTrackName.text = $"{MainController.Instance.TextManager.GetText(1020)}{trackName}";
        }

        public void SetCurrentTrack(TrackData trackData)
        {
            if (_currentTrackData == trackData)
                return;

            StopCurrentTrack();

            if (_currentTrackData != null)
                _currentTrackData.OnChangeName -= OnCurrentTrackNameChanged;

            _currentTrackData = trackData;

            if (_currentTrackData != null)
                _currentTrackData.OnChangeName += OnCurrentTrackNameChanged;

            if (_currentTrackData != null && _currentTrackData.IsExist())
                EnsureCurrentTrackAudioInfo();

            UpdateCurrentTrackName();
            ResetCurrentTrackProgress();
        }

        private void PlayCurrentTrack()
        {
            if (_currentTrackData == null || !_currentTrackData.IsExist())
                return;

            EnsureCurrentTrackAudioInfo();

            if (_currentTrackAudioInfo == null)
                return;

            _currentTrackAudioInfo.Play(false);
            _currentTrackAudioInfo.SetProgress(_currentTrackProgress);
            UpdatePlayTimeText();
        }

        private void PauseCurrentTrack()
        {
            if (_currentTrackAudioInfo == null)
                return;

            _currentTrackAudioInfo.Pause();
            UpdatePlayTimeText();
        }

        private void StopCurrentTrack()
        {
            if (_currentTrackAudioInfo != null)
            {
                _currentTrackAudioInfo.Stop();
                _currentTrackAudioInfo.Remove();
                _currentTrackAudioInfo = null;
            }

            ResetCurrentTrackProgress();
        }

        private void EnsureCurrentTrackAudioInfo()
        {
            if (_currentTrackAudioInfo != null &&
                MainController.Instance.AudioManager.CheckContainsAudioInfo(_currentTrackAudioInfo))
                return;

            _currentTrackAudioInfo = MainController.Instance.AudioManager
                .Create(_currentTrackData.GetPathTrack(), TypeGroup.Track, true)
                .OnChangeProgress(OnCurrentTrackProgressChanged)
                .OnCompleted(OnCurrentTrackCompleted);
        }

        private void ResetCurrentTrackView()
        {
            UpdateCurrentTrackName();
            ResetCurrentTrackProgress();
        }

        private void ResetCurrentTrackProgress()
        {
            _currentTrackProgress = 0f;
            SetCurrentTrackSliderValue(0f);
            UpdatePlayTimeText();
        }

        private void UpdateCurrentTrackName()
        {
            SetCurrentTrackName(_currentTrackData == null ? "" : _currentTrackData.GetNameTrack());
        }

        private void UpdatePlayTimeText()
        {
            float duration = GetCurrentTrackDuration();
            float currentTime = duration * _currentTrackProgress;
            _playTimeText.text = $"{FormatTrackTime(currentTime)}/{FormatTrackTime(duration)}";
        }

        private float GetCurrentTrackDuration()
        {
            if (_currentTrackAudioInfo == null || _currentTrackAudioInfo.AudioClip == null)
                return 0f;

            return _currentTrackAudioInfo.GetAudioClipLenght();
        }

        private string FormatTrackTime(float time)
        {
            time = Mathf.Max(0f, time);

            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int milliseconds = Mathf.FloorToInt((time - Mathf.Floor(time)) * 100f);

            return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }

        private void SetCurrentTrackSliderValue(float value)
        {
            _isUpdatingTrackSlider = true;
            _currentTrackSlider.value = value;
            _isUpdatingTrackSlider = false;
        }

        private void OpenPlayersPanel()
        {
            _mainBoard.OpenPlayersPanel();
        }

        #region Show / hide

        public void Show(PlayerData playerData)
        {
            _root.gameObject.SetActive(true);
            
            _playerData = playerData;
            
            _namePlayerInputField.text = _playerData.GetNamePlayer();
            
            _recordsPanel.BuildRecords(playerData.GetRecords());
        }
        
        public void Hide()
        {
            _root.gameObject.SetActive(false);
        }

        #endregion

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _closeButton.gameObject)
            {
                OpenPlayersPanel();
            }
            else if (selectedObj == _playButton.gameObject)
            {
                PlayCurrentTrack();
            }
            else if (selectedObj == _pauseButton.gameObject)
            {
                PauseCurrentTrack();
            }
            else if (selectedObj == _stopButtonButton.gameObject)
            {
                StopCurrentTrack();
            }
            
            return true;
        }

        #region Events

        private void OnCurrentTrackSliderValueChanged(float value)
        {
            if (_isUpdatingTrackSlider)
                return;

            _currentTrackProgress = value;

            if (_currentTrackAudioInfo != null)
                _currentTrackAudioInfo.SetProgress(value);

            UpdatePlayTimeText();
        }

        private void OnCurrentTrackProgressChanged(float progress)
        {
            _currentTrackProgress = progress;
            SetCurrentTrackSliderValue(progress);
            UpdatePlayTimeText();
        }

        private void OnCurrentTrackCompleted()
        {
            _currentTrackAudioInfo = null;
            ResetCurrentTrackProgress();
        }

        private void OnCurrentTrackNameChanged()
        {
            UpdateCurrentTrackName();
        }

        private void OnDestroy()
        {
            if (_currentTrackData != null)
                _currentTrackData.OnChangeName -= OnCurrentTrackNameChanged;
        }

        #endregion
    }
}
