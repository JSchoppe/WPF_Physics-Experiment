using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsExperiment
{
    public static class Utilities
    {
        public static int IndexOfSmallest(double[] numbers)
        {
            int smallestIndex = 0;
            double smallestValue = numbers[0];

            for (int i = 1; i < numbers.Length; i++)
            {
                if (numbers[i] < smallestValue)
                {
                    smallestIndex = i;
                    smallestValue = numbers[i];
                }
            }

            return smallestIndex;
        }
    }
}
