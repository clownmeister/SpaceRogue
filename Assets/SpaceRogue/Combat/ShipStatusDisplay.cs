using UnityEngine;
using UnityEngine.UI;

namespace SpaceRogue.Combat
{
    [RequireComponent(typeof(Hull))]
    public class ShipStatusDisplay : MonoBehaviour
    {
        public float yOffset = -1;
        public Canvas canvas; // World Space Canvas
        private readonly Color _hullBarColor = Color.green;
        //TODO: fix
        // private Color shieldBarColor = new Color(0,150,255,1);
        // private Color hullBarColor = new Color(0,171, 8,1);
        private readonly Color _shieldBarColor = Color.blue;
        private bool _hasShield;
        private Slider _hullBar;
        private Slider _shieldBar;
        private Hull _shipHull;

        private Shield _shipShield;

        private void Start()
        {
            _shipHull = GetComponent<Hull>();
            _shipShield = GetComponent<Shield>();
            _hasShield = _shipShield != null;

            _hullBar = CreateStatusBar("HullBar", _hullBarColor);
            _hullBar.maxValue = _shipHull.hullIntegrity;

            if (_hasShield)
            {
                _shieldBar = CreateStatusBar("ShieldBar", _shieldBarColor);
                _shieldBar.maxValue = _shipShield.shieldStrength;
            }

            UpdatePosition();
        }

        private void Update()
        {
            // Check if the Shield component exists and update its status
            if (_hasShield && _shipShield != null)
            {
                _shieldBar.value = _shipShield.CurrentHitPoints;
            }

            // Update the Hull status, checking if _hullBar and _shipHull are not null
            if (_hullBar != null && _shipHull != null)
            {
                _hullBar.value = _shipHull.CurrentHitPoints;
            }
            else
            {
                Debug.LogError("HullBar or ShipHull is null!");
            }

            UpdatePosition();
        }

        private Slider CreateStatusBar(string name, Color color)
        {
            // Create the main slider object
            GameObject statusBar = new GameObject(name, typeof(Image));
            statusBar.transform.SetParent(canvas.transform, false); // Set as child of the canvas

            // Add the Slider component
            Slider slider = statusBar.AddComponent<Slider>();
            slider.interactable = false; // Make the slider non-interactive

            // Create the Fill Area
            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(statusBar.transform, false);

            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.5f);
            fillAreaRect.anchorMax = new Vector2(1, 0.5f);
            fillAreaRect.pivot = new Vector2(0.5f, 0.5f);
            fillAreaRect.sizeDelta = new Vector2(0, 3); // Height of the fill area

            // Create the Fill
            GameObject fill = new GameObject("Fill", typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);

            Image fillImage = fill.GetComponent<Image>();
            fillImage.color = color;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0, 0.5f);
            fillRect.anchorMax = new Vector2(1, 0.5f);
            fillRect.pivot = new Vector2(0, 0.5f);
            fillRect.sizeDelta = new Vector2(0, 0); // Initial size of the fill

            // Configure slider properties
            slider.fillRect = fillRect;
            slider.direction = Slider.Direction.LeftToRight;

            // Configure the main slider RectTransform
            RectTransform statusBarRect = statusBar.GetComponent<RectTransform>();
            statusBarRect.sizeDelta = new Vector2(50, 3); // Default size, adjust as needed
            statusBarRect.anchorMin = statusBarRect.anchorMax = statusBarRect.pivot = new Vector2(0.5f, 0.5f);

            return slider;
        }

        private void UpdatePosition()
        {
            Vector3 shipPosition = transform.position;
            shipPosition.y += yOffset;

            if (_hasShield && _shieldBar != null)
            {
                RectTransform shieldBarRect = _shieldBar.GetComponent<RectTransform>();
                shieldBarRect.position = shipPosition;
                shipPosition.y -= .1f; // Assuming 5 is the desired offset in world units
            }

            if (_hullBar != null)
            {
                _hullBar.GetComponent<RectTransform>().position = shipPosition;
            }
        }
    }
}