using System;
using System.Collections.Generic;

namespace Controllers.GameChanges
{
    [Serializable]
    public class GameChangesInfo
    {
        public TypePlatform _typePlatform;
        public float _version = 1f;
        public List<string> _pathToPages = new List<string>();
    }
}