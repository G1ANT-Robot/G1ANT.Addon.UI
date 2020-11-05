using FlaUI.Core.AutomationElements;
using System.Collections.Generic;

namespace G1ANT.Addon.UI.Api.Patterns
{
    public abstract class UIPatternBase : IUIPattern
    {
        protected AutomationElement automationElement;
        protected List<string> indexes = new List<string>();

        public UIPatternBase(AutomationElement automationElement)
        {
            this.automationElement = automationElement;
        }

        public IList<string> AvailableProperties => indexes;

        public virtual string PatternName => throw new System.NotImplementedException();

        public virtual object GetPropertyValue(string name)
        {
            throw new System.NotImplementedException();
        }

        public virtual void SetPropertyValue(string name, object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
