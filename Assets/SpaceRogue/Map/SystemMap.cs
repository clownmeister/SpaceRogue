using System.Collections.Generic;
using System.Linq;
using SpaceRogue.Map.Node;
using SpaceRogue.Map.Settings;
using UnityEngine;

namespace SpaceRogue.Map
{
    public class SystemMap
    {
        private Dictionary<Vector2, MapNode> nodes;
        private int finalAmount;

        public SystemMap(SystemMapSettings settings, int seed = 0)
        {
            this.nodes = Generate(settings, seed);
        }

        private Dictionary<Vector2, MapNode> Generate(SystemMapSettings settings, int seed = 0)
        {
            Random.InitState(seed);
            Dictionary<Vector2, MapNode> result = new();

            this.finalAmount = settings.nodeAmount + Random.Range(-settings.nodeAmountVariation, settings.nodeAmountVariation + 1);

            for (int i = 0; i < this.finalAmount; i++)
            {
                Vector2 position = new(Random.Range(0, settings.mapSize.x), Random.Range(0, settings.mapSize.y));
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
    }
}