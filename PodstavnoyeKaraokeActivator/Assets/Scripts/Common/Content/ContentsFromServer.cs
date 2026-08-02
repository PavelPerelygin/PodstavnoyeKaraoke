using System;
using System.Collections.Generic;

namespace Game.Common.Content
{
    [Serializable]
    public class ContentsFromServer
    {
        public List<string> GUIDS = new List<string>();

        public bool CheckContainsGUID(string guid)
        {
            return GUIDS.Contains(guid);
        }
    }
}