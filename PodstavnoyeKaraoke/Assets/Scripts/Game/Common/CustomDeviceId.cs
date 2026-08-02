using System;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Game.Common
{
    public static class CustomDeviceId
    {
        private const string DeviceIdPrefsKey = "CustomDeviceId";

        /// <summary>
        /// Получаем уникальный идентификатор устройства.
        /// Этот идентификатор будет одинаковым для всех сборок, как в Editor, так и в Runtime.
        /// </summary>
        public static string GetDeviceId()
        {
            string savedDeviceId = PlayerPrefs.GetString(DeviceIdPrefsKey, string.Empty);
            if (!string.IsNullOrEmpty(savedDeviceId))
            {
                return savedDeviceId;
            }

            string deviceId = GenerateDeviceId();
            PlayerPrefs.SetString(DeviceIdPrefsKey, deviceId);
            PlayerPrefs.Save();

            return deviceId;
        }

        private static string GenerateDeviceId()
        {
            string macAddress = GetMacAddress();
        
            if (macAddress != "Unknown")
            {
                return HashSystemInfo($"MAC Address: {macAddress}");
            }
            else
            {
                Debug.LogWarning("MAC-адрес не доступен, используем SystemInfo.deviceUniqueIdentifier.");
                return SystemInfo.deviceUniqueIdentifier; // Fallback к SystemInfo.deviceUniqueIdentifier
            }
        }

        /// <summary>
        /// Получаем MAC-адрес устройства.
        /// Если MAC-адрес не доступен, возвращаем "Unknown".
        /// </summary>
        private static string GetMacAddress()
        {
            string macAddress = string.Empty;

            try
            {
                // Получаем все сетевые интерфейсы
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                // Сначала ищем Ethernet (проводное соединение)
                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        macAddress = networkInterface.GetPhysicalAddress().ToString();
                        if (!string.IsNullOrEmpty(macAddress))
                        {
                            break; // Если нашли, сразу выходим
                        }
                    }
                }

                // Если не нашли Ethernet, ищем Wi-Fi (беспроводное соединение)
                if (string.IsNullOrEmpty(macAddress))
                {
                    foreach (NetworkInterface networkInterface in networkInterfaces)
                    {
                        if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                        {
                            macAddress = networkInterface.GetPhysicalAddress().ToString();
                            if (!string.IsNullOrEmpty(macAddress))
                            {
                                break; // Если нашли, сразу выходим
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка при получении MAC-адреса: " + ex.Message);
            }

            return string.IsNullOrEmpty(macAddress) ? "Unknown" : macAddress;
        }

        /// <summary>
        /// Хэшируем системную информацию для получения уникального идентификатора.
        /// </summary>
        private static string HashSystemInfo(string systemInfo)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(systemInfo));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
