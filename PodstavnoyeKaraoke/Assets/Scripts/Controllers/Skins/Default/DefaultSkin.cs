using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Controllers.Skins.Default
{
    public class DefaultSkin : ScriptableObject
    {
        private static DefaultSkin _instance;

        [SerializeField] private List<SpriteResource> _sprites = new List<SpriteResource>();
        [SerializeField] private List<ColorResource> _colors = new List<ColorResource>();
        
        public static DefaultSkin Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Load();

                return _instance;
            }
        }
        
        private static DefaultSkin Load()
        {
            return Resources.Load<DefaultSkin>("Data/DefaultSkin/DefaultSkin");
        }
        
        private void OnValidate()
        {
            for (int i = 0; i < _sprites.Count; i++)
            {
                _sprites[i].UpdateName();
            }
        }

        public Sprite GetSpriteByName(string spriteName)
        {
            for (int i = 0; i < _sprites.Count; i++)
            {
                var spriteResource = _sprites[i];
                if (spriteResource.GetResourceName() == spriteName)
                    return spriteResource.GetResource();
            }
            
            Log.Assert($"not found sprite by name {spriteName}");

            return null;
        }

        public Texture2D GetTexture2DByName(string textureName)
        {
            var sprite = GetSpriteByName(textureName);
            if(sprite == null)
                return null;
            
            Texture2D texture = sprite.texture;
            
            Texture2D texture2D = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            
            Color[] pixels = texture.GetPixels(
                (int)sprite.textureRect.x,
                (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height
            );
            
            texture2D.SetPixels(pixels);
            texture2D.Apply();

            return texture2D;
        }
        
        public Sprite GetColorByName(string colorName)
        {
            for (int i = 0; i < _sprites.Count; i++)
            {
                var spriteResource = _sprites[i];
                if (spriteResource.GetResourceName() == colorName)
                    return spriteResource.GetResource();
            }
            
            Log.Assert($"not found sprite by name {colorName}");

            return null;
        }
        
        public void Save()
        {
#if UNITY_EDITOR
            EditorToolsFunctions.Save(this);
#endif
        }
    }
}