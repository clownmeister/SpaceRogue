using UnityEngine;

namespace SpaceRogue.Combat
{
    public class Shield : MonoBehaviour, IDamageable
    {
        public float maxShieldStrength = 100;
        public float shieldStrength = 100;

        public void TakeDamage(float damageAmount)
        {
            this.shieldStrength = Mathf.Max(shieldStrength - damageAmount, 0);
        }

        public float CurrentHitPoints
        {
            get { return shieldStrength; }
        }

        public float MaxHitPoints
        {
            get { return maxShieldStrength; }
        }
    }
}