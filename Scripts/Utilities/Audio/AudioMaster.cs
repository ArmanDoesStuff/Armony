//ArmanDoesStuff 2017

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Armony.Utilities.Singleton;
using UnityEngine;
using UnityEngine.Audio;

namespace Armony.Utilities.Audio
{
    public enum SoundType
    {
        Master,
        General,
        Effect,
        Dialogue,
        Music
    };

    public class AudioMaster : MonoBehaviour
    {
        private static AudioMaster Instance => SingletonManager.GetInstance<AudioMaster>();

        private static readonly ConcurrentStack<AudioSource> Sources = new();

        [SerializeField]
        private AnimationCurve m_falloffCurve;
        internal static AnimationCurve FalloffCurve => Instance.m_falloffCurve;
        
        [SerializeField]
        private AudioMixer m_mainMixer;
        private AudioMixer MainMixer => m_mainMixer;

        internal static AudioMixerGroup[] MixerGroups => Instance.MainMixer.FindMatchingGroups(string.Empty);

        public static float[] Volumes
        {
            get
            {
                float[] volumes = new float[Enum.GetNames(typeof(SoundType)).Length];
                for (int volume = 0; volume < volumes.Length; volume++)
                {
                    volumes[volume] = GetVolume((SoundType)volume);
                }

                return volumes;
            }
        }

        public static bool IsPlaying => Sources.Any(a => a.isPlaying);

        private static float GetVolume(SoundType soundType)
        {
            Instance.MainMixer.GetFloat($"volume{soundType.ToString()}", out float volume);
            volume = (volume + 80f) / 80f;
            return volume;
        }

        private static void SetVolume(SoundType soundType, float volume)
        {
            volume = (Mathf.Clamp01(volume) * 80f) - 80f;
            Instance.MainMixer.SetFloat($"volume{soundType.ToString()}", volume);
        }

        public static void SetVolumes(float[] volumes)
        {
            for (int i = 0; i < volumes.Length; i++)
            {
                SetVolume((SoundType)i, volumes[i]);
            }
        }

        internal static AudioSource GetAudioSourceFromPool()
        {
            Sources.TryPop(out AudioSource source);
            if (source == null)
            {
                GameObject sourceHolder = new()
                {
                    transform =
                    {
                        parent = Instance.transform
                    }
                };
                source = sourceHolder.AddComponent<AudioSource>();
                source.SetFalloff();
            }

            ActivateSource(source);
            return source;
        }

        private static async void ActivateSource(AudioSource source)
        {
            while (source.isPlaying)
            {
                await Task.Yield();
                if(source == null)
                    return;
            }

            Sources.Push(source);
        }
    }

    public static class AudioClipExtensions
    {
        public static float DefaultSpatialBlend = 1f;

        public static AudioSource Play(
            this AudioClip clip,
            SoundType soundType = SoundType.General,
            float pitch = 1,
            float volume = 1,
            bool applySoundType = true,
            Vector3? position = null
        )
        {
            AudioSource audS = AudioMaster.GetAudioSourceFromPool();
#if UNITY_EDITOR
            audS.gameObject.name = clip.name;
#endif
            if (applySoundType)
                audS.outputAudioMixerGroup = AudioMaster.MixerGroups[(int)soundType];
            audS.clip = clip;
            audS.pitch = pitch;
            audS.volume = volume;
            if (position.HasValue)
                audS.gameObject.transform.position = position.Value;
            audS.spatialBlend = position.HasValue ? DefaultSpatialBlend : 0f;
            audS.Play();
            return audS;
        }

        public static AudioSource PlayRandom(
            this AudioClip[] clips,
            SoundType soundType = SoundType.General,
            float pitch = 1, float volume = 1,
            bool applySoundType = true,
            Vector3? position = null
        ) => Play(clips[UnityEngine.Random.Range(0, clips.Length)], soundType, pitch, volume, applySoundType, position);

        public static AudioSource PlayRandom(
            this List<AudioClip> clips,
            SoundType soundType = SoundType.General,
            float pitch = 1,
            float volume = 1,
            bool applySoundType = true,
            Vector3? position = null
        ) => PlayRandom(clips.ToArray(), soundType, pitch, volume, applySoundType, position);

        public static void SetFalloff(this AudioSource source, float maxDistance = 30f, float? spatialBlend = null)
        {
            source.rolloffMode = AudioRolloffMode.Custom;
            source.maxDistance = maxDistance;
            source.spatialBlend = spatialBlend ?? DefaultSpatialBlend;
            source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, AudioMaster.FalloffCurve);
        }
    }
}