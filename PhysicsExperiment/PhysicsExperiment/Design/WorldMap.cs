using PhysicsExperiment.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsExperiment.Design
{
    public class WorldMap
    {
        public List<MapScreen> maps;

        public WorldMap()
        {
            maps = new List<MapScreen>()
            {
                // Declare maps within this screen.
                new MapScreen("TestRoom", "TestArt", 0, 0)
            };
        }
    }
}