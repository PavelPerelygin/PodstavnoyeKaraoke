using System;
using System.Collections.Generic;
using Controllers;
using Managers.Settings;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.Background
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField] private string _screenName = "";
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private List<VideoPart> _videoParts = new List<VideoPart>();

        public void Init()
        {
            for (int i = 0; i < _videoParts.Count; i++)
                _videoParts[i].Init();

            if(_screenName == "main")
                MainController.Instance.LocalSettings.GetMainScreenBackground().OnChangeSource += UpdateBackground;
            else if(_screenName == "select")
                MainController.Instance.LocalSettings.GetSelectScreenBackground().OnChangeSource += UpdateBackground;
            else if(_screenName == "game")
                MainController.Instance.LocalSettings.GetSelectScreenBackground().OnChangeSource += UpdateBackground;

            UpdateBackground();
        }

        private void PrepareVideo(Action onComplete)
        {
            var count = _videoParts.Count;

            for (int i = 0; i < _videoParts.Count; i++)
            {
                _videoParts[i].PrepareVideo(() =>
                {
                    count--;
                    if(count <= 0)
                        onComplete?.Invoke();
                });
            }
        }

        private void Play(bool onStart = false)
        {
            var videoPart = GetNotPlayingPart();
            if (videoPart == null)
            {
                Log.Assert();
                return;
            }
            
            videoPart.Hide(false,0);
            
            videoPart.transform.SetAsLastSibling();
            
            videoPart.Play((float v) =>
            {
                if(v > 2f)
                    return;
                
                Play();
                
                videoPart.ClearOnChangeProgressEvent();
            });
            
            if(!onStart) videoPart.Show(true,2f);
            else videoPart.Show(true,0.1f);
        }

        private void Stop()
        {
            for (int i = 0; i < _videoParts.Count; i++)
                _videoParts[i].Stop();
        }

        private VideoPart GetNotPlayingPart()
        {
            for (int i = 0; i < _videoParts.Count; i++)
            {
                var videPart = _videoParts[i];

                if (!videPart.IsPlaying)
                    return videPart;
            }

            return null;
        }

        private void OnEnable()
        {
            UpdateBackground();
        }

        private void OnDisable()
        {
            Stop();
        }

        private void UpdateBackground()
        {
            SourceData sourceData = null;
            if(_screenName == "main") sourceData = MainController.Instance.LocalSettings.GetMainScreenBackground();
            else if(_screenName == "select") sourceData = MainController.Instance.LocalSettings.GetSelectScreenBackground();
            else if(_screenName == "game") sourceData = MainController.Instance.LocalSettings.GetGameScreenBackground();

            if (sourceData == null)
            {
                Log.Assert();
                return;
            }
            
            if (sourceData.IsExistSource())
            {
                if (sourceData.GetExtension() == ".png" || sourceData.GetExtension() == ".jpg")
                {
                    Stop();

                    for (int i = 0; i < _videoParts.Count; i++)
                        _videoParts[i].gameObject.SetActive(false);
                    
                    _rawImage.gameObject.SetActive(true);
                    _rawImage.texture = sourceData.GetTexture();
                }
                else if (sourceData.GetExtension() == ".mp4")
                {
                    for (int i = 0; i < _videoParts.Count; i++)
                        _videoParts[i].gameObject.SetActive(true);
                    
                    _rawImage.gameObject.SetActive(false);
                    
                    for (int i = 0; i < _videoParts.Count; i++)
                        _videoParts[i].SetPathToVideo(sourceData.GetPathToSource());
                
                    PrepareVideo(() =>
                    {
                        Play(true);
                    });
                }
                    
            }
            else
            {
                Stop();
            
                for (int i = 0; i < _videoParts.Count; i++)
                    _videoParts[i].gameObject.SetActive(false);
                    
                _rawImage.gameObject.SetActive(false);
            }
        }
    }
}