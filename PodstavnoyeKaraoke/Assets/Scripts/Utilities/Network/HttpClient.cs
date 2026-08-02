using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace Utilities.Network
{
    public static class HttpClient
    {
        private static System.Net.Http.HttpClient _httpClient;

        private const bool _enableLog = false;
        
        public static async Task GetRequest(string url,
            Dictionary<string, string> parameters = null,
            Action<string> onSuccessfully = null,
            Action<string> onError = null)
        {
            if (_httpClient == null)
                _httpClient = new System.Net.Http.HttpClient();

            string concatenationUrl = url + "?";

            if (parameters != null)
            {
                foreach (var parametr in parameters)
                    concatenationUrl += $"{parametr.Key}={parametr.Value}&";
            }

            concatenationUrl = concatenationUrl.Substring(0, concatenationUrl.Length - 1);
            
            if(_enableLog)
                Log.Message($"[Request] {concatenationUrl}");
            
            try	
            {
                HttpResponseMessage response = await _httpClient.GetAsync(concatenationUrl);
                response.EnsureSuccessStatusCode();
                var responseStr = await response.Content.ReadAsStringAsync();
                
                if(_enableLog)
                    Log.Message($"[Response] {responseStr}");
                
                onSuccessfully?.Invoke(responseStr);
            }
            catch(HttpRequestException e)
            {
                if(_enableLog)
                    Log.Message($"[Response] {e.Message}");
                
                onError?.Invoke(e.Message);
            }
        }
        
        public static async void PostRequest(string url, Dictionary<string, string> parameters = null,Action<string> onCompleted = null)
        {
            if (_httpClient == null)
                _httpClient = new System.Net.Http.HttpClient();

            var stringContent = new FormUrlEncodedContent(Array.Empty<KeyValuePair<string, string>>());
            if (parameters != null)
                stringContent = new FormUrlEncodedContent(parameters);

            try	
            {
                HttpResponseMessage response = await _httpClient.PostAsync(url,stringContent);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                
                if(_enableLog)
                    Log.Message($"[Response] {result}");
                
                onCompleted?.Invoke(result);
            }
            catch(HttpRequestException e)
            {
                if(_enableLog)
                    Log.Message($"[Response] {e.Message}");
                
                onCompleted?.Invoke(e.Message);
            }
        }
    }
}