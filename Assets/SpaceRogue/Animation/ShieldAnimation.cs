using UnityEngine;

namespace SpaceRogue.Animation
{
    public class ShieldAnimation : MonoBehaviour
    {
        public float scaleLimit = .1f;
        public float scaleSpeed = 1;
        public float rotationSpeed = 1;
        private bool expanding = true;

        private Vector3 initialScale, minScale, maxScale, scaleStep;

        void Start()
        {
            this.initialScale = transform.localScale;
            Vector3 diffVector = new Vector3(this.scaleLimit, this.scaleLimit, this.scaleLimit);
            this.minScale = this.initialScale - diffVector;
            this.maxScale = this.initialScale + diffVector;
            this.scaleStep = new Vector3(this.scaleSpeed, this.scaleSpeed, this.scaleSpeed);
        }

        void Update()
        {
            transform.Rotate(0, 0, this.rotationSpeed * Time.deltaTime);

            if (transform.localScale.x > this.maxScale.x) {
                this.expanding = false;
            }
            else if (transform.localScale.x < this.minScale.x) {
                this.expanding = true;
            }

            transform.localScale += (this.expanding ? this.scaleStep : -this.scaleStep) * Time.deltaTime;
        }
    }
}