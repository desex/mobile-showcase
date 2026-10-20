using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace TierStudios.UIParticleSystem.Utilities
{
    public class AudioSourcePool
    {
        private Stack<AudioSource> availableSources;
        private List<AudioSource> activeSources;
        private Transform parent;
        private int poolCapacity;
        private int totalCreated = 0;

        public AudioSourcePool(Transform parentTransform, int capacity = 10)
        {
            parent = parentTransform;
            poolCapacity = capacity;
            availableSources = new Stack<AudioSource>(capacity);
            activeSources = new List<AudioSource>(capacity);
        }

        public AudioSource Get()
        {
            AudioSource source = null;

            while (availableSources.Count > 0)
            {
                source = availableSources.Pop();

                if (source != null && source.gameObject != null)
                {
                    source.gameObject.SetActive(true);
                    activeSources.Add(source);
                    return source;
                }

                source = null;
            }

            source = CreateNewAudioSource();
            if (source != null)
            {
                activeSources.Add(source);
            }
            return source;
        }

        private AudioSource CreateNewAudioSource()
        {
            if (parent == null || parent.gameObject == null || !parent.gameObject.activeInHierarchy)
            {
                return null;
            }

            GameObject audioObj = new GameObject($"PooledAudioSource_{totalCreated}");
            audioObj.hideFlags = HideFlags.HideInHierarchy;

            if (parent != null && parent.gameObject != null && parent.gameObject.activeInHierarchy)
            {
                audioObj.transform.SetParent(parent, false);
                audioObj.transform.localPosition = Vector3.zero;
            }
            else
            {
                if (Application.isPlaying)
                    Object.Destroy(audioObj);
                else
                    Object.DestroyImmediate(audioObj);
                return null;
            }

            AudioSource source = audioObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;

            totalCreated++;
            return source;
        }

        public void Return(AudioSource source)
        {
            if (source == null || source.gameObject == null)
                return;

            activeSources.Remove(source);

            source.Stop();
            source.clip = null;
            source.loop = false;
            source.outputAudioMixerGroup = null;

            source.gameObject.SetActive(false);

            if (availableSources.Count < poolCapacity)
            {
                availableSources.Push(source);
            }
            else
            {
                if (Application.isPlaying)
                    Object.Destroy(source.gameObject);
                else
                    Object.DestroyImmediate(source.gameObject);
            }
        }

        public void UpdateActiveSources()
        {
            for (int i = activeSources.Count - 1; i >= 0; i--)
            {
                AudioSource source = activeSources[i];

                if (source == null || source.gameObject == null || !source.isPlaying)
                {
                    if (source != null)
                        Return(source);
                    else
                        activeSources.RemoveAt(i);
                }
            }
        }

        public void Clear()
        {
            bool isParentBeingDeactivated = parent != null &&
                                           parent.gameObject != null &&
                                           !parent.gameObject.activeInHierarchy;

            for (int i = activeSources.Count - 1; i >= 0; i--)
            {
                AudioSource source = activeSources[i];
                if (source != null && source.gameObject != null)
                {
                    source.Stop();
                    source.gameObject.SetActive(false);

                    if (isParentBeingDeactivated)
                    {
                        if (Application.isPlaying)
                            Object.Destroy(source.gameObject);
                    }
                    else
                    {
                        if (Application.isPlaying)
                            Object.Destroy(source.gameObject);
                        else
                            Object.DestroyImmediate(source.gameObject);
                    }
                }
            }

            activeSources.Clear();

            while (availableSources.Count > 0)
            {
                var source = availableSources.Pop();
                if (source != null && source.gameObject != null)
                {
                    source.gameObject.SetActive(false);

                    if (isParentBeingDeactivated)
                    {
                        if (Application.isPlaying)
                            Object.Destroy(source.gameObject);
                    }
                    else
                    {
                        if (Application.isPlaying)
                            Object.Destroy(source.gameObject);
                        else
                            Object.DestroyImmediate(source.gameObject);
                    }
                }
            }

            availableSources.Clear();
        }

        public int GetActiveCount()
        {
            return activeSources.Count;
        }

        public int GetAvailableCount()
        {
            return availableSources.Count;
        }

        public int GetTotalCreated()
        {
            return totalCreated;
        }
    }
}