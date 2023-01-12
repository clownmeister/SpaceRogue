using System.Collections.Generic;
using System.Linq;
using SpaceRogue.Map.Node;
using SpaceRogue.Map.Settings;
using UnityEngine;

namespace SpaceRogue.Map
{
    public class SystemMap
    {
        public Dictionary<Vector2, MapNode> Nodes { get; private set; }

        private int finalAmount;
        private int currentSeed;
        private readonly SystemMapSettings settings;

        public SystemMap(SystemMapSettings settings)
        {
            this.settings = settings;
        }

        public void Generate(int seed)
        {
            this.currentSeed = seed;
            Nodes = GenerateNodes(this.currentSeed);
        }
        
        private Dictionary<Vector2, MapNode> GenerateNodes(int seed)
        {
            Debug.Log("Generating map");
            Random.InitState(seed);
            Dictionary<Vector2, MapNode> result = new();

            this.finalAmount = settings.nodeAmount + Random.Range(-settings.nodeAmountVariation, settings.nodeAmountVariation + 1);

            for (int i = 0; i < this.finalAmount; i++)
            {
                Vector2 boundaryX = GetMapAxisBoundaries(settings.mapSize.x, settings.mapPadding.x);
                Vector2 boundaryY = GetMapAxisBoundaries(settings.mapSize.y, settings.mapPadding.y);
                Vector2 position = new(Random.Range(boundaryX.x, boundaryX.y), Random.Range(boundaryY.x, boundaryY.y));
                if (!ValidPosition(result, settings, position))
                {
                    i--;
                    Debug.Log("Invalid position resetting");
                    continue;
                }

                result.Add(position, new MapNode());
                Debug.Log(position);
            }

            return result;
        }

        private bool ValidPosition(Dictionary<Vector2, MapNode> nodes, SystemMapSettings settings, Vector2 position)
        {
            return nodes.All(node => Vector2.Distance(node.Key, position) >= settings.minNodeDistance);
        }
        
        private Vector2 GetMapAxisBoundaries(float axisMax, float axisPadding)
        {
            return new Vector2(0 + axisPadding, axisMax - axisPadding);
        }
    }
}