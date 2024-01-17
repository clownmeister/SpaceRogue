using SpaceRogue.UI.Handler;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Controller
{
    public class GameMenuController : MonoBehaviour
    {
        private const string MAP_BUTTON_CLASS_NAME = "-btn-target-exit";

        public UIDocument document;
        // public string initialPageState = "pause";

        // private MenuHandler _menuHandler;

        private void Start()
        {
            VisualElement root = document.rootVisualElement;

            // this._menuHandler = new MenuHandler(root, this.initialPageState);

            RegisterNewGameButton(root);
        }

        private void RegisterNewGameButton(VisualElement root)
        {
            root.Q(className: MAP_BUTTON_CLASS_NAME)
                .RegisterCallback<ClickEvent>(MapAction);
        }

        private void MapAction(ClickEvent clickEvent)
        {
            Scene mapScene = SceneManager.GetSceneByName("Map");
            if (!mapScene.isLoaded)
            {
                SceneManager.LoadScene("Map", LoadSceneMode.Additive);
                SceneManager.sceneLoaded += OnSceneLoaded;
                return;
            }
            
            SceneManager.SetActiveScene(mapScene);
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.SetActiveScene(scene);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}