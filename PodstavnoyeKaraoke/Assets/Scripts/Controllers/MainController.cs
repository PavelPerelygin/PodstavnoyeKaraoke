using System;
using System.Collections;
using System.Globalization;
using Bars;
using Blurs;
using Boards.Base;
using Controllers.GameChanges;
using Controllers.Levels;
using Controllers.License;
using Controllers.Skins;
using Controllers.Update;
using Fades;
using Layers;
using Managers;
using Managers.Audio;
using Managers.Settings;
using Managers.Settings.Local;
using Scenes.Base;
using UnityEngine;
using Utilities;
using RemoteSettings = Managers.Settings.RemoteSettings;

namespace Controllers
{
    public class MainController : MonoBehaviour
    {
        [SerializeField] private MicrophoneController _microphoneController;
        public static MainController Instance { get; private set; }
    
        public Camera MainCamera { get; set; }
        public Scene ActiveScene { get; set; }
        public NavigationBar NavigationBar { get; set; }
        public LicenseController LicenseController { get; private set; }
        public UpdateController UpdateController { get; private set; }
        public GameChangesController GameChangesController { get; private set; }
        public TextManager TextManager { get; private set; }
        public DialogsController DialogsController { get; private set; }
        public UserSettings UserSettings { get; private set; }
        public LocalSettings LocalSettings { get; private set; }
        public GameSettings GameSettings { get; private set; }
        public RemoteSettings RemoteSettings { get; private set; }
        public AudioManager AudioManager { get; private set; }
        public HelperController HelperController { get; private set; }
        public ScreensController ScreensController { get; private set; }
        public MicrophoneController MicrophoneController => _microphoneController;
        public LevelsController LevelsController { get; private set; }
        public SkinsController SkinsController { get; private set; }

        public NumberStyles NumberStyle { get; private set; } = NumberStyles.Number;
        public CultureInfo CultureInfo { get; private set; } = CultureInfo.CreateSpecificCulture("en-GB");

        private bool _hasFocus;

        private void Awake()
        {
            Init();
            DontDestroyOnLoad(gameObject);
        }

        public void Init()
        {
            Instance = this;
            
            MainCamera = Camera.main;
            AudioManager = new AudioManager();
            LicenseController = new LicenseController();
            UpdateController = new UpdateController();
            GameChangesController = new GameChangesController();
            TextManager = new TextManager();
            DialogsController = new DialogsController();
            GameSettings = GameSettings.Load();
            UserSettings = UserSettings.Load();
            LocalSettings = LocalSettings.Load();
            RemoteSettings = new RemoteSettings();
            HelperController = new HelperController();
            ScreensController = new ScreensController();
            MicrophoneController.Init();
            LevelsController = new LevelsController();
            SkinsController = new SkinsController();

            CheckNeedRunInBackground();
        }

        private void CheckNeedRunInBackground()
        {
            Application.runInBackground = GameSettings.RunInBackground;
        }

        public float GetApplicationVersion()
        {
            if (float.TryParse(Application.version, NumberStyle, CultureInfo, out var currentVersion))
                return currentVersion;

            Log.Assert();
            return -1f;
        }

        #region Events

        private void OnApplicationFocus(bool hasFocus)
        {
            _hasFocus = hasFocus;
        }

        private void Update()
        {
            UpdateAlways();

            if (_hasFocus)
                UpdateIsFocus();
        }

        private void OnDestroy()
        {
            UserSettings.Save();
            LocalSettings.Save();
        }

        #endregion

        private void UpdateAlways()
        {
            if(HelperController != null)
                HelperController.Update();
            
            if(AudioManager != null)
                AudioManager.OnUpdate();
            
            if(UserSettings != null)
                UserSettings.OnUpdate();
            
            if(LocalSettings != null)
                LocalSettings.OnUpdate();

            UpdateCheckGlobalKeyDown();
        }

        private void UpdateIsFocus()
        {
        }

        private void UpdateCheckGlobalKeyDown()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }else if (Input.GetKeyDown(KeyCode.F1))
            {
                ScreensController.MinimizeSelectedScreen();
            }
        }

        public void DestroyObj(UnityEngine.Object obj)
        {
            Destroy(obj);
        }

        public T CreateObj <T>(T obj,Transform parent) where T: UnityEngine.Object
        {
            return Instantiate(obj, parent);
        }

        public Coroutine RunCoroutine(IEnumerator iEnumerator)
        {
            return StartCoroutine(iEnumerator);
        }

        public void LoadScene(TypeScene typeScene, bool needFade = true)
        {
            string nameScene = "";
        
            if (typeScene == TypeScene.Loader)
                nameScene = "Loader";
            if (typeScene == TypeScene.Main)
                nameScene = "Main";

            if (needFade)
            {
                Fade fade = GetFadeByType(TypeFade.Dialog);
                fade.EnableFade(1f,1f,0f, () => {UnityEngine.SceneManagement.SceneManager.LoadScene(nameScene); });
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nameScene); 
            }
        }

        public void RememberGameObject(GameObject obj)
        {
            ActiveScene.RememberGameObject(obj);
        }

        public GameObjectInfo GetGameObjectInfo(GameObject obj)
        {
            return ActiveScene.GetGameObjectInfo(obj);
        }

        public Fade GetFadeByType(TypeFade type)
        {
            if (ActiveScene == null)
                return null;

            return ActiveScene.GetFadeByType(type);
        }

        public Layer GetLayerByType(TypeLayer type)
        {
            if (ActiveScene == null)
                return null;

            return ActiveScene.GetLayerByType(type);
        }

        public Blur GetBlurByType(TypeBlur type)
        {
            if (ActiveScene == null)
                return null;

            return ActiveScene.GetBlurByType(type);
        }
    
        public Board GetBoardByType(TypeBoard typeBoard)
        {
            if (ActiveScene == null)
                return null;

            return ActiveScene.GetBoardByType(typeBoard);
        }
    }
}