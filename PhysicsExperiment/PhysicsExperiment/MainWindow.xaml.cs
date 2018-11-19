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
            double BorderAddedX = Width - MainGrid.ActualWidth;
            double BorderAddedY = Height - MainGrid.ActualHeight;

            // Retrieve two-thirds of the work area resolution
            int screenWidth = (int)(2.0/3.0 * WorkArea.Width);
            int screenHeight = (int)(2.0 / 3.0 * WorkArea.Height);

            // Round down to even(so window can be centered).
            screenWidth -= screenWidth % 2;
            screenHeight -= screenHeight % 2;

            // Calculate the maximum client areas.
            int maxClientX = (int)(screenWidth - BorderAddedX);
            int maxClientY = (int)(screenHeight - BorderAddedY);

            // Which client maximum is more restrictive? (Assuming that there is a x = 2y relationship).
            if (2 * maxClientY > maxClientX)
            {
                // The maximum X is more restrictive.
                StartupWindow.Width = maxClientX + BorderAddedX;
                StartupWindow.Height = (maxClientX / 2) + BorderAddedY;
            }
            else
            {
                // The maximum Y is more restrictive.
                StartupWindow.Width = (maxClientY * 2) + BorderAddedX;
                StartupWindow.Height = maxClientY + BorderAddedY;
            }

            // Get the directory that Game.cs is in.
            DirectoryInfo resourcesPath = new DirectoryInfo(".");

            // Cycle up through the files until the parent is the solution title.
            while (resourcesPath.Parent.Name != "PhysicsExperiment")
            {
                resourcesPath = resourcesPath.Parent;
            }

            // Get the resources folder.
            resourcesPath = new DirectoryInfo(resourcesPath.Parent.FullName + "/Resources");
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}