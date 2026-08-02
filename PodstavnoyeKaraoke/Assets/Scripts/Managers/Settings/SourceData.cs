using System;
using System.IO;
using Controllers;
using SFB;
using UnityEngine;
using Utilities.Files;
using File = Utilities.Files.File;

namespace Managers.Settings
{
    [Serializable]
    public class SourceData
    {
        public string sourcePath = "";
        public string extension = "";
        
        private Texture2D _texture;
        
        public event Action OnChangeSource;
        
        public string GetPathToSource()
        {
            return sourcePath;
        }
        
        public Texture2D GetTexture()
        {
            if (_texture == null)
            {
                if (IsExistSource() && GetExtension() != ".gif" && GetExtension() != ".mp4")
                    _texture = File.LoadTextureFromStreamingAssets(GetPathToSource());
            }
            
            return _texture;
        }

        public  void SetPathToSource(string value)
        {
            sourcePath = value;
            
            MainController.Instance.LocalSettings.Save();
            
            OnChangeSource?.Invoke();
        }
        
        public void SetExtension(string value)
        {
            extension = value.ToLower();
            
            MainController.Instance.LocalSettings.Save();
        }
        
        public string GetExtension()
        {
            return extension;
        }

        public bool IsExistSource()
        {
            if(GetPathToSource() == "")
                return false;

            return File.FileExistFromStreamingAssets(GetPathToSource());
        }

        public void OpenSource(Action onComplete = null)
        {
            OpenSourceCustom((string v) =>
            {
                if (v == "")
                {
                    onComplete?.Invoke();
                    return;
                }
                
                SetExtension(Path.GetExtension(v));
                CopyFileToStreamingAssets(v);
                
                MainController.Instance.LocalSettings.Save();
                
                onComplete?.Invoke();
            });
        }

        public void OpenSourceCustom(Action<string> onComplete = null)
        {
            File.OpenFile(new []{new ExtensionFilter("File", "png","jpg","mp4")},MainController.Instance.TextManager.GetText(538), (string v) =>
            {
                onComplete?.Invoke(v);
            });
        }

        public void CopyFileToStreamingAssets(string path)
        {
            SetPathToSource(File.CopyFileToStreamingAssets(path));
        }

        public void RemoveSource()
        {
            _texture = null;
            
            if(!IsExistSource())
                return;

            var path = GetPathToSource();
            SetPathToSource("");
            
            File.DeleteFiletFromStreamingAssets(path);
            
            MainController.Instance.LocalSettings.Save();
        }
    }
}