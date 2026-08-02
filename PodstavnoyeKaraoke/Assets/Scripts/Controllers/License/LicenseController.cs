using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dialogs.Base;
using Game.Common;
using UnityEngine;
using Utilities;
using Utilities.Network;

namespace Controllers.License
{
    public class LicenseController
    {
        private LicenseInfo _licenseInfo = new LicenseInfo();
        
        public LicenseController()
        {
            TryLoadLicense();
        }
        
        private void TryLoadLicense()
        {
            var encryptedLicense = PlayerPrefs.GetString("license");
            if (encryptedLicense == "")
                return;
        
            try
            {
                var requestCode = GetRequestCode();
                var offlineEncryptionKey = Encryption.GetOfflineEncryptionKey(requestCode);
                var decryptedLicense = Encryption.Decrypter(encryptedLicense, offlineEncryptionKey);
                
                _licenseInfo = JsonUtility.FromJson<LicenseInfo>(decryptedLicense);
            }
            catch
            {
                // ignored
            }
        }
        
        public void CheckGameActivation(Action<bool> onCompleted = null)
        {
            bool needCreateActivaterDialog = false;
        
            if (IsGameActivated())
            {
                onCompleted?.Invoke(true);
            }
            else
            {
                ShowActivationDialog();
                onCompleted?.Invoke(false);
            }
        }

        private void ShowActivationDialog()
        {
            var dialog = MainController.Instance.DialogsController.CreateDialog(TypeDialog.Activation);
            dialog.Init();
            dialog.Show();
        }
        
        public bool IsGameActivated()
        {
            if (_licenseInfo == null)
                return false;
            
            if (_licenseInfo.NameGame != Application.productName )
                return false;

            if (_licenseInfo.Type == "offline" && _licenseInfo.RequestCode != GetRequestCode())
                return false;
            if (_licenseInfo.Type == "online" && _licenseInfo.DeviceId != CustomDeviceId.GetDeviceId())
                return false;

            return true;
        }

        public string GetTypeLicense()
        {
            if (_licenseInfo == null)
            {
                Log.Assert();
                return "offline";
            }
            
            return _licenseInfo.Type;
        }

        public string GetActivationKey()
        {
            if (_licenseInfo == null)
            {
                Log.Assert();
                return "";   
            }

            if (_licenseInfo.Type == "offline")
            {
                Log.Assert();
                return "";
            }

            if (_licenseInfo.ActivationKey == "")
            {
                Log.Assert();
                return "";
            }
            
            return _licenseInfo.ActivationKey;
        }

        public async void OfflineActivateGame(string responseCode, Action<bool> onCompleted)
        {
            var requestCode = GetRequestCode();
            var encryptionKey = Encryption.GetOfflineEncryptionKey(requestCode);
            var decryptedResponse = Encryption.Decrypter(responseCode, encryptionKey);

            try
            {
                var licenseInfo = JsonUtility.FromJson<LicenseInfo>(decryptedResponse);
                if (licenseInfo != null && licenseInfo.NameGame == Application.productName && licenseInfo.RequestCode == requestCode)
                {
                    _licenseInfo = licenseInfo;
                    Save();
                    
                    onCompleted?.Invoke(true);
                }
                else
                {
                    onCompleted?.Invoke(false);
                }
            }
            catch (Exception e)
            {
                onCompleted?.Invoke(false);
            }
        }
        
        public async void OnlineActivateGame(string activationKey, Action<string> onCompleted)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                {"NameGame", Constants.NAME_GAME},
                {"ActivationKey", activationKey},
                {"UniqueIdentifier", CustomDeviceId.GetDeviceId()}
            };
        
            await HttpClient.GetRequest(Constants.ACTIVATION_URL, parameters, (string responseStr) =>
            {
                string encryptionKey = Encryption.GetOnlineEncryptionKey(activationKey);
                string decryptedResponse = Encryption.Decrypter(responseStr, encryptionKey);

                try
                {
                    _licenseInfo = JsonUtility.FromJson<LicenseInfo>(decryptedResponse);

                    if (_licenseInfo != null)
                    {
                        Save();
                        onCompleted.Invoke("complete");
                    }
                    else
                    {
                        onCompleted.Invoke(responseStr);
                    }
                }
                catch (Exception e)
                {
                    onCompleted.Invoke(responseStr);
                }
            }, (string responseStr) =>
            {
                onCompleted.Invoke("");
            });
        }
        
        public string GetRequestCode()
        {
            return SystemInfo.unsupportedIdentifier != CustomDeviceId.GetDeviceId() ? CustomDeviceId.GetDeviceId() : "K1TO9GzXmlUghQZthl37";
        }
        
        private void Save()
        {
            string serilizebleLicense = JsonUtility.ToJson(_licenseInfo);
            
            var requestCode = GetRequestCode();
            var encryptionKey = Encryption.GetOfflineEncryptionKey(requestCode);
            var encryptedLicense = Encryption.Encryptier(serilizebleLicense, encryptionKey);
            
            PlayerPrefs.SetString("license", encryptedLicense);
        }
    }
}