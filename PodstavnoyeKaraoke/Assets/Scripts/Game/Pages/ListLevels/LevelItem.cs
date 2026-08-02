using System;
using System.Collections.Generic;
using Controllers;
using Controllers.Levels;
using Extensions;
using Game.Common.Content;
using Game.Pages.Common.SkinItem;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.Pages.ListLevels
{
    public class LevelItem : Interactable
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private Button _button;
        [SerializeField] private Text _lableText;
        [SerializeField] private GameObject _starContainer;
        [SerializeField] private GameObject _star;
        [SerializeField] private GameObject _starFill;
        [SerializeField] private GameObject _numberContainer;
        [SerializeField] private Text _numberText;
        
        [SerializeField] private List<RawImageSkin> _rawImageSkins = new List<RawImageSkin>();
        [SerializeField] private List<ImageSkin> _imageSkins = new List<ImageSkin>();

        private ListLevelPage _listLevelPage;
        private LTDescr _scaleLtd;
        public ContentsWrapping<LevelData> СontentsWrapping { get; private set; }

        #region Init

        public void Init(ContentsWrapping<LevelData> contentsWrapping, ListLevelPage listLevelPage)
        {
            СontentsWrapping = contentsWrapping;
            _listLevelPage = listLevelPage;
            
            InitButton();
            InitText();

            UpdateStar();
            
            СontentsWrapping.GetContent().OnFinished += OnFinished;
        }

        private void InitButton()
        {
            _button.onClick.AddListener(ButtonPress);
            _button.DisableOverDownColors();
        }

        private void InitText()
        {
            _lableText.text =  СontentsWrapping.GetContent().GetNameLevel();
            _numberText.text = MainController.Instance.LevelsController.GetLevelNumber(СontentsWrapping).ToString();
        }

        #endregion

        #region Show / hide

        public void Show(bool smoothly, float delay, float time, Action onComplete = null)
        {
            TryCancelScaleLtd();

            if (smoothly)
            {
                _scaleLtd = _root.LeanScale(Vector3.one, time).setDelay(delay).setEase(LeanTweenType.easeOutExpo)
                    .setOnComplete(
                        () =>
                        {
                            _scaleLtd = null;
                            
                            onComplete?.Invoke();
                        });
            }
            else
            {
                _root.localScale = Vector3.one;
                
                onComplete?.Invoke();
            }
        }
        
        public void Hide(bool smoothly, float delay, float time, Action onComplete = null)
        {
            TryCancelScaleLtd();

            if (smoothly)
            {
                _scaleLtd = _root.LeanScale(Vector3.zero, time).setDelay(delay).setEase(LeanTweenType.easeInQuart)
                    .setOnComplete(
                        () =>
                        {
                            _scaleLtd = null;
                            
                            onComplete?.Invoke();
                        });
            }
            else
            {
                _root.localScale = Vector3.zero;
                
                onComplete?.Invoke();
            }
        }

        private void TryCancelScaleLtd()
        {
            if(_scaleLtd == null)
                return;
            
            LeanTween.cancel(_scaleLtd.id);
            _scaleLtd = null;
        }

        #endregion

        private void UpdateStar()
        {
            if ( СontentsWrapping.GetContent().GetIsFinished())
            {
                _star.gameObject.SetActive(false);
                _starFill.gameObject.SetActive(true);
            }
            else
            {
                _star.gameObject.SetActive(true);
                _starFill.gameObject.SetActive(false);
            }
        }

        private void OnClick()
        {
            _listLevelPage.OpenLevel(this);
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _button.gameObject)
            {
                OnClick();
            }

            return true;
        }
        
        #region Events

        public void UpdateSkin()
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

        private void OnFinished()
        {
            UpdateStar();
        }

        protected virtual void OnDestroy()
        {
            СontentsWrapping.GetContent().OnFinished -= OnFinished;
        }

        #endregion
    }
}