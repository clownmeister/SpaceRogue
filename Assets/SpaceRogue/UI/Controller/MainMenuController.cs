using SpaceRogue.UI.Handler;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Controller
{
    public class MainMenuController : MonoBehaviour
    {
        private const string ExitButtonClassName = "-btn-target-exit";
        private const string NewGameButtonClassName = "-btn-target-new-game";

        public UIDocument document;
        public string initialPageState = "main";

        private MenuHandler menuHandler;

        private void OnEnable()
        {
            VisualElement root = this.document.rootVisualElement;

            this.menuHandler = new MenuHandler(root, this.initialPageState);

            RegisterExitButton(root);
            RegisterNewGameButton(root);
        }

        private void RegisterExitButton(VisualElement root)
        {
            root.Q(className: ExitButtonClassName).RegisterCallback<ClickEvent>(clickEvent =>
            {
                // if (clickEvent.currentTarget is not ButtonElement button) throw new NullReferenceException("Could not find button in event");
                Application.Quit();
            });
        }

        private void RegisterNewGameButton(VisualElement root)
        {
            root.Q(className: NewGameButtonClassName).RegisterCallback<ClickEvent>(clickEvent => { SceneManager.LoadScene("Game"); });
        }
    }
}