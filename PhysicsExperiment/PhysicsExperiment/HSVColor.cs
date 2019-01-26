using System;
using System.Drawing;

namespace PhysicsExperiment
{
    /// <summary>Color composed of hue, saturation, and value components</summary>
    public class HSVColor
    {
        // Color components.
        public double H { get; private set; } // Hue [0, 360)
        public double S { get; private set; } // Saturation [0, 1]
        public double V { get; private set; } // Value [0, 1]
        public int A { get; private set; }    // Alpha [0, 255]

        // HSV constructor(see setters for validity of values).
        public HSVColor(double hue, double saturation, double value, int alpha)
        {
            SetHue(hue);
            SetSat(saturation);
            SetVal(value);
            SetAlpha(alpha);
        }

        // Converting to HSV https://cs.stackexchange.com/questions/64549/convert-hsv-to-rgb-colors
        // nbro https://cs.stackexchange.com/users/20691/nbro
        // More formulas https://en.wikipedia.org/wiki/HSL_and_HSV

        /// <summary>Converts an rgba color into hsva</summary>
        /// <param name="rgbaColor">The color to convert</param>
        /// <returns>HSVColor that is equivalent to the input</returns>
        public static HSVColor FromRGBA(Color rgbaColor)
        {
            // Convert the color values to 0-1 range.
            double red = rgbaColor.R / 255.0;
            double green = rgbaColor.G / 255.0;
            double blue = rgbaColor.B / 255.0;

            // Declare doubles to store output values.
            double hue = 0; // [0, 360)
            double sat = 0; // [0, 1]
            double val = 0; // [0, 1]

            // Determine the min and max RGB values.
            double min = red;
            double max = red;
            if (green < min) { min = green; }
            if (green > max) { max = green; }
            if (blue < min) { min = blue; }
            if (blue > max) { max = blue; }

            // Value will be the maximum color value.
            val = max;

            // If the max is 0, saturation is 0(default).
            if (max != 0)
            {
                sat = (max - min) / max;
            }

            // If there is a range between min and max, then there is a hue.
            // Else hue is undefined(hue value doesn't matter so we leave it at 0).
            if (max != min)
            {
                if (red == max)
                {
                    hue = 60 * (((green - blue) / (max - min)) % 6);
                }
                else if (green == max)
                {
                    hue = 60 * ((blue - red) / (max - min) + 2);
                }
                else // Blue == max.
                {
                    hue = 60 * ((red - green) / (max - min) + 4);
                }
            }

            // If the hue is less than 0, wrap it back around.
            if (hue < 0) { hue += 360; }

            // Return the HSV color.
            return new HSVColor(hue, sat, val, rgbaColor.A);
        }

        /// <summary>Converts this color into RGBA</summary>
        /// <returns>An RGBA Color</returns>
        public Color ToRGBA()
        {
            // Declare doubles to store color values from 0-1.
            double red = 0;
            double green = 0;
            double blue = 0;

            // Based on color conversion formulas:
            double chroma = V * S;
            double hueInterval = H / 60;
            double intermediate = chroma * (1 - Math.Abs((hueInterval % 2) - 1));
            double matchVal = V - chroma;

            // Is the hue defined? Else return black.
            if (V != 0)
            {
                if (hueInterval <= 1)
                {
                    red = chroma;
                    green = intermediate;
                    blue = 0;
                }
                else if (hueInterval <= 2)
                {
                    red = intermediate;
                    green = chroma;
                    blue = 0;
                }
                else if (hueInterval <= 3)
                {
                    red = 0;
                    green = chroma;
                    blue = intermediate;
                }
                else if (hueInterval <= 4)
                {
                    red = 0;
                    green = intermediate;
                    blue = chroma;
                }
                else if (hueInterval <= 5)
                {
                    red = intermediate;
                    green = 0;
                    blue = chroma;
                }
                else //hueInterval <= 6
                {
                    red = chroma;
                    green = 0;
                    blue = intermediate;
                }

                red += matchVal;
                green += matchVal;
                blue += matchVal;
            }

            // Return the closest representation of the HSV color.
            return Color.FromArgb
            (
                A,
                (int)(red * 255),
                (int)(green * 255),
                (int)(blue * 255)
            );
        }

        // Methods for setting the values that ensure valid values.
        public void SetHue(double hue)
        {
            // Ensure hue is in the interval 0-360
            H = hue % 360;
            if (H < 0) { H += 360; }
        }

        public void SetVal(double val)
        {
            // Clamp value between 0-1
            if (val < 0) { V = 0; }
            else if (val > 1) { V = 1; }
            else { V = val; }
        }

        public void SetSat(double sat)
        {
            // Clamp saturation between 0-1
            if (sat < 0) { S = 0; }
            else if (sat > 1) { S = 1; }
            else { S = sat; }
        }

        public void SetAlpha(int alpha)
        {
            // Clamp alpha between 0-255
            if (alpha < 0) { A = 0; }
            else if (alpha > 255) { A = 255; }
            else { A = alpha; }
        }
    }
}