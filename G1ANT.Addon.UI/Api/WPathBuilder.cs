using FlaUI.Core.AutomationElements;
using FlaUI.Core.Exceptions;
using G1ANT.Addon.UI.ExtensionMethods;
using G1ANT.Addon.UI.Structures;
using System.Collections.Generic;
using System.Linq;

namespace G1ANT.Addon.UI.Api
{
    public class WPathBuilder
    {
        class UIElementCachedProperties : UIElement
        {
            private Dictionary<string, object> cachedProperties = new Dictionary<string, object>();
            public UIElementCachedProperties(AutomationElement element, int index) : base(element, index)
            {
            }

            public override object GetPropertyValue(string propName)
            {
                propName = propName.ToLower();
                if (!cachedProperties.ContainsKey(propName))
                {
                    object result;
                    try
                    {
                        result = base.GetPropertyValue(propName);
                    }
                    catch
                    {
                        result = null;
                    }
                    cachedProperties[propName] = result;
                }
                return cachedProperties[propName];
            }

        }

        private const string DescendantPrefix = "/descendant::";
        public static Dictionary<string, bool> SearchByProperties = new Dictionary<string, bool>()
        {
            { UIElement.Indexes.Id, true },
            { UIElement.Indexes.Class, true },
            { UIElement.Indexes.Name, true },
            { UIElement.Indexes.Type, true },
            { UIElement.Indexes.Index, false }
        };
        private string[] GetSearchByProperties() => SearchByProperties.Where(x => x.Value).Select(x => x.Key).ToArray();

        public WPathBuilder()
        {
        }

        public WPathStructure GetWPathStructure(AutomationElement element, AutomationElement rootElement = null)
        {
            return new WPathStructure(GetSimpleWPath(element, rootElement));
        }

        public string GetWPath(AutomationElement element, AutomationElement rootElement = null)
        {
            var automationRoot = rootElement ?? AutomationSingleton.Automation.GetDesktop();
            var nodesDescriptionStack = BuildUIElementsStack(element, automationRoot);
            return ConvertNodesDescriptionToWPath(nodesDescriptionStack.Pop(), nodesDescriptionStack);
        }

        public string GetSimpleWPath(AutomationElement element, AutomationElement rootElement = null,
            List<string> properties = null)
        {
            var automationRoot = rootElement ?? AutomationSingleton.Automation.GetDesktop();
            var nodesDescriptionStack = BuildUIElementsStack(element, automationRoot);
            nodesDescriptionStack.Pop();
            return NodesToSimpleWPath(nodesDescriptionStack, properties);
        }

        public Stack<UIElement> BuildUIElementsStack(AutomationElement element, AutomationElement rootElement)
        {
            var stack = BuildAutomationElementsStack(element, rootElement);
            var elementList = new List<UIElement>();
            AutomationElement parent = null;

            foreach (var item in stack)
            {
                var index = FindElementIndexInParent(parent, item);
                var uiElement = new UIElementCachedProperties(item, index);
                elementList.Add(uiElement);
                parent = item;
            }
            elementList.Reverse();
            return new Stack<UIElement>(elementList);
        }

        private int FindElementIndexInParent(AutomationElement parent, AutomationElement element)
        {
            if (parent == null)
                return 0;

            var index = 0;
            var treeWalker = parent.GetTreeWalker();
            var elementNode = treeWalker.GetFirstChild(parent);
            while (elementNode != null)
            {
                if (elementNode.Equals(element))
                    return index;
                index++;
                elementNode = treeWalker.GetNextSibling(elementNode);
            }
            throw new ElementNotAvailableException();
        }

        private Stack<AutomationElement> BuildAutomationElementsStack(AutomationElement element, AutomationElement rootElement)
        {
            var elementStack = new Stack<AutomationElement>();
            var node = element;
            var automationRoot = rootElement ?? AutomationSingleton.Automation.GetDesktop();
            var walker = automationRoot.GetTreeWalker();

            do          
            {
                try
                {
                    // is element still alive?     
                    var isSupported = node.Properties.Name.IsSupported;
                }
                catch 
                {
                    break;
                }
                elementStack.Push(node);
                var elementParent = walker.GetParent(node);
                if (elementParent == null)
                {
                    break;
                }

                node = elementParent;
            }
            while (true);
            return elementStack;
        }

        private string NodesToSimpleWPath(Stack<UIElement> nodesStack, List<string> properties = null)
        {
            string wpath = "";
            var fillPropsSet = properties ?? GetSearchByProperties().ToList();

            foreach (var node in nodesStack)
            {
                var xpath = BuildXpathPart(node, fillPropsSet);
                wpath += $"/{xpath}";
            }
            return wpath;
        }

        private string ConvertNodesDescriptionToWPath(UIElement parent, Stack<UIElement> nodesDescriptionStack)
        {
            if (nodesDescriptionStack.Count == 0)
                return string.Empty;

            var currentElement = nodesDescriptionStack.Pop();
            var tmp = parent.GetPropertyValue(UIElement.Indexes.Children) as List<object>;
            var children = tmp?.Cast<UIElement>().Select(
                (x, index) => new UIElementCachedProperties(x.AutomationElement, index)).ToList<UIElement>();

            if (children.Count == 1 && nodesDescriptionStack.Count > 1)
            {
                var xpath = ConvertNodesDescriptionToWPath(currentElement, nodesDescriptionStack);
                if (string.IsNullOrEmpty(xpath) || xpath.StartsWith(DescendantPrefix))
                    return xpath;
                return DescendantPrefix + xpath.Remove(0, 1);
            }

            foreach (var searchByProps in GetPossibleCombinations(GetSearchByProperties()))
            {
                if (HasUniqueProperties(searchByProps, currentElement, children))
                {
                    var xpath = BuildXpathPart(currentElement, searchByProps);
                    return $"/{xpath}" + ConvertNodesDescriptionToWPath(currentElement, nodesDescriptionStack);
                }
            }

            var currentElementIndex = children.FindIndex(x => x.Equals(currentElement));
            return $"/ui[{currentElementIndex}]" + ConvertNodesDescriptionToWPath(currentElement, nodesDescriptionStack);
        }

        private bool HasUniqueProperties(List<string> propNames, UIElement element, List<UIElement> searchIn)
        {
            var currentHash = GetElementPropertiesHash(propNames, element);
            if (string.IsNullOrEmpty(currentHash))
                return false;
            foreach (var e in searchIn)
            {
                if (!element.Equals(e))
                {
                    var hash = GetElementPropertiesHash(propNames, e);
                    if (currentHash == hash)
                        return false;
                }
            }
            return true;
        }

        private string BuildFilterCondition(UIElement element, List<string> propNames)
        {
            List<string> filters = new List<string>();
            foreach (var propName in propNames)
            {
                var val = element.GetPropertyValue(propName);
                if (val != null)
                {
                    filters.Add(BuildFilterPart(propName, val));
                }
            }
            return string.Join(" and ", filters);
        }

        private string BuildFilterPart(string name, object value)
        {
            if (value.IsNumber())
                return $"@{name}={value}";
            else
                return $"@{name}='{value}'";
        }

        private string BuildXpathPart(UIElement element, List<string> properties)
        {
            var filter = BuildFilterCondition(element, properties);
            return $"ui[{filter}]";
        }

        private string GetElementPropertiesHash(List<string> propNames, UIElement element)
        {
            string result = "";
            foreach (var propName in propNames)
            {
                var val = element.GetPropertyValue(propName);
                if (val != null)
                    result += $"#{val}";
            }
            return result;
        }

        private static List<List<string>> GetCombinations(string[] elements, int k)
        {
            List<List<string>> result = new List<List<string>>();

            int N = elements.Length;

            if (k > N)
                return null;

            // init combination index array
            int[] pointers = new int[k];


            int r = 0; // index for combination array
            int i = 0; // index for elements array

            while (r >= 0)
            {

                // forward step if i < (N + (r-K))
                if (i <= (N + (r - k)))
                {
                    pointers[r] = i;

                    // if combination array is full print and increment i;
                    if (r == k - 1)
                    {
                        result.Add(pointers.Select(x => elements[x]).ToList());
                        i++;
                    }
                    else
                    {
                        // if combination is not full yet, select next element
                        i = pointers[r] + 1;
                        r++;
                    }
                }

                // backward step
                else
                {
                    r--;
                    if (r >= 0)
                        i = pointers[r] + 1;

                }
            }
            return result;
        }

        private static List<List<string>> GetPossibleCombinations(string[] initialElement)
        {
            return Enumerable.Range(1, initialElement.Length).
                SelectMany(x => GetCombinations(initialElement, x)).ToList();
        }
    }
}
