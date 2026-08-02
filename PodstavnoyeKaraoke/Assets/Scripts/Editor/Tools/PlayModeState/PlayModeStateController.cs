using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools.Editor.PlayModeState
{
    [InitializeOnLoad]
    public static class PlayModeStateController
    {
        static PlayModeStateController ()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        private static void PlayModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.ExitingEditMode)
                ExitingEditMode();
            else if (obj == PlayModeStateChange.EnteredEditMode)
                EnteredEditMode();
        }
        
        private static void ExitingEditMode()
        {
            var currentScene = EditorSceneManager.GetActiveScene();

            var sceneNotFromTheProject = true;

            for (int i = 0; i <  EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].path == currentScene.path)
                {
                    sceneNotFromTheProject = false;
                    break;
                }
            }

            if(sceneNotFromTheProject)
                return;

            var loadingScene = EditorBuildSettings.scenes[0];
            if (currentScene.path == loadingScene.path)
                return;

            var info = new PlayModeStateInfo();
            
            info._openScenePath = currentScene.path;

            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                info._openPrefabPath = PrefabStageUtility.GetCurrentPrefabStage().prefabAssetPath;

            if (Selection.activeGameObject != null)
                info._activeGameObjectInfo = GetObjectInfoNesting(Selection.activeGameObject);

            var jsonStr = JsonUtility.ToJson(info);
            PlayerPrefs.SetString("PlayModeStateInfo",jsonStr);
            PlayerPrefs.Save();

            EditorApplication.ExitPlaymode();
            EditorSceneManager.SaveOpenScenes();
            OpenAssetsHistory.OpenAssetsHistory.Enable = false;
            EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
            OpenAssetsHistory.OpenAssetsHistory.Enable = true;
            EditorApplication.EnterPlaymode();
        }

        private static void EnteredEditMode()
        {
            var jsonStr = PlayerPrefs.GetString("PlayModeStateInfo");
            var info = JsonUtility.FromJson<PlayModeStateInfo>(jsonStr);
            
            if(info == null)
                return;
            
            if (info._openScenePath != "")
            {
                OpenAssetsHistory.OpenAssetsHistory.Enable = false;
                EditorSceneManager.OpenScene(info._openScenePath);
                OpenAssetsHistory.OpenAssetsHistory.Enable = true;
            }
            
            if (info._openPrefabPath != "")
            {
                var prefab = AssetDatabase.LoadAssetAtPath(info._openPrefabPath, typeof(GameObject));
                AssetDatabase.OpenAsset(prefab);
                
                if (info._activeGameObjectInfo != "")
                {
                    var openPrefab = PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;
                    var allGameObjectInPrefab = GetAllGameObjectsInPrefab(openPrefab);

                    SetActivePreviouslyGameObject(allGameObjectInPrefab, info._activeGameObjectInfo);
                    info._activeGameObjectInfo = "";
                }
            }
            
            if (info._activeGameObjectInfo != "")
            {
                var objects = GameObject.FindObjectsOfType<GameObject>().ToList();
                SetActivePreviouslyGameObject(objects,info._activeGameObjectInfo);
            }
            
            PlayerPrefs.DeleteKey("PlayModeStateInfo");
            PlayerPrefs.Save();
        }

        private static string GetObjectInfoNesting(GameObject obj)
        {
            string info = $"{obj.name}_{obj.transform.GetSiblingIndex()}|";
            
            Transform parent = obj.transform.parent;

            while (true)
            {
                if (parent != null)
                    info += $"{parent.name}_{parent.GetSiblingIndex()}|";
                else
                    break;

                parent = parent.transform.parent;
            }

            if(info.Length > 0)
                info = info.Substring(0, info.Length - 1);

            return info;
        }
        private static List<GameObject> GetAllGameObjectsInPrefab(GameObject prefab)
        {
            var result = new List<GameObject>(){prefab.gameObject};
            for (int i = 0; i < prefab.transform.childCount; i++)
            {
                result.AddRange(GetAllGameObjectsInPrefab(prefab.transform.GetChild(i).gameObject));
            }

            return result;
        }

        private static void SetActivePreviouslyGameObject(List<GameObject> allObjects, string infoObj)
        {
            for (int i = 0; i < allObjects.Count; i++)
            {
                var obj = allObjects[i];
                    
                if(GetObjectInfoNesting(obj) != infoObj)
                    continue;

                Selection.activeGameObject = obj;
                return;
            }
        }
    }
}