using System;
using System.Collections.Generic;
using Controllers;
using Controllers.Levels;
using Extensions;
using Game.Pages.Common.SkinItem;
using Game.Pages.Level.Items.Obstacles;
using Game.Pages.Level.Items.Others;
using UnityEngine;
using UnityEngine.UI;
using Text = Game.Pages.Level.Items.Texts.Text;

namespace Game.Pages.Level
{
    public class Track : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _obstaclesContainer;
        [SerializeField] private RectTransform _othersContainer;
        [SerializeField] private RectTransform _textsContainer;
        
        [SerializeField] private List<RawImageSkin> _rawImageSkins = new List<RawImageSkin>();
        [SerializeField] private List<ImageSkin> _imageSkins = new List<ImageSkin>();
        
        private List<Obstacle> _obstacles = new List<Obstacle>();
        private List<Text> _texts = new List<Text>();
        private List<Bomb> _bombs = new List<Bomb>();
        private List<Star> _stars = new List<Star>();
        private List<Coin> _coins = new List<Coin>();
        private List<Gift> _gifts = new List<Gift>();
        private List<Ruby> _rubies = new List<Ruby>();
        
        public LevelPage LevelPage { get; private set; }

        public void Init(LevelPage levelPage)
        {
            LevelPage = levelPage;

            UpdateSkin();

            MainController.Instance.LocalSettings.OnChangeSkin += UpdateSkin;
        }

        #region Obstacles create

        public void CreateObstacle(ObstacleData obstacleData)
        {
            var prefab = Resources.Load<Obstacle>($"Prefabs/Game/Pages/Level/Obstacles/{obstacleData.GetNameObstacle()}");
            var item = Instantiate(prefab, _obstaclesContainer);
            item.Init(this,obstacleData);
            
            _obstacles.Add(item);
        }
        
        public void ClearObstacles()
        {
            for (int i = 0; i < _obstacles.Count; i++)
                Destroy(_obstacles[i].gameObject);
            
            _obstacles.Clear();
        }

        #endregion

        #region Text create / clear / show / hide

        public void CreateText(TextData textData)
        {
            var prefab = Resources.Load<Text>($"Prefabs/Game/Pages/Level/Text/Text");
            var item = Instantiate(prefab, _textsContainer);
            item.Init(this,textData);
            
            _texts.Add(item);
        }
        
        public void ClearText()
        {
            for (int i = 0; i < _texts.Count; i++)
                Destroy(_texts[i].gameObject);
            
            _texts.Clear();
        }

        public void ShowTexts(bool smoothly, Action onComplete = null)
        {
            var time = 0.3f;
            var count = _texts.Count;

            for (int i = 0; i < _texts.Count; i++)
            {
                var text = _texts[i];
                text.Show(smoothly,time,0f, () =>
                {
                    count--;
                    if(count <= 0)
                        onComplete?.Invoke();
                });
            }
        }
        
        public void HideTexts(bool smoothly, Action onComplete = null)
        {
            var time = 0.3f;
            var count = _texts.Count;

            for (int i = 0; i < _texts.Count; i++)
            {
                var text = _texts[i];
                text.Hide(smoothly,time,0f, () =>
                {
                    count--;
                    if(count <= 0)
                        onComplete?.Invoke();
                });
            }
        }

        #endregion
        
        #region Orher item create / remove / show / hide
        
        public void CreateBomb(BombData bombData)
        {
            var prefab = Resources.Load<Bomb>($"Prefabs/Game/Pages/Level/Others/Bomb");
            var item = Instantiate(prefab, _othersContainer);
            item.Init(this,bombData);
            
            _bombs.Add(item);
        }

        public void ClearBomb()
        {
            for (int i = 0; i < _bombs.Count; i++)
                Destroy(_bombs[i].gameObject);
            
            _bombs.Clear();
        }
        
        public void CreateStar(StarData starData)
        {
            var prefab = Resources.Load<Star>($"Prefabs/Game/Pages/Level/Others/Star");
            var item = Instantiate(prefab, _othersContainer);
            item.Init(this, starData);

            _stars.Add(item);
        }

        public void ClearStar()
        {
            for (int i = 0; i < _stars.Count; i++)
                Destroy(_stars[i].gameObject);

            _stars.Clear();
        }
        
        public void CreateCoin(CoinData coinData)
        {
            var prefab = Resources.Load<Coin>($"Prefabs/Game/Pages/Level/Others/Coin");
            var item = Instantiate(prefab, _othersContainer);
            item.Init(this, coinData);

            _coins.Add(item);
        }

        public void ClearCoin()
        {
            for (int i = 0; i < _coins.Count; i++)
                Destroy(_coins[i].gameObject);

            _coins.Clear();
        }
        
        public void CreateGift(GiftData giftData)
        {
            var prefab = Resources.Load<Gift>($"Prefabs/Game/Pages/Level/Others/Gift");
            var item = Instantiate(prefab, _othersContainer);
            item.Init(this, giftData);

            _gifts.Add(item);
        }

        public void ClearGift()
        {
            for (int i = 0; i < _gifts.Count; i++)
                Destroy(_gifts[i].gameObject);

            _gifts.Clear();
        }
        
        public void CreateRuby(RubyData rubyData)
        {
            var prefab = Resources.Load<Ruby>($"Prefabs/Game/Pages/Level/Others/Ruby");
            var item = Instantiate(prefab, _othersContainer);
            item.Init(this, rubyData);

            _rubies.Add(item);
        }

        public void ClearRuby()
        {
            for (int i = 0; i < _rubies.Count; i++)
                Destroy(_rubies[i].gameObject);

            _rubies.Clear();
        }

        public void ShowAllOtherItems(bool smoothly)
        {
            for (int i = 0; i < _bombs.Count; i++)
                _bombs[i].Show(smoothly);
            
            for (int i = 0; i < _stars.Count; i++)
                _stars[i].Show(smoothly);
            
            for (int i = 0; i < _coins.Count; i++)
                _coins[i].Show(smoothly);
            
            for (int i = 0; i < _gifts.Count; i++)
                _gifts[i].Show(smoothly);
            
            for (int i = 0; i < _rubies.Count; i++)
                _rubies[i].Show(smoothly);
        }

        #endregion

        #region Events

        private void UpdateSkin()
        {
            for (int i = 0; i < _rawImageSkins.Count; i++)
            {
                var rawImageSkin = _rawImageSkins[i];
                var nameResource = rawImageSkin.GetNameResource();
                var texture = MainController.Instance.SkinsController.GetTexture2DByName(nameResource);
                rawImageSkin.SetTexture2D(texture);
            }
            
            for (int i = 0; i < _imageSkins.Count; i++)
            {
                var imageSkin = _imageSkins[i];
                var nameResource = imageSkin.GetNameResource();
                var sprite = MainController.Instance.SkinsController.GetSpriteByName(nameResource);
                imageSkin.SetSprite(sprite);
            }

            for (int i = 0; i < _obstacles.Count; i++)
                _obstacles[i].UpdateSkin();
            
            for (int i = 0; i < _texts.Count; i++)
                _texts[i].UpdateSkin();
        }
        
        public void RecalculateSize()
        {
            var needWidth = _rectTransform.rect.width - LevelPage.ViewPortWidth;
            _obstaclesContainer.SetSizeX(needWidth);
            _obstaclesContainer.LeanSetLocalPosX(0);
            _textsContainer.SetSizeX(needWidth);
            _textsContainer.LeanSetLocalPosX(0);
        }

        private void OnDestroy()
        {
            MainController.Instance.LocalSettings.OnChangeSkin -= UpdateSkin;
        }

        #endregion
    }
}