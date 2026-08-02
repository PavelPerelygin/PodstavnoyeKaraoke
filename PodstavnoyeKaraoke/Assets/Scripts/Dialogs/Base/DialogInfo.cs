using System;
using System.Collections.Generic;
using Layers;
using UnityEngine;

namespace Dialogs.Base
{
    [Serializable]
    public class DialogInfo
    {
        public TypeDialog _typeDialog;
        public Vector3 _createPosition = Vector3.zero;
        public bool _needCloseByClick = false;
        public bool _enableFade = true;
        public bool _hideOnSecondScreen = false;
        public float _intensityFade = 0.8f;
        public bool _enableBlur = true;
        public float _intensityBlur = 0.05f;
        public List<TypeLayer> _blockLayers = new List<TypeLayer>() {TypeLayer.Boards,
                                                                    TypeLayer.Boards_ofs,
                                                                    TypeLayer.Hud,
                                                                    TypeLayer.Hud_ofs, };
    }
}