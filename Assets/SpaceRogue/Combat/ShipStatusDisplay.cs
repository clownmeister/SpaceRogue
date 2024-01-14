using UnityEngine;
using UnityEngine.UI;

namespace SpaceRogue.Combat
{
    [RequireComponent(typeof(Hull))]
    public class ShipStatusDisplay : MonoBehaviour
    {
        public float yOffset = -1;
        public float ySpacing = .12f;
        public Canvas canvas; // World Space Canvas
        private readonly Color _hullBarColor = Color.green;
        private readonly Color _shieldBarColor = Color.blue;
        private bool _hasShield;
        private Image _hullBar;
        private Image _shieldBar;
        private Hull _shipHull;
        private Shield _shipShield;

        private void Start()
        {
            _shipHull = GetComponent<Hull>();
            _shipShield = GetComponentInChildren<Shield>();
            _hasShield = _shipShield != null;

            _hullBar = CreateStatusBar("HullBar", _hullBarColor);
            if (_hasShield)
            {
                _shieldBar = CreateStatusBar("ShieldBar", _shieldBarColor);
            }

            UpdatePosition();
        }

        private void Update()
        {
            if (_hasShield && _shipShield != null)
            {
                UpdateBarSize(_shieldBar, _shipShield.CurrentHitPoints, _shipShield.MaxHitPoints);
            }

            if (_hullBar != null && _shipHull != null)
            {
                UpdateBarSize(_hullBar, _shipHull.CurrentHitPoints, _shipHull.MaxHitPoints);
            }

            UpdatePosition();
        }

        public void OnDestroy()
        {
            Destroy(this._hullBar);
            Destroy(this._shieldBar);
        }

        private Image CreateStatusBar(string name, Color color)
        {
            GameObject statusBar = new GameObject(name, typeof(Image));
            statusBar.transform.SetParent(canvas.transform, false);

            Image image = statusBar.GetComponent<Image>();
            image.color = color;

            RectTransform rect = statusBar.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(50, 3); // Default full size, adjust as needed
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

            return image;
        }

        private void UpdateBarSize(Image bar, float current, float max)
        {
            RectTransform rect = bar.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(current / max * 50, rect.sizeDelta.y);
        }

        private void UpdatePosition()
        {
            Vector3 shipPosition = transform.position;
            shipPosition.y += yOffset;

            if (_hasShield && _shieldBar != null)
            {
                RectTransform shieldBarRect = _shieldBar.GetComponent<RectTransform>();
                shieldBarRect.position = shipPosition;
                shipPosition.y -= ySpacing;
            }

            if (_hullBar != null)
            {
                _hullBar.GetComponent<RectTransform>().position = shipPosition;
            }
        }
    }
}