using System;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class Range : Interactable
    {
        [SerializeField] float m_min;
        [SerializeField] float m_max;
        [SerializeField] float m_step;
    
        [SerializeField] Text m_valueLabel;
        [SerializeField] Button m_nextButton;
        [SerializeField] Button m_backButton;

        float m_value;

        Action<float> m_onChage;
    
        void Awake()
        {
            InitButtons();
        }
    
        public float value
        {
            get { return m_value; }
            set
            {
                m_value = value;
            
                if (m_value > m_max)
                    m_value = m_max;

                if (m_value < m_min)
                    m_value = m_min;
            
                m_value = (float)Math.Round(m_value,2);
            
                m_valueLabel.text = m_value.ToString().Replace(',','.');
            }
        }
    
        public void OnChange(Action<float> action)
        {
            m_onChage = action;
        }
    
        void InitButtons()
        {
            m_nextButton.onClick.AddListener(ButtonPress);
            m_nextButton.DisableOverDownColors();

            m_backButton.onClick.AddListener(ButtonPress);
            m_backButton.DisableOverDownColors();
        }
    
        void NextValue()
        {
            value += m_step;
            m_onChage?.Invoke(value);
        }
    
        void BackValue()
        {
            value -= m_step;
            m_onChage?.Invoke(value);
        }
    
        protected override bool GameObjectClickHandler(GameObject selectedObj)
        {
            if (!base.GameObjectClickHandler(selectedObj))
                return false;

            if (selectedObj == m_nextButton.gameObject)
            {
                NextValue();
            }else if (selectedObj == m_backButton.gameObject)
            {
                BackValue();
            }

            return true;
        }
    }
}