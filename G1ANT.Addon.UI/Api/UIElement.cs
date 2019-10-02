﻿using System;
using System.Windows.Automation;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using CodePlex.XPathParser;
using G1ANT.Language;
using G1ANT.Addon.UI.ExtensionMethods;

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
            TreeWalker walker = TreeWalker.ControlViewWalker;
            AutomationElement elementParent;
            AutomationElement node = automationElement;
            AutomationElement automationRoot = root != null ? root.automationElement : AutomationElement.RootElement;

            do
            {
                elementStack.Push(new NodeDescription()
                {
                    id = node.Current.AutomationId,
                    name = node.Current.Name,
                    className = node.Current.ClassName,
                    type = node.Current.ControlType
                });

                elementParent = walker.GetParent(node);

                if (elementParent == automationRoot)
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
            if (automationElement.TryGetCurrentPattern(InvokePattern.Pattern, out var invokePattern))
            {
                (invokePattern as InvokePattern)?.Invoke();
            }
            else if (automationElement.TryGetCurrentPattern(SelectionItemPattern.Pattern, out var selectionPattern))
            {
                (selectionPattern as SelectionItemPattern)?.Select();
            }
            else if (automationElement.TryGetClickablePoint(out var pt))
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

            throw new Exception($"Could not click element: {automationElement.Current.Name}");
        }

        public void SetFocus()
        {
            automationElement.SetFocus();
        }

        public void SetText(string text, int timeout)
        {
            object valuePattern = null;
            if (automationElement.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
            {
                automationElement.SetFocus();
                ((ValuePattern)valuePattern).SetValue(text);
            }
            else if (automationElement.Current.NativeWindowHandle != 0)
            {
                automationElement.SetFocus();
                var wndHandle = new IntPtr(automationElement.Current.NativeWindowHandle);
                KeyboardTyper.TypeWithSendInput($"{SpecialChars.KeyBegin}ctrl+home{SpecialChars.KeyEnd}", null, wndHandle, IntPtr.Zero, timeout, false, 0); // Move to start of control
                KeyboardTyper.TypeWithSendInput($"{SpecialChars.KeyBegin}ctrl+shift+end{SpecialChars.KeyEnd}", null, wndHandle, IntPtr.Zero, timeout, false, 0); // Select everything
                KeyboardTyper.TypeWithSendInput(text, null, wndHandle, IntPtr.Zero, timeout, false, 0);
            }
            else
                throw new NotSupportedException("SetText is not supported");
        }

        public System.Windows.Rect GetRectangle()
        {
            var boundingRectNoDefault = automationElement.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty, true);
            if (boundingRectNoDefault != AutomationElement.NotSupported)
            {
                return (System.Windows.Rect)boundingRectNoDefault;
            }
            else if (automationElement.Current.NativeWindowHandle != 0)
            {
                RobotWin32.Rect rect = new RobotWin32.Rect();
                var wndHandle = new IntPtr(automationElement.Current.NativeWindowHandle);
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
