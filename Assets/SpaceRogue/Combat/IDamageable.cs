namespace SpaceRogue.Combat
{
    public interface IDamageable
    {
        float CurrentHitPoints { get; }

        float MaxHitPoints { get; }

        void TakeDamage(float damageAmount);
    }
}