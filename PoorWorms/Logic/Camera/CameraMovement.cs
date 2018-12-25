using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PoorWorms.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Camera
{
    public class CameraMovement
    {
        public Camera Camera { get; }
        public int ScreenWidth { get; }
        public int ScreenHeight { get; }
        public int MovementOffset { get; set; } = 200;
        public Entity CameraLock { get; set; } = null;
        public CameraMovement(Camera camera, int screenWidth, int screenHeight)
        {
            if (screenWidth <= 0)
                throw new ArgumentException("screenWidth must be > 0");
            if (screenHeight <= 0)
                throw new ArgumentException("screenHeight must be >0");
            if (camera == null)
                throw new ArgumentNullException("camera was null");

            Camera = camera;
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;
        }
        public void Update(GameTime gameTime)
        {
            Vector2 pos = Mouse.GetState().Position.ToVector2();
            Camera camera = Camera;
            if (pos.X < MovementOffset)
            {
                camera.Position = new Vector2(camera.Position.X - camera.Speed, camera.Position.Y);
            }
            if (pos.Y < MovementOffset)
            {
                camera.Position = new Vector2(camera.Position.X, camera.Position.Y - camera.Speed);
            }
            if (pos.X > ScreenWidth - MovementOffset)
            {
                camera.Position = new Vector2(camera.Position.X + camera.Speed, camera.Position.Y);
            }
            if (pos.Y > ScreenHeight - MovementOffset)
            {
                camera.Position = new Vector2(camera.Position.X, camera.Position.Y + camera.Speed);
            }
            if(CameraLock != null)
            {
                Vector2 center = CameraLock.Position- Camera.Scissor.Size.ToVector2()/2 / Camera.Zoom;
                camera.Position = Vector2.Lerp(Camera.Position, center, 0.07f);
            }
        }
    }
}
