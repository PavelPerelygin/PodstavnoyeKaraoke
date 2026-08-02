using System;
using UnityEngine;

namespace Game.Common
{
    [Serializable]
    public class CheckSum
    {
        public string nameGame = Constants.NAME_GAME;
        public string sum = "";
        
        public static string ToJson(string sum,string guid)
        {
            var checkSum = new CheckSum();
            checkSum.sum = sum;
            
            var decryptionKey = $"{guid}_{Constants.NAME_GAME}";
            var serilizeble = JsonUtility.ToJson(checkSum,false);
            
            return Encryption.Encryptier(serilizeble, decryptionKey);;
        }

        public static CheckSum FromJson(string json, string guid)
        {
            CheckSum checkSum = null;
            var decryptionKey = $"{guid}_{Constants.NAME_GAME}";
            var decrypterJson = Encryption.Decrypter(json, decryptionKey);
            
            try
            {
                checkSum = JsonUtility.FromJson<CheckSum>(decrypterJson);
            }
            catch (Exception e)
            {
                // ignored
            }

            return checkSum;
        }
    }
}