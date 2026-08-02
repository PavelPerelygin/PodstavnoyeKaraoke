using System;
using UnityEngine;
using Utilities.Files;

namespace Controllers.Skins
{
    [Serializable]
    public class SpriteResourceData
    {
        public TypeResource typeResource;
        public string nameResource = "";

        [NonSerialized] private SkinData _skinData;
        [NonSerialized] private Texture2D _texture;

        public void Init(SkinData skinData)
        {
            _skinData = skinData;
        }

        #region Type

        public TypeResource GetTypeResource()
        {
            return typeResource;
        }

        #endregion

        #region Name

        public string GetNameResource()
        {
            return nameResource;
        }

        #endregion

        #region Resource
        
        public bool IsExistResource()
        {
            if (_texture == null)
                return false;

            return true;
        }

        public Texture2D GetTexture2D()
        {
            TryLoadTexture();
            
            return _texture;
        }

        private void TryLoadTexture()
        {
            if(_texture != null)
                return;

            var path = $"{_skinData.GetPathToLoadResources()}/{nameResource}.res";
            
            if(!File.FileExists(path))
                return;
            
            _texture = File.LoadTexture(path);
        }
        
        public Sprite GetSprite()
        {
            var texture = GetTexture2D();
            if (texture == null)
                return null;
            
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            
            Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

            return sprite;
        }

        #endregion
        
        #region Get / set

        public string GetPathsToContents(string saveFolderPath)
        {
            var pathToResource = $"{saveFolderPath}/{nameResource}.res";
            return pathToResource;
        }

        #endregion
    }
}