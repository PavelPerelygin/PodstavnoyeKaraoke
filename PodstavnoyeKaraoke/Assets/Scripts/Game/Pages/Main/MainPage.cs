using System;
using Boards;
using UnityEngine;
using Utilities;

namespace Game.Pages.Main
{
    public class MainPage : Page
    {
        private LtdManager _ltdManager = new LtdManager();
        
        public override void Init(MainBoard mainBoard)
        {
            base.Init(mainBoard);
        }

        public override TypePage GetTypePage()
        {
            return TypePage.Main;
        }

        public override float Open(bool smoothly, float delay, Action onComplete = null)
        {
            base.Open(smoothly, delay, onComplete); 
            
            _root.gameObject.SetActive(true);
            
            var fadeTime = ShowFade(smoothly, delay);
            
            return fadeTime;
        }

        public override float Close(bool smoothly, Action onComplete = null)
        {
            base.Close(smoothly, onComplete);

            var fadeTime = HideFade(smoothly, 0, () =>
            {
                _root.gameObject.SetActive(false);
            });
            
            return fadeTime;
        }
    }
}