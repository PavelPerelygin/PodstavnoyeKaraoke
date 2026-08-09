using System;
using Dialogs;
using Dialogs.Base;
using UnityEngine;
using Utilities.Audio;
using FileUtility = Utilities.Files.File;

namespace Controllers
{
    public class MicrophoneController : MonoBehaviour
    {
        private const string ErrorLogFileName = "error.log";
        private const int MaxErrorLogLines = 2000;
        private const float RestartMicrophoneDelay = 3f;
        private const float StuckPositionDelay = 2f;
        private const int DefaultFrequency = 44100;
        private const int FallbackFrequency = 48000;
        private const int MaxRecordingLengthSeconds = 3599;

        private string _micName = "";
        private AudioClip _recordedClip;
        private AudioClip _recordingClip;
        private int _sampleWindow = 128;
        private float _nextRestartMicrophoneTime;
        private float _lastPositionChangedTime;
        private int _lastMicrophonePosition = -1;
        private bool _isRecording;

        public void Init()
        {
            MainController.Instance.LocalSettings.OnChangeMicrophoneName += OnChangeMicrophoneName;

            EnableMicrophone();
        }

        #region Check / ste / get

        public string[] GetAvailableMicrophones()
        {
            try
            {
                return Microphone.devices;
            }
            catch (Exception exception)
            {
                LogError("Failed to get available microphones.", exception);
                return new string[0];
            }
        }

        public float GetMicrophoneVolume()
        {
            try
            {
                if (string.IsNullOrEmpty(_micName))
                    return 0f;

                if (_isRecording)
                    return 0f;

                if (!Microphone.IsRecording(_micName))
                {
                    TryRestartMicrophone("Microphone recording stopped unexpectedly while reading volume.");
                    return 0f;
                }

                if (_recordedClip == null)
                {
                    TryRestartMicrophone("Recorded clip is null while reading volume.");
                    return 0f;
                }

                float num = 0f;
                float[] array = new float[_sampleWindow];
                int position = Microphone.GetPosition(_micName);
                CheckMicrophonePositionStuck(position);

                int num2 = position - (_sampleWindow + 1);
                if (num2 < 0)
                {
                    return 0f;
                }

                _recordedClip.GetData(array, num2);
                for (int i = 0; i < _sampleWindow; i++)
                {
                    float num3 = array[i] * array[i];
                    if (num < num3)
                    {
                        num = num3;
                    }
                }

                num *= MainController.Instance.LocalSettings.GetSensitivityMicrophone();

                return num;
            }
            catch (Exception exception)
            {
                TryRestartMicrophone("Failed to get microphone volume.", exception);
                return 0f;
            }
        }

        public bool CheckAvailableMicrophoneByName(string micName)
        {
            var devices = GetAvailableMicrophones();
            for (int i = 0; i < devices.Length; i++)
            {
                var device = devices[i];

                if (device == micName)
                    return true;
            }

            return false;
        }

        public bool CheckSelectionMicrophone()
        {
            var selectedMicrophone = true;

            var microphoneName = MainController.Instance.LocalSettings.GetMicrophoneName();
            if (microphoneName == "")
            {
                selectedMicrophone = false;
            }
            else
            {
                selectedMicrophone =
                    MainController.Instance.MicrophoneController.CheckAvailableMicrophoneByName(microphoneName);
            }

            if (selectedMicrophone)
                return true;

            var dialog =
                MainController.Instance.DialogsController.CreateDialog(TypeDialog.Warning) as WarningDialog;
            if (dialog != null)
            {
                dialog.Init(MainController.Instance.TextManager.GetText(544));
                dialog.Show(0.4f);
            }

            return false;
        }

        private bool CheckSelectedAvailableMicrophone()
        {
            var micName = MainController.Instance.LocalSettings.GetMicrophoneName();

            return CheckAvailableMicrophoneByName(micName);
        }

        public float GetMicrophonePosition()
        {
            try
            {
                if (string.IsNullOrEmpty(_micName))
                    return 0f;

                return Microphone.GetPosition(_micName);
            }
            catch (Exception exception)
            {
                TryRestartMicrophone("Failed to get microphone position.", exception);
                return 0f;
            }
        }

        #endregion

        #region Recording

        public bool IsRecording()
        {
            return _isRecording;
        }

        public bool StartRecording()
        {
            if (_isRecording)
                return true;

            if (!CheckSelectionMicrophone())
                return false;

            var micName = MainController.Instance.LocalSettings.GetMicrophoneName();
            if (string.IsNullOrEmpty(micName) || !CheckAvailableMicrophoneByName(micName))
                return false;

            DisableMicrophone();
            _micName = micName;
            _recordingClip = null;

            try
            {
                var frequency = GetSupportedFrequency(_micName);
                _recordingClip = Microphone.Start(_micName, false, MaxRecordingLengthSeconds, frequency);
                _isRecording = _recordingClip != null;

                if (!_isRecording)
                    LogError($"Microphone.Start returned null recording clip. Selected: '{_micName}'.");

                return _isRecording;
            }
            catch (Exception exception)
            {
                _recordingClip = null;
                _isRecording = false;
                LogError("Failed to start track recording.", exception);
                EnableMicrophone();
                return false;
            }
        }

        public string StopRecordingToStreamingAssets()
        {
            if (!_isRecording)
                return "";

            var localPath = "";

            try
            {
                int position = Microphone.GetPosition(_micName);
                Microphone.End(_micName);

                if (_recordingClip != null && position > 0)
                {
                    var samples = new float[position * _recordingClip.channels];
                    _recordingClip.GetData(samples, 0);

                    var clip = AudioClip.Create("Record", position, _recordingClip.channels, _recordingClip.frequency, false);
                    clip.SetData(samples, 0);

                    string unusedPath;
                    var bytes = WavUtility.FromAudioClip(clip, out unusedPath, false);
                    localPath = FileUtility.SaveBytesToStreamingAssets(bytes, ".wav");
                }
            }
            catch (Exception exception)
            {
                LogError("Failed to stop and save track recording.", exception);
            }

            _recordingClip = null;
            _isRecording = false;
            EnableMicrophone();

            return localPath;
        }

        #endregion

        private void EnableMicrophone()
        {
            if (_isRecording)
                return;

            DisableMicrophone();

            var micName = MainController.Instance.LocalSettings.GetMicrophoneName();

            if (string.IsNullOrEmpty(micName))
            {
                _micName = "";
                _recordedClip = null;
                return;
            }

            if (!CheckAvailableMicrophoneByName(micName))
            {
                LogError($"Selected microphone is not available. Selected: '{micName}'.");
                return;
            }

            _micName = micName;

            try
            {
                var frequency = GetSupportedFrequency(_micName);
                _recordedClip = Microphone.Start(_micName, true, 1, frequency);
                ResetPositionState();

                if (_recordedClip == null)
                    LogError($"Microphone.Start returned null clip. Selected: '{_micName}'.");
                else if (!Microphone.IsRecording(_micName))
                    LogError($"Microphone.Start returned clip, but recording is not active. Selected: '{_micName}'.");
            }
            catch (Exception exception)
            {
                _recordedClip = null;
                LogError("Failed to start microphone recording.", exception);
            }
        }

        private void DisableMicrophone()
        {
            try
            {
                if (string.IsNullOrEmpty(_micName))
                {
                    _recordedClip = null;
                    ResetPositionState();
                    return;
                }

                if (Microphone.IsRecording(_micName) || _recordedClip != null)
                {
                    Microphone.End(_micName);
                }

                _recordedClip = null;
                ResetPositionState();
            }
            catch (Exception exception)
            {
                _recordedClip = null;
                ResetPositionState();
                LogError("Failed to stop microphone recording.", exception);
            }
        }

        #region Events

        private void OnChangeMicrophoneName()
        {
            EnableMicrophone();
        }

        #endregion

        #region Log / Assert

        private void Log(string message)
        {
            Utilities.Log.Message($"[MicrophoneController] {message}");
        }

        private void Assert(string message = "Assert!")
        {
            Utilities.Log.Assert($"[MicrophoneController] {message}");
        }

        private void LogError(string message, Exception exception = null)
        {
            var logMessage = $"[MicrophoneController] {message}";
            Debug.LogError(logMessage);
            WriteErrorToFile(logMessage, exception);
        }

        private void TryRestartMicrophone(string reason, Exception exception = null)
        {
            if (Time.unscaledTime < _nextRestartMicrophoneTime)
                return;

            _nextRestartMicrophoneTime = Time.unscaledTime + RestartMicrophoneDelay;
            LogError($"{reason} Trying to restart microphone.", exception);
            EnableMicrophone();
        }

        private void CheckMicrophonePositionStuck(int position)
        {
            if (position != _lastMicrophonePosition)
            {
                _lastMicrophonePosition = position;
                _lastPositionChangedTime = Time.unscaledTime;
                return;
            }

            if (Time.unscaledTime - _lastPositionChangedTime < StuckPositionDelay)
                return;

            TryRestartMicrophone($"Microphone position stuck at {position}.");
        }

        private void ResetPositionState()
        {
            _lastMicrophonePosition = -1;
            _lastPositionChangedTime = Time.unscaledTime;
        }

        private int GetSupportedFrequency(string micName)
        {
            try
            {
                Microphone.GetDeviceCaps(micName, out var minFrequency, out var maxFrequency);

                if (minFrequency == 0 && maxFrequency == 0)
                    return DefaultFrequency;

                if (IsFrequencySupported(DefaultFrequency, minFrequency, maxFrequency))
                    return DefaultFrequency;

                if (IsFrequencySupported(FallbackFrequency, minFrequency, maxFrequency))
                    return FallbackFrequency;

                var frequency = maxFrequency > 0 ? maxFrequency : minFrequency;
                Log($"Default microphone frequencies are not supported. Using device frequency: {frequency}. Device caps: min={minFrequency}, max={maxFrequency}.");
                return frequency;
            }
            catch (Exception exception)
            {
                LogError("Failed to get microphone device caps. Using default frequency.", exception);
                return DefaultFrequency;
            }
        }

        private bool IsFrequencySupported(int frequency, int minFrequency, int maxFrequency)
        {
            return frequency >= minFrequency && frequency <= maxFrequency;
        }

        private void WriteErrorToFile(string message, Exception exception)
        {
            try
            {
                var path = FileUtility.PathCombine(FileUtility.GetPathToStreamingAssets(), ErrorLogFileName);
                var text = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}{Environment.NewLine}" +
                           $"Selected microphone: '{_micName}'{Environment.NewLine}" +
                           $"Available microphones: {GetAvailableMicrophonesForLog()}{Environment.NewLine}" +
                           $"Device caps: {GetDeviceCapsForLog(_micName)}{Environment.NewLine}" +
                           $"Is recording: {GetIsRecordingForLog(_micName)}{Environment.NewLine}";

                if (exception != null)
                    text += exception + Environment.NewLine;

                text += Environment.NewLine;

                System.IO.File.AppendAllText(path, text);
                TrimErrorLogIfNeeded(path);
            }
            catch (Exception fileException)
            {
                Debug.LogError($"[MicrophoneController] Failed to write microphone error log: {fileException}");
            }
        }

        private void TrimErrorLogIfNeeded(string path)
        {
            if (!System.IO.File.Exists(path))
                return;

            var lines = System.IO.File.ReadAllLines(path);
            if (lines.Length <= MaxErrorLogLines)
                return;

            var trimCount = lines.Length - MaxErrorLogLines;
            var trimmedLines = new string[MaxErrorLogLines];
            Array.Copy(lines, trimCount, trimmedLines, 0, MaxErrorLogLines);

            System.IO.File.WriteAllLines(path, trimmedLines);
        }

        private string GetAvailableMicrophonesForLog()
        {
            try
            {
                return string.Join(", ", Microphone.devices);
            }
            catch (Exception exception)
            {
                return $"Failed to get microphone list: {exception}";
            }
        }

        private string GetDeviceCapsForLog(string micName)
        {
            if (string.IsNullOrEmpty(micName))
                return "Microphone is not selected.";

            try
            {
                Microphone.GetDeviceCaps(micName, out var minFrequency, out var maxFrequency);
                return $"min={minFrequency}, max={maxFrequency}";
            }
            catch (Exception exception)
            {
                return $"Failed to get device caps: {exception}";
            }
        }

        private string GetIsRecordingForLog(string micName)
        {
            if (string.IsNullOrEmpty(micName))
                return "false";

            try
            {
                return Microphone.IsRecording(micName).ToString();
            }
            catch (Exception exception)
            {
                return $"Failed to get recording state: {exception}";
            }
        }

        #endregion
    }
}
