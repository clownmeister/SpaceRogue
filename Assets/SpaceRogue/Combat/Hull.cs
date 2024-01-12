using UnityEngine;

namespace SpaceRogue.Combat
{
    public class Hull : MonoBehaviour, IDamageable
    {
        public float maxHullIntegrity = 100;
        public float hullIntegrity = 100;

        public void TakeDamage(float damageAmount)
        {
            this.hullIntegrity = Mathf.Max(hullIntegrity - damageAmount, 0);
            if (hullIntegrity <= 0)
            {
                Die();
            }
        }

        public float CurrentHitPoints
        {
            get { return hullIntegrity; }
        }

        public float MaxHitPoints
        {
            get { return maxHullIntegrity; }
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}