using UnityEngine;

namespace UI
{
    public class IndicatorVolume : MonoBehaviour
    {
        [SerializeField] private GameObject m_wave_1;
        [SerializeField] private GameObject m_wave_2;

        private float m_value = 0f;
        //----------------------------------------------------------------------------------
        private void Awake()
        {
            Value(m_value);
        }

        //----------------------------------------------------------------------------------
        public void Value(float value)
        {
            m_value = value / 100;
            if (m_value > 1f)
                m_value = 1f;
            else if (m_value < 0f)
                m_value = 0f;

            float procent = m_value * 100f;
            if (procent < 50f)
            {
                float procentFromwave_1 = 0f;
                if(procent > 0)
                    procentFromwave_1 = procent / (50f / 100f);
                SetAlpha(m_wave_1,procentFromwave_1 / 100f);
                SetAlpha(m_wave_2,0f);
            }
            else
            {
                SetAlpha(m_wave_1,1f);
                float procentFromwave_2 = 0f;
                if(procent - 50f > 0)
                    procentFromwave_2 = (procent - 50f) / (50f / 100f);
                SetAlpha(m_wave_2,procentFromwave_2/100f); 
            }
        }
        //----------------------------------------------------------------------------------
        void SetAlpha(GameObject obj, float needAlpha)
        {
            CanvasGroup cg = obj.GetComponent<CanvasGroup>();
            cg.alpha = needAlpha;
        }

    }
}
