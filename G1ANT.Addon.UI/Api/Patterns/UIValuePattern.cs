using FlaUI.Core.AutomationElements;
using System;

namespace G1ANT.Addon.UI.Api.Patterns
{
    public class UIValuePattern : UIPatternBase
    {
        private const string PropIsReadOnly = "isreadonly";
        private const string PropValue = "value";

        public UIValuePattern(AutomationElement automationElement) : base(automationElement)
        {
            indexes.Add(PropIsReadOnly);
            indexes.Add(PropValue);
        }

        public override string PatternName => "Value";

        public override object GetPropertyValue(string name)
        {
            switch (name.ToLower())
            {
                case PropIsReadOnly:
                    return automationElement.Patterns.Value.Pattern.IsReadOnly.Value;
                case PropValue:
                    return automationElement.Patterns.Value.Pattern.Value.Value;
            }
            throw new ArgumentException($"Unknown index '{name}'");
        }

        public override void SetPropertyValue(string name, object value)
        {
            switch (name.ToLower())
            {
                case PropValue:
                    automationElement.Patterns.Value.Pattern.SetValue(value.ToString());
                    break;
                default:
                    throw new ArgumentException($"Index '{name}' is read only");
            }
        }
    }
}
