﻿using FlaUI.Core.AutomationElements;
using FlaUI.UIA3.Patterns;

namespace G1ANT.Addon.UI.ExtensionMethods
{
    public static class AutomationExtensions
    {
        public static string GetText(this AutomationElement element)
        {
            object patternObj = null;
            if (element.FrameworkAutomationElement.TryGetNativePattern(ValuePattern.Pattern, out patternObj))
            {
                var valuePattern = (ValuePattern)patternObj;
                return valuePattern.Value;
            }
            else if (element.FrameworkAutomationElement.TryGetNativePattern(TextPattern.Pattern, out patternObj))
            {
                var textPattern = (TextPattern)patternObj;
                return textPattern.DocumentRange.GetText(-1).TrimEnd('\r');
            }
            return element.Properties.Name;
        }
    }
}