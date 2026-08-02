using Extensions;
using UnityEngine;
using Utilities;

namespace Dialogs.SettingsDialog.Pages
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BasePage : Interactable
    {
        [SerializeField] private TypePage _type;

        private CanvasGroup _canvasGroup;
        private PageState _pageState;
        public TypePage TypePage => _type;
        public bool Completed { get; private set;}

        private void Awake()
        {
            _pageState = PageState.Created;
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void Init(){ }

        public void Show()
        {
            _pageState = PageState.Shown;
            
            Completed = false;
            _canvasGroup.alpha = 0f;
            _canvasGroup.AlphaCanvas(1f, 0.2f).setEase(LeanTweenType.easeInCubic).setOnComplete(() => { Completed = true;});
        }

        public void Hide()
        {
            _pageState = PageState.Hided;
            
            Completed = false;
            _canvasGroup.AlphaCanvas(0f, 0.2f).setEase(LeanTweenType.easeOutCubic).setOnComplete(() => {Destroy(gameObject);});
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;
            
            if (_pageState != PageState.Shown)
                return false;

            return true;
        }

        public virtual void OnUpdate() { }
    }
}