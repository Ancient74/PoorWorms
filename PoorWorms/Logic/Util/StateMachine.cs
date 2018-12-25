using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorWorms.Logic.Util
{
    public enum GameStates
    {
        Deploy,
        Walk,
        Charge,
        Shoot,
        Jump,
        AfterShoot,
        WaitingAfterShoot,
        ChangingPlayer,
        GameOver,
        Restart,
        Aim,
        BeforeCharge
    }

    public static class StateMachine
    {
        public static GameStates GameState { get; set; }
    }
}
