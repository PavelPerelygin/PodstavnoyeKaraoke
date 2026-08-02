using System;
using Dialogs.Base;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Dialogs
{
    public class DelayDialog : Dialog
    {
        [SerializeField] private Text _timeLabel;

        private float _remainingTime;
        
        public void Init(int time, Action onComplete)
        {
            _remainingTime = time;
            
            OnHideComplete.Add(onComplete);
            
            UpdateTimeLabel();
        }

        protected override void Update()
        {
            base.Update();

            UpdateTime();
        }

        private void UpdateTime()
        {
            if(StateDialog != StateDialog.Shown)
                return;
            
            if(_remainingTime <= 0)
                return;
            
            _remainingTime -= Time.deltaTime;

            UpdateTimeLabel();

            if (_remainingTime <= 0)
                OnTimeIsUp();
        }

        private void UpdateTimeLabel()
        {
            _timeLabel.text = ((int)_remainingTime).ToString();
        }

        private void OnTimeIsUp()
        {
            Hide();
        }
    }
}