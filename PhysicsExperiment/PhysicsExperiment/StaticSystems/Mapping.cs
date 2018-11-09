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
        // Determines the area that colliders will be expanded past the screen boundaries.
        // This prevents clipping out of bounds through walls.
        private static int padding = 10;


        // Bitmap class.
        // https://docs.microsoft.com/en-us/dotnet/api/system.drawing.bitmap?view=netframework-4.7.2

        /// <summary>Uses a bitmap to populate static box colliders to a region</summary>
        /// <param name="collisionMap">The black and white collision bitmap</param>
        /// <param name="outputPixelSize">The resolution of the output client area</param>
        public static BoxCollider[] ParseCollisionMap(Bitmap collisionMap)
        {
            // Generate an array to track which pixels have been accounted for.
            bool[,] filledPixels = new bool[collisionMap.Width, collisionMap.Height];

            // Create a list that will keep track of the generated colliders.
            List<BoxCollider> generatedColliders = new List<BoxCollider>();

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

                        // Generate the boxcollider to add based on the unitary edges.
                        BoxCollider toAdd = new BoxCollider(x, y, bottomRightX + 1, bottomRightY + 1);

                        // Does this collider start at the left edge of the screen?
                        if (x == 0)
                        {
                            // If so, add padding out to the left.
                            toAdd.leftEdge -= padding;
                        }

                        // Does this collider end at the right edge of the screen?
                        if (bottomRightX == collisionMap.Width - 1)
                        {
                            // If so, add padding out to the right.
                            toAdd.rightEdge += padding;
                        }

                        // Does this collider start at the top edge of the screen?
                        if (y == 0)
                        {
                            // If so, add padding out the top.
                            toAdd.topEdge -= padding;
                        }

                        // Does this collider end at the bottom edge of the screen?
                        if (bottomRightY == collisionMap.Height - 1)
                        {
                            // If so, add padding out the bottom.
                            toAdd.bottomEdge += padding;
                        }

                        // Add the collider to the return list.
                        generatedColliders.Add(toAdd);

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

            // Return the colliders as an array.
            return generatedColliders.ToArray();
        }
    }
}