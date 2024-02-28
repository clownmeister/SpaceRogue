using System;
using System.Collections.Generic;
using SpaceRogue.UI.Element;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Handler
{
    public class MenuHandler
    {
        private const string ButtonClass = "-btn-target-page-open";
        private const string PageClass = "-page-target";
        private const string PageDataClassPrefix = "-page-target-";
        private readonly PageVisibilityHandler _pageVisibilityHandler;

        private readonly VisualElement _root;
        private Dictionary<string, VisualElement> _menuPages;

        public MenuHandler(VisualElement root, string initialPageState)
        {
            _root = root;

            InitPages();
            RegisterButtons();

            _pageVisibilityHandler = new PageVisibilityHandler(_menuPages, initialPageState);
        }

        private void RegisterButtons()
        {
            UQueryBuilder<Button> mainButtons = _root.Query<Button>(className: ButtonClass);
            mainButtons.ForEach(button => { button.RegisterCallback<ClickEvent>(OnClickPageButton); });
        }

        private void InitPages()
        {
            _menuPages = new Dictionary<string, VisualElement>();
            UQueryBuilder<VisualElement> pages = _root.Query<VisualElement>(className: PageClass);
            pages.ForEach(page =>
            {
                string target = null;
                IEnumerable<string> classes = page.GetClasses();
                foreach (string className in classes)
                    if (className.StartsWith(PageDataClassPrefix))
                        target = className.Replace(PageDataClassPrefix, "");

                if (target == null) throw new NullReferenceException("Page is not properly set up. Missing class.");

                _menuPages.Add(target, page);
            });
        }

        private void OnClickPageButton(ClickEvent @event)
        {
            Debug.Log("click");
            if (@event.currentTarget is not ButtonElement button) throw new NullReferenceException("Could not find button in event");
            _pageVisibilityHandler.HandleState(button.Data);
            @event.StopImmediatePropagation();
        }
    }
}