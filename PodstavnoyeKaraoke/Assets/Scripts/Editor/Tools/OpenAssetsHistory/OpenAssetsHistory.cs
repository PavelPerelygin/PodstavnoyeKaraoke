using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools.Editor.OpenAssetsHistory
{
    [InitializeOnLoad]
    public static class OpenAssetsHistory
    {
        public static HistoryInfo _history;
        public static bool Enable = true;

        private static bool _skipKeyDown = false;
        private static bool _skipSaveHistory = false;
        private static int _maxLenghtHistory = 20;

        static OpenAssetsHistory()
        {
            Load();

            PrefabStage.prefabStageOpened += OnOpenPrefab;
            EditorSceneManager.sceneOpened += OnOpenScene;
        }
        
        private static void OnOpenPrefab(PrefabStage prefabStage)
        {
            if(!Enable)
                return;
            
            AddHistory(prefabStage.prefabAssetPath);
        }
        
        private static void OnOpenScene(Scene scene,OpenSceneMode mode)
        {
            if(!Enable)
                return;
            
            AddHistory(scene.path);
        }
        
        private static void AddHistory(string pathToAsset)
        {
            if (_skipSaveHistory)
            {
                _skipSaveHistory = false;
                return;
            }

            for (int i = _history._assets.Count - 1; i >= 0 ; i--)
            {
                string historyPath = _history._assets[i]._path;
                if (historyPath == pathToAsset)
                    _history._assets.RemoveAt(i);
            }

            AssetType type = GetAssetType(pathToAsset);
            string iconName = GetAssetIconName(type);
            AssetInfo info = new AssetInfo(type, pathToAsset,iconName);

            _history._assets.Insert(0,info);

            if(_history._assets.Count > _maxLenghtHistory)
                _history._assets.RemoveAt(_history._assets.Count - 1);

            Save();
        }

        private static AssetType GetAssetType(string path)
        {
            AssetType type = AssetType.UNDERFINED;

            string extension = Path.GetExtension(path);
            
            if (extension == ".prefab")
                type = AssetType.PREFAB;
            else if (extension == ".unity")
                type = AssetType.SCENE;

            return type;
        }
        
        private static string GetAssetIconName(AssetType type)
        {
            string name = "";

            if (type == AssetType.PREFAB)
                name = "Prefab Icon";
            else if (type == AssetType.SCENE)
                name = "BuildSettings.Editor.Small";

            return name;
        }

        public static void OpenAssetInHistory(AssetInfo info)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(info._path);
            if (obj != null)
            {
                EditorUtility.FocusProjectWindow();
                ProjectWindowUtil.ShowCreatedAsset(obj);
            }
        }

        public static void ClearHistory()
        {
            _history._assets.Clear();
            Save();
        }

        private static void Load()
        {
            string path = $"{Application.persistentDataPath}/History.txt";
            if (File.Exists(path))
            {
                var jsonStr =File.ReadAllText(path);
                _history = JsonUtility.FromJson<HistoryInfo>(jsonStr);
                if (_history == null)
                    _history = new HistoryInfo();
            }
            else
            {
                _history = new HistoryInfo();
            }
        }

        private static void Save()
        {
            string path = $"{Application.persistentDataPath}/History.txt";
            var jsonStr = JsonUtility.ToJson(_history, true);
            File.WriteAllText(path,jsonStr);
        }

    }
}
