using System;
using System.IO;
using System.Windows;
using System.Drawing;

// Thanks to David Antognoli
// for helping me understand preprocesser directives

namespace PhysicsExperiment.DataStructures
{
    /// <summary>Data that represents a screen on the world map</summary>
    public class MapScreen
    {
        /// <summary>The static colliders in this screen</summary>
        public BoxCollider[] staticColliders;

        /// <summary>Uri for the background image of this screen</summary>
        public Uri aestheticLayer;

        // Following coordinate values are bytes to prevent constructing with negative map indeces.

        /// <summary>Represents the x location in the world map</summary>
        public byte coordinateX;

        /// <summary>Represents the y location in the world map(top-down)</summary>
        public byte coordinateY;

        /// <summary>Construct a map screen with that the player cannot enter and will have no collision</summary>
        public MapScreen()
        {
            // This is used by the design parser to fill gaps the level maps array.
            // Designer does not have to specify these in the WorldMap.
            aestheticLayer = new Uri(Game.resourcesPath.FullName + "/AestheticMaps/UndiscoveredScreen.png");
        }

        /// <summary>Construct a map screen</summary>
        public MapScreen(string collisionMapName, string aestheticMapName, byte coordX, byte coordY)
        {
            // Does the passed collision map exist in the resources?
            if (File.Exists(Game.resourcesPath.FullName + "/CollisionMaps/" + collisionMapName + ".bmp"))
            {
                // If so, calculate this screen's static colliders from the bitmap.
                staticColliders = Mapping.ParseCollisionMap(new Bitmap(Game.resourcesPath.FullName + "/CollisionMaps/" + collisionMapName + ".bmp"));
            }
            else
            {
                // Initialize the box colliders array as empty.
                staticColliders = new BoxCollider[0];
#if DEBUG
                // Show an error popup if in debug build.
                MessageBox.Show("The requested collision map: " + collisionMapName + ".bmp does not exist in the collision maps folder!", "Unknown Asset Error");
#endif
            }

            // Does the passed aesthetic map exist in the resources?
            if (File.Exists(Game.resourcesPath.FullName + "/AestheticMaps/" + aestheticMapName + ".png"))
            {
                // If so, retrieve it as a Uri.
                aestheticLayer = new Uri(Game.resourcesPath.FullName + "/AestheticMaps/" + aestheticMapName + ".png");
            }
            else
            {
                // Replace with default image.
                aestheticLayer = new Uri(Game.resourcesPath.FullName + "/AestheticMaps/UndiscoveredScreen.png");
#if DEBUG
                // Show an error popup if in debug build.
                MessageBox.Show("The requested aesthetic map: " + aestheticMapName + ".png does not exist in the resources folder!", "Unknown Asset Error");
#endif
            }

            // Set this screens map coordinates.
            coordinateX = coordX;
            coordinateY = coordY;
        }
    }
}