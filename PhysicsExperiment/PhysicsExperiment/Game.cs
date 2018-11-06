using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PhysicsExperiment
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    public static class Game
    {
        // The current delta time of the last tick.
        public static double deltaTime = 0.02;

        // It is currently assumed that gravity is down.
        public static readonly double gravity = 1200;

        public static DirectoryInfo resourcesPath;

        public static void Start ()
        {
            // Get the directory that Game.cs is in.
            resourcesPath = new DirectoryInfo(".");

            // Cycle up through the files until the parent is the solution title.
            while (resourcesPath.Parent.Name != "PhysicsExperiment")
            {
                resourcesPath = resourcesPath.Parent;
            }

            resourcesPath = new DirectoryInfo(resourcesPath.Parent.FullName + "/Resources");
        }

        public static string GetAssetPath(string AssetFileName)
        {
            return resourcesPath.FullName + "/" + AssetFileName;
        }
    }
}
