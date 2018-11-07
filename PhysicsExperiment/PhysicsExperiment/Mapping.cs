using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;

namespace PhysicsExperiment
{
    public static class Mapping
    {
        // Bitmap class.
        // https://docs.microsoft.com/en-us/dotnet/api/system.drawing.bitmap?view=netframework-4.7.2

        /// <summary>Uses a bitmap to populate static box colliders to a region</summary>
        /// <param name="collisionMap">The black and white collision bitmap</param>
        /// <param name="outputPixelSize">The resolution of the output client area</param>
        public static void ParseCollisionMap(Bitmap collisionMap, Vector outputPixelSize)
        {
            // Generate an array to track which pixels have been accounted for.
            bool[,] filledPixels = new bool[collisionMap.Width, collisionMap.Height];

            // Cycle through all the pixels in the image.
            for (int y = 0; y < collisionMap.Height; y++)
            {
                for (int x = 0; x < collisionMap.Width; x++)
                {
                    // Does collision need to be here, and hasn't been covered by a previous box?
                    // Only the red value is checked to determine if the pixels are black or white.
                    if (collisionMap.GetPixel(x, y).R == 0 && !filledPixels[x, y])
                    {
                        // X and Y mark the top left corner of the rectangle collider.

                        // Declare variables to represent the lower right as the rectangle
                        // attempts to resize and cover as much space as possible.
                        int bottomRightX = x;
                        int bottomRightY = y;

                        // Attempt to resize to the right first.
                        while (true)
                        {
                            // Is the next pixel to the right white, or outside of the image?
                            if (bottomRightX + 1 >= collisionMap.Width || collisionMap.GetPixel(bottomRightX + 1, y).R != 0)
                            {
                                // If so, stop resizing.
                                break;
                            }
                            bottomRightX++;
                        }

                        // Attempt to resize down.
                        bool resize = true;
                        while (resize)
                        {
                            // Ran out of rows on the image?
                            if (bottomRightY + 1 >= collisionMap.Height)
                            {
                                // If so, stop resizing.
                                resize = false;
                                break;
                            }

                            // Cycle through the next row below.
                            for (int column = x; column <= bottomRightX; column++)
                            {
                                // Is a pixel in this row white?
                                if (collisionMap.GetPixel(column, bottomRightY + 1).R != 0)
                                {
                                    // If so, stop resizing.
                                    resize = false;
                                    break;
                                }
                            }
                            if (resize)
                            {
                                bottomRightY++;
                            }
                        }

                        // Add this collider to the scene, scaling it to fit the client resolution.
                        Collision.sceneColliders.Add(new BoxCollider
                        (
                            // Calculate the left edge of the collider.
                            outputPixelSize.X * (x / (double)collisionMap.Width),

                            // Calculate the top edge of the collider.
                            outputPixelSize.Y * (y / (double)collisionMap.Height),

                            // Calculate the right edge of the collider.
                            outputPixelSize.X * ((bottomRightX + 1) / (double)collisionMap.Width),

                            // Calculate the bottom edge of the collider.
                            outputPixelSize.Y * ((bottomRightY + 1) / (double)collisionMap.Height)
                        ));

                        // Now that the collider has been added, mark these tiles as complete.
                        // This ensures that they will be skipped over.
                        for (int tileY = y; tileY <= bottomRightY; tileY++)
                        {
                            for (int tileX = x; tileX <= bottomRightX; tileX++)
                            {
                                filledPixels[tileX, tileY] = true;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>Represents one window of a world map</summary>
    public class MapScreen
    {
        Bitmap collisionMap;
    }

}