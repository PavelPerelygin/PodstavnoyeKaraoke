using UnityEngine;

namespace Utilities.Network
{
    public static class Browser
    {
        public static void OpenWebsite(string url)
        {
            Application.OpenURL(url);
        }
    }
}