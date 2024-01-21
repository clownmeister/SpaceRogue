using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Controller
{
    public class MapMenuController : MonoBehaviour
    {
        private const string GAME_BUTTON_CLASS_NAME = "-btn-target-game";

        public UIDocument document;

        private void OnEnable()
        {
            VisualElement root = document.rootVisualElement;

            RegisterNewGameButton(root);
        }

        private void RegisterNewGameButton(VisualElement root)
        {
            Debug.Log(root.Q(className: GAME_BUTTON_CLASS_NAME));
            root.Q(className: GAME_BUTTON_CLASS_NAME).RegisterCallback<ClickEvent>(GameAction);
        }

        private void GameAction(ClickEvent clickEvent)
        {
            Time.timeScale = 1;
            Debug.Log("test click game");
            ActiveSceneManager.Instance.SwitchScene(SceneState.Game);
        }
    }
}