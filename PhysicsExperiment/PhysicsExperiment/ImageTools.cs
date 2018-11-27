using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

// TODO: experiment with custom exceptions.
// User-defined exceptions: https://docs.microsoft.com/en-us/dotnet/standard/exceptions/how-to-create-user-defined-exceptions

namespace PhysicsExperiment
{
    /// <summary>Contains methods for manipulating bitmaps</summary>
    public class ImageTools
    {
        /// <summary>Merges bitmaps together, using the indeces as draw order</summary>
        /// <param name="layers">The bitmaps to merge together</param>
        /// <returns>One bitmap with all layers merged, null if the passed parameters are invalid</returns>
        public static Bitmap MergeDown(Bitmap[] layers)
        {
            // Is there any reason the passed collection could not be operated on?
            if (layers == null || layers.Length == 0 || layers[0] == null)
            {
                // Return null.
                return null;
            }

            // Declare ints for the bitmap size.
            int rows = layers[0].Height;
            int columns = layers[0].Width;

            // Is every bitmap identical in size?
            foreach (Bitmap bmp in layers)
            {
                if (bmp == null || bmp.Height != rows || bmp.Width != columns)
                {
                    // If not, return null.
                    return null;
                }
            }

            // Generate a new bitmap to return.
            // NOTE: Originally this started as = layers[0], but this is reference type and thus overwrites it.
            // Bitmap.Clone() would supposedly work, but I've been told to avoid IClonable.
            Bitmap returnMap = new Bitmap(layers[0].Width, layers[0].Height);

            // For each layer to merge down(not including the first).
            for (int i = 0; i < layers.Length; i++)
            {
                // For each row in this map.
                for (int y = 0; y < layers[i].Height; y++)
                {
                    // For each pixel in this row.
                    for (int x = 0; x < layers[i].Width; x++)
                    {
                        Color currentPixel = layers[i].GetPixel(x, y);

                        // Is the current pixel not transparent?
                        if (currentPixel.A != 0)
                        {
                            // If so, overwrite this pixel in the return map.
                            returnMap.SetPixel(x, y, currentPixel);
                        }
                    }
                }
            }

            // Return the merged bitmap.
            return returnMap;
        }

        /// <summary>Applys a one pixel stroke of color around transparent image regions</summary>
        /// <param name="toStroke">The bitmap that will be stroked</param>
        /// <param name="strokeColor">The pixel color to use for the stroke</param>
        /// <returns>A bitmap with one pixel stroke, null if the passed parameters are invalid</returns>
        public static Bitmap Stroke(Bitmap toStroke, Color strokeColor)
        {
            // Is there any reason the passed bitmap could not be operated on?
            if (toStroke == null)
            {
                return null;
            }

            // Initialize a list to track the pixels to fill with the stroke.
            List<Point> pointsToFill = new List<Point>();

            // For each row in this map.
            for (int y = 0; y < toStroke.Height; y++)
            {
                // For each pixel in this row.
                for (int x = 0; x < toStroke.Width; x++)
                {
                    Color currentPixel = toStroke.GetPixel(x, y);

                    // Is the current pixel transparent?
                    if (currentPixel.A == 0)
                    {
                        // Check the surrounding pixels.
                        foreach(Color color in GetSurroundingPixels(toStroke, new Point(x, y)))
                        {
                            // Does a surrounding pixel contain data?
                            if (color.A != 0)
                            {
                                // Add this pixel to the fill list.
                                pointsToFill.Add(new Point(x, y));
                                break;
                            }
                        }
                    }
                }
            }

            // Declare a bitmap to return.
            Bitmap returnMap = toStroke;

            // Fill in the stroke color.
            foreach(Point point in pointsToFill)
            {
                returnMap.SetPixel(point.X, point.Y, strokeColor);
            }

            // Return the stroked map.
            return returnMap;
        }

        /// <summary>Crops a bitmap, removing transparent space from each edge</summary>
        /// <param name="toCrop">The bitmap to be cropped</param>
        /// <returns>A cropped bitmap, null if the passed parameters are invalid</returns>
        public static Bitmap CropByAlpha(Bitmap toCrop)
        {
            // Is there any reason the passed bitmap could not be operated on?
            if (toCrop == null || toCrop.Width == 0 || toCrop.Height == 0)
            {
                // Return null.
                return null;
            }

            // Declare variables to track the cropping coordinates.
            int topCrop = 0;
            int bottomCrop = toCrop.Height - 1;
            int leftCrop = 0;
            int rightCrop = toCrop.Width - 1;

            // Calculate the crop coordinates based on alpha values.
            while(IsRowEmpty(toCrop, topCrop)) { topCrop++; }
            while(IsRowEmpty(toCrop, bottomCrop)) { bottomCrop--; }
            while(IsColumnEmpty(toCrop, leftCrop)) { leftCrop++; }
            while(IsColumnEmpty(toCrop, rightCrop)) { rightCrop--; }

            // Create a new bitmap with the size designated by the crop.
            Bitmap returnMap = new Bitmap(rightCrop - leftCrop + 1, bottomCrop - topCrop + 1);

            // Fill the return bitmap with contents of the original.
            for (int y = 0; y < returnMap.Height; y++)
            {
                for (int x = 0; x < returnMap.Width; x++)
                {
                    returnMap.SetPixel(x, y, toCrop.GetPixel(x + leftCrop, y + topCrop));
                }
            }

            // Return the cropped map.
            return returnMap;
        }

        /// <summary>Adds a one pixel transparent margin to a bitmap</summary>
        /// <param name="toResize">The bitmap to add the margin to</param>
        /// <returns>Bitmap with margin, null if the passed parameters are invalid</returns>
        public static Bitmap AddMargin(Bitmap toResize)
        {
            // Is there any reason the passed bitmap could not be operated on?
            if (toResize == null || toResize.Width == 0 || toResize.Height == 0)
            {
                // Return null.
                return null;
            }

            // Creater a wider bitmap to be returned.
            Bitmap returnMap = new Bitmap(toResize.Width + 2, toResize.Height + 2);

            // For each row:
            for (int y = 0; y < toResize.Height; y++)
            {
                // For each pixel in the row:
                for (int x = 0; x < toResize.Width; x++)
                {
                    // Set the pixel, given the margin offset.
                    returnMap.SetPixel(x + 1, y + 1, toResize.GetPixel(x, y));
                }
            }

            // Set the top and bottom rows to transparent.
            for (int x = 1; x < returnMap.Width - 1; x++)
            {
                returnMap.SetPixel(x, 0, Color.Transparent);
                returnMap.SetPixel(x, returnMap.Height - 1, Color.Transparent);
            }

            // Set the left and right columns to transparent.
            for (int y = 0; y < returnMap.Height; y++)
            {
                returnMap.SetPixel(0, y, Color.Transparent);
                returnMap.SetPixel(returnMap.Width - 1, y, Color.Transparent);
            }

            // Return the expanded map.
            return returnMap;
        }

        /// <summary>Adjusts the HSV values of a bitmap</summary>
        /// <param name="toAdjust">The bitmap to adjust</param>
        /// <param name="deltaHue">Change in hue(in degrees)</param>
        /// <param name="deltaSat">Change in saturation(from -1 to 1)</param>
        /// <param name="deltaVal">Change in value(from -1 to 1)</param>
        /// <returns>Returns the adjusted bitmap, null if the passed parameters are invalid</returns>
        public static Bitmap AdjustHSV(Bitmap toAdjust, double deltaHue, double deltaSat, double deltaVal)
        {
            // Is there any reason the passed bitmap could not be operated on?
            if (toAdjust == null || toAdjust.Width == 0 || toAdjust.Height == 0)
            {
                // Return null.
                return null;
            }

            // Create a bitmap to operate on.
            Bitmap returnMap = new Bitmap(toAdjust.Width, toAdjust.Height);

            // For each row:
            for (int y = 0; y < returnMap.Height; y++)
            {
                // For each pixel in the row:
                for (int x = 0; x < returnMap.Width; x++)
                {
                    // Get the color to be adjusted.
                    Color adjustColor = toAdjust.GetPixel(x, y);

                    // Get the HSV version of this pixel's color.
                    HSVColor hsvColor = RGBtoHSV(adjustColor);

                    // Adjust the HSV values based on input.
                    // The setters will ensure these values remain in range.
                    hsvColor.SetHue(hsvColor.H + deltaHue);
                    hsvColor.SetSat(hsvColor.S + deltaSat);
                    hsvColor.SetVal(hsvColor.V + deltaVal);

                    // Convert back to RGB and set the pixel.
                    returnMap.SetPixel(x, y, HSVtoRGB(hsvColor));
                }
            }

            // Return the adjusted map.
            return returnMap;
        }


        // Saving a bitmap for image controls https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image
        // Thanks to Garret https://stackoverflow.com/users/2659716/gerret
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            // Open a new memorystream.
            using (MemoryStream memory = new MemoryStream())
            {
                // Save this bitmap into memory with PNG format.
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);

                // Set the stream position to 0.
                memory.Position = 0;

                // Declare a bitmap image to return.
                BitmapImage bitmapimage = new BitmapImage();

                // Initialize the BitmapImage.
                bitmapimage.BeginInit();
                // The image pulls from the memorystream.
                bitmapimage.StreamSource = memory;
                // Set the memory caching setting.
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                // Return the memory backed BitmapImage.
                return bitmapimage;
            }
        }

        // Checks to see if a row in a bitmap is empty.
        private static bool IsRowEmpty(Bitmap map, int row)
        {
            // Is this row within the passed bitmap?
            if (row < 0 || row > map.Height - 1)
            {
                // If not, return true by default.
                return true;
            }

            // For each pixel in the row:
            for (int x = 0; x < map.Width; x++)
            {
                if (map.GetPixel(x, row).A != 0)
                {
                    // Return false if the pixel isn't transparent.
                    return false;
                }
            }

            // Return true if all pixels are transparent.
            return true;
        }

        // Checks to see if a column in a bitmap is empty.
        private static bool IsColumnEmpty(Bitmap map, int column)
        {
            // Is this column within the passed bitmap?
            if (column < 0 || column > map.Width - 1)
            {
                // If not, return true by default.
                return true;
            }

            // For each pixel in the column:
            for (int y = 0; y < map.Height; y++)
            {
                if (map.GetPixel(column, y).A != 0)
                {
                    // Return false if the pixel isn't transparent.
                    return false;
                }
            }

            // Return true if all pixels are transparent.
            return true;
        }

        // Returns a list of colors representing the surrounding pixels.
        private static List<Color> GetSurroundingPixels(Bitmap map, Point coordinate)
        {
            // Declare a list to be returned.
            List<Color> returnList = new List<Color>();

            // Check the surrounding pixels.
            // Are there pixels above?
            if (coordinate.Y != 0)
            {
                // Is there a pixel up left?
                if (coordinate.X > 0)
                {
                    returnList.Add(map.GetPixel(coordinate.X - 1, coordinate.Y - 1));
                }

                // Return the pixel directly above.
                returnList.Add(map.GetPixel(coordinate.X, coordinate.Y - 1));

                // Is there a pixel up right?
                if (coordinate.X < map.Width - 1)
                {
                    returnList.Add(map.GetPixel(coordinate.X + 1, coordinate.Y - 1));
                }
            }

            // Are there pixels below?
            if (coordinate.Y < map.Height - 1)
            {
                // Is there a pixel down left?
                if (coordinate.X > 0)
                {
                    returnList.Add(map.GetPixel(coordinate.X - 1, coordinate.Y + 1));
                }

                // Return the pixel directly below.
                returnList.Add(map.GetPixel(coordinate.X, coordinate.Y + 1));

                // Is there a pixel down right?
                if (coordinate.X < map.Width - 1)
                {
                    returnList.Add(map.GetPixel(coordinate.X + 1, coordinate.Y + 1));
                }
            }

            // Is there a pixel left?
            if (coordinate.X > 0)
            {
                returnList.Add(map.GetPixel(coordinate.X - 1, coordinate.Y));
            }

            // Is there a pixel right?
            if (coordinate.X < map.Width - 1)
            {
                returnList.Add(map.GetPixel(coordinate.X + 1, coordinate.Y));
            }

            // Return the list of surrounding pixels.
            return returnList;
        }

        // Converts from HSV to RGB color type.
        public static Color HSVtoRGB(HSVColor color)
        {
            // Declare doubles to store color values from 0-1.
            double red = 0;
            double green = 0;
            double blue = 0;

            // Based on color conversion formulas:
            double chroma = color.V * color.S;
            double hueInterval = color.H / 60;
            double intermediate = chroma * (1 - Math.Abs((hueInterval % 2) - 1));
            double matchVal = color.V - chroma;

            // Is the hue defined? Else return black.
            if (color.V != 0)
            {
                if (hueInterval <= 1)
                {
                    red = chroma;
                    green = intermediate;
                    blue = 0;
                }
                if (hueInterval <= 2)
                {
                    red = intermediate;
                    green = chroma;
                    blue = 0;
                }
                if (hueInterval <= 3)
                {
                    red = 0;
                    green = chroma;
                    blue = intermediate;
                }
                if (hueInterval <= 4)
                {
                    red = 0;
                    green = intermediate;
                    blue = chroma;
                }
                if (hueInterval <= 5)
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
                color.A,
                (int)(red * 255),
                (int)(green * 255),
                (int)(blue * 255)
            );
        }

        // Converts from RGB to HSV color type.
        public static HSVColor RGBtoHSV(Color color)
        {
            // Convert the color values to 0-1 range.
            double red = color.R / 255.0;
            double green = color.G / 255.0;
            double blue = color.B / 255.0;

            // Declare doubles to store output values.
            double hue = 0; // [0, 360)
            double sat = 0; // [0, 1]
            double val = 0; // [0, 1]

            // Determine the min and max RGB values.
            double min = red;
            double max = red;
            if (green < min){ min = green; }
            if (green > max){ max = green; }
            if (blue < min){ min = blue; }
            if (blue > max){ max = blue; }

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
                else if(green == max)
                {
                    hue = 60 * ((blue - red) / (max - min) + 2);
                }
                else // Blue == max.
                {
                    hue = 60 * ((red - green) / (max - min) + 4);
                }
            }

            // If the hue is less than 0, wrap it back around.
            if (hue < 0){ hue += 360; }

            // Return the HSV color.
            return new HSVColor(
                hue, sat, val, color.A
            );
        }

        // Converting to HSV https://cs.stackexchange.com/questions/64549/convert-hsv-to-rgb-colors
        // nbro https://cs.stackexchange.com/users/20691/nbro
        // More formulas https://en.wikipedia.org/wiki/HSL_and_HSV
    }

    // Represents an HSVA color(Alpha channel simply to mirror RGBA).
    public class HSVColor
    {
        // Color components.
        public double H { get; private set; } // Hue [0, 360)
        public double S { get; private set; } // Saturation [0, 1]
        public double V { get; private set; } // Value [0, 1]
        public int A { get; private set; }    // Alpha [0, 255]

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

        // HSV constructor(see setters for validity of values).
        public HSVColor(double hue, double saturation, double value, int alpha)
        {
            SetHue(hue);
            SetSat(saturation);
            SetVal(value);
            SetAlpha(alpha);
        }
    }
}