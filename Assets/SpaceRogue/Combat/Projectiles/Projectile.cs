using UnityEngine;

namespace SpaceRogue.Combat.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public float physicalDamage;
        public float energyDamage;
        public bool canPenetrateShields;

        private void OnTriggerEnter2D(Collider2D other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            switch (damageable)
            {
                case null:
                    return;
                case Shield:
                    {
                        if (damageable.CurrentHitPoints <= 0) break;
                        damageable.TakeDamage(energyDamage);
                        if (!canPenetrateShields)
                        {
                            Die();
                        }

                        break;
                    }
                case Hull:
                    if (!canPenetrateShields)
                    {
                        Shield shield = other.GetComponentInChildren<Shield>();
                        if (shield != null && shield.CurrentHitPoints > 0)
                        {
                            shield.TakeDamage(energyDamage);
                            Die();
                        }
                    }

                    damageable.TakeDamage(physicalDamage);
                    break;
            }
        }

        private void Die()
        {
            //TODO: death animation, IDeathAnimator
            Destroy(this.transform.gameObject);
        }
    }
}