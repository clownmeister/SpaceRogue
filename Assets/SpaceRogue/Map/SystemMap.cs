using SpaceRogue.Map.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceRogue.Map
{
    public class SystemMap
    {
        public Dictionary<Vector2, MapNode> Nodes { get; private set; }

        public MapNode StartNode { get; set; }
        public MapNode EndNode { get; set; }

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
            EnsureNoIsolatedNodes();
            ConnectIsolatedGroups();
            FinalConnectivityCheck();
            SetStartAndEndNodes();
        }

        private void SetStartAndEndNodes()
        {
            // We are cheating since BFS was a pain
            // Find the leftmost 5 and rightmost 5 nodes
            List<MapNode> leftMostNodes = Nodes.OrderBy(node => node.Key.x).Take(5).Select(node => node.Value).ToList();
            List<MapNode> rightMostNodes = Nodes.OrderByDescending(node => node.Key.x).Take(5).Select(node => node.Value).ToList();

            // Randomly pick one from each
            StartNode = leftMostNodes[Random.Range(0, leftMostNodes.Count)];
            EndNode = rightMostNodes[Random.Range(0, rightMostNodes.Count)];

            // Set start and end nodes
            StartNode.Variant = MapNodeVariant.Start;
            EndNode.Variant = MapNodeVariant.End;

            Debug.Log($"Start Node: {StartNode.Position}, End Node: {EndNode.Position}");
        }

        private void FinalConnectivityCheck()
        {
            HashSet<MapNode> visited = new HashSet<MapNode>();
            List<List<MapNode>> groups = new List<List<MapNode>>();

            // Re-identify groups after initial connections
            foreach (MapNode node in Nodes.Values)
            {
                if (visited.Contains(node)) continue;
                List<MapNode> group = new List<MapNode>();
                Traverse(node, visited, group);
                groups.Add(group);
            }

            // If more than one group exists, connect them
            while (groups.Count > 1)
            {
                List<MapNode> groupA = groups[0];
                List<MapNode> groupB = groups[1];

                Tuple<MapNode, MapNode, float> closestNodesPair = FindClosestNodes(groupA, groupB);

                if (closestNodesPair.Item1 != null && closestNodesPair.Item2 != null)
                {
                    closestNodesPair.Item1.AddNeighbour(closestNodesPair.Item2);
                    closestNodesPair.Item2.AddNeighbour(closestNodesPair.Item1);
                }

                // Merge the groups and continue
                groupA.AddRange(groupB);
                groups.RemoveAt(1);
            }
        }

        private void ConnectIsolatedGroups()
        {
            HashSet<MapNode> visited = new HashSet<MapNode>();
            List<List<MapNode>> groups = new List<List<MapNode>>();

            // Identify disconnected groups
            foreach (MapNode node in Nodes.Values)
            {
                if (visited.Contains(node)) continue;
                List<MapNode> group = new List<MapNode>();
                Traverse(node, visited, group);
                groups.Add(group);
            }

            // Connect the closest groups
            while (groups.Count > 1)
            {
                int closestGroupIndex = -1;
                Tuple<MapNode, MapNode> closestNodesPair = null;
                float minDistance = float.MaxValue;

                for (int i = 0; i < groups.Count; i++)
                {
                    for (int j = i + 1; j < groups.Count; j++)
                    {
                        Tuple<MapNode, MapNode, float> currentClosestPair = FindClosestNodes(groups[i], groups[j]);
                        if (!(currentClosestPair.Item3 < minDistance)) continue;
                        minDistance = currentClosestPair.Item3;
                        closestNodesPair = new Tuple<MapNode, MapNode>(currentClosestPair.Item1, currentClosestPair.Item2);
                        closestGroupIndex = j;
                    }
                }

                if (closestNodesPair != null)
                {
                    closestNodesPair.Item1.AddNeighbour(closestNodesPair.Item2);
                    closestNodesPair.Item2.AddNeighbour(closestNodesPair.Item1);
                    groups[0].AddRange(groups[closestGroupIndex]);
                    groups.RemoveAt(closestGroupIndex);
                }
                else
                {
                    break; // No more connections possible
                }
            }
        }

        private Tuple<MapNode, MapNode, float> FindClosestNodes(List<MapNode> groupA, List<MapNode> groupB)
        {
            MapNode closestNodeA = null;
            MapNode closestNodeB = null;
            float minDistance = float.MaxValue;

            foreach (MapNode nodeA in groupA)
            {
                foreach (MapNode nodeB in groupB)
                {
                    float distance = Vector2.Distance(nodeA.Position, nodeB.Position);
                    if (!(distance < minDistance)) continue;
                    minDistance = distance;
                    closestNodeA = nodeA;
                    closestNodeB = nodeB;
                }
            }

            return new Tuple<MapNode, MapNode, float>(closestNodeA, closestNodeB, minDistance);
        }

        private static void Traverse(MapNode node, HashSet<MapNode> visited, List<MapNode> group)
        {
            visited.Add(node);
            group.Add(node);

            foreach (MapNode neighbour in node.Neighbours)
            {
                if (!visited.Contains(neighbour))
                {
                    Traverse(neighbour, visited, group);
                }
            }
        }

        private void EnsureNoIsolatedNodes()
        {
            List<KeyValuePair<Vector2, MapNode>> isolatedNodes = Nodes.Where(node => node.Value.Neighbours.Length == 0).ToList();

            foreach (KeyValuePair<Vector2, MapNode> isolatedNode in isolatedNodes)
            {
                MapNode closestNode = Nodes
                    .Where(node => node.Key != isolatedNode.Key)
                    .OrderBy(node => Vector2.Distance(node.Key, isolatedNode.Key))
                    .First().Value;

                // Connect the isolated node with the closest node
                isolatedNode.Value.AddNeighbour(closestNode);
                closestNode.AddNeighbour(isolatedNode.Value);
            }
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
                        Debug.LogWarning($"Exceeded max attempts on node placement. Not enough space! Created {result.Count} out of {this._finalAmount} desired nodes.");
                        return result;
                    }

                    i--;
                    // Debug.Log("Not enough space for node placement. Invalid position resetting");
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
                // Determine random number of neighbours (between 1 and maxNodeConnection)
                int numOfNeighbours = Random.Range(_settings.minNodeConnections, _settings.maxNodeConnection + 1);

                List<MapNode> potentialNeighbours = Nodes
                    .Where(n => n.Key != item.Key && Vector2.Distance(n.Key, item.Key) <= _settings.maxNodeConnectionDistance)
                    .OrderBy(n => Vector2.Distance(n.Key, item.Key))
                    .Take(numOfNeighbours)
                    .Select(n => n.Value)
                    .ToList();

                foreach (MapNode neighbour in potentialNeighbours.Where(neighbour => neighbour.Neighbours.Length < _settings.maxNodeConnection && !neighbour.Neighbours.Contains(item.Value)))
                {
                    item.Value.AddNeighbour(neighbour);
                }
            }
        }

        private static bool ValidPosition(Dictionary<Vector2, MapNode> nodes, SystemMapSettings settings, Vector2 position)
        {
            return nodes.All(node => Vector2.Distance(node.Key, position) >= settings.minNodeDistance);
        }

        private static Vector2 GetMapAxisBoundaries(float axisMax, float axisPadding)
        {
            return new Vector2(-axisMax + axisPadding, axisMax - axisPadding);
        }
    }
}