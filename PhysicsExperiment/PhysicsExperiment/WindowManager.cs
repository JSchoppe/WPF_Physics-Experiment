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


        public static void AddWindow()
        {
            if (once)
            {
                Start();

                Inventory window1 = new Inventory();
                window1.Show();
                window1.Left = - SystemParameters.ResizeFrameVerticalBorderWidth * 2;
                window1.Top = 0;
                window1.Width = clientX + BorderAddedX;
                window1.Height = clientY + BorderAddedY;

                MainWindow window2 = new MainWindow();
                window2.Show();
                window2.Left = clientX + BorderAddedX - SystemParameters.ResizeFrameVerticalBorderWidth * 6;
                window2.Top = 0;
                window2.Width = clientX + BorderAddedX;
                window2.Height = clientY + BorderAddedY;

                MainWindow window3 = new MainWindow();
                window3.Show();
                window3.Left = 2 * (clientX + BorderAddedX) - SystemParameters.ResizeFrameVerticalBorderWidth * 10;
                window3.Top = 0;
                window3.Width = clientX + BorderAddedX;
                window3.Height = clientY + BorderAddedY;

                MainWindow window4 = new MainWindow();
                window4.Show();
                window4.Left = -SystemParameters.ResizeFrameVerticalBorderWidth * 2;
                window4.Top = clientY + BorderAddedY - SystemParameters.ResizeFrameHorizontalBorderHeight * 2;
                window4.Width = clientX + BorderAddedX;
                window4.Height = clientY + BorderAddedY;

                //MainWindow window5 = new MainWindow();
                //window5.Show();
                //window5.Left = 0;
                //window5.Top = 0;
                //window5.Width = clientX + BorderAddedX;
                //window5.Height = clientY + BorderAddedY;

                //MainWindow window6 = new MainWindow();
                //window6.Show();
                //window6.Left = 2 * windowX - SystemParameters.ResizeFrameVerticalBorderWidth * 10;
                //window6.Top = windowY - SystemParameters.ResizeFrameHorizontalBorderHeight * 2;
                //window6.Width = windowX;
                //window6.Height = windowY;

                //MainWindow window7 = new MainWindow();
                //window7.Show();
                //window7.Left = -SystemParameters.ResizeFrameVerticalBorderWidth * 2;
                //window7.Top = 2 * windowY - SystemParameters.ResizeFrameHorizontalBorderHeight * 4;
                //window7.Width = windowX;
                //window7.Height = windowY;

                //MainWindow window8 = new MainWindow();
                //window8.Show();
                //window8.Left = windowX - SystemParameters.ResizeFrameVerticalBorderWidth * 6;
                //window8.Top = 2 * windowY - SystemParameters.ResizeFrameHorizontalBorderHeight * 4;
                //window8.Width = windowX;
                //window8.Height = windowY;

                //MainWindow window9 = new MainWindow();
                //window9.Show();
                //window9.Left = 2 * windowX - SystemParameters.ResizeFrameVerticalBorderWidth * 10;
                //window9.Top = 2 * windowY - SystemParameters.ResizeFrameHorizontalBorderHeight * 4;
                //window9.Width = windowX;
                //window9.Height = windowY;
            }
            once = false;
        }
    }
}