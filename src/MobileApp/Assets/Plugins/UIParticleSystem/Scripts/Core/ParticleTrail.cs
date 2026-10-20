using UnityEngine;
using System.Collections.Generic;

namespace TierStudios.UIParticleSystem.Core
{
    [System.Serializable]
    public class ParticleTrail
    {
        private Vector2[] pointsArray;
        private float[] timesArray;
        private int head = 0;
        private int tail = 0;
        private int count = 0;
        private int capacity = 64;

        public List<Vector2> points => GetPointsList();
        public List<float> times => GetTimesList();

        public Color startColor = Color.white;
        public Color endColor = Color.clear;
        public float width = 2f;

        public bool colorApplied = false;

        public bool needsRebuild = false;
        public int lastVertexCount = 0;
        public float lastUpdateTime = 0f;

        private List<Vector2> cachedPointsList;
        private List<float> cachedTimesList;
        private bool listsCacheDirty = true;

        public ParticleTrail()
        {
            InitializeArrays(capacity);
        }

        public ParticleTrail(int initialCapacity)
        {
            capacity = Mathf.Max(initialCapacity, 16);
            InitializeArrays(capacity);
        }

        private void InitializeArrays(int size)
        {
            pointsArray = new Vector2[size];
            timesArray = new float[size];
            cachedPointsList = new List<Vector2>(size);
            cachedTimesList = new List<float>(size);
        }

        public void AddPoint(Vector2 point)
        {
            if (count >= capacity)
            {
                ExpandCapacity();
            }

            pointsArray[head] = point;
            timesArray[head] = 0f;
            head = (head + 1) % capacity;
            count++;

            needsRebuild = true;
            listsCacheDirty = true;
        }

        private void ExpandCapacity()
        {
            int newCapacity = capacity * 2;
            Vector2[] newPoints = new Vector2[newCapacity];
            float[] newTimes = new float[newCapacity];

            for (int i = 0; i < count; i++)
            {
                int index = (tail + i) % capacity;
                newPoints[i] = pointsArray[index];
                newTimes[i] = timesArray[index];
            }

            pointsArray = newPoints;
            timesArray = newTimes;
            tail = 0;
            head = count;
            capacity = newCapacity;
        }

        public Vector2 GetLastPosition()
        {
            if (count == 0)
                return Vector2.zero;

            int lastIndex = (head - 1 + capacity) % capacity;
            return pointsArray[lastIndex];
        }

        public void UpdateTimes(float deltaTime, float lifetime)
        {
            if (count == 0) return;

            bool pointsRemoved = false;
            int removedCount = 0;

            for (int i = 0; i < count; i++)
            {
                int index = (tail + i) % capacity;
                timesArray[index] += deltaTime;
            }

            while (count > 0)
            {
                if (timesArray[tail] > lifetime)
                {
                    tail = (tail + 1) % capacity;
                    count--;
                    removedCount++;
                    pointsRemoved = true;
                }
                else
                {
                    break;
                }
            }

            if (pointsRemoved)
            {
                needsRebuild = true;
                listsCacheDirty = true;
            }
        }

        public void Clear()
        {
            head = 0;
            tail = 0;
            count = 0;
            needsRebuild = true;
            listsCacheDirty = true;
        }

        public int GetPointCount()
        {
            return count;
        }

        public bool HasValidTrail()
        {
            return count >= 2;
        }

        public void TrimToMaxPoints(int maxPoints)
        {
            if (count <= maxPoints) return;

            int toRemove = count - maxPoints;
            tail = (tail + toRemove) % capacity;
            count = maxPoints;

            needsRebuild = true;
            listsCacheDirty = true;
        }

        public Vector2 GetPoint(int index)
        {
            if (index < 0 || index >= count)
                return Vector2.zero;

            int actualIndex = (tail + index) % capacity;
            return pointsArray[actualIndex];
        }

        public float GetTime(int index)
        {
            if (index < 0 || index >= count)
                return 0f;

            int actualIndex = (tail + index) % capacity;
            return timesArray[actualIndex];
        }

        private List<Vector2> GetPointsList()
        {
            if (listsCacheDirty)
            {
                cachedPointsList.Clear();
                for (int i = 0; i < count; i++)
                {
                    cachedPointsList.Add(GetPoint(i));
                }
                listsCacheDirty = false;
            }
            return cachedPointsList;
        }

        private List<float> GetTimesList()
        {
            if (listsCacheDirty)
            {
                cachedTimesList.Clear();
                for (int i = 0; i < count; i++)
                {
                    cachedTimesList.Add(GetTime(i));
                }
            }
            return cachedTimesList;
        }
    }
}