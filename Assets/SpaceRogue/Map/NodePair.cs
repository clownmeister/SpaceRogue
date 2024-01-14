using UnityEngine;

namespace SpaceRogue.Map
{
    public struct NodePair
    {
        public Vector2 Node1;
        public Vector2 Node2;

        public NodePair(Vector2 node1, Vector2 node2)
        {
            Node1 = node1;
            Node2 = node2;
        }
    }

}