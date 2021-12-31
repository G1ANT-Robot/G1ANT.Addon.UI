using System;
using System.Collections.Generic;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Exceptions;
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
using G1ANT.Addon.UI.Api.Interfaces;
using G1ANT.Addon.UI.Api.Services;

namespace G1ANT.Addon.UI.Api
{
    public class XPathUIElementBuilder : IXPathBuilder<object>
    {
        private IXPathUIElementFinder uIElementFinder = null;

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
            return uIElementFinder.FindDescendant(elem, compare);
        }

        protected UIElement FindChild(UIElement elem, CompareFunc compare)
        {
            return uIElementFinder.FindChild(elem, compare);
        }

        protected UIElement FindFollowingSibling(UIElement elem, CompareFunc compare)
        {
            return uIElementFinder.FindFollowingSibling(elem, compare);
        }

        protected UIElement FindDescendantOrSelf(UIElement elem, CompareFunc compare)
        {
            return uIElementFinder.FindDescendantOrSelf(elem, compare);
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
            return uIElementFinder.Number(value);
        }

        protected object EqOperator(object left, object right)
        {
            if (left is string propertyName)
            {
                return new CompareFunc((elem, index) =>
                {
                    if (!elem.IsPropertySupported(propertyName))
                        return false;
                    var value = elem.GetPropertyValue(propertyName);
                    if (value == null)
                        return false;
                    if (value.GetType() == right.GetType())
                        return value.Equals(right);
                    try
                    {
                        return value.Equals(Convert.ChangeType(right, value.GetType()));
                    }
                    catch
                    {
                        return value.ToString().Equals(right);
                    }
                });
            }
            throw new NotSupportedException("Left side of 'equal' operator has not been recognized.");
        }

        public object Operator(XPathOperator op, object left, object right)
        {
            switch (op)
            {
                case XPathOperator.Eq:
                    return uIElementFinder.EqOperator(left, right);
                case XPathOperator.And:
                    return uIElementFinder.AndOperator(left, right);
                case XPathOperator.Or:
                    return uIElementFinder.OrOperator(left, right);
            }
            throw new NotSupportedException($"Operator {op} is not supported.");
        }

        public object Axis(XPathAxis xpathAxis, System.Xml.XPath.XPathNodeType nodeType, string prefix, string name)
        {
            if (nodeType == System.Xml.XPath.XPathNodeType.Element)
            {
                switch (name.ToLower())
                {
                    case "v2":
                        uIElementFinder = new XPathUIElementFinderV2();
                        return (GetElementFunc)((element) => element);
                    case "ui":
                        uIElementFinder = uIElementFinder ?? new XPathUIElementFinderV1(Root);
                        break;
                    default:
                        throw new NotSupportedException($"{name} element is not supported.");
                }
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
                    return name.ToLower();
            }
            return null;
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
            return uIElementFinder.Function(prefix, name, args);
        }
    }
}
