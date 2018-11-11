using System.Collections.Generic;

using PhysicsExperiment.DataStructures;

namespace PhysicsExperiment.Design
{
    public class WorldMap
    {
        public List<MapScreen> maps;

        public WorldMap()
        {
            maps = new List<MapScreen>()
            {
                // Declare maps within this world.
                new MapScreen("LeftRoom", "LeftRoom", 0, 1),
                new MapScreen("OriginRoom", "OriginRoom", 1, 1),
                new MapScreen("LeftTower", "LeftTower", 0, 0),
                new MapScreen("TowerCrossing", "TowerCrossing", 1, 0)
            };
        }
    }
}