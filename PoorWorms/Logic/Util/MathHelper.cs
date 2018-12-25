using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Util
{
    public static class MathHelperExtension
    {
        public static float Map(float x, float minX, float maxX, float newMinX, float newMaxX)
        {
            x = (x - minX) / (maxX - minX);
            return newMinX + x * (newMaxX - newMinX);
        }
        public static float[] PerlinNoise(float[]seed,int octaves)
        {
            if (octaves <= 0)
                throw new ArgumentException("Octaves must be > 0");
            if (octaves >= 10)
                throw new ArgumentException("Octaves must be < 10");
            
            int n = seed.Length;
            float[] perlinNoise = new float[n];
            for (int i = 0; i < n; i++)
            {
                float noise = 0f;
                float scale = 1f;
                float scaleSum = 0;
                for (int j = 0; j < octaves; j++)
                {
                    int pitch = n >> j;
                    int sample1 = (i / pitch) * pitch;
                    int sample2 = (sample1 + pitch) % n;

                    float blend = (float)(i - sample1) / (float)pitch;

                    float sample = (1 - blend) * seed[sample1] + blend * seed[sample2];

                    noise += scale * sample;
                    scaleSum += scale;
                    scale /= 2;
                }
                perlinNoise[i] = noise/scaleSum;
            }
            return perlinNoise;
        }
    }
}
