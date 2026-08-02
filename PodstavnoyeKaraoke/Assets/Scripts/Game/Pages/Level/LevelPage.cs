using System;
using System.Collections.Generic;
using Boards;
using Boards.Base;
using Controllers;
using Controllers.Levels;
using Dialogs;
using Dialogs.Base;
using Extensions;
using Fades;
using Game.Common.Content;
using Game.Pages.Common.SkinItem;
using Game.Pages.Level.Ball;
using Game.Pages.Level.Collectable;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Game.Pages.Level
{
    public class LevelPage : Page
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Track _track;
        [SerializeField] private BallController _ballController;
        [SerializeField] private CollectableItemsPanel _collectableItemsPanel;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _playButton;
        
        [SerializeField] private List<RawImageSkin> _rawImageSkins = new List<RawImageSkin>();
        [SerializeField] private List<ImageSkin> _imageSkins = new List<ImageSkin>();
        
        private LtdManager _ltdManager = new LtdManager();
        private GameObjectInfo _playButtonInfo;
        private StateLevel _stateLevel;
        private float _timeLevel;
        
        public ContentsWrapping<LevelData> ContentsWrapping {get; private set;}
        public List<CollectableItemData> CollectableItems {get; private set;} = new List<CollectableItemData>();
        public Font TextFont { get; private set; }
        public int SizeFont { get; private set; }
        
        public float ViewPortWidth { get; private set; }
        public float ContentWidth { get; private set; }

        #region Init

        public override void Init(MainBoard mainBoard)
        {
            base.Init(mainBoard);
            
            _scrollRect.enabled = false;
            
            ViewPortWidth = _scrollRect.viewport.rect.width;
            
            _track.Init(this);
            _ballController.Init(this);
            _collectableItemsPanel.Init(this);
            
            InitButton();
        }

        private void InitButton()
        {
            _playButtonInfo = new GameObjectInfo(_playButton.gameObject);
            _playButton.onClick.AddListener(ButtonPress);
            _playButton.DisableOverDownColors();
        }

        #endregion

        public override TypePage GetTypePage()
        {
            return TypePage.Level;
        }
        
        #region Lelev

        public void SetLevel(ContentsWrapping<LevelData> contentsWrapping)
        {
            SetStateLevel(StateLevel.Stop);
            
            ContentsWrapping = contentsWrapping;

            _scrollRect.ScrollToNormalizedPositionX(0);

            UpdateFont();
            BuildLevel();
            ClearAllCollectableItems();
            
            _collectableItemsPanel.OnSetLevel();
        }

        private void UpdateFont()
        {
            TextFont = FontSettings.GetFontByName(ContentsWrapping.GetContent().GetTextFont());
            SizeFont = ContentsWrapping.GetContent().GetSizeFont();
        }
        
        private void BuildLevel()
        {
            RecalculateSizeGameField();
            BuildObstacles();
            BuildBomb();
            BuildCoin();
            BuildGift();
            BuildRuby();
            BuildStar();
            BuildText();
            
            HideTexts(false);
        }
        
        private void RecalculateSizeGameField()
        {
            var lenghtAudioClip = ContentsWrapping.GetContent().GetLenghtFinalAudioClip();
            var width = ContentsWrapping.GetContent().GetSpeed() * lenghtAudioClip;
            
            ContentWidth = width + ViewPortWidth;
            
            _scrollRect.content.SetSizeX(ContentWidth);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
            
            _track.RecalculateSize();
        }
        
        private void SetHorizontalNormalizedPosition(float value)
        {
            _scrollRect.horizontalNormalizedPosition = value;
        }
        
        #region Obstacle build / add / remove

        private void BuildObstacles()
        {
            ClearObstacles();
            
            var obstacles = ContentsWrapping.GetContent().GetObstacles();

            for (int i = 0; i < obstacles.Count; i++)
            {
                _track.CreateObstacle(obstacles[i]);
            }
        }

        private void ClearObstacles()
        {
            _track.ClearObstacles();
        }

        #endregion
        
        #region Text build / add / remove / show / hide

        private void BuildText()
        {
            ClearText();
            
            var texts = ContentsWrapping.GetContent().GetTexts();

            for (int i = 0; i < texts.Count; i++)
            {
                _track.CreateText(texts[i]);
            }
        }
        
        private void ClearText()
        {
            _track.ClearText();
        }

        private void ShowTexts(bool smoothly, Action onComplete = null)
        {
            _track.ShowTexts(smoothly,onComplete);
        }
        
        private void HideTexts(bool smoothly, Action onComplete = null)
        {
            _track.HideTexts(smoothly,onComplete);
        }

        #endregion

        #region Other items build / add / remove

        private void BuildBomb()
        {
            ClearBomb();
            
            var bombs = ContentsWrapping.GetContent().GetBombs();

            for (int i = 0; i < bombs.Count; i++)
            {
                _track.CreateBomb(bombs[i]);
            }
        }
        
        private void ClearBomb()
        {
            _track.ClearBomb();
        }
        
        private void BuildStar()
        {
            ClearStar();

            var stars = ContentsWrapping.GetContent().GetStars();

            for (int i = 0; i < stars.Count; i++)
            {
                _track.CreateStar(stars[i]);
            }
        }

        private void ClearStar()
        {
            _track.ClearStar();
        }

        private void BuildCoin()
        {
            ClearCoin();

            var coins = ContentsWrapping.GetContent().GetCoins();

            for (int i = 0; i < coins.Count; i++)
            {
                _track.CreateCoin(coins[i]);
            }
        }

        private void ClearCoin()
        {
            _track.ClearCoin();
        }
        
        private void BuildGift()
        {
            ClearGift();

            var gifts = ContentsWrapping.GetContent().GetGifts();

            for (int i = 0; i < gifts.Count; i++)
            {
                _track.CreateGift(gifts[i]);
            }
        }

        private void ClearGift()
        {
            _track.ClearGift();
        }

        private void BuildRuby()
        {
            ClearRuby();

            var rubies = ContentsWrapping.GetContent().GetRubies();

            for (int i = 0; i < rubies.Count; i++)
            {
                _track.CreateRuby(rubies[i]);
            }
        }

        private void ClearRuby()
        {
            _track.ClearRuby();
        }

        #endregion

        #endregion

        #region State level

        private void CloseLevel()
        {
            SetStateLevel(StateLevel.Stop);
            
            _ballController.Stop();
            
            _audioSource.Stop();
        }

        public void PlayLevel()
        {
            SetStateLevel(StateLevel.Play);

            _timeLevel = 0;
            
            SetHorizontalNormalizedPosition(0);

            HidePlayButton(true, 0);
            ShowTexts(true);
            _track.ShowAllOtherItems(true);
                
            _ballController.Play();
            
            _audioSource.clip = ContentsWrapping.GetContent().GetFinalAudioClip();
            _audioSource.Play();
        }

        public void StopLevel()
        {
            SetStateLevel(StateLevel.Stop);
            
            ShowPlayButton(true, 0.4f);
            
            HideTexts(true);
            
            _ballController.Stop();
            
            _audioSource.Stop();
        }

        public void FinishLevel()
        {
            SetStateLevel(StateLevel.Finish);
            
            ContentsWrapping.GetContent().SetIsFinished(true);
        }

        private void SetStateLevel(StateLevel stateLevel)
        {
            _stateLevel = stateLevel;
        }

        private StateLevel GetStateLevel()
        {
            return _stateLevel;
        }
        
        #endregion

        #region Collectable Items

        public void IncrementCollectableItem(TypeCollectableItem typeCollectableItem)
        {
            var item = GetOrCreateCollectableItem(typeCollectableItem);
            item.IncrementCount();
            
            _collectableItemsPanel.OnCollectedItem();
        }

        private CollectableItemData GetOrCreateCollectableItem(TypeCollectableItem typeCollectableItem)
        {
            var item = GetCollectableItem(typeCollectableItem);
            if(item != null)
                return item;
            
            item = new CollectableItemData(typeCollectableItem);
            CollectableItems.Add(item);
            
            return item;
        }

        private CollectableItemData GetCollectableItem(TypeCollectableItem typeCollectableItem)
        {
            for (int i = 0; i < CollectableItems.Count; i++)
            {
                var collectableItem = CollectableItems[i];
                if (collectableItem.GetTypeCollectableItem() == typeCollectableItem)
                    return collectableItem;
            }

            return null;
        }

        private void ClearAllCollectableItems()
        {
            CollectableItems.Clear();
            
            _collectableItemsPanel.OnClearAllCollectableItems();
        }

        #endregion

        #region Update

        protected override void Update()
        {
            base.Update();
            
            UpdateProgressLevel();
            UpdatePlayLevel();
        }

        private void UpdateProgressLevel()
        {
            if(GetStateLevel() != StateLevel.Play)
                return;
            
            if(!_audioSource.isPlaying)
                return;

            _timeLevel += Time.deltaTime;

            var progress = _timeLevel / _audioSource.clip.length;
            
            SetHorizontalNormalizedPosition(progress);
        }
        
        private void UpdatePlayLevel()
        {
            if(GetStateLevel() != StateLevel.Play)
                return;
            
            if ((_audioSource.time >= _audioSource.clip.length) ||(_audioSource.time <= 0 && !_audioSource.isPlaying))
            {
                FinishLevel();
            }
        }

        #endregion

        #region Open / close

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
            
            var playButtonTime = ShowPlayButton(smoothly, fadeTime,() =>
            {
                count--;
                if(count <= 0)
                    onComplete?.Invoke();
            });

            return Mathf.Max(fadeTime,playButtonTime);
        }

        private float ShowPlayButton(bool smoothly, float delay, Action onComplete = null)
        {
            var time = 0.3f;
            
            _ltdManager.TryCancelLtd("play_button_move");
            
            var position = _playButtonInfo.localPosition;

            if (smoothly)
            {
                var moveTween = _playButton.gameObject.LeanMoveLocal(position, time).setEase(LeanTweenType.easeOutBack).setDelay(delay)
                    .setOnComplete(() =>
                    {
                        _ltdManager.TrySetNullLtd("play_button_move");
                        
                        onComplete?.Invoke();
                    });
                
                _ltdManager.AddLtd("play_button_move",moveTween);
            }
            else
            {
                _playButton.transform.localPosition = position;
                
                onComplete?.Invoke();
            }
            
            return time + delay;
        }

        public override float Close(bool smoothly, Action onComplete = null)
        {
            base.Close(smoothly, onComplete);
            
            CloseLevel();

            var playButtonTime = HidePlayButton(smoothly, 0f);
            
            var fadeTime = HideFade(smoothly, playButtonTime, () =>
            {
                _root.gameObject.SetActive(false);
            });

            return Mathf.Max(playButtonTime,fadeTime);
        }
        
        private float HidePlayButton(bool smoothly, float delay, Action onComplete = null)
        {
            var time = 0.3f;
            
            _ltdManager.TryCancelLtd("play_button_move");

            var position = _playButton.gameObject.GetPositionOffScreenByDirection(Vector2.down, true);

            if (smoothly)
            {
                var moveTween = _playButton.gameObject.LeanMoveLocal(position, time).setEase(LeanTweenType.easeInBack).setDelay(delay)
                    .setOnComplete(() =>
                    {
                        _ltdManager.TrySetNullLtd("play_button_move");
                        
                        onComplete?.Invoke();
                    });
                
                _ltdManager.AddLtd("play_button_move",moveTween);
            }
            else
            {
                _playButton.transform.localPosition = position;
                
                onComplete?.Invoke();
            }
            
            return time + delay;
        }

        #endregion

        private void OnClickPlayLevelButton()
        {
            var dialog = MainController.Instance.DialogsController.CreateDialog(TypeDialog.Delay) as DelayDialog;
            if (dialog != null)
            {
                HidePlayButton(true, 0);
                
                dialog.Init(3, PlayLevel);
                dialog.Show();
            }
        }

        public void OnClickStopButton()
        {
            ClearAllCollectableItems();
            
            StopLevel();
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;

            if (MainController.Instance.DialogsController.GetCountDialogs() > 0)
                return false;

            if (selectedObj == _playButton.gameObject)
            {
                OnClickPlayLevelButton();
            }

            return true;
        }
        
        #region Events

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion
    }
}