using SpaceRogue.UI.Handler;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Controller
{
    public class MainMenuController : MonoBehaviour
    {
        public UIDocument document;
        public string initialPageState = "main";

        private MenuHandler menuHandler;

        private void OnEnable()
        {
            VisualElement root = this.document.rootVisualElement;

            this.menuHandler = new MenuHandler(root, this.initialPageState);
        }
    }
}