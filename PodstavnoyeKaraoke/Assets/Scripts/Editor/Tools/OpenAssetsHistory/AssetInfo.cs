using System;
using System.IO;
using UnityEngine;

namespace Tools.Editor.OpenAssetsHistory
{
    public enum AssetType
    {
        UNDERFINED,
        PREFAB,
        SCENE
    }
    
    [Serializable]
    public class AssetInfo
    {
        public AssetType _type;
        public string _name;
        public string _iconName;
        public string _path;
        
        public AssetInfo(AssetType type, string path,  string iconName)
        {
            _type = type;
            _path = path;
            _iconName = iconName;

            _name = Path.GetFileNameWithoutExtension(path);
        }
    }
}