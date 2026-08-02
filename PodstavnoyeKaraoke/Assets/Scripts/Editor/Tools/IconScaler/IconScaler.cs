using Tools;
using Tools.Editor.OpenAssetsHistory;
using UnityEditor;
using UnityEngine;
using Utilities;
using Utilities.Files;

namespace Editor.Tools.IconScaler
{
    public class IconScaler
    {
        [MenuItem("Tools/Start icon scaling")]
        public static void StartIconScaling()
        {
            var pathToIconsFolder = File.PathCombine(Application.dataPath, "Images/Icons");
            File.CreateDirectoryIfNeed(pathToIconsFolder);
            
            var pathToTexture_1024 = File.PathCombine(pathToIconsFolder, "1024.png");
            var texture_1024 = File.LoadTexture(pathToTexture_1024);
            if (texture_1024 == null)
            {
                Log.Assert("icon 1024.png not found");
                return;
            }

            RescaleIconAndSave(texture_1024, 512,File.PathCombine(pathToIconsFolder, "512.png"));
            RescaleIconAndSave(texture_1024, 256,File.PathCombine(pathToIconsFolder, "256.png"));
            RescaleIconAndSave(texture_1024, 128,File.PathCombine(pathToIconsFolder, "128.png"));
            RescaleIconAndSave(texture_1024, 64,File.PathCombine(pathToIconsFolder, "64.png"));
            RescaleIconAndSave(texture_1024, 48,File.PathCombine(pathToIconsFolder, "48.png"));
            RescaleIconAndSave(texture_1024, 32,File.PathCombine(pathToIconsFolder, "32.png"));
            RescaleIconAndSave(texture_1024, 16,File.PathCombine(pathToIconsFolder, "16.png"));
        }

        private static void RescaleIconAndSave(Texture2D texture,int size, string pathToSave)
        {
            Resize(texture, size, size);
            File.SaveTexture(texture, pathToSave);
        }
        
        private static void Resize(Texture2D texture2D, int targetX, int targetY, bool mipmap =true, FilterMode filter = FilterMode.Bilinear)
        {
            RenderTexture rt = RenderTexture.GetTemporary(targetX, targetY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);
            texture2D.Reinitialize(targetX, targetY, texture2D.format, mipmap);
            texture2D.filterMode = filter;

            try
            {
                texture2D.ReadPixels(new Rect(0.0f, 0.0f, targetX, targetY), 0, 0);
                texture2D.Apply();
            }
            catch
            {
                Debug.LogError("Read/Write is not enabled on texture "+ texture2D.name);
            }


            RenderTexture.ReleaseTemporary(rt);
        }
    }
}