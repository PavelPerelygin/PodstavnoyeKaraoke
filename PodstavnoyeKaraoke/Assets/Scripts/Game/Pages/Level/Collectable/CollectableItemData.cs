namespace Game.Pages.Level.Collectable
{
    public class CollectableItemData
    {
        private TypeCollectableItem _typeCollectableItem;
        private int _count;
        private int _scores;

        public CollectableItemData(TypeCollectableItem typeCollectableItem)
        {
            _typeCollectableItem = typeCollectableItem;
        }
        
        public TypeCollectableItem GetTypeCollectableItem()
        {
            return _typeCollectableItem;
        }

        public void SetCount(int value)
        {
            _count = value;
        }

        public void IncrementCount()
        {
            SetCount(GetCount() + 1);
            IncrementScores();
        }

        private void IncrementScores()
        {
            if (_typeCollectableItem == TypeCollectableItem.Bomb)
                _scores += -30;
            else if (_typeCollectableItem == TypeCollectableItem.Coin)
                _scores += 10;
            else if (_typeCollectableItem == TypeCollectableItem.Star)
                _scores += 30;
            else if (_typeCollectableItem == TypeCollectableItem.Ruby)
                _scores += 50;
            else if (_typeCollectableItem == TypeCollectableItem.Gift)
                _scores += UnityEngine.Random.Range(-30, 61);
        }

        public int GetCount()
        {
            return _count;
        }

        public int GetScores()
        {
            return _scores;
        }
    }
}