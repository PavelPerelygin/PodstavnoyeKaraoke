using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class GrayScalerInfo
    {
        public Type _type;
        
        public Image _image;
        public Text _text;
        public Shadow _shadow;
        public Outline _outline;
        
        public Color _color;
        public Color _grayColor;
    }
    public class GrayScaler : MonoBehaviour
    {
        private LTDescr _changeColorLTD;
        private bool _hasInitialized;
        private float _grayIntensive = 0f;
        private List<GrayScalerInfo> _infos = new List<GrayScalerInfo>();
        
        private void Init()
        {
            _hasInitialized = true;

            var objects = GetComponentsInChildren<Component>();

            for (int i = 0; i < objects.Length; i++)
            {
                var obj = objects[i];
                var type = obj.GetType();

                if(!CheckAvailableType(type))
                    continue;
                
                var info = new GrayScalerInfo();
                info._type = type;
                
                if (type == typeof(Image))
                {
                    info._image = obj as Image;
                    info._color = info._image.color;
                }else if (type == typeof(Text))
                {
                    info._text = obj as Text;
                    info._color = info._text.color;
                }else if (type == typeof(Shadow))
                {
                    info._shadow = obj as Shadow;
                    info._color = info._shadow.effectColor;
                }else if (type == typeof(Outline))
                {
                    info._outline = obj as Outline;
                    info._color = info._outline.effectColor;
                }
                
                info._grayColor = new Color(info._color.grayscale,info._color.grayscale,info._color.grayscale,1);
                
                _infos.Add(info);
            }
        }

        private bool CheckAvailableType(Type type)
        {
            return type == typeof(Image) || type == typeof(Text) || type == typeof(Shadow) || type == typeof(Outline);
        }

        public LTDescr LeanGray(float to,float time)
        {
            if(!_hasInitialized)
                Init();

            if (_changeColorLTD != null)
            {
                LeanTween.cancel(_changeColorLTD.id);
                _changeColorLTD = null;
            }

            _changeColorLTD = LeanTween.value(_grayIntensive, to, time).setOnUpdate((float v) =>
            {
                _changeColorLTD = null;
                SetGrayColor(v);
            });

            return _changeColorLTD;
        }

        public void EnableGray()
        {
            if(!_hasInitialized)
                Init();

            if (_changeColorLTD != null)
            {
                LeanTween.cancel(_changeColorLTD.id);
                _changeColorLTD = null;
            }
            
            SetGrayColor(1);
        }

        public void DisableGray()
        {
            if(!_hasInitialized)
                Init();

            if (_changeColorLTD != null)
            {
                LeanTween.cancel(_changeColorLTD.id);
                _changeColorLTD = null;
            }
            
            SetGrayColor(0);
        }

        private void SetGrayColor(float interpolation)
        {
            _grayIntensive = interpolation;
            
            for (int i = 0; i < _infos.Count; i++)
            {
                var info = _infos[i];
                
                var color = ColorExtensions.Lerp(info._color, info._grayColor, interpolation);

                if (info._type == typeof(Image))
                {
                    info._image.color = color;
                    
                    if (info._image.material.name != "Custom/GrayScaleShader")
                        info._image.material = MaterialExtensions.CreateMaterial("Custom/GrayScaleShader");

                    info._image.material.SetFloat("_EffectAmount", interpolation);
                }
                else if (info._type == typeof(Text))
                {
                    info._text.color = color;   
                }
                else if (info._type == typeof(Shadow))
                    info._shadow.effectColor = color;
                else if (info._type == typeof(Shadow))
                    info._shadow.effectColor = color;
                else if (info._type == typeof(Outline))
                    info._outline.effectColor = color;
            }
        }
    }
}