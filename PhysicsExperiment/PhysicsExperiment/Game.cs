using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsExperiment
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    public static class Game
    {
        // 
        public static double deltaTime = 0.02;

        // It is currently assumed that gravity is down.
        public static readonly double gravity = 1200;
    }
}
