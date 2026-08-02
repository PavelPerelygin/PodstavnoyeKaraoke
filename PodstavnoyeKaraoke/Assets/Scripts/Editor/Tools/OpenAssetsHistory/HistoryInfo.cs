using System;
using System.Collections.Generic;

namespace Tools.Editor.OpenAssetsHistory
{
    [Serializable]
    public class HistoryInfo
    {
        public List<AssetInfo> _assets = new List<AssetInfo>();
    }
}