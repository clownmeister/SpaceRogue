namespace SpaceRogue.Map
{
    public enum NodeEventType
    {
        Nothing,
        Ambush,
        Shop,
    }

    public class NodeEvent
    {
        private NodeEventType type;

        public NodeEvent(NodeEventType type)
        {
            this.type = type;
        }
    }
}