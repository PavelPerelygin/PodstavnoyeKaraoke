using System;
using System.Collections.Generic;
using Blurs;
using Dialogs;
using Dialogs.Base;
using Fades;
using Layers;
using UnityEngine;
using Utilities;

namespace Controllers
{
    public class DialogsController
    {
        private List<Dialog> _dialogs = new List<Dialog>();
        private List<TypeLayer> _blockTypeLayers = new List<TypeLayer>();

        public void OpenDialog(TypeDialog type,float delay = 0f)
        {
            var dialog = CreateDialog(type);
            dialog.Init();
            dialog.Show(delay);
        }

        public Dialog CreateDialog(TypeDialog type)
        {
            Layer createLayer = MainController.Instance.GetLayerByType(TypeLayer.Dialogs);
            if (createLayer == null)
            {
                Log.Assert("layer not found");
                return null;
            }

            string path = $"Prefabs/Dialogs/{type}";

            var dialog = MainController.Instance.CreateObj(Resources.Load<Dialog>(path), createLayer.transform);
            dialog.SetSortingLayerAndOrder(TypeLayer.Dialogs.ToString(),GetMaxDialogOrder());

            if (!dialog.HideOnSecondScreen)
                dialog.gameObject.layer = LayerMask.NameToLayer(TypeLayer.Dialogs.ToString());
            else
                dialog.gameObject.layer = LayerMask.NameToLayer(TypeLayer.Dialogs_ofs.ToString());

            _dialogs.Add(dialog);
            HideOtherDialogs();
            
            UpdateBlockLayer();

            return dialog;
        }
        
        public int GetMaxDialogOrder()
        {
            var maxOrder = 0;
            
            for (var i = 0; i < _dialogs.Count; ++i)
            {
                var dialog = _dialogs[i];
                
                int sortingOrder = dialog.GetMaxSortingOrder();

                if (sortingOrder > maxOrder)
                    maxOrder = sortingOrder;
            }

            return maxOrder + 5;
        }

        private void HideOtherDialogs()
        {
            for (int i = 0; i < _dialogs.Count-1; i++)
            {
                _dialogs[i].Hide(false);
            }
        }

        public Dialog SearchDialogByType (TypeDialog type)
        {
            for (int i = 0; i < _dialogs.Count; i++)
            {
                if (_dialogs[i].TypeDialog == type)
                    return _dialogs[i];
            }

            return null;
        }

        public int GetCountDialogs()
        {
            return _dialogs.Count;
        }
    
        public void RemoveDialog(Dialog dialog)
        {
            if (_dialogs.Contains(dialog))
                _dialogs.Remove(dialog);
            else
                Log.Assert($"[{dialog.name}] dialog not find");

            if(_dialogs.Count > 0)
                _dialogs[_dialogs.Count - 1].Show(0.2f);

            UpdateBlockLayer();
            UpdateFade();
            UpdateBlur();
        }

        public bool CheckDialogueExists(Dialog dialog)
        {
            return _dialogs.Contains(dialog);
        }
    
        public void UpdateFade(float delay = 0f)
        {
            var fade = MainController.Instance.GetFadeByType(TypeFade.Dialog);
            
            var activeDialogs = GetActiveDialogs();
            var enableFade = false;
            var maxIntensity = 0f;
            var layerName = TypeLayer.Fade_dialogs.ToString();
            
            for (int i = 0; i < activeDialogs.Count; i++)
            {
                var dialog = activeDialogs[i];

                if (dialog.EnableFade)
                {
                    enableFade = true;
                    
                    if (dialog.HideOnSecondScreen)
                        layerName = TypeLayer.Fade_dialogs_ofs.ToString();
                }
                else
                {
                    continue;
                }

                if (dialog.IntensityFade > maxIntensity)
                    maxIntensity = dialog.IntensityFade;
            }

            if (enableFade)
            {
                fade.gameObject.layer = LayerMask.NameToLayer(layerName);
                fade.EnableFade(maxIntensity,0.5f,delay); 
            }
            else
            {
                fade.DisableFade(0.5f,delay, () =>
                {
                    fade.gameObject.layer = LayerMask.NameToLayer(layerName);
                });
            }
        }
        
        public void UpdateBlur(float delay = 0f)
        {
            var blur = MainController.Instance.GetBlurByType(TypeBlur.Dialog);
            
            var activeDialogs = GetActiveDialogs();
            var enableBlur = false;
            var maxIntensity = 0f;
            var layerName = TypeLayer.Blur_dialogs.ToString();

            for (int i = 0; i < activeDialogs.Count; i++)
            {
                var dialog = activeDialogs[i];

                if (dialog.EnableBlur)
                {
                    enableBlur = true;
                    
                    if (dialog.HideOnSecondScreen)
                        layerName = TypeLayer.Blur_dialogs_ofs.ToString();
                }
                else
                {
                    continue;
                }

                if (dialog.IntensityBlur > maxIntensity)
                    maxIntensity = dialog.IntensityBlur;
            }
            
            if (enableBlur)
            {
                blur.gameObject.layer = LayerMask.NameToLayer(layerName);
                blur.EnableBlur(maxIntensity,0.5f,delay);
            }
            else
            {
                blur.DisableBlur(0.5f,delay, () =>
                {
                    blur.gameObject.layer = LayerMask.NameToLayer(layerName);
                });
            }
        }
    
        private void UpdateBlockLayer()
        {
            List<Dialog> activeDialogs = GetActiveDialogs();
            List<TypeLayer> layersForBlocking = new List<TypeLayer>();

            for (int i = 0; i < activeDialogs.Count; i++)
            {
                Dialog activeDialog = activeDialogs[i];
                for (int j = 0; j < activeDialog.BlockLayers.Count; j++)
                {
                    if(!layersForBlocking.Contains(activeDialog.BlockLayers[j]))
                        layersForBlocking.Add(activeDialog.BlockLayers[j]);
                }
            }

            for (int i = _blockTypeLayers.Count - 1; i >= 0; i--)
            {
                if (!layersForBlocking.Contains(_blockTypeLayers[i]))
                {
                    Layer layer = MainController.Instance.GetLayerByType(_blockTypeLayers[i]);
                    if(layer == null)
                        continue;
                    
                    layer.Unlock();
                    _blockTypeLayers.RemoveAt(i);
                }
                    
            }

            for (int i = 0; i < layersForBlocking.Count; i++)
            {
                if (!_blockTypeLayers.Contains(layersForBlocking[i]))
                {
                    Layer layer = MainController.Instance.GetLayerByType(layersForBlocking[i]);
                    if(layer == null)
                        continue;
                    
                    layer.Lock();
                    _blockTypeLayers.Add(layersForBlocking[i]);
                }
            }
        }

        private List<Dialog> GetActiveDialogs()
        {
            List<Dialog> dialogs = new List<Dialog>();

            for (int i = 0; i < _dialogs.Count; i++)
            {
                Dialog dialog = _dialogs[i];
                
                if(dialog.StateDialog != StateDialog.Hiding || dialog.StateDialog != StateDialog.Hidden)
                    dialogs.Add(dialog);
            }

            return dialogs;
        }

        public List<Dialog> GetUnlockDialogs()
        {
            List<Dialog> dialogs = new List<Dialog>();

            for (int i = 0; i < _dialogs.Count; i++)
            {
                Dialog dialog = _dialogs[i];
                
                if(!dialog.IsLock)
                    dialogs.Add(dialog);
            }

            return dialogs;
        }

        public void UpdateAllDialogs()
        {
            for (int i = 0; i < _dialogs.Count; i++)
                _dialogs[i].OnUpdate();
        }

        public void UpdateDialogByType(TypeDialog type)
        {
            Dialog dialog = SearchDialogByType(type);
            dialog.OnUpdate();
        }
    }
}
