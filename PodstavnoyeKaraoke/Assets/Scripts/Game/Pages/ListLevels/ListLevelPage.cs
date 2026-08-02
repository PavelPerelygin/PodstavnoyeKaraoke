using System;
using System.Collections.Generic;
using Boards;
using Controllers;
using Controllers.Levels;
using Game.Common.Content;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.Pages.ListLevels
{
    public class ListLevelPage : Page
    {
        [SerializeField] private ScrollRect _levelsScrollRect;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private List<LevelItem> _levelItemPrefabs = new List<LevelItem>();

        private int _currentIndexPrefab;
        private LtdManager _ltdManager = new LtdManager();
        private List<LevelItem> _levelItems = new List<LevelItem>();

        #region Init

        public override void Init(MainBoard mainBoard)
        {
            base.Init(mainBoard);
            
            BuildLevelItems();
            
            MainController.Instance.LevelsController.OnSortingContent += OnSortingContent;
            MainController.Instance.LocalSettings.OnChangeSkin += UpdateSkin;
        }

        private void BuildLevelItems()
        {
            ClearLevelItems();
            
            _currentIndexPrefab = 0;
            
            var levels = MainController.Instance.LevelsController.GetContents();

            for (int i = 0; i < levels.Count; i++)
                CreateLevelItem(levels[i]);
            
            if(IsOpened)
                ShowLevelItems(true,0);

            UpdateSkin();
        }

        private void CreateLevelItem(ContentsWrapping<LevelData> levelData)
        {
            var item = Instantiate(GetLevelItemPrefab(),_levelsScrollRect.content);
            item.Init(levelData,this);
            item.Hide(false,0f,0f);
            
            _levelItems.Add(item);
        }

        private LevelItem GetLevelItemPrefab()
        {
            if (_currentIndexPrefab > _levelItemPrefabs.Count - 1)
                _currentIndexPrefab = 0;
            
            var prefab = _levelItemPrefabs[_currentIndexPrefab];
            _currentIndexPrefab++;

            return prefab;
        }

        private void ClearLevelItems()
        {
            for (int i = 0; i < _levelItems.Count; i++)
                Destroy(_levelItems[i].gameObject);
            
            _levelItems.Clear();
        }

        private void OnSortingContent()
        {
            ClearLevelItems();
            BuildLevelItems();
        }

        #endregion

        public override TypePage GetTypePage()
        {
            return TypePage.ListLevels;
        }

        public void OpenLevel(LevelItem levelItem)
        {
            _mainBoard.OpenLevelPage(true,levelItem.СontentsWrapping);
        }

        public override float Open(bool smoothly, float delay, Action onComplete = null)
        {
            base.Open(smoothly, delay, onComplete);

            var count = 2;
            
            _root.gameObject.SetActive(true);

            var fadeTime = ShowFade(smoothly, delay, () =>
            {
                count--;
                if(count <= 0)
                    onComplete?.Invoke();
            });
            
            var timeLevelItems = ShowLevelItems(smoothly, fadeTime, () =>
            {
                count--;
                if(count <= 0)
                    onComplete?.Invoke();
            });
            
            return Mathf.Max(fadeTime,timeLevelItems);
        }
        
        private float ShowLevelItems(bool smoothly, float delay, Action onComplete = null)
        {
            var time = 0.5f;
            
            var d = time / _levelItems.Count;
            var showTime = time - d;
            if (showTime <= 0) showTime = time;
            var count = _levelItems.Count;
            
            for (int i = 0; i < _levelItems.Count; i++)
            {
                var item = _levelItems[i];
                
                item.Show(smoothly,(d * i) + delay, showTime, () =>
                {
                    count--;
                    if(count <= 0)
                        onComplete?.Invoke();
                });
            }
            
            return time + delay;
        }

        public override float Close(bool smoothly, Action onComplete = null)
        {
            base.Close(smoothly, onComplete);
            
            var timeLevelItems = HideLevelItems(smoothly,0f);

            var fadeTime = HideFade(smoothly, timeLevelItems, () =>
            {
                _root.gameObject.SetActive(false);
            });
            
            return Mathf.Max(timeLevelItems,fadeTime);
        }
        
        private float HideLevelItems(bool smoothly, float delay, Action onComplete = null)
        {
            var time = 0.5f;
            
            var d = time / _levelItems.Count;
            var hideTime = time - d;
            if (hideTime <= 0) hideTime = time;
            var count = _levelItems.Count;
            
            for (int i = 0; i < _levelItems.Count; i++)
            {
                var item = _levelItems[i];
                
                item.Hide(smoothly,(d * i) + delay, hideTime, () =>
                {
                    count--;
                    if(count <= 0)
                        onComplete?.Invoke();
                });
            }
            
            return time + delay;
        }

        #region Events
        
        private void UpdateSkin()
        {
            for (int i = 0; i < _levelItems.Count; i++)
            {
                _levelItems[i].UpdateSkin();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            MainController.Instance.LevelsController.OnSortingContent -= OnSortingContent;
            MainController.Instance.LocalSettings.OnChangeSkin -= UpdateSkin;
        }

        #endregion
    }
}