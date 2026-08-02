using UnityEngine;

namespace Layers
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Layer : MonoBehaviour
    {
        [SerializeField] private TypeLayer _type;

        private CanvasGroup _canvasGroup;
        public TypeLayer Type => _type;

        public bool IsLock => !_canvasGroup.blocksRaycasts;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
        public void Lock()
        {
            _canvasGroup.blocksRaycasts = false;
        }
        
        public void Unlock()
        {
            _canvasGroup.blocksRaycasts = true;
        }
    }
}