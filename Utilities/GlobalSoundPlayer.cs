//ArmanDoesStuff 2017

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace Armony.Utilities
{
    public static class GlobalSoundPlayer
    {
        #region variables
        public static ConcurrentStack<AudioSource> sources = new ConcurrentStack<AudioSource>();

        public enum SoundTypes
        {
            Master,
            General,
            Effect,
            Dialogue,
            Music
        };

        private static GameObject audioHolder;
        private static GameObject AudioHolder => audioHolder ??= GameObject.Find("SoundPlayer") ?? new GameObject("SoundPlayer");

        private static AudioMixer mainMixer;
        private static AudioMixer MainMixer => mainMixer ??= Resources.Load<AudioMixer>("MainMixer"); //TODO: ??= new AudioMixer()

        private static AudioMixerGroup[] MixerGroups => MainMixer.FindMatchingGroups(String.Empty);
        #endregion

        #region accessors
        public static float[] SoundVolumes
        {
            get
            {
                float[] soundVols = new float[Enum.GetNames(typeof(SoundTypes)).Length];
                for (int i = 0; i < soundVols.Length; i++)
                {
                    soundVols[i] = GetVolume((SoundTypes)i);
                }
                return soundVols;
            }
        }

        public static bool IsPlaying
        {
            get
            {
                foreach (AudioSource a in sources)
                {
                    if (a.isPlaying)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region functions
        public static float GetVolume(SoundTypes soundType)
        {
            float vol;
            MainMixer.GetFloat($"vol{soundType.ToString()}", out vol);
            vol = (vol + 80f) / 80f;
            return vol;
        }

        public static void SetVolume(SoundTypes soundType, float vol)
        {
            vol = (Mathf.Clamp01(vol) * 80f) - 80f;
            MainMixer.SetFloat($"vol{soundType.ToString()}", vol);
        }

        public static void SetVolumes(float[] vols)
        {
            for (int i = 0; i < vols.Length; i++)
            {
                SetVolume((SoundTypes)i, vols[i]);
            }
        }

        public static AudioSource Play(
            this AudioClip clip,
            SoundTypes soundType = SoundTypes.General,
            float pitch = 1,
            float volume = 1,
            bool applySoundType = true
        )
        {
            AudioSource audS = GetAudioSourceFromPool();
            if (applySoundType)
                audS.outputAudioMixerGroup = MixerGroups[(int)soundType];
            audS.clip = clip;
            audS.pitch = pitch;
            audS.volume = volume;
            audS.Play();
            return audS;
        }

        public static AudioSource PlayRandom(this AudioClip[] clips, SoundTypes soundType = SoundTypes.General, float pitch = 1, float volume = 1)
        {
            return Play(clips[UnityEngine.Random.Range(0, clips.Length)], soundType, pitch, volume);
        }

        public static AudioSource PlayRandom(this List<AudioClip> clips, SoundTypes soundType = SoundTypes.General, float pitch = 1, float volume = 1)
        {
            return PlayRandom(clips.ToArray(), soundType, pitch, volume);
        }

        private static AudioSource GetAudioSourceFromPool()
        {
            sources.TryPop(out AudioSource source);
            if (source == null)
                source = AudioHolder.AddComponent<AudioSource>();
            ActivateSource(source);
            return source;
        }

        private static async void ActivateSource(AudioSource source)
        {
            while (source.isPlaying)
            {
                await Task.Yield();
            }
            sources.Push(source);
        }
        #endregion
    }
}
