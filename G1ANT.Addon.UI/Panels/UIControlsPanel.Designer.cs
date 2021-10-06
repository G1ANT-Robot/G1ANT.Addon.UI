namespace G1ANT.Addon.UI.Panels
{
    partial class UIControlsPanel
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
            this.inspectSingleButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.wpathPropertiesSelector = new G1ANT.Addon.UI.Api.WPathPropertiesSelectorButton();
            this.controlsTree = new System.Windows.Forms.TreeView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.highlightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesGrid = new System.Windows.Forms.DataGridView();
            this.Property = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolStrip.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.propertiesGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertWPathButton,
            this.inspectSingleButton,
            this.toolStripSeparator1,
            this.refreshButton,
            this.wpathPropertiesSelector});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(222, 25);
            this.toolStrip.TabIndex = 0;
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
            // inspectSingleButton
            // 
            this.inspectSingleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.inspectSingleButton.Image = global::G1ANT.Addon.UI.Resources.inspect1;
            this.inspectSingleButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.inspectSingleButton.Name = "inspectSingleButton";
            this.inspectSingleButton.Size = new System.Drawing.Size(23, 22);
            this.inspectSingleButton.Text = "Inspect single UI element";
            this.inspectSingleButton.Click += new System.EventHandler(this.inspectSingleButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // refreshButton
            // 
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.Image = global::G1ANT.Addon.UI.Resources.refresh;
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(23, 22);
            this.refreshButton.Text = "Refresh controls";
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // wpathPropertiesSelector
            // 
            this.wpathPropertiesSelector.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.wpathPropertiesSelector.Image = global::G1ANT.Addon.UI.Resources.index_properties;
            this.wpathPropertiesSelector.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.wpathPropertiesSelector.Name = "wpathPropertiesSelector";
            this.wpathPropertiesSelector.Size = new System.Drawing.Size(29, 22);
            this.wpathPropertiesSelector.Text = "wPathPropertiesSelectorButton1";
            // 
            // controlsTree
            // 
            this.controlsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlsTree.HideSelection = false;
            this.controlsTree.Location = new System.Drawing.Point(0, 0);
            this.controlsTree.Name = "controlsTree";
            this.controlsTree.ShowNodeToolTips = true;
            this.controlsTree.Size = new System.Drawing.Size(222, 300);
            this.controlsTree.TabIndex = 1;
            this.controlsTree.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.controlsTree_AfterCollapse);
            this.controlsTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.controlsTree_BeforeExpand);
            this.controlsTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.controlsTree_AfterSelect);
            this.controlsTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.controlsTree_NodeMouseClick);
            this.controlsTree.DoubleClick += new System.EventHandler(this.controlsTree_DoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.highlightToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(125, 26);
            // 
            // highlightToolStripMenuItem
            // 
            this.highlightToolStripMenuItem.Name = "highlightToolStripMenuItem";
            this.highlightToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.highlightToolStripMenuItem.Text = "Highlight";
            this.highlightToolStripMenuItem.Click += new System.EventHandler(this.highlightToolStripMenuItem_Click);
            // 
            // propertiesGrid
            // 
            this.propertiesGrid.AllowUserToAddRows = false;
            this.propertiesGrid.AllowUserToDeleteRows = false;
            this.propertiesGrid.AllowUserToResizeRows = false;
            this.propertiesGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.propertiesGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.propertiesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.propertiesGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Property,
            this.Value});
            this.propertiesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesGrid.Location = new System.Drawing.Point(0, 0);
            this.propertiesGrid.MultiSelect = false;
            this.propertiesGrid.Name = "propertiesGrid";
            this.propertiesGrid.RowHeadersVisible = false;
            this.propertiesGrid.RowTemplate.ReadOnly = true;
            this.propertiesGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.propertiesGrid.ShowCellErrors = false;
            this.propertiesGrid.ShowEditingIcon = false;
            this.propertiesGrid.ShowRowErrors = false;
            this.propertiesGrid.Size = new System.Drawing.Size(222, 122);
            this.propertiesGrid.TabIndex = 2;
            // 
            // Property
            // 
            this.Property.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Property.HeaderText = "Index";
            this.Property.MinimumWidth = 21;
            this.Property.Name = "Property";
            this.Property.ReadOnly = true;
            this.Property.Width = 71;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Value.HeaderText = "Value";
            this.Value.MinimumWidth = 100;
            this.Value.Name = "Value";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.controlsTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertiesGrid);
            this.splitContainer1.Size = new System.Drawing.Size(222, 426);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.TabIndex = 3;
            // 
            // UIControlsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip);
            this.Name = "UIControlsPanel";
            this.Size = new System.Drawing.Size(222, 454);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.propertiesGrid)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.TreeView controlsTree;
        private System.Windows.Forms.ToolStripButton insertWPathButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem highlightToolStripMenuItem;
        internal System.Windows.Forms.DataGridView propertiesGrid;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Property;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.ToolStripButton inspectSingleButton;
        private Api.WPathPropertiesSelectorButton wpathPropertiesSelector;
    }
}
