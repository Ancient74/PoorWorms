using PoorWorms.Logic.Map;
using PoorWorms.Logic.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Entities.Weapons
{
    public static class WeaponFactory
    {

        public static Weapon Create(string name,Worm player)
        {
            switch(name)
            {
                case "Rocket":
                    {
                        Rocket rocket = new Rocket(25, Assets.RocketTexture);
                        rocket.Owner = player;
                        return rocket;
                    }
                default:
                    {
                        return null;
                    }
            }

        }
    }
}
