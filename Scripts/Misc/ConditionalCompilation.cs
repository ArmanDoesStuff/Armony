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

        private void Awake()
        {
            if (KeepObject())
            {
                Destroy(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private bool KeepObject()
        {
#if UNITY_EDITOR
            if (!m_editor)
                return true;
#endif
#if UNITY_ANDROID
            if (!m_android)
                return true;
#endif
#if UNITY_IOS
            if (!m_ios)
                returntrue;
#endif
#if UNITY_WEBGL
            if (!m_webGL)
                return true;

#endif
#if UNITY_WSA
            if (!m_windows)
                return true;

#endif
#if UNITY_XBOXONE
            if (!m_xbone)
                return true;
#endif
#if UNITY_PS4
            if (!m_ps4)
                return true;
#endif
#if UNITY_STANDALONE
            if (!m_pc)
                return true;
#endif
            return false;
        }
    }
}