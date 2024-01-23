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
            if (_mapDetailBtnClose == null) return;
            _mapDetailBtnClose.UnregisterCallback<ClickEvent>(CloseDetailAction);
        }

        private void InitializeUI()
        {
            VisualElement root = document.rootVisualElement;

            RegisterNewGameButton(root);
            _mapDetailElement = SetupUIComponent<VisualElement>(root, MAP_DETAIL_ELEMENT_CLASS_NAME);
            _mapDetailLabel = SetupUIComponent<Label>(root, MAP_DETAIL_LABEL_CLASS_NAME);
            _mapDetailBtnJump = SetupUIComponent<Button>(root, MAP_DETAIL_BTN_JUMP_CLASS_NAME);
            _mapDetailBtnInfo = SetupUIComponent<Button>(root, MAP_DETAIL_BTN_INFO_CLASS_NAME);
            _mapDetailBtnClose = SetupUIComponent<Button>(root, MAP_DETAIL_BTN_CLOSE_CLASS_NAME);
            Debug.Log(_mapDetailElement);
            Debug.Log(_mapDetailLabel);
            Debug.Log(_mapDetailBtnJump);
            Debug.Log(_mapDetailBtnInfo);
            Debug.Log(_mapDetailBtnClose);
        }

        private T SetupUIComponent<T>(VisualElement root, string className) where T : VisualElement
        {
            return root.Q<T>(className: className);
        }

        private void RegisterNewGameButton(VisualElement root)
        {
            Button gameButton = SetupUIComponent<Button>(root, GAME_BUTTON_CLASS_NAME);
            gameButton.RegisterCallback<ClickEvent>(GameAction);
        }

        private void GameAction(ClickEvent clickEvent)
        {
            Time.timeScale = 1;
            Debug.Log("GAME ACTION");
            ActiveSceneManager.Instance.SwitchScene(SceneState.Game);
        }

        private void DetailAction(MapNode node)
        {
            Debug.Log("DETAIL ACTION");
            Debug.Log(node.Name);
            Debug.Log(node.Type);
            Debug.Log(node.Variant);
            _mapDetailElement.style.display = DisplayStyle.Flex;
            _mapDetailLabel.text = node.Name;
            _mapDetailBtnJump.style.visibility = Visibility.Hidden;
        }

        private void CloseDetailAction(ClickEvent evt)
        {
            Debug.Log("CLOSE ACTION");
            _mapDetailElement.style.display = DisplayStyle.None;
        }
    }
}
