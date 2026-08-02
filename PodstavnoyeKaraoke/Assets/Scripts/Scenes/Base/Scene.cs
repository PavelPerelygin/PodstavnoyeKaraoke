using System.Collections;
using System.Collections.Generic;
using Bars;
using Blurs;
using Boards.Base;
using Controllers;
using Fades;
using Layers;
using UnityEngine;
using Utilities;

namespace Scenes.Base
{
    public abstract class Scene : MonoBehaviour
    {
        [SerializeField] Camera _mainCamera;
        [SerializeField] protected List<Board> _boards = new List<Board>();
        [SerializeField] protected List<Layer> _layers = new List<Layer>();
        [SerializeField] protected List<Fade> _fades = new List<Fade>();
        [SerializeField] protected List<Blur> _blurs = new List<Blur>();
        [SerializeField] private NavigationBar _navigationBar;

        private Dictionary<GameObject, GameObjectInfo> _gameObjectInfos = new Dictionary<GameObject, GameObjectInfo>();

        public TypeScene TypeScene { get; protected set; }
        public StateScene StateScene { get; protected set; }
        
        public Board ActiveBoard { get; protected set; }
        public NavigationBar NavigationBar => _navigationBar;

        protected virtual void Awake()
        {
            StateScene = StateScene.Create;

            MainController.Instance.MainCamera = _mainCamera;
            MainController.Instance.ActiveScene = this;
            MainController.Instance.NavigationBar = _navigationBar;
            
            InitFades();
            InitBlurs();
            
            if(_navigationBar != null) 
                _navigationBar.Init();
        }
        
        protected virtual void Start()
        {
            StateScene = StateScene.Loading;
            StartCoroutine(WaitForEndOfFrame());;
        }
        
        private IEnumerator WaitForEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            SceneLoadingComplete();
        }
        
        protected virtual void SceneLoadingComplete()
        {
            StateScene = StateScene.Complited;

            InitBoards();
        }

        private void InitBoards()
        {
            for (int i = 0; i < _boards.Count; i++)
            {
                _boards[i].Init();
                _boards[i].DisableBoard(false);
            }
        }

        private void InitFades()
        {
            for (int i = 0; i < _fades.Count; i++)
                _fades[i].Init();
        }
        
        private void InitBlurs()
        {
            for (int i = 0; i < _blurs.Count; i++)
                _blurs[i].Init();
        }

        public Board GetBoardByType (TypeBoard type)
        {

            for (int i = 0; i < _boards.Count; i++)
                if (_boards[i].TypeBoard == type)
                    return _boards[i];

            return null;
        }

        public Layer GetLayerByType(TypeLayer type)
        {
            for (int i = 0; i < _layers.Count; i++)
                if (_layers[i].Type == type)
                    return _layers[i];

            return null;
        }
        
        public Fade GetFadeByType(TypeFade type)
        {
            for (int i = 0; i < _fades.Count; i++)
                if (_fades[i].Type == type)
                    return _fades[i];

            return null;
        }
        
        public Blur GetBlurByType(TypeBlur type)
        {
            for (int i = 0; i < _blurs.Count; i++)
                if (_blurs[i].Type == type)
                    return _blurs[i];

            return null;
        }
        
        public virtual bool LoadBoard(TypeBoard type, bool smoothly = true, float delay = 0f)
        {
            if (ActiveBoard != null)
            {
                if (ActiveBoard.IgnoreTimeLeft > 0)
                    return false;
                
                ActiveBoard.DisableBoard(smoothly);
                Resources.UnloadUnusedAssets();
                TryRemoveNullGameObjectInfo();
            }

            ActiveBoard = GetBoardByType(type);
            ActiveBoard.EnableBoard(smoothly,delay);

            return true;
        }

        public void RememberGameObject(GameObject obj)
        {
            if (_gameObjectInfos.ContainsKey(obj))
            {
                Log.Assert("already remember");
                return;
            }
            
            _gameObjectInfos.Add(obj,new GameObjectInfo(obj));
        }
        
        private void TryRemoveNullGameObjectInfo()
        {
            if(!_gameObjectInfos.HasNullKeys())
                return;
            
            _gameObjectInfos.RemoveNullKeys();
        }

        public GameObjectInfo GetGameObjectInfo(GameObject obj)
        {
            if (!_gameObjectInfos.ContainsKey(obj))
            {
                Log.Assert("not remember");
                return null;
            }

            return _gameObjectInfos[obj];
        }
    }
}
