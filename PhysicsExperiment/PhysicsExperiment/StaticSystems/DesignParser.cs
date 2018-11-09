using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysicsExperiment.DataStructures;
using PhysicsExperiment.Design;

namespace PhysicsExperiment
{
    /// <summary>Contains all methods for parsing the contents of the Design folder</summary>
    public static class DesignParser
    {
        /// <summary>Parses all resources into the game</summary>
        public static void ParseAll()
        {
            ParseMaps();
        }

        private static void ParseMaps()
        {
            WorldMap currentWorld = new WorldMap();

            // Declare vars to track the bounding scale of the world.
            byte mapScaleX = 0;
            byte mapScaleY = 0;

            // Determine the scale of the world by searching for the highest coordinates.
            foreach(MapScreen map in currentWorld.maps)
            {
                if (map.coordinateX > mapScaleX)
                {
                    mapScaleX = map.coordinateX;
                }

                if (map.coordinateY > mapScaleY)
                {
                    mapScaleY = map.coordinateY;
                }
            }

            // Use the maxima values to declare the world maps array.
            World.maps = new MapScreen[mapScaleX + 1, mapScaleY + 1];

            // Assign the maps to their position in the world array.
            foreach (MapScreen map in currentWorld.maps)
            {
                World.maps[map.coordinateX, map.coordinateY] = map;
            }

            // Check for empty coordinates in the world and fill them with empty screens.
            for (int x = 0; x < mapScaleX; x++)
            {
                for (int y = 0; y < mapScaleY; y++)
                {
                    if (World.maps[x, y] == null)
                    {
                        // Default constructor creates an empty map screen.
                        World.maps[x, y] = new MapScreen();
                    }
                }
            }
        }
    }
}