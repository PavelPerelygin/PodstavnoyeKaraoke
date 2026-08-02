using System;
using System.Collections.Generic;
using Boards;
using Controllers;
using Game.Background;
using Game.Pages.Common;
using Game.Pages.Common.SkinItem;
using UnityEngine;
using Utilities;

namespace Game.Pages
{
    public abstract class Page : Interactable
    {
        [SerializeField] private BackgroundController _backgroundController;
        [SerializeField] protected RectTransform _root;
        [SerializeField] private Fade _fade;
        
        [SerializeField] private List<RawImageSkin> _rawImageSkins = new List<RawImageSkin>();
        [SerializeField] private List<ImageSkin> _imageSkins = new List<ImageSkin>();

        protected MainBoard _mainBoard;
        
        public bool IsOpened { get; protected set; } = false;
        public TypePage PreviousPage { get; protected set; } = TypePage.None;

        public virtual void Init(MainBoard mainBoard)
        {
            _mainBoard = mainBoard;
            
            _backgroundController.Init();
            
            UpdateSkin();
            
            MainController.Instance.LocalSettings.OnChangeSkin += UpdateSkin;
        }
        public abstract TypePage GetTypePage();

        public void SetPreviousPage(TypePage typePage)
        {
            PreviousPage = typePage;
        }

        public virtual float Open(bool smoothly, float delay, Action onComplete = null)
        {
            IsOpened = true;

            return 0f;
        }

        protected float ShowFade(bool smoothly, float delay, Action onComplete = null)
        {
            var time = 0.25f;

            _fade.SetAlpha(1, false, 0, 0);
            _fade.SetAlpha(0,smoothly,time,delay,onComplete);

            return delay + time;
        }

        public virtual float Close(bool smoothly, Action onComplete = null)
        {
            IsOpened = false;

            return 0f;
        }
        
        protected float HideFade(bool smoothly, float delay, Action onComplete = null)
        {
            var time = 0.25f;
            
            return _fade.SetAlpha(0,smoothly,time,delay,onComplete);
        }
        
        #region Events

        private void UpdateSkin()
        {
            for (int i = 0; i < _rawImageSkins.Count; i++)
            {
                var rawImageSkin = _rawImageSkins[i];
                var nameResource = rawImageSkin.GetNameResource();
                var texture = MainController.Instance.SkinsController.GetTexture2DByName(nameResource);
                rawImageSkin.SetTexture2D(texture);
            }
            
            for (int i = 0; i < _imageSkins.Count; i++)
            {
                var imageSkin = _imageSkins[i];
                var nameResource = imageSkin.GetNameResource();
                var sprite = MainController.Instance.SkinsController.GetSpriteByName(nameResource);
                imageSkin.SetSprite(sprite);
            }
        }

        protected virtual void OnDestroy()
        {
            MainController.Instance.LocalSettings.OnChangeSkin -= UpdateSkin;
        }

        #endregion
    }
}