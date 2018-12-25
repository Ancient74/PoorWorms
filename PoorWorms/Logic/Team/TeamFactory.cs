using Microsoft.Xna.Framework;
using PoorWorms.Logic.Entities;
using PoorWorms.Logic.Entities.Weapons;
using PoorWorms.Logic.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Team
{
    public class TeamFactory : IEnumerable<Worm>
    {
        public static int MaxTeams { get; } = 4;
        public static int MaxPlayersInTeam { get; } = 4;
        public static Color[] TeamColors = { Color.Blue, Color.Purple, Color.Cyan, Color.Yellow };
        public static string[] Names = { "Blue", "Purple", "Cyan", "Yellow" };
        public TeamFactory(int playersInTeam, int teams)
        {
            InitTeams(playersInTeam,teams);
            ActiveTeamIndex = 0;
            ActiveWorm = Teams[0].Worms[0];
            ActiveWorm.Active = true;
            ActiveWorm.CurrentWeapon = WeaponFactory.Create("Rocket", ActiveWorm);
        }

        public Team[] Teams { get; private set; }

        public Worm ActiveWorm
        {
            get
            {
                if (_activeWorm.Dead)
                    ChangePlayer();
                return _activeWorm;
            }
            set
            {
                _activeWorm = value;
            }
        }
        public int ActiveTeamIndex { get; private set; }

        private Worm _activeWorm;

        public void InitTeams(int playersInTeam,int teams)
        {
            if(teams <=0 || teams > MaxTeams)
                throw new ArgumentException("invalid teams count");
            Team[] res = new Team[teams];
            for (int i = 0; i < teams; i++)
            {
                res[i] = new Team(TeamColors[i],Names[i] ,playersInTeam,true);
            }
            Teams = res;
        }

        public Worm SelectTarget()
        {
            Random r = new Random();
            int index = 0;
            do
            {
                index = r.Next(0, Teams.Length);
            } while (index == ActiveTeamIndex || Teams[index].TeamHealth <= 0);
            Team team = Teams[index];
            do
            {
                index = r.Next(0, team.Worms.Length);
            } while (team.Worms[index].Health <= 0);
            return team.Worms[index];

        }

        public Worm ChangePlayer()
        {
            Worm preActive = _activeWorm;
            _activeWorm.Active = false;
            _activeWorm.CurrentWeapon.Dead = true;
            _activeWorm.CurrentWeapon = null;
            int currentTeamIndex = ActiveTeamIndex;
            do
            {
                ActiveTeamIndex = ++ActiveTeamIndex % Teams.Length;
                if (Teams[ActiveTeamIndex].TeamHealth > 0)
                {
                    break;
                }
            } while (currentTeamIndex != ActiveTeamIndex);
            if(currentTeamIndex == ActiveTeamIndex)
            {
                return preActive;//вернуть прошлого активного игрока, так как в других командах уже все умерли -> команда этого игрока выиграла
            }
            ActiveWorm = Teams[ActiveTeamIndex].ChangeWorm();
            ActiveWorm.Active = true;
            ActiveWorm.CurrentWeapon = WeaponFactory.Create("Rocket",ActiveWorm);
            Assets.Camera.CameraMovement.CameraLock = ActiveWorm;
            return ActiveWorm;
        }

        public IEnumerator<Worm> GetEnumerator()
        {
            for (int i = 0; i < Teams.Length; i++)
            {
                for (int j = 0; j < Teams[i].Worms.Length; j++)
                {
                    yield return Teams[i].Worms[j];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
