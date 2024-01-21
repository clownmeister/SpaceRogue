using System;
using System.Collections.Generic;
using SpaceRogue.Map.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceRogue.Map
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance;

        public MapNode CurrentNode { get; set; }

        private MapNode _selectedNode;

        public int seed;
        public GameObject lineRendererPrefab;
        public SystemMapSettings systemMapSettings;
        public float nodeZ = -2f;
        public float lineZ = -1.5f;
        public LayerMask clickLayerMask;

        [Header("Gizmo settings")]
        public bool drawGizmos;
        public Color boundariesColor = Color.cyan;

        private Transform _systemMapParent;
        private SystemMap _systemMap;
        private HashSet<NodePair> _drawnConnections = new HashSet<NodePair>();

        // Double tap
        private float _lastTapTime = 0f;
        private const float DOUBLE_TAP_INTERVAL = 0.3f;

        private int _mapLayer;

        private const string NODE_PARENT_NAME = "MapNodes";

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            Gizmos.color = boundariesColor;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(systemMapSettings.mapSize.x * 2, systemMapSettings.mapSize.y * 2, 0));
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _mapLayer = LayerMask.NameToLayer("Map");
            _systemMap = new SystemMap(systemMapSettings);
        }

        private void InitNodeParent()
        {
            GameObject parent = GameObject.Find(NODE_PARENT_NAME);
            if (parent)
            {
                _systemMapParent = parent.transform;
                return;
            };
            GameObject mapNodes = new GameObject(NODE_PARENT_NAME);
            mapNodes.layer = _mapLayer;
            _systemMapParent = mapNodes.transform;
        }

        public void RegenerateMap(int? seed = null)
        {
            InitNodeParent();
            ClearMap();
            _systemMap.Generate(seed ?? this.seed);
            InitCurrentNode();

            DrawSystemMap();
        }

        private void InitCurrentNode()
        {
            CurrentNode = _systemMap.StartNode;
        }

        private void ClearMap()
        {
            DestroyAllChildren(_systemMapParent);
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
                RegenerateMap(seed);
            }

            // Detect double-tap on mobile
            if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Ended)
            {
                if (Time.time - _lastTapTime < DOUBLE_TAP_INTERVAL)
                {
                    // Double tap detected, regenerate the map
                    RegenerateMap(Random.Range(0, 999_999));
                    _lastTapTime = 0f; // Reset the last tap time
                }
                else
                {
                    _lastTapTime = Time.time;
                }
            }

            // if (Input.GetMouseButtonDown(0))
            // {
            //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //     RaycastHit hit;
            //     if (!Physics.Raycast(ray, out hit, Mathf.Infinity,clickLayerMask)) return;
            //     NodeClickHandler clickHandler = hit.collider.GetComponent<NodeClickHandler>();
            //     if (clickHandler != null)
            //     {
            //         //TODO: fix? and add mobile click
            //         Debug.Log("Click from ray found");
            //         HandleNodeSelection(clickHandler.MapNodeData);
            //     }
            // }
        }

        public void HandleNodeSelection(MapNode selectedNode)
        {
            if (selectedNode == _selectedNode) return;
            if (_selectedNode != null)
            {
                MapNode oldNode = _selectedNode;
                _selectedNode = null;
                SetNodeColor(oldNode, GetNodeColor(oldNode));
            }

            _selectedNode = selectedNode;
            SetNodeColor(selectedNode, GetNodeColor(selectedNode));
        }

        private static void SetNodeColor(GameObject nodeGameObject, Color color)
        {
            SpriteRenderer[] renderers = nodeGameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in renderers)
            {
                spriteRenderer.color = color;
            }
        }

        private static void SetNodeColor(MapNode node, Color color)
        {
            SetNodeColor(node.GameObject, color);
        }

        private void DrawSystemMap()
        {
            foreach (KeyValuePair<Vector2, MapNode> item in _systemMap.Nodes)
            {
                MapNode node = item.Value;
                Vector2 position = item.Key;
                // TODO: Do we need translation?
                // Vector2 position = TranslatePointOnMapToScreenSize(node.Key);
                CreateNodeGameObject(node, position);
                DrawConnections(position, node);
            }
        }

        private void CreateNodeGameObject(MapNode node, Vector2 position)
        {
            // Create node + apply color
            GameObject nodeGameObject = Instantiate(GetNodePrefab(node), new Vector3(position.x, position.y, nodeZ), Quaternion.identity, _systemMapParent);
            nodeGameObject.layer = _mapLayer;
            SetNodeColor(nodeGameObject, GetNodeColor(node));

            //Link
            NodeClickHandler handler = nodeGameObject.AddComponent<NodeClickHandler>();
            handler.MapNodeData = node;
            node.GameObject = nodeGameObject;
        }

        private GameObject GetNodePrefab(MapNode node)
        {
            GameObject prefab;

            //Get correct prefab
            switch (node.Type)
            {
                case MapNodeType.Empty:
                case MapNodeType.Planet:
                case MapNodeType.Storm:
                case MapNodeType.Asteroids:
                case MapNodeType.BlackHole:
                    prefab = systemMapSettings.emptyNodePrefab;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return prefab;
        }

        private Color GetNodeColor(MapNode node)
        {
            Color color;
            if (node == CurrentNode)
            {
                color = systemMapSettings.currentNodeColor;
            } else if (node == _selectedNode)
            {
                color = systemMapSettings.selectedNodeColor;
            }
            else
            {
                color = node.Variant switch
                {
                    MapNodeVariant.Normal => systemMapSettings.emptyNodeColor,
                    MapNodeVariant.End => systemMapSettings.enemyNodeColor,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return color;
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

                GameObject lineObj = Instantiate(lineRendererPrefab, _systemMapParent);
                lineObj.layer = _mapLayer;
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
            return new Vector2(Screen.width, Screen.height) / systemMapSettings.mapSize * point;
        }
    }
}