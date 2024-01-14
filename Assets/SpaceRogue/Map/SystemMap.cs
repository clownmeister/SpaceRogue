using System.Collections.Generic;
using System.Linq;
using SpaceRogue.Map.Settings;
using UnityEngine;

namespace SpaceRogue.Map
{
    public class SystemMap
    {
        public Dictionary<Vector2, MapNode> Nodes { get; private set; }

        private int _finalAmount;
        private int _currentSeed;
        private readonly SystemMapSettings _settings;

        public SystemMap(SystemMapSettings settings)
        {
            this._settings = settings;
        }

        public void Generate(int seed)
        {
            this._currentSeed = seed;
            Nodes = GenerateNodes(this._currentSeed);
            DetermineNeighbours();
        }
        
        private Dictionary<Vector2, MapNode> GenerateNodes(int seed)
        {
            Debug.Log("Generating map");
            Random.InitState(seed);
            Dictionary<Vector2, MapNode> result = new Dictionary<Vector2, MapNode>();

            this._finalAmount = _settings.nodeAmount + Random.Range(-_settings.nodeAmountVariation, _settings.nodeAmountVariation + 1);
            int attempts = 0;
            for (int i = 0; i < this._finalAmount; i++)
            {
                Vector2 boundaryX = GetMapAxisBoundaries(_settings.mapSize.x, _settings.mapPadding.x);
                Vector2 boundaryY = GetMapAxisBoundaries(_settings.mapSize.y, _settings.mapPadding.y);
                Vector2 position = new Vector2(Random.Range(boundaryX.x, boundaryX.y), Random.Range(boundaryY.x, boundaryY.y));
                if (!ValidPosition(result, _settings, position))
                {
                    attempts++;
                    if (attempts > this._settings.maxAttemptsNodePlacement)
                    {
                        Debug.LogWarning("Not enough free positions. Skipping additional node creation!");
                        return result;
                    }
                    
                    i--;
                    Debug.Log("Not enough space for node placement. Invalid position resetting");
                    continue;
                }

                attempts = 0;
                result.Add(position, new MapNode(position));
                // Debug.Log(position);
            }

            return result;
        }

        private void DetermineNeighbours()
        {
            foreach (KeyValuePair<Vector2, MapNode> item in this.Nodes)
            {
                MapNode[] neighbours = Nodes
                    .Where(n => n.Key != item.Key)
                    .OrderBy(n => Vector2.Distance(n.Key, item.Key))
                    .Take(_settings.maxNeighbours)
                    .Select(n => n.Value)
                    .ToArray();

                item.Value.SetNeighbours(neighbours);
            }
        }

        private bool ValidPosition(Dictionary<Vector2, MapNode> nodes, SystemMapSettings settings, Vector2 position)
        {
            return nodes.All(node => Vector2.Distance(node.Key, position) >= settings.minNodeDistance);
        }
        
        private Vector2 GetMapAxisBoundaries(float axisMax, float axisPadding)
        {
            return new Vector2(-axisMax + axisPadding, axisMax - axisPadding);
        }
    }
}