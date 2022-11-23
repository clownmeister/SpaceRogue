using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Animator
{
    public class MainMenuBackgroundAnimator : MonoBehaviour
    {
        private const string ElementClassName = "main-menu__background";

        public UIDocument document;

        private VisualElement backgroundElement;

        private void Update()
        {
            // this.backgroundElement.style.translate = new Translate(1000, 500, 0);
        }

        private void OnEnable()
        {
            // VisualElement root = this.document.rootVisualElement;
            // this.backgroundElement = root.Query<VisualElement>(className: ElementClassName);
            // this.backgroundElement.style.translate = new Translate(0, 0, 0);
        }
    }
}