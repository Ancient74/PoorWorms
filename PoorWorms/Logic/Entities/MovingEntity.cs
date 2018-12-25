using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Entities
{
    public abstract class MovingEntity : Entity
    {
        public Vector2 PrevPosition { get; private set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public int Bounces { get; set; }
        public Vector2 Friction
        {
            get
            {
                return _friction;
            }
            set
            {
                if (value.X < 0)
                    throw new ArgumentException("Friction.X must be >= 0");
                if (value.Y < 0)
                    throw new ArgumentException("Friction.Y must be >= 0");
                if (value.X > 1)
                    throw new ArgumentException("Friction.X must be <= 1");
                if (value.Y > 1)
                    throw new ArgumentException("Friction.Y must be <= 1");
                _friction = value;
            }
        }
        public float Mass
        {
            get
            {
                return _mass;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Mass must be > 0");
                _mass = value;
            }
        }
        public float Rotation
        {
            get
            {
                return ((float)Math.Atan2(Velocity.Y, Velocity.X));
            }
        }
        public bool IsJumping =>Math.Abs(Velocity.Y) > 0.1f ;

        private Vector2 _friction =new Vector2(0.002f);
        private float _mass = 10f;

        public MovingEntity(Texture2D texture) : base(texture)
        {
            Velocity = new Vector2();
            Acceleration = new Vector2();
        }

        public override void Update(GameTime gameTime)
        {

            Velocity = Velocity + Acceleration*gameTime.ElapsedGameTime.Milliseconds;

            if (Velocity.Length() < 0.01f)
            {
                Velocity = Vector2.Zero;
            }

            PrevPosition = Position;
            Position = Position + Velocity * gameTime.ElapsedGameTime.Milliseconds;

            
            Acceleration = Vector2.Zero;
        }
        public void ApplyForce(Vector2 force)
        {
            Acceleration += force/Mass;
        }
    }
}
