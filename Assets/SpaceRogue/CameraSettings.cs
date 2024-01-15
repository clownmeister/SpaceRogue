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

        [Header("Inertia")]
        public float inertiaDuration = 0.5f;  // Duration of inertia effect
        public float minInertiaSpeed = 70f;   // Minimum speed to consider for inertia
        public float inertiaVelocityModifier = .1f;
        public float inertiaLimit = 10f;

    }
}