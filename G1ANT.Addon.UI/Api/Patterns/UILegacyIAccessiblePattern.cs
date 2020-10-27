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
            if (automationElement.Patterns.LegacyIAccessible.Pattern.Value.IsSupported)
                indexes.Add(PropValue);
            if (automationElement.Patterns.LegacyIAccessible.Pattern.DefaultAction.IsSupported)
                indexes.Add(PropDefaultAction);
            if (automationElement.Patterns.LegacyIAccessible.Pattern.Description.IsSupported)
                indexes.Add(PropDescription);
            if (automationElement.Patterns.LegacyIAccessible.Pattern.Help.IsSupported)
                indexes.Add(PropHelp);
            if (automationElement.Patterns.LegacyIAccessible.Pattern.Name.IsSupported)
                indexes.Add(PropName);
        }

        public override string PatternName => "LegacyIAccessible";

        public override object GetPropertyValue(string name)
        {
            switch (name.ToLower())
            {
                case PropValue:
                    return automationElement.Patterns.LegacyIAccessible.Pattern.Value.ValueOrDefault;
                case PropDefaultAction:
                    return automationElement.Patterns.LegacyIAccessible.Pattern.DefaultAction.ValueOrDefault;
                case PropDescription:
                    return automationElement.Patterns.LegacyIAccessible.Pattern.Description.ValueOrDefault;
                case PropHelp:
                    return automationElement.Patterns.LegacyIAccessible.Pattern.Help.ValueOrDefault;
                case PropName:
                    return automationElement.Patterns.LegacyIAccessible.Pattern.Name.ValueOrDefault;
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
