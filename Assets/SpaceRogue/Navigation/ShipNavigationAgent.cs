using SpaceRogue.Navigation;
using System;
using UnityEngine;

namespace SpaceRogue
{
    public enum ShipMode
    {
        Parking,
        Turning,
        Travel,
        Idle
    }

    public enum AccelerationState
    {
        Accelerating,
        Decelerating,
        Idle
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class ShipNavigationAgent : MonoBehaviour, INavigationAgent
    {
        public float maxSpeed = 10f;
        public float accelerationRate = 2f;
        public float decelerationRate = 2f;

        public float rotationAcceleration = 60f;
        public float maxRotationSpeed = 50f;

        public bool parkingEnabled = true;
        public float parkingDistance = 3f;
        public float targetConeAngle = 70f;
        public float alignmentTolerance = 1.0f;
        public float stoppingDistanceTolerance = .1f;

        [SerializeField] private ShipMode currentMode = ShipMode.Idle;

        [SerializeField] private AccelerationState accelerationState = AccelerationState.Idle;
        private float _currentRotationSpeed;
        private float _currentSpeed;

        private Action<bool> _onFinishCallback;

        private Rigidbody2D _shipRigidbody;
        private Transform _target;
        private float _targetAngle;

        private void Start()
        {
            _shipRigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (_target != null)
            {
                MoveAndRotateShip();
            }
            else
            {
                DecelerateIfMoving();
            }
        }

        public void SetTarget(Vector3 position, Action<bool> onFinish = null)
        {
            CreateOrMoveTarget(position);
            _onFinishCallback = onFinish;
        }

        public void Stop()
        {
            Destroy(_target.gameObject);
            _target = null;
            OnPathFinished(false);
        }

        private void DecelerateIfMoving()
        {
            if (_shipRigidbody.velocity == Vector2.zero)
            {
                return;
            }

            Vector2 velocity = _shipRigidbody.velocity;
            accelerationState = AccelerationState.Decelerating;

            Vector2 decelerationVector = -velocity.normalized * (decelerationRate * Time.deltaTime);
            _shipRigidbody.velocity = Vector2.ClampMagnitude(velocity + decelerationVector, velocity.magnitude);

            if (_shipRigidbody.velocity.magnitude < 0.01f)
            {
                _shipRigidbody.velocity = Vector2.zero;
                _currentSpeed = 0;
                accelerationState = AccelerationState.Idle;
            }
        }

        private void MoveAndRotateShip()
        {
            if (_target == null)
            {
                currentMode = ShipMode.Idle;
                return;
            }

            Vector2 targetDirection = _target.position - transform.position;
            float distanceToTarget = targetDirection.magnitude;
            _targetAngle = Vector2.SignedAngle(transform.right, targetDirection);

            // Check if the target is hit
            if (IsTargetHit(distanceToTarget))
            {
                // _currentSpeed = 0;
                // _currentRotationSpeed = 0;

                Destroy(_target.gameObject); // Assuming you want to destroy the target
                _target = null;
                currentMode = ShipMode.Idle;
                accelerationState = AccelerationState.Idle;
                OnPathFinished();
                return; // Exit the method as the target is hit
            }

            // Update mode based on position and angle
            bool isInParkingZone = distanceToTarget <= parkingDistance;
            bool isInTargetCone = Mathf.Abs(_targetAngle) <= targetConeAngle;

            if (isInParkingZone && parkingEnabled)
            {
                currentMode = ShipMode.Parking;
                HandleParkingMode(distanceToTarget);
            }
            else if (!isInTargetCone)
            {
                currentMode = ShipMode.Turning;
                HandleTurningMode();
            }
            else
            {
                currentMode = ShipMode.Travel;
                HandleTravelMode(distanceToTarget);
            }

            // Rotation logic with alignment tolerance
            float rotationDirection = Mathf.Sign(_targetAngle);
            if (Mathf.Abs(_targetAngle) > alignmentTolerance)
            {
                _currentRotationSpeed = Mathf.Clamp(_currentRotationSpeed + rotationAcceleration * Time.deltaTime, 0, maxRotationSpeed);
                _shipRigidbody.rotation += _currentRotationSpeed * rotationDirection * Time.deltaTime;
            }
            else
            {
                // Within tolerance, reduce or reset rotation speed
                _currentRotationSpeed = 0;
            }

            // Apply the velocity
            _shipRigidbody.velocity = transform.right * _currentSpeed;
        }

        private void HandleParkingMode(float distanceToTarget)
        {
            // Calculate the distance needed to stop from the current speed
            float brakingDistance = CalculateBrakingDistance();

            // Check if the ship needs to start decelerating
            if (distanceToTarget <= brakingDistance && _currentSpeed > 0)
            {
                // Decelerate if within braking distance
                _currentSpeed = Mathf.Max(_currentSpeed - decelerationRate * Time.deltaTime, 0);
                accelerationState = AccelerationState.Decelerating;
            }
            else if (Mathf.Abs(_targetAngle) < 1.0f) // Threshold for alignment
            {
                // Accelerate towards the target once aligned
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, maxSpeed, accelerationRate * Time.deltaTime);
                accelerationState = AccelerationState.Accelerating;
            }
            else
            {
                // Maintain current speed if not aligned and not in braking distance
                accelerationState = AccelerationState.Idle;
            }
        }

        private void HandleTurningMode()
        {
            // Decelerate for turning
            _currentSpeed = Mathf.Max(_currentSpeed - decelerationRate * Time.deltaTime, 0);
            accelerationState = AccelerationState.Decelerating;
        }

        private void HandleTravelMode(float distanceToTarget)
        {
            // Accelerate or maintain speed, decelerate as needed
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, maxSpeed, accelerationRate * Time.deltaTime);
            accelerationState = AccelerationState.Accelerating;
            if (distanceToTarget <= CalculateBrakingDistance())
            {
                _currentSpeed = Mathf.Max(_currentSpeed - decelerationRate * Time.deltaTime, 0);
            }
        }

        private float CalculateBrakingDistance()
        {
            return (_currentSpeed * _currentSpeed) / (2 * decelerationRate);
        }

        private bool IsTargetHit(float distanceToTarget)
        {
            return distanceToTarget <= stoppingDistanceTolerance;
        }

        private void CreateOrMoveTarget(Vector3 position)
        {
            if (_target == null)
            {
                CreateTargetAtPosition(position);
            }
            else
            {
                _target.position = position;
            }
        }

        private void OnPathFinished(bool success = true)
        {
            if (_onFinishCallback == null) return;
            _onFinishCallback(success);
            _onFinishCallback = null;
        }

        private void CreateTargetAtPosition(Vector3 position)
        {
            GameObject targetObj = new GameObject("Target");
            _target = targetObj.transform;
            _target.position = position;
        }
    }
}