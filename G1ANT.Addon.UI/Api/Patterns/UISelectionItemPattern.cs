using FlaUI.Core.AutomationElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G1ANT.Addon.UI.Api.Patterns
{
    public class UISelectionItemPattern : UIPatternBase
    {
        private const string PropIsSelected = "isselected";

        public UISelectionItemPattern(AutomationElement automationElement) : base(automationElement)
        {
            indexes.Add(PropIsSelected);
        }

        public override string PatternName => "SelectionItem";

        public override object GetPropertyValue(string name)
        {
            var pattern = automationElement.Patterns.SelectionItem.Pattern;
            switch (name.ToLower())
            {
                case PropIsSelected:
                    return pattern.IsSelected.Value;
            }
            throw new ArgumentException($"Unknown index '{name}'");
        }

        public override void SetPropertyValue(string name, object value)
        {
            switch (name.ToLower())
            {
                case PropIsSelected:
                    if (value is bool sel && sel)
                        automationElement.Patterns.SelectionItem.Pattern.AddToSelection();
                    else
                        automationElement.Patterns.SelectionItem.Pattern.RemoveFromSelection();
                    break;
                default:
                    throw new ArgumentException($"Index '{name}' is read only");
            }
        }
    }
}
