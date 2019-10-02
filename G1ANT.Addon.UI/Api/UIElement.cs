using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using CodePlex.XPathParser;
using G1ANT.Language;
using G1ANT.Addon.UI.ExtensionMethods;
using AutomationElement = FlaUI.Core.AutomationElements.AutomationElement;
using ControlType = FlaUI.Core.Definitions.ControlType;
using InvokePattern = FlaUI.UIA3.Patterns.InvokePattern;
using SelectionItemPattern = FlaUI.UIA3.Patterns.SelectionItemPattern;
using ValuePattern = FlaUI.UIA3.Patterns.ValuePattern;
using FlaUI.Core.Identifiers;
using FlaUI.UIA3.Identifiers;

namespace G1ANT.Addon.UI
{
    public class UIElement
    {
        public static UIElement RootElement { get; set; } = null;

        protected AutomationElement automationElement;

        private UIElement()
        {
        }

        public UIElement(AutomationElement element)
        {
            automationElement = element ?? throw new NullReferenceException("Cannot create UIElement class from empty AutomationElement");
        }

        public static UIElement FromWPath(WPathStructure wpath)
        {
            object xe = new XPathParser<object>().Parse(wpath.Value, new XPathUIElementBuilder(RootElement?.automationElement));
            if (xe is AutomationElement element)
            {
                return new UIElement()
                {
                    automationElement = element
                };
            }
            throw new NullReferenceException($"Cannot find UI element described by \"{wpath.Value}\".");
        }

        public class NodeDescription
        {
            public string id;
            public string name;
            public string className;
            public ControlType type;
        }

        public WPathStructure ToWPath(UIElement root = null)
        {
            Stack<NodeDescription> elementStack = new Stack<NodeDescription>();
            AutomationElement elementParent;
            AutomationElement node = automationElement;
            AutomationElement automationRoot = root != null ? root.automationElement : AutomationSingleton.Automation.GetDesktop();
            var walker = automationRoot.Automation.TreeWalkerFactory.GetControlViewWalker();
            do
            {
                elementStack.Push(new NodeDescription()
                {
                    id = node.Properties.AutomationId.ValueOrDefault,
                    name = node.Properties.Name.ValueOrDefault,
                    className = node.Properties.ClassName.ValueOrDefault,
                    type = node.Properties.ControlType.ValueOrDefault
                });

                elementParent = walker.GetParent(node);

                if (elementParent == automationRoot || elementParent == null)
                {
                    break;
                }

                node = elementParent;
            }
            while (true);

            bool isParentEmpty = false;
            string wpath = "";
            foreach (var element in elementStack)
            {
                if (IsParentEmpty(element))
                {
                    isParentEmpty = true;
                }
                else
                {
                    string xpath = "";
                    if (isParentEmpty)
                    {
                        xpath += "descendant::";
                    }
                    if (string.IsNullOrEmpty(element.id) == false)
                    {
                        xpath += $"ui[@id='{element.id}']";
                    }
                    else if (string.IsNullOrEmpty(element.name) == false)
                    {
                        xpath += $"ui[@name='{element.name}']";
                    }
                    wpath += $"/{xpath}";
                    isParentEmpty = false;
                }
            }
            return new WPathStructure(wpath);
        }

        private bool IsParentEmpty(NodeDescription element)
        {
            return string.IsNullOrEmpty(element.id) && string.IsNullOrEmpty(element.name);
        }

        public void Click()
        {
            if (automationElement.TryGetClickablePoint(out var pt))
            {
                var tempPos = MouseWin32.GetPhysicalCursorPosition();
                var currentPos = new Point(tempPos.X, tempPos.Y);
                var targetPos = new Point((int)pt.X, (int)pt.Y);

                List<MouseStr.MouseEventArgs> mouseArgs =
                    MouseStr.ToMouseEventsArgs(
                        targetPos.X,
                        targetPos.Y,
                        currentPos.X,
                        currentPos.Y,
                        "left",
                        "press",
                        1);

                foreach (var arg in mouseArgs)
                {
                    MouseWin32.MouseEvent(arg.dwFlags, arg.dx, arg.dy, arg.dwData);
                    Thread.Sleep(10);
                }
            }
            else if (automationElement.FrameworkAutomationElement.TryGetNativePattern<object>(InvokePattern.Pattern, out var invokePattern))
            {
                (invokePattern as InvokePattern)?.Invoke();
            }
            else if (automationElement.FrameworkAutomationElement.TryGetNativePattern<object>(SelectionItemPattern.Pattern, out var selectionPattern))
            {
                (selectionPattern as SelectionItemPattern)?.Select();
            }
        }

        public void SetFocus()
        {
            automationElement.Focus();
        }

        public void SetText(string text, int timeout)
        {
            object valuePattern = null;
            if (automationElement.FrameworkAutomationElement.TryGetNativePattern(ValuePattern.Pattern, out valuePattern))
            {
                automationElement.Focus();;
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

        public System.Windows.Rect GetRectangle()
        {
            var rectanglePropertyId = new PropertyId(AutomationObjectIds.BoundingRectangleProperty.Id, AutomationObjectIds.BoundingRectangleProperty.Name);
            if (automationElement.FrameworkAutomationElement.TryGetPropertyValue(rectanglePropertyId, out var boundingRectNoDefault))
            {
                return (System.Windows.Rect)boundingRectNoDefault;

            }

            if (automationElement.FrameworkAutomationElement.NativeWindowHandle != IntPtr.Zero)
            {
                RobotWin32.Rect rect = new RobotWin32.Rect();
                IntPtr wndHandle = automationElement.FrameworkAutomationElement.NativeWindowHandle;
                if (RobotWin32.GetWindowRectangle(wndHandle, ref rect))
                {
                    return new System.Windows.Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
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
