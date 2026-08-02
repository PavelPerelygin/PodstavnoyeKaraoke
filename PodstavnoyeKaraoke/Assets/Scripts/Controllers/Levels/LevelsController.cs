using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Dialogs;
using Game.Common;
using Game.Common.Content;
using SFB;
using UnityEngine;
using Utilities.Network;
using File = Utilities.Files.File;

namespace Controllers.Levels
{
    public class LevelsController
    {
        private ContentsFromServer _contentsFromServer;
        private List<ContentsWrapping<LevelData>> _contents = new List<ContentsWrapping<LevelData>>();
        
        public event Action OnSortingContent;

        public LevelsController()
        {
            LoadContents();
            LoadContentsFromServer();
        }

        #region Main

        private void LoadContents()
        {
            var path = $"{File.GetPathToStreamingAssets()}/Content/Levels";

            if (!File.FolderExists(path))
                return;
            
            var jsonFiles = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);

            foreach (var file in jsonFiles)
            {
                var jsonContent = File.ReadAllText(file);

                try
                {
                    var contentData = JsonUtility.FromJson<LevelData>(jsonContent);
                    
                    var contentWrapper = new ContentsWrapping<LevelData>(contentData,true);
                    
                    //----decryptier version
                    if (!CheckVersion(contentWrapper))
                        contentWrapper.SetValid(false);
                    //----decryptier sum
                    if (!CheckSum(contentWrapper))
                        contentWrapper.SetValid(false);
                    //----validate
                    if (MainController.Instance.LicenseController.GetTypeLicense() == "online")
                    {
                        if (!CheckValid(contentWrapper))
                            contentWrapper.SetValid(false);   
                    }
                    
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
            _contents = _contents.OrderBy(content => content.GetContent().GetNameLevel()).ToList();
            
            OnSortingContent?.Invoke();
        }
        
        public List<ContentsWrapping<LevelData>> GetContents(bool skipNotValid = false)
        {
            var result = new List<ContentsWrapping<LevelData>>();

            for (int i = 0; i < _contents.Count; i++)
            {
                var content = _contents[i];
                
                if(skipNotValid && !content.IsValid())
                    continue;
                
                result.Add(content);
            }
            
            return result;
        }
        
        private ContentsWrapping<LevelData> GetContentByGUID(string guid,bool skipNotValid = false)
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
                new ExtensionFilter("Vov level fille", "vovl"),
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

                    var contentData = LevelData.Load(sourceDir);
                    if (contentData == null)
                        return;

                    if (CheckContainsContentByGUID(contentData.GetGuid()))
                    {
                        File.DeleteFolder(sourceDir);
                    
                        MainController.Instance.DialogsController.CloseAwaitDialog();
                        onComplete?.Invoke("package_has_already_been_added");
                        return;
                    }
            
                    var destinationDir = $"{File.GetPathToStreamingAssets()}/Content/Levels/{contentData.GetGuid()}";
            
                    File.Copy(sourceDir,destinationDir);
            
                    File.DeleteFolder(sourceDir);
                    
                    var contentWrapper = new ContentsWrapping<LevelData>(contentData,true);
                    
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
                    if (MainController.Instance.LicenseController.GetTypeLicense() == "online")
                    {
                        if (!CheckValid(contentWrapper))
                        {
                            File.DeleteFolder(destinationDir);
                    
                            MainController.Instance.DialogsController.CloseAwaitDialog();
                            onComplete?.Invoke("not_valid_from_server");
                            return;   
                        }
                    }
            
                    _contents.Add(contentWrapper);

                    SortingContents(); 
                    
                    MainController.Instance.DialogsController.CloseAwaitDialog();
                    onComplete?.Invoke("complete");
                });
        }
        
        public void RemoveContent(ContentsWrapping<LevelData> content)
        {
            if(!_contents.Contains(content))
                return;
            
            var sourceDir = $"{File.GetPathToStreamingAssets()}/Content/Levels/{content.GetContent().GetGuid()}";
            if(File.FolderExists(sourceDir))
                File.DeleteFolder(sourceDir);
            
            _contents.Remove(content);
            
            SortingContents();
        }

        #endregion

        #region Special

        public int GetLevelNumber(ContentsWrapping<LevelData> contentsWrapping)
        {
            return _contents.IndexOf(contentsWrapping) + 1;
        }

        #endregion
        
        #region Check

        private bool CheckVersion(ContentsWrapping<LevelData> content)
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

        private bool CheckSum(ContentsWrapping<LevelData> content)
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
        
        private bool CheckValid(ContentsWrapping<LevelData> content)
        {
            if (_contentsFromServer == null)
                return true;
            
            return _contentsFromServer.CheckContainsGUID(content.GetContent().GetGuid());
        }

        #endregion
    }
}