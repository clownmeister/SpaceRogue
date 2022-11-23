using SpaceRogue.Handler;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceRogue.Controller
{
    public class GuiController : MonoBehaviour
    {
        public UIDocument document;
        private MainMenuMainHandler mainMenuHandler;

        private void OnEnable()
        {
            VisualElement root = this.document.rootVisualElement;

            this.mainMenuHandler = new MainMenuMainHandler(root);

            this.mainMenuHandler.Init();
        }
    }
}