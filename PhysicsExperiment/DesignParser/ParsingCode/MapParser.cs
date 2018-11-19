using PhysicsExperiment.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignParser
{
    class MapParser
    {
        // Parses all of the current maps in Design>WorldMap.
        public MapParser()
        {
            // Fetch the default world map.
            WorldMap currentWorld = new WorldMap();

            // Declare vars to track the bounding scale of the world.
            byte mapScaleX = 0;
            byte mapScaleY = 0;

            // Determine the scale of the world by searching for the highest coordinates.
            foreach (MapScreen map in currentWorld.maps)
            {
                // Does this screen push the x boundary?
                if (map.coordinateX > mapScaleX)
                {
                    mapScaleX = map.coordinateX;
                }

                // Does this screen push the y boundary?
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
            for (int x = 0; x < mapScaleX + 1; x++)
            {
                for (int y = 0; y < mapScaleY + 1; y++)
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