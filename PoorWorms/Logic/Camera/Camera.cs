using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Camera
{
    public class Camera
    {
        public Matrix Transform { get;private set; }
        public Vector2 Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                if (value.X < 0.1)
                {
                    value = new Vector2(0.1f, value.Y);
                }
                if (value.Y < 0.1)
                {
                    value = new Vector2(value.X, 0.1f);
                }
                _zoom = value;
            }
        }
        public float Rotate { get; set; } = 0f;
        public float Speed { get; set; } = 10f;
        public Vector2 Position { get; set; }
        public int Width => Screen.Width;
        public int Height => Screen.Height;

        public CameraMovement CameraMovement { get; set; }

        public Rectangle Screen { get;private set; }
        public Rectangle Scissor { get;private set; }

        private Vector2 _zoom = new Vector2(1f);

        public Camera(Rectangle screenRect, Rectangle scissorRect)
        {
            if (screenRect == null)
                throw new ArgumentNullException("screenRect was null");
            if (scissorRect == null)
                throw new ArgumentNullException("scissorRect was null");

            Screen = screenRect;
            Scissor = scissorRect;
            CameraMovement = new CameraMovement(this, Scissor.Width, Scissor.Height);
        }

        public void Update(Vector2 position)
        {
            if (position.X < 0)
                position = new Vector2(0, position.Y);
            if (position.Y < 0)
                position = new Vector2(position.X, 0);
            if (position.X >= Screen.Width - Scissor.Width / Zoom.X)
                position = new Vector2(Screen.Width - Scissor.Width / Zoom.X, position.Y);
            if (position.Y >= Screen.Height - Scissor.Height / Zoom.Y)
                position = new Vector2(position.X, Screen.Height - Scissor.Height / Zoom.Y);

            
            Position = position;
            Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                                                 Matrix.CreateRotationZ(Rotate) *
                                                 Matrix.CreateScale(Zoom.X, Zoom.Y, 0);
        }

    }
}
