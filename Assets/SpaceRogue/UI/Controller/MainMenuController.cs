using SpaceRogue.UI.Handler;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Controller
{
    public class MainMenuController : MonoBehaviour
    {
        private const string ExitButtonClassName = "-btn-target-exit";

        public UIDocument document;
        public string initialPageState = "main";

        private MenuHandler menuHandler;

        private void OnEnable()
        {
            VisualElement root = this.document.rootVisualElement;

            this.menuHandler = new MenuHandler(root, this.initialPageState);
        }

        private void RegisterExitButton(VisualElement root)
        {
            root.Q(className: ExitButtonClassName).RegisterCallback<ClickEvent>(clickEvent =>
            {
                // if (clickEvent.currentTarget is not ButtonElement button) throw new NullReferenceException("Could not find button in event");
                Application.Quit();
            });
        }
    }
}