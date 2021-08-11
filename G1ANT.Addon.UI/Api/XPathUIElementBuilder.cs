using System;
using System.Collections.Generic;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Exceptions;
using FlaUI.Core.Identifiers;
using FlaUI.UIA3.Identifiers;
using G1ANT.Addon.UI.XPathParser;
using CompareFunc = System.Func<G1ANT.Addon.UI.Api.UIElement, int, bool>;
using GetElementFunc = System.Func<
    G1ANT.Addon.UI.Api.UIElement,
    G1ANT.Addon.UI.Api.UIElement>;
using FindElementFunc = System.Func<
    G1ANT.Addon.UI.Api.UIElement,
    System.Func<G1ANT.Addon.UI.Api.UIElement, int, bool>,
    G1ANT.Addon.UI.Api.UIElement>;
using NotSupportedException = System.NotSupportedException;
using G1ANT.Addon.UI.ExtensionMethods;

namespace G1ANT.Addon.UI.Api
{
    public class XPathUIElementBuilder : IXPathBuilder<object>
    {
        public enum UiAutomationElement
        {
            Id,
            ProgrammaticName,
        }

        public AutomationElement Root { get; }
        public XPathUIElementBuilder(AutomationElement root = null)
        {
            Root = root ?? AutomationSingleton.Automation.GetDesktop();
        }

        protected UIElement FindDescendant(UIElement elem, CompareFunc compare)
        {
            int index = 0;
            var treeWalker = GetTreeWalker(elem.AutomationElement);
            var elementNode = treeWalker.GetFirstChild(elem.AutomationElement).ToUIElement(index);
            while (elementNode != null)
            {
                if (compare(elementNode, index))
                {
                    return elementNode;
                }

                var descendantElement = FindDescendant(elementNode, compare);
                if (descendantElement != null)
                {
                    return descendantElement;
                }

                index++;
                elementNode = treeWalker.GetNextSibling(elementNode.AutomationElement).ToUIElement(index);
            }
            return null;
        }

        private ITreeWalker GetTreeWalker(AutomationElement elem)
        {
            var rootElement = elem ?? Root;
            return rootElement.GetTreeWalker();
        }

        protected UIElement FindChild(UIElement elem, CompareFunc compare)
        {
            var index = 0;
            var treeWalker = GetTreeWalker(elem.AutomationElement);
            var elementNode = treeWalker.GetFirstChild(elem.AutomationElement).ToUIElement(index);
            while (elementNode != null)
            {
                if (compare(elementNode, index))
                {
                    return elementNode;
                }

                index++;
                elementNode = treeWalker.GetNextSibling(elementNode.AutomationElement).ToUIElement(index);
            }
            throw new ElementNotAvailableException();
        }

        protected UIElement FindFollowingSibling(UIElement elem, CompareFunc compare)
        {
            var index = 0;
            var treeWalker = GetTreeWalker(elem.AutomationElement);
            var elementNode = treeWalker.GetFirstChild(elem.AutomationElement).ToUIElement(index);
            while (elementNode != null)
            {
                if (compare(elementNode, index))
                {
                    return elementNode;
                }

                index++;
                elementNode = treeWalker.GetNextSibling(elementNode.AutomationElement).ToUIElement(index);
            }
            throw new ElementNotAvailableException();
        }

        protected UIElement FindDescendantOrSelf(UIElement elem, CompareFunc compare)
        {
            if (compare(elem, -1))
                return elem;
            return FindDescendant(elem, compare);
        }

        public void StartBuild()
        {
        }

        public object EndBuild(object result)
        {
            return result;
        }

        public object String(string value)
        {
            return value;
        }

        public object Number(string value)
        {
            int result = -1;
            if (int.TryParse(value, out result))
                return result;
            throw new NotSupportedException($"Number '{value}' is not supported.");
        }

        protected object EqOperator(object left, object right)
        {
            if (left is PropertyId propertyId)
            {
                return new CompareFunc((elem, index) =>
                {
                    return elem.AutomationElement.FrameworkAutomationElement.TryGetPropertyValue(propertyId, out var propValue) && propValue != null && propValue.Equals(right);
                });
            }
            else if (left is UiAutomationElement en)
            {
                if (UiAutomationElement.ProgrammaticName == en)
                {
                    return new CompareFunc((elem, index) =>
                    {
                        return elem.AutomationElement.Properties.ControlType.IsSupported 
                            && elem.AutomationElement.Properties.ControlType.Value.ToString().Replace("ControlType.", "").Equals(right);
                    });
                }
                if (UiAutomationElement.Id == en)
                {
                    return new CompareFunc((elem, index) =>
                    {
                        return elem.AutomationElement.Properties.ControlType.IsSupported 
                            && elem.AutomationElement.Properties.ControlType.Value.ToString().Equals(right);
                    });
                }
            }
            throw new NotSupportedException("Left side of 'equal' operator has not been recognized.");
        }

        protected object AndOperator(object left, object right)
        {
            if (left is CompareFunc cmp1 && right is CompareFunc cmp2)
            {
                return new CompareFunc((elem, index) =>
                {
                    return cmp1(elem, index) && cmp2(elem, index);
                });
            }
            throw new NotSupportedException("Left or right side of 'and' operator has not been recognized.");
        }

        protected object OrOperator(object left, object right)
        {
            if (left is CompareFunc cmp1 && right is CompareFunc cmp2)
            {
                return new CompareFunc((elem, index) =>
                {
                    return cmp1(elem, index) || cmp2(elem, index);
                });
            }
            throw new NotSupportedException("Left or right side of 'or' operator has not been recognized.");
        }

        public object Operator(XPathOperator op, object left, object right)
        {
            switch (op)
            {
                case XPathOperator.Eq:
                    return EqOperator(left, right);
                case XPathOperator.And:
                    return AndOperator(left, right);
                case XPathOperator.Or:
                    return OrOperator(left, right);
            }
            throw new NotSupportedException($"Operator {op} is not supported.");
        }

        public object Axis(XPathAxis xpathAxis, System.Xml.XPath.XPathNodeType nodeType, string prefix, string name)
        {
            if (nodeType == System.Xml.XPath.XPathNodeType.Element)
            {
                if (name != "ui")
                    throw new NotSupportedException($"{name} element is not supported.");
            }
            switch (xpathAxis)
            {
                case XPathAxis.Root:
                    return Root.ToUIElement(0);
                case XPathAxis.Descendant:
                    return (FindElementFunc)FindDescendant;
                case XPathAxis.DescendantOrSelf:
                    return (FindElementFunc)FindDescendantOrSelf;
                case XPathAxis.FollowingSibling:
                    return (FindElementFunc)FindFollowingSibling;
                case XPathAxis.Child:
                    return (FindElementFunc)FindChild;
                case XPathAxis.Attribute:
                    return GetAttributeId(name.ToLower());
            }
            return null;
        }

        private object GetAttributeId(string name)
        {
            if (name == "id")
                return AutomationObjectIds.AutomationIdProperty;
            if (name == "name")
                return AutomationObjectIds.NameProperty;
            if (name == "class")
                return AutomationObjectIds.ClassNameProperty;
            if (name == "type")
                return UiAutomationElement.ProgrammaticName;
            if (name == "typeid")
                return UiAutomationElement.Id;
            throw new NotSupportedException($"Attribute {name} is not supportet.");
        }

        public object JoinStep(object left, object right)
        {
            if (left is UIElement elem && right is GetElementFunc func)
            {
                return func(elem);
            }
            if (left is GetElementFunc inner && right is GetElementFunc outer)
            {
                GetElementFunc retFunc = (element) =>
                {
                    var ret = inner(element);
                    if (ret == null)
                        throw new ElementNotAvailableException();
                    return outer(ret);
                };
                return retFunc;
            }
            return null;
        }

        public object Predicate(object node, object condition, bool reverseStep)
        {
            if (node is FindElementFunc outer1 && condition is CompareFunc inner1)
            {
                GetElementFunc func = (elem) =>
                {
                    return outer1(elem, inner1);
                };
                return func;
            }
            else if (node is FindElementFunc outer2 && condition is int value)
            {
                GetElementFunc func = (elem) =>
                {
                    return outer2(elem, (childElem, childIndex) => { return childIndex == value; });
                };
                return func;
            }
            return null;
        }

        public object Variable(string prefix, string name)
        {
            throw new NotImplementedException("Method 'Variable' is not implemented.");
        }

        public object Function(string prefix, string name, IList<object> args)
        {
            if (args.Count == 2 && IsXpathFunction(name) && args[0] is PropertyId propertyId && args[1] is string text)
            {
                CompareFunc func = (elem, index) =>
                {

                    if (elem.AutomationElement.FrameworkAutomationElement.TryGetPropertyValue(propertyId, out var propValue))
                    {
                        return propValue is string str && IsConditionMet(name, str, text);
                    }

                    return false;
                };
                return func;
            }
            throw new NotSupportedException($"Function {name} is not supported.");
        }

        private bool IsConditionMet(string name, string str, string text)
        {
            switch (name.ToLower())
            {
                case "starts-with": return str.StartsWith(text);
                case "ends-with": return str.EndsWith(text);
                case "contains": return str.Contains(text);
                default: return false;

            }
        }

        private bool IsXpathFunction (string name)
        {
            name = name.ToLower();
            return name == "starts-with" || name == "ends-with" || name == "contains";
        }
    }
}
