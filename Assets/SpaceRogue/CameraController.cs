using UnityEngine;

namespace SpaceRogue
{
    public class CameraController : MonoBehaviour
    {
        public Vector2 panLimit;
        public float zoomSpeed = 20000f;
        public float minZoom = 5f;
        public float maxZoom = 20f;

        // Mobile specific settings
        public float mobileMinPanSpeed = 0.3f;
        public float mobileMaxPanSpeed = 1f;
        public float mobileZoomSpeed = .5f;

        // PC specific settings
        public float pcPanModifier = 10f;

        private Camera _cam;

        // Current movement speed based on zoom
        private float _currentMoveSpeed;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
        }

        void Update()
        {
            Vector3 pos = transform.position;

            // Update current movement speed based on zoom
            if (Application.isMobilePlatform)
            {
                float zoomFactor = (_cam.orthographicSize - minZoom) / (maxZoom - minZoom);
                _currentMoveSpeed = Mathf.Lerp(mobileMinPanSpeed, mobileMaxPanSpeed, zoomFactor);
            }
            else
            {
                _currentMoveSpeed = pcPanModifier;
            }

            HandleKeyboardMovement(ref pos);
            HandleZoom(ref pos);
            HandleMobileControls(ref pos);

            pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
            pos.y = Mathf.Clamp(pos.y, -panLimit.y, panLimit.y);
            transform.position = pos;
        }

        void HandleKeyboardMovement(ref Vector3 pos)
        {
            float speed = Application.isMobilePlatform ? _currentMoveSpeed : pcPanModifier;

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

        void HandleZoom(ref Vector3 pos)
        {
            float zoom = Input.GetAxis("Mouse ScrollWheel");
            _cam.orthographicSize -= zoom * zoomSpeed * Time.deltaTime;
            _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, minZoom, maxZoom);
        }

        void HandleMobileControls(ref Vector3 pos)
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

                _cam.orthographicSize -= difference * mobileZoomSpeed * Time.deltaTime;
                _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, minZoom, maxZoom);
            }
        }
    }
}