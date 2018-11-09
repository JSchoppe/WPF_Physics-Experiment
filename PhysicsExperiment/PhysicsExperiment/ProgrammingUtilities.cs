using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhysicsExperiment
{
    public static class ProgrammingUtilities
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

        public static Vector PolarizeVector(Vector toPolarize)
        {
            if (Math.Abs(toPolarize.X) > Math.Abs(toPolarize.Y))
            {
                if (toPolarize.X > 0)
                {
                    return new Vector(toPolarize.X, 0);
                }
                else
                {
                    return new Vector(-toPolarize.X, 0);
                }
            }
            else
            {
                if (toPolarize.Y > 0)
                {
                    return new Vector(0, toPolarize.Y);
                }
                else
                {
                    return new Vector(0, -toPolarize.Y);
                }
            }
        }

        public static Direction PolarizeVectorDirection(Vector toPolarize)
        {
            if (Math.Abs(toPolarize.X) > Math.Abs(toPolarize.Y))
            {
                if (toPolarize.X > 0)
                {
                    return Direction.Right;
                }
                else
                {
                    return Direction.Left;
                }
            }
            else
            {
                if (toPolarize.Y > 0)
                {
                    return Direction.Up;
                }
                else
                {
                    return Direction.Down;
                }
            }
        }
    }
}
