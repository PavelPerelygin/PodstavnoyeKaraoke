using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using Utilities;
using Utilities.Files;

namespace Controllers.Skins
{
    [Serializable]
    public class SkinData
    {
        public string guid = "";
        public string version = "";
        public string checksum = "";
        public string nameSkin = "";
        public List<SpriteResourceData> spriteResources = new List<SpriteResourceData>();
        public List<ColorResourceData> colorResources = new List<ColorResourceData>();
        
        private const string _nameJson = "skin_data.json";
        public static readonly string DefaultSkinName = "По умолчанию";

        public void Init()
        {
            for (int i = 0; i < spriteResources.Count; i++)
                spriteResources[i].Init(this);
        }

        #region Version

        public string GetVersion()
        {
            return version;
        }

        #endregion
        
        #region Check sum

        public string GetChecksum()
        {
            return checksum;
        }

        #endregion
        
        #region Name

        public void SetSkinName(string skinName)
        {
            nameSkin = skinName;
        }

        public string GetNameSkin()
        {
            return nameSkin;
        }

        #endregion
        
        #region Guid

        public string GetGuid()
        {
            return guid;
        }

        #endregion

        #region Resources

        public List<SpriteResourceData> GetSpriteResources()
        {
            return spriteResources;
        }
        
        public List<ColorResourceData> GetColorResources()
        {
            return colorResources;
        }

        public Sprite GetSpriteByName(string spriteName)
        {
            for (int i = 0; i < spriteResources.Count; i++)
            {
                var spriteResource = spriteResources[i];
                if (spriteResource.GetNameResource() == spriteName)
                    return spriteResource.GetSprite();
            }
            
            Log.Assert($"Can't find sprite with name: {spriteName}");
            return null;
        }
        
        public Texture2D GetTexture2DByName(string nameTexture)
        {
            for (int i = 0; i < spriteResources.Count; i++)
            {
                var spriteResource = spriteResources[i];
                if (spriteResource.GetNameResource() == nameTexture)
                    return spriteResource.GetTexture2D();
            }
            
            Log.Assert($"Can't find texture 2d with name: {nameTexture}");
            return null;
        }
        
        public Color GetColorByName(string colorName)
        {
            for (int i = 0; i < colorResources.Count; i++)
            {
                var colorResource = colorResources[i];
                if (colorResource.GetNameResource() == colorName)
                    return ColorExtensions.ConvertHexToColor(colorResource.GetResource());
            }
            
            Log.Assert($"Can't find color with name: {colorName}");
            return Color.white;
        }

        #endregion
        
        #region Get / set
        
        public string GetPathToSave()
        {
            return File.PathCombine(File.GetPathToStreamingAssets(), $"Content/Skins/{GetGuid()}");
        }

        public List<string> GetPathsToContents()
        {
            var result = new List<string>();

            var pathToFolder = GetPathToSave();

            for (int i = 0; i < spriteResources.Count; i++)
            {
                var spriteResource = spriteResources[i];
                var pathsToContent = spriteResource.GetPathsToContents(pathToFolder);
                if(pathsToContent != "")
                    result.Add(pathsToContent);
            }
            
            return result;
        }

        #endregion
        
        #region Load / remove

        public static SkinData Load(string path)
        {
            var pathToJson = $"{path}/{_nameJson}";

            SkinData skinData = null;
            
            if (File.FileExists(pathToJson))
            {
                try
                {
                    skinData = JsonUtility.FromJson<SkinData>(File.ReadAllText(pathToJson));
                }
                catch (Exception e)
                {
                    skinData = new SkinData();
                }
            }
            else
            {
                skinData = new SkinData();
            }
            
            return skinData;
        }
        
        public string GetPathToLoadResources()
        {
            return File.PathCombine(File.GetPathToStreamingAssets(), $"Content/Skins/{GetGuid()}");
        }

        #endregion
    }
}