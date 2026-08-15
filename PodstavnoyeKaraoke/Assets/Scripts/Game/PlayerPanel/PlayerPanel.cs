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
        [SerializeField] private Text _recordTimeText;
        [SerializeField] private RecordsPanel.RecordsPanel _recordsPanel;
        
        private MainBoard _mainBoard;
        private PlayerData _playerData;
        private TrackData _currentTrackData;
        private AudioInfo _currentTrackAudioInfo;
        private bool _isUpdatingTrackSlider;
        private float _currentTrackProgress;
        private bool _isRecording;
        private float _recordStartTime;

        public void Init(MainBoard mainBoard)
        {
            _mainBoard = mainBoard;
            
            InitButtons();
            InitInputField();
            InitCurrentTrackSlider();
            _recordsPanel.Init(this);
            ResetCurrentTrackView();
            ResetRecordTimeText();
        }

        private void InitButtons()
        {
            _closeButton.onClick.AddListener(ButtonPress);
            _closeButton.DisableOverDownColors();

            _removePlayerButton.onClick.AddListener(ButtonPress);
            _removePlayerButton.DisableOverDownColors();
            
            _playButton.onClick.AddListener(ButtonPress);
            _playButton.DisableOverDownColors();
            
            _pauseButton.onClick.AddListener(ButtonPress);
            _pauseButton.DisableOverDownColors();
            
            _stopButtonButton.onClick.AddListener(ButtonPress);
            _stopButtonButton.DisableOverDownColors();
            
            _recordButtonButton.onClick.AddListener(ButtonPress);
            _recordButtonButton.DisableOverDownColors();

            SetPlaybackButtonsState(false);
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

            Log($"SetCurrentTrack requested. Previous: {GetTrackDataInfoForLog(_currentTrackData)}. New: {GetTrackDataInfoForLog(trackData)}. Panel state: {GetPanelStateForLog()}");
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
            Log($"PlayCurrentTrack requested. State before: {GetPanelStateForLog()}");

            if (_currentTrackData == null || !_currentTrackData.IsExist())
            {
                Log($"PlayCurrentTrack skipped because current track is missing or does not exist. Track: {GetTrackDataInfoForLog(_currentTrackData)}");
                return;
            }

            _recordsPanel.StopPlayingRecord();
            EnsureCurrentTrackAudioInfo();

            if (_currentTrackAudioInfo == null)
            {
                Log("PlayCurrentTrack skipped because AudioInfo is null after EnsureCurrentTrackAudioInfo.");
                return;
            }

            _currentTrackAudioInfo.Play(false);
            _currentTrackAudioInfo.SetProgress(_currentTrackProgress);
            SetPlaybackButtonsState(true);
            UpdatePlayTimeText();

            if (MainController.Instance.LocalSettings.GetAutoRecord())
                StartCurrentRecording();

            Log($"PlayCurrentTrack completed. State after: {GetPanelStateForLog()}");
        }

        private void PauseCurrentTrack()
        {
            Log($"PauseCurrentTrack requested. State before: {GetPanelStateForLog()}");
            StopCurrentRecording();

            if (_currentTrackAudioInfo == null)
            {
                Log("PauseCurrentTrack stopped after StopCurrentRecording because current track AudioInfo is null.");
                return;
            }

            _currentTrackAudioInfo.Pause();
            SetPlaybackButtonsState(false);
            UpdatePlayTimeText();
            Log($"PauseCurrentTrack completed. State after: {GetPanelStateForLog()}");
        }

        public void StopCurrentTrack()
        {
            Log($"StopCurrentTrack requested. State before: {GetPanelStateForLog()}");
            StopCurrentRecording();

            if (_currentTrackAudioInfo != null)
            {
                _currentTrackAudioInfo.Stop();
                _currentTrackAudioInfo.Remove();
                _currentTrackAudioInfo = null;
            }

            ResetCurrentTrackProgress();
            SetPlaybackButtonsState(false);
            Log($"StopCurrentTrack completed. State after: {GetPanelStateForLog()}");
        }

        public void RemoveRecord(RecordData recordData)
        {
            if (_playerData == null || recordData == null)
                return;

            _playerData.RemoveRecord(recordData);
        }

        private void EnsureCurrentTrackAudioInfo()
        {
            if (_currentTrackAudioInfo != null &&
                MainController.Instance.AudioManager.CheckContainsAudioInfo(_currentTrackAudioInfo))
            {
                Log($"EnsureCurrentTrackAudioInfo reused existing AudioInfo. State: {GetPanelStateForLog()}");
                return;
            }

            Log($"EnsureCurrentTrackAudioInfo creating AudioInfo for track: {GetTrackDataInfoForLog(_currentTrackData)}");
            _currentTrackAudioInfo = MainController.Instance.AudioManager
                .Create(_currentTrackData.GetPathTrack(), TypeGroup.Track, true)
                .OnChangeProgress(OnCurrentTrackProgressChanged)
                .OnCompleted(OnCurrentTrackCompleted);
            Log($"EnsureCurrentTrackAudioInfo created AudioInfo. State: {GetPanelStateForLog()}");
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

        private void SetPlaybackButtonsState(bool isPlaying)
        {
            SetButtonAlpha(_playButton, isPlaying ? 0.5f : 1f);
            SetButtonAlpha(_pauseButton, isPlaying ? 1f : 0.5f);
        }

        private void SetButtonAlpha(Button button, float alpha)
        {
            if (button == null || button.targetGraphic == null)
                return;

            var color = button.targetGraphic.color;
            color.a = alpha;
            button.targetGraphic.color = color;
        }

        private void StartCurrentRecording()
        {
            Log($"StartCurrentRecording requested. State before: {GetPanelStateForLog()}");

            if (_isRecording)
            {
                Log("StartCurrentRecording skipped because panel recording flag is already true.");
                return;
            }

            if (_playerData == null || _currentTrackData == null)
            {
                Log($"StartCurrentRecording skipped because player or track is null. Player null: {_playerData == null}. Track: {GetTrackDataInfoForLog(_currentTrackData)}");
                return;
            }

            if (!MainController.Instance.MicrophoneController.StartRecording())
            {
                Log("StartCurrentRecording failed because MicrophoneController.StartRecording returned false.");
                return;
            }

            _isRecording = true;
            _recordStartTime = Time.time;
            UpdateRecordTimeText();
            Log($"StartCurrentRecording completed. State after: {GetPanelStateForLog()}");
        }

        private void StopCurrentRecording()
        {
            Log($"StopCurrentRecording requested. State before: {GetPanelStateForLog()}");

            if (!_isRecording)
            {
                Log("StopCurrentRecording skipped because panel recording flag is false.");
                return;
            }

            _isRecording = false;

            var patchToRecord = MainController.Instance.MicrophoneController.StopRecordingToStreamingAssets();
            Log($"StopCurrentRecording got recording path: '{patchToRecord}'. Player null: {_playerData == null}. Track null: {_currentTrackData == null}.");
            if (!string.IsNullOrEmpty(patchToRecord) && _playerData != null && _currentTrackData != null)
            {
                var recordName = GetUniqueRecordName(_currentTrackData.GetNameTrack());
                var recordData = _playerData.AddRecord(recordName, patchToRecord);
                _recordsPanel.AddRecord(recordData);
                Log($"StopCurrentRecording added record. Name: '{recordName}'. Path: '{patchToRecord}'.");
            }
            else
            {
                Log($"StopCurrentRecording did not add record. Empty path: {string.IsNullOrEmpty(patchToRecord)}. Player null: {_playerData == null}. Track null: {_currentTrackData == null}.");
            }

            ResetRecordTimeText();
            Log($"StopCurrentRecording completed. State after: {GetPanelStateForLog()}");
        }

        private string GetUniqueRecordName(string baseName)
        {
            var index = 1;
            var result = $"{baseName} ({index})";

            while (CheckRecordNameExists(result))
            {
                index++;
                result = $"{baseName} ({index})";
            }

            return result;
        }

        public string GetRecordSaveFileName(RecordData recordData)
        {
            if (_playerData == null || recordData == null)
                return "";

            var playerName = _playerData.GetNamePlayer();
            var recordName = GetRecordNameWithIndex(recordData.GetRecordName());

            if (recordName.StartsWith($"{playerName} - "))
                return recordName;

            return $"{playerName} - {recordName}";
        }

        private string GetRecordNameWithIndex(string recordName)
        {
            if (string.IsNullOrEmpty(recordName))
                return "(1)";

            if (recordName.EndsWith(")"))
                return recordName;

            return $"{recordName} (1)";
        }

        private bool CheckRecordNameExists(string recordName)
        {
            var records = _playerData.GetRecords();
            for (int i = 0; i < records.Count; i++)
            {
                if (records[i].GetRecordName() == recordName)
                    return true;
            }

            return false;
        }

        private void UpdateRecordTimeText()
        {
            var recordTime = _isRecording ? Time.time - _recordStartTime : 0f;
            _recordTimeText.text = $"{MainController.Instance.TextManager.GetText(1021)}{FormatTrackTime(recordTime)}";
        }

        private void ResetRecordTimeText()
        {
            _recordTimeText.text = $"{MainController.Instance.TextManager.GetText(1021)}{FormatTrackTime(0f)}";
        }

        private void OpenPlayersPanel()
        {
            _mainBoard.OpenPlayersPanel();
        }

        private void RemoveCurrentPlayer()
        {
            if (_playerData == null)
                return;

            _mainBoard.RemovePlayer(_playerData);
        }

        #region Show / hide

        public void Show(PlayerData playerData)
        {
            Log($"Show requested. Player null: {playerData == null}. State before: {GetPanelStateForLog()}");
            _root.gameObject.SetActive(true);
            
            _playerData = playerData;
            
            _namePlayerInputField.text = _playerData.GetNamePlayer();
            
            _recordsPanel.BuildRecords(playerData.GetRecords());
        }
        
        public void Hide()
        {
            Log($"Hide requested. State before: {GetPanelStateForLog()}");
            StopCurrentTrack();
            _recordsPanel.StopPlayingRecord();
            _root.gameObject.SetActive(false);
            Log($"Hide completed. State after: {GetPanelStateForLog()}");
        }

        #endregion

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _closeButton.gameObject)
            {
                Log("Button clicked: close.");
                OpenPlayersPanel();
            }
            else if (selectedObj == _removePlayerButton.gameObject)
            {
                Log("Button clicked: remove player.");
                RemoveCurrentPlayer();
            }
            else if (selectedObj == _playButton.gameObject)
            {
                Log("Button clicked: play.");
                PlayCurrentTrack();
            }
            else if (selectedObj == _pauseButton.gameObject)
            {
                Log("Button clicked: pause.");
                PauseCurrentTrack();
            }
            else if (selectedObj == _stopButtonButton.gameObject)
            {
                Log("Button clicked: stop.");
                StopCurrentTrack();
            }
            else if (selectedObj == _recordButtonButton.gameObject)
            {
                Log("Button clicked: record.");
                StartCurrentRecording();
            }
            
            return true;
        }

        #region Events

        private void OnCurrentTrackSliderValueChanged(float value)
        {
            if (_isUpdatingTrackSlider)
                return;

            Log($"Track slider changed by user. Value: {value:0.000}. State before: {GetPanelStateForLog()}");
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
            Log($"OnCurrentTrackCompleted invoked. State before: {GetPanelStateForLog()}");
            StopCurrentRecording();
            _currentTrackAudioInfo = null;
            ResetCurrentTrackProgress();
            SetPlaybackButtonsState(false);
            Log($"OnCurrentTrackCompleted completed. State after: {GetPanelStateForLog()}");
        }

        private void OnCurrentTrackNameChanged()
        {
            UpdateCurrentTrackName();
        }

        private void OnDestroy()
        {
            Log($"OnDestroy invoked. State before: {GetPanelStateForLog()}");
            StopCurrentRecording();
            _recordsPanel.StopPlayingRecord();

            if (_currentTrackData != null)
                _currentTrackData.OnChangeName -= OnCurrentTrackNameChanged;
        }

        #endregion

        protected override void Update()
        {
            base.Update();

            if (_isRecording)
                UpdateRecordTimeText();
        }

        private void Log(string message)
        {
            Utilities.Log.Message($"[PlayerPanel] {message}");
        }

        private string GetPanelStateForLog()
        {
            var audioInfoState = _currentTrackAudioInfo == null ? "null" : _currentTrackAudioInfo.GetDebugState();
            return $"panelRecording={_isRecording}, recordElapsed={(Time.time - _recordStartTime):0.000}, progress={_currentTrackProgress:0.000}, " +
                   $"playerNull={_playerData == null}, track={GetTrackDataInfoForLog(_currentTrackData)}, audioInfo={audioInfoState}";
        }

        private string GetTrackDataInfoForLog(TrackData trackData)
        {
            if (trackData == null)
                return "null";

            return $"name='{trackData.GetNameTrack()}', path='{trackData.GetPathTrack()}', exists={trackData.IsExist()}";
        }
    }
}
