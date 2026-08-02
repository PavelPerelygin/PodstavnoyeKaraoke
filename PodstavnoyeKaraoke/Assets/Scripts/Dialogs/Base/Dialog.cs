using System;
using System.Collections.Generic;
using Controllers;
using Extensions;
using Layers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Dialogs.Base
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class Dialog : Interactable
    {
        [SerializeField] protected DialogInfo _info = new DialogInfo();
        [SerializeField] protected VisualizationInfo _showInfo = VisualizationInfo.ShowVisualization();
        [SerializeField] protected VisualizationInfo _hideInfo = VisualizationInfo.HideVisualization();
        
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        public TypeDialog TypeDialog => _info._typeDialog;
        public bool EnableFade => _info._enableFade;
        public float IntensityFade => _info._intensityFade;
        public bool HideOnSecondScreen => _info._hideOnSecondScreen;
        public bool EnableBlur => _info._enableBlur;
        public float IntensityBlur => _info._intensityBlur;
        public bool IsLock => !_canvasGroup.blocksRaycasts;
        public StateDialog StateDialog { get; protected set; }
        public List<TypeLayer> BlockLayers => _info._blockLayers;

        public List<Action> OnHide { get; set; } = new List<Action>();
        public List<Action> OnHideComplete { get; set; } = new List<Action>();
        public List<Action> OnShowComplete { get; set; } = new List<Action>();

        [HideInInspector] public int SortingOrder => _canvas.sortingOrder;


        #region INIT
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();

            StateDialog = StateDialog.Created;
            transform.position = _info._createPosition;
        }

        public virtual void Init()
        {
        }
        #endregion

        #region UPDATE
        protected override void Update()
        {
            base.Update();
        
            CheckNeedCloseClickOut();
        }

        private void CheckNeedCloseClickOut()
        {
            if(!_info._needCloseByClick)
                return;
            
            if(StateDialog != StateDialog.Shown)
                return;

            if(_ignoreTimeLeft > 0)
                return;

            if (!Input.GetMouseButtonDown(0))
                return;
            
            if (!gameObject.CheckMouseClickOnObject())
                return;
            
            Hide();
        }
        
        public virtual void OnUpdate() { }
        #endregion

        #region SHOW
        
        public virtual void Show(float delay = 0f)
        {
            if(StateDialog == StateDialog.Showing)
                return;
            
            StateDialog = StateDialog.Showing;

            _canvasGroup.blocksRaycasts = false;
            
            switch (_showInfo._type)
            {
                case TypeVisualization.Move: MoveShow(delay); break;
                case TypeVisualization.MoveAlpha: MoveAlphaShow(delay); break;
                case TypeVisualization.Alpha: AlphaShow(delay); break;
                case TypeVisualization.Scale: ScaleShow(delay); break;
                case TypeVisualization.ScaleAlpha: ScaleAlphaShow(delay); break;
                case TypeVisualization.Immediate: ShowImmediate(); break;
                case TypeVisualization.Animator: ShowAnimator(); break;
            }
            
            MainController.Instance.DialogsController.UpdateFade(delay);
            MainController.Instance.DialogsController.UpdateBlur(delay);
        }

        private void MoveShow(float delay)
        {
            _canvasGroup.alpha = 1f;
            gameObject.transform.position = gameObject.GetPositionOffScreenByDirection(_showInfo._direction);
            gameObject.LeanMove(_info._createPosition, _showInfo._time).setEase(_showInfo._tweenType).setDelay(delay).setOnComplete(OnShown);
        }
        
        private void MoveAlphaShow(float delay)
        {
            gameObject.transform.position = gameObject.GetPositionOffScreenByDirection(_showInfo._direction);
            _canvasGroup.alpha = 0;
            _canvasGroup.AlphaCanvas(1f, _showInfo._time).setDelay(delay);
            gameObject.LeanMove(_info._createPosition, _showInfo._time).setDelay(delay).setEase(_showInfo._tweenType).setOnComplete(OnShown);
        }
        
        private void AlphaShow(float delay)
        {
            gameObject.transform.position = _info._createPosition;
            _canvasGroup.alpha = 0;
            _canvasGroup.AlphaCanvas(1f, _showInfo._time).setDelay(delay).setOnComplete(OnShown);
        }
        
        private void ScaleShow(float delay)
        {
            _canvasGroup.alpha = 1f;
            gameObject.transform.position = _info._createPosition;
            gameObject.transform.localScale = Vector3.zero;
            gameObject.LeanScale(Vector3.one, _showInfo._time).setDelay(delay).setEase(_showInfo._tweenType).setOnComplete(OnShown);
        }
        
        private void ScaleAlphaShow(float delay)
        {
            gameObject.transform.position = _info._createPosition;
            gameObject.transform.localScale = Vector3.zero;
            _canvasGroup.alpha = 0;
            _canvasGroup.AlphaCanvas(1f, _showInfo._time).setDelay(delay);
            gameObject.LeanScale(Vector3.one, _showInfo._time).setDelay(delay).setEase(_showInfo._tweenType).setOnComplete(OnShown);
        }

        private void ShowImmediate()
        {
            gameObject.transform.position = _info._createPosition;
            gameObject.transform.localScale = Vector3.one;
            _canvasGroup.alpha = 1;
            OnShown();
        }
        
        private void ShowAnimator()
        {
            Log.Assert();
            
            gameObject.transform.position = _info._createPosition;
            gameObject.transform.localScale = Vector3.one;
            _canvasGroup.alpha = 1;
            OnShown();
        }
        
        protected virtual void OnShown()
        {
            StateDialog = StateDialog.Shown;
            _canvasGroup.blocksRaycasts = true;
            
            for (int i = 0; i < OnShowComplete.Count; i++)
                OnShowComplete[i].Invoke();
            
            OnShowComplete.Clear();
        }

        #endregion

        #region HIDE
        public virtual void Hide(bool needRemove = true)
        {
            if (StateDialog == StateDialog.Hiding)
                return;

            StateDialog = StateDialog.Hiding;
            
            _canvasGroup.blocksRaycasts = false;

            if (needRemove)
                MainController.Instance.DialogsController.RemoveDialog(this);
            
            switch (_hideInfo._type)
            {
                case TypeVisualization.Move: MoveHide(); break;
                case TypeVisualization.MoveAlpha: MoveAlphaHide(); break;
                case TypeVisualization.Alpha: AlphaHide(); break;
                case TypeVisualization.Scale: ScaleHide(); break;
                case TypeVisualization.ScaleAlpha: ScaleAlphaHide(); break;
                case TypeVisualization.Immediate: ImmediateHide(); break;
                case TypeVisualization.Animator: AnimatorHide(); break;
            }

            for (int i = 0; i < OnHide.Count; i++)
                OnHide[i]?.Invoke();
            
            OnHide.Clear();
        }

        private void MoveHide()
        {
            var hidePosition = gameObject.GetPositionOffScreenByDirection(_hideInfo._direction);
            gameObject.LeanMove(hidePosition, _hideInfo._time).setEase(_hideInfo._tweenType).setOnComplete(OnHidden);
        }

        private void MoveAlphaHide()
        {
            var hidePosition = gameObject.GetPositionOffScreenByDirection(_hideInfo._direction);
            _canvasGroup.AlphaCanvas(0f, _hideInfo._time).setEase(_hideInfo._tweenType);
            gameObject.LeanMove(hidePosition, _hideInfo._time).setEase(_hideInfo._tweenType).setOnComplete(OnHidden);
        }

        private void AlphaHide()
        {
            _canvasGroup.AlphaCanvas(0f, _hideInfo._time).setEase(_hideInfo._tweenType).setOnComplete(OnHidden);
        }

        private void ScaleHide()
        {
            gameObject.LeanScale(Vector3.zero, _hideInfo._time).setEase(_hideInfo._tweenType).setOnComplete(OnHidden);
        }
        
        private void ScaleAlphaHide()
        {
            _canvasGroup.AlphaCanvas(0, _hideInfo._time);
            gameObject.LeanScale(Vector3.zero, _hideInfo._time).setEase(_hideInfo._tweenType).setOnComplete(OnHidden);
        }
        
        private void ImmediateHide()
        {
            var hidePosition = gameObject.GetPositionOffScreenByDirection(_hideInfo._direction);
            gameObject.transform.position = hidePosition;
            _canvasGroup.alpha = 0;
            OnHidden();
        }

        private void AnimatorHide()
        {
            Log.Assert();
            
            var hidePosition = gameObject.GetPositionOffScreenByDirection(_hideInfo._direction);
            gameObject.transform.position = hidePosition;
            _canvasGroup.alpha = 0;
            OnHidden();
        }

        protected virtual void OnHidden()
        {
            StateDialog = StateDialog.Hidden;
            
            _canvasGroup.blocksRaycasts = true;

            for (int i = 0; i < OnHideComplete.Count; i++)
                OnHideComplete[i]?.Invoke();
            
            OnHideComplete.Clear();

            if(!MainController.Instance.DialogsController.CheckDialogueExists(this))
                Destroy(gameObject);
        }
        #endregion

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;
            
            if(StateDialog != StateDialog.Shown)
                return false;

            if (!_canvasGroup.blocksRaycasts)
                return false;

            return true;
        }

        protected override bool KeyPressHandler()
        {
            if(StateDialog != StateDialog.Shown)
                return false;

            if (!_canvasGroup.blocksRaycasts)
                return false;

            if(_ignoreTimeLeft > 0)
                return false;

            return true;
        }

        #region API
        public void SetSortingLayerAndOrder(string layer,int order)
        {
            _canvas.sortingLayerName = layer;
            _canvas.sortingOrder = order;
        }
        
        public int GetMaxSortingOrder()
        {
            int maxSortingOrder = _canvas.sortingOrder;

            var canvases = gameObject.GetComponentsInChildren<Canvas>();

            for (int i = 0; i < canvases.Length; i++)
            {
                var canvas = canvases[i];
                if(!canvas.overrideSorting)
                    continue;
                
                if(canvas.sortingLayerName != _canvas.sortingLayerName)
                    continue;

                if (canvas.sortingOrder > maxSortingOrder)
                    maxSortingOrder = canvas.sortingOrder;

            }

            return maxSortingOrder;
        } 

        public void Lock()
        {
            _canvasGroup.blocksRaycasts = false;
        }
        
        public void Unlock()
        {
            _canvasGroup.blocksRaycasts = true;
        }
        #endregion

    }
}