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
        public float minNodeDistance = 3;

        public int minNodeConnections = 1;
        public int maxNodeConnection = 3;
        public int maxNodeConnectionDistance = 5;
        public int shopChance;

        [Header("Prefabs")]
        public GameObject emptyNodePrefab;

        [Header("Colors")]
        public Color emptyNodeColor;
        public Color enemyNodeColor;
        public Color friendlyNodeColor;
        public Color selectedNodeColor;
        public Color currentNodeColor;
    }
}