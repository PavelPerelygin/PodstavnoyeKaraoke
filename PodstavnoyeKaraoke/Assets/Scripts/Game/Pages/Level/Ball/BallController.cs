using Controllers;
using Game.Pages.Level.Collectable;
using Game.Pages.Level.Items.Obstacles;
using Game.Pages.Level.Items.Others;
using Managers.Audio;
using UnityEngine;
using Utilities;

namespace Game.Pages.Level.Ball
{
    public class BallController : MonoBehaviour
    {
        [SerializeField] private MoveBallController _moveBallController;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Ball _ball;
        [SerializeField] private RectTransform _defaultBallDummy;
        
        private LevelPage _levelPage;
        private bool _isPlaying;

        public void Init(LevelPage levelPage)
        {
            _levelPage = levelPage;
            
            _ball.Init(this);
        }

        public void Play()
        {
            SetBallToDefaultPosition();
            
            _ball.Show(true);

            _isPlaying = true;
            
            _moveBallController.Play();
        }

        public void Stop()
        {
            _ball.Hide(true);

            _isPlaying = false;
            
            _moveBallController.Stop();
        }

        private void SetBallToDefaultPosition()
        {
            _ball.transform.localPosition = _defaultBallDummy.localPosition;
        }

        #region Event

        public void OnCollisionWithObstacle(Obstacle obstacle)
        {
            Log.Message($"OnCollisionWithObstacle {obstacle.name}");
            
            _levelPage.StopLevel();
        }
        
        public void OnCollisionWithBomb(Bomb bomb)
        {
            if(!_isPlaying)
                return;
            
            Log.Message($"OnCollisionWithBomb {bomb.name}");
            
            bomb.Hide(true,0.4f);
            
            _levelPage.IncrementCollectableItem(TypeCollectableItem.Bomb);

            MainController.Instance.AudioManager.Create("bad_item", TypeGroup.Sound).Play();
        }
        
        public void OnCollisionWithStar(Star star)
        {
            if(!_isPlaying)
                return;
            
            Log.Message($"OnCollisionWithStar {star.name}");
            
            star.Hide(true,0.2f);
            
            _levelPage.IncrementCollectableItem(TypeCollectableItem.Star);
            
            MainController.Instance.AudioManager.Create("good_item", TypeGroup.Sound).Play();
        }
        
        public void OnCollisionWithCoin(Coin coin)
        {
            if(!_isPlaying)
                return;
            
            Log.Message($"OnCollisionWithCoin {coin.name}");
            
            coin.Hide(true,0.2f);
            
            _levelPage.IncrementCollectableItem(TypeCollectableItem.Coin);
            
            MainController.Instance.AudioManager.Create("good_item", TypeGroup.Sound).Play();
        }
        
        public void OnCollisionWithGift(Gift gift)
        {
            if(!_isPlaying)
                return;
            
            Log.Message($"OnCollisionWithCoin {gift.name}");
            
            gift.Hide(true,0.2f);
            
            _levelPage.IncrementCollectableItem(TypeCollectableItem.Gift);
            
            MainController.Instance.AudioManager.Create("good_item", TypeGroup.Sound).Play();
        }
        
        public void OnCollisionWithRuby(Ruby ruby)
        {
            if(!_isPlaying)
                return;
            
            Log.Message($"OnCollisionWithCoin {ruby.name}");
            
            ruby.Hide(true,0.2f);
            
            _levelPage.IncrementCollectableItem(TypeCollectableItem.Ruby);
            
            MainController.Instance.AudioManager.Create("good_item", TypeGroup.Sound).Play();
        }

        #endregion
    }
}