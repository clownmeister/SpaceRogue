using System.ComponentModel;
using UnityEngine;

namespace SpaceRogue.Map.Settings
{
    [CreateAssetMenu(fileName = "New System Map Settings", menuName = "Map/SystemMap", order = 100)]
    public class SystemMapSettings : AbstractMapSettings
    {
        [Header("Map settings")]
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

        [Header("Node type distribution")]
        public float emptyRatio;
        public float planetRatio;
        public float blackHoleRatio;
        public float nebulaRatio;
        public float asteroidRatio;

        [Header("Render Settings")]
        [Header("Prefabs")]
        public GameObject lineRendererPrefab;
        public GameObject emptyNodePrefab;
        public GameObject planetNodePrefab;
        public GameObject blackHoleNodePrefab;
        public GameObject nebulaNodePrefab;
        public GameObject asteroidFieldNodePrefab;

        [Header("Colors")]
        public Color emptyNodeColor;
        public Color enemyNodeColor;
        public Color friendlyNodeColor;
        public Color selectedNodeColor;
        public Color currentNodeColor;

        [Header("Render Order")]
        public float nodeZ = -2f;
        public float lineZ = -1.5f;
    }
}