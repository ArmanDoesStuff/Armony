//Created by Arman Awan - ArmanDoesStuff 2018

using System.Collections;
using UnityEngine;

namespace Armony.Misc
{
    public class CamShake : MonoBehaviour
    {
        public static CamShake Instance;
        [Range(0, 1)]
        [SerializeField]
        private float m_trauma;
        [SerializeField]
        private float m_traumaMult = 16; //the power of the shake
        [SerializeField]
        private float m_traumaMag = 0.8f; //the range of movment
        [SerializeField]
        private float m_traumaRotMag = 17f; //the rotational power
        [SerializeField]
        private float m_traumaDepthMag = 0.6f; //the depth multiplier
        [SerializeField]
        private float m_traumaDecay = 1.3f; //how quickly the shake falls off

        private float m_timeCounter = 0; //counter stored for smooth transition

        [SerializeField]
        private bool m_shakeActive = true;
        
        public bool ShakeActive
        {
            get => m_shakeActive;
            set
            {
                StopAllCoroutines();
                m_shakeActive = value;
                if (!value)
                    StartCoroutine(DeactivateShake_Croute()); //In case it is turned off mid-shake, lerp back to zero
            }
        }
        public float Trauma //accessor is used to keep trauma within 0 to 1 range
        {
            get => m_trauma;
            set => m_trauma = Mathf.Clamp01(value); //can possibly have a min value to clamp to, if needed
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Debug.LogError($"CamShake instance already exists on {Instance.gameObject.name}");
        }

        //Get a perlin float between -1 & 1, based off the time counter.
        private float GetFloat(float seed)
        {
            return (Mathf.PerlinNoise(seed, m_timeCounter) - 0.5f) * 2f;
        }

        //use the above function to generate a Vector3, different seeds are used to ensure different numbers
        private Vector3 GetVec3()
        {
            return new Vector3(
                GetFloat(1),
                GetFloat(10),
                GetFloat(100) * m_traumaDepthMag
                );
        }

        private IEnumerator DeactivateShake_Croute()
        {
            while (Vector3.Distance(transform.localPosition, Vector3.zero) < 0.1f)
            {
                //lerp back towards default position and rotation once shake is done
                Vector3 newPos = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime);
                transform.localPosition = newPos;
                transform.localRotation = Quaternion.Euler(newPos * m_traumaRotMag);
                yield return new WaitForFixedUpdate();
            }
        }

        private void FixedUpdate()
        {
            if (m_shakeActive)
            {
                if (m_trauma != 0) //Only run if trauma is high enough
                {
                    //increase the time counter (how fast the position changes) based off the traumaMult and some root of the Trauma
                    m_timeCounter += Time.deltaTime * Mathf.Pow(m_trauma, 0.3f) * m_traumaMult;
                    //Bind the movement to the desired range
                    Vector3 newPos = GetVec3() * (m_traumaMag * m_trauma);
                    transform.localPosition = newPos;
                    //rotation modifier applied here
                    transform.localRotation = Quaternion.Euler(newPos * m_traumaRotMag);
                    //decay faster at higher values
                    m_trauma -= Time.deltaTime * m_traumaDecay * (m_trauma + 0.3f);

                    //Reset when below threshold
                    if (m_trauma < 0.001f)
                    {
                        m_trauma = 0;
                        transform.localRotation = Quaternion.identity;
                        transform.localPosition = Vector3.zero;
                    }
                }
            }
        }
    }
}