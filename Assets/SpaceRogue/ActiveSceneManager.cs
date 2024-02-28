using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceRogue
{
    public enum SceneState
    {
        MainMenu,
        Game,
        Map
    }

    public class ActiveSceneManager : MonoBehaviour
    {
        public static ActiveSceneManager Instance { get; private set; }

        private GameObject[] _gameToggleObjects;
        private GameObject[] _mapToggleObjects;
        [SerializeField]
        private SceneState _currentState;

        public SceneState CurrentState
        {
            get { return _currentState; }
            private set { _currentState = value; }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                _gameToggleObjects = Array.Empty<GameObject>();
                _mapToggleObjects = Array.Empty<GameObject>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name != "Game") return;
            if (CurrentState == SceneState.Game) return;
            SwitchScene(SceneState.Game);
        }

        public void SwitchScene(SceneState newState)
        {
            switch (newState)
            {
                case SceneState.Game:
                    StartCoroutine(LoadGameScene());
                    break;
                case SceneState.MainMenu:
                    StartCoroutine(LoadMainMenu());
                    return;
                case SceneState.Map:
                    StartCoroutine(LoadMapScene());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }

            ToggleSceneObjects(CurrentState, false);
            ToggleSceneObjects(newState, true);

            CurrentState = newState;
        }

        public void AddToToggleObjects(SceneState state, GameObject obj)
        {
            ref GameObject[] targetArray = ref (state == SceneState.Game ? ref _gameToggleObjects : ref _mapToggleObjects);
            Array.Resize(ref targetArray, targetArray.Length + 1);
            targetArray[^1] = obj;
        }

        public void RemoveFromToggleObjects(SceneState state, GameObject obj)
        {
            ref GameObject[] targetArray = ref (state == SceneState.Game ? ref _gameToggleObjects : ref _mapToggleObjects);
            targetArray = targetArray.Where(o => o != obj).ToArray();
        }

        private static IEnumerator LoadGameScene()
        {
            if (!SceneManager.GetSceneByName("Game").isLoaded)
            {
                yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
                yield return SceneManager.LoadSceneAsync("Map", LoadSceneMode.Additive);
            }
            // yield return SceneManager.LoadSceneAsync("Overlay", LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game"));
        }

        private static IEnumerator LoadMainMenu()
        {
            yield return SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main"));
        }

        private static IEnumerator LoadMapScene()
        {
            if (!SceneManager.GetSceneByName("Map").isLoaded)
            {
                yield return SceneManager.LoadSceneAsync("Map", LoadSceneMode.Additive);
            }
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Map"));
        }

        private void ToggleSceneObjects(SceneState state, bool enable)
        {
            GameObject[] toggleObjects = state switch
            {
                SceneState.Game => _gameToggleObjects,
                SceneState.Map => _mapToggleObjects,
                _ => null,
            };

            if (toggleObjects == null) return;
            foreach (GameObject obj in toggleObjects)
            {
                obj.SetActive(enable);
            }
        }
    }
}