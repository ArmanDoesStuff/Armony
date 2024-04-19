//ArmanDoesStuff 2017

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    
    public class AudioMaster : Singleton<AudioMaster>
    {
        private static readonly ConcurrentStack<AudioSource> Sources = new();

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
                source = Instance.gameObject.AddComponent<AudioSource>();
            ActivateSource(source);
            return source;
        }

        private static async void ActivateSource(AudioSource source)
        {
            while (source.isPlaying)
            {
                await Task.Yield();
            }

            Sources.Push(source);
        }
    }

    public static class AudioClipExtensions
    {
        public static AudioSource Play(
            this AudioClip clip,
            SoundType soundType = SoundType.General,
            float pitch = 1,
            float volume = 1,
            bool applySoundType = true
        )
        {
            AudioSource audS = AudioMaster.GetAudioSourceFromPool();
            if (applySoundType)
                audS.outputAudioMixerGroup = AudioMaster.MixerGroups[(int)soundType];
            audS.clip = clip;
            audS.pitch = pitch;
            audS.volume = volume;
            audS.Play();
            return audS;
        }

        public static AudioSource PlayRandom(this AudioClip[] clips, SoundType soundType = SoundType.General, float pitch = 1, float volume = 1)
        {
            return Play(clips[UnityEngine.Random.Range(0, clips.Length)], soundType, pitch, volume);
        }

        public static AudioSource PlayRandom(this List<AudioClip> clips, SoundType soundType = SoundType.General, float pitch = 1, float volume = 1)
        {
            return PlayRandom(clips.ToArray(), soundType, pitch, volume);
        }
    }
}