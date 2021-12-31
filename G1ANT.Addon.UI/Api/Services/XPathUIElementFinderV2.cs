using FlaUI.Core.Exceptions;
using G1ANT.Addon.UI.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G1ANT.Addon.UI.Api.Services
{
    public class XPathUIElementFinderV2 : XPathUIElementFinderBase
    {
        public override UIElement FindChild(UIElement elem, Func<UIElement, int, bool> compare)
        {
            var index = 0;
            foreach (var elementNode in elem.AutomationElement.FindAllChildren())
            {
                var uiElement = elementNode.ToUIElement(index);
                if (compare(uiElement, index))
                    return uiElement;
                index++;
            }
            throw new ElementNotAvailableException();
        }

        public override UIElement FindDescendant(UIElement elem, Func<UIElement, int, bool> compare)
        {
            int index = 0;
            foreach (var elementNode in elem.AutomationElement.FindAllChildren())
            {
                var uiElement = elementNode.ToUIElement(index);
                if (compare(uiElement, index))
                    return uiElement;

                var descendantElement = FindDescendant(uiElement, compare);
                if (descendantElement != null)
                    return descendantElement;
                index++;
            }
            return null;
        }

        public override UIElement FindDescendantOrSelf(UIElement elem, Func<UIElement, int, bool> compare)
        {
            if (compare(elem, -1))
                return elem;
            return FindDescendant(elem, compare);
        }

        public override UIElement FindFollowingSibling(UIElement elem, Func<UIElement, int, bool> compare)
        {
            var index = 0;
            foreach (var elementNode in elem.AutomationElement.FindAllChildren())
            {
                var uiElement = elementNode.ToUIElement(index);
                if (compare(uiElement, index))
                    return uiElement;
                index++;
            }
            throw new ElementNotAvailableException();
        }
    }
}
