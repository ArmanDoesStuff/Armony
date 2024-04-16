//Created by Arman Awan - ArmanDoesStuff 2018

using TMPro;
using UnityEngine;

namespace Armony.Misc.UserInterface
{
    [RequireComponent(typeof(TMP_Text))]
    public class FpsDisplay : MonoBehaviour
    {
        private TMP_Text m_fpsText;

        private int m_frameCounter = 0;
        private float m_timeCounter = 0.0f;
        private float m_lastFramerate = 0.0f;
        public float RefreshTime = 0.5f;

        private void Start()
        {
            m_fpsText = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            if (m_timeCounter < RefreshTime)
            {
                m_timeCounter += Time.deltaTime;
                m_frameCounter++;
            }
            else
            {
                m_lastFramerate = m_frameCounter / m_timeCounter;
                m_fpsText.text = "FPS: " + m_lastFramerate.ToString("000");

                m_frameCounter = 0;
                m_timeCounter = 0.0f;
            }
        }
    }
}