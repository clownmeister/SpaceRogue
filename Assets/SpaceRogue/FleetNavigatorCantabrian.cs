using UnityEngine;

namespace SpaceRogue
{
    public class FleetNavigatorCantabrian : MonoBehaviour
    {
        public Transform target;
        public float radius = 5.0f;
        public float speed = 5.0f;
        public float rotationSpeed = 90.0f; // Degrees per second
        private State currentState;

        private Vector3 targetOffset;

        void Start()
        {
            if (target != null) {
                ChangeState(State.Approaching);
            }
        }

        void Update()
        {
            if (target == null) return;

            switch (currentState) {
                case State.Approaching:
                    ApproachTarget();
                    break;
                case State.Orbiting:
                    OrbitTarget();
                    break;
            }
        }

        private void ChangeState(State newState)
        {
            currentState = newState;
            if (newState == State.Orbiting) {
                targetOffset = (transform.position - target.position).normalized * radius;
            }
        }

        private void ApproachTarget()
        {
            Vector3 tangentPoint = CalculateTangentPoint();
            MoveTowards(tangentPoint);

            if (Vector3.Distance(transform.position, tangentPoint) < 0.1f) {
                ChangeState(State.Orbiting);
            }
        }

        private Vector3 CalculateTangentPoint()
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Vector3 perpendicularDirection = new Vector3(-directionToTarget.y, directionToTarget.x, 0);
            Vector3 tangentPoint = target.position + perpendicularDirection * radius;

            return tangentPoint;
        }

        private void MoveTowards(Vector3 position)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);

            Vector3 direction = (position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle - 90), rotationSpeed * Time.deltaTime);
        }

        private void OrbitTarget()
        {
            float timeSinceStart = Time.timeSinceLevelLoad;
            Vector3 orbitPosition = target.position + new Vector3(
                Mathf.Cos(timeSinceStart * speed) * radius,
                Mathf.Sin(timeSinceStart * speed) * radius,
                0);

            transform.position = Vector3.MoveTowards(transform.position, orbitPosition, speed * Time.deltaTime);

            Vector3 direction = (orbitPosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            if (Vector3.Distance(target.position, transform.position) > radius * 1.5f) {
                ChangeState(State.Approaching);
            }
        }

        private enum State
        {
            Approaching,
            Orbiting
        }
    }
}