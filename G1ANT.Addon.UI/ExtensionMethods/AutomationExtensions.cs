using System.Windows.Automation;

namespace G1ANT.Addon.UI.ExtensionMethods
{
    public static class AutomationExtensions
    {
        public static string GetText(this AutomationElement element)
        {
            object patternObj;

            if (element.TryGetCurrentPattern(ValuePattern.Pattern, out patternObj))
            {
                var valuePattern = (ValuePattern)patternObj;
                return valuePattern.Current.Value;
            }
            else if (element.TryGetCurrentPattern(TextPattern.Pattern, out patternObj))
            {
                var textPattern = (TextPattern)patternObj;
                return textPattern.DocumentRange.GetText(-1).TrimEnd('\r');
            }
            return element.Current.Name;
        }
    }
}
