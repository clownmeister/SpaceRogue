using UnityEngine;

namespace SpaceRogue.Map
{
    public enum MapNodeType
    {
        Empty,
        Planet,
        Station,
        System,
        Galaxy
    }

    public class MapNode
    {
        public Vector2 Position;
        public MapNodeType Type;
        public NodeEvent Event;
        public MapNode[] Neighbours;

        public MapNode(Vector2 position, MapNodeType nodeType = MapNodeType.Empty, NodeEventType eventType = NodeEventType.Nothing)
        {
            Position = position;
            Type = nodeType;
            Event = new NodeEvent(eventType);
        }

        public void SetNeighbours(MapNode[] neighbours)
        {
            this.Neighbours = neighbours;
        }
    }
}