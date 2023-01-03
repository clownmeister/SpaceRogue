namespace SpaceRogue.Map.Node
{
    public class MapNode
    {
        public MapNodeType Type;
        public NodeEvent Event;
        public MapNode[] Neighbours;

        public MapNode(MapNodeType nodeType = MapNodeType.Empty, NodeEventType eventType = NodeEventType.Nothing)
        {
            this.Type = nodeType;
            this.Event = new NodeEvent(eventType);
        }
    }
}