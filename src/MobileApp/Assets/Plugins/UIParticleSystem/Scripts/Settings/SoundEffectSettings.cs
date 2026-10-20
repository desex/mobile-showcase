using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace TierStudios.UIParticleSystem.Settings
{
    [System.Serializable]
    public class SoundEffectSettings
    {
        [Header("Sound Effect Settings")]
        [SerializeField] private bool enableSoundEffects = false;
        [SerializeField] private SoundEffectTrigger soundTrigger = SoundEffectTrigger.OnParticleBirth;

        [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();
        [SerializeField] private AudioClipPlayMode clipPlayMode = AudioClipPlayMode.Random;

        [Header("Advanced Settings")]
        [SerializeField] private AudioMixerGroup audioMixerGroup = null;

        [SerializeField][Range(0f, 1f)] private float volume = 1f;
        [SerializeField] private bool useRandomVolume = false;
        [SerializeField][Range(0f, 1f)] private float minVolume = 0.8f;
        [SerializeField][Range(0f, 1f)] private float maxVolume = 1f;

        [SerializeField] private PitchMode pitchMode = PitchMode.Constant;
        [SerializeField][Range(-3f, 3f)] private float pitch = 1f;
        [SerializeField][Range(-3f, 3f)] private float minPitch = 0.9f;
        [SerializeField][Range(-3f, 3f)] private float maxPitch = 1.1f;

        [SerializeField][Range(0f, 1f)] private float spatialBlend = 0f;
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 500f;

        [SerializeField] private bool loop = false;
        [SerializeField][Range(0f, 5f)] private float cooldownTime = 0f;
        [SerializeField] private int maxSimultaneousSounds = 10;
        [SerializeField] private bool playOnlyOnce = false;

        private int currentClipIndex = 0;
        private float lastPlayTime = -999f;
        private bool hasPlayedOnce = false;

        public bool EnableSoundEffects
        {
            get => enableSoundEffects;
            set => enableSoundEffects = value;
        }

        public SoundEffectTrigger SoundTrigger
        {
            get => soundTrigger;
            set => soundTrigger = value;
        }

        public List<AudioClip> AudioClips => audioClips;

        public AudioClipPlayMode ClipPlayMode
        {
            get => clipPlayMode;
            set => clipPlayMode = value;
        }

        public AudioMixerGroup AudioMixerGroup
        {
            get => audioMixerGroup;
            set => audioMixerGroup = value;
        }

        public float Volume => volume;
        public bool UseRandomVolume => useRandomVolume;
        public float MinVolume => minVolume;
        public float MaxVolume => maxVolume;

        public PitchMode PitchModeValue => pitchMode;
        public float Pitch => pitch;
        public float MinPitch => minPitch;
        public float MaxPitch => maxPitch;

        public float SpatialBlend => spatialBlend;
        public float MinDistance => minDistance;
        public float MaxDistance => maxDistance;

        public bool Loop => loop;
        public float CooldownTime => cooldownTime;
        public int MaxSimultaneousSounds => maxSimultaneousSounds;
        public bool PlayOnlyOnce => playOnlyOnce;

        public AudioClip GetNextClip()
        {
            if (audioClips == null || audioClips.Count == 0)
                return null;

            audioClips.RemoveAll(clip => clip == null);

            if (audioClips.Count == 0)
                return null;

            AudioClip selectedClip = null;

            switch (clipPlayMode)
            {
                case AudioClipPlayMode.Random:
                    selectedClip = audioClips[Random.Range(0, audioClips.Count)];
                    break;

                case AudioClipPlayMode.Sequential:
                    selectedClip = audioClips[currentClipIndex];
                    currentClipIndex = (currentClipIndex + 1) % audioClips.Count;
                    break;

                case AudioClipPlayMode.Single:
                    selectedClip = audioClips[0];
                    break;
            }

            return selectedClip;
        }

        public float GetVolume()
        {
            if (useRandomVolume)
                return Random.Range(minVolume, maxVolume);
            return volume;
        }

        public float GetPitch()
        {
            switch (pitchMode)
            {
                case PitchMode.Random:
                    return Random.Range(minPitch, maxPitch);

                case PitchMode.Sequential:
                    float range = maxPitch - minPitch;
                    float step = range / Mathf.Max(1, maxSimultaneousSounds);
                    float pitchValue = minPitch + (currentClipIndex * step);
                    return Mathf.Clamp(pitchValue, minPitch, maxPitch);

                case PitchMode.Constant:
                default:
                    return pitch;
            }
        }

        public bool CanPlay(float currentTime)
        {
            if (playOnlyOnce && hasPlayedOnce)
                return false;

            if (currentTime - lastPlayTime < cooldownTime)
                return false;

            return true;
        }

        public void MarkAsPlayed(float currentTime)
        {
            lastPlayTime = currentTime;

            if (playOnlyOnce)
                hasPlayedOnce = true;
        }

        public void ResetPlayState()
        {
            hasPlayedOnce = false;
            lastPlayTime = -999f;
            currentClipIndex = 0;
        }
    }

    public enum SoundEffectTrigger
    {
        OnParticleBirth,
        OnParticleDeath,
        OnSystemStart,
        OnSystemStop,
        OnParticleCollision
    }

    public enum AudioClipPlayMode
    {
        Random,
        Sequential,
        Single
    }

    public enum PitchMode
    {
        Constant,
        Random,
        Sequential
    }
}