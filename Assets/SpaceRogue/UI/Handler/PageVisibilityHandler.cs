using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Handler
{
    public class PageVisibilityHandler
    {
        private readonly Dictionary<string, VisualElement> _elements;
        private string _state;

        public PageVisibilityHandler(Dictionary<string, VisualElement> elements, string initialState)
        {
            this._elements = elements;
            this._state = initialState;

            Init();
        }

        private void Init()
        {
            foreach (var entry in this._elements)
            {
                DisplayStyle display = entry.Key == this._state ? DisplayStyle.Flex : DisplayStyle.None;
                this._elements[entry.Key].style.display = new StyleEnum<DisplayStyle>(display);
            }
        }

        public void HandleState(string state)
        {
            if (this._state == state) return;

            if (!this._elements.ContainsKey(state))
            {
                throw new NullReferenceException("Button data target not properly set up. Page does not exist.");
            }

            this._elements[state].style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            this._elements[this._state].style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            this._state = state;
        }
    }
}