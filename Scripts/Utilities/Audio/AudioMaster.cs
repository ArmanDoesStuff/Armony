//ArmanDoesStuff 2017

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Armony.Utilities.Singleton;
using UnityEngine;
using UnityEngine.Audio;

// ReSharper disable MemberCanBePrivate.Global
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

    [DisallowMultipleComponent]
    public class AudioMaster : MonoBehaviour
    {
        private static AudioMaster Instance => SingletonManager.GetInstance<AudioMaster>();

        [SerializeField]
        private AudioMixer mainMixer;

        [SerializeField]
        private AnimationCurve falloffCurve = AnimationCurve.Linear(0, 1, 1, 0);
        internal static AnimationCurve FalloffCurve => Instance.falloffCurve;

        private static readonly ConcurrentStack<AudioSource> sources = new();

        internal static AudioMixerGroup[] MixerGroups => Instance.mainMixer.FindMatchingGroups(string.Empty);

        public static bool IsPlaying => sources.Any(_a => _a.isPlaying);

        public static float GetVolume(SoundType _soundType)
        {
            Instance.mainMixer.GetFloat(_soundType.ToString(), out float volume);
            volume = (volume + 80f) / 80f;
            return volume;
        }

        public static Dictionary<SoundType, float> GetVolumes()
        {
            Dictionary<SoundType, float> volumes = new();
            foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
            {
                volumes[type] = GetVolume(type);
            }

            return volumes;
        }

        public static void SetVolume(SoundType _soundType, float _volume)
        {
            _volume = (Mathf.Clamp01(_volume) * 80f) - 80f;
            Instance.mainMixer.SetFloat(_soundType.ToString(), _volume);
        }

        public static void SetVolumes(Dictionary<SoundType, float> _volumes)
        {
            foreach (KeyValuePair<SoundType, float> pair in _volumes)
            {
                SetVolume(pair.Key, pair.Value);
            }
        }

        internal static AudioSource GetAudioSourceFromPool()
        {
            sources.TryPop(out AudioSource source);
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

        private static async void ActivateSource(AudioSource _source)
        {
            await Task.Yield();
            while (_source.isPlaying)
            {
                await Task.Yield();
                if (_source == null)
                    return;
            }

            sources.Push(_source);
        }
    }

    public static class AudioClipExtensions
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
        //TODO: Add setter that calls SetFalloff on all sources
        public static float defaultSpatialBlend = 1f;

        public static AudioSource Play(
            this AudioClip _clip,
            SoundType _soundType = SoundType.General,
            float _pitch = 1,
            float _volume = 1,
            Vector3? _position = null
        )
        {
            AudioSource audS = AudioMaster.GetAudioSourceFromPool();
#if UNITY_EDITOR
            audS.gameObject.name = _clip.name;
#endif
            audS.outputAudioMixerGroup = AudioMaster.MixerGroups[(int)_soundType];
            audS.clip = _clip;
            audS.pitch = _pitch;
            audS.volume = _volume;
            if (_position.HasValue)
                audS.gameObject.transform.position = _position.Value;
            audS.spatialBlend = _position.HasValue ? defaultSpatialBlend : 0f;
            audS.Play();
            return audS;
        }

        public static AudioSource PlayRandom(
            this AudioClip[] _clips,
            SoundType _soundType = SoundType.General,
            float _pitch = 1, float _volume = 1,
            Vector3? _position = null
        ) => Play(_clips[UnityEngine.Random.Range(0, _clips.Length)], _soundType, _pitch, _volume, _position);

        public static AudioSource PlayRandom(
            this List<AudioClip> _clips,
            SoundType _soundType = SoundType.General,
            float _pitch = 1,
            float _volume = 1,
            Vector3? _position = null
        ) => PlayRandom(_clips.ToArray(), _soundType, _pitch, _volume, _position);

        public static void SetFalloff(this AudioSource _source, float _maxDistance = 30f, float? _spatialBlend = null)
        {
            _source.rolloffMode = AudioRolloffMode.Custom;
            _source.maxDistance = _maxDistance;
            _source.spatialBlend = _spatialBlend ?? defaultSpatialBlend;
            _source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, AudioMaster.FalloffCurve);
        }
    }
}