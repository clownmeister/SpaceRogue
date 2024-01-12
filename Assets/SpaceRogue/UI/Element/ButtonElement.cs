using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace SpaceRogue.UI.Element
{
    public class ButtonElement : Button
    {
        public string Data { get; set; }

        public new class UxmlFactory : UxmlFactory<ButtonElement, UxmlTraits> { }

        public new class UxmlTraits : Button.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _data = new() { name = "data", defaultValue = "" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is not ButtonElement button) throw new Exception("Invalid element type provided");

                button.Data = this._data.GetValueFromBag(bag, cc);
                focusable.defaultValue = true;
            }
        }
    }
}