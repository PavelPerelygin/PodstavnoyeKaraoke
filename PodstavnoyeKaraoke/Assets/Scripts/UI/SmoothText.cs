using System;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SmoothText : MonoBehaviour
    {
        private enum SmoothShowState
        {
            None = 0,
            Show = 1,
            Hide = 2,
        }
        
        [SerializeField] private Text _label;
        [SerializeField] private Shadow _shadow;
        [SerializeField] private bool _smoothlyAlpha = true;
        [SerializeField] private float _alphaStep = 0.08f;
        [SerializeField] private int _tailCount = 10;
        [SerializeField] private bool _removeHtmlTags;
        
        private string _text;
        private SmoothShowState _smoothShowState;
        private float _currentTime;
        private float _needTime;
        private Action _onCompleted;
        private Action _onUpdate;
        private Color _textColor;

        private float _initAlphaShadow = 0f;

        private void Awake()
        {
            if (_shadow != null)
            {
                var initColor = _shadow.effectColor;
                _initAlphaShadow = initColor.a;
            }
        }

        public void Show(string text, float time, Action onCompleted = null)
        {
            _smoothShowState = SmoothShowState.Show;
            _text = text;
            _textColor = _label.color;
            _currentTime = 0;
            _needTime = time;
            _onCompleted = onCompleted;
        }

        public void Hide(string text, float time, Action onCompleted = null)
        {
            _smoothShowState = SmoothShowState.Hide;
            _text = text;
            _textColor = _label.color;
            _currentTime = 0;
            _needTime = time;
            _onCompleted = onCompleted;
        }

        private void Update()
        {
            if(_smoothShowState == SmoothShowState.None)
                return;

            if (_smoothShowState == SmoothShowState.Show)
                UpdateShowText();
            else if (_smoothShowState == SmoothShowState.Hide)
                UpdateHideText();
        }

        private void UpdateShowText()
        {
            _currentTime += Time.deltaTime;

            var showProgress = 0f;
            var needShowSymbol = 0;
            
            if (_needTime > 0)
            {
                showProgress = _currentTime / _needTime;
                needShowSymbol = (int)(_text.Length * showProgress);
            }
            else
            {
                showProgress = 1f;
                needShowSymbol = _text.Length;
            }

            if (showProgress >= 1f)
            {
                _onCompleted?.Invoke();
                _smoothShowState = SmoothShowState.None;
            }

            if (needShowSymbol < 0)
                needShowSymbol = 0;
            if (needShowSymbol > _text.Length - 1)
                needShowSymbol = _text.Length - 1;

            string richTxtStr = CreateRichTextString(_text, needShowSymbol);
            _label.text = richTxtStr;

            if (_shadow != null)
            {
                var color = _shadow.effectColor;
                color.a = Mathf.Lerp(0, _initAlphaShadow, showProgress);
                _shadow.effectColor = color;
            }
        }
        
        private void UpdateHideText()
        {
            _currentTime += Time.deltaTime;
            
            float hideProgress = _currentTime / _needTime;
            float hideTextPart = _text.Length * (1f - hideProgress);
            int needShowSymbol = (int)hideTextPart;

            if (needShowSymbol > _text.Length - 1)
                needShowSymbol = _text.Length - 1;
            else if (needShowSymbol < 0)
                needShowSymbol = 0;

            if (hideProgress >= 1f)
            {
                _onCompleted?.Invoke();
                _smoothShowState = SmoothShowState.None;
            }

            string richTxtStr = CreateRichTextString(_text, needShowSymbol);
            _label.text = richTxtStr;

            if (_shadow != null)
            {
                var color = _shadow.effectColor;
                color.a = Mathf.Lerp(_initAlphaShadow, 0, hideProgress);
                _shadow.effectColor = color;
            }
        }
        
        string CreateRichTextString(string text, int needShowSymbolIdx)
        {
            if (text == "")
                return "";
            
            float currentSymbolAlpha = 1f;
            Color color = _textColor;
            string hexColor = "";

            string richTextStr = "";

            if (_removeHtmlTags)
            {
                if (needShowSymbolIdx != 0)
                    richTextStr = text.Substring(0, needShowSymbolIdx);
            }
            else
            {
                for (int i = 0; i < needShowSymbolIdx; i++)
                {
                    color.a = currentSymbolAlpha;
                    hexColor = color.ConvertColorToHex();
                    richTextStr += $"<color={hexColor}>{text[i]}</color>";
                }
            }
            
            int tailCount = _tailCount;
            
            if (needShowSymbolIdx + tailCount > text.Length - 1)
                tailCount = text.Length - needShowSymbolIdx;
            
            if (needShowSymbolIdx - tailCount < 0)
                tailCount = needShowSymbolIdx;
            

            if (!_smoothlyAlpha)
            {
                tailCount = 0;
            }

            for (int i = 0; i < tailCount; ++i)
            {
                color.a = currentSymbolAlpha - _alphaStep * i;
                hexColor = color.ConvertColorToHex();
                richTextStr += $"<color={hexColor}>{text[needShowSymbolIdx + i]}</color>";
            }

            if (needShowSymbolIdx + tailCount <= text.Length - 1)
            {
                color.a = 0;
                hexColor = color.ConvertColorToHex();
                string nonAlphaSymbols = text.Substring(needShowSymbolIdx + tailCount, text.Length - (needShowSymbolIdx + tailCount));
                richTextStr += $"<color={hexColor}>{nonAlphaSymbols}</color>";
            }

            return richTextStr;
        }

        public bool IsCompleted()
        {
            if (_smoothShowState == SmoothShowState.None)
                return true;

            return false;
        }

        public void SetText(string text)
        {
            _label.text = text;
        }
        
        public void Refresh()
        {
            _label.text = "";
            _smoothShowState = SmoothShowState.None;
        }
    }
}