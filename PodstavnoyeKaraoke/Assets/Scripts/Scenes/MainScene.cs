using Bars;
using Boards.Base;
using Controllers;
using Fades;
using Scenes.Base;
using UnityEngine;

namespace Scenes
{
    public class MainScene : Scene
    {
        protected override void Awake()
        {
            TypeScene = TypeScene.Main;

            base.Awake();
        }
    
        protected override void Start()
        {
            Fade fade = MainController.Instance.GetFadeByType(TypeFade.Global);
            fade.EnableFade(1f);

            base.Start();
        }
    
        protected override void SceneLoadingComplete()
        {
            base.SceneLoadingComplete();

            LoadBoard(TypeBoard.Main,false);

            if (MainController.Instance.RemoteSettings.EnableActivationGame)
            {
                MainController.Instance.LicenseController.CheckGameActivation((bool state) =>
                {
                    if(state && MainController.Instance.UserSettings.GetNeedUpdateGame())
                        MainController.Instance.UpdateController.CheckUpdateGame();
                    
                    MainController.Instance.GameChangesController.CheckNeedShowChangeLogDialog();
                });
            }

            Fade globalFade = MainController.Instance.GetFadeByType(TypeFade.Global);
            globalFade.DisableFade();
        }
    }
}
