using SpaceRogue.UI.Element;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Handler
{
    public class MenuHandler
    {
        private const string ButtonClass = "-btn-target-page-open";
        private const string PageClass = "-page-target";
        private const string PageDataClassPrefix = "-page-target-";
        private readonly PageVisibilityHandler pageVisibilityHandler;

        private readonly VisualElement root;
        private Dictionary<string, VisualElement> menuPages;

        public MenuHandler(VisualElement root, string initialPageState)
        {
            this.root = root;

            InitPages();
            RegisterButtons();

            this.pageVisibilityHandler = new PageVisibilityHandler(this.menuPages, initialPageState);
        }

        private void RegisterButtons()
        {
            UQueryBuilder<Button> mainButtons = this.root.Query<Button>(className: ButtonClass);
            mainButtons.ForEach(button => { button.RegisterCallback<ClickEvent>(OnClickPageButton); });
        }

        private void InitPages()
        {
            this.menuPages = new Dictionary<string, VisualElement>();
            UQueryBuilder<VisualElement> pages = this.root.Query<VisualElement>(className: PageClass);
            pages.ForEach(page =>
            {
                string target = null;
                IEnumerable<string> classes = page.GetClasses();
                foreach (string className in classes)
                    if (className.StartsWith(PageDataClassPrefix))
                        target = className.Replace(PageDataClassPrefix, "");

                if (target == null) throw new NullReferenceException("Page is not properly set up. Missing class.");

                this.menuPages.Add(target, page);
            });
        }

        private void OnClickPageButton(ClickEvent clickEvent)
        {
            Debug.Log("click");
            if (clickEvent.currentTarget is not ButtonElement button) throw new NullReferenceException("Could not find button in event");
            this.pageVisibilityHandler.HandleState(button.Data);
        }
    }
}