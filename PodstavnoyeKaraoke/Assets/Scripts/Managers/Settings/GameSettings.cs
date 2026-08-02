using UnityEngine;

namespace Managers.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        private static GameSettings _instance;
        public static GameSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Load();

                return _instance;
            }
        }
        
        public static GameSettings Load()
        {
            return Resources.Load<GameSettings>("Data/GameSettings");
        }
        
        [SerializeField] private string _gameKeyCode;
        public string GameKeyCode => _gameKeyCode;

        [Header("Default resolution")]
        [SerializeField] private int _defaultResolutionWidth;
        public int DefaultResolutionWidth => _defaultResolutionWidth;
        [SerializeField] private int _defaultResolutionHeight;
        public int DefaultResolutionHeight => _defaultResolutionHeight;

        [Header("Social network links")]
        [SerializeField] private string _vkUrl;
        public string VkUrl => _vkUrl; 
        [SerializeField] private string _instagramUrl;
        public string InstagramUrl => _instagramUrl; 
        [SerializeField] private string _youtubeUrl;
        public string YoutubeUrl => _youtubeUrl;
        [SerializeField] private string _telegramUrl;
        public string TelegramUrl => _telegramUrl; 
        
        [SerializeField] private string _gameDownloadUrl;
        public string GameDownloadUrl => _gameDownloadUrl; 
        
        [SerializeField] private string _packagesDownloadUrl;
        public string PackagesDownloadUrl => _packagesDownloadUrl; 
        
        [Header("Enable Disable")]
        [SerializeField] private bool _cleanStreamingAssetsFolderInBuild;
        public bool СleanStreamingAssetsFolderInBuild => _cleanStreamingAssetsFolderInBuild;
        [SerializeField] private bool _createSetuper;
        public bool CreateSetuper => _createSetuper;
        [SerializeField] private bool _runInBackground = true;
        public bool RunInBackground => _runInBackground;
        [SerializeField] private bool _enableFeedbackButtons = true;
        public bool EnableFeedbackButtons => _enableFeedbackButtons;

    }
}