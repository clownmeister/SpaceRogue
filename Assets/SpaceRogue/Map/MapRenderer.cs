using System;
using System.Collections.Generic;
using SpaceRogue.Map.Settings;
using UnityEngine;

namespace SpaceRogue.Map
{
    public class MapRenderer : MonoBehaviour
    {
        private const string NODE_PARENT_NAME = "MapNodes";

        private SystemMapSettings _settings;

        private Transform _systemMapParent;
        private HashSet<NodePair> _drawnConnections = new HashSet<NodePair>();
        private int _mapLayer;

        private void Awake()
        {
            _mapLayer = LayerMask.NameToLayer("Map");
            _settings = MapManager.Instance.systemMapSettings;
        }

        public void Clear()
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

        public void Render(SystemMap systemMap)
        {
            InitNodeParent();
            Clear();
            foreach ((Vector2 position, MapNode node) in systemMap.Nodes)
            {
                // TODO: Do we need translation?
                // Vector2 position = TranslatePointOnMapToScreenSize(node.Key);
                Debug.Log("test123");
                Debug.Log(position);
                Debug.Log(node);
                CreateNodeGameObject(node, position);
                DrawConnections(node, position);
            }
        }



        private static void SetNodeColor(GameObject nodeGameObject, Color color)
        {
            SpriteRenderer[] renderers = nodeGameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in renderers)
            {
                spriteRenderer.color = color;
            }
        }

        public void SetNodeColor(MapNode node)
        {
            SetNodeColor(node.GameObject, GetNodeColor(node));
        }

        private void CreateNodeGameObject(MapNode node, Vector2 position)
        {
            // Create node + apply color
            GameObject nodeGameObject = Instantiate(GetNodePrefab(node), new Vector3(position.x, position.y, _settings.nodeZ), Quaternion.identity, _systemMapParent);
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
                    prefab = _settings.emptyNodePrefab;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return prefab;
        }

        private Color GetNodeColor(MapNode node)
        {
            if (node == MapManager.Instance.CurrentNode || node == MapManager.Instance.SelectedNode)
            {
                return _settings.currentNodeColor;
            }

            return node.Variant switch
            {
                MapNodeVariant.Normal => _settings.emptyNodeColor,
                MapNodeVariant.End => _settings.enemyNodeColor,
                MapNodeVariant.Start => _settings.emptyNodeColor,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void DrawConnections(MapNode node, Vector2 position)
        {
            foreach (MapNode neighbour in node.Neighbours)
            {
                NodePair pair = new NodePair(position, neighbour.Position);

                // Check if the connection is already drawn
                if (_drawnConnections.Contains(pair) || _drawnConnections.Contains(new NodePair(neighbour.Position, position)))
                {
                    continue;
                }

                GameObject lineObj = Instantiate(_settings.lineRendererPrefab, _systemMapParent);
                lineObj.layer = _mapLayer;
                LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();

                Vector3 startPosition = new Vector3(position.x, position.y, _settings.lineZ); // Adjust Z axis if needed
                Vector3 endPosition = new Vector3(neighbour.Position.x, neighbour.Position.y, _settings.lineZ); // Adjust Z axis if needed

                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, startPosition);
                lineRenderer.SetPosition(1, endPosition);

                // Add the pair to the HashSet
                _drawnConnections.Add(pair);
            }
        }
    }
}