using SpaceRogue.Map.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceRogue.Map
{
    [RequireComponent(typeof(MapRenderer))]
    public class MapManager : MonoBehaviour
    {
        public delegate void NodeSelectedEventHandler(MapNode selectedNode);
        public event NodeSelectedEventHandler OnNodeSelected;

        private const float DOUBLE_TAP_INTERVAL = 0.3f;
        public static MapManager Instance;

        public int seed;
        public SystemMapSettings systemMapSettings;

        [Header("Gizmo settings")]
        public bool drawGizmos;
        public Color boundariesColor = Color.cyan;

        // Double tap
        private float _lastTapTime;
        private MapRenderer _renderer;

        private Map _map;

        public MapNode CurrentNode { get; set; }

        public MapNode SelectedNode { get; private set; }

        public bool IsConnectedToCurrent(MapNode potentialNeighbour)
        {
            if (CurrentNode == null)
            {
                return false;
            }

            return potentialNeighbour != CurrentNode && CurrentNode.IsConnected(potentialNeighbour);
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

            _map = new Map(systemMapSettings);
            _renderer = GetComponent<MapRenderer>();
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
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            Gizmos.color = boundariesColor;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(systemMapSettings.mapSize.x * 2, systemMapSettings.mapSize.y * 2, 0));
        }

        public void Jump()
        {
            Debug.Log(CurrentNode);
            Debug.Log(SelectedNode);
            if (!IsConnectedToCurrent(SelectedNode))
            {
                Debug.LogWarning("Tried to jump to not connected node.");
                return;
            }

            MapNode oldNode = CurrentNode;
            CurrentNode = SelectedNode;

            _renderer.UpdateNodeColor(oldNode);
            _renderer.UpdateNodeColor(CurrentNode);
        }

        public void RegenerateMap(int? seed = null)
        {
            SelectedNode = null;
            _map.Generate(seed ?? this.seed);
            InitCurrentNode();

            _renderer.Render(_map);
        }

        private void InitCurrentNode()
        {
            CurrentNode = _map.StartNode;
        }

        public void HandleNodeSelection(MapNode selectedNode)
        {
            if (selectedNode == SelectedNode) return;
            OnNodeSelected?.Invoke(selectedNode);
            SelectNode(selectedNode);
        }

        private void SelectNode(MapNode selectedNode)
        {
            if (SelectedNode != null)
            {
                MapNode oldNode = SelectedNode;
                SelectedNode = null;
                _renderer.UpdateNodeColor(oldNode);
            }

            SelectedNode = selectedNode;
            _renderer.UpdateNodeColor(selectedNode);
        }
    }
}