using SpaceRogue.UIElement;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceRogue.Handler
{
    public class MainMenuMainHandler
    {
        private const string mainMenuButtonClassName = "main__button";

        private readonly VisualElement root;

        public MainMenuMainHandler(VisualElement root)
        {
            this.root = root;
        }

        public void Init()
        {
            var mainButtons = this.root.Query<Button>(className: mainMenuButtonClassName);
            mainButtons.ForEach((Button button) => { button.RegisterCallback<ClickEvent>(OnClickButton); });
        }

        private void OnClickButton(ClickEvent clickEvent)
        {
            Debug.Log("clicked button");
            ButtonElement button = clickEvent.currentTarget as ButtonElement;
            Debug.Log(button?.data);
        }
    }
}