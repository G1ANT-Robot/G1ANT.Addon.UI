﻿using System;
using System.Linq;
using System.Drawing;
using System.Threading;
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
        private WPathBuilder wPathBuilder = new WPathBuilder();
        private WPathStructure cachedWPath;

        public static UIElement RootElement { get; set; }

        private AutomationElement automationElement = null;
        public AutomationElement AutomationElement 
        { 
            get
            {
                if (automationElement == null)
                {
                    if (cachedWPath == null)
                        throw new NullReferenceException("AutomationElement has not been initialized");
                    var element = FromWPath(cachedWPath?.Value);
                    automationElement = element.AutomationElement;
                    Index = element.Index;
                }
                return automationElement;
            }
            private set
            {
                automationElement = value;
            }
        }

        private int _index = -1;
        public int Index 
        { 
            get
            {
                if (_index == -1)
                    _index = FindElementIndex();
                return _index;
            }
            private set => _index = value; 
        }

        private UIElement() { }

        public UIElement(string wPath)
        {
            cachedWPath = new WPathStructure(wPath);
        }

        public UIElement(AutomationElement element, int index = -1)
        {
            AutomationElement = element ?? throw new NullReferenceException("Cannot create UIElement class from empty AutomationElement");
            Index = index;
        }

        public static UIElement FromWPath(WPathStructure wPath)
        {
            return FromWPath(wPath.Value);
        }

        private int FindElementIndex()
        {
            var parent = AutomationElement?.GetParentControl();
            if (parent == null)
                return -1;

            return parent.FindAllChildren().ToList().FindIndex(x => x.Equals(AutomationElement));
        }

        public override bool Equals(Object obj)
        {
            return obj is UIElement elem && elem.AutomationElement.Equals(AutomationElement);
        }

        public static UIElement FromWPath(string wPath)
        {
            var element = new XPathParser<object>().Parse(wPath, new XPathUIElementBuilder(RootElement?.AutomationElement));
            if (element is UIElement uiElement)
            {
                return uiElement;
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

        public WPathStructure ToWPath(AutomationElement rootElement = null, bool rebuild = false, WPathBuilderOptions options = null)
        {
            if (cachedWPath == null || rebuild)
            {
                try
                {
                    var automationRoot = rootElement ?? AutomationSingleton.Automation.GetDesktop();
                    cachedWPath = wPathBuilder.GetWPathStructure(AutomationElement, automationRoot, options);
                }
                catch
                {
                    return new WPathStructure("");
                }
            }
            return cachedWPath;
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
                        Mouse.Click(relative, MouseButton.Left);
                        break;
                    case EventTypes.MouseRightClick:
                        Mouse.RightClick(relative);
                        break;
                    case EventTypes.MouseDoubleClick:
                        Mouse.DoubleClick(relative, MouseButton.Left);
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
