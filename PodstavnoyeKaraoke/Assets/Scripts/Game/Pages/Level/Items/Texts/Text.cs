using System;
using System.Collections.Generic;
using Controllers;
using Controllers.Levels;
using Extensions;
using Game.Pages.Common.SkinItem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace Game.Pages.Level.Items.Texts
{
    public class Text : GameFieldItem
    {
        [SerializeField] private UnityEngine.UI.Text _text;
        [SerializeField] private List<RectTransform> _layouts = new List<RectTransform>();
        [SerializeField] private TextSkin _textSkin;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        
        private LtdManager _ltdManager = new LtdManager();
        public TextData TextData { get; private set; }

        #region Init

        public void Init(Track track, TextData textData)
        {
            base.Init(track);
            
            TextData = textData;

            UpdateFont();
            
            SetText(textData.GetTextContent());
            SetLocalPosition(textData.GetLocalPosition());

            ForceRebuildLayoutImmediate();
            
            UpdateSkin();
        }
        
        public void UpdateFont()
        {
            _text.font = _track.LevelPage.TextFont;
            _text.fontSize = _track.LevelPage.SizeFont;

            ForceRebuildLayoutImmediate();
        }
        
        private void ForceRebuildLayoutImmediate()
        {
            for (int i = 0; i < _layouts.Count; i++)
                LayoutRebuilder.ForceRebuildLayoutImmediate(_layouts[i]);   
        }

        #endregion

        #region Show / hide

        public override void Show(bool smoothly, float time, float delay = 0, Action onComplete = null)
        {
            _ltdManager.TryCancelLtd("alpha");

            if (smoothly)
            {
                var alphaTween = _canvasGroup.LeanAlpha(1, time).setDelay(delay).setOnComplete(() =>
                {
                    _ltdManager.TrySetNullLtd("alpha");
                    
                    onComplete?.Invoke();
                });
                
                _ltdManager.AddLtd("alpha", alphaTween);
            }
            else
            {
                _canvasGroup.alpha = 1;
                
                onComplete?.Invoke();
            }
        }

        public override void Hide(bool smoothly, float time, float delay = 0, Action onComplete = null)
        {
            _ltdManager.TryCancelLtd("alpha");

            if (smoothly)
            {
                var alphaTween = _canvasGroup.LeanAlpha(0, time).setDelay(delay).setOnComplete(() =>
                {
                    _ltdManager.TrySetNullLtd("alpha");
                    
                    onComplete?.Invoke();
                });
                
                _ltdManager.AddLtd("alpha", alphaTween);
            }
            else
            {
                _canvasGroup.alpha = 0;
                
                onComplete?.Invoke();
            }
        }

        #endregion

        private void SetText(string value)
        {
            _text.text = value;
        }
        
        protected override void SetLocalPosition(Vector3 position)
        {
            transform.localPosition = position;
        }
        
        public void UpdateSkin()
        {
            var nameResource = _textSkin.GetNameResource();
            var color = MainController.Instance.SkinsController.GetColorByName(nameResource);
            _textSkin.SetColor(color);
        }
    }
}