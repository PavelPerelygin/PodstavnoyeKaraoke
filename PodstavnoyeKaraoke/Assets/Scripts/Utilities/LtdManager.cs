using System.Collections.Generic;

namespace Utilities
{
    public class LtdManager
    {
        private class LtdData
        {
            public string nameLtd;
            public LTDescr ltd;
        }
        
        private List<LtdData> _data = new List<LtdData>();

        public bool IsHaveLtd(string nameLtd)
        {
            for (int i = _data.Count - 1; i >= 0; i--)
            {
                var data = _data[i];
                
                if(data.nameLtd == nameLtd && data.ltd != null)
                    return true;
            }
            
            return false;
        }

        public void AddLtd(string nameLtd, LTDescr ltd)
        {
            for (int i = _data.Count - 1; i >= 0; i--)
            {
                var data = _data[i];
                
                if(data.nameLtd != nameLtd)
                    continue;
                
                data.ltd = ltd;
                return;
            }
            
            var newData = new LtdData
            {
                nameLtd = nameLtd,
                ltd = ltd
            };

            _data.Add(newData);
        }

        public void TrySetNullLtd(string nameLtd)
        {
            for (int i = _data.Count - 1; i >= 0; i--)
            {
                var data = _data[i];
                
                if(data.nameLtd != nameLtd)
                    continue;
                
                data.ltd = null;
                
                if(_data.Contains(data))
                    _data.Remove(data);
                
                return;
            }
        }

        public void TryCancelLtd(string nameLtd, bool needComplete = false)
        {
            for (int i = _data.Count - 1; i >= 0; i--)
            {
                var data = _data[i];
                
                if(data.nameLtd != nameLtd)
                    continue;
                
                if(data.ltd == null)
                    continue;
                
                LeanTween.cancel(data.ltd.id,needComplete);
                
                if(_data.Contains(data))
                    _data.Remove(data);
                
                return;
            }
        }
    }
}