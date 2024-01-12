using UnityEngine;

namespace SpaceRogue.Utility
{
    public static class Gizmo
    {
        public static void DrawCircle(Vector3 center, float radius, Color color)
        {
            const int segmentCount = 40;
            Gizmos.color = color;
            Vector3 lastPoint = center + new Vector3(radius, 0, 0);
            const float angleStep = 360f / segmentCount;
            for (int i = 1; i <= segmentCount; i++) {
                float currentAngle = angleStep * i;
                Vector3 currentPoint = center + new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0) * radius;
                Gizmos.DrawLine(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }
        }
    }
}