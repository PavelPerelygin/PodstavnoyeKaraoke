using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Game.Common;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    [SerializeField] private Text _headerText;
    [SerializeField] private InputField _requestCodeInputField;
    [SerializeField] private Button _responseCodeButton;
    [SerializeField] private Text _responseCodeText;
    [SerializeField] private Button _generateButton;
    
    private void Start()
    {
        _headerText.text = Constants.NAME_GAME;
        
        _requestCodeInputField.text = "";
        
        _responseCodeButton.onClick.AddListener(CopyRequestKeyInBuffer);
        _responseCodeText.text = "";
        
        _generateButton.onClick.AddListener(OnGenerate);
    }
    
    private void CopyRequestKeyInBuffer()
    {
        GUIUtility.systemCopyBuffer = _responseCodeText.text;
    }

    private void OnGenerate()
    {
        var requestCode = _requestCodeInputField.text;
        
        if(requestCode == "")
            return;

        var response = new LicenseInfo();
        response.RequestCode = requestCode;
        response.NameGame = Constants.NAME_GAME;
        response.Type = "offline";
        var responseString = JsonUtility.ToJson(response);
        
        var encryptionKey = Encryption.GetOfflineEncryptionKey(requestCode);
        var encryptedResponse = Encryption.Encryptier(responseString, encryptionKey);
        var decryptedResponse = Encryption.Decrypter(encryptedResponse, encryptionKey);

        try
        {
            var licenseInfo = JsonUtility.FromJson<LicenseInfo>(decryptedResponse);
            if (licenseInfo != null && licenseInfo.NameGame == Constants.NAME_GAME && licenseInfo.RequestCode == requestCode)
            {
                _responseCodeText.text = encryptedResponse;
            }
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}
