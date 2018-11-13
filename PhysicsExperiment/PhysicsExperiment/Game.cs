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
    /// <summary>Contains the fundamental game rules and methods</summary>
    public static class Game
    {
        
        /// <summary>Time passed this tick</summary>
        public static double deltaTime { get; private set; }

        /// <summary>The current gravity magnitude</summary>
        public static double gravity = 1200;

        public static DirectoryInfo resourcesPath;

        public static DispatcherTimer tickTimer = new DispatcherTimer();

        private static Player player1;

        private static DateTime lastFrame;

        public static void Start (Window startScreen)
        {


            // Set the tick timer to run at 60 hertz.
            tickTimer.Interval = TimeSpan.FromSeconds(1 / 90.00);

            // Call OnTick whenever this timer ticks.
            tickTimer.Tick += OnTick;

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