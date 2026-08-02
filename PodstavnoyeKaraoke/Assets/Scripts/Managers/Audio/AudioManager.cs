using System;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.Audio;
using Utilities;
using Utilities.Files;

namespace Managers.Audio
{
    public class AudioManager
    {
        private GameObject _gameObject;
        private AudioMixer _mixer;
        private List<AudioSource> _audioSources;

        private List<AudioInfo> _audioInfos = new List<AudioInfo>();

        public AudioManager()
        {
            _gameObject = MainController.Instance.CreateObj(new GameObject("Audio"), MainController.Instance.transform);
            _mixer = Resources.Load("Audio/AudioMixer") as AudioMixer;
            _audioSources = new List<AudioSource>();
        }

        public void OnUpdate()
        {
            for (int i = 0; i < _audioInfos.Count; i++)
            {
                _audioInfos[i].OnUpdate();
            }
        }

        public AudioInfo Create(string path, TypeGroup typeGroup,bool external = false)
        {
            var audioInfo = new AudioInfo
            {
                TypeGroup = typeGroup,
                External = external
            };

            if (external)
            {
                audioInfo.Path = path;
                audioInfo.AudioClip = File.LoadAudioClipFromStreamingAssets(path);
            }
            else
            {
                audioInfo.Path = File.PathCombine("Audio/source/", path);
                audioInfo.AudioClip = Resources.Load(audioInfo.Path) as AudioClip;
            }

            audioInfo.Name = File.GetNameFile(audioInfo.Path);

            if (audioInfo.AudioClip == null)
                Log.Assert("clip not found");

            _audioInfos.Add(audioInfo);

            return audioInfo;
        }

        public void RemoveAudioInfo(AudioInfo audioInfo)
        {
            if(!_audioInfos.Contains(audioInfo))
                return;

            _audioInfos.Remove(audioInfo);
            
            Resources.UnloadUnusedAssets();
        }

        public bool CheckContainsAudioInfo(AudioInfo audioInfo)
        {
            if (_audioInfos.Contains(audioInfo))
                return true;

            return false;
        }

        public void SetVolumeAuidoGroup(TypeGroup group, float value)
        {
            string nameParametr = $"{group}Volume";
        
            float volume = ConvertPercentageToVolume(value);

            SetFloatParametr("Master",nameParametr, volume);
        }

        public void SmoothVolumeChange(TypeGroup group, float needValue,float time = 1f, Action onComplite = null)
        {
            TypeGroup g = group;

            string nameParametr = $"{group}Volume";
            float currentVolume = GetFloatParametr("Master",nameParametr);
            float currentPercent = ConvertVolumeToPercentage(currentVolume);

            LeanTween.value(currentPercent, needValue, time).setOnUpdate((float v) =>
            {
                SetVolumeAuidoGroup(g, v);
            }).setOnComplete(() =>
            {
                onComplite?.Invoke();
            });

        }

        public void SetFloatParametr(string nameGroup, string nameParametr, float value)
        {
            _mixer.FindMatchingGroups(nameGroup)[0].audioMixer.SetFloat(nameParametr, value);
        }
        
        public float GetFloatParametr(string nameGroup, string nameParametr)
        {
            float value;
            _mixer.FindMatchingGroups(nameGroup)[0].audioMixer.GetFloat(nameParametr, out value);

            return value;
        }

        private float ConvertVolumeToPercentage(float volume)
        {
            float v = volume;
            if (v > 0f) v = 0f;
            else if (v < -70f) v = -70f;

            float percent = Mathf.InverseLerp(-70f, 0f, v);
            return percent;
        }
        
        private float ConvertPercentageToVolume(float percent)
        {
            float volume = Mathf.Lerp(-70, 0, percent);
            if (volume <= -70f)
                volume = -80f;

            return volume;
        }
        
        public AudioSource GetFreeAudioSource()
        {
            AudioSource audioSource = null;

            for (int i = 0; i < _audioSources.Count; i++)
            {
                if (_audioSources[i].isPlaying)
                    continue;
                
                if(_audioSources[i].clip != null)
                    continue;

                audioSource = _audioSources[i];
                break;
            }

            if (audioSource == null)
            {
                audioSource = _gameObject.AddComponent<AudioSource>();
                _audioSources.Add(audioSource);
            }

            return audioSource;
        }

        private AudioSource FindAudioSourceByName(string name)
        {
            AudioSource audioSource = null;

            for (int i = 0; i < _audioSources.Count; i++)
            {
                if(_audioSources[i].clip.name != name)
                    continue;

                audioSource = _audioSources[i];
                break;
            }

            return audioSource;
        }
        
        private List<AudioSource> FindAudioSourcesByGroup(TypeGroup group)
        {
            AudioMixerGroup audioMixerGroup = GetAudioMixerGroup(group);

            List<AudioSource> needed = new List<AudioSource>();

            for (int i = 0; i < _audioSources.Count; i++)
            {
                if(_audioSources[i].outputAudioMixerGroup != audioMixerGroup)
                    continue;
                
                needed.Add(_audioSources[i]);
            }

            return needed;
        }
        
        public AudioMixerGroup GetAudioMixerGroup (TypeGroup group)
        {
            AudioMixerGroup needGroup = _mixer.FindMatchingGroups(group.ToString())[0];
            
            return needGroup;
        }
    }
}