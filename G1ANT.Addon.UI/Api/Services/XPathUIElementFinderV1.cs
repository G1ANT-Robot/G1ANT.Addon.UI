using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Exceptions;
using G1ANT.Addon.UI.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G1ANT.Addon.UI.Api.Services
{
    public class XPathUIElementFinderV1 : XPathUIElementFinderBase
    {
        private AutomationElement root;

        public XPathUIElementFinderV1(AutomationElement root = null)
        {
            this.root = root ?? AutomationSingleton.Automation.GetDesktop();
        }

        private int FindElementIndex(AutomationElement automationElement)
        {
            if (automationElement?.Parent == null)
                return -1;

            var index = 0;
            var treeWalker = automationElement.Parent.GetTreeWalker();
            var elementNode = treeWalker.GetFirstChild(automationElement.Parent);
            while (elementNode != null)
            {
                if (elementNode.Equals(automationElement))
                    return index;
                index++;
                elementNode = treeWalker.GetNextSibling(elementNode);
            }
            return -1;
        }

        public override object GetPropertyValue(UIElement element, string propertyName)
        {
            if (propertyName.ToLower() == UIElement.Indexes.Index)
                return FindElementIndex(element.AutomationElement);
            return base.GetPropertyValue(element, propertyName);
        }

        public override UIElement FindChild(UIElement elem, Func<UIElement, int, bool> compare)
        {
            var index = 0;
            var treeWalker = GetTreeWalker(elem.AutomationElement);
            var elementNode = treeWalker.GetFirstChild(elem.AutomationElement);
            while (elementNode != null)
            {
                var uiElement = elementNode.ToUIElement(index);
                if (compare(uiElement, index))
                {
                    return uiElement;
                }

                index++;
                elementNode = treeWalker.GetNextSibling(elementNode);
            }
            throw new ElementNotAvailableException();
        }

        public override UIElement FindDescendant(UIElement elem, Func<UIElement, int, bool> compare)
        {
            int index = 0;
            var treeWalker = GetTreeWalker(elem.AutomationElement);
            var elementNode = treeWalker.GetFirstChild(elem.AutomationElement);
            while (elementNode != null)
            {
                var uiElement = elementNode.ToUIElement(index);
                if (compare(uiElement, index))
                {
                    return uiElement;
                }

                var descendantElement = FindDescendant(uiElement, compare);
                if (descendantElement != null)
                {
                    return descendantElement;
                }

                index++;
                elementNode = treeWalker.GetNextSibling(elementNode);
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
            var treeWalker = GetTreeWalker(elem.AutomationElement);
            var elementNode = treeWalker.GetFirstChild(elem.AutomationElement);
            while (elementNode != null)
            {
                var uiElement = elementNode.ToUIElement(index);
                if (compare(uiElement, index))
                {
                    return uiElement;
                }

                index++;
                elementNode = treeWalker.GetNextSibling(elementNode);
            }
            throw new ElementNotAvailableException();
        }

        private ITreeWalker GetTreeWalker(AutomationElement elem)
        {
            var rootElement = elem ?? root;
            return rootElement.GetTreeWalker();
        }

    }
}
