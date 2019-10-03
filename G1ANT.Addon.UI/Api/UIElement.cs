using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Identifiers;
using FlaUI.UIA3.Identifiers;
using G1ANT.Addon.UI.XPathParser;
using G1ANT.Addon.UI.ExtensionMethods;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;
using ControlType = FlaUI.Core.Definitions.ControlType;
using InvokePattern = FlaUI.UIA3.Patterns.InvokePattern;
using SelectionItemPattern = FlaUI.UIA3.Patterns.SelectionItemPattern;
using ValuePattern = FlaUI.UIA3.Patterns.ValuePattern;
using Rect = System.Windows.Rect;
namespace G1ANT.Addon.UI.Api
{
    public class UIElement
    {
        internal class AutomationNodeDescription
        {
            public string id;
            public string name;
            public string className;
            public ControlType type;

            public AutomationNodeDescription(FrameworkAutomationElementBase.IProperties properties)
            {
                id = properties.AutomationId.ValueOrDefault;
                name = properties.Name.ValueOrDefault;
                className = properties.ClassName.ValueOrDefault;
                type = properties.ControlType.ValueOrDefault;
            }
        }
        public static UIElement RootElement { get; set; }    

        protected AutomationElement automationElement;

        private UIElement(){}

        public UIElement(AutomationElement element)
        {
            automationElement = element ?? throw new NullReferenceException("Cannot create UIElement class from empty AutomationElement");
        }

        public static UIElement FromWPath(WPathStructure wPath)
        {
            var xe = new XPathParser<object>().Parse(wPath.Value, new XPathUIElementBuilder(RootElement?.automationElement));
            if (xe is AutomationElement element)
            {
                return new UIElement(){ automationElement = element };
            }
            throw new NullReferenceException($"Cannot find UI element described by \"{wPath.Value}\".");
        }

        private IEnumerable<AutomationNodeDescription> GetStackNodes(AutomationElement rootElement)
        {
            var elementStack = new Stack<AutomationNodeDescription>();
            var node = automationElement;
            var automationRoot = rootElement ?? AutomationSingleton.Automation.GetDesktop();
            var walker = automationRoot.Automation.TreeWalkerFactory.GetControlViewWalker();

            do
            {
                elementStack.Push(new AutomationNodeDescription(node.Properties));

                var elementParent = walker.GetParent(node);

                if (elementParent == automationRoot || elementParent == null)
                {
                    break;
                }

                node = elementParent;
            }
            while (true);

            return elementStack;
        }

        public WPathStructure ToWPath(AutomationElement rootElement = null)
        {
            var automationRoot = rootElement ?? AutomationSingleton.Automation.GetDesktop();
            var nodesDescriptionStack = GetStackNodes(automationRoot);
            var wPath = ConvertNodesDescriptionToWPath(nodesDescriptionStack);
            return new WPathStructure(wPath);
        }

        private string ConvertNodesDescriptionToWPath(IEnumerable<AutomationNodeDescription> nodesDescriptionStack)
        {
            var isParentEmpty = false;
            var result = "";

            foreach (var element in nodesDescriptionStack)
            {
                if (IsParentEmpty(element))
                {
                    isParentEmpty = true;
                }
                else
                {
                    var xpath = "";
                    if (isParentEmpty)
                    {
                        xpath += "descendant::";
                    }
                    if (!string.IsNullOrEmpty(element.id))
                    {
                        xpath += $"ui[@id='{element.id}']";
                    }
                    else if (!string.IsNullOrEmpty(element.name))
                    {
                        xpath += $"ui[@name='{element.name}']";
                    }
                    result += $"/{xpath}";
                    isParentEmpty = false;
                }
            }

            return result;
        }

        private static bool IsParentEmpty(AutomationNodeDescription element)
        {
            return string.IsNullOrEmpty(element.id) && string.IsNullOrEmpty(element.name);
        }

        public void Click()
        {
            if (automationElement.IsPatternSupported(InvokePattern.Pattern) && automationElement.Patterns.Invoke.TryGetPattern(out var invokePattern))
            {
                (invokePattern as InvokePattern)?.Invoke();
            }
            else if (automationElement.IsPatternSupported(SelectionItemPattern.Pattern) && automationElement.Patterns.SelectionItem.TryGetPattern(out var selectionPattern))
            {
                (selectionPattern as SelectionItemPattern)?.Select();
            }
            else if (automationElement.TryGetClickablePoint(out var pt))
            {
                var tempPos = MouseWin32.GetPhysicalCursorPosition();
                var currentPos = new Point(tempPos.X, tempPos.Y);
                var targetPos = new Point(pt.X, pt.Y);
                var mouseArgs = MouseStr.ToMouseEventsArgs(targetPos.X, targetPos.Y, currentPos.X, currentPos.Y, "left", "press", 1);

                foreach (var arg in mouseArgs)
                {
                    MouseWin32.MouseEvent(arg.dwFlags, arg.dx, arg.dy, arg.dwData);
                    Thread.Sleep(10);
                }
            }
            else
            {
                throw new Exception($"Could not click element: {automationElement.Name}");
            }
        }

        public void SetFocus()
        {
            automationElement.Focus();
        }

        public void SetText(string text, int timeout)
        {
            if (automationElement.Patterns.Value.IsSupported && automationElement.Patterns.Value.TryGetPattern(out var valuePattern))
            {
                automationElement.Focus();
                ((ValuePattern)valuePattern).SetValue(text);
            }
            else if (automationElement.Properties.NativeWindowHandle != IntPtr.Zero)
            {
                automationElement.Focus();
                IntPtr wndHandle = automationElement.FrameworkAutomationElement.NativeWindowHandle;
                KeyboardTyper.TypeWithSendInput($"{SpecialChars.KeyBegin}ctrl+home{SpecialChars.KeyEnd}", null, wndHandle, IntPtr.Zero, timeout, false, 0); // Move to start of control
                KeyboardTyper.TypeWithSendInput($"{SpecialChars.KeyBegin}ctrl+shift+end{SpecialChars.KeyEnd}", null, wndHandle, IntPtr.Zero, timeout, false, 0); // Select everything
                KeyboardTyper.TypeWithSendInput(text, null, wndHandle, IntPtr.Zero, timeout, false, 0);
            }
            else
                throw new NotSupportedException("SetText is not supported");
        }

        public Rectangle GetRectangle()
        {

            if (automationElement.Properties.BoundingRectangle.TryGetValue(out var boundingRectNoDefault))
            {
                return boundingRectNoDefault;
            }

            if (automationElement.FrameworkAutomationElement.NativeWindowHandle != IntPtr.Zero)
            {
                var rect = new RobotWin32.Rect();
                IntPtr wndHandle = automationElement.FrameworkAutomationElement.NativeWindowHandle;
                if (RobotWin32.GetWindowRectangle(wndHandle, ref rect))
                {
                    return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                }
            }
            throw new NotSupportedException("Cannot get rectangle for that kind of UI element.");
        }

        public string GetText()
        {
            return automationElement.GetText();
        }
    }
}
