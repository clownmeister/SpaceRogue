using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Controller
{
    public class GameMenuController : MonoBehaviour
    {
        private const string MAP_BUTTON_CLASS_NAME = "-btn-target-map";

        public UIDocument document;

        private void OnEnable()
        {
            VisualElement root = document.rootVisualElement;

            RegisterNewGameButton(root);
        }

        private void RegisterNewGameButton(VisualElement root)
        {
            root.Q(className: MAP_BUTTON_CLASS_NAME).RegisterCallback<ClickEvent>(MapAction);
        }

        private void MapAction(ClickEvent clickEvent)
        {
            Time.timeScale = 0;
            Debug.Log("test click map");
            ActiveSceneManager.Instance.SwitchScene(SceneState.Map);
        }
    }
}