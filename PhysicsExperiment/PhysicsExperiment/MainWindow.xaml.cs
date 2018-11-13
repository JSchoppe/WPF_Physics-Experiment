using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Drawing;

using static System.Windows.SystemParameters;
using System.IO;

namespace PhysicsExperiment
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        // Construction of the initial window.
        public MainWindow()
        {
            InitializeComponent();
        }

        // This will run immediately after the window controls have initialized.
        private void StartupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowManager.BorderAddedX = Width - MainGrid.ActualWidth;
            WindowManager.BorderAddedY = Height - MainGrid.ActualHeight;

            // Retrieve two-thirds of the work area resolution
            int screenWidth = (int)(2.0/3.0 * WorkArea.Width);
            int screenHeight = (int)(2.0 / 3.0 * WorkArea.Height);

            // Round down to even(so window can be centered).
            screenWidth -= screenWidth % 2;
            screenHeight -= screenHeight % 2;

            // Calculate the maximum client areas.
            int maxClientX = (int)(screenWidth - WindowManager.BorderAddedX);
            int maxClientY = (int)(screenHeight - WindowManager.BorderAddedY);

            // Which client maximum is more restrictive? (Assuming that there is a x = 2y relationship).
            if (2 * maxClientY > maxClientX)
            {
                // The maximum X is more restrictive.
                StartupWindow.Width = maxClientX + WindowManager.BorderAddedX;
                StartupWindow.Height = (maxClientX / 2) + WindowManager.BorderAddedY;
            }
            else
            {
                // The maximum Y is more restrictive.
                StartupWindow.Width = (maxClientY * 2) + WindowManager.BorderAddedX;
                StartupWindow.Height = maxClientY + WindowManager.BorderAddedY;
            }

            // Get the directory that Game.cs is in.
            Game.resourcesPath = new DirectoryInfo(".");

            // Cycle up through the files until the parent is the solution title.
            while (Game.resourcesPath.Parent.Name != "PhysicsExperiment")
            {
                Game.resourcesPath = Game.resourcesPath.Parent;
            }

            // Get the resources folder.
            Game.resourcesPath = new DirectoryInfo(Game.resourcesPath.Parent.FullName + "/Resources");

            // Parse everything in the design folder into the game.
            DesignParser.ParseAll();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            // Start the game.
            Game.Start(this);
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}