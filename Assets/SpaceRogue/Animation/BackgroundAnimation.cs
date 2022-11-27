using UnityEngine;

namespace SpaceRogue.Animation
{
    public class BackgroundAnimation : MonoBehaviour
    {
        public Vector2 positionClamps;
        public float stepSize = .1f;
        public bool spherical = true;

        private Vector2 oldTarget, target;
        private float step;

        private void Update()
        {
            if (this.step >= 1) {
                this.oldTarget = this.target;
                this.target = GetTarget();
                this.step = 0;
            }

            this.step += this.stepSize * Time.deltaTime;
            if (this.spherical) {
                transform.localPosition = Vector3.SlerpUnclamped(this.oldTarget, this.target, Mathf.SmoothStep(0, 1, this.step));
                return;
            }

            transform.localPosition = Vector3.SlerpUnclamped(this.oldTarget, this.target, Mathf.SmoothStep(0, 1, this.step));
        }

        private void OnEnable()
        {
            transform.localPosition = Vector3.zero;
            this.oldTarget = Vector3.zero;
            this.target = GetTarget();
            this.step = 0;
        }

        private Vector2 GetTarget()
        {
            return new Vector2(
                Random.Range(-this.positionClamps.x, this.positionClamps.x),
                Random.Range(-this.positionClamps.y, this.positionClamps.y)
            );
        }
    }
}