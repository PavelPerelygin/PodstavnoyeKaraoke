using UnityEngine;

namespace UI
{
    public class WaitProcess : MonoBehaviour
    {
        [SerializeField] private GameObject m_indicator;
        void Update()
        {
            Vector3 rotation = m_indicator.transform.eulerAngles;
            rotation.z -= 0.5f;
            m_indicator.transform.eulerAngles = rotation;
        }
    }
}
