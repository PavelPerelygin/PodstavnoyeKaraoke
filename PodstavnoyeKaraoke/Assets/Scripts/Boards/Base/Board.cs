using System.Collections.Generic;
using Controllers;
using UnityEngine;
using Utilities;

namespace Boards.Base
{
    public abstract class Board : Interactable
    {
        [SerializeField] private TypeBoard _typeBoard;
        
        public TypeBoard TypeBoard => _typeBoard;
        public StateBoard StateBoard { get; protected set; }
        
        protected virtual void Awake()
        {
            StateBoard = StateBoard.Show;
        }
        
        public virtual void Init(){ }

        public void EnableBoard(bool smoothly, float delay = 0)
        {
            OnEnableBoard();
            Show(smoothly,delay);
            
            StateBoard = StateBoard.Show;
        }

        public void DisableBoard(bool smoothly)
        {
            OnDisableBoard();
            Hide(smoothly);
            
            StateBoard = StateBoard.Hide;
        }

        protected virtual void OnEnableBoard (){ }

        protected virtual void OnDisableBoard() { }
        
        protected virtual void Hide(bool smoothly) { }
        
        protected virtual void Show(bool smoothly, float delay = 0f) { }

        public virtual bool OpenMainBoard()
        {
            if(_ignoreTimeLeft > 0)
                return false;

            if(MainController.Instance.DialogsController.GetCountDialogs() > 0)
                return false;

            MainController.Instance.ActiveScene.LoadBoard(TypeBoard.Main,true,1f);

            return true;
        }

        public void SetIgnoreTime(float time)
        {
            _ignoreTimeLeft = time;
        }

        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;
            
            if(StateBoard != StateBoard.Show)
                return false;

            return true;
        }

        protected override bool KeyPressHandler()
        {
            if(StateBoard != StateBoard.Show)
                return false;
        
            if(_ignoreTimeLeft > 0)
                return false;
        
            if(MainController.Instance.DialogsController.GetCountDialogs() > 0)
                return false;

            return true;
        }
    }
}
