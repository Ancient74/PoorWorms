using Microsoft.Xna.Framework.Graphics;
using PoorWorms.Logic.Entities;
using PoorWorms.Logic.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Util
{
    public static class Assets
    {
        public static Texture2D RocketTexture { get; set; }
        public static Texture2D PlayerTexture { get; set; }
        public static Texture2D CrossTexture { get; set; }
        public static Texture2D GraveTexture { get; set; }
        public static SpriteFont EndGameFont { get; set; }
        public static MapBase Map { get; set; }
        public static Camera.Camera Camera { get; set; }
        public static List<CircularBody> Entities { get; set; }
        public static GraphicsDevice GraphicsDevice { get; set; }
    }
}
