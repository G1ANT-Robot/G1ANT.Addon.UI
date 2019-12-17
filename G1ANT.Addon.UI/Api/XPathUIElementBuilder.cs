using System;
using System.Collections.Generic;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Exceptions;
using FlaUI.Core.Identifiers;
using FlaUI.UIA3.Identifiers;
using G1ANT.Addon.UI.XPathParser;
using CompareFunc = System.Func<FlaUI.Core.AutomationElements.AutomationElement, int, bool>;
using GetElementFunc = System.Func<
    FlaUI.Core.AutomationElements.AutomationElement,
    FlaUI.Core.AutomationElements.AutomationElement>;
using FindElementFunc = System.Func<
    FlaUI.Core.AutomationElements.AutomationElement,
    System.Func<FlaUI.Core.AutomationElements.AutomationElement, int, bool>,
    FlaUI.Core.AutomationElements.AutomationElement>;
using NotSupportedException = System.NotSupportedException;

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

        protected AutomationElement FindDescendant(AutomationElement elem, CompareFunc compare)
        {
            var treeWalker = GetTreeWalker(elem);
            var elementNode = treeWalker.GetFirstChild(elem);
            int index = 0;
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

                elementNode = treeWalker.GetNextSibling(elementNode);
                index++;
            }
            return null;
        }

        private ITreeWalker GetTreeWalker(AutomationElement elem)
        {
            var rootElement = elem ?? Root;
            return rootElement.Automation.TreeWalkerFactory.GetControlViewWalker();
        }

        protected AutomationElement FindChild(AutomationElement elem, CompareFunc compare)
        {
            var treeWalker = GetTreeWalker(elem);
            var elementNode = treeWalker.GetFirstChild(elem);
            var index = 0;
            while (elementNode != null)
            {
                if (compare(elementNode, index))
                {
                    return elementNode;
                }

                elementNode = treeWalker.GetNextSibling(elementNode);
                index++;
            }
            return elem;
        }

        protected AutomationElement FindFollowingSibling(AutomationElement elem, CompareFunc compare)
        {
            var treeWalker = GetTreeWalker(elem);
            var elementNode = treeWalker.GetFirstChild(elem);
            var index = 0;
            while (elementNode != null)
            {
                if (compare(elementNode, index))
                {
                    return elementNode;
                }

                elementNode = treeWalker.GetNextSibling(elementNode);
                index++;
            }
            throw new ElementNotAvailableException();
        }

        protected AutomationElement FindDescendantOrSelf(AutomationElement elem, CompareFunc compare)
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

        public object Operator(XPathOperator op, object left, object right)
        {
            if (op == XPathOperator.Eq)
            {
                if (left is PropertyId propertyId)
                {
                    return new CompareFunc((elem, index) =>
                    {
                        return elem.FrameworkAutomationElement.TryGetPropertyValue(propertyId, out var propValue) && propValue != null && propValue.Equals(right);
                    });
                }
                else if (left is UiAutomationElement en)
                {
                    if (UiAutomationElement.ProgrammaticName == en)
                    {
                        return new CompareFunc((elem, index) =>
                        {
                            return elem.Properties.ControlType.IsSupported && elem.Properties.ControlType.Value.ToString().Replace("ControlType.", "").Equals(right);
                        });
                    }
                    if (UiAutomationElement.Id == en)
                    {
                        return new CompareFunc((elem, index) =>
                        {
                            return elem.Properties.ControlType.IsSupported && elem.Properties.ControlType.Value.ToString().Equals(right);
                        });
                    }
                }
            }
            throw new NotSupportedException($"Operator {op.ToString()} is not supported.");
        }

        public object Axis(XPathAxis xpathAxis, System.Xml.XPath.XPathNodeType nodeType, string prefix, string name)
        {
            if (xpathAxis == XPathAxis.Root)
                return Root;
            if (nodeType == System.Xml.XPath.XPathNodeType.Element)
            {
                if (name != "ui")
                    throw new NotSupportedException($"{name} element is not supported.");
            }
            if (xpathAxis == XPathAxis.Descendant)
            {
                FindElementFunc func = FindDescendant;
                return func;
            }
            if (xpathAxis == XPathAxis.DescendantOrSelf)
            {
                FindElementFunc func = FindDescendantOrSelf;
                return func;
            }
            if (xpathAxis == XPathAxis.FollowingSibling)
            {
                FindElementFunc func = FindFollowingSibling;
                return func;
            }
            if (xpathAxis == XPathAxis.Child)
            {
                FindElementFunc func = FindChild;
                return func;
            }
            if (xpathAxis == XPathAxis.Attribute)
            {
                string lowerCaseName = name.ToLower();
                if (lowerCaseName == "id")
                    return AutomationObjectIds.AutomationIdProperty;
                if (lowerCaseName == "name")
                    return AutomationObjectIds.NameProperty;
                if (lowerCaseName == "class")
                    return AutomationObjectIds.ClassNameProperty;
                if (lowerCaseName == "type")
                    return UiAutomationElement.ProgrammaticName;
                if (lowerCaseName == "typeid")
                    return UiAutomationElement.Id;
                throw new NotSupportedException($"Attribute {name} is not supportet.");
            }
            return null;
        }

        public object JoinStep(object left, object right)
        {
            if (left is AutomationElement elem && right is GetElementFunc func)
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

                    if (elem.FrameworkAutomationElement.TryGetPropertyValue(propertyId, out var propValue))
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
