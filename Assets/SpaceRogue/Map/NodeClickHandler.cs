using UnityEngine;

namespace SpaceRogue.Map
{
    public class NodeClickHandler : MonoBehaviour
    {
        public MapNode MapNodeData { get; set; }
        private void OnMouseDown()
        {
            //TODO: figure out how to handle click properly
            Debug.Log("Click");
            MapManager.Instance.HandleNodeSelection(MapNodeData);
        }
    }
}