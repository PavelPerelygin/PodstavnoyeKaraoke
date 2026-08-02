using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Controllers.Levels;
using Controllers.Skins.Default;
using Dialogs;
using Game.Common;
using Game.Common.Content;
using SFB;
using UnityEngine;
using Utilities.Network;
using File = Utilities.Files.File;

namespace Controllers.Skins
{
    public class SkinsController
    {
        private ContentsFromServer _contentsFromServer;
        private List<ContentsWrapping<SkinData>> _contents = new List<ContentsWrapping<SkinData>>();

        public event Action OnSortingContent;

        public SkinsController()
        {
            LoadContents();
            LoadContentsFromServer();
        }

        #region Main

        private void LoadContents()
        {
            var path = $"{File.GetPathToStreamingAssets()}/Content/Skins";

            if (!File.FolderExists(path))
                return;
            
            var jsonFiles = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);

            foreach (var file in jsonFiles)
            {
                var jsonContent = File.ReadAllText(file);

                try
                {
                    var contentData = JsonUtility.FromJson<SkinData>(jsonContent);
                    
                    contentData.Init();
                    
                    var contentWrapper = new ContentsWrapping<SkinData>(contentData,true);
                    
                    //----decryptier version
                    if (!CheckVersion(contentWrapper))
                        contentWrapper.SetValid(false);
                    //----decryptier sum
                    if (!CheckSum(contentWrapper))
                        contentWrapper.SetValid(false);
                    //----validate
                    if (!CheckValid(contentWrapper))
                        contentWrapper.SetValid(false);
                    
                    _contents.Add(contentWrapper);
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
            
            SortingContents();
        }
        
        private async void LoadContentsFromServer()
        {
            if (MainController.Instance.LicenseController.GetTypeLicense() == "offline")
                return;
            
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                {"NameGame", Constants.NAME_GAME},
                {"ActivationKey", MainController.Instance.LicenseController.GetActivationKey()},
                {"UniqueIdentifier", CustomDeviceId.GetDeviceId()}
            };
        
            await HttpClient.GetRequest(Constants.GET_CONTENT_URL, parameters, (string responseStr) =>
            {
                try
                {
                    string encryptionKey = Encryption.GetOnlineEncryptionKey(MainController.Instance.LicenseController.GetActivationKey());
                    string decryptedResponse = Encryption.Decrypter(responseStr, encryptionKey);

                    _contentsFromServer = JsonUtility.FromJson<ContentsFromServer>(decryptedResponse);
                    
                    ValidateAllContent();
                }
                catch (Exception e)
                {
                    // ignored
                }
            }, (string responseStr) =>
            {
            });
        }
        
        private void SortingContents()
        {
            _contents = _contents.OrderBy(content => content.GetContent().GetNameSkin()).ToList();
            
            OnSortingContent?.Invoke();
        }
        
        public List<ContentsWrapping<SkinData>> GetContents(bool skipNotValid = false)
        {
            var result = new List<ContentsWrapping<SkinData>>();

            for (int i = 0; i < _contents.Count; i++)
            {
                var content = _contents[i];
                
                if(skipNotValid && !content.IsValid())
                    continue;
                
                result.Add(content);
            }
            
            return result;
        }
        
        private ContentsWrapping<SkinData> GetContentByGUID(string guid,bool skipNotValid = false)
        {
            var contents = GetContents(skipNotValid);
            
            for (int i = 0; i < contents.Count; i++)
            {
                var package = contents[i];
                if(package.GetContent().GetGuid() == guid)
                    return package;
            }
            
            return null;
        }
        
        private bool CheckContainsContentByGUID(string guid)
        {
            var content = GetContentByGUID(guid);
            if (content != null)
                return true;

            return false;
        }
        
        private void ValidateAllContent()
        {
            var contents = GetContents();

            for (int i = 0; i < contents.Count; i++)
            {
                var content = contents[i];
                if(!CheckValid(content))
                    content.SetValid(false);
            }
        }
        
        public void OpenContent(Action<string> onComplete)
        {
            var extensions = new[] {
                new ExtensionFilter("Vov skin fille", "vovs"),
            };
            
            string[] pathFile = StandaloneFileBrowser.OpenFilePanel(MainController.Instance.TextManager.GetText(538), "", extensions, false);
            if (pathFile.Length <= 0)
                return;

            MainController.Instance.DialogsController.OpenAwaitProgressDialog(
                MainController.Instance.TextManager.GetText(540),
                async (AwaitDialog awaitDialog) =>
                {
                    var sourceDir = $"{File.GetPathToStreamingAssets()}/Content/Tmp";
                    if(File.FolderExists(sourceDir))
                        File.DeleteFolder(sourceDir);
            
                    File.CreateDirectoryInStreamingAssetsIfNeed("Content/Tmp");
                    
                    await PackerUtility.UnpackSimpleAsync(
                        pathFile[0],
                        sourceDir,
                        new Progress<float>(awaitDialog.SetProgress)
                    );
                    

                    var contentData = SkinData.Load(sourceDir);
                    if (contentData == null)
                        return;
                    
                    contentData.Init();

                    if (CheckContainsContentByGUID(contentData.GetGuid()))
                    {
                        File.DeleteFolder(sourceDir);
                    
                        MainController.Instance.DialogsController.CloseAwaitDialog();
                        onComplete?.Invoke("package_has_already_been_added");
                        return;
                    }
            
                    var destinationDir = $"{File.GetPathToStreamingAssets()}/Content/Skins/{contentData.GetGuid()}";
            
                    File.Copy(sourceDir,destinationDir);
                    
                    File.DeleteFolder(sourceDir);
                    
                    var contentWrapper = new ContentsWrapping<SkinData>(contentData,true);
                                
                    //----decryptier version
                    if (!CheckVersion(contentWrapper))
                    {
                        File.DeleteFolder(destinationDir);
                    
                        MainController.Instance.DialogsController.CloseAwaitDialog();
                        onComplete?.Invoke("need_update");
                        return;
                    }
                    //----decryptier sum
                    if (!CheckSum(contentWrapper))
                    {
                        File.DeleteFolder(destinationDir);
                    
                        MainController.Instance.DialogsController.CloseAwaitDialog();
                        onComplete?.Invoke("not_valid_sum");
                        return;
                    }
                    //----validate
                    if (!CheckValid(contentWrapper))
                    {
                        File.DeleteFolder(destinationDir);
                    
                        MainController.Instance.DialogsController.CloseAwaitDialog();
                        onComplete?.Invoke("not_valid_from_server");
                        return;   
                    }
            
                    _contents.Add(contentWrapper);

                    SortingContents(); 
                    
                    MainController.Instance.DialogsController.CloseAwaitDialog();
                    onComplete?.Invoke("complete");
                });
        }
        
        public void RemoveContent(ContentsWrapping<SkinData> content)
        {
            if(!_contents.Contains(content))
                return;
            
            var sourceDir = $"{File.GetPathToStreamingAssets()}/Content/Skins/{content.GetContent().GetGuid()}";
            if(File.FolderExists(sourceDir))
                File.DeleteFolder(sourceDir);
            
            _contents.Remove(content);
            
            MainController.Instance.LocalSettings.SetSkinName(SkinData.DefaultSkinName);
            
            SortingContents();
        }

        #endregion

        #region Special

        private ContentsWrapping<SkinData> GetSkinByName(string skinName)
        {
            for (int i = 0; i < _contents.Count; i++)
            {
                var content = _contents[i];
                if(content.GetContent().GetNameSkin() == skinName)
                    return content;
            }

            return null;
        }

        #endregion
        
        #region Check

        private bool CheckVersion(ContentsWrapping<SkinData> content)
        {
            var guid = content.GetContent().GetGuid();
            var packageVersion = VersionGame.FromJson(content.GetContent().GetVersion(), guid);

            if (packageVersion == null ||
                packageVersion.nameGame != Constants.NAME_GAME ||
                packageVersion.version > Constants.VERSION)
            {
                return false;
            }

            return true;
        }

        private bool CheckSum(ContentsWrapping<SkinData> content)
        {
            var checkSumA = Game.Common.CheckSum.FromJson(content.GetContent().GetChecksum(), content.GetContent().GetGuid());
            
            var checkSumB = new CheckSum();
            var pathsToContents = content.GetContent().GetPathsToContents();
            checkSumB.sum = PackerUtility.GetChecksum(pathsToContents);

            if (checkSumA == null ||
                checkSumA.nameGame != checkSumB.nameGame ||
                checkSumA.sum != checkSumB.sum)
            {
                return false;
            }

            return true;
        }
        
        private bool CheckValid(ContentsWrapping<SkinData> content)
        {
            if (_contentsFromServer == null)
                return true;
            
            return _contentsFromServer.CheckContainsGUID(content.GetContent().GetGuid());
        }

        #endregion

        #region Resources

        public Sprite GetSpriteByName(string nameSprite)
        {
            Sprite result = null;
            
            var currentSkinName = MainController.Instance.LocalSettings.GetSkinName();
            var skin = GetSkinByName(currentSkinName);
            if (skin != null)
            {
                result = skin.GetContent().GetSpriteByName(nameSprite);
            }

            if (skin == null || result == null)
            {
                result = DefaultSkin.Instance.GetSpriteByName(nameSprite);
            }
            
            return result;
        }
        
        public Texture2D GetTexture2DByName(string nameTexture)
        {
            Texture2D result = null;
            
            var currentSkinName = MainController.Instance.LocalSettings.GetSkinName();
            var skin = GetSkinByName(currentSkinName);
            if (skin != null)
            {
                result = skin.GetContent().GetTexture2DByName(nameTexture);
            }

            if (skin == null || result == null)
            {
                result = DefaultSkin.Instance.GetTexture2DByName(nameTexture);
            }
            
            return result;
        }
        
        public Color GetColorByName(string nameColor)
        {
            Color result = Color.white;
            
            var currentSkinName = MainController.Instance.LocalSettings.GetSkinName();
            var skin = GetSkinByName(currentSkinName);
            if (skin != null)
            {
                result = skin.GetContent().GetColorByName(nameColor);
            }
            
            return result;
        }

        #endregion
    }
}