using FlaUI.Core;
using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Api.Patterns;
using G1ANT.Addon.UI.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace G1ANT.Addon.UI.Api
{
    public partial class UIElement
    {
        private const string PropPatterns = "patterns";
        private const string PropChildren = "children";

        private Dictionary<string, string> supportedProperties = new Dictionary<string, string>()
        {
            { "id", "AutomationId" },
            { "name", "Name" },
            { "class", "ClassName" },
            { "controltype", "ControlType" },
            { "isenabled", "IsEnabled" },
            { "clickablepoint", "ClickablePoint" },
            { "centerpoint", "CenterPoint" },
            { "boundingrectangle", "BoundingRectangle" },
            { "ispassword", "IsPassword" },
            { "description", "FullDescription" },
            { "helptext", "HelpText" },
        };

        private List<string> availableProperties = null;
        public IList<string> AvailableProperties
        {
            get
            {
                if (AutomationElement == null)
                    throw new NullReferenceException("Underlying automation element has not been initialised");

                if (availableProperties == null)
                {
                    availableProperties = new List<string>();
                    foreach (var propDef in supportedProperties)
                    {
                        if (IsAutomationPropertySupported(propDef.Value))
                            availableProperties.Add(propDef.Key);
                    }
                    if (SupportedPatterns().Count > 0)
                        availableProperties.Add(PropPatterns);
                    availableProperties.Add(PropChildren);
                }
                return availableProperties;
            }
        }

        public object GetPropertyValue(string name)
        {
            if (!AvailableProperties.Contains(name.ToLower()))
                throw new ArgumentException($"Property {name} is not supported by control {ToWPath().Value}");

            name = name.ToLower();
            if (supportedProperties.ContainsKey(name))
                return GetAutomationPropertyValue(supportedProperties[name]);

            switch (name.ToLower())
            {
                case PropPatterns:
                    return SupportedPatterns().ToDictionary(x => x.PatternName, x => (object)x);
                case PropChildren:
                     return GetChildren().ToList<object>();
            }
            throw new ArgumentException($"Unknown index '{name}'");
        }

        public void SetPropertyValue(string name, object value)
        {
            if (!AvailableProperties.Contains(name.ToLower()))
                throw new ArgumentException($"Property {name} is not supported by control {ToWPath().Value}");

            throw new ArgumentException($"Index '{name}' is read only");
        }

        private IList<UIElement> cachedChildren;
        private IList<UIElement> GetChildren()
        {
            if (cachedChildren == null)
                cachedChildren = AutomationElement.FindAllChildren().Select(x => new UIElement(x)).ToList();
            return cachedChildren;
        }

        private bool IsAutomationPropertySupported(string propName)
        {
            var propInstance = GetAutomationPropertyObject(propName);
            if (propInstance == null)
                return false;
            try
            {
                return propInstance.GetType().GetProperty("IsSupported")?.GetValue(propInstance) is bool isSupported && isSupported;
            }
            catch
            {
                return false;
            }
        }

        private object GetAutomationPropertyValue(string propName)
        {
            var propInstance = GetAutomationPropertyObject(propName);
            if (propInstance == null)
                return false;
            try
            {
                var value = propInstance.GetType().GetProperty("Value")?.GetValue(propInstance);
                if (value is Enum)
                    return value.ToString();
                return value;
            }
            catch
            {
                return null;
            }
        }

        private object GetAutomationPropertyObject(string propName)
        {
            try
            {
                var propDef = typeof(FrameworkAutomationElementBase.IProperties).GetProperties().Where(x => x.Name == propName).FirstOrDefault();
                if (propDef == null)
                    return null;

                return propDef.GetValue(AutomationElement.Properties);
            }
            catch
            {
                return null;
            }
        }

        private List<UIPatternWrapper> supportedPatterns;
        private List<UIPatternWrapper> SupportedPatterns()
        {
            if (supportedPatterns == null)
            {
                supportedPatterns = new List<UIPatternWrapper>();
                if (AutomationElement.Patterns.Value.IsSupported)
                    supportedPatterns.Add(new UIPatternWrapper(new UIValuePattern(AutomationElement)));
                if (AutomationElement.Patterns.LegacyIAccessible.IsSupported)
                    supportedPatterns.Add(new UIPatternWrapper(new UILegacyIAccessiblePattern(AutomationElement)));
                if (AutomationElement.Patterns.Selection.IsSupported)
                    supportedPatterns.Add(new UIPatternWrapper(new UISelectionPattern(AutomationElement)));
                if (AutomationElement.Patterns.SelectionItem.IsSupported)
                    supportedPatterns.Add(new UIPatternWrapper(new UISelectionItemPattern(AutomationElement)));
            }
            return supportedPatterns;
        }
    }
}
