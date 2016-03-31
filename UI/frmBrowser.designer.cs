namespace UI
{
    partial class frmBrowser
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBrowser));
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            this.toolMain = new System.Windows.Forms.ToolStrip();
            this.toolSelect = new System.Windows.Forms.ToolStripButton();
            this.toolRefrash = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolNew = new System.Windows.Forms.ToolStripButton();
            this.toolView = new System.Windows.Forms.ToolStripButton();
            this.toolEdit = new System.Windows.Forms.ToolStripButton();
            this.toolDelete = new System.Windows.Forms.ToolStripButton();
            this.toolUndelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolCheck = new System.Windows.Forms.ToolStripButton();
            this.toolUnCheck = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripHistory = new System.Windows.Forms.ToolStripButton();
            this.toolImport = new System.Windows.Forms.ToolStripButton();
            this.toolExport = new System.Windows.Forms.ToolStripButton();
            this.toolBestWidth = new System.Windows.Forms.ToolStripButton();
            this.toolBestHeight = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolClose = new System.Windows.Forms.ToolStripButton();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.panelGrid = new System.Windows.Forms.Panel();
            this.grid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.toolFinished = new System.Windows.Forms.ToolStripButton();
            this.toolMain.SuspendLayout();
            this.panelGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // 
            // toolMain
            // 
            this.toolMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolSelect,
            this.toolRefrash,
            this.toolStripSeparator2,
            this.toolNew,
            this.toolView,
            this.toolEdit,
            this.toolDelete,
            this.toolUndelete,
            this.toolStripSeparator4,
            this.toolCopy,
            this.toolStripButton1,
            this.toolCheck,
            this.toolUnCheck,
            this.toolFinished,
            this.toolStripSeparator1,
            this.toolStripHistory,
            this.toolImport,
            this.toolExport,
            this.toolBestWidth,
            this.toolBestHeight,
            this.toolStripSeparator3,
            this.toolClose});
            this.toolMain.Location = new System.Drawing.Point(0, 0);
            this.toolMain.Name = "toolMain";
            this.toolMain.Size = new System.Drawing.Size(841, 38);
            this.toolMain.TabIndex = 10;
            this.toolMain.Text = "toolStrip1";
            // 
            // toolSelect
            // 
            this.toolSelect.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolSelect.Image = ((System.Drawing.Image)(resources.GetObject("toolSelect.Image")));
            this.toolSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSelect.Name = "toolSelect";
            this.toolSelect.Size = new System.Drawing.Size(39, 35);
            this.toolSelect.Text = "选择";
            this.toolSelect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolSelect.Click += new System.EventHandler(this.toolSelect_Click);
            // 
            // toolRefrash
            // 
            this.toolRefrash.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolRefrash.Image = ((System.Drawing.Image)(resources.GetObject("toolRefrash.Image")));
            this.toolRefrash.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolRefrash.Name = "toolRefrash";
            this.toolRefrash.Size = new System.Drawing.Size(39, 35);
            this.toolRefrash.Text = "刷新";
            this.toolRefrash.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolRefrash.Click += new System.EventHandler(this.toolRefrash_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // toolNew
            // 
            this.toolNew.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolNew.Image = ((System.Drawing.Image)(resources.GetObject("toolNew.Image")));
            this.toolNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolNew.Name = "toolNew";
            this.toolNew.Size = new System.Drawing.Size(39, 35);
            this.toolNew.Text = "新增";
            this.toolNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolNew.Click += new System.EventHandler(this.toolNew_Click);
            // 
            // toolView
            // 
            this.toolView.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolView.Image = ((System.Drawing.Image)(resources.GetObject("toolView.Image")));
            this.toolView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolView.Name = "toolView";
            this.toolView.Size = new System.Drawing.Size(39, 35);
            this.toolView.Text = "查看";
            this.toolView.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolView.Click += new System.EventHandler(this.toolView_Click);
            // 
            // toolEdit
            // 
            this.toolEdit.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolEdit.Image = ((System.Drawing.Image)(resources.GetObject("toolEdit.Image")));
            this.toolEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolEdit.Name = "toolEdit";
            this.toolEdit.Size = new System.Drawing.Size(39, 35);
            this.toolEdit.Text = "修改";
            this.toolEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolEdit.Click += new System.EventHandler(this.toolEdit_Click);
            // 
            // toolDelete
            // 
            this.toolDelete.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolDelete.Image")));
            this.toolDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolDelete.Name = "toolDelete";
            this.toolDelete.Size = new System.Drawing.Size(39, 35);
            this.toolDelete.Text = "删除";
            this.toolDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolDelete.Click += new System.EventHandler(this.toolDelete_Click);
            // 
            // toolUndelete
            // 
            this.toolUndelete.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolUndelete.Image = ((System.Drawing.Image)(resources.GetObject("toolUndelete.Image")));
            this.toolUndelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolUndelete.Name = "toolUndelete";
            this.toolUndelete.Size = new System.Drawing.Size(39, 35);
            this.toolUndelete.Text = "恢复";
            this.toolUndelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolUndelete.Click += new System.EventHandler(this.toolUndelete_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 38);
            // 
            // toolCopy
            // 
            this.toolCopy.Image = ((System.Drawing.Image)(resources.GetObject("toolCopy.Image")));
            this.toolCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolCopy.Name = "toolCopy";
            this.toolCopy.Size = new System.Drawing.Size(35, 35);
            this.toolCopy.Text = "复制";
            this.toolCopy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolCopy.Click += new System.EventHandler(this.toolCopy_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(6, 38);
            // 
            // toolCheck
            // 
            this.toolCheck.Image = ((System.Drawing.Image)(resources.GetObject("toolCheck.Image")));
            this.toolCheck.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolCheck.Name = "toolCheck";
            this.toolCheck.Size = new System.Drawing.Size(35, 35);
            this.toolCheck.Text = "复核";
            this.toolCheck.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolCheck.Click += new System.EventHandler(this.toolCheck_Click);
            // 
            // toolUnCheck
            // 
            this.toolUnCheck.Image = ((System.Drawing.Image)(resources.GetObject("toolUnCheck.Image")));
            this.toolUnCheck.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolUnCheck.Name = "toolUnCheck";
            this.toolUnCheck.Size = new System.Drawing.Size(47, 35);
            this.toolUnCheck.Text = "反复核";
            this.toolUnCheck.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolUnCheck.Click += new System.EventHandler(this.toolUncheck_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripHistory
            // 
            this.toolStripHistory.CheckOnClick = true;
            this.toolStripHistory.Image = ((System.Drawing.Image)(resources.GetObject("toolStripHistory.Image")));
            this.toolStripHistory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripHistory.Name = "toolStripHistory";
            this.toolStripHistory.Size = new System.Drawing.Size(35, 35);
            this.toolStripHistory.Text = "历史";
            this.toolStripHistory.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripHistory.CheckedChanged += new System.EventHandler(this.toolStripHistory_CheckedChanged);
            // 
            // toolImport
            // 
            this.toolImport.Image = global::UI.Properties.Resources.Import;
            this.toolImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolImport.Name = "toolImport";
            this.toolImport.Size = new System.Drawing.Size(35, 35);
            this.toolImport.Text = "导入";
            this.toolImport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolImport.Click += new System.EventHandler(this.toolImport_Click);
            // 
            // toolExport
            // 
            this.toolExport.Image = ((System.Drawing.Image)(resources.GetObject("toolExport.Image")));
            this.toolExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolExport.Name = "toolExport";
            this.toolExport.Size = new System.Drawing.Size(35, 35);
            this.toolExport.Text = "导出";
            this.toolExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolExport.Click += new System.EventHandler(this.toolExport_Click);
            // 
            // toolBestWidth
            // 
            this.toolBestWidth.Image = global::UI.Properties.Resources.BestWidth;
            this.toolBestWidth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBestWidth.Name = "toolBestWidth";
            this.toolBestWidth.Size = new System.Drawing.Size(59, 35);
            this.toolBestWidth.Text = "最佳列宽";
            this.toolBestWidth.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolBestWidth.Click += new System.EventHandler(this.toolBestWidth_Click);
            // 
            // toolBestHeight
            // 
            this.toolBestHeight.Image = global::UI.Properties.Resources.BestHeight;
            this.toolBestHeight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBestHeight.Name = "toolBestHeight";
            this.toolBestHeight.Size = new System.Drawing.Size(59, 35);
            this.toolBestHeight.Text = "最佳行高";
            this.toolBestHeight.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolBestHeight.Click += new System.EventHandler(this.toolBestHeight_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 38);
            // 
            // toolClose
            // 
            this.toolClose.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolClose.Image = ((System.Drawing.Image)(resources.GetObject("toolClose.Image")));
            this.toolClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolClose.Name = "toolClose";
            this.toolClose.Size = new System.Drawing.Size(39, 35);
            this.toolClose.Text = "关闭";
            this.toolClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolClose.Click += new System.EventHandler(this.toolClose_Click);
            // 
            // panelFilter
            // 
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 38);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(841, 51);
            this.panelFilter.TabIndex = 11;
            // 
            // panelGrid
            // 
            this.panelGrid.Controls.Add(this.grid);
            this.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrid.Location = new System.Drawing.Point(0, 89);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Size = new System.Drawing.Size(841, 302);
            this.panelGrid.TabIndex = 12;
            // 
            // grid
            // 
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grid.DisplayLayout.Appearance = appearance13;
            this.grid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance14.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance14.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance14.BorderColor = System.Drawing.SystemColors.Window;
            this.grid.DisplayLayout.GroupByBox.Appearance = appearance14;
            appearance15.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance15;
            this.grid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance16.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance16.BackColor2 = System.Drawing.SystemColors.Control;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grid.DisplayLayout.GroupByBox.PromptAppearance = appearance16;
            this.grid.DisplayLayout.MaxColScrollRegions = 1;
            this.grid.DisplayLayout.MaxRowScrollRegions = 1;
            appearance17.BackColor = System.Drawing.SystemColors.Window;
            appearance17.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grid.DisplayLayout.Override.ActiveCellAppearance = appearance17;
            appearance18.BackColor = System.Drawing.SystemColors.Highlight;
            appearance18.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grid.DisplayLayout.Override.ActiveRowAppearance = appearance18;
            this.grid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.grid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance19.BackColor = System.Drawing.SystemColors.Window;
            this.grid.DisplayLayout.Override.CardAreaAppearance = appearance19;
            appearance20.BorderColor = System.Drawing.Color.Silver;
            appearance20.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grid.DisplayLayout.Override.CellAppearance = appearance20;
            this.grid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
            this.grid.DisplayLayout.Override.CellPadding = 0;
            appearance21.BackColor = System.Drawing.SystemColors.Control;
            appearance21.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance21.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance21.BorderColor = System.Drawing.SystemColors.Window;
            this.grid.DisplayLayout.Override.GroupByRowAppearance = appearance21;
            appearance22.TextHAlignAsString = "Left";
            this.grid.DisplayLayout.Override.HeaderAppearance = appearance22;
            this.grid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance23.BackColor = System.Drawing.SystemColors.Window;
            appearance23.BorderColor = System.Drawing.Color.Silver;
            this.grid.DisplayLayout.Override.RowAppearance = appearance23;
            appearance24.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grid.DisplayLayout.Override.TemplateAddRowAppearance = appearance24;
            this.grid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(0, 0);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(841, 302);
            this.grid.TabIndex = 0;
            this.grid.AfterRowActivate += new System.EventHandler(this.grid_AfterRowActivate);
            this.grid.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.grid_BeforeRowsDeleted);
            this.grid.BeforeRowRegionScroll += new Infragistics.Win.UltraWinGrid.BeforeRowRegionScrollEventHandler(this.grid_BeforeRowRegionScroll);
            this.grid.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.grid_DoubleClickRow);
            // 
            // toolFinished
            // 
            this.toolFinished.Image = global::UI.Properties.Resources.Finish;
            this.toolFinished.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolFinished.Name = "toolFinished";
            this.toolFinished.Size = new System.Drawing.Size(35, 35);
            this.toolFinished.Text = "完成";
            this.toolFinished.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolFinished.Click += new System.EventHandler(this.toolFinished_Click);
            // 
            // frmBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 391);
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelFilter);
            this.Controls.Add(this.toolMain);
            this.Name = "frmBrowser";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmBrowser";
            this.Load += new System.EventHandler(this.frmBrowser_Load);
            this.Click += new System.EventHandler(this.frmBrowser_Load);
            this.toolMain.ResumeLayout(false);
            this.toolMain.PerformLayout();
            this.panelGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripButton toolSelect;
        private System.Windows.Forms.ToolStripButton toolRefrash;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolNew;
        private System.Windows.Forms.ToolStripButton toolView;
        public  System.Windows.Forms.ToolStripButton toolEdit;
        private System.Windows.Forms.ToolStripButton toolDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripButton1;
        public  System.Windows.Forms.ToolStripButton toolCheck;
        public  System.Windows.Forms.ToolStripButton toolUnCheck;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolClose;
        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.Panel panelGrid;
        private System.Windows.Forms.ToolStripButton toolExport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolCopy;
        private System.Windows.Forms.ToolStripButton toolUndelete;
        public Infragistics.Win.UltraWinGrid.UltraGrid grid;
        private System.Windows.Forms.ToolStripButton toolStripHistory;
        public System.Windows.Forms.ToolStrip toolMain;
        private System.Windows.Forms.ToolStripButton toolBestWidth;
        private System.Windows.Forms.ToolStripButton toolBestHeight;
        private System.Windows.Forms.ToolStripButton toolImport;
        private System.Windows.Forms.ToolStripButton toolFinished;
    }
}