using FlaUI.Core.AutomationElements;
using System;
using System.Linq;

namespace G1ANT.Addon.UI.Api.Patterns
{
    public class UISelectionPattern : UIPatternBase
    {
        private const string PropCanSelectMultiple  = "canselectmultiple";
        private const string PropIsSelectionRequired = "isselectionrequired";
        private const string PropSelection = "selection";

        public UISelectionPattern(AutomationElement automationElement) : base(automationElement)
        {
            indexes.Add(PropCanSelectMultiple);
            indexes.Add(PropIsSelectionRequired);
            indexes.Add(PropSelection);
        }

        public override string PatternName => "Selection";

        public override object GetPropertyValue(string name)
        {
            var pattern = automationElement.Patterns.Selection.Pattern;
            switch (name.ToLower())
            {
                case PropCanSelectMultiple:
                    return pattern.CanSelectMultiple.Value;
                case PropIsSelectionRequired:
                    return pattern.IsSelectionRequired.Value;
                case PropSelection:
                    return pattern.Selection.Value.Select(x => new UIElement(x)).ToList<object>();
            }
            throw new ArgumentException($"Unknown index '{name}'");
        }

        public override void SetPropertyValue(string name, object value)
        {
            throw new ArgumentException($"Index '{name}' is read only");
        }
    }
}
