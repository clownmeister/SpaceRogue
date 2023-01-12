using UnityEngine;
using UnityEngine.UI;

namespace SpaceRogue.Animation
{
    public class BackgroundScrollAnimation : MonoBehaviour
    {
        [SerializeField] private RawImage image;
        [SerializeField] private float speedX, speedY;
        public float speedMultiplier = 1;

        private void Update()
        {
            this.image.uvRect = new Rect(
                this.image.uvRect.position + new Vector2(this.speedX, this.speedY) * (this.speedMultiplier * Time.deltaTime), 
                this.image.uvRect.size);
        }
    }
}