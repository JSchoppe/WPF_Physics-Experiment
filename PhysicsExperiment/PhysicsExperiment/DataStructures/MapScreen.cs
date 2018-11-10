using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.IO;

// Thanks to David Antognoli
// for helping me understand preprocesser directives

namespace PhysicsExperiment.DataStructures
{
    public class MapScreen
    {
        public BoxCollider[] staticColliders;
        public Uri aestheticLayer;
        public byte coordinateX;
        public byte coordinateY;

        /// <summary>Construct a map screen with that the player cannot enter and will have no collision</summary>
        public MapScreen()
        {
            // This is used by the design parser to fill gaps the level maps array.
            // Designer does not have to specify these in the WorldMap.
            aestheticLayer = new Uri(Game.resourcesPath.FullName + "/UndiscoveredScreen.png");
        }

        public MapScreen(string collisionMapName, string aestheticMapName, byte coordX, byte coordY)
        {
            if (File.Exists(Game.resourcesPath.FullName + "/CollisionMaps/" + collisionMapName + ".bmp"))
            {
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

            if (File.Exists(Game.resourcesPath.FullName + "/" + aestheticMapName + ".png"))
            {
                aestheticLayer = new Uri(Game.resourcesPath.FullName + "/" + aestheticMapName + ".png");
            }
            else
            {
                // Replace with default image.
                aestheticLayer = new Uri(Game.resourcesPath.FullName + "/UndiscoveredScreen.png");
#if DEBUG
                // Show an error popup if in debug build.
                MessageBox.Show("The requested aesthetic map: " + aestheticMapName + ".png does not exist in the resources folder!", "Unknown Asset Error");
#endif
            }

            coordinateX = coordX;
            coordinateY = coordY;
        }
    }
}
