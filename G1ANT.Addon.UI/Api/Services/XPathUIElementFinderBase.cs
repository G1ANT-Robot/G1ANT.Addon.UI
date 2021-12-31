using G1ANT.Addon.UI.Api.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G1ANT.Addon.UI.Api.Services
{
    using CompareFunc = System.Func<G1ANT.Addon.UI.Api.UIElement, int, bool>;

    public abstract class XPathUIElementFinderBase : IXPathUIElementFinder
    {
        public abstract UIElement FindDescendant(UIElement elem, CompareFunc compare);

        public abstract UIElement FindChild(UIElement elem, CompareFunc compare);

        public abstract UIElement FindFollowingSibling(UIElement elem, CompareFunc compare);

        public abstract UIElement FindDescendantOrSelf(UIElement elem, CompareFunc compare);

        public virtual object GetPropertyValue(UIElement element, string propertyName)
        {
            if (!element.IsPropertySupported(propertyName))
                return null;
            return element.GetPropertyValue(propertyName);
        }

        public object EqOperator(object left, object right)
        {
            if (left is string propertyName)
            {
                return new CompareFunc((elem, index) =>
                {
                    var value = GetPropertyValue(elem, propertyName);
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

        public object Number(string value)
        {
            int result = -1;
            if (int.TryParse(value, out result))
                return result;
            throw new NotSupportedException($"Number '{value}' is not supported.");
        }

        public object AndOperator(object left, object right)
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

        public object OrOperator(object left, object right)
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

        public object Function(string prefix, string name, IList<object> args)
        {
            switch (name.ToLower())
            {
                case "starts-with":
                case "ends-with":
                case "contains":
                    return BuildStringCompareFunction(name, args);
            }

            throw new NotSupportedException($"Function {name} is not supported.");
        }

        private CompareFunc BuildStringCompareFunction(string functionName, IList<object> args)
        {
            if (args.Count == 2 && args[0] is string propertyName && args[1] is string text)
            {
                CompareFunc func = (elem, index) =>
                {
                    if (!elem.IsPropertySupported(propertyName))
                        return false;
                    var value = elem.GetPropertyValue(propertyName);
                    return value is string str && IsConditionMet(functionName, str, text);
                };
                return func;
            }
            throw new NotSupportedException($"Function {functionName} should has two parameters.");
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
    }
}
