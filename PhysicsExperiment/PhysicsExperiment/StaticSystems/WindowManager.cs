using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PhysicsExperiment
{
    // System Parameters Class.
    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.systemparameters?view=netframework-4.7.2

    public static class WindowManager
    {

        public static List<PushOperation> currentPushes = new List<PushOperation>();

        public static int clientX;
        public static int clientY;

        public static double BorderAddedY = 0;
        public static double BorderAddedX = 0;

        public static Window[] windowMatrix = new Window[9];

        public static double UnitarySpaceToWindowSpace(double toConvert)
        {
            return toConvert * (clientY / 12.0);
        }

        public static void Start()
        {
            // Based on the monitor resolution and window border size,
            // calculate the window sizing.

            // First, retrieve the screen resolution down to the nearest factor of 3 pixels.
            int screenWidth = (int)(SystemParameters.WorkArea.Width - SystemParameters.WorkArea.Width % 3);
            int screenHeight = (int)(SystemParameters.WorkArea.Height - SystemParameters.WorkArea.Height % 3);

            // Calculate the maximum client areas.
            int maxClientX = (int)(screenWidth - 3 * BorderAddedX) / 3;
            int maxClientY = (int)(screenHeight - 3 * BorderAddedY) / 3;

            // Which client maximum is more restrictive? (Assuming that there is a x = 2y relationship).
            if (2 * maxClientY > maxClientX)
            {
                // The maximum X is more restrictive.
                clientX = maxClientX;
                clientY = maxClientX / 2;
            }
            else
            {
                // The maximum Y is more restrictive.
                clientY = maxClientY;
                clientX = maxClientY * 2;
            }
        }

        public static void CloseAll()
        {
            foreach (Window window in windowMatrix)
            {
                if (window != null)
                {
                    window.Close();
                }
            }
        }

        private static bool once = true;

        public static void PushAll(Direction toPush)
        {
            if (currentPushes.Count > 0)
            {
                return;
            }
            switch (toPush)
            {
                case Direction.Left:
                    foreach (Window window in windowMatrix)
                    {
                        currentPushes.Add(new PushOperation
                        (
                            window,
                            (int)(- clientX - BorderAddedX), 0,
                            0.2
                        ));
                    }
                    break;
                case Direction.Right:
                    foreach (Window window in windowMatrix)
                    {
                        currentPushes.Add(new PushOperation
                        (
                            window,
                            (int)(clientX + BorderAddedX), 0,
                            0.2
                        ));
                    }
                    break;
                case Direction.Up:
                    foreach (Window window in windowMatrix)
                    {
                        currentPushes.Add(new PushOperation
                        (
                            window,
                            0, (int)(- clientY - BorderAddedY),
                            0.2
                        ));
                    }
                    break;
                case Direction.Down:
                    foreach (Window window in windowMatrix)
                    {
                        currentPushes.Add(new PushOperation
                        (
                            window,
                            0, (int)(clientY + BorderAddedY),
                            0.2
                        ));
                    }
                    break;
            }
        }

        public static void CreateWindows(byte centerX, byte centerY)
        {
            if (once)
            {
                Start();
                for (int i = 0; i < windowMatrix.Length; i++)
                {
                    if (i == 0)
                    {
                        windowMatrix[i] = new Inventory();
                    }
                    else
                    {
                        Level level = new Level();

                        // Calculate the world position based on the windows position.
                        int coordX = centerX + ((i % 3) - 1);
                        int coordY = centerY + ((i / 3) - 1);

                        // Is this world position within the defined world?
                        if (CommonUtilities.IndexInTwoDimensionalArray(coordX, coordY, World.maps))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = World.maps[coordX, coordY].aestheticLayer;
                            bitmap.EndInit();
                            level.AestheticLayer.Source = bitmap;
                        }
                        else
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(Game.resourcesPath + "/AestheticMaps/UndiscoveredScreen.png");
                            bitmap.EndInit();
                            level.AestheticLayer.Source = bitmap;
                        }

                        windowMatrix[i] = level;
                    }
                    windowMatrix[i].Width = clientX + BorderAddedX;
                    windowMatrix[i].Height = clientY + BorderAddedY;

                    windowMatrix[i].Left = (i % 3) * (clientX + BorderAddedX);
                    windowMatrix[i].Top = (i / 3) * (clientY + BorderAddedY);

                    windowMatrix[i].Show();
                }
            }
            once = false;
        }

        public class PushOperation
        {
            DateTime pushStart;
            Window pushedWindow;

            double initialX;
            double initialY;

            double pushX;
            double pushY;

            double pushDuration;

            public PushOperation(Window toPush, int pixelsX, int pixelsY, double timeToPush)
            {
                pushStart = DateTime.Now;
                pushedWindow = toPush;
                pushDuration = timeToPush;

                initialX = toPush.Left;
                initialY = toPush.Top;

                pushX = pixelsX;
                pushY = pixelsY;

                Game.tickTimer.Tick += Update;
            }

            public void Update(object sender, EventArgs e)
            {
                double timeElapsed = (DateTime.Now - pushStart).TotalSeconds;
                if (timeElapsed > pushDuration)
                {
                    pushedWindow.Left = initialX + pushX;
                    pushedWindow.Top = initialY + pushY;

                    currentPushes.Remove(this);

                    Game.tickTimer.Tick -= Update;
                }
                else
                {
                    pushedWindow.Left = initialX + (timeElapsed / pushDuration) * pushX;
                    pushedWindow.Top = initialY + (timeElapsed / pushDuration) * pushY;
                }
            }
        }
    }
}