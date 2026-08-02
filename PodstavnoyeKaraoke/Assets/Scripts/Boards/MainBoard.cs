using System;
using System.Collections.Generic;
using System.Linq;
using Boards.Base;
using Controllers;
using Controllers.Levels;
using Dialogs.Base;
using Game.Background;
using Game.Common.Content;
using Game.Pages;
using Game.Pages.Level;
using UnityEngine;
using UnityEngine.UI;

namespace Boards
{
    public class MainBoard : Board
    {
        [SerializeField] private List<Page> _pages = new List<Page>();
        
        protected override void OnEnableBoard()
        {
            OpenPage(TypePage.Main,true);
        }

        protected override void OnDisableBoard()
        {
            
        }

        public void OpenSettingsDialog()
        {
            if(_ignoreTimeLeft > 0)
                return;
            
            if(MainController.Instance.DialogsController.GetCountDialogs() > 0)
                return;
            
            var dialog = MainController.Instance.DialogsController.CreateDialog(TypeDialog.Settings);
            dialog.Init();
            dialog.Show();
        }

        public override void Init()
        {
            base.Init();
            
            InitPages();
            CloseAllPages(false);
        }

        #region Pages

        private void InitPages()
        {
            for (int i = 0; i < _pages.Count; i++)
                _pages[i].Init(this);
        }

        private T GetPageByType<T>(TypePage typePage) where T : Page
        {
            for (int i = 0; i < _pages.Count; i++)
            {
                var page = _pages[i];
                
                if(page.GetTypePage() != typePage)
                    continue;
                
                return page as T;
            }

            return null;
        }

        private Page GetOpenedPage()
        {
            for (int i = 0; i < _pages.Count; i++)
            {
                var page = _pages[i];
                
                if(page.IsOpened)
                    return page;
            }
            
            return null;
        }

        public TypePage GetCurrentTypePage()
        {
            var page = GetOpenedPage();
            if (page == null)
                return TypePage.None;
            
            return page.GetTypePage();
        }

        private void OpenPage(TypePage typePage, bool smoothly, bool isPrevious = false)
        {
            if(_ignoreTimeLeft > 0)
                return;
            
            var delay = 0f;
            
            var previousPage = GetOpenedPage();
            if (previousPage != null)
            {
                if(previousPage.GetTypePage() == typePage)
                    return;
                
                delay = previousPage.Close(smoothly);
            }

            var page = GetPageByType<Page>(typePage);
            var timeOpen = page.Open(smoothly,delay);
            
            if(!isPrevious && previousPage != null) page.SetPreviousPage(previousPage.GetTypePage());
            
            MainController.Instance.ActiveScene.NavigationBar?.OnChangePage(smoothly,delay);
            
            SetIgnoreTime(timeOpen);
        }

        private void CloseAllPages(bool smoothly)
        {
            for (int i = 0; i < _pages.Count; i++)
                _pages[i].Close(smoothly);
        }

        public void OpenMainPage(bool smoothly)
        {
            OpenPage(TypePage.Main, smoothly);
        }
        
        public void OpenListLevelsPage(bool smoothly)
        {
            OpenPage(TypePage.ListLevels, smoothly);
        }

        public void OpenPreviousPage(bool smoothly)
        {
            var currentPage = GetOpenedPage();
            
            if(currentPage.PreviousPage == TypePage.None)
                return;
            
            OpenPage(currentPage.PreviousPage,smoothly, true);
        }
        
        public void StopLevel()
        {
            var currentPage = GetOpenedPage();

            if (currentPage is LevelPage levelPage)
            {
                levelPage.OnClickStopButton();
            }
        }

        public void OpenLevelPage(bool smoothly, ContentsWrapping<LevelData> levelData)
        {
            var page = GetPageByType<LevelPage>(TypePage.Level);
            page.SetLevel(levelData);
            
            OpenPage(TypePage.Level,smoothly);
        }

        #endregion

        protected override void Show(bool smoothly, float delay)
        {
        }

        protected override void Hide(bool smoothly)
        {
        }
    }
}