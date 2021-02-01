using G1ANT.Addon.UI.Api.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G1ANT.Addon.UI.Api
{
    public class UIPatternWrapper : IUIPattern
    {
        private IUIPattern pattern;
        public UIPatternWrapper(IUIPattern pattern)
        {
            this.pattern = pattern;
        }

        public string PatternName => pattern.PatternName;

        public IList<string> AvailableProperties => pattern.AvailableProperties;

        public object GetPropertyValue(string name)
        {
            return pattern.GetPropertyValue(name);
        }

        public void SetPropertyValue(string name, object value)
        {
            pattern.SetPropertyValue(name, value);
        }
    }
}
