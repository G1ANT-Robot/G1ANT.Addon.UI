using FlaUI.Core;
using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Api.Patterns;
using G1ANT.Addon.UI.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

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
                if (automationElement == null)
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

        private IList<UIElement> GetChildren()
        {
            return automationElement.FindAllChildren().Select(x => new UIElement(x)).ToList();
        }

        private bool IsAutomationPropertySupported(string propName)
        {
            var propInstance = GetAutomationPropertyObject(propName);
            if (propInstance == null)
                return false;
            return propInstance.GetType().GetProperty("IsSupported")?.GetValue(propInstance) is bool isSupported && isSupported;
        }

        private object GetAutomationPropertyValue(string propName)
        {
            var propInstance = GetAutomationPropertyObject(propName);
            if (propInstance == null)
                return false;
            var value = propInstance.GetType().GetProperty("Value")?.GetValue(propInstance);
            if (value is Enum)
                return value.ToString();
            return value;
        }

        private object GetAutomationPropertyObject(string propName)
        {
            var propDef = typeof(FrameworkAutomationElementBase.IProperties).GetProperties().Where(x => x.Name == propName).FirstOrDefault();
            if (propDef == null)
                return null;

            return propDef.GetValue(automationElement.Properties);
        }

        private List<UIPatternWrapper> supportedPatterns;
        private List<UIPatternWrapper> SupportedPatterns()
        {
            if (supportedPatterns == null)
            {
                supportedPatterns = new List<UIPatternWrapper>();
                if (automationElement.Patterns.Value.IsSupported)
                    supportedPatterns.Add(new UIPatternWrapper(new UIValuePattern(automationElement)));
                if (automationElement.Patterns.LegacyIAccessible.IsSupported)
                    supportedPatterns.Add(new UIPatternWrapper(new UILegacyIAccessiblePattern(automationElement)));
            }
            return supportedPatterns;
        }
    }
}
