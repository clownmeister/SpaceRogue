using SpaceRogue.Map.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceRogue.Map
{
    [RequireComponent(typeof(MapRenderer))]
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance;
        public delegate void NodeSelectedEventHandler(MapNode selectedNode);
        public event NodeSelectedEventHandler OnNodeSelected;

        public MapNode CurrentNode { get; set; }

        public MapNode SelectedNode { get; private set; }

        public int seed;
        public SystemMapSettings systemMapSettings;


        [Header("Gizmo settings")]
        public bool drawGizmos;
        public Color boundariesColor = Color.cyan;

        private SystemMap _systemMap;
        private MapRenderer _renderer;

        // Double tap
        private float _lastTapTime = 0f;
        private const float DOUBLE_TAP_INTERVAL = 0.3f;

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

            _systemMap = new SystemMap(systemMapSettings);
            _renderer = GetComponent<MapRenderer>();
        }

        public void RegenerateMap(int? seed = null)
        {
            _systemMap.Generate(seed ?? this.seed);
            InitCurrentNode();

            _renderer.Render(_systemMap);
        }

        private void InitCurrentNode()
        {
            CurrentNode = _systemMap.StartNode;
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
                _renderer.SetNodeColor(oldNode);
            }

            SelectedNode = selectedNode;
            _renderer.SetNodeColor(selectedNode);
        }
    }
}