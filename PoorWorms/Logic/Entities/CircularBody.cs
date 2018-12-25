using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Entities
{
    public abstract class CircularBody : MovingEntity
    {
        public readonly float Radius;
        public float Diameter => Radius * 2;
        public bool OnTheGround { get; set; } = false;
        public bool CanBounce { get; set; } = true;
        public CircularBody(float radius, Texture2D texture) : base(texture)
        {
            if (radius <= 0)
                throw new ArgumentException("radius must be > 0");

            Radius = radius;
        }
    }
}
