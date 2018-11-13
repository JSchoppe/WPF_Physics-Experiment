using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhysicsExperiment
{
    public static class UserInput
    {
        public static int primaryX { get; private set; }

        public static int primaryY { get; private set; }

        public static Dictionary<KeyboardBinding, State> states = new Dictionary<KeyboardBinding, State>();

        private static Dictionary<KeyboardBinding, Key> keyToCheck = new Dictionary<KeyboardBinding, Key>()
        {
            { KeyboardBinding.PrimaryXNegative, Key.A },
            { KeyboardBinding.PrimaryXPositive, Key.D },
            { KeyboardBinding.PrimaryYNegative, Key.S },
            { KeyboardBinding.PrimaryYPositive, Key.W },
            { KeyboardBinding.SecondaryXNegative, Key.Left },
            { KeyboardBinding.SecondaryXPositive, Key.Right },
            { KeyboardBinding.SecondaryYNegative, Key.Down },
            { KeyboardBinding.SecondaryYPositive, Key.Up }
        };

        public static void Update()
        {
            foreach(KeyboardBinding binding in Enum.GetValues(typeof(KeyboardBinding)))
            {
                
            }
        }

        public static void Mute(KeyboardBinding toMute)
        {

        }

        public static void UnMute(KeyboardBinding toUnMute)
        {

        }

        public static void ReBind(KeyboardBinding toReBind, ConsoleKey toBindTo)
        {

        }
    }
}
