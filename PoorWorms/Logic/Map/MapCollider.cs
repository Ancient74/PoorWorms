using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorWorms.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Map
{
    public class MapCollider
    {
        public MapBase Map { get; }

        public MapCollider(MapBase map)
        {
            if (map == null)
                throw new ArgumentNullException("map was null");
            Map = map;
        }
        public bool CollideWithCircularBody(CircularBody body)
        {
            if (body == null)
                return false;

            float rotation = body.Rotation;
            float velmag = body.Velocity.Length();
            Vector2 response = Vector2.Zero;
            bool collision = false;
            for (float angle = rotation - (float)Math.PI / 2f; angle < rotation + Math.PI / 2f; angle += (float)Math.PI / 16f)
            {
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * body.Radius;
                Vector2 testPos = dir + body.Position;

                int x = (int)testPos.X;
                int y = (int)testPos.Y;

                if (x >= 0 && x < Map.Width && y>=0 &&  y < Map.Height)
                {
                    if (Map.Map[x + y * Map.Width])
                    {
                        response += body.Position - testPos;
                        collision = true;
                    }
                }
               
            }

            if (collision)
            {
                Vector2 respNorm = Vector2.Normalize(response);
                float dot = Vector2.Dot(body.Velocity,respNorm );
                body.Position = body.PrevPosition;
                if (body.CanBounce)
                {
                    body.Velocity = body.Friction * (-2 * dot * respNorm + body.Velocity);
                }
                body.OnTheGround = true;
                return true;
            }
            body.OnTheGround = false;


            return false;
        }
        public bool WeaponAndPlayerCollide(Weapon weapon, List<Worm> worms)
        {
            if (weapon == null)
                return false;
            foreach (var worm in worms.Where(x=>x.Health>0))
            {
                if (weapon.Owner != worm)
                {
                    if (weapon.CanCollide && Vector2.Distance(weapon.Position, worm.Position) < weapon.Radius + worm.Radius)
                        return true;

                }
            }
            return false;
        }
    }
}
