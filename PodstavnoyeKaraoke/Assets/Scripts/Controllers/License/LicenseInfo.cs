using System;

namespace Controllers.License
{
    [Serializable]
    public class LicenseInfo
    {
        public string NameGame = "";
        public string Type = "";
        public string ActivationKey = "";
        public string RequestCode = "";
        public string DeviceId = "";
    }
}