using System.Collections;
using UnityEngine;

namespace SpaceRogue.Combat
{
    public class Shield : MonoBehaviour, IDamageable
    {
        public float maxShieldStrength = 100;
        public float shieldStrength = 100;
        public bool rechargeable = true;
        public float rechargeCombatCooldown = 3f; // Cooldown time in seconds
        public float rechargeHps = 5f;

        [Header("Optional Graphic settings")]
        public SpriteRenderer shieldSprite;
        public float fadeTime = 1.0f;
        private float _currentAlpha;
        private Coroutine _fadeRoutine;

        private float _initialAlpha;
        private bool _isFading = false;
        private float _lastHitTime = -Mathf.Infinity;

        private void Start()
        {
            if (shieldSprite == null) return;
            _initialAlpha = shieldSprite.color.a;
            _currentAlpha = _initialAlpha;
        }

        private void Update()
        {
            RechargeShield();

            _currentAlpha = _initialAlpha * (shieldStrength / maxShieldStrength);
            if (_isFading) return;
            Color color = shieldSprite.color;
            shieldSprite.color = new Color(color.r, color.g, color.b, _currentAlpha);
        }

        public void TakeDamage(float damageAmount)
        {
            _isFading = true;
            shieldStrength = Mathf.Max(shieldStrength - damageAmount, 0);
            _lastHitTime = Time.time;

            if (shieldSprite == null) return;
            if (_fadeRoutine != null)
            {
                StopCoroutine(_fadeRoutine);
            }

            float increasedAlpha = Mathf.Min(_currentAlpha + 0.2f, 1.0f);
            Color color = shieldSprite.color;
            shieldSprite.color = new Color(color.r, color.g, color.b, increasedAlpha);
            _fadeRoutine = StartCoroutine(FadeToCurrentAlpha());
            _isFading = false;
        }

        public float CurrentHitPoints
        {
            get { return shieldStrength; }
        }

        public float MaxHitPoints
        {
            get { return maxShieldStrength; }
        }

        private void RechargeShield()
        {
            if (!rechargeable) return;
            if (CurrentHitPoints >= MaxHitPoints) return;
            if (Time.time - _lastHitTime < rechargeCombatCooldown) return;

            shieldStrength = Mathf.Min(shieldStrength + rechargeHps * Time.deltaTime, maxShieldStrength);
        }

        private IEnumerator FadeToCurrentAlpha()
        {
            float timeElapsed = 0;

            while (timeElapsed < fadeTime)
            {
                float newAlpha = Mathf.Lerp(Mathf.Min(_currentAlpha + 0.2f, 1.0f), _currentAlpha, timeElapsed / fadeTime);
                Color color = shieldSprite.color;
                shieldSprite.color = new Color(color.r, color.g, color.b, newAlpha);

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            Color shieldColor = shieldSprite.color;
            shieldSprite.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, _currentAlpha);
        }
    }
}