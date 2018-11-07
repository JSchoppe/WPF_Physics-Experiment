using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.IO;

// Thanks to David Antognoli
// for helping me understand preprocesser directives

namespace PhysicsExperiment.DataStructures
{
    public class MapScreen
    {
        public BoxCollider[] staticColliders;

        public Uri aestheticLayer;

        public MapScreen(string collisionMapName, string aestheticMapName)
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
        }
    }
}
