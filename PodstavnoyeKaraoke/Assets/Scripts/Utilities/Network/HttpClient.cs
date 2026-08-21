using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utilities.Network
{
    public static class HttpClient
    {
        private const bool _enableLog = false;
        private const string DisabledMessage = "HTTP client is disabled.";
        
        public static Task GetRequest(string url,
            Dictionary<string, string> parameters = null,
            Action<string> onSuccessfully = null,
            Action<string> onError = null)
        {
            if(_enableLog)
                Log.Message($"[Request disabled] {url}");

            onError?.Invoke(DisabledMessage);
            return Task.CompletedTask;
        }
        
        public static void PostRequest(string url, Dictionary<string, string> parameters = null,Action<string> onCompleted = null)
        {
            if(_enableLog)
                Log.Message($"[Request disabled] {url}");

            onCompleted?.Invoke(DisabledMessage);
        }
    }
}
