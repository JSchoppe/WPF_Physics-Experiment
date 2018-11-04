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

namespace PhysicsExperiment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer tickTimer = new DispatcherTimer();

        private DateTime lastFrame;


        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // DispatcherTimer
            // https://docs.microsoft.com/en-us/dotnet/api/system.windows.threading.dispatchertimer?redirectedfrom=MSDN&view=netframework-4.7.2

            // Set the tick timer to run a 50 hertz.
            tickTimer.Interval = TimeSpan.FromSeconds(0.02);

            // Call frameTick whenever this timer ticks.
            tickTimer.Tick += FrameTick;

            Collision.AddColliderFromRectangle(Floor);
            lastFrame = DateTime.Now;

            Player player1 = new Player(PlayerBox);

            tickTimer.Tick += player1.Update;

            tickTimer.Start();
        }


        private void FrameTick(object sender, EventArgs e)
        {
            Game.deltaTime = (DateTime.Now - lastFrame).TotalSeconds;
            lastFrame = DateTime.Now;
        }
    }
}
