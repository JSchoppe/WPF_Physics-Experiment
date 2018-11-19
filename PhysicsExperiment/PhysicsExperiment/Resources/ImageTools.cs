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

            // Start the return bitmap with the base layer.
            Bitmap returnMap = layers[0];

            // For each layer to merge down(not including the first).
            for (int i = 1; i < layers.Length; i++)
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
            // Is there any reason the passed collection could not be operated on?
            if (toStroke == null)
            {
                // Return null.
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


        // https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image
        // Thanks to Garret https://stackoverflow.com/users/2659716/gerret

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
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
    }
}