using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoorWorms.Logic.Entities
{
    public abstract class Weapon : CircularBody
    {
        public bool NeedToCharge { get; set; } = true;
        public float ExplosionPower { get; set; }
        public int ExplosionRadius { get; set; }
        public int Damage { get; set; }
        public bool Released { get;protected set; }
        public Worm Owner { get; set; }
        public bool CanCollide { get; set; } = true; //with player

        public event Action<Weapon> OnDead;

        public int Charge
        {
            get
            {
                return _charge;
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value >= 100)
                {
                    value = 100;
                }
                if (!NeedToCharge)
                    value = 100;
                _charge = value;
            }
        }

        protected int _charge = 0;

        public Weapon(float radius, Texture2D texture) : base(radius, texture)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (Released)
            {
                base.Update(gameTime);
            }

        }

        public int Release()
        {
            Released = true;
            return NeedToCharge ? Charge : 100;
        }

    }
}
