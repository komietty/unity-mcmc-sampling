﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace komietty.Math
{
    public class MCMC
    {
        public static readonly int LIMIT_RESET_LOOP_COUNT = 100;
        public Vector4[] Data { get; private set; }
        public Vector3 Scale { get; private set; }

        Vector3 _curr;
        float _currDensity = 0f;

        public MCMC(Vector4[] data, Vector3 scale)
        {
            this.Data = data;
            this.Scale = scale;
        }

        public void Reset()
        {
            for (var i = 0; _currDensity <= 0f && i < LIMIT_RESET_LOOP_COUNT; i++)
            {
                _curr = new Vector3(Scale.x * Random.value, Scale.y * Random.value, Scale.z * Random.value);
                _currDensity = Density(_curr);
            }
        }

        public IEnumerable<Vector3> Sequence(int nInitialize, int limit)
        {
            return Sequence(nInitialize, limit, 0);
        }
        public IEnumerable<Vector3> Sequence(int nInitialize, int limit, int nSkip)
        {
            Reset();

            for (var i = 0; i < nInitialize; i++)
                Next();

            for (var i = 0; i < limit; i++)
            {
                for (var j = 0; j < nSkip; j++)
                    Next();
                yield return _curr;
                Next();
            }
        }

        void Next()
        {
            Vector3 next = GaussianDistributionCubic.GenerateRandomPointStandard() + _curr;

            var densityNext = Density(next);
            if (_currDensity <= 0f || Mathf.Min(1f, densityNext / _currDensity) >= Random.value)
            {
                _curr = next;
                _currDensity = densityNext;
            }
        }

        float Density(Vector3 pos)
        {
            float weight = 0f;
            for (int i = 0; i < Data.Length; i++)
            {
                Vector3 posi = Data[i];
                float mag = Vector3.SqrMagnitude(pos - posi);
                weight += Mathf.Exp(-mag) * Data[i].w;
            }
            return weight;
        }
    }
}