using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoorWorms.Logic.Entities
{
    public class Rocket : Weapon
    {
        private float _animationSpeed = 0.1f;
        private Rectangle _spriteRect;
        private int _frameOffset = 13;
        private int _frameW = 51;
        private int _frameH = 13;
        private float _currentFrame = 0;
        private bool _dead = false;

        public override bool Dead
        {
            get
            {
                return _dead;
            }
            set
            {
                _dead |= value;
            }
        }

        public Rocket(float radius, Texture2D texture) : base(radius, texture)
        {
            Mass = 100f;
            Friction =new Vector2(0.002f);
            ExplosionPower = 0.9f;
            ExplosionRadius = 100;
            Damage = 30;
            CanBounce = false;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Released)
            {
                spriteBatch.Draw(Texture, destinationRectangle: new Rectangle((int)Position.X, (int)Position.Y, _frameW, _frameH), sourceRectangle: _spriteRect, color: Color.White, rotation: Rotation);
            }
        }
        public override void Update(GameTime gameTime)
        {
            Dead |= OnTheGround;
            _currentFrame += _animationSpeed;
            if (_currentFrame >= 3)
                _currentFrame = 0;
            _spriteRect = new Rectangle(0, _frameOffset * (int)_currentFrame, _frameW, _frameH);

            base.Update(gameTime);
            
            
        }
    }
}
