using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonGlare : MonoBehaviour
    {
        [SerializeField] private GameObject _glareGameObject;
        [SerializeField] private bool _playOnAwake;
        [SerializeField] private float _speed = 0.5f;
        [SerializeField] private float _delay = 2f;
        
        private float _buttonWidth;
        private float _glareWidth;
        private float _leftTimeToNextMove;

        private void Awake()
        {
            _glareGameObject.SetActive(false);
            _buttonWidth = gameObject.Size().x;
            _glareWidth = Mathf.Max(_glareGameObject.Size().x, _glareGameObject.Size().y);

            if (_playOnAwake)
                _leftTimeToNextMove = _delay;
        }

        private void Update()
        {
            if(!_playOnAwake)
                return;

            if (_leftTimeToNextMove > 0)
            {
                _leftTimeToNextMove -= Time.deltaTime;
                return;
            }
            
            PlayGlare();

            _leftTimeToNextMove = _delay;
        }

        public void PlayGlare()
        {
            var startXPosition = (-_buttonWidth / 2f) - (_glareWidth / 2f);
            var finishXPosition = (_buttonWidth / 2f) + (_glareWidth / 2f);
            
            _glareGameObject.SetLocalX(startXPosition);
            _glareGameObject.SetActive(true);
            _glareGameObject.LeanMoveLocalX(finishXPosition, _speed).setOnComplete(() => {_glareGameObject.SetActive(false);});
        }
    }
}