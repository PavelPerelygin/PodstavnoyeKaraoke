using System;
using System.Collections.Generic;
using Controllers.GameChanges;
using Dialogs.Base;
using Extensions;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs
{
    public class GameChangesDialog : Dialog
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Transform _pageDummy;

        private List<GameChangesPage> _pages = new List<GameChangesPage>();
        private GameChangesPage _currentPage;
        private bool _showNextPageComplete = true;

        public void Init(List<GameChangesPage> pages)
        {
            _pages = pages;

            InitButtons();
            
            ShowNextPage(false);
        }

        private void InitButtons()
        {
            _closeButton.onClick.AddListener(ButtonPress);
            _closeButton.DisableOverDownColors();
        }

        private void ShowNextPage(bool smoothly)
        {
            if(!_showNextPageComplete)
                return;
            
            var nextPage = GetNextPage();
            if (nextPage == null)
                return;

            if (_currentPage != null)
            {
                HidePage(_currentPage);
                _currentPage = null;
            }

            _currentPage = Instantiate(nextPage, _pageDummy);

            _showNextPageComplete = false;
                
            SetSizeDialogByPage(_currentPage, smoothly);
            ShowPage(_currentPage, () =>
            {
                _showNextPageComplete = true;
            });
        }

        private GameChangesPage GetNextPage()
        {
            if (_pages.Count <= 0)
                return null;

            var page = _pages[0];
            _pages.RemoveAt(0);

            return page;
        }

        private void HidePage(GameChangesPage page)
        {
            page.CanvasGroup.LeanAlpha(0f, 0.3f).setOnComplete(() =>
            {
                Destroy(page.gameObject);
            });
        }

        private void ShowPage(GameChangesPage page, Action onCompleted)
        {
            page.CanvasGroup.LeanAlpha(1f, 0.3f).setOnComplete(() =>
            {
                onCompleted?.Invoke();
            });
        }

        private void SetSizeDialogByPage(GameChangesPage page, bool smoothly)
        {
            var needSize = page.RectTransform.sizeDelta;
            needSize.x += 200;
            needSize.y += 50;

            if (needSize.x > 1855)
                needSize.x = 1855;
            
            if (needSize.y > 950)
                needSize.y = 950;
            
            if (smoothly)
            {
                _rectTransform.LeanSize(needSize, 0.3f);
            }
            else
            {
                _rectTransform.sizeDelta = needSize;
            }
        }

        private void OnHideDialog()
        {
            
            if (_pages.Count > 0)
                ShowNextPage(true);
            else
                Hide();
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == _closeButton.gameObject)
            {
                OnHideDialog();
            }

            return true;
        }
    }
}