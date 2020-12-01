using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using System;
using System.Windows.Media.Animation;

namespace G1ANT.Addon.UI.Api.Patterns
{
    public class UILegacyIAccessiblePattern : UIPatternBase
    {
        class Indexes
        {
            public const string Value = "value";
            public const string DefaultAction = "defaultaction";
            public const string Description = "description";
            public const string Help = "help";
            public const string Name = "name";
            public const string Role = "role";
            public const string State = "state";
            public const string KeyboardShortcut = "shortcut";
        }

        public UILegacyIAccessiblePattern(AutomationElement automationElement) : base(automationElement)
        {
            var pattern = automationElement.Patterns.LegacyIAccessible.Pattern;
            if (pattern.Value.IsSupported)
                indexes.Add(Indexes.Value);
            if (pattern.DefaultAction.IsSupported)
                indexes.Add(Indexes.DefaultAction);
            if (pattern.Description.IsSupported)
                indexes.Add(Indexes.Description);
            if (pattern.Help.IsSupported)
                indexes.Add(Indexes.Help);
            if (pattern.Name.IsSupported)
                indexes.Add(Indexes.Name);
            if (pattern.Role.IsSupported)
                indexes.Add(Indexes.Role);
            if (pattern.State.IsSupported)
                indexes.Add(Indexes.State);
            if (pattern.KeyboardShortcut.IsSupported)
                indexes.Add(Indexes.KeyboardShortcut);
        }

        public override string PatternName => "LegacyIAccessible";

        public override object GetPropertyValue(string name)
        {
            var pattern = automationElement.Patterns.LegacyIAccessible.Pattern;
            switch (name.ToLower())
            {
                case Indexes.Value:
                    return pattern.Value.ValueOrDefault;
                case Indexes.DefaultAction:
                    return pattern.DefaultAction.ValueOrDefault;
                case Indexes.Description:
                    return pattern.Description.ValueOrDefault;
                case Indexes.Help:
                    return pattern.Help.ValueOrDefault;
                case Indexes.Name:
                    return pattern.Name.ValueOrDefault;
                case Indexes.State:
                    return AccessibilityTextResolver.GetStateText(pattern.State.ValueOrDefault);
                case Indexes.Role:
                    return AccessibilityTextResolver.GetRoleText(pattern.Role.ValueOrDefault);
                case Indexes.KeyboardShortcut:
                    return pattern.KeyboardShortcut.ValueOrDefault;
            }
            throw new ArgumentException($"Unknown index '{name}'");
        }

        public override void SetPropertyValue(string name, object value)
        {
            switch (name.ToLower())
            {
                case Indexes.Value:
                    automationElement.Patterns.LegacyIAccessible.Pattern.SetValue(value.ToString());
                    break;
                default:
                    throw new ArgumentException($"Index '{name}' is read only");
            }
        }
    }
}
