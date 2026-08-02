using System;
using System.Collections.Generic;
using Dialogs.Base;
using Dialogs.SettingsDialog.Pages;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.SettingsDialog
{
    public class SettingsDialog : Dialog
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private GameObject _marker;
        [SerializeField] private Button _mainPageButton;
        [SerializeField] private Button _hotkeyPageButton;
        [SerializeField] private Button _specialPageButton;
        [SerializeField] private Button _cabinetPageButton;
        [SerializeField] private Transform _pagesRoot;
        [SerializeField] private BasePage _pageMain;
        [SerializeField] private BasePage _pageHotkeys;
        [SerializeField] private BasePage _pageSpecial;
        [SerializeField] private BasePage _pageCabinet;

        private BasePage _activePage;
        private RectTransform _markerRectTransform;
    
        public override void Init()
        {
            _markerRectTransform = _marker.GetComponent<RectTransform>();

            InitButton();
            
            OpenPageByType(TypePage.Main);
        }

        private void InitButton()
        {
            _closeButton.onClick.AddListener(ButtonPress);
            _closeButton.DisableOverDownColors();

            InitPageButtons();
        }

        private void InitPageButtons()
        {
            _mainPageButton.onClick.AddListener(ButtonPress);
            _hotkeyPageButton.onClick.AddListener(ButtonPress);
            _specialPageButton.onClick.AddListener(ButtonPress);
            _cabinetPageButton.onClick.AddListener(ButtonPress);
        }

        private void OpenPageByType(TypePage typePage)
        {
            if (_activePage != null)
            {
                if(!_activePage.Completed)
                    return;
                
                if(_activePage.TypePage == typePage)
                    return;
                
                _activePage.Hide();
                _activePage = null;
            }

            MoveMarkerToSelectButtonPage(typePage);
            
            _activePage = Instantiate(GetPageByType(typePage),_pagesRoot);
            _activePage.Init();
            _activePage.Show();
        }

        private void MoveMarkerToSelectButtonPage(TypePage typePage)
        {
            GameObject targetButton = null;

            if (typePage == TypePage.Main)
                targetButton = _mainPageButton.gameObject;
            else if (typePage == TypePage.Hotkey)
                targetButton = _hotkeyPageButton.gameObject;
            else if (typePage == TypePage.Special)
                targetButton = _specialPageButton.gameObject;
            else if (typePage == TypePage.Cabinet)
                targetButton = _cabinetPageButton.gameObject;
            
            _marker.transform.SetParent(targetButton.transform);
            _marker.transform.SetAsFirstSibling();
            
            LeanTween.cancel(_marker);

            _marker.LeanMoveLocal(Vector3.zero, 0.3f).setEase(LeanTweenType.easeOutQuint);
            _markerRectTransform.LeanSizeX(targetButton.Size().x, 0.3f).setEase(LeanTweenType.easeOutQuint);
        }

        private BasePage GetPageByType(TypePage typePage)
        {
            if (typePage == TypePage.Main)
                return _pageMain;
            else if (typePage == TypePage.Hotkey)
                return _pageHotkeys;
            else if (typePage == TypePage.Special)
                return _pageSpecial;
            else if (typePage == TypePage.Cabinet)
                return _pageCabinet;

            return null;
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _closeButton.gameObject)
            {
                Hide();
            }
            else if (selectedObj == _mainPageButton.gameObject)
            {
                OpenPageByType(TypePage.Main);
            }
            else if (selectedObj == _hotkeyPageButton.gameObject)
            {
                OpenPageByType(TypePage.Hotkey);
            }
            else if (selectedObj == _specialPageButton.gameObject)
            {
                OpenPageByType(TypePage.Special);
            }
            else if (selectedObj == _cabinetPageButton.gameObject)
            {
                OpenPageByType(TypePage.Cabinet);
            }

            return true;
        }

        public override void OnUpdate()
        {
            if(_activePage == null)
                return;
            
            _activePage.OnUpdate();
        }

    }
}