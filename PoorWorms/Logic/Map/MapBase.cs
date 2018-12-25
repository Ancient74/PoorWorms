using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Map
{
    public abstract class MapBase
    {
        public readonly int Width;
        public readonly int Height;
        public bool[] Map { get; protected set; }
        public Color[] Colors { get; protected set; }
        public Texture2D Texture { get; }
        public int Size => Width * Height;
        public MapBase(int width,int height, Texture2D texture)
        {
            if (width <= 0)
                throw new ArgumentException("Width must be > 0");
            if (height <= 0)
                throw new ArgumentException("Height must be > 0");
            if (texture == null)
                throw new ArgumentNullException("texture was null");
            Texture = texture;
            Width = width;
            Height = height;
            Map = new bool[Size];
            Colors = new Color[Size];
        }
        public abstract void SetMapElement(int x, int y, bool solid);

        public abstract void UpdateTexture();
    }
}
