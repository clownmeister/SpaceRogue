using SpaceRogue.Map;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Controller
{
    public class MapMenuController : MonoBehaviour
    {
        private const string GAME_BUTTON_CLASS_NAME = "-btn-target-game";
        private const string MAP_DETAIL_ELEMENT_CLASS_NAME = "-target-detail-element";
        private const string MAP_DETAIL_LABEL_CLASS_NAME = "-btn-target-detail-label";
        private const string MAP_DETAIL_BTN_JUMP_CLASS_NAME = "-btn-target-jump";
        private const string MAP_DETAIL_BTN_INFO_CLASS_NAME = "-btn-target-info";
        private const string MAP_DETAIL_BTN_CLOSE_CLASS_NAME = "-btn-target-detail-close";

        private VisualElement _mapDetailElement;
        private Label _mapDetailLabel;
        private Button _mapGameButton;
        private Button _mapDetailBtnJump;
        private Button _mapDetailBtnInfo;
        private Button _mapDetailBtnClose;

        public UIDocument document;

        private void OnEnable()
        {
            InitializeUI();
            MapManager.Instance.OnNodeSelected += DetailAction;
            _mapDetailBtnClose.RegisterCallback<ClickEvent>(CloseDetailAction);
        }

        private void OnDisable()
        {
            MapManager.Instance.OnNodeSelected -= DetailAction;
            _mapDetailBtnClose?.UnregisterCallback<ClickEvent>(CloseDetailAction);
        }

        private void InitializeUI()
        {
            VisualElement root = document.rootVisualElement;

            _mapGameButton = SetupUIComponent<Button>(root, GAME_BUTTON_CLASS_NAME);
            _mapDetailElement = SetupUIComponent<VisualElement>(root, MAP_DETAIL_ELEMENT_CLASS_NAME);
            _mapDetailLabel = SetupUIComponent<Label>(root, MAP_DETAIL_LABEL_CLASS_NAME);
            _mapDetailBtnJump = SetupUIComponent<Button>(root, MAP_DETAIL_BTN_JUMP_CLASS_NAME);
            _mapDetailBtnInfo = SetupUIComponent<Button>(root, MAP_DETAIL_BTN_INFO_CLASS_NAME);
            _mapDetailBtnClose = SetupUIComponent<Button>(root, MAP_DETAIL_BTN_CLOSE_CLASS_NAME);

            _mapGameButton.RegisterCallback<ClickEvent>(GameAction);
            _mapDetailBtnJump.RegisterCallback<ClickEvent>(JumpAction);
        }

        private static T SetupUIComponent<T>(VisualElement root, string className) where T : VisualElement
        {
            return root.Q<T>(className: className);
        }

        private static void GameAction(ClickEvent @event)
        {
            Time.timeScale = 1;
            Debug.Log("GAME ACTION");
            ActiveSceneManager.Instance.SwitchScene(SceneState.Game);
            @event.StopImmediatePropagation();
        }

        private void DetailAction(MapNode node)
        {
            Debug.Log("DETAIL ACTION");
            _mapDetailElement.style.display = DisplayStyle.Flex;
            _mapDetailLabel.text = node.Name;

            _mapDetailBtnJump.style.visibility = MapManager.Instance.IsConnectedToCurrent(node) ? Visibility.Visible : Visibility.Hidden;
        }

        private void CloseDetailAction(ClickEvent @event)
        {
            Debug.Log("CLOSE ACTION");
            _mapDetailElement.style.display = DisplayStyle.None;
            @event.StopImmediatePropagation();
        }

        private void JumpAction(ClickEvent @event)
        {
            Debug.Log("JUMP ACTION");
            MapManager.Instance.Jump();

            if (MapManager.Instance.CurrentNode == MapManager.Instance.SelectedNode)
            {
                _mapDetailBtnJump.style.visibility = Visibility.Hidden;
            }
            @event.StopImmediatePropagation();
        }
    }
}
