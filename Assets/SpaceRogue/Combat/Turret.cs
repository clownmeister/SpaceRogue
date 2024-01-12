using UnityEngine;

namespace SpaceRogue.Combat
{
    public class Turret : MonoBehaviour
    {
        public GameObject projectilePrefab;
        public float projectileSpeed = 20f;
        public float projectileLifespan = 5f;
        public Camera mainCamera;
        public float rotationSpeed = 5f;
        public float accuracy = 1f; // Accuracy between 0 and 1
        public float accuracySpread = 5f; // Accuracy between 0 and 1
        public float fireRate = 1f; // Number of shots per second
        private float _nextFireTime = 0f;

        private Quaternion _targetRotation;

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                SetTargetRotation();
                RotateTurret();

                if (Quaternion.Angle(transform.rotation, _targetRotation) < 0.1f && Time.time >= _nextFireTime)
                {
                    Shoot();
                    _nextFireTime = Time.time + 1f / fireRate;
                }
            }
        }

        void SetTargetRotation()
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.y));
            mousePosition.z = transform.position.z;
            Vector3 aimDirection = (mousePosition - transform.position).normalized;
            aimDirection.z = 0;

            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            _targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        void RotateTurret()
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
        }

        void Shoot()
        {
            Quaternion projectileRotation = ApplyAccuracy(transform.rotation);
            GameObject projectile = Instantiate(projectilePrefab, transform.position, projectileRotation);
            projectile.GetComponent<Rigidbody2D>().velocity = projectileRotation * Vector2.right * projectileSpeed;
            Destroy(projectile, projectileLifespan);
        }

        Quaternion ApplyAccuracy(Quaternion originalRotation)
        {
            float deviation = (1 - accuracy) * Random.Range(-accuracySpread, accuracySpread); // Deviation based on accuracy
            return originalRotation * Quaternion.Euler(0, 0, deviation);
        }
    }
}