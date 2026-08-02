using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameHelper
{
    public class GameHelpereMessage : MonoBehaviour
    {
        [TextArea] private string _messageStr;
        [SerializeField] private Text _header;
        [SerializeField] private Text _message;

        private void Awake()
        {
            _header.gameObject.SetActive(false);
            _message.gameObject.SetActive(false);
        }

        public void SetHeader(string value)
        {
            if(!_header.gameObject.activeSelf)
                _header.gameObject.SetActive(true);

            _header.text = value;
        }

        public void SetMessage(string value)
        {
            if(!_message.gameObject.activeSelf)
                _message.gameObject.SetActive(true);
            
            _message.text = value.Replace("\\n","\n");
        }

        public void Show(Action onCompleted = null)
        {
            gameObject.transform.localScale = Vector3.zero;
            gameObject.LeanScale(Vector3.one, 0.3f).setOnComplete(() => {onCompleted?.Invoke();});
        }

        public void Hide(Action onCompleted = null)
        {
            gameObject.LeanScale(Vector3.zero, 0.3f).setOnComplete(() =>
            {
                Destroy(gameObject);
                onCompleted?.Invoke();
            });
        }
    }
}