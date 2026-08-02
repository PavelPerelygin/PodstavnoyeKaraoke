using System;
using UnityEngine;

namespace Game.Common
{
    [Serializable]
    public class VersionGame
    {
        public string nameGame = Constants.NAME_GAME;
        public int version = Constants.VERSION;

        public static string ToJson(string guid)
        {
            var versionGame = new VersionGame();
            
            var decryptionKey = $"{guid}_{Constants.NAME_GAME}";
            var serilizeble = JsonUtility.ToJson(versionGame,false);
            
            return Encryption.Encryptier(serilizeble, decryptionKey);;
        }

        public static VersionGame FromJson(string json, string guid)
        {
            VersionGame versionGame = null;
            var decryptionKey = $"{guid}_{Constants.NAME_GAME}";
            var decrypterJson = Encryption.Decrypter(json, decryptionKey);
            
            try
            {
                versionGame = JsonUtility.FromJson<VersionGame>(decrypterJson);
            }
            catch (Exception e)
            {
                // ignored
            }

            return versionGame;
        }
    }
}