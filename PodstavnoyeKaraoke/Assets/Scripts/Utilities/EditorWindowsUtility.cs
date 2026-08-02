using System.IO;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public static class EditorWindowsUtility
    {
        public static void Save(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            SetDirty(obj);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        public static void SetDirty(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(obj);
#endif
        }

        public static T LoadScriptableObject<T>(string path) where T:UnityEngine.ScriptableObject
        {
#if UNITY_EDITOR
            string pathToFile = GetDirectoryName(path);

            CreatePathIfNeeded(pathToFile);

            T data = AssetDatabase.LoadAssetAtPath<T>(path);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(data, path);
            }

            return data;
#else
            return null;
#endif
        }

        public static T CreateScriptableObject<T>(string path, string name) where T : UnityEngine.ScriptableObject
        {
#if UNITY_EDITOR
            CreatePathIfNeeded(path);
            
            T obj = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(obj, $"{path}/{name}.asset");

            return obj;
#else
            return null;
#endif
        }

        public static void RemoveScriptableObject(UnityEngine.ScriptableObject obj)
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(obj);
            AssetDatabase.DeleteAsset(path);
#endif
        }
        
        public static Texture2D GetTextureByColor(Color color)
        {
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            Color[] colors = new Color[1];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = color;
            }
            
            texture2D.SetPixels(colors);

            return texture2D;
        }

        private static void CreatePathIfNeeded(string path)
        {
            string fullPath = $"{Application.dataPath}/{path.Replace("Assets/","")}";

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }
        
        public static void DeleteFileFolder(string pathToFile)
        {
        #if UNITY_EDITOR
            string pathToFolder = GetDirectoryName(pathToFile);
            string fullPath = $"{Application.dataPath}/{pathToFolder.Replace("Assets/","")}";

            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath,true);
                AssetDatabase.Refresh();
            }
        #endif
        }

        private static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path).Replace('\\', '/');
        }
        
        public static void DrawLine()
        {
#if UNITY_EDITOR
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
#endif
        }
    }
}