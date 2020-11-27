namespace G1ANT.Addon.UI.Panels
{
    partial class UIInspectMultiplePanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.insertWPathButton = new System.Windows.Forms.ToolStripButton();
            this.inspectMultipleButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearAllButton = new System.Windows.Forms.ToolStripButton();
            this.listUIElements = new System.Windows.Forms.ListBox();
            this.listToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertWPathButton,
            this.inspectMultipleButton,
            this.toolStripSeparator1,
            this.clearAllButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(286, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // insertWPathButton
            // 
            this.insertWPathButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.insertWPathButton.Image = global::G1ANT.Addon.UI.Resources.insert_into;
            this.insertWPathButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.insertWPathButton.Name = "insertWPathButton";
            this.insertWPathButton.Size = new System.Drawing.Size(23, 22);
            this.insertWPathButton.Text = "Insert WPath";
            this.insertWPathButton.ToolTipText = "Insert WPath of selected control";
            this.insertWPathButton.Click += new System.EventHandler(this.insertWPathButton_Click);
            // 
            // inspectMultipleButton
            // 
            this.inspectMultipleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.inspectMultipleButton.Image = global::G1ANT.Addon.UI.Resources.inspect2;
            this.inspectMultipleButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.inspectMultipleButton.Name = "inspectMultipleButton";
            this.inspectMultipleButton.Size = new System.Drawing.Size(23, 22);
            this.inspectMultipleButton.Text = "Inspect multiple UI element";
            this.inspectMultipleButton.Click += new System.EventHandler(this.inspectMultipleButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // clearAllButton
            // 
            this.clearAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearAllButton.Image = global::G1ANT.Addon.UI.Resources.trash;
            this.clearAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearAllButton.Name = "clearAllButton";
            this.clearAllButton.Size = new System.Drawing.Size(23, 22);
            this.clearAllButton.Text = "Clear collected elements";
            this.clearAllButton.Click += new System.EventHandler(this.clearAllButton_Click);
            // 
            // listUIElements
            // 
            this.listUIElements.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listUIElements.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listUIElements.IntegralHeight = false;
            this.listUIElements.ItemHeight = 50;
            this.listUIElements.Location = new System.Drawing.Point(0, 28);
            this.listUIElements.Name = "listUIElements";
            this.listUIElements.Size = new System.Drawing.Size(286, 253);
            this.listUIElements.TabIndex = 2;
            this.listUIElements.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listUIElements_DrawItem);
            this.listUIElements.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listUIElements_MouseDoubleClick);
            this.listUIElements.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listUIElements_MouseMove);
            // 
            // UIInspectMultiplePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listUIElements);
            this.Controls.Add(this.toolStrip);
            this.Name = "UIInspectMultiplePanel";
            this.Size = new System.Drawing.Size(286, 284);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton insertWPathButton;
        private System.Windows.Forms.ToolStripButton inspectMultipleButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton clearAllButton;
        private System.Windows.Forms.ListBox listUIElements;
        private System.Windows.Forms.ToolTip listToolTip;
    }
}
