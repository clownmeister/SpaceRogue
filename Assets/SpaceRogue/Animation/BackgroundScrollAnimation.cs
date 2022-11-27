using UnityEngine;
using UnityEngine.UI;

namespace SpaceRogue.Animation
{
    public class BackgroundScrollAnimation : MonoBehaviour
    {
        [SerializeField] private RawImage image;
        [SerializeField] private float x, y;

        private void Update()
        {
            this.image.uvRect = new Rect(this.image.uvRect.position + new Vector2(this.x, this.y) * Time.deltaTime, this.image.uvRect.size);
        }
    }
}