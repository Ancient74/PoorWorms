using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorWorms.Logic.Entities;
using PoorWorms.Logic.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Map
{
    public static class ExplosionMaker
    {
        private static int particleRadius = 4;
        public static BoomParticle[] Explode(MapBase map,List<CircularBody>entities,int dmg, float radius, float power, Vector2 pos, GraphicsDevice graphicsDevice)
        {
            int x = (int)radius - 1;//https://en.wikipedia.org/wiki/Midpoint_circle_algorithm
            int y =  0;
            int dx = 1;
            int dy = 1;
            int err = dx - ((int)radius << 1);
            int x0 = (int)pos.X;
            int y0 = (int)pos.Y;
            int total = 0;
            long colorSum = 0;
            Action<int, int, int> line = (int x1, int x2, int yn) => 
            {
                int Xend = Math.Max(x1, x2);
                int Xbeg = Math.Min(x1, x2);
                for (int xi = Xbeg; xi < Xend; xi++)
                {
                    int index = xi + yn * map.Width;
                    if(xi >= 0 && xi < map.Width && yn >= 0 && yn < map.Height)
                    {
                        if (map.Map[index])
                        {
                            colorSum += map.Colors[index].PackedValue;
                            total++;
                        }
                    }
                       
                    map.SetMapElement(xi, yn, false);
                }
            };
            while (x >= y)
            {
                line(x0 - x, x0 + x, y0 - y);
                line(x0 - y, x0 + y, y0 - x);
                line(x0 - x, x0 + x, y0 + y);
                line(x0 - y, x0 + y, y0 + x);
                if (err <= 0)
                {
                    y++;
                    err += dy;
                    dy += 2;
                }

                if (err > 0)
                {
                    x--;
                    dx += 2;
                    err += dx - ((int)radius << 1);
                }
            }
            Random r = new Random();
            foreach (CircularBody body in entities)//Ударная волна для юнитов в радиусе взрыва
            {
                float dist = Vector2.Distance(pos, body.Position);
                if (dist < 0.01f)
                    dist = 0.01f;
                if (dist < radius)
                {
                    float revDist = 1 / dist;
                    Vector2 dir = body.Position - pos;
                    if (dir.Length() == 0)
                    {
                        dir = new Vector2(dist);
                    }
                    dir.Normalize();
                    body.Velocity = power *  dir;
                    if(body is Worm)
                    {
                        dmg = dmg + (int)MathHelperExtension.Map(revDist, 1 / radius, 1 / 0.01f,-dmg * 0.1f,dmg * 0.1f);
                        (body as Worm).Health -= dmg;
                        (body as Worm).Health = Math.Max(0, (body as Worm).Health);
                    }
                }
            }

            BoomParticle[] particles = new BoomParticle[(int)radius/10];
            for (int i = 0; i < particles.Length; i++)
            {
                uint color;
                if (total != 0)
                    color = (uint)(colorSum / total);
                else
                    color = Color.Green.PackedValue;
                particles[i] = new BoomParticle(pos, particleRadius, 5, new Color(color),new Texture2D(graphicsDevice,particleRadius*2,particleRadius*2));
                particles[i].Velocity = new Vector2((float)r.NextDouble() - 0.5f, (float)r.NextDouble() - 0.5f);
            }
            map.UpdateTexture();
            return particles;
        }
    }
}
