namespace Game.Common.Content
{
    public class ContentsWrapping<T>
    {
        private T _packageData;
        private bool _isValid;

        public ContentsWrapping(T packageData, bool isValid)
        {
            SetContent(packageData);
            SetValid(isValid);
        }

        public void SetContent(T value)
        {
            _packageData = value;
        }

        public T GetContent()
        {
            return _packageData;
        }

        public void SetValid(bool value)
        {
            _isValid = value;
        }

        public bool IsValid()
        {
            return _isValid;
        }
    }
}