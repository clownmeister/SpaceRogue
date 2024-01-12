using UnityEngine;

namespace SpaceRogue.Navigation
{
    [RequireComponent(typeof(ShipNavigationAgent))]
    public class PlayerShipNavigator : MonoBehaviour
    {
        private ShipNavigationAgent _shipNavigationAgent;

        void Start()
        {
            _shipNavigationAgent = GetComponent<ShipNavigationAgent>();
        }

        void Update()
        {
            HandleMouseInput();
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                Vector3 mouseWorldPosition = GetMouseWorldPosition();
                _shipNavigationAgent.SetTarget(mouseWorldPosition);
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;
            return mouseWorldPosition;
        }
    }
}