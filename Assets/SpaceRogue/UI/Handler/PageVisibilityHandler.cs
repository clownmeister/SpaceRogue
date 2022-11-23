using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Handler
{
    public class PageVisibilityHandler
    {
        private readonly Dictionary<string, VisualElement> elements;
        private string state;

        public PageVisibilityHandler(Dictionary<string, VisualElement> elements, string initialState)
        {
            this.elements = elements;
            this.state = initialState;

            Init();
        }

        private void Init()
        {
            foreach (var entry in this.elements) {
                DisplayStyle display = entry.Key == this.state ? DisplayStyle.Flex : DisplayStyle.None;
                this.elements[entry.Key].style.display = new StyleEnum<DisplayStyle>(display);
            }
        }

        public void HandleState(string state)
        {
            if (this.state == state) return;

            if (!this.elements.ContainsKey(state)) {
                throw new NullReferenceException("Button data target not properly set up. Page does not exist.");
            }

            this.elements[state].style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            this.elements[this.state].style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            this.state = state;
        }
    }
}