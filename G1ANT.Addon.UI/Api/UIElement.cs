using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using G1ANT.Addon.UI.Enums;
using G1ANT.Addon.UI.XPathParser;
using G1ANT.Addon.UI.ExtensionMethods;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;
using ControlType = FlaUI.Core.Definitions.ControlType;
using InvokePattern = FlaUI.UIA3.Patterns.InvokePattern;
using SelectionItemPattern = FlaUI.UIA3.Patterns.SelectionItemPattern;
using ValuePattern = FlaUI.UIA3.Patterns.ValuePattern;
using FlaUI.Core.Definitions;

namespace G1ANT.Addon.UI.Api
{
    public partial class UIElement
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

        public AutomationElement AutomationElement { get; private set; }

        private UIElement() { }

        public UIElement(AutomationElement element)
        {
            AutomationElement = element ?? throw new NullReferenceException("Cannot create UIElement class from empty AutomationElement");
        }

        public static UIElement FromWPath(WPathStructure wPath)
        {
            return FromWPath(wPath.Value);
        } 

        public static UIElement FromWPath(string wPath)
        {
            var xe = new XPathParser<object>().Parse(wPath, new XPathUIElementBuilder(RootElement?.AutomationElement));
            if (xe is AutomationElement element)
            {
                return new UIElement() { AutomationElement = element };
            }
            throw new NullReferenceException($"Cannot find UI element described by \"{wPath}\".");
        }

        public ToggleState GetToggledState()
        {
            switch(AutomationElement.ControlType)
            {
                case ControlType.CheckBox:
                    return AutomationElement.AsCheckBox().ToggleState;
                case ControlType.RadioButton:
                    return AutomationElement.AsRadioButton().IsChecked ? ToggleState.On : ToggleState.Off;
                default:
                    throw new Exception("Element is not CheckBox or RadioButton");
            }
        }

        private IEnumerable<AutomationNodeDescription> GetStackNodes(AutomationElement rootElement)
        {
            var elementStack = new Stack<AutomationNodeDescription>();
            var node = AutomationElement;
            var automationRoot = rootElement ?? AutomationSingleton.Automation.GetDesktop();
            var walker = automationRoot.Automation.TreeWalkerFactory.GetControlViewWalker();

            do
            {
                elementStack.Push(new AutomationNodeDescription(node.Properties));

                var elementParent = walker.GetParent(node);

                if (elementParent.Equals(automationRoot) || elementParent == null)
                {
                    break;
                }

                node = elementParent;
            }
            while (true);

            return elementStack;
        }

        private WPathStructure cachedWPath;
        public WPathStructure ToWPath(AutomationElement rootElement = null)
        {
            if (cachedWPath == null)
            {
                var automationRoot = rootElement ?? AutomationSingleton.Automation.GetDesktop();
                var nodesDescriptionStack = GetStackNodes(automationRoot);
                var wPath = ConvertNodesDescriptionToWPath(nodesDescriptionStack);
                cachedWPath = new WPathStructure(wPath);
            }
            return cachedWPath;
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

        public void MouseClick(EventTypes eventType, int? x, int? y)
        {
            var srcPoint = AutomationElement.GetClickablePoint();

            if (x.HasValue && x != 0 && y.HasValue && y != 0 && srcPoint != Point.Empty)
            {
                var relative = new Point(AutomationElement.BoundingRectangle.X + x.Value, AutomationElement.BoundingRectangle.Y + y.Value);

                switch (eventType)
                {
                    case EventTypes.MouseLeftClick:
                        Mouse.Click(MouseButton.Left, relative);
                        break;
                    case EventTypes.MouseRightClick:
                        Mouse.RightClick(relative);
                        break;
                    case EventTypes.MouseDoubleClick:
                        Mouse.DoubleClick(MouseButton.Left, relative);
                        break;
                }
            }
            else
            {
                switch (eventType)
                {
                    case EventTypes.MouseLeftClick:
                        AutomationElement.Click(true);
                        break;
                    case EventTypes.MouseRightClick:
                        AutomationElement.RightClick(true);
                        break;
                    case EventTypes.MouseDoubleClick:
                        AutomationElement.DoubleClick(true);
                        break;
                }
            }
        }

        public void Click()
        {
            if (AutomationElement.IsPatternSupported(InvokePattern.Pattern) && AutomationElement.Patterns.Invoke.TryGetPattern(out var invokePattern))
            {
                (invokePattern as InvokePattern)?.Invoke();
            }
            else if (AutomationElement.IsPatternSupported(SelectionItemPattern.Pattern) && AutomationElement.Patterns.SelectionItem.TryGetPattern(out var selectionPattern))
            {
                (selectionPattern as SelectionItemPattern)?.Select();
            }
            else if (AutomationElement.TryGetClickablePoint(out var pt))
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
                throw new Exception($"Could not click element: {AutomationElement.Name}");
            }
        }

        public void SetFocus()
        {
            AutomationElement.Focus();
            var currentFocusedElement = AutomationSingleton.Automation.FocusedElement();
            if (!AutomationElement.Equals(currentFocusedElement))
            {
                var parentWindow = AutomationElement.GetParentElementClosestToDesktopElement();
                if (parentWindow != null)
                {
                    parentWindow.Focus();
                    AutomationElement.Focus();
                }
            }
        }

        public void SetText(string text, int timeout)
        {
            if (AutomationElement.Patterns.Value.IsSupported && AutomationElement.Patterns.Value.TryGetPattern(out var valuePattern))
            {
                AutomationElement.Focus();
                ((ValuePattern)valuePattern).SetValue(text);
            }
            else if (AutomationElement.Properties.NativeWindowHandle != IntPtr.Zero)
            {
                AutomationElement.Focus();
                IntPtr wndHandle = AutomationElement.FrameworkAutomationElement.NativeWindowHandle;
                KeyboardTyper.TypeWithSendInput($"{SpecialChars.KeyBegin}ctrl+home{SpecialChars.KeyEnd}", null, wndHandle, IntPtr.Zero, timeout, false, 0); // Move to start of control
                KeyboardTyper.TypeWithSendInput($"{SpecialChars.KeyBegin}ctrl+shift+end{SpecialChars.KeyEnd}", null, wndHandle, IntPtr.Zero, timeout, false, 0); // Select everything
                KeyboardTyper.TypeWithSendInput(text, null, wndHandle, IntPtr.Zero, timeout, false, 0);
            }
            else
                throw new NotSupportedException("SetText is not supported");
        }

        public Rectangle GetRectangle()
        {

            if (AutomationElement.Properties.BoundingRectangle.TryGetValue(out var boundingRectNoDefault))
            {
                return boundingRectNoDefault;
            }

            if (AutomationElement.FrameworkAutomationElement.NativeWindowHandle != IntPtr.Zero)
            {
                var rect = new RobotWin32.Rect();
                IntPtr wndHandle = AutomationElement.FrameworkAutomationElement.NativeWindowHandle;
                if (RobotWin32.GetWindowRectangle(wndHandle, ref rect))
                {
                    return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                }
            }
            throw new NotSupportedException("Cannot get rectangle for that kind of UI element.");
        }

        public string GetText()
        {
            return AutomationElement.GetText();
        }
    }
}
