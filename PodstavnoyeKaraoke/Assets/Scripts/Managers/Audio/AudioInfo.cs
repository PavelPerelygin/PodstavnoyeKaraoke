using System;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.Audio;
using Utilities;
using Utilities.Audio;
using Utilities.Files;

namespace Managers.Audio
{
    public class AudioInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public TypeGroup TypeGroup { get; set; }
        public bool External { get; set; }
        public AudioClip AudioClip { get; set; }
        public AudioSource AudioSource { get; set; }
        public float FadeInTime { get; private set; } = -1f;
        public float FadeOutTime { get; private set; } = -1f;
        public bool RemoveAfterStop { get; private set; } = true;
        public bool RemoveAfterCompleted { get; private set; } = true;

        private Action<float> _changeTime;
        private Action<float> _changeProgress;
        private Action _pause;
        private Action _play;
        private Action _stop;
        private Action _completed;

        private AudioManager _audioManager;
        private LTDescr _fadeLTD;
        private bool _loggedUnexpectedStoppedAtStart;
        
        public bool _wasPlay;

        public AudioInfo()
        {
            _audioManager = MainController.Instance.AudioManager;
        }

        #region Update

        public void OnUpdate()
        {
            CalculateLeftTime();
            CalculateProgress();
            CheckCompletedTrack();
        }

        private void CalculateLeftTime()
        {
            if (AudioClip == null || AudioSource == null)
                return;
            
            if(!AudioSource.isPlaying)
                return;

            float leftTime = AudioClip.length - AudioSource.time;
            
            _changeTime?.Invoke(leftTime);
        }
        
        private void CalculateProgress()
        {
            if (AudioClip == null || AudioSource == null)
                return;

            float progress = Mathf.InverseLerp(0, AudioClip.length, AudioSource.time);
            
            _changeProgress?.Invoke(progress);
        }
        
        private void CheckCompletedTrack()
        {
            if (AudioClip == null || AudioSource == null)
                return;
            
            if ((AudioSource.time >= AudioClip.length) ||(AudioSource.time <= 0 && !AudioSource.isPlaying))
            {
                LogDebug($"CheckCompletedTrack detected completed/stopped state. State before completion handling: {GetDebugState()}");
                _wasPlay = false;
                AudioSource.time = 0f;

                if (!AudioSource.loop)
                {
                    if(RemoveAfterCompleted)
                        Remove();
                    
                    _completed?.Invoke();
                }
            }
            else if (_wasPlay && !AudioSource.isPlaying && !_loggedUnexpectedStoppedAtStart)
            {
                _loggedUnexpectedStoppedAtStart = true;
                LogDebug($"AudioSource is not playing while AudioInfo thinks it was playing. State: {GetDebugState()}");
            }
        }

        #endregion
        
        #region Events

        public AudioInfo OnChangeTime(Action<float> action)
        {
            _changeTime = action;

            return this;
        }
        
        public AudioInfo OnChangeProgress(Action<float> action)
        {
            _changeProgress = action;

            return this;
        }
        
        public AudioInfo OnCompleted(Action action)
        {
            _completed = action;

            return this;
        }

        public AudioInfo OnPlay(Action action)
        {
            _play = action;

            return this;
        }
        
        public AudioInfo OnStop(Action action)
        {
            _stop = action;

            return this;
        }
        
        public AudioInfo OnPause(Action action)
        {
            _pause = action;

            return this;
        }

        #endregion

        #region API

        public AudioInfo Play(bool loop = false, float volume = 1f)
        {
            if(_wasPlay)
            {
                LogDebug($"Play skipped because _wasPlay is already true. State: {GetDebugState()}");
                return this;
            }
                
            if (!_audioManager.CheckContainsAudioInfo(this))
            {
                Log.Assert("AudioInfo not found");
                LogDebug($"Play failed because AudioInfo is not registered. State: {GetDebugState()}");
                return this;
            }
            
            if (AudioClip == null)
            {
                LogDebug($"Play failed because AudioClip is null. State before Remove: {GetDebugState()}");
                Remove();
                return this;
            }

            if (AudioSource == null || AudioSource.clip != AudioClip)
            {
                LogDebug($"Play requesting free AudioSource. Current state: {GetDebugState()}");
                AudioSource = _audioManager.GetFreeAudioSource();
            }

            AudioSource.outputAudioMixerGroup = _audioManager.GetAudioMixerGroup(TypeGroup);
            
            AudioClip.name = File.GetNameFile(Path);
            AudioSource.clip = AudioClip;
            AudioSource.loop = loop;
            AudioSource.volume = volume;
            AudioSource.Play();

            _wasPlay = true;
            _loggedUnexpectedStoppedAtStart = false;
            LogDebug($"Play started. Loop: {loop}. Volume: {volume:0.000}. State after AudioSource.Play: {GetDebugState()}");

            if (FadeInTime > 0)
            {
                AudioSource.volume = 0;
                
                SmoothChangeVolume(1f,FadeInTime);
            }
            
            _play?.Invoke();

            return this;
        }

        public AudioInfo Stop()
        {
            LogDebug($"Stop requested. State before: {GetDebugState()}");

            if (!_audioManager.CheckContainsAudioInfo(this))
            {
                Log.Assert("AudioInfo not found");
                LogDebug($"Stop failed because AudioInfo is not registered. State: {GetDebugState()}");
                return this;
            }
            
            if (AudioClip == null || AudioSource == null)
            {
                LogDebug($"Stop skipped because AudioClip or AudioSource is null. State: {GetDebugState()}");
                return this;
            }

            _wasPlay = false;
            
            if (FadeOutTime > 0)
                SmoothChangeVolume(0,FadeOutTime,StopAudioSource);
            else
                StopAudioSource();

            return this;
        }

        public AudioInfo Pause()
        {
            LogDebug($"Pause requested. State before: {GetDebugState()}");

            if (!_audioManager.CheckContainsAudioInfo(this))
            {
                Log.Assert("AudioInfo not found");
                LogDebug($"Pause failed because AudioInfo is not registered. State: {GetDebugState()}");
                return this;
            }
            
            if (AudioClip == null || AudioSource == null)
            {
                LogDebug($"Pause skipped because AudioClip or AudioSource is null. State: {GetDebugState()}");
                return this;
            }

            _wasPlay = false;
            
            if(!AudioSource.isPlaying)
            {
                LogDebug($"Pause skipped because AudioSource is already not playing. State: {GetDebugState()}");
                return this;
            }

            if (FadeOutTime > 0)
                SmoothChangeVolume(0, FadeOutTime,PauseAudioSource);
            else
                PauseAudioSource();

            return this;
        }
        
        public AudioInfo SetFadeIn(float time)
        {
            if (!_audioManager.CheckContainsAudioInfo(this))
            {
                Log.Assert("AudioInfo not found");
                return this;
            }
            
            FadeInTime = time;

            return this;
        }

        public AudioInfo SetFadeOut(float time)
        {
            if (!_audioManager.CheckContainsAudioInfo(this))
            {
                Log.Assert("AudioInfo not found");
                return this;
            }
            
            FadeOutTime = time;

            return this;
        }
        
        public AudioInfo SetProgress(float value)
        {
            LogDebug($"SetProgress requested. Value: {value:0.000}. State before: {GetDebugState()}");

            if (!_audioManager.CheckContainsAudioInfo(this))
            {
                Log.Assert("AudioInfo not found");
                LogDebug($"SetProgress failed because AudioInfo is not registered. State: {GetDebugState()}");
                return this;
            }
            
            if (AudioClip == null || AudioSource == null)
            {
                LogDebug($"SetProgress skipped because AudioClip or AudioSource is null. State: {GetDebugState()}");
                return this;
            }

            float time = Mathf.Lerp(0, AudioClip.length, value);
            AudioSource.time = time;
            LogDebug($"SetProgress completed. Target time: {time:0.000}. State after: {GetDebugState()}");

            return this;
        }
        
        public AudioInfo Revers()
        {
            if (!_audioManager.CheckContainsAudioInfo(this))
            {
                Log.Assert("AudioInfo not found");
                return this;
            }
            
            if (AudioClip == null)
            {
                return this;
            }

            var bytes = WavUtility.GetBytesFromAudioClip(AudioClip);
            WavRevers wav = new WavRevers(bytes);
            wav.Revers();
		
            AudioClip = AudioClip.Create(Name, wav.SampleCount, 1,wav.Frequency, false, false);

            AudioClip.SetData(wav.LeftChannel, 0);
            AudioSource.clip = AudioClip;
            AudioSource.time = 0f;

            return this;
        }

        public float GetAudioClipLenght()
        {
            if (AudioClip == null)
            {
                Log.Assert();
                return -1f;
            }

            return AudioClip.length;
        }

        #endregion
        
        private void StopAudioSource()
        {
            LogDebug($"StopAudioSource invoked. State before AudioSource.Stop: {GetDebugState()}");
            AudioSource.Stop();
            if (RemoveAfterStop)
                Remove();
            
            _stop?.Invoke();
            LogDebug($"StopAudioSource completed. State after: {GetDebugState()}");
        }

        private void PauseAudioSource()
        {
            LogDebug($"PauseAudioSource invoked. State before AudioSource.Pause: {GetDebugState()}");
            AudioSource.Pause();
            
            _pause?.Invoke();
            LogDebug($"PauseAudioSource completed. State after: {GetDebugState()}");
        }

        private void SmoothChangeVolume(float needValue,float time = 1f, Action onCompleted = null)
        {
            float currentVolume = AudioSource.volume;
            float needVolume = needValue;

            if (_fadeLTD != null)
            {
                LeanTween.cancel(_fadeLTD.id);
                _fadeLTD = null;
            }

            _fadeLTD = LeanTween.value(currentVolume, needValue, time).setOnUpdate((float v) =>
            {
                AudioSource.volume = v;
            }).setOnComplete(() =>
            {
                _fadeLTD = null;
                onCompleted?.Invoke();
            });
        }

        public void Remove()
        {
            LogDebug($"Remove requested. State before: {GetDebugState()}");
            if(AudioSource != null)
                AudioSource.clip = null;
                
            _audioManager.RemoveAudioInfo(this);
            LogDebug($"Remove completed. State after: {GetDebugState()}");
        }

        public string GetDebugState()
        {
            var clipState = AudioClip == null
                ? "clip=null"
                : $"clip='{AudioClip.name}', length={AudioClip.length:0.000}, samples={AudioClip.samples}, channels={AudioClip.channels}, frequency={AudioClip.frequency}";

            var sourceState = AudioSource == null
                ? "source=null"
                : $"sourcePlaying={AudioSource.isPlaying}, sourceTime={AudioSource.time:0.000}, sourceLoop={AudioSource.loop}, sourceClip='{(AudioSource.clip == null ? "null" : AudioSource.clip.name)}'";

            return $"name='{Name}', path='{Path}', group={TypeGroup}, external={External}, wasPlay={_wasPlay}, {clipState}, {sourceState}";
        }

        private void LogDebug(string message)
        {
            Utilities.Log.Message($"[AudioInfo] {message}");
        }
    }
}
