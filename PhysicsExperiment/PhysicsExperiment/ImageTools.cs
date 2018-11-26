using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;

namespace PhysicsExperiment
{
    public static class ImageTools
    {
        /// <summary>Merges bitmaps together, using the indeces as draw order</summary>
        /// <param name="layers">The bitmaps to merge together</param>
        /// <returns>One bitmap with all layers merged, null if the operation cannot be carried out</returns>
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

            Bitmap returnMap = new Bitmap(rightCrop - leftCrop + 1, bottomCrop - topCrop + 1);

            for (int y = 0; y < returnMap.Height; y++)
            {
                for (int x = 0; x < returnMap.Width; x++)
                {
                    returnMap.SetPixel(x, y, toCrop.GetPixel(x + leftCrop, y + topCrop));
                }
            }

            return returnMap;
        }

        public static Bitmap AddMargin(Bitmap toResize)
        {
            // Is there any reason the passed bitmap could not be operated on?
            if (toResize == null || toResize.Width == 0 || toResize.Height == 0)
            {
                // Return null.
                return null;
            }

            Bitmap returnMap = new Bitmap(toResize.Width + 2, toResize.Height + 2);

            for (int y = 0; y < toResize.Height; y++)
            {
                for (int x = 0; x < toResize.Width; x++)
                {
                    returnMap.SetPixel(x + 1, y + 1, toResize.GetPixel(x, y));
                }
            }

            for (int x = 1; x < returnMap.Width - 1; x++)
            {
                returnMap.SetPixel(x, 0, Color.Transparent);
                returnMap.SetPixel(x, returnMap.Height - 1, Color.Transparent);
            }

            for (int y = 0; y < returnMap.Height; y++)
            {
                returnMap.SetPixel(0, y, Color.Transparent);
                returnMap.SetPixel(returnMap.Width - 1, y, Color.Transparent);
            }

            return returnMap;
        }


        // Saving a bitmap for image controls https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image
        // Thanks to Garret https://stackoverflow.com/users/2659716/gerret
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            // Open a new memorystream.
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

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

        public static Color HSVtoRGB(HSVColor color)
        {
            // Declare doubles to store color values from 0-1.
            double red = 0;
            double green = 0;
            double blue = 0;

            // Set the base RGB values based on the hue.
            switch (color.H / 60)
            {
                case 0:
                    red = 1;
                    green = color.H / 60.0;
                    blue = 0;
                    break;
                case 1:
                    red = 1 - ((color.H - 60) / 60.0);
                    green = 1;
                    blue = 0;
                    break;
                case 2:
                    red = 0;
                    green = 1;
                    blue = (color.H - 120) / 60.0;
                    break;
                case 3:
                    red = 0;
                    green = 1 - ((color.H - 180) / 60.0);
                    blue = 1;
                    break;
                case 4:
                    red = (color.H - 240) / 60.0;
                    green = 0;
                    blue = 1;
                    break;
                case 5:
                    red = 1;
                    green = 0;
                    blue = 1 - ((color.H - 300) / 60.0);
                    break;
            }

            // Desaturate each of the color values.
            red = 1 - (1 - red) * color.S;
            green = 1 - (1 - green) * color.S;
            blue = 1 - (1 - blue) * color.S;

            // Apply the value to each color channel.
            red *= color.V;
            green *= color.V;
            blue *= color.V;

            // Return the closest representation of the HSV color.
            return Color.FromArgb
            (
                color.A,
                (int)(red * 255),
                (int)(green * 255),
                (int)(blue * 255)
            );
        }


        // Converting to HSV https://cs.stackexchange.com/questions/64549/convert-hsv-to-rgb-colors
        // nbro https://cs.stackexchange.com/users/20691/nbro

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
            double min = 0;
            double max = 0;
            if (red < min){ min = red; }
            if (red > max){ max = red; }
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
                    hue = 60 * ((green - blue) / (max - min));
                }
                else if(green == max)
                {
                    hue = 60 * (2 + (blue - red) / (max - min));
                }
                else // Blue == max.
                {
                    hue = 60 * (4 + (red - green) / (max - min));
                }
            }

            // If the hue is less than 0, wrap it back around.
            if (hue < 0){ hue += 360; }

            // Return the HSV color.
            return new HSVColor(
                hue, sat, val, color.A
            );
        }
    }

    public class HSVColor
    {
        // Color components:
        public double H; // Hue [0, 360)
        public double S; // Saturation [0, 1]
        public double V; // Value [0, 1]
        public int A; // Alpha [0, 255]

        public HSVColor(double hue, double saturation, double value, double alpha)
        {
            H = (int)(hue % 360);
            
            if (saturation > 1)
            {
                S = 1;
            }
            else
            {
                S = (int)saturation;
            }

            if (value > 1)
            {
                V = 1;
            }
            else
            {
                V = (int)value;
            }

            if (alpha > 255)
            {
                A = 255;
            }
            else
            {
                A = (int)alpha;
            }
        }
    }
}