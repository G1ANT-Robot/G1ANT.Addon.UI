using FlaUI.Core.AutomationElements;
using FlaUI.UIA3.Patterns;

namespace G1ANT.Addon.UI.ExtensionMethods
{
    public static class AutomationExtensions
    {
        public static string GetText(this AutomationElement element)
        {
            if (element.Patterns.Value.IsSupported)
            {
                return element.Patterns.Value.Pattern.Value;
            }

            if (element.Patterns.Text.IsSupported)
            {
                return element.Patterns.Text.Pattern.DocumentRange.GetText(-1).TrimEnd('\r');
            }

            return element.Properties.Name;
        }


        public static AutomationElement GetParentElementClosestToDesktopElement(this AutomationElement element)
        {
            var desktop = AutomationSingleton.Automation.GetDesktop();
            var currentElement = element;
            while (!desktop.Equals(currentElement.Parent))
            {
                currentElement = currentElement.Parent;
            }

            return currentElement;
        }

    }
}
