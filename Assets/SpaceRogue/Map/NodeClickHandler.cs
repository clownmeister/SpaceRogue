using UnityEngine;

namespace SpaceRogue.Map
{
    public class NodeClickHandler : MonoBehaviour
    {
        public MapNode MapNodeData { get; set; }
        private void OnMouseDown()
        {
            //TODO: figure out how to handle click properly
            // also why does this work?
            Debug.Log("Click Node");
            MapManager.Instance.HandleNodeSelection(MapNodeData);
        }
    }
}