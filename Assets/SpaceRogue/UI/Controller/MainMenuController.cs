using SpaceRogue.UI.Handler;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Controller
{
    public class MainMenuController : MonoBehaviour
    {
        private const string EXIT_BUTTON_CLASS_NAME = "-btn-target-exit";
        private const string NEW_GAME_BUTTON_CLASS_NAME = "-btn-target-new-game";

        public UIDocument document;
        public string initialPageState = "main";

        private MenuHandler _menuHandler;

        private void OnEnable()
        {
            VisualElement root = this.document.rootVisualElement;

            this._menuHandler = new MenuHandler(root, this.initialPageState);

            RegisterExitButton(root);
            RegisterNewGameButton(root);
        }

        private void RegisterExitButton(VisualElement root)
        {
            root.Q(className: EXIT_BUTTON_CLASS_NAME).RegisterCallback<ClickEvent>(clickEvent =>
            {
                // if (clickEvent.currentTarget is not ButtonElement button) throw new NullReferenceException("Could not find button in event");
                Application.Quit();
            });
        }

        private void RegisterNewGameButton(VisualElement root)
        {
            root.Q(className: NEW_GAME_BUTTON_CLASS_NAME).RegisterCallback<ClickEvent>(clickEvent => { SceneManager.LoadScene("Game"); });
        }
    }
}