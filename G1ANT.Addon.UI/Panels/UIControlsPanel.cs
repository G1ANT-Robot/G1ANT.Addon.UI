﻿using System;
using System.Windows.Forms;
using G1ANT.Language;
using System.Text;
using System.Drawing;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using G1ANT.Addon.UI.Api;
using System.Linq;
using G1ANT.Addon.UI.Structures;
using G1ANT.Addon.UI.ExtensionMethods;
using static G1ANT.Addon.UI.Api.InspectUIElement;
using System.Collections.Generic;

namespace G1ANT.Addon.UI.Panels
{
    [Panel(Name = "Windows Tree", DockingSide = DockingSide.Right, InitialAppear = false, Width = 400)]
    public partial class UIControlsPanel : RobotPanel
    {
        private Form blinkingRectForm;
        private InspectUIElement inspectUIElement;
        private WPathBuilder wpathBuilder = new WPathBuilder();

        public UIControlsPanel()
        {
            InitializeComponent();
            ControlType.Button.GetType();
        }

        public override void Initialize(IMainForm mainForm)
        {
            base.Initialize(mainForm);
            InitRootElement();

            inspectUIElement = new InspectUIElement();
            inspectUIElement.OnFinished += Inspect_Finished;
            inspectUIElement.OnElementClicked += Inspect_ElementSelected;
        }

        public override void RefreshContent()
        {
        }

        private void InitRootElement()
        {
            controlsTree.Nodes.Clear();

            var root = AutomationSingleton.Automation.GetDesktop();
            var rootNode = controlsTree.Nodes.Add(root.FrameworkAutomationElement.Name);
            rootNode.Tag = new UIElement(root, 0);
            rootNode.Nodes.Add("");
            rootNode.Expand();
        }

        private void controlsTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {

        }

        private string GetControlTypeString(AutomationElement element)
        {
            try
            {
                var controlType = element.ControlType.ToString();
                return string.IsNullOrEmpty(controlType) ? "" : controlType.Replace("ControlType.", "");
            }
            catch
            {
                return "";
            }
        }

        private string GetTreeNodeName(AutomationElement element)
        {
            if (element == null)
                return "";
            string id = "";
            if (!string.IsNullOrWhiteSpace(element.Properties.AutomationId.ValueOrDefault))
                id = $" #{element.Properties.AutomationId.ValueOrDefault}";
            return $"{GetControlTypeString(element)}{id} \"{element.Properties.Name.ValueOrDefault}\"";
        }

        private string GetTreeNodeTooltip(UIElement element)
        {
            if (element == null)
                return null;
            var result = new StringBuilder();

            if (element.IsPropertySupported(UIElement.Indexes.Id))
                result.AppendLine($"id: {element.GetPropertyValue(UIElement.Indexes.Id)}");
            if (element.IsPropertySupported(UIElement.Indexes.Type))
                result.AppendLine($"type: {element.GetPropertyValue(UIElement.Indexes.Type)}");
            if (element.IsPropertySupported(UIElement.Indexes.TypeId))
                result.AppendLine($"typeid: {element.GetPropertyValue(UIElement.Indexes.TypeId)}");
            if (element.IsPropertySupported(UIElement.Indexes.Class))
                result.AppendLine($"class: {element.GetPropertyValue(UIElement.Indexes.Class)}");
            if (element.IsPropertySupported(UIElement.Indexes.Name))
                result.AppendLine($"name: {element.GetPropertyValue(UIElement.Indexes.Name)}");
            result.AppendLine($"control index: {element.Index}");
            return result.ToString();
        }

        private void controlsTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            try
            {
                if (e.Node.Tag is UIElement element)
                {
                    var index = 0;
                    foreach (var elem in element.AutomationElement.FindAllChildren())
                    {
                        try
                        {
                            var uiElement = elem.ToUIElement(index++);
                            var node = e.Node.Nodes.Add(GetTreeNodeName(elem));
                            node.Tag = uiElement;
                            node.ToolTipText = GetTreeNodeTooltip(uiElement);
                            node.Nodes.Add("");
                        }
                        catch (Exception ex)
                        {
                            scripter?.Logger?.Warn($"Cannot display Window Tree item", ex);
                        }
                    }
                }
            }
            catch
            { }
        }

        private void InsertWPathIntoScript()
        {
            try
            {
                if (controlsTree.SelectedNode != null)
                {
                    if (controlsTree.SelectedNode.Tag is UIElement uiElement)
                    {
                        var wpath = uiElement.ToWPath(rebuild: true, options: WPathPropertiesSelectorButton.Options);
                        MainForm.InsertTextIntoCurrentEditor($"{SpecialChars.Text}{wpath}{SpecialChars.Text}");
                    }
                }
            }
            catch
            { }
        }

        private void controlsTree_DoubleClick(object sender, EventArgs e)
        {
            InsertWPathIntoScript();
        }

        private void insertWPathButton_Click(object sender, EventArgs e)
        {
            InsertWPathIntoScript();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            var selectedElement = controlsTree.SelectedNode?.Tag as UIElement;
            InitRootElement();
            SelectUIElement(selectedElement);
        }

        #region RectangleForm

        private AutomationElement GetTopLevelWindow(AutomationElement element)
        {
            var desktop = AutomationSingleton.Automation.GetDesktop();
            if (element.Equals(desktop))
            {
                return element;
            }

            var elementParent = element.GetParentControl();
            return elementParent.Equals(desktop) ? element : GetTopLevelWindow(elementParent);
        }

        private void highlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (controlsTree.SelectedNode != null)
                {
                    if (controlsTree.SelectedNode.Tag is UIElement uiElement)
                    {
                        var element = UIElement.FromWPath(uiElement.ToWPath(rebuild: true));
                        if (element != null)
                        {
                            var window = GetTopLevelWindow(uiElement.AutomationElement);
                            if (window != null)
                            {
                                var iHandle = window.FrameworkAutomationElement.NativeWindowHandle;
                                if (iHandle != IntPtr.Zero)
                                {
                                    RobotWin32.BringWindowToFront(iHandle);
                                    var rect = element.GetRectangle();
                                    if (rect != null)
                                    {
                                        InitializeRectangleForm(rect);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RobotMessageBox.Show(ex.Message, "Error");
            }
        }

        private Timer blinkTimer;
        private int blinkTimes;
        private void InitializeRectangleForm(Rectangle rect)
        {
            blinkingRectForm = new Form();
            Panel transparentPanel = new Panel();
            transparentPanel.BackColor = Color.Pink;
            transparentPanel.Location = new Point(3, 3);
            transparentPanel.Padding = new System.Windows.Forms.Padding(30);
            transparentPanel.Parent = blinkingRectForm;
            blinkingRectForm.Controls.Add(transparentPanel);
            blinkingRectForm.ShowInTaskbar = false;
            blinkingRectForm.TransparencyKey = Color.Pink;
            blinkingRectForm.BackColor = Color.Red;
            blinkingRectForm.ForeColor = Color.Red;
            blinkingRectForm.TopMost = true;
            blinkingRectForm.FormBorderStyle = FormBorderStyle.None;
            blinkingRectForm.ControlBox = false;
            blinkingRectForm.Text = string.Empty;
            blinkingRectForm.StartPosition = FormStartPosition.Manual;
            blinkingRectForm.MinimumSize = new Size(10, 10);
            blinkingRectForm.Location = new Point((int)rect.Left, (int)rect.Top);
            blinkingRectForm.Size = new Size((int)(rect.Right - rect.Left), (int)(rect.Bottom - rect.Top));
            transparentPanel.Size = new Size(blinkingRectForm.Size.Width - 6, blinkingRectForm.Size.Height - 6);
            blinkingRectForm.Shown += RectangleForm_Shown;
            blinkingRectForm.Show();
        }

        private void RectangleForm_Shown(object sender, EventArgs e)
        {
            blinkTimer = new Timer();
            blinkTimer.Interval = 300;
            blinkTimes = 10;
            blinkTimer.Tick -= BlinkTimer_Tick;
            blinkTimer.Tick += BlinkTimer_Tick;
            blinkTimer.Enabled = true;
        }

        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            blinkingRectForm.Visible = !blinkingRectForm.Visible;
            if (blinkTimes-- == 0)
            {
                blinkTimer.Enabled = false;
                blinkingRectForm.Close();
            }
        }

        #endregion

        private void controlsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            controlsTree.SelectedNode = e.Node;
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip.Show(Control.MousePosition);
            }
        }

        private IList<string> GetComponentIndexes(UIComponentStructure component)
        {
            return component.Indexes.Where(x => x != UIElement.Indexes.Children).ToList();
        }

        private void controlsTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var selectedNode = controlsTree.SelectedNode;
            var selectedModel = selectedNode?.Tag;

            propertiesGrid.Rows.Clear();

            if (selectedModel is UIElement uiElement)
            {
                var uiComponent = new UIComponentStructure(uiElement, "", scripter);

                var properties = GetComponentIndexes(uiComponent);
                if (properties?.Any() == true)
                {
                    propertiesGrid.Rows.AddRange(
                        properties
                            .Select(p => new System.Windows.Forms.DataGridViewRow()
                            {
                                Cells = {
                                    new DataGridViewTextBoxCell() { Value = p },
                                    new DataGridViewTextBoxCell() { 
                                        Value = uiComponent.Get(p).ToString(), 
                                        ToolTipText = uiComponent.Get(p).ToString() 
                                    },
                                },
                            })
                            .ToArray()
                    );
                }
            }
        }

        private void SelectUIElement(UIElement element)
        {
            SelectUIElement(element?.AutomationElement);
        }

        private void SelectUIElement(AutomationElement element)
        {
            if (element == null)
                return;

            var elements = wpathBuilder.BuildUIElementsStack(element, AutomationSingleton.Automation.GetDesktop());
            TreeNodeCollection nodes = controlsTree.Nodes;
            TreeNode foundNode = null;

            foreach (var el in elements)
            {
                foundNode = nodes.OfType<TreeNode>().FirstOrDefault(node => node.Tag.Equals(el));
                if (foundNode == null)
                {
                    RobotMessageBox.Show("Cannot find element");
                    return;
                }
                foundNode.Expand();
                nodes = foundNode.Nodes;
            }
            controlsTree.SelectedNode = foundNode;
        }

        private void inspectSingleButton_Click(object sender, EventArgs e)
        {
            if (MainForm is Form form)
                form.Hide();
            inspectUIElement.Start();
        }

        private void Inspect_Finished()
        {
            if (MainForm is Form form)
            {
                form.Show();
                form.Activate();
            }
        }

        private void Inspect_ElementSelected(InspectSelectedElement element)
        {
            inspectUIElement.Stop();
            InitRootElement();
            SelectUIElement(element.AutomationElement);
        }
    }
}
