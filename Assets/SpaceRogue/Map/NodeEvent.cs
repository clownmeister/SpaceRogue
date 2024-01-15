namespace SpaceRogue.Map
{
    public enum NodeEventType
    {
        Nothing,
        Ambush,
        Surprise,
        Distress,
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