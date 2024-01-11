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
    public class ShipController : MonoBehaviour
    {
        public float maxSpeed = 10f;
        public float accelerationRate = 2f;
        public float decelerationRate = 2f;
        public float rotationAcceleration = 60f;
        public float maxRotationSpeed = 50f;
        public float parkingDistance = 3f;
        public float targetConeAngle = 70f;
        public float alignmentTolerance = 1.0f;

        [SerializeField] private ShipMode currentMode = ShipMode.Idle;

        [SerializeField] private AccelerationState accelerationState = AccelerationState.Idle;
        private float currentRotationSpeed = 0f;
        private float currentSpeed = 0f;

        private Rigidbody2D shipRigidbody;
        private Transform target;
        private float targetAngle;


        void Start()
        {
            shipRigidbody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (target != null) {
                MoveAndRotateShip();
            }
            else {
                DecelerateIfMoving();
            }

            HandleMouseInput();
        }

        void DecelerateIfMoving()
        {
            if (shipRigidbody.velocity.magnitude > 0) {
                Vector2 decelerationVector = -this.shipRigidbody.velocity.normalized * (this.decelerationRate * Time.deltaTime);
                shipRigidbody.velocity = Vector2.ClampMagnitude(shipRigidbody.velocity + decelerationVector, shipRigidbody.velocity.magnitude);

                if (shipRigidbody.velocity.magnitude < 0.01f) // Threshold to stop completely
                {
                    shipRigidbody.velocity = Vector2.zero;
                    currentSpeed = 0;
                    accelerationState = AccelerationState.Idle;
                }
                else {
                    accelerationState = AccelerationState.Decelerating;
                }
            }
        }

        void MoveAndRotateShip()
        {
            if (target == null) {
                currentMode = ShipMode.Idle;
                return; // Exit early if there's no target
            }

            Vector2 targetDirection = (Vector2)(target.position - transform.position);
            float distanceToTarget = targetDirection.magnitude;
            targetAngle = Vector2.SignedAngle(transform.right, targetDirection);

            // Check if the target is hit
            if (IsTargetHit(distanceToTarget)) {
                currentSpeed = 0;
                currentRotationSpeed = 0;
                Destroy(target.gameObject); // Assuming you want to destroy the target
                target = null;
                currentMode = ShipMode.Idle;
                accelerationState = AccelerationState.Idle;
                return; // Exit the method as the target is hit
            }

            // Update mode based on position and angle
            bool isInParkingZone = distanceToTarget <= parkingDistance;
            bool isInTargetCone = Mathf.Abs(targetAngle) <= targetConeAngle;

            if (isInParkingZone) {
                currentMode = ShipMode.Parking;
                HandleParkingMode(distanceToTarget);
            }
            else if (!isInTargetCone) {
                currentMode = ShipMode.Turning;
                HandleTurningMode();
            }
            else {
                currentMode = ShipMode.Travel;
                HandleTravelMode(distanceToTarget);
            }

            // Rotation logic with alignment tolerance
            float rotationDirection = Mathf.Sign(targetAngle);
            if (Mathf.Abs(targetAngle) > alignmentTolerance) {
                currentRotationSpeed = Mathf.Clamp(currentRotationSpeed + rotationAcceleration * Time.deltaTime, 0, maxRotationSpeed);
                shipRigidbody.rotation += currentRotationSpeed * rotationDirection * Time.deltaTime;
            }
            else {
                // Within tolerance, reduce or reset rotation speed
                currentRotationSpeed = 0;
            }

            // Apply the velocity
            shipRigidbody.velocity = transform.right * currentSpeed;
        }

        void HandleParkingMode(float distanceToTarget)
        {
            // Calculate the distance needed to stop from the current speed
            float brakingDistance = CalculateBrakingDistance();

            // Check if the ship needs to start decelerating
            if (distanceToTarget <= brakingDistance && currentSpeed > 0) {
                // Decelerate if within braking distance
                currentSpeed = Mathf.Max(currentSpeed - decelerationRate * Time.deltaTime, 0);
                accelerationState = AccelerationState.Decelerating;
            }
            else if (Mathf.Abs(targetAngle) < 1.0f) // Threshold for alignment
            {
                // Accelerate towards the target once aligned
                currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, accelerationRate * Time.deltaTime);
                accelerationState = AccelerationState.Accelerating;
            }
            else {
                // Maintain current speed if not aligned and not in braking distance
                accelerationState = AccelerationState.Idle;
            }
        }

        void HandleTurningMode()
        {
            // Decelerate for turning
            currentSpeed = Mathf.Max(currentSpeed - decelerationRate * Time.deltaTime, 0);
            accelerationState = AccelerationState.Decelerating;
        }

        void HandleTravelMode(float distanceToTarget)
        {
            // Accelerate or maintain speed, decelerate as needed
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, accelerationRate * Time.deltaTime);
            accelerationState = AccelerationState.Accelerating;
            if (distanceToTarget <= CalculateBrakingDistance()) {
                currentSpeed = Mathf.Max(currentSpeed - decelerationRate * Time.deltaTime, 0);
            }
        }

        float CalculateBrakingDistance()
        {
            return (currentSpeed * currentSpeed) / (2 * decelerationRate);
        }

        private bool IsTargetHit(float distanceToTarget)
        {
            return distanceToTarget <= 0.1f;
        }

        void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0)) {
                // This is triggered the moment the button is pressed down
                Vector3 mouseWorldPosition = GetMouseWorldPosition();
                CreateOrMoveTarget(mouseWorldPosition);
            }
            else if (Input.GetMouseButton(0)) {
                // This is triggered every frame the button is held down
                Vector3 mouseWorldPosition = GetMouseWorldPosition();
                MoveTarget(mouseWorldPosition);
            }
        }

        Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;
            return mouseWorldPosition;
        }

        void CreateOrMoveTarget(Vector3 position)
        {
            if (target == null) {
                CreateTargetAtPosition(position);
            }
            else {
                target.position = position;
            }
        }

        void MoveTarget(Vector3 position)
        {
            if (target != null) {
                target.position = position;
            }
        }

        void CreateTargetAtPosition(Vector3 position)
        {
            GameObject targetObj = new GameObject("Target");
            target = targetObj.transform;
            target.position = position;
        }
    }
}