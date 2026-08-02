using System;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class CopyText : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Text _label;

        private void Awake()
        {
            _button.onClick.AddListener(OnCopy);
        }

        private void OnCopy()
        {
            GUIUtility.systemCopyBuffer = _label.text;
        }
    }
}