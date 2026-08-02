using Controllers;
using UnityEngine;

namespace Game
{
    public class MoveBallController : MonoBehaviour
    {
        [SerializeField] private RectTransform _ballContainer;
        [SerializeField] private RectTransform _ball;

        private bool _isPlay;
        private Vector3 _targetPosition;
        private float _speed = 30f;
        
        public void Play()
        {
            _isPlay = true;
        }

        public void Stop()
        {
            _isPlay = false;
        }
        
        #region Update

        private void Update()
        {
            if(!_isPlay)
                return;
            
            UpdateTargetPosition();
            UpdatePosition();
        }

        private void UpdateTargetPosition()
        {
            var ballWeight = MainController.Instance.LocalSettings.GetBallWeight();

            var shiftA = Vector3.down * ballWeight * Time.deltaTime;
            
            //----------------------------
            
            var microphoneVolume = MainController.Instance.MicrophoneController.GetMicrophoneVolume();
            
            var liftingForce = MainController.Instance.LocalSettings.GetLiftingForce();
            
            float jumpForce = microphoneVolume * liftingForce;

            var shiftB = Vector3.up * jumpForce * Time.deltaTime;

            _targetPosition = ClampBallPosition(shiftA + shiftB);
        }

        private void UpdatePosition()
        {
            _ball.transform.localPosition = Vector3.Lerp(_ball.transform.localPosition, _targetPosition, Time.deltaTime * _speed);
        }

        #endregion
        
        private Vector3 ClampBallPosition(Vector3 shift)
        {
            var newPosition = _ball.transform.localPosition + shift;
            
            if(newPosition.y + _ball.sizeDelta.y / 2f > _ballContainer.sizeDelta.y / 2f)
                newPosition.y = _ballContainer.sizeDelta.y / 2f - _ball.sizeDelta.y / 2f;
            else if (newPosition.y - _ball.sizeDelta.y / 2f < -_ballContainer.sizeDelta.y / 2f)
                newPosition.y = -_ballContainer.sizeDelta.y / 2f + _ball.sizeDelta.y / 2f;

            return newPosition;
        }
    }
}