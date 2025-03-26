//Created by Arman Awan - ArmanDoesStuff 2018

using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Armony.Misc.UserInterface
{
    [RequireComponent(typeof(TMP_Text))]
    public class FpsDisplay : MonoBehaviour
    {
        private TMP_Text mFPSText;

        private int mFrameCounter;
        private float mTimeCounter;
        private float mLastFramerate;
        public float refreshTime = 0.5f;

        private void Start()
        {
            mFPSText = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            if (mTimeCounter < refreshTime)
            {
                mTimeCounter += Time.deltaTime;
                mFrameCounter++;
            }
            else
            {
                mLastFramerate = mFrameCounter / mTimeCounter;
                mFPSText.text = "FPS: " + mLastFramerate.ToString("000");

                mFrameCounter = 0;
                mTimeCounter = 0.0f;
            }
        }
    }
}