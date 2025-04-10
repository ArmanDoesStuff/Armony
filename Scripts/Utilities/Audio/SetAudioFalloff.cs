using Armony.Utilities.Audio;
using UnityEngine;

namespace Armony.Scripts.Utilities.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SetAudioFalloff : MonoBehaviour
    {
        [SerializeField]
        private float maxDistance = 30;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private bool forceSpatialBlend;
        
        [SerializeField, Range(0f,1f)]
        private float spatialBlend;

        private void Start()
        {
            if (forceSpatialBlend)
            {
                audioSource.SetFalloff(maxDistance, _spatialBlend: spatialBlend);
                return;
            }
            audioSource.SetFalloff(maxDistance);
        }

        private void OnValidate()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }
    }
}
