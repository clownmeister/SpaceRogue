using UnityEngine;

namespace SpaceRogue
{
    public class CameraController : MonoBehaviour
    {
        public float panSpeed = 10f;
        public float panBorderThickness = 10f;
        public Vector2 panLimit;
        public float zoomSpeed = 20000f;
        public float minZoom = 5f;
        public float maxZoom = 20f;

        private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 pos = transform.position;

            // Keyboard movement
            // if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness) {
            if (Input.GetKey("w")) {
                pos.y += panSpeed * Time.deltaTime;
            }

            // if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness) {
            if (Input.GetKey("s")) {
                pos.y -= panSpeed * Time.deltaTime;
            }

            // if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness) {
            if (Input.GetKey("d")) {
                pos.x += panSpeed * Time.deltaTime;
            }

            // if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness) {
            if (Input.GetKey("a")) {
                pos.x -= panSpeed * Time.deltaTime;
            }

            // Zoom adjustment for orthographic camera
            float zoom = Input.GetAxis("Mouse ScrollWheel");
            cam.orthographicSize -= zoom * zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

            // Mobile touch movement and pinch zoom
            if (Input.touchCount == 1) {
                Touch touch = Input.GetTouch(0);
                pos.x += touch.deltaPosition.x * panSpeed * Time.deltaTime;
                pos.y += touch.deltaPosition.y * panSpeed * Time.deltaTime;
            }
            else if (Input.touchCount == 2) {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                cam.orthographicSize += difference * zoomSpeed * Time.deltaTime;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
            }

            // Apply position limits
            pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
            pos.y = Mathf.Clamp(pos.y, -panLimit.y, panLimit.y);

            transform.position = pos;
        }
    }
}