using UnityEngine;

namespace SpaceRogue
{
    [CreateAssetMenu(fileName = "New Camera Settings", menuName = "Camera/CameraSettings", order = 100)]
    public class CameraSettings : ScriptableObject
    {
        [Header("PC settings")]
        public float zoomSpeed = 20000f;
        public float pcPanModifier = 10f;
        [Header("Mobile")]
        public float mobileMinPanSpeed = 0.3f;
        public float mobileMaxPanSpeed = 1f;
        public float mobileZoomSpeed = .5f;
    }
}