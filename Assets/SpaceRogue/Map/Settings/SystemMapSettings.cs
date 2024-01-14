using System.ComponentModel;
using UnityEngine;

namespace SpaceRogue.Map.Settings
{
    [CreateAssetMenu(fileName = "New System Map Settings", menuName = "Map/SystemMap", order = 100)]
    public class SystemMapSettings : AbstractMapSettings
    {
        public Vector2 mapSize;
        public Vector2 mapPadding;
        [Range(1, 100)]
        public int nodeAmount = 18;
        [Description("Randomness limit to NodeAmount should result as same for each seed.")]
        public int nodeAmountVariation = 4;
        public int maxAttemptsNodePlacement = 100;
        public int maxNeighbours = 3;

        public float minNodeDistance = 100;

        public int shopChance;
        public int minNodeConnections;
        public int maxNodeConnection;

        public GameObject nodePrefab;
    }
}