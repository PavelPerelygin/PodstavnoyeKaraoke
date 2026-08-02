using System;
using System.Collections.Generic;
using System.Drawing;
using Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Level.Collectable
{
    public class CollectableItemsPanel : MonoBehaviour
    {
        [SerializeField] private List<CollectableItem> _items = new List<CollectableItem>();
        [SerializeField] private Text _fullScoresText;
        
        private LevelPage _levelPage;

        public void Init(LevelPage levelPage)
        {
            _levelPage = levelPage;
        }

        private void UpdateStateItems()
        {
            var needEnableFullScores = false;
            var levelData = _levelPage.ContentsWrapping;

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];

                if (item.GetTypeCollectableItem() == TypeCollectableItem.Bomb)
                {
                    if (levelData.GetContent().GetBombs().Count > 0)
                    {
                        item.Enable();
                        needEnableFullScores = true;
                    }
                    else
                    {
                        item.Disable();
                    }
                }
                else if (item.GetTypeCollectableItem() == TypeCollectableItem.Coin)
                {
                    if (levelData.GetContent().GetCoins().Count > 0)
                    {
                        item.Enable();
                        needEnableFullScores = true;
                    }
                    else
                    {
                        item.Disable();
                    }
                }
                else if (item.GetTypeCollectableItem() == TypeCollectableItem.Gift)
                {
                    if (levelData.GetContent().GetGifts().Count > 0)
                    {
                        item.Enable();
                        needEnableFullScores = true;
                    }
                    else
                    {
                        item.Disable();
                    }
                }
                else if (item.GetTypeCollectableItem() == TypeCollectableItem.Star)
                {
                    if (levelData.GetContent().GetStars().Count > 0)
                    {
                        item.Enable();
                        needEnableFullScores = true;
                    }
                    else
                    {
                        item.Disable();
                    }
                }
                else if (item.GetTypeCollectableItem() == TypeCollectableItem.Ruby)
                {
                    if (levelData.GetContent().GetRubies().Count > 0)
                    {
                        item.Enable();
                        needEnableFullScores = true;
                    }
                    else
                    {
                        item.Disable();
                    }
                }
                
                if(needEnableFullScores) _fullScoresText.gameObject.SetActive(true);
                else _fullScoresText.gameObject.SetActive(false);
            }
        }

        private void UpdateFullScores()
        {
            var scores = 0;

            for (int i = 0; i < _levelPage.CollectableItems.Count; i++)
            {
                var item = _levelPage.CollectableItems[i];
                
                scores += item.GetScores();
            }

            _fullScoresText.text = $"{MainController.Instance.TextManager.GetText(551,true)}: {scores}";
        }

        #region Events

        public void OnSetLevel()
        {
            UpdateStateItems();
            UpdateFullScores();
        }

        public void OnCollectedItem()
        {
            UpdateFullScores();
        }

        public void OnClearAllCollectableItems()
        {
            UpdateStateItems();
            UpdateFullScores();
        }

        #endregion
    }
}