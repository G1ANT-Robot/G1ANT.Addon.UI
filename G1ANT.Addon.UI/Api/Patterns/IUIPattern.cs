using System.Collections.Generic;

namespace G1ANT.Addon.UI.Api.Patterns
{
    public interface IUIPattern
    {
        string PatternName { get; }
        IList<string> AvailableProperties { get; }
        object GetPropertyValue(string name);
        void SetPropertyValue(string name, object value);
    }
}
