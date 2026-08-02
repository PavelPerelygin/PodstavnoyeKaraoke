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
                _wasPlay = false;
                AudioSource.time = 0f;

                if (!AudioSource.loop)
                {
                    if(RemoveAfterCompleted)
                        Remove();
                    
                    _completed?.Invoke();
                }
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
                return this;
                
            if (!_audioManager.CheckContainsAudioInfo(this))
            {
                Log.Assert("AudioInfo not found");
                return this;
            }
            
            if (AudioClip == null)
            {
                Remove();
                return this;
            }

            AudioSource = _audioManager.GetFreeAudioSource();
            AudioSource.outputAudioMixerGroup = _audioManager.GetAudioMixerGroup(TypeGroup);
            
            AudioClip.name = File.GetNameFile(Path);
            AudioSource.clip = AudioClip;
            AudioSource.loop = loop;
            AudioSource.volume = volume;
            AudioSource.Play();

            _wasPlay = true;

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
            if (!_audioManager.CheckContainsAudioInfo(this))
            {
                Log.Assert("AudioInfo not found");
                return this;
            }
            
            if (AudioClip == null || AudioSource == null)
                return this;

            _wasPlay = false;
            
            if(!AudioSource.isPlaying)
                return this;

            if (FadeOutTime > 0)
                SmoothChangeVolume(0,FadeOutTime,StopAudioSource);
            else
                StopAudioSource();

            return this;
        }

        public AudioInfo Pause()
        {
            if (!_audioManager.CheckContainsAudioInfo(this))
            {
                Log.Assert("AudioInfo not found");
                return this;
            }
            
            if (AudioClip == null || AudioSource == null)
                return this;

            _wasPlay = false;
            
            if(!AudioSource.isPlaying)
                return this;

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
            if (!_audioManager.CheckContainsAudioInfo(this))
            {
                Log.Assert("AudioInfo not found");
                return this;
            }
            
            if (AudioClip == null || AudioSource == null)
                return this;

            float time = Mathf.Lerp(0, AudioClip.length, value);
            AudioSource.time = time;

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
            AudioSource.Stop();
            if (RemoveAfterStop)
                Remove();
            
            _stop?.Invoke();
        }

        private void PauseAudioSource()
        {
            AudioSource.Pause();
            
            _pause?.Invoke();
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
            if(AudioSource != null)
                AudioSource.clip = null;
                
            _audioManager.RemoveAudioInfo(this);
        }
    }
}