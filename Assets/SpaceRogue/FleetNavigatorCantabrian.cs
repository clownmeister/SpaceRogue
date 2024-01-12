using System;
using SpaceRogue.Utility;
using UnityEngine;

namespace SpaceRogue
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FleetNavigatorCantabrian : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Approach,
            Orbit
        }

        public State currentState;

        [Header("Movement Settings")] public float maxSpeed = 5f;

        public float acceleration = 2f;
        public float rotationSpeed = 2f;

        [Header("Orbit Settings")] public GameObject targetShip;

        public float radius = 5f;
        public float orbitDistanceTolerance = 1f;
        public float nextWaypointAngle = 10f;

        [Header("Gizmos")] public bool drawGizmos = true;

        private float currentSpeed = 0f;

        private Vector2? navigationPoint = null; // Nullable for initial state
        private Color orbitColor = Color.red;
        private Rigidbody2D rigidBody;
        private Color tangentColor = Color.yellow;
        private Color toleranceColor = new Color(1, 0, 0, 0.3f);
        private Color waypointColor = Color.green;

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            currentState = State.Idle;
        }

        private void Update()
        {
            HandleStateSwitch();
            switch (currentState) {
                case State.Idle:
                    HandleIdle();
                    break;
                case State.Approach:
                    HandleApproach();
                    break;
                case State.Orbit:
                    HandleOrbit();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDrawGizmos()
        {
            if (targetShip == null) return;

            if (drawGizmos) {
                // Draw orbit circle
                Gizmo.DrawCircle(targetShip.transform.position, radius, orbitColor);

                // Draw tolerance circles
                Gizmo.DrawCircle(targetShip.transform.position, radius - orbitDistanceTolerance, toleranceColor);
                Gizmo.DrawCircle(targetShip.transform.position, radius + orbitDistanceTolerance, toleranceColor);

                if (navigationPoint.HasValue) {
                    // Draw navigation point
                    Gizmos.color = waypointColor;
                    Gizmos.DrawSphere(navigationPoint.Value, 0.2f);
                }
            }
        }

        private void HandleStateSwitch()
        {
            // If we have a target and we are not already approaching or orbiting, then start approaching
            if (targetShip != null && currentState == State.Idle) {
                currentState = State.Approach;
                navigationPoint = null; // Reset navigation point when we start approaching
            }
        }

        private void HandleIdle()
        {
            Decelerate();
        }

        private void HandleApproach()
        {
            if (targetShip == null) return;
            if (!navigationPoint.HasValue) {
                navigationPoint = CalculateApproachPoint();
            }

            MoveTowards(navigationPoint.Value);
            if (Vector2.Distance(transform.position, navigationPoint.Value) <= orbitDistanceTolerance) {
                currentState = State.Orbit;
                navigationPoint = null; // Reset navigation point when transitioning to orbit
            }
        }

        private void HandleOrbit()
        {
            if (targetShip == null) return;
            if (!navigationPoint.HasValue) {
                navigationPoint = CalculateNextWaypoint();
            }

            MoveTowards(navigationPoint.Value);
            if (Vector2.Distance(transform.position, navigationPoint.Value) <= orbitDistanceTolerance) {
                navigationPoint = CalculateNextWaypoint(); // Calculate new waypoint after reaching the current one
            }
        }

        private void MoveTowards(Vector2 point)
        {
            Vector2 direction = (point - this.rigidBody.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(this.rigidBody.rotation, targetAngle, rotationSpeed * Time.deltaTime);
            this.rigidBody.rotation = angle;
            Accelerate();
        }

        private void Accelerate()
        {
            if (currentSpeed < maxSpeed) {
                currentSpeed += acceleration * Time.deltaTime;
            }

            this.rigidBody.velocity = transform.right * currentSpeed;
        }

        private void Decelerate()
        {
            if (currentSpeed > 0) {
                currentSpeed -= acceleration * Time.deltaTime;
            }
            else {
                currentSpeed = 0;
            }

            this.rigidBody.velocity = transform.right * currentSpeed;
        }

        private Vector2 CalculateApproachPoint()
        {
            // Direction from the agent to the target ship's center
            Vector2 toCenter = (targetShip.transform.position - transform.position).normalized;

            // Calculate two perpendicular directions for each side of the target ship
            Vector2 perpendicularClockwise = new Vector2(-toCenter.y, toCenter.x);
            Vector2 perpendicularCounterClockwise = new Vector2(toCenter.y, -toCenter.x);

            // Calculate the two potential approach points
            Vector2 approachPointClockwise = (Vector2)targetShip.transform.position + perpendicularClockwise * radius;
            Vector2 approachPointCounterClockwise = (Vector2)targetShip.transform.position + perpendicularCounterClockwise * radius;

            // Determine which point is more in line with the agent's current forward direction
            Vector2 agentForward = transform.right; // Assuming the agent's forward direction is along its local right axis
            float dotClockwise = Vector2.Dot(agentForward, (approachPointClockwise - (Vector2)transform.position).normalized);
            float dotCounterClockwise = Vector2.Dot(agentForward, (approachPointCounterClockwise - (Vector2)transform.position).normalized);

            // Choose the point that has a higher dot product value (indicating it's more in the forward direction)
            return dotClockwise > dotCounterClockwise ? approachPointClockwise : approachPointCounterClockwise;
        }

        private Vector2 CalculateNextWaypoint()
        {
            Vector2 toNavigator = (Vector2)transform.position - (Vector2)targetShip.transform.position;
            float currentAngle = Mathf.Atan2(toNavigator.y, toNavigator.x) * Mathf.Rad2Deg;

            // Determine the direction of the agent's movement relative to the orbit's center
            Vector2 orbitMovementDirection = rigidBody.velocity.normalized;
            Vector2 toRight = new Vector2(toNavigator.y, -toNavigator.x).normalized; // Right-hand perpendicular to the toNavigator

            // Determine if the movement is more clockwise or counter-clockwise
            bool isClockwise = Vector2.Dot(orbitMovementDirection, toRight) > 0;

            // Adjust the angle for the next waypoint based on the direction and angle increment
            float angleIncrement = isClockwise ? -nextWaypointAngle : nextWaypointAngle;
            float nextAngle = (currentAngle + angleIncrement) * Mathf.Deg2Rad;

            // Calculate the next waypoint
            Vector2 nextWaypoint = (Vector2)targetShip.transform.position + new Vector2(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * radius;
            return nextWaypoint;
        }
    }
}