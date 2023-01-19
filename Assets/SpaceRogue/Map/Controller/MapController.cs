using System.Collections.Generic;
using SpaceRogue.Map.Node;
using SpaceRogue.Map.Settings;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace SpaceRogue.Map.Controller
{
    public class MapController : MonoBehaviour
    {
        public int seed;
        public SystemMapSettings systemMapSettings;
        public Canvas mapCanvas;
        public RectTransform systemMapParent;
        
        private SystemMap systemMap;
        void Start()
        {
            this.systemMap = new SystemMap(this.systemMapSettings);
            RegenerateMap(this.seed);
        }

        private void RegenerateMap(int seed)
        {
            this.seed = seed;
            DestroyAllChildren(this.systemMapParent);
            this.systemMap.Generate(this.seed);
            DrawSystemMap();
        }

        private static void DestroyAllChildren(Component component)
        {
            foreach (Transform child in component.transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        private void Update()
        {
            //TODO: DEBUG
            if (Input.GetKeyDown(KeyCode.M))
            {
                ShowMap(!this.mapCanvas.isActiveAndEnabled);
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                RegenerateMap(Random.Range(0, 999_999));
            }
        }

        private void ShowMap(bool show = true)
        {
            this.mapCanvas.enabled = show;
        }
        
        private void DrawSystemMap()
        {
            foreach (KeyValuePair<Vector2, MapNode> node in this.systemMap.Nodes)
            {
                Vector2 position = TranslatePointOnMapToScreenSize(node.Key, this.mapCanvas.renderingDisplaySize, this.systemMapSettings.mapSize);
                Instantiate(this.systemMapSettings.nodePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity, this.systemMapParent);
            }
        }
        
        private Vector2 TranslatePointOnMapToScreenSize(Vector2 point, Vector2 screenSize, Vector2 mapSize)
        {
            return screenSize / mapSize * point;
        }
    }
}