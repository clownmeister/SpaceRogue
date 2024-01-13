using UnityEngine;

namespace SpaceRogue.Combat
{
    public class TargetingUnit : MonoBehaviour
    {
        private Turret _turret;
        private void Start()
        {
            _turret = GetComponent<Turret>();
        }

        public Hull GetClosestEnemyHull()
        {
            Hull[] allHulls = FindObjectsOfType<Hull>();
            Hull closestHull = null;
            float closestDistance = Mathf.Infinity;
            Vector2 currentPosition = transform.position;

            foreach (Hull hull in allHulls)
            {
                if (hull.gameObject.layer == gameObject.layer) continue;
                float distance = Vector2.Distance(currentPosition, hull.transform.position);
                if (distance > closestDistance) continue;
                if (distance > _turret.maxRange) continue;
                closestHull = hull;
                closestDistance = distance;
            }

            return closestHull;
        }
    }
}