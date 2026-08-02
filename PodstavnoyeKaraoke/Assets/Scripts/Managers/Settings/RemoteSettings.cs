using Unity.RemoteConfig;

namespace Managers.Settings
{
    public struct UserAttributes { }
    public struct AppAttributes { }
    
    public class RemoteSettings
    {
        public bool EnableActivationGame { get; private set; } = true;
        public string WebsiteUrl { get; private set; } = "http://holidaygames.ru";
        public RemoteSettings()
        {
            ConfigManager.FetchCompleted += OnFetchCompleted;
            ConfigManager.FetchConfigs(new UserAttributes(), new AppAttributes());
        }

        private void OnFetchCompleted(ConfigResponse configResponse)
        {
            LoadEnableActivation();
        }
        
        private void LoadEnableActivation()
        {
            if(ConfigManager.appConfig.HasKey("EnableActivation"))
                EnableActivationGame = ConfigManager.appConfig.GetBool("EnableActivation");
            
            if(ConfigManager.appConfig.HasKey("WebsiteUrl"))
                WebsiteUrl = ConfigManager.appConfig.GetString("WebsiteUrl");
        }
    }
}