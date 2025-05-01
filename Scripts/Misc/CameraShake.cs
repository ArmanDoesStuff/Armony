//Created by Arman Awan - ArmanDoesStuff 2018

using System.Collections;
using Armony.Utilities.Singleton;
using UnityEngine;
using UnityEngine.Serialization;

namespace Armony.Misc
{
    public class CameraShake : MonoBehaviour
    {
        private static CameraShake Instance => SingletonManager.GetInstance<CameraShake>();
        [Range(0, 1)]
        [SerializeField]
        private float trauma;
        [SerializeField]
        private float traumaMult = 16; //the power of the shake
        [SerializeField]
        private float traumaMag = 0.8f; //the range of movment
        [SerializeField]
        private float traumaRotMag = 17f; //the rotational power
        [SerializeField]
        private float traumaDepthMag = 0.6f; //the depth multiplier
        [SerializeField]
        private float traumaDecay = 1.3f; //how quickly the shake falls off

        private float timeCounter = 0; //counter stored for smooth transition

        [SerializeField]
        private bool shakeActive = true;
        
        public bool ShakeActive
        {
            get => shakeActive;
            set
            {
                StopAllCoroutines();
                shakeActive = value;
                if (!value)
                    StartCoroutine(DeactivateShake()); //In case it is turned off mid-shake, lerp back to zero
            }
        }
        
        public float Trauma
        {
            get => trauma;
            set => trauma = Mathf.Clamp01(value); //can add a min value to clamp to, if needed
        }

        private Vector3 GetPerlinVector()
        {
            return new Vector3(
                GetFloat(1),
                GetFloat(10),
                GetFloat(100) * traumaDepthMag
                );
            
            float GetFloat(float _seed)
            {
                return (Mathf.PerlinNoise(_seed, timeCounter) - 0.5f) * 2f;
            }
        }

        private IEnumerator DeactivateShake()
        {
            while (Vector3.Distance(transform.localPosition, Vector3.zero) < 0.1f)
            {
                //lerp back towards default position and rotation once shake is done
                Vector3 newPos = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime);
                transform.localPosition = newPos;
                transform.localRotation = Quaternion.Euler(newPos * traumaRotMag);
                yield return new WaitForFixedUpdate();
            }
        }

        private void FixedUpdate()
        {
            if (!ShakeActive) return;
            if (trauma == 0) return;
            //increase the time counter (how fast the position changes) based off the traumaMult and some root of the Trauma
            timeCounter += Time.deltaTime * Mathf.Pow(trauma, 0.3f) * traumaMult;
            //Bind the movement to the desired range
            Vector3 newPos = GetPerlinVector() * (traumaMag * trauma);
            transform.localPosition = newPos;
            //rotation modifier applied here
            transform.localRotation = Quaternion.Euler(newPos * traumaRotMag);
            //decay faster at higher values
            Trauma -= Time.deltaTime * traumaDecay * (Trauma + 0.3f);

            //Reset when below threshold
            if (Trauma > 0.001f) return;
            Trauma = 0;
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
        }
    }
}