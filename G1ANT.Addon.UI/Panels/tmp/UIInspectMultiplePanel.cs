using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using G1ANT.Language;
using G1ANT.Addon.UI.Api;
using FlaUI.Core.AutomationElements;
using static G1ANT.Addon.UI.Api.InspectUIElement;
using ListBox = System.Windows.Forms.ListBox;

namespace G1ANT.Addon.UI.Panels
{
    [Panel(Name = "Inspect multiple controls", DockingSide = DockingSide.Right, InitialAppear = false, Width = 400)]
    public partial class UIInspectMultiplePanel : RobotPanel
    {
        private InspectUIElement inspectUIElement;
        private List<InspectSelectedElement> collectedItems = new List<InspectSelectedElement>();

        public UIInspectMultiplePanel()
        {
            InitializeComponent();

            inspectUIElement = new InspectUIElement();
            inspectUIElement.OnFinished += Inspect_Finished;
            inspectUIElement.OnElementClicked += Inspect_ElementSelected;
        }

        public override void RefreshContent()
        {
        }

        private void Inspect_Finished()
        {
            if (MainForm is Form form)
            {
                form.Show();
                form.Activate();
            }

            listUIElements.Items.AddRange(collectedItems.ToArray());
            collectedItems.Clear();
        }

        private void Inspect_ElementSelected(InspectSelectedElement element)
        {
            collectedItems.Add(element);
        }

        private void listUIElements_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= listUIElements.Items.Count || e.Index <= -1)
                return;

            var item = listUIElements.Items[e.Index] as InspectSelectedElement;
            if (item == null)
                return;

            e.DrawBackground();
            e.DrawFocusRectangle();

            var rect = e.Bounds;
            rect.Inflate(-2, -2);

            var dy = (rect.Height - item.Image.Height) / 2;
            if (dy > 0)
            {
                rect.Y += dy;
                e.Graphics.DrawImageUnscaled(item.Image, rect);
            }
            else
                e.Graphics.DrawImageUnscaledAndClipped(item.Image, rect);
        }

        private void insertWPathButton_Click(object sender, EventArgs e)
        {
            try
            {
                var selItem = listUIElements.SelectedItem as InspectSelectedElement;
                if (selItem != null)
                {
                    MainForm.InsertTextIntoCurrentEditor($"{SpecialChars.Text}{selItem.WPath}{SpecialChars.Text}");
                }
            }
            catch
            { }
        }

        private void inspectMultipleButton_Click(object sender, EventArgs e)
        {
            if (MainForm is Form form)
                form.Hide();
            collectedItems.Clear();
            inspectUIElement.Start();
        }

        private void clearAllButton_Click(object sender, EventArgs e)
        {
            listUIElements.Items.Clear();
        }

        private void listUIElements_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            insertWPathButton_Click(sender, e);
        }

        private void listUIElements_MouseMove(object sender, MouseEventArgs e)
        {
            int index = listUIElements.IndexFromPoint(e.Location);
            if (index == ListBox.NoMatches)
            {
                listToolTip.SetToolTip(listUIElements, "");
                return;
            }

            var item = listUIElements.Items[index] as InspectSelectedElement;
            if (item == null)
                return;

            string tip = $"{item.Name}\n{item.WPath}";

            if (listToolTip.GetToolTip(listUIElements) != tip)
                listToolTip.SetToolTip(listUIElements, tip);
        }
    }
}
