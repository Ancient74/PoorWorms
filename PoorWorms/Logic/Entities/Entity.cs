using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Entities
{
    public abstract class Entity
    {
        public Vector2 Position { get; set; }
        public virtual bool Dead { get; set; } = false;
        public Texture2D Texture { get; set; }

        public Entity(Texture2D texture)
        {
            Texture = texture;
            Position = new Vector2();
        }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
