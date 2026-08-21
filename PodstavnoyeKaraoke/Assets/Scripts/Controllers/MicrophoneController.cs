using System;
using System.Collections.Generic;
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
        private const float MonitoringDiagnosticsInterval = 3f;
        private const int DefaultFrequency = 44100;
        private const int FallbackFrequency = 48000;
        private const int MaxRecordingLengthSeconds = 3599;

        private string _micName = "";
        private AudioClip _recordedClip;
        private AudioClip _recordingClip;
        private int _sampleWindow = 128;
        private float _nextRestartMicrophoneTime;
        private float _nextMonitoringDiagnosticsTime;
        private float _lastPositionChangedTime;
        private int _lastMicrophonePosition = -1;
        private int _lastRecordingPosition;
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

                if (!Microphone.IsRecording(_micName))
                {
                    if (!_isRecording)
                        TryRestartMicrophone("Microphone recording stopped unexpectedly while reading volume.");
                    return 0f;
                }

                var clip = _isRecording ? _recordingClip : _recordedClip;
                if (clip == null)
                {
                    if (!_isRecording)
                        TryRestartMicrophone("Recorded clip is null while reading volume.");
                    return 0f;
                }

                float rawPeak = 0f;
                float[] array = new float[_sampleWindow * clip.channels];
                int position = Microphone.GetPosition(_micName);
                if (_isRecording)
                    _lastRecordingPosition = Mathf.Max(_lastRecordingPosition, position);
                else
                    CheckMicrophonePositionStuck(position);

                int num2 = position - (_sampleWindow + 1);
                if (num2 < 0)
                {
                    LogMonitoringSampleStatsIfNeeded(position, clip, 0f, MainController.Instance.LocalSettings.GetSensitivityMicrophone(), 0f, "waiting for enough samples");
                    return 0f;
                }

                clip.GetData(array, num2);
                for (int i = 0; i < array.Length; i++)
                {
                    float num3 = Mathf.Abs(array[i]);
                    if (rawPeak < num3)
                    {
                        rawPeak = num3;
                    }
                }

                var sensitivity = MainController.Instance.LocalSettings.GetSensitivityMicrophone();
                var num = rawPeak * sensitivity;

                LogMonitoringSampleStatsIfNeeded(position, clip, rawPeak, sensitivity, num, "samples read");

                return Mathf.Clamp01(num);
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
                dialog.Init(MainController.Instance.TextManager.GetText(1024));
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
            Log($"StartRecording requested. State before: {GetRecordingStateForLog()}");

            if (_isRecording)
            {
                Log("StartRecording skipped because track recording is already active.");
                return true;
            }

            if (!CheckSelectionMicrophone())
            {
                Log("StartRecording failed because microphone selection check failed.");
                return false;
            }

            var micName = MainController.Instance.LocalSettings.GetMicrophoneName();
            if (string.IsNullOrEmpty(micName) || !CheckAvailableMicrophoneByName(micName))
            {
                Log($"StartRecording failed because selected microphone is unavailable. Requested: '{micName}'. Available: {GetAvailableMicrophonesForLog()}");
                return false;
            }

            DisableMicrophone();
            _micName = micName;
            _recordingClip = null;

            try
            {
                var frequency = 0;
                _recordingClip = TryStartMicrophoneClip(_micName, false, MaxRecordingLengthSeconds, out frequency);
                _isRecording = _recordingClip != null;
                _lastRecordingPosition = 0;

                if (!_isRecording)
                    LogError($"Microphone.Start returned null recording clip. Selected: '{_micName}'.");
                else
                    Log($"StartRecording succeeded. Frequency: {frequency}. Recording clip: {GetClipInfoForLog(_recordingClip)}. State after: {GetRecordingStateForLog()}");

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
            Log($"StopRecordingToStreamingAssets requested. State before: {GetRecordingStateForLog()}");

            if (!_isRecording)
            {
                Log("StopRecordingToStreamingAssets skipped because track recording flag is false.");
                return "";
            }

            var localPath = "";

            try
            {
                int position = GetRecordingPositionForStop();
                var actualPosition = GetActualRecordingPosition(position);
                Log($"StopRecordingToStreamingAssets microphone position before End: {position}. State: {GetRecordingStateForLog()}");
                Microphone.End(_micName);
                Log($"StopRecordingToStreamingAssets called Microphone.End. Recording clip: {GetClipInfoForLog(_recordingClip)}");

                if (_recordingClip != null && actualPosition > 0)
                {
                    var samples = new float[actualPosition * _recordingClip.channels];
                    Log($"StopRecordingToStreamingAssets reading samples. Position: {actualPosition}. Last raw position: {position}. Channels: {_recordingClip.channels}. Samples array length: {samples.Length}.");
                    _recordingClip.GetData(samples, 0);
                    Log($"StopRecordingToStreamingAssets raw sample stats: {GetSampleStatsForLog(samples)}.");
                    ApplySensitivityToSamples(samples);
                    Log($"StopRecordingToStreamingAssets samples after sensitivity: {GetSampleStatsForLog(samples)}.");

                    var clip = AudioClip.Create("Record", actualPosition, _recordingClip.channels, _recordingClip.frequency, false);
                    clip.SetData(samples, 0);

                    string unusedPath;
                    var bytes = WavUtility.FromAudioClip(clip, out unusedPath, false);
                    localPath = FileUtility.SaveBytesToStreamingAssets(bytes, ".wav");
                    Log($"StopRecordingToStreamingAssets saved recording. Bytes: {bytes.Length}. Expected wav data bytes: {samples.Length * 2}. Local path: '{localPath}'.");
                }
                else
                {
                    LogError($"StopRecordingToStreamingAssets did not save recording. Recording clip null: {_recordingClip == null}. Position: {position}. Last recording position: {_lastRecordingPosition}. State: {GetRecordingStateForLog()}");
                }
            }
            catch (Exception exception)
            {
                LogError("Failed to stop and save track recording.", exception);
            }

            _recordingClip = null;
            _isRecording = false;
            _lastRecordingPosition = 0;
            EnableMicrophone();

            Log($"StopRecordingToStreamingAssets completed. Result path: '{localPath}'. State after: {GetRecordingStateForLog()}");
            return localPath;
        }

        private void ApplySensitivityToSamples(float[] samples)
        {
            var sensitivity = MainController.Instance.LocalSettings.GetSensitivityMicrophone();
            var multiplier = Mathf.InverseLerp(0f, 100f, sensitivity);

            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = Mathf.Clamp(samples[i] * multiplier, -1f, 1f);
            }
        }

        private int GetRecordingPositionForStop()
        {
            try
            {
                return Microphone.GetPosition(_micName);
            }
            catch (Exception exception)
            {
                LogError($"Failed to get microphone position before stopping. Last recording position: {_lastRecordingPosition}.", exception);
                return 0;
            }
        }

        private int GetActualRecordingPosition(int position)
        {
            if (position > 0)
            {
                _lastRecordingPosition = position;
                return position;
            }

            if (_lastRecordingPosition > 0)
            {
                Log($"Using last known recording position because Microphone.GetPosition returned 0. Last recording position: {_lastRecordingPosition}.");
                return _lastRecordingPosition;
            }

            return 0;
        }

        private void UpdateRecordingPosition()
        {
            if (!_isRecording || string.IsNullOrEmpty(_micName))
                return;

            try
            {
                var position = Microphone.GetPosition(_micName);
                if (position > 0)
                    _lastRecordingPosition = position;
            }
            catch (Exception exception)
            {
                LogError("Failed to update track recording position.", exception);
            }
        }

        #endregion

        private void EnableMicrophone()
        {
            if (_isRecording)
            {
                Log("EnableMicrophone skipped because track recording is active.");
                return;
            }

            Log($"EnableMicrophone requested. State before: {GetRecordingStateForLog()}");
            DisableMicrophone();

            var micName = MainController.Instance.LocalSettings.GetMicrophoneName();

            if (string.IsNullOrEmpty(micName))
            {
                _micName = "";
                _recordedClip = null;
                Log("EnableMicrophone skipped because no microphone is selected.");
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
                var frequency = 0;
                _recordedClip = TryStartMicrophoneClip(_micName, true, 1, out frequency);
                ResetPositionState();

                if (_recordedClip == null)
                    LogError($"Microphone.Start returned null clip. Selected: '{_micName}'.");
                else if (!Microphone.IsRecording(_micName))
                    LogError($"Microphone.Start returned clip, but recording is not active. Selected: '{_micName}'.");
                else
                    Log($"EnableMicrophone succeeded. Frequency: {frequency}. Monitoring clip: {GetClipInfoForLog(_recordedClip)}. State after: {GetRecordingStateForLog()}");
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
                Log($"DisableMicrophone requested. State before: {GetRecordingStateForLog()}");

                if (string.IsNullOrEmpty(_micName))
                {
                    _recordedClip = null;
                    ResetPositionState();
                    Log("DisableMicrophone completed with empty microphone name.");
                    return;
                }

                if (Microphone.IsRecording(_micName) || _recordedClip != null)
                {
                    Log($"DisableMicrophone calling Microphone.End for '{_micName}'. Monitoring clip: {GetClipInfoForLog(_recordedClip)}");
                    Microphone.End(_micName);
                }

                _recordedClip = null;
                ResetPositionState();
                Log($"DisableMicrophone completed. State after: {GetRecordingStateForLog()}");
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
            Log($"Microphone selection changed. New selected microphone: '{MainController.Instance.LocalSettings.GetMicrophoneName()}'.");
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
            _nextMonitoringDiagnosticsTime = 0f;
        }

        private void Update()
        {
            UpdateRecordingPosition();
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

        private void LogMonitoringSampleStatsIfNeeded(int position, AudioClip clip, float rawPeak, float sensitivity, float scaledPeak, string reason)
        {
            if (_isRecording || Time.unscaledTime < _nextMonitoringDiagnosticsTime)
                return;

            _nextMonitoringDiagnosticsTime = Time.unscaledTime + MonitoringDiagnosticsInterval;
            Log($"Monitoring sample stats ({reason}). Position: {position}. RawPeak: {rawPeak:0.000000}. Sensitivity: {sensitivity:0.000}. ScaledPeak: {scaledPeak:0.000000}. Clip: {GetClipInfoForLog(clip)}.");
        }

        private AudioClip TryStartMicrophoneClip(string micName, bool loop, int lengthSec, out int frequency)
        {
            frequency = 0;
            var frequencies = GetCandidateFrequencies(micName);
            Log($"Trying to start microphone '{micName}'. Loop: {loop}. LengthSec: {lengthSec}. Device caps: {GetDeviceCapsForLog(micName)}. Candidate frequencies: {string.Join(", ", frequencies)}. AudioSettings.outputSampleRate: {AudioSettings.outputSampleRate}.");

            for (int i = 0; i < frequencies.Count; i++)
            {
                var candidateFrequency = frequencies[i];

                try
                {
                    var clip = Microphone.Start(micName, loop, lengthSec, candidateFrequency);
                    var isRecording = GetIsRecordingForLog(micName);

                    if (clip != null && Microphone.IsRecording(micName))
                    {
                        frequency = candidateFrequency;
                        Log($"Microphone.Start succeeded. Frequency: {candidateFrequency}. IsRecording: {isRecording}. Clip: {GetClipInfoForLog(clip)}.");
                        return clip;
                    }

                    LogError($"Microphone.Start did not produce an active clip. Frequency: {candidateFrequency}. Clip: {GetClipInfoForLog(clip)}. IsRecording: {isRecording}.");

                    if (clip != null || Microphone.IsRecording(micName))
                        Microphone.End(micName);
                }
                catch (Exception exception)
                {
                    LogError($"Microphone.Start failed. Frequency: {candidateFrequency}.", exception);
                }
            }

            LogError($"All Microphone.Start attempts failed for '{micName}'. Candidate frequencies: {string.Join(", ", frequencies)}.");
            return null;
        }

        private List<int> GetCandidateFrequencies(string micName)
        {
            var frequencies = new List<int>();

            try
            {
                Microphone.GetDeviceCaps(micName, out var minFrequency, out var maxFrequency);

                if (minFrequency == 0 && maxFrequency == 0)
                {
                    AddUniqueFrequency(frequencies, FallbackFrequency);
                    AddUniqueFrequency(frequencies, AudioSettings.outputSampleRate);
                    AddUniqueFrequency(frequencies, DefaultFrequency);
                    return frequencies;
                }

                AddSupportedFrequency(frequencies, FallbackFrequency, minFrequency, maxFrequency);
                AddSupportedFrequency(frequencies, AudioSettings.outputSampleRate, minFrequency, maxFrequency);
                AddSupportedFrequency(frequencies, DefaultFrequency, minFrequency, maxFrequency);
                AddSupportedFrequency(frequencies, maxFrequency, minFrequency, maxFrequency);
                AddSupportedFrequency(frequencies, minFrequency, minFrequency, maxFrequency);
            }
            catch (Exception exception)
            {
                LogError("Failed to get microphone device caps while building frequency candidates.", exception);
            }

            AddUniqueFrequency(frequencies, DefaultFrequency);

            return frequencies;
        }

        private void AddSupportedFrequency(List<int> frequencies, int frequency, int minFrequency, int maxFrequency)
        {
            if (IsFrequencySupported(frequency, minFrequency, maxFrequency))
                AddUniqueFrequency(frequencies, frequency);
        }

        private void AddUniqueFrequency(List<int> frequencies, int frequency)
        {
            if (frequency <= 0 || frequencies.Contains(frequency))
                return;

            frequencies.Add(frequency);
        }

        private string GetRecordingStateForLog()
        {
            return $"mic='{_micName}', trackRecordingFlag={_isRecording}, selected='{MainController.Instance.LocalSettings.GetMicrophoneName()}', " +
                   $"position={GetMicrophonePositionForLog(_micName)}, isRecording={GetIsRecordingForLog(_micName)}, recordingClip={GetClipInfoForLog(_recordingClip)}, monitoringClip={GetClipInfoForLog(_recordedClip)}";
        }

        private string GetClipInfoForLog(AudioClip clip)
        {
            if (clip == null)
                return "null";

            return $"name='{clip.name}', samples={clip.samples}, channels={clip.channels}, frequency={clip.frequency}, length={clip.length:0.000}";
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
                var devices = Microphone.devices;
                if (devices == null || devices.Length == 0)
                    return "none";

                var result = new string[devices.Length];
                for (int i = 0; i < devices.Length; i++)
                    result[i] = $"'{devices[i]}' ({GetDeviceCapsForLog(devices[i])})";

                return string.Join(", ", result);
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

        private string GetMicrophonePositionForLog(string micName)
        {
            if (string.IsNullOrEmpty(micName))
                return "0";

            try
            {
                return Microphone.GetPosition(micName).ToString();
            }
            catch (Exception exception)
            {
                return $"Failed to get position: {exception.Message}";
            }
        }

        private string GetSampleStatsForLog(float[] samples)
        {
            if (samples == null || samples.Length == 0)
                return "empty";

            var peak = 0f;
            var sum = 0f;
            var nonZeroSamples = 0;

            for (int i = 0; i < samples.Length; i++)
            {
                var abs = Mathf.Abs(samples[i]);
                if (abs > peak)
                    peak = abs;

                sum += abs;

                if (abs > 0.0001f)
                    nonZeroSamples++;
            }

            return $"peak={peak:0.000000}, average={sum / samples.Length:0.000000}, nonZero={nonZeroSamples}/{samples.Length}";
        }

        #endregion
    }
}
