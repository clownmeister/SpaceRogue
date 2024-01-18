using UnityEngine;

namespace SpaceRogue
{
    public class ToggleOnSceneChange : MonoBehaviour
    {
        [SerializeField]
        private SceneState targetSceneState;

        public SceneState TargetSceneState
        {
            get { return targetSceneState; }
            set { targetSceneState = value; }
        }

        private void Start()
        {
            gameObject.SetActive(ActiveSceneManager.Instance.CurrentState == TargetSceneState);
            ActiveSceneManager.Instance.AddToToggleObjects(targetSceneState, gameObject);
        }

        private void OnDestroy()
        {
            ActiveSceneManager.Instance.RemoveFromToggleObjects(targetSceneState, gameObject);
        }
    }
}