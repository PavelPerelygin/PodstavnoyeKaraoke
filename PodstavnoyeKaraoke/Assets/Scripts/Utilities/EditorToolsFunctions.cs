using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Extensions;
using UnityEngine;
using Application = UnityEngine.Application;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Utilities
{
    public static class EditorToolsFunctions
    {
        public static NumberStyles NumberStyle { get; private set; } = NumberStyles.Number;
        public static CultureInfo CultureInfo { get; private set; } = CultureInfo.CreateSpecificCulture("en-GB");
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

        public static string GetAssetsPath(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(obj);
            return path;
#endif
            return "";
        }

        public static string GetPathToParentFolder(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            var pathToObject = GetAssetsPath(obj);
            var pathToDirectory = Path.GetDirectoryName(pathToObject);

            return pathToDirectory;
#endif
            return "";
        }
        
        public static string GetPathToParentFolderToPrefab(GameObject gameObject)
        {
#if UNITY_EDITOR
            var prefabStage = (PrefabStage)StageUtility.GetStage(gameObject);
            var pathToDirectory = Path.GetDirectoryName(prefabStage.assetPath);
            return pathToDirectory;
#endif
            return "";
        }
        
        public static string GetPathToFile(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(obj);
            path = Path.GetDirectoryName(path);
            return path;
#endif
            return "";
        }

        public static string GetFileName(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(obj);
            var fileName = Path.GetFileNameWithoutExtension(path);
            return fileName;
#endif
            return "";
        }
        
        public static string GetFileNamePrefab(GameObject gameObject)
        {
#if UNITY_EDITOR
            var prefabStage = (PrefabStage)StageUtility.GetStage(gameObject);
            var fileName = Path.GetFileNameWithoutExtension(prefabStage.assetPath);
            return fileName;
#endif
            return "";
        }
        
        public static string GetParentFolderName(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(obj);
            var parentFolderName = Directory.GetParent(path).Name;
            return parentFolderName;
#endif
            return "";
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

        public static void RemoveScriptableObject(UnityEngine.ScriptableObject obj, bool removeFolder = false)
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(obj);
            AssetDatabase.DeleteAsset(path);

            if (removeFolder)
            {
                var pathToDirectory = Path.GetDirectoryName(path);
                AssetDatabase.DeleteAsset(pathToDirectory);
            }
#endif
        }
        
        public static Texture2D GetTextureGreen(bool selected)
        {
            var color = "";
            if (selected) color = "#409A26";
            else color = "#63D942";
            
            return GetTextureByColor(ColorExtensions.ConvertHexToColor(color));
        }

        public static Texture2D GetTextureRed(bool selected)
        {
            var color = "";
            if (selected) color = "#C45149";
            else color = "#D9665E";
            
            return GetTextureByColor(ColorExtensions.ConvertHexToColor(color));
        }

        public static Texture2D GetTextureGray(bool selected)
        {
            var color = "";
            if (selected) color = "#979797";
            else color = "#C8C8C8";
            
            return GetTextureByColor(ColorExtensions.ConvertHexToColor(color));
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
            texture2D.Apply();

            return texture2D;
        }

        public static string NormalizePath(string value)
        {
            return value.Replace("\\", "/");
        }

        public static void CreatePathIfNeeded(string path)
        {
            path = NormalizePath(path);
            
            string fullPath = $"{Application.dataPath}/{path.Replace("Assets/","")}";

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }

        public static List<string> GetFolderNames(string path)
        {
            var result = new List<string>();
            
            string fullPath = $"{Application.dataPath}/{path.Replace("Assets/","")}";
            
            if (!Directory.Exists(fullPath))
            {
                return result;
            }
            else
            {
                var directories = Directory.GetDirectories(fullPath);
                for (int i = 0; i < directories.Length; i++)
                {
                    string folder = new DirectoryInfo(directories[i]).Name;
                    result.Add(folder);
                }
            }

            return result;
        }

        public static List<string> GetFileNames(string path)
        {
            var result = new List<string>();
            
            string fullPath = $"{Application.dataPath}/{path.Replace("Assets/","")}";
            
            if (!Directory.Exists(fullPath))
            {
                return result;
            }
            else
            {
                var files= Directory.GetFiles(fullPath);
                for (int i = 0; i < files.Length; i++)
                {
                    string file = Path.GetFileName(files[i]);
                    result.Add(file);
                }
            }

            return result;
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
        

        #region Define
        
#if UNITY_EDITOR
        [MenuItem("Tools/Clear all defines")]
        public static void DevToDevEnable()
        {
            ClearAllDefine();
        }
#endif

        public static bool CheckDefineEnable(string define)
        {
#if UNITY_EDITOR
            var defines = new List<string>();

            var needGroups = new List<BuildTargetGroup>()
                {BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA , BuildTargetGroup.WebGL};

            for (int i = 0; i < needGroups.Count; i++)
            {
                var definesStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(needGroups[i]);

                var definesParts = definesStr.Split(';');

                for (int j = 0; j < definesParts.Length; j++)
                {
                    var part = definesParts[j];
                    if(!defines.Contains(part))
                        defines.Add(part); 
                }
            }

            if (defines.Contains(define))
                return true;
#endif
            
            return false;
        }

        public static bool SwitchEnableDefine(string define)
        {
#if UNITY_EDITOR
            var defines = new List<string>();

            var needGroups = new List<BuildTargetGroup>()
                { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA, BuildTargetGroup.WebGL };

            for (int i = 0; i < needGroups.Count; i++)
            {
                var definesStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(needGroups[i]);

                var definesParts = definesStr.Split(';');

                for (int j = 0; j < definesParts.Length; j++)
                {
                    var part = definesParts[j];
                    if(!defines.Contains(part))
                        defines.Add(part); 
                }
            }

            var result = false;
            
            if (defines.Contains(define))
            {
                defines.Remove(define);
                result = false;
            }
            else
            {
                defines.Add(define);
                result = true;
            }

            for (int i = 0; i < needGroups.Count; i++)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(needGroups[i], defines.ToArray());
            }

            return result; 
#endif

            return false;
        }

        public static void SetEnableDefine(string define, bool enable)
        {
#if UNITY_EDITOR
            var defines = new List<string>();

            var needGroups = new List<BuildTargetGroup>()
                { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA, BuildTargetGroup.WebGL };

            for (int i = 0; i < needGroups.Count; i++)
            {
                var definesStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(needGroups[i]);

                var definesParts = definesStr.Split(';');

                for (int j = 0; j < definesParts.Length; j++)
                {
                    var part = definesParts[j];
                    if(!defines.Contains(part))
                        defines.Add(part); 
                }
            }

            if (enable && !defines.Contains(define))
                defines.Add(define);
            else if (!enable && defines.Contains(define))
                defines.Remove(define);

            for (int i = 0; i < needGroups.Count; i++)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(needGroups[i], defines.ToArray());
            }
#endif
        }

        public static void ClearAllDefine()
        {
#if UNITY_EDITOR
            var needGroups = new List<BuildTargetGroup>()
                { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA, BuildTargetGroup.WebGL };
            

            for (int i = 0; i < needGroups.Count; i++)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(needGroups[i],new []{""} );
            }
#endif
        }

        #endregion
    }
}