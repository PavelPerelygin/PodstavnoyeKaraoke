using System;
using Controllers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Game.Pages.Level.Items
{
    public class GameFieldItem : MonoBehaviour
    {
        [SerializeField] protected RectTransform _rectTransform;
        
        private bool _wasInitialized;
        
        protected Track _track;
        
        public RectTransform RectTransform => _rectTransform;
        
        public void Init(Track track)
        {
            _track = track;

            _wasInitialized = true;
        }

        #region Show / hide

        public virtual void Show(bool smoothly, float time, float delay = 0f, Action onComplete = null)
        {
            gameObject.SetActive(true);
            
            onComplete?.Invoke();
        }
        
        public virtual void Hide(bool smoothly, float time, float delay = 0f, Action onComplete = null)
        {
            gameObject.SetActive(false);
            
            onComplete?.Invoke();
        }

        #endregion

        #region Position

        protected virtual void SetGlobalPosition(Vector3 position)
        {
            transform.position = position;

            UpdatePosition();
        }

        protected virtual void SetLocalPosition(Vector3 position)
        {
            transform.localPosition = position;

            UpdatePosition();
        }

        protected virtual void UpdatePosition() { }

        #endregion
    }
}