using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoorWorms.Logic.Entities
{
    public class BoomParticle : CircularBody
    {
        public override bool Dead { get => Bounces <= 0; }

        public BoomParticle(Vector2 pos,float radius, int bounces, Color avgColor,Texture2D texture) : base(radius,texture)
        {
            Color[] data = new Color[(int)(radius * radius *4)];
            Random r = new Random();
            data = data.Select(x => new Color((uint)(avgColor.PackedValue + x.PackedValue + r.Next(-100, 100)))).ToArray();
            texture.SetData(data);
            Position = pos;
            Bounces = bounces;
            Mass = 10f;
            Friction = new Vector2(0.9f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, destinationRectangle: new Rectangle(Position.ToPoint(), new Point(Texture.Width, Texture.Height)), color: Color.White, rotation: Rotation);
        }
    }
}
