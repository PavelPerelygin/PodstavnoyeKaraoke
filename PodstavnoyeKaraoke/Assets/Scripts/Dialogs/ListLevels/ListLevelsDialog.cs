using System;
using System.Collections.Generic;
using Controllers;
using Controllers.Levels;
using Dialogs.Base;
using Extensions;
using Game.Common.Content;
using Managers.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.ListLevels
{
    public class ListLevelsDialog : Dialog
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _addButton;
        [SerializeField] private LevelItem _levelItemPrefab;
        
        private bool _lockDialog;
        private List<LevelItem> _levelItems = new List<LevelItem>();
        
        public override void Init()
        {
            InitButtons();
            
            BuildLevelItems();
            
            MainController.Instance.LevelsController.OnSortingContent += OnSortingContent;
        }

        private void InitButtons()
        {
            _closeButton.onClick.AddListener(ButtonPress);
            _closeButton.DisableOverDownColors();
            
            _addButton.onClick.AddListener(ButtonPress);
            _addButton.DisableOverDownColors();
        }

        private void BuildLevelItems()
        {
            ClearLevelItems();
            
            var levels = MainController.Instance.LevelsController.GetContents();

            for (int i = 0; i < levels.Count; i++)
            {
                CreateLevelItem(levels[i]);
            }
        }

        private void ClearLevelItems()
        {
            for (int i = 0; i < _levelItems.Count; i++)
                Destroy(_levelItems[i].gameObject);
            
            _levelItems.Clear();
        }

        private void CreateLevelItem(ContentsWrapping<LevelData> contentsWrapping)
        {
            var item = Instantiate(_levelItemPrefab, _scrollRect.content);
            item.Init(this,contentsWrapping);
            
            _levelItems.Add(item);
        }

        private void AddLevel()
        {
            MainController.Instance.LevelsController.OpenContent((string result) =>
            {
                if (result == "need_update")
                {
                    LeanTween.delayedCall(1f, () =>
                    {
                        _lockDialog = false;
                        
                        var dialog = MainController.Instance.DialogsController.CreateDialog(TypeDialog.Update) as UpdateDialog;
                        if (dialog != null)
                        {
                            dialog.Init(MainController.Instance.TextManager.GetText(562), GameSettings.Instance.GameDownloadUrl);
                            dialog.Show(0);   
                        }
                    });
                }
                else if (result == "package_has_already_been_added")
                {
                    _lockDialog = false;
                    
                    LeanTween.delayedCall(1f, () =>
                    {
                        _lockDialog = false;
                        
                        var dialog = MainController.Instance.DialogsController.CreateDialog(TypeDialog.Info) as InfoDialog;
                        if (dialog != null)
                        {
                            dialog.Init(MainController.Instance.TextManager.GetText(563), MainController.Instance.TextManager.GetText(566));
                            dialog.Show(0);   
                        }
                    });
                }
                else if (result == "not_valid_sum")
                {
                    _lockDialog = false;
                    
                    LeanTween.delayedCall(1f, () =>
                    {
                        _lockDialog = false;
                        
                        var dialog = MainController.Instance.DialogsController.CreateDialog(TypeDialog.Info) as InfoDialog;
                        if (dialog != null)
                        {
                            dialog.Init(MainController.Instance.TextManager.GetText(563), MainController.Instance.TextManager.GetText(567));
                            dialog.Show(0);   
                        }
                    });
                }
                else if (result == "not_valid_from_server")
                {
                    _lockDialog = false;
                    
                    LeanTween.delayedCall(1f, () =>
                    {
                        _lockDialog = false;
                        
                        var dialog = MainController.Instance.DialogsController.CreateDialog(TypeDialog.Info) as InfoDialog;
                        if (dialog != null)
                        {
                            dialog.Init(MainController.Instance.TextManager.GetText(563), MainController.Instance.TextManager.GetText(568));
                            dialog.Show(0);   
                        }
                    });
                }
                else
                {
                    _lockDialog = false;
                }
            });
        }

        public void RemoveLevel(LevelItem levelItem)
        {
            if (_lockDialog)
                return;
            
            MainController.Instance.LevelsController.RemoveContent(levelItem.ContentsWrapping);
        }

        private void OnSortingContent()
        {
            BuildLevelItems();
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;
            
            if (_lockDialog)
                return false;

            if (selectedObj == _closeButton.gameObject)
            {
                Hide();
            }
            else if (selectedObj == _addButton.gameObject)
            {
                AddLevel();
            }

            return true;
        }

        private void OnDestroy()
        {
            MainController.Instance.LevelsController.OnSortingContent -= OnSortingContent;
        }
    }
}