using UnityEngine;

namespace SpaceRogue
{
    public class CameraController : MonoBehaviour
    {
        public Vector2 panLimit;
        public float minZoom = 5f;
        public float maxZoom = 20f;

        public CameraSettings cameraSettings;

        private Camera _cam;

        // Current movement speed based on zoom
        private float _currentMoveSpeed;
        private Vector2 _lastTouchDelta;
        private Vector2 _inertiaVelocity;
        private float _inertiaDuration = 0.5f;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
        }

        private void Update()
        {
            Vector3 pos = transform.position;

            // Update current movement speed based on zoom
            if (Application.isMobilePlatform)
            {
                float zoomFactor = (_cam.orthographicSize - minZoom) / (maxZoom - minZoom);
                _currentMoveSpeed = Mathf.Lerp(cameraSettings.mobileMinPanSpeed, cameraSettings.mobileMaxPanSpeed, zoomFactor);
            }
            else
            {
                _currentMoveSpeed = cameraSettings.pcPanModifier;
            }

            HandleKeyboardMovement(ref pos);
            HandleZoom(ref pos);
            HandleMobileControls(ref pos);

            pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
            pos.y = Mathf.Clamp(pos.y, -panLimit.y, panLimit.y);
            transform.position = pos;
        }

        private void HandleKeyboardMovement(ref Vector3 pos)
        {
            float speed = Application.isMobilePlatform ? _currentMoveSpeed : cameraSettings.pcPanModifier;

            if (Input.GetKey("w"))
            {
                pos.y += speed * Time.deltaTime;
            }

            if (Input.GetKey("s"))
            {
                pos.y -= speed * Time.deltaTime;
            }

            if (Input.GetKey("d"))
            {
                pos.x += speed * Time.deltaTime;
            }

            if (Input.GetKey("a"))
            {
                pos.x -= speed * Time.deltaTime;
            }
        }

        private void HandleZoom(ref Vector3 pos)
        {
            float zoom = Input.GetAxis("Mouse ScrollWheel");
            _cam.orthographicSize -= zoom * cameraSettings.zoomSpeed * Time.deltaTime;
            _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, minZoom, maxZoom);
        }

        private void HandleMobileControls(ref Vector3 pos)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                pos.x -= touch.deltaPosition.x * _currentMoveSpeed * Time.deltaTime;
                pos.y -= touch.deltaPosition.y * _currentMoveSpeed * Time.deltaTime;
            }
            else if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                _cam.orthographicSize -= difference * cameraSettings.mobileZoomSpeed * Time.deltaTime;
                _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, minZoom, maxZoom);
            }
        }
    }
}