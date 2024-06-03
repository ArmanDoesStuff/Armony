//ArmanDoesStuff 2017

using UnityEngine;

namespace Armony.Misc
{
    public class ConditionalCompilation : MonoBehaviour
    {
        [SerializeField]
        private bool m_editor,
            m_android,
            m_ios,
            m_webGL,
            m_windows,
            m_xbone,
            m_ps4,
            m_pc;

        [SerializeField]
        private byte m_uniqueIdentifier;

        private void Awake()
        {
            bool destroyObject = false;

#if UNITY_EDITOR
            if (!m_editor)
                destroyObject = true;
#endif
#if UNITY_ANDROID
            if (!m_android)
                destroyObject = true;
#endif
#if UNITY_IOS
            if (!m_ios)
                destroyObject = true;
#endif
#if UNITY_WEBGL
            if (!m_webGL)
                destroyObject = true;

#endif
#if UNITY_WSA
            if (!m_windows)
                destroyObject = true;

#endif
#if UNITY_XBOXONE
            if (!m_xbone)
                destroyObject = true;
#endif
#if UNITY_PS4
            if (!m_ps4)
                destroyObject = true;
#endif
#if UNITY_STANDALONE
            if (!m_pc)
                destroyObject = true;
#endif
            if (m_uniqueIdentifier > 0)
            {
                if (FindObjectsOfType<ConditionalCompilation>() != null)
                {
                    Destroy(gameObject);
                }
            }

            if (destroyObject)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }
    }
}
