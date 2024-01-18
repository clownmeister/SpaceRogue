using System;
using SpaceRogue.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceRogue.Combat
{
    public class Turret : MonoBehaviour
    {
        public const string PROJECTILE_PARENT_NAME = "Projectiles";
        public GameObject projectilePrefab;
        public float maxRange = 10;
        public float projectileSpeed = 10f;
        public Camera mainCamera;
        public float rotationSpeed = 100f;
        public float accuracy = 1f; // Accuracy between 0 and 1
        public float accuracySpread = 5f; // Accuracy between 0 and 1
        public float fireRate = 1f; // Number of shots per second

        [Header("Gizmos")] public bool drawGizmos = false;
        private readonly Color _rangeColor = new Color(0, 255, 0, .7f);
        private Hull _currentTarget;
        private float _nextFireTime = 0f;

        private TargetingUnit _targetingUnit;
        private Quaternion _targetRotation;
        private Transform _projectileParent;

        private void Start()
        {
            _targetingUnit = GetComponent<TargetingUnit>();
            _projectileParent = GetProjectileParent();
        }

        private static Transform GetProjectileParent()
        {
            GameObject parent = GameObject.Find(PROJECTILE_PARENT_NAME);
            if (parent == null)
            {
                parent = new GameObject(PROJECTILE_PARENT_NAME);
            }

            // ToggleOnSceneChange toggleComponent = parent.AddComponent<ToggleOnSceneChange>();
            // toggleComponent.TargetSceneState = SceneState.Game;

            return parent.transform;
        }

        private void Update()
        {
            if (_targetingUnit == null)
            {
                gameObject.SetActive(false);
                throw new Exception("Turret is missing a targeting unit");
            }

            _currentTarget = _targetingUnit.GetClosestEnemyHull();
            if (_currentTarget == null) return;

            SetTargetRotation();
            RotateTurret();

            if (Quaternion.Angle(transform.rotation, _targetRotation) < 0.1f && Time.time >= _nextFireTime)
            {
                Shoot();
                _nextFireTime = Time.time + 1f / fireRate;
            }
        }

        private void OnDrawGizmos()
        {
            if (!this.drawGizmos) return;
            Gizmo.DrawCircle(transform.position, this.maxRange, this._rangeColor);
        }

        private void SetTargetRotation()
        {
            Vector2 targetPosition = _currentTarget.transform.position;
            Vector2 position = transform.position;

            //Prediction
            float distance = Vector2.Distance(position, targetPosition);
            float travelTime = distance / projectileSpeed;
            Vector2 predictPosition = targetPosition + _currentTarget.GetComponent<Rigidbody2D>().velocity * travelTime;

            //Use prediction or target position here
            //TODO: probably optimize
            Vector2 aimDirection = (predictPosition - position).normalized;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            _targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        private void RotateTurret()
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
        }

        private void Shoot()
        {
            Quaternion projectileRotation = ApplyAccuracy(transform.rotation);
            GameObject projectile = Instantiate(projectilePrefab, transform.position, projectileRotation, _projectileParent);
            projectile.layer = gameObject.layer;
            projectile.GetComponent<Rigidbody2D>().velocity = projectileRotation * Vector2.right * projectileSpeed;
            Destroy(projectile, maxRange / projectileSpeed);
        }

        private Quaternion ApplyAccuracy(Quaternion originalRotation)
        {
            float deviation = (1 - accuracy) * Random.Range(-accuracySpread, accuracySpread); // Deviation based on accuracy
            return originalRotation * Quaternion.Euler(0, 0, deviation);
        }
    }
}