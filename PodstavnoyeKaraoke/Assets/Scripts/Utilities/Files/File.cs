using System;
using System.Collections.Generic;
using System.IO;
using Extensions;
using NAudio.Wave;
using SFB;
using UnityEngine;
using UnityEngine.Networking;

namespace Utilities.Files
{
    public static class File
    {
        public static string GetPathToStreamingAssets()
        {
            var path = GetCorrectPath(Application.streamingAssetsPath);

            CreateDirectoryIfNeed(path);

            return path;
        }

        public static string GetGlobalPath(string localPath)
        {
            return PathCombine(Application.streamingAssetsPath, localPath);
        }

        public static string GetNameFile(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }
        
        public static string GetCorrectPath(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        
        public static bool FileExists(string path)
        {
            if (System.IO.File.Exists(path))
                return true;
            else
                return false;
        }

        public static bool FolderExists(string path)
        {
            return Directory.Exists(path);
        }

        public static bool FileExistFromStreamingAssets(string path)
        {
            return FileExists(PathCombine(GetPathToStreamingAssets(), path));
        }

        public static void DeleteFolder(string path)
        {
            System.IO.Directory.Delete(path,true);
        }

        public static void CreateDirectoryIfNeed(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        
        public static void DeleteFiletFromStreamingAssets(string localPath)
        {
            string path = PathCombine(GetPathToStreamingAssets(), localPath);
            
            if(System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        public static string PathCombine(params string[] paths)
        {
            string result = "";

            for (int i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
                
                if(path.Length <= 0)
                    continue;
                
                char lastChar = path[path.Length - 1];
                if (lastChar == Path.DirectorySeparatorChar || lastChar == Path.AltDirectorySeparatorChar || lastChar == Path.VolumeSeparatorChar)
                    path = path.DeleteLastCharacter();

                path = GetCorrectPath(path);
                
                result += $"{path}/";
            }

            return result.DeleteLastCharacter();
        }
        
        public static string GetDirectoryFile(string path)
        {
            return GetCorrectPath(Path.GetDirectoryName(path));
        }

        public static string ReadAllText(string path)
        {
            return System.IO.File.ReadAllText(path);
        }
        
        public static void Copy(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(sourceDir))
            {
                Log.Assert();
                return;
            }
            
            Directory.CreateDirectory(destinationDir);
            
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                System.IO.File.Copy(file, destFile, true);
            }
            
            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                Copy(subDir, destSubDir);
            }
        }

        public static void WriteAllText(string path, string contents)
        {
            System.IO.File.WriteAllText(path, contents);
        }
        
        public static void WriteAllLines(string path, IEnumerable<string> contents)
        {
            System.IO.File.WriteAllLines(path, contents);
        }
        
        public static TypeContent GetTypeFile(string path)
        {
            TypeContent typeContent = TypeContent.None;
            string typeStr = Path.GetExtension(path);
        
            if (typeStr == ".wav" || typeStr == ".mp3")
                typeContent = TypeContent.Sound;
            else if (typeStr == ".mp4")
                typeContent = TypeContent.Video;
            else if (typeStr == ".png" || typeStr == ".jpg"||typeStr == ".jpeg")
                typeContent = TypeContent.Image;

            return typeContent;
        }

        public static string GetRandomFileName(string nameFile, bool needExtension = true)
        {
            string extension = "";
            if(needExtension)
                extension = Path.GetExtension(nameFile);
        
            string fileName = System.Guid.NewGuid() + extension;

            return fileName;
        }

        public static void OpenFile(TypeContent typeContent, string header,Action<string> onOpen)
        {
            var extensions = new[] {
                GetExtensionFilterByTypeContent(typeContent),
            };
            string[] path = StandaloneFileBrowser.OpenFilePanel(header, "", extensions, false);
            if (path.Length > 0)
            {
                if(path[0] == "")
                    return;
            
                onOpen(path[0]);
            }
        }
        
        public static void OpenFile( ExtensionFilter [] extensionFilter, string header,Action<string> onOpen)
        {
            string[] path = StandaloneFileBrowser.OpenFilePanel(header, "", extensionFilter, false);
            if (path.Length > 0)
            {
                if(path[0] == "")
                    return;
            
                onOpen(path[0]);
            }
        }

        private static ExtensionFilter GetExtensionFilterByTypeContent(TypeContent type)
        {
            if (type == TypeContent.Sound)
            {
#if UNITY_STANDALONE_WIN
                return new ExtensionFilter("Sound Files", "mp3");
#elif UNITY_STANDALONE_OSX
                return new ExtensionFilter("Sound Files", "wav");
#endif
            }
            if (type == TypeContent.Image)
                return new ExtensionFilter("Image Files", "png","jpg");
            if (type == TypeContent.Video)
                return new ExtensionFilter("Vide Files", "mp4");
            else
                return new ExtensionFilter();
        }

        public static void SaveFile(TypeContent typeContent, string header,string localPath, string defaultFileNameWithoutExtension = "", Action<string> onSave = null, Action onNotSave = null)
        {
            var extensions = new[] {
                GetExtensionFilterByTypeContent(typeContent),
            };

            var name = GetDefaultSaveFileName(localPath, defaultFileNameWithoutExtension);
            
            var path = StandaloneFileBrowser.SaveFilePanel(header, "", name, extensions);
            if (!string.IsNullOrEmpty(path))
            {
                var globalPath = GetGlobalPath(localPath);
                if (!System.IO.File.Exists(globalPath))
                {
                    Debug.LogError($"[FileUtility] SaveFile failed because source file does not exist. Local path: '{localPath}'. Full path: '{globalPath}'.");
                    onNotSave?.Invoke();
                    return;
                }

                System.IO.File.Copy(globalPath, path, true);
                onSave?.Invoke(path);
            }
            else
            {
                onNotSave?.Invoke();
            }
        }

        private static string GetDefaultSaveFileName(string localPath, string defaultFileNameWithoutExtension)
        {
            if (string.IsNullOrEmpty(defaultFileNameWithoutExtension))
                return Path.GetFileName(localPath);

            var fileName = SanitizeFileName(defaultFileNameWithoutExtension);
            if (string.IsNullOrEmpty(fileName))
                return Path.GetFileName(localPath);

            return fileName + Path.GetExtension(localPath);
        }

        public static string SaveBytesToStreamingAssets(byte[] bytes,string extension, string where = "Content")
        {
            CreateDirectoryIfNeed(PathCombine(GetPathToStreamingAssets(),where));
            
            string fileName = System.Guid.NewGuid() + extension;
            string path = PathCombine(GetPathToStreamingAssets(),where,fileName);
            
            Debug.Log($"[FileUtility] SaveBytesToStreamingAssets requested. Extension: '{extension}'. Folder: '{where}'. Bytes: {(bytes == null ? -1 : bytes.Length)}. Full path: '{path}'.");
            System.IO.File.WriteAllBytes(path,bytes);
            
            string localPath = PathCombine(where, fileName);
            Debug.Log($"[FileUtility] SaveBytesToStreamingAssets completed. Local path: '{localPath}'. Full path: '{path}'.");
            
            return localPath;
        }

        private static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            for (int i = 0; i < invalidChars.Length; i++)
            {
                fileName = fileName.Replace(invalidChars[i], '_');
            }

            return fileName.Trim();
        }
        
        public static string CopyFileToStreamingAssets(string from, string where = "Content")
        {
            CreateDirectoryIfNeed(PathCombine(GetPathToStreamingAssets(),where));
            
            string randomName = GetRandomFileName(from);
            string path = PathCombine(GetPathToStreamingAssets(),where,randomName);

            System.IO.File.Copy(from, path, true);

            string localPath = PathCombine(where, randomName);
            
            return localPath;
        }
        
        public static string CopyMusicFileToStreamingAssets(string from, string where = "Content")
        {
#if UNITY_STANDALONE_WIN
            return CopyAndConvectMp3FileToContentFromStreamingAssets(from,where);
#elif UNITY_STANDALONE_OSX
            return CopyFileToStreamingAssets(from,where);
#endif
        }
        
        private static string CopyAndConvectMp3FileToContentFromStreamingAssets(string from, string where)
        {
            CreateDirectoryIfNeed(PathCombine(GetPathToStreamingAssets(),where));
            
            string randomName = GetRandomFileName(from, false) + ".wav";
            string path = PathCombine(GetPathToStreamingAssets(),where,randomName);

            using (Mp3FileReader mp3 = new Mp3FileReader(from))
            {
                using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                {
                    WaveFileWriter.CreateWaveFile(path, pcm);
                }
            }
            
            string localPath = PathCombine(where, randomName);
            
            return localPath;
        }

        public static Texture2D LoadTextureFromStreamingAssets(string path)
        {
            var fullPath =PathCombine(GetPathToStreamingAssets(), path);

            return LoadTexture(fullPath);
        }

        public static Texture2D LoadTexture(string path)
        {
            var bytes = System.IO.File.ReadAllBytes(path);

            Texture2D texture =  new Texture2D(2, 2, TextureFormat.ARGB32, false);
            texture.LoadImage(bytes);
            texture.wrapMode = TextureWrapMode.Clamp;

            return texture;
        }

        public static string SaveTexture(Texture2D texture, string path)
        {
            byte[] bytes = texture.EncodeToPNG();
            
            System.IO.File.WriteAllBytes(path, bytes);

            return path;
        }
        
        public static AudioClip LoadAudioClipFromStreamingAssets(string path)
        {
            string fullPath = PathCombine(GetPathToStreamingAssets(), path);
            
            Debug.Log($"[FileUtility] LoadAudioClipFromStreamingAssets requested. Local path: '{path}'. Full path: '{fullPath}'. Exists: {System.IO.File.Exists(fullPath)}.");
            WWW www = new WWW("file://" + fullPath);
        
            AudioClip clip = www.GetAudioClip(false, true, AudioType.WAV);

            if (clip == null)
            {
                Debug.Log($"[FileUtility] LoadAudioClipFromStreamingAssets completed with null clip. Local path: '{path}'. WWW error: '{www.error}'.");
                return null;
            }

            clip.name = "Track";
            Debug.Log($"[FileUtility] LoadAudioClipFromStreamingAssets completed. Local path: '{path}'. Clip length: {clip.length:0.000}. Samples: {clip.samples}. Channels: {clip.channels}. Frequency: {clip.frequency}. WWW error: '{www.error}'.");
            return clip;
        }
        
        public static void CreateDirectoryInStreamingAssetsIfNeed(string path)
        {
            CreateDirectoryIfNeed(PathCombine(GetPathToStreamingAssets(),path));
        }
        
        public static AudioClip LoadAudioClip(string path)
        {
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.WAV);
            
            www.SendWebRequest();
            
            while (!www.isDone)
            {
                // Это может вызвать зависание, если загрузка займет много времени.
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error loading audio clip: {www.error}");
                return null;
            }
            
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            clip.name = "Track";
            return clip;
        }

    }
}
