using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceRogue.Map
{
    public enum MapNodeType
    {
        Empty,
        Planet,
        Storm,
        Asteroids,
        BlackHole,
    }

    public enum MapNodeVariant
    {
        Normal,
        Start,
        End
    }

    public class MapNode
    {
        public Vector2 Position;
        public MapNodeType Type;
        public MapNodeVariant Variant;
        public NodeEvent Event;
        public MapNode[] Neighbours;

        public GameObject GameObject { get; set; }

        public MapNode(Vector2 position, MapNodeType nodeType = MapNodeType.Empty, NodeEventType eventType = NodeEventType.Nothing)
        {
            Position = position;
            Type = nodeType;
            Event = new NodeEvent(eventType);
            this.Neighbours = Array.Empty<MapNode>();
        }


        public void AddNeighbour(MapNode neighbour)
        {
            List<MapNode> neighboursList = Neighbours.ToList();
            if (neighboursList.Contains(neighbour)) return;
            neighboursList.Add(neighbour);
            Neighbours = neighboursList.ToArray();
        }
    }
}