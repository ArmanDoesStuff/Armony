using Armony.Utilities.Audio;
using UnityEngine;

namespace Armony.Scripts.Utilities.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SetAudioFalloff : MonoBehaviour
    {
        [SerializeField]
        private float m_maxDistance = 30;
        private float MaxDistance => m_maxDistance;

        [SerializeField]
        private AudioSource m_audioSource;
        private AudioSource AudioSource => m_audioSource;

        [SerializeField]
        private bool m_forceSpatialBlend;
        private bool ForceSpatialBlend => m_forceSpatialBlend;
        
        [SerializeField, Range(0f,1f)]
        private float m_spatialBlend;
        private float SpatialBlend => m_spatialBlend;

        private void Start()
        {
            if (ForceSpatialBlend)
            {
                AudioSource.SetFalloff(MaxDistance, spatialBlend: SpatialBlend);
                return;
            }
            AudioSource.SetFalloff(MaxDistance);
        }

        private void OnValidate()
        {
            if (AudioSource == null)
                m_audioSource = GetComponent<AudioSource>();
        }
    }
}
