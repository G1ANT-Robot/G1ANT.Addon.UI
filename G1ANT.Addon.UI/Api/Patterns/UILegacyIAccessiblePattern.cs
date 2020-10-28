using FlaUI.Core.AutomationElements;
using System;
using System.Windows.Media.Animation;

namespace G1ANT.Addon.UI.Api.Patterns
{
    public class UILegacyIAccessiblePattern : UIPatternBase
    {
        private const string PropValue = "value";
        private const string PropDefaultAction = "defaultaction";
        private const string PropDescription = "description";
        private const string PropHelp = "help";
        private const string PropName = "name";

        public UILegacyIAccessiblePattern(AutomationElement automationElement) : base(automationElement)
        {
            var pattern = automationElement.Patterns.LegacyIAccessible.Pattern;
            if (pattern.Value.IsSupported)
                indexes.Add(PropValue);
            if (pattern.DefaultAction.IsSupported)
                indexes.Add(PropDefaultAction);
            if (pattern.Description.IsSupported)
                indexes.Add(PropDescription);
            if (pattern.Help.IsSupported)
                indexes.Add(PropHelp);
            if (pattern.Name.IsSupported)
                indexes.Add(PropName);
        }

        public override string PatternName => "LegacyIAccessible";

        public override object GetPropertyValue(string name)
        {
            var pattern = automationElement.Patterns.LegacyIAccessible.Pattern;
            switch (name.ToLower())
            {
                case PropValue:
                    return pattern.Value.ValueOrDefault;
                case PropDefaultAction:
                    return pattern.DefaultAction.ValueOrDefault;
                case PropDescription:
                    return pattern.Description.ValueOrDefault;
                case PropHelp:
                    return pattern.Help.ValueOrDefault;
                case PropName:
                    return pattern.Name.ValueOrDefault;
            }
            throw new ArgumentException($"Unknown index '{name}'");
        }

        public override void SetPropertyValue(string name, object value)
        {
            switch (name.ToLower())
            {
                case PropValue:
                    automationElement.Patterns.LegacyIAccessible.Pattern.SetValue(value.ToString());
                    break;
                default:
                    throw new ArgumentException($"Index '{name}' is read only");
            }
        }
    }
}
