using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3.Patterns;
using G1ANT.Addon.UI.Api;
using System;

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
            while (currentElement != null && !desktop.Equals(currentElement.Parent))
            {
                currentElement = currentElement.GetParentControl();
            }

            return currentElement;
        }

        public static ITreeWalker GetTreeWalker(this AutomationElement element)
        {
            var rootElement = element ?? throw new ArgumentException();
            return rootElement.Automation.TreeWalkerFactory.GetRawViewWalker();
        }

        public static UIElement ToUIElement(this AutomationElement element, int index = -1)
        {
            return new UIElement(element, index);
        }

        public static AutomationElement GetParentControl(this AutomationElement element)
        {
            return element?.Automation.TreeWalkerFactory.GetControlViewWalker().GetParent(element);
        }
    }
}
