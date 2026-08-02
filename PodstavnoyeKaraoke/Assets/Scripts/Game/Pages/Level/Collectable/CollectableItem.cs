using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Level.Collectable
{
    public class CollectableItem : MonoBehaviour
    {
        [SerializeField] private TypeCollectableItem _typeCollectableItem;
        [SerializeField] private Text _countText;

        public TypeCollectableItem GetTypeCollectableItem()
        {
            return _typeCollectableItem;
        }

        public void SetCount(int count)
        {
            _countText.text = count.ToString();
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
        
    }
}