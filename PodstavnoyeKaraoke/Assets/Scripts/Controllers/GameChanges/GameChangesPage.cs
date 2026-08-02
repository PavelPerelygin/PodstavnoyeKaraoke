using UnityEngine;

namespace Controllers.GameChanges
{
    public class GameChangesPage : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _rectTransform;
        

        public CanvasGroup CanvasGroup => _canvasGroup;
        public RectTransform RectTransform => _rectTransform;

        private void Awake()
        {
            _canvasGroup.alpha = 0;
        }
    }
}