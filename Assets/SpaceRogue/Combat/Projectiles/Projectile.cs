using UnityEngine;

namespace SpaceRogue.Combat.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public float physicalDamage;
        public float energyDamage;
        public bool canPenetrateShields;

        private void OnCollisionEnter(Collision collision)
        {
            IDamageable damageable = collision.collider.GetComponent<IDamageable>();
            switch (damageable)
            {
                case null:
                    return;
                case Shield:
                    {
                        damageable.TakeDamage(energyDamage);
                        if (!canPenetrateShields)
                        {
                            Die();
                        }

                        break;
                    }
                case Hull:
                    damageable.TakeDamage(physicalDamage);
                    break;
            }
        }

        private void Die()
        {
            //TODO: death animation, IDeathAnimator
            Destroy(gameObject);
        }
    }
}