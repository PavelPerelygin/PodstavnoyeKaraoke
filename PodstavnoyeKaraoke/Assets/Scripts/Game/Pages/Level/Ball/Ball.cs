using Game.Pages.Level.Items.Obstacles;
using Game.Pages.Level.Items.Others;
using UnityEngine;

namespace Game.Pages.Level.Ball
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        private LTDescr _scaleLtd;
        private BallController _ballController;
        
        public RectTransform RectTransform => _rectTransform;

        public void Init(BallController ballController)
        {
            _ballController = ballController;
            
            Hide(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out Obstacle obstacle))
            {
                _ballController.OnCollisionWithObstacle(obstacle);
            }
            else if (other.gameObject.TryGetComponent(out Bomb bomb))
            {
                _ballController.OnCollisionWithBomb(bomb);
            }
            else if (other.gameObject.TryGetComponent(out Star star))
            {
                _ballController.OnCollisionWithStar(star);
            }
            else if (other.gameObject.TryGetComponent(out Coin coin))
            {
                _ballController.OnCollisionWithCoin(coin);
            }
            else if (other.gameObject.TryGetComponent(out Gift gift))
            {
                _ballController.OnCollisionWithGift(gift);
            }
            else if (other.gameObject.TryGetComponent(out Ruby ruby))
            {
                _ballController.OnCollisionWithRuby(ruby);
            }
        }

        #region Show / hide

        public void Show(bool smoothly)
        {
            TryCancelScaleLtd();

            if (smoothly)
            {
                _scaleLtd = gameObject.LeanScale(Vector3.one, 0.3f).setEase(LeanTweenType.easeOutBack).setOnComplete(
                    () =>
                    {
                        _scaleLtd = null;
                    });
            }
            else
            {
                transform.localScale = Vector3.one;
            }
        }
        
        public void Hide(bool smoothly)
        {
            TryCancelScaleLtd();

            if (smoothly)
            {
                _scaleLtd = gameObject.LeanScale(Vector3.zero, 0.3f).setEase(LeanTweenType.easeInBack).setOnComplete(
                    () =>
                    {
                        _scaleLtd = null;
                    });
            }
            else
            {
                transform.localScale = Vector3.zero;
            }
        }

        private void TryCancelScaleLtd()
        {
            if(_scaleLtd == null)
                return;
            
            LeanTween.cancel(_scaleLtd.id);
            _scaleLtd = null;
        }

        #endregion
    }
}