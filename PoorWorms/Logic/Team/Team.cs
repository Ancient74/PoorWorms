using Microsoft.Xna.Framework;
using PoorWorms.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Team
{
    public class Team
    {
        public Color TeamColor { get; }
        public Worm[] Worms { get; }
        public int MaxTeamHealth => Worms.Sum(x => x.MaxHealth);
        public int TeamHealth => Worms.Sum(x => x.Health);
        public bool Controllable { get; }

        public int ActiveWormIndex { get; private set; } = 0;
        public Worm ActiveWorm { get; private set; }

        public string Name { get; }

        public Team(Color color, string name, int players, bool controllable)
        {
            if (players <= 0 || players > TeamFactory.MaxPlayersInTeam)
                throw new ArgumentException("invalid player count");

            Name = name;
            Controllable = controllable;
            Worms = new Worm[players];
            for (int i = 0; i < players; i++)
            {
                Worms[i] = Worm.Create(color,Controllable);
            }
            ActiveWorm = Worms[0];
            TeamColor = color;
        }

        public Worm ChangeWorm()
        {
            do
            { 
                ActiveWormIndex = ++ActiveWormIndex % Worms.Length;
            }while(Worms[ActiveWormIndex].Health<=0);

            return ActiveWorm = Worms[ActiveWormIndex];
        }
    }
}
