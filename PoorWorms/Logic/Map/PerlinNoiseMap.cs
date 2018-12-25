using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorWorms.Logic.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Map
{
    public class PerlinNoiseMap : MapBase
    {
        private Color _bkg = Color.CornflowerBlue;

        public PerlinNoiseMap(int width,int height,Texture2D texture) : base(width,height,texture) 
        {
        }

        private void Reset()
        {
            Map = new bool[Size];
            Colors = Colors.Select(x => _bkg).ToArray();
        }

        public void GenerateMap(int octaves)
        {
            Reset();
            float[] seed = new float[Width];
            Random r = new Random();
            for (int i = 0; i < Width; i++)
            {
                seed[i] = (float)r.NextDouble();
            }
            seed[0] = 0.5f;
            float[] noise = MathHelperExtension.PerlinNoise(seed, octaves);
            for (int x = 0; x < Width; x++)
            {
                int y = (int)MathHelperExtension.Map(noise[x], 0, 1, 0, Height);
                for (; y < Height; y++)
                {
                    SetMapElement(x, y, true);
                }
            }
            GC.Collect();
        }

        public override void SetMapElement(int x, int y, bool solid)
        {
            int index = x + y * Width;
            if(x >= 0 && x < Width && y >= 0 && y < Height)
            {
                Map[index] = solid;
                Colors[index] = solid ? Color.Green : _bkg;
            }
        }
        public override void UpdateTexture()
        {
            Texture.SetData(Colors);
        }
    }
}
