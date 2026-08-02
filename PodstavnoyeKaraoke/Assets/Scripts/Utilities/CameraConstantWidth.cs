using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class CameraConstantWidth : MonoBehaviour
    {
        [SerializeField] RectTransform m_frame;
        [SerializeField] List<Camera> m_camers = new List<Camera>();
        [SerializeField] Vector2 DefaultResolution = new Vector2(720, 1280);
        [Range(0f, 1f)] public float WidthOrHeight = 0;

        List<float> m_initSizes = new List<float>();
        List<float> m_targetAspects = new List<float>();
        List<float> m_initialFovs = new List<float>();
        List<float> m_horizontalFovs = new List<float>();
    
        private void Start()
        {
            for (int i = 0; i < m_camers.Count; i++)
            {
                m_initSizes.Add(m_camers[i].orthographicSize);
                m_targetAspects.Add(DefaultResolution.x / DefaultResolution.y);
                m_initialFovs.Add(m_camers[i].fieldOfView);
                m_horizontalFovs.Add(CalcVerticalFov(m_initialFovs[i], 1 / m_targetAspects[i]));
            }
        }

        private void Update()
        {
            for (int i = 0; i < m_camers.Count; i++)
            {
                if (m_camers[i].orthographic)
                {
                    float constantWidthSize = m_initSizes[i] * (m_targetAspects[i] / m_camers[i].aspect);
                    float orthographicSize = Mathf.Lerp(constantWidthSize, m_initSizes[i], WidthOrHeight);
                    m_camers[i].orthographicSize = orthographicSize;
                    m_frame.sizeDelta = new Vector2(m_frame.sizeDelta.x, orthographicSize * 2f);
                }
                else
                {
                    float constantWidthFov = CalcVerticalFov(m_horizontalFovs[i], m_camers[i].aspect);
                    m_camers[i].fieldOfView = Mathf.Lerp(constantWidthFov, m_initialFovs[i], WidthOrHeight);
                }
            }
        }

        private float CalcVerticalFov(float hFovInDeg, float aspectRatio)
        {
            float hFovInRads = hFovInDeg * Mathf.Deg2Rad;

            float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / aspectRatio);

            return vFovInRads * Mathf.Rad2Deg;
        }
    }
}