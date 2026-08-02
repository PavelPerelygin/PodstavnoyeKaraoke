using System;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Files;

namespace Game.Background
{
    public class VideoPart : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Video.VideoPlayer _videoPlayer;
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private CanvasGroup _canvasGroup;

        private string _pathToVideo = "";
        private LTDescr _alphaLtd;
        private Action<float> _onProgress;
        private Action _onPrepareCompleted;

        public bool IsPlaying { get; private set; }

        public void Init()
        {
            CreateRenderTexture();
            
            _videoPlayer.loopPointReached += VideoCompleted;
            _videoPlayer.prepareCompleted += PrepareCompleted;

            Hide(false,0f);
        }

        public void SetPathToVideo(string path)
        {
            _pathToVideo = path;
        }

        private void PrepareCompleted(UnityEngine.Video.VideoPlayer source)
        {
            _onPrepareCompleted?.Invoke();

            _onPrepareCompleted = null;
        }

        private void VideoCompleted(UnityEngine.Video.VideoPlayer source)
        {
            IsPlaying = false;
            
            _videoPlayer.Stop();
            
            _videoPlayer.Prepare();
        }

        private void CreateRenderTexture()
        {
            RenderTexture renderTexture = new RenderTexture(1920, 1080, 24);

            _rawImage.texture = renderTexture;
            _videoPlayer.targetTexture = renderTexture;
        }

        public void PrepareVideo(Action onComplete)
        {
            var fullPath = File.PathCombine(File.GetPathToStreamingAssets(), _pathToVideo);
            _videoPlayer.url = fullPath;
            
            _videoPlayer.Prepare();

            _onPrepareCompleted = onComplete;

            for (int i = 0; i < _videoPlayer.controlledAudioTrackCount; i++)
            {
                _videoPlayer.SetDirectAudioVolume((ushort)i, 0);
            }
        }

        public void Play(Action<float> onProgress)
        {
            IsPlaying = true;
            
            _onProgress = onProgress;
            
            _videoPlayer.Play();
        }

        public void Stop()
        {
            IsPlaying = false;

            _onProgress = null;
            
            _videoPlayer.Stop();
        }

        public void ClearOnChangeProgressEvent()
        {
            _onProgress = null;
        }

        #region Show & hide

        public void Show(bool smoothly, float time = 0)
        {
            TryCancelAlphaLtd();

            if (smoothly)
            {
                _alphaLtd = _canvasGroup.LeanAlpha(1, time).setOnComplete(() =>
                {
                    _alphaLtd = null;
                });
            }
            else
            {
                _canvasGroup.alpha = 1;
            }
        }

        public void Hide(bool smoothly, float time = 0)
        {
            TryCancelAlphaLtd();

            if (smoothly)
            {
                _alphaLtd = _canvasGroup.LeanAlpha(0, time).setOnComplete(() =>
                {
                    _alphaLtd = null;
                });
            }
            else
            {
                _canvasGroup.alpha = 0;
            }
        }

        private void TryCancelAlphaLtd()
        {
            if(_alphaLtd == null)
                return;
            
            LeanTween.cancel(_alphaLtd.id);
            _alphaLtd = null;
        }

        #endregion

        #region Update

        private void Update()
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            if(!IsPlaying)
                return;

            var leftTime = (float)(_videoPlayer.length - _videoPlayer.time);

            //Debug.Log($"left time ({gameObject.name}):{leftTime}");
            
            _onProgress?.Invoke(leftTime);
        }

        #endregion
    }
}