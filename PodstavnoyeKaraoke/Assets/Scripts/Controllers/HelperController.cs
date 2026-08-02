using System;
using System.Collections.Generic;
using Blurs;
using Dialogs.Base;
using Extensions;
using Fades;
using GameHelper;
using Layers;
using UnityEngine;

namespace Controllers
{
    public class HelperController
    {
        private List<GameHelperInfo> _steps = new List<GameHelperInfo>();
        private int _currentStep = 0;
        private GameObject _objectsToFocus;
        private List<Layer> _lockedLayers = new List<Layer>();
        private List<Dialog> _lockedDialogs = new List<Dialog>();
        private GameHelpereMessage _activeMessage;
        private bool _stepChanges;
        private bool _isStarted;

        public void Update()
        {
            CheckClickMouse();
        }

        private void CheckClickMouse()
        {
            if(!_isStarted || _stepChanges)
                return;
            
            if(!Input.GetMouseButtonDown(0))
                return;

            GoToNextStep();
        }

        public void StartGameHelper(List<GameHelperInfo> steps)
        {
            Reset();
            
            _isStarted = true;
            
            _steps = steps;

            LockLayers();
            LockDialogs();

            Fade fade = MainController.Instance.GetFadeByType(TypeFade.GameHelper);
            Blur blur = MainController.Instance.GetBlurByType(TypeBlur.GameHelper);
            
            blur.EnableBlur(0.05f,0.3f);
            fade.EnableFade(0.8f,0.3f,0f, () =>
            {
                var info = _steps[_currentStep];
                LaunchStep(info);
            });
        }

        private void LockLayers()
        {
            var layers = new[] {TypeLayer.Boards,
                                            TypeLayer.Boards_ofs,
                                            TypeLayer.Dialogs,
                                            TypeLayer.Dialogs_ofs,
                                            TypeLayer.Hud,
                                            TypeLayer.Hud_ofs,
            };

            for (int i = 0; i < layers.Length; i++)
            {
                Layer layer = MainController.Instance.GetLayerByType(layers[i]);
                if(layer == null)
                    continue;
                
                if(layer.IsLock)
                    continue;

                layer.Lock();
                _lockedLayers.Add(layer);
            }
        }

        private void LockDialogs()
        {
            _lockedDialogs = MainController.Instance.DialogsController.GetUnlockDialogs();

            for (int i = 0; i < _lockedDialogs.Count; i++)
            {
                _lockedDialogs[i].Lock();
            }
        }
        
        private void UnlockLayers()
        {
            for (int i = 0; i < _lockedLayers.Count; i++)
            {
                _lockedLayers[i].Unlock();
            }
        }
        
        private void UnlockDialogs()
        {
            for (int i = 0; i < _lockedDialogs.Count; i++)
            {
                _lockedDialogs[i].Unlock();
            }
        }

        private void GoToNextStep()
        {
            _currentStep++;

            RemoveDuplicateFocusObject();
            if (_currentStep >= _steps.Count)
            {
                if (_activeMessage != null)
                    _activeMessage.Hide(StopGameHelper);
                else
                    StopGameHelper();
                
                return;
            }

            var info = _steps[_currentStep];

            _stepChanges = true;
            
            if (_activeMessage != null)
                _activeMessage.Hide(() => { LaunchStep(info); });
            else
                LaunchStep(info);
        }

        private void LaunchStep(GameHelperInfo info)
        {
            _stepChanges = true;
            
            CreateDuplicateFocusObject(info._target, () => { CreateMessageObject(info);});
        }
        
        private void CreateDuplicateFocusObject(GameObject original, Action onCompleted)
        {
            Layer layer = MainController.Instance.GetLayerByType(TypeLayer.Game_helper);

            _objectsToFocus = MainController.Instance.CreateObj(original, layer.transform);
            _objectsToFocus.transform.position = original.transform.position;
            
            CanvasGroup cg = _objectsToFocus.GetComponent<CanvasGroup>();
            if(cg == null)
                cg = _objectsToFocus.AddComponent<CanvasGroup>();
            cg.alpha = 0f;

            cg.AlphaCanvas(1f, 0.2f).setOnComplete(onCompleted);
        }
        
        private void RemoveDuplicateFocusObject()
        {
            if(_objectsToFocus == null)
                return;

            GameObject obj = _objectsToFocus;
            CanvasGroup cg = obj.GetComponent<CanvasGroup>();
            cg.AlphaCanvas(0f, 0.2f).setOnComplete(() =>
            {
                MainController.Instance.DestroyObj(obj);
            });

            _objectsToFocus = null;
        }

        private void CreateMessageObject(GameHelperInfo info)
        {
            var gameHelperLayer = MainController.Instance.GetLayerByType(TypeLayer.Game_helper);
            _activeMessage = MainController.Instance.CreateObj(Resources.Load<GameHelpereMessage>("Prefabs/GameHelper/GameHelperMessage"), gameHelperLayer.transform);

            var createPosition = info._target.transform.position;
            createPosition.y -= info._target.Size().y / 2f;
            _activeMessage.transform.position = createPosition;
            
            if(info._headerId > 0)
                _activeMessage.SetHeader(MainController.Instance.TextManager.GetText(info._headerId));
            
            if(info._messageId > 0)
                _activeMessage.SetMessage(MainController.Instance.TextManager.GetText(info._messageId));
            
            _activeMessage.Show(() => { _stepChanges = false;});
        }

        private void StopGameHelper()
        {
            Fade fade = MainController.Instance.GetFadeByType(TypeFade.GameHelper);
            Blur blur = MainController.Instance.GetBlurByType(TypeBlur.GameHelper);
            
            blur.DisableBlur(0.05f);
            fade.DisableFade(0.3f,0f, () =>
            {
                _isStarted = false;
                
                RemoveDuplicateFocusObject();
                UnlockLayers();
                UnlockDialogs();
            });
        }

        private void Reset()
        {
            _currentStep = 0;
            _steps = new List<GameHelperInfo>();
            _objectsToFocus = null;
            _lockedLayers = new List<Layer>();
            _lockedDialogs = new List<Dialog>();
            _activeMessage = null;
            _stepChanges = false;
            _isStarted = false;
        }
    }
}