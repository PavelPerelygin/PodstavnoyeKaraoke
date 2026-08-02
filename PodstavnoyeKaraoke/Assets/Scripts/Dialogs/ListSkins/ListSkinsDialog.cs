using System;
using System.Collections.Generic;
using Controllers;
using Controllers.Skins;
using Dialogs.Base;
using Extensions;
using Game.Common.Content;
using Managers.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.ListSkins
{
    public class ListSkinsDialog : Dialog
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _addButton;
        [SerializeField] private SkinItem _skinItemPrefab;
        
        private bool _lockDialog;
        private List<SkinItem> _skinItems = new List<SkinItem>();

        public override void Init()
        {
            InitButtons();
            
            BuildSkinItems();
            
            MainController.Instance.SkinsController.OnSortingContent += OnSortingContent;
        }

        private void InitButtons()
        {
            _closeButton.onClick.AddListener(ButtonPress);
            _closeButton.DisableOverDownColors();
            
            _addButton.onClick.AddListener(ButtonPress);
            _addButton.DisableOverDownColors();
        }

        private void BuildSkinItems()
        {
            ClearSkinItems();

            CreateDefaultSkin();
            
            var skins = MainController.Instance.SkinsController.GetContents();

            for (int i = 0; i < skins.Count; i++)
            {
                CreateSkinItem(skins[i]);
            }
        }

        private void CreateDefaultSkin()
        {
            var skinData = new SkinData();
            skinData.SetSkinName(SkinData.DefaultSkinName);
            
            var contentWrapper = new ContentsWrapping<SkinData>(skinData,true);
            
            CreateSkinItem(contentWrapper);
        }

        private void ClearSkinItems()
        {
            for (int i = 0; i < _skinItems.Count; i++)
            {
                if(_skinItems[i].gameObject != null)
                    Destroy(_skinItems[i].gameObject);   
            }
            
            _skinItems.Clear();
        }

        private void CreateSkinItem(ContentsWrapping<SkinData> contentsWrapping)
        {
            var item = Instantiate(_skinItemPrefab, _scrollRect.content);
            item.Init(this,contentsWrapping);
            
            _skinItems.Add(item);
        }

        private void OnSortingContent()
        {
            BuildSkinItems();
        }

        private void AddSkin()
        {
            MainController.Instance.SkinsController.OpenContent((string result) =>
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
                            dialog.Init(MainController.Instance.TextManager.GetText(563), MainController.Instance.TextManager.GetText(564));
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
                            dialog.Init(MainController.Instance.TextManager.GetText(563), MainController.Instance.TextManager.GetText(565));
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

        public void RemoveSkin(SkinItem skinItem)
        {
            if (_lockDialog)
                return;
            
            MainController.Instance.SkinsController.RemoveContent(skinItem.ContentsWrapping);
        }

        public void EnableSkin(SkinItem skinItem)
        {
            for (int i = 0; i < _skinItems.Count; i++)
            {
                var item = _skinItems[i];

                if (item == skinItem)
                {
                    var skinName = item.ContentsWrapping.GetContent().GetNameSkin();
                    MainController.Instance.LocalSettings.SetSkinName(skinName);
                }
                else
                {
                    item.DisableSkin();
                }
            }
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if(!base.GameObjectClickHandler(selectedObj))
                return false;
            
            if (_lockDialog)
                return false;

            if (selectedObj == _closeButton.gameObject)
            {
                Hide();
            }
            else if (selectedObj == _addButton.gameObject)
            {
                AddSkin();
            }

            return true;
        }

        private void OnDestroy()
        {
            MainController.Instance.SkinsController.OnSortingContent -= OnSortingContent;
        }
    }
}