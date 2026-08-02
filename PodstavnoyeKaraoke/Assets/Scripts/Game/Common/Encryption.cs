using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Game.Common
{
    public static class Encryption
    {
        public static string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
        
        public static string GetOfflineEncryptionKey(string key)
        {
            return Constants.NAME_GAME + key;
        }
        
        public static string GetOnlineEncryptionKey(string key)
        {
            return Constants.NAME_GAME + key + CustomDeviceId.GetDeviceId();
        }
        
        public static string Decrypter(string value, string encryptionKey)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));

            byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key;
            encryptor.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
        
            string plainText = "";
        
            byte[] cipherBytes = new byte[] { };

            try
            {
                cipherBytes = Convert.FromBase64String(value);
            }
            catch
            {
                return plainText;
            }
        
            if (cipherBytes.Length <= 0)
                return plainText;

            try
            {
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] plainBytes = memoryStream.ToArray();
                plainText = Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);

                memoryStream.Close();
                cryptoStream.Close();
            }
            catch (Exception e)
            {
                // ignored
            }

            return plainText;
        }
        
        public static string Encryptier(string value, string decryptionKey)
        {
            string cipherText = "";

            try
            {
                Aes encryptor = Aes.Create();
                encryptor.Mode = CipherMode.CBC;
        
                byte[] aesKey = new byte[32];
                SHA256 mySHA256 = SHA256Managed.Create();
                byte[] key = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(decryptionKey));
                byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
                Array.Copy(key, 0, aesKey, 0, 32);
                encryptor.Key = aesKey;
                encryptor.IV = iv;
        
                MemoryStream memoryStream = new MemoryStream();
        
                ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
        
                CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
        
                byte[] plainBytes = Encoding.UTF8.GetBytes(value);
        
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
        
                cryptoStream.FlushFinalBlock();
        
                byte[] cipherBytes = memoryStream.ToArray();

                memoryStream.Close();
                cryptoStream.Close();
        
                cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
            }
            catch (Exception e)
            {
                // ignored
            }

            return cipherText;
        }
    }
}