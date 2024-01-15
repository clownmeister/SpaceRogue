using SpaceRogue.Map.Settings;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceRogue.Map
{
    public class MapController : MonoBehaviour
    {
        public int seed;
        public GameObject lineRendererPrefab;
        public SystemMapSettings systemMapSettings;
        public float nodeZ = -2f;
        public float lineZ = -1.5f;
        [Header("Gizmo settings")]
        public bool drawGizmos;
        public Color boundariesColor = Color.cyan;

        private Transform _systemMapParent;
        private SystemMap _systemMap;
        private HashSet<NodePair> _drawnConnections = new HashSet<NodePair>();
        private void OnDrawGizmos()
        {
            if (!this.drawGizmos) return;
            Gizmos.color = this.boundariesColor;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(systemMapSettings.mapSize.x * 2, systemMapSettings.mapSize.y * 2, 0));
        }

        void Start()
        {
            GameObject mapNodes = new GameObject("MapNodes");
            _systemMapParent = mapNodes.transform;

            this._systemMap = new SystemMap(this.systemMapSettings);
            RegenerateMap(this.seed);
        }

        private void RegenerateMap(int seed)
        {
            ClearMap();

            this.seed = seed;
            this._systemMap.Generate(this.seed);
            DrawSystemMap();
        }

        private void ClearMap()
        {
            DestroyAllChildren(this._systemMapParent);
            _drawnConnections.Clear();
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
            if (Input.GetKeyDown(KeyCode.G))
            {
                RegenerateMap(Random.Range(0, 999_999));
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                RegenerateMap(this.seed);
            }
        }

        private void DrawSystemMap()
        {
            foreach (KeyValuePair<Vector2, MapNode> node in this._systemMap.Nodes)
            {
                // Vector2 position = TranslatePointOnMapToScreenSize(node.Key);
                Vector2 position = node.Key;
                Instantiate(this.systemMapSettings.nodePrefab, new Vector3(position.x, position.y, nodeZ), Quaternion.identity, this._systemMapParent);
                DrawConnections(position, node.Value);
            }
        }

        private void DrawConnections(Vector2 position, MapNode node)
        {
            foreach (MapNode neighbour in node.Neighbours)
            {
                NodePair pair = new NodePair(position, neighbour.Position);

                // Check if the connection is already drawn
                if (_drawnConnections.Contains(pair) || _drawnConnections.Contains(new NodePair(neighbour.Position, position)))
                {
                    continue;
                }

                GameObject lineObj = Instantiate(lineRendererPrefab, this._systemMapParent);
                LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();

                Vector3 startPosition = new Vector3(position.x, position.y, lineZ); // Adjust Z axis if needed
                Vector3 endPosition = new Vector3(neighbour.Position.x, neighbour.Position.y, lineZ); // Adjust Z axis if needed

                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, startPosition);
                lineRenderer.SetPosition(1, endPosition);

                // Add the pair to the HashSet
                _drawnConnections.Add(pair);
            }
        }
        
        private Vector2 TranslatePointOnMapToScreenSize(Vector2 point)
        {
            return new Vector2(Screen.width, Screen.height) / this.systemMapSettings.mapSize * point;
        }
    }
}