using Dialogs.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs
{
    public class AwaitDialog : Dialog
    {
        [SerializeField] private Text _headerText;
        [SerializeField] private Image _progressBar;
        [SerializeField] private Text _progressText;

        public void InitHeader()
        {
            _headerText.gameObject.SetActive(true);
            _headerText.text = "";
            
            _progressBar.gameObject.SetActive(false);
            _progressText.gameObject.SetActive(false);
        }

        public void InitProgress()
        {
            _progressBar.gameObject.SetActive(true);
            _progressText.gameObject.SetActive(true);
            
            _progressBar.fillAmount = 0;
            _progressText.text = "0%";
            
            _headerText.gameObject.SetActive(false);
        }
        public void SetHeader(string header)
        {
            _headerText.text = header;
        }
        
        public void SetProgress(float progress)
        {
            _progressText.text = $"{(int)(progress * 100)}%";
            _progressBar.fillAmount = progress;
        }
    }
}