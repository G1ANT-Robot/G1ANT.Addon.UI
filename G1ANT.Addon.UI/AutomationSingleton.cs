using FlaUI.Core;
using FlaUI.UIA3;

namespace G1ANT.Addon.UI
{
    internal static class AutomationSingleton
    {
        private static AutomationBase automationRoot;
        internal static AutomationBase Automation => automationRoot ?? (automationRoot = new UIA3Automation());
    }
}
