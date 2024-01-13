using SpaceRogue.Utility;
using System;
using UnityEngine;

namespace SpaceRogue.Navigation
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(ShipNavigationAgent))]
    public class CantabrianNavigator : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Approach,
            Orbit
        }

        public State currentState;

        [Header("Orbit Settings")] public GameObject targetShip;
        public float radius = 5f;
        public float orbitDistanceTolerance = 1f;
        public float nextWaypointAngle = 30f;

        [SerializeField]
        private float currentSpeed = 0f;

        [Header("Gizmos")] public bool drawGizmos = false;

        private readonly Color _orbitColor = Color.red;
        private readonly Color _tangentColor = Color.yellow;
        private readonly Color _toleranceColor = new Color(1, 0, 0, 0.3f);
        private readonly Color _waypointColor = Color.green;
        private Vector2? _navigationPoint = null;
        private Rigidbody2D _rigidBody;
        private ShipNavigationAgent _shipNavigationAgent;

        private void Start()
        {
            _shipNavigationAgent = GetComponent<ShipNavigationAgent>();
            _rigidBody = GetComponent<Rigidbody2D>();
            currentState = State.Idle;
        }

        private void Update()
        {
            HandleStateSwitch();
            switch (currentState)
            {
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

            if (!this.drawGizmos) return;
            // Draw orbit circle
            Gizmo.DrawCircle(this.targetShip.transform.position, this.radius, this._orbitColor);

            // Draw tolerance circles
            Gizmo.DrawCircle(this.targetShip.transform.position, this.radius - this.orbitDistanceTolerance, this._toleranceColor);
            Gizmo.DrawCircle(this.targetShip.transform.position, this.radius + this.orbitDistanceTolerance, this._toleranceColor);

            if (!this._navigationPoint.HasValue) return;
            // Draw navigation point
            Gizmos.color = this._waypointColor;
            Gizmos.DrawSphere(this._navigationPoint.Value, 0.2f);
            if (this.currentState == State.Idle) return;
            Gizmos.color = this._tangentColor;
            Gizmos.DrawLine(transform.position, this._navigationPoint.Value);
        }

        private void HandleStateSwitch()
        {
            if (targetShip != null && currentState == State.Idle)
            {
                currentState = State.Approach;
                _navigationPoint = null;
            }
        }

        private void HandleIdle()
        {
            _shipNavigationAgent.Stop();
        }

        private void HandleApproach()
        {
            if (targetShip == null) return;
            _navigationPoint ??= CalculateApproachPoint();

            if (!_navigationPoint.HasValue) return;
            _shipNavigationAgent.SetTarget(_navigationPoint.Value, OnNavigationFinished);

        }

        private void HandleOrbit()
        {
            if (targetShip == null) return;
            _navigationPoint ??= CalculateNextWaypoint();

            if (!_navigationPoint.HasValue) return;
            _shipNavigationAgent.SetTarget(_navigationPoint.Value, OnNavigationFinished);
        }

        private void OnNavigationFinished(bool success)
        {
            if (targetShip == null)
            {
                currentState = State.Idle;
                return;
            }

            float distanceToTargetShip = Vector2.Distance(transform.position, targetShip.transform.position);
            if (distanceToTargetShip > radius + orbitDistanceTolerance)
            {
                currentState = State.Approach;
            }
            else
            {
                currentState = success ? State.Orbit : State.Approach;
            }

            _navigationPoint = null;
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
            Vector2 orbitMovementDirection = _rigidBody.velocity.normalized;
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