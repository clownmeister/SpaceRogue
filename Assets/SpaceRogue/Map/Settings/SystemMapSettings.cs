using System.ComponentModel;
using UnityEngine;

namespace SpaceRogue.Map.Settings
{
    [CreateAssetMenu(fileName = "New System Map Settings", menuName = "Map/SystemMap", order = 100)]
    public class SystemMapSettings : AbstractMapSettings
    {
        public Vector2 mapSize;
        [Range(1, 100)]
        public int nodeAmount;
        [Description("Randomnes limit to NodeAmount should result as same for each seed.")]
        public int nodeAmountVariation;

        public float minNodeDistance;

        public int shopChance;
        public int minNodeConnections;
        public int maxNodeConnection;
    }
}