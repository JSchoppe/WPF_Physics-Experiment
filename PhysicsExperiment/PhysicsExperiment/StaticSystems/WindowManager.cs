using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhysicsExperiment
{
    // System Parameters Class.
    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.systemparameters?view=netframework-4.7.2

    public static class WindowManager
    {

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

        private static bool once = true;

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


        public static void AddWindow()
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
                        windowMatrix[i] = new Windows.Level();
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
    }
}