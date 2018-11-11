using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Threading;
using System.Windows;

// DispatcherTimer
// https://docs.microsoft.com/en-us/dotnet/api/system.windows.threading.dispatchertimer?redirectedfrom=MSDN&view=netframework-4.7.2

namespace PhysicsExperiment
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    public static class Game
    {
        // The current delta time of the last tick.
        public static double deltaTime;

        // It is currently assumed that gravity is down.
        public static readonly double gravity = 1200;

        public static DirectoryInfo resourcesPath;

        private static Player player1;

        public static DispatcherTimer tickTimer = new DispatcherTimer();

        private static DateTime lastFrame;

        public static void Start (Window startScreen)
        {
            // Get the directory that Game.cs is in.
            resourcesPath = new DirectoryInfo(".");

            // Cycle up through the files until the parent is the solution title.
            while (resourcesPath.Parent.Name != "PhysicsExperiment")
            {
                resourcesPath = resourcesPath.Parent;
            }

            // Get the resources folder.
            resourcesPath = new DirectoryInfo(resourcesPath.Parent.FullName + "/Resources");


            // Set the tick timer to run at 60 hertz.
            tickTimer.Interval = TimeSpan.FromSeconds(1 / 90.00);

            // Call OnTick whenever this timer ticks.
            tickTimer.Tick += OnTick;

            DesignParser.ParseAll();

            WindowManager.Start();
            WindowManager.CreateWindows(1,1);

            // Close the start screen window.
            startScreen.Close();

            Collision.SetCollidersFromNormalizedSet(World.maps[1, 1].staticColliders);

            player1 = new Player((Level)WindowManager.windowMatrix[4]);

            // Start the timer.
            tickTimer.Start();
        }

        private static void OnTick(object sender, EventArgs e)
        {
            // Calculate the deltaTime for this tick:
            deltaTime = (DateTime.Now - lastFrame).TotalSeconds;
            lastFrame = DateTime.Now;
            player1.Update();
        }


        public static string GetAssetPath(string AssetFileName)
        {
            return resourcesPath.FullName + "/" + AssetFileName;
        }
    }
}
