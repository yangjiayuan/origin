namespace UI
{
    partial class frmColumnConfig
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LineNumber", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FieldName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OldDescription");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NewDescription");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Width");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Hidden");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OrderBy");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            this.panelCommand = new System.Windows.Forms.Panel();
            this.butConfigDown = new System.Windows.Forms.Button();
            this.butConfigUp = new System.Windows.Forms.Button();
            this.butGetWidth = new System.Windows.Forms.Button();
            this.butSaveAs = new System.Windows.Forms.Button();
            this.butDefault = new System.Windows.Forms.Button();
            this.butSave = new System.Windows.Forms.Button();
            this.butDelete = new System.Windows.Forms.Button();
            this.butChoice = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.splitTop = new System.Windows.Forms.SplitContainer();
            this.listConfig = new System.Windows.Forms.ListView();
            this.panelConfigTop = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panelConfigBottom = new System.Windows.Forms.Panel();
            this.gridColumn = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.panel1 = new System.Windows.Forms.Panel();
            this.butDown = new System.Windows.Forms.Button();
            this.butUp = new System.Windows.Forms.Button();
            this.panelCommand.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.splitTop.Panel1.SuspendLayout();
            this.splitTop.Panel2.SuspendLayout();
            this.splitTop.SuspendLayout();
            this.panelConfigBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridColumn)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelCommand
            // 
            this.panelCommand.Controls.Add(this.butConfigDown);
            this.panelCommand.Controls.Add(this.butConfigUp);
            this.panelCommand.Controls.Add(this.butGetWidth);
            this.panelCommand.Controls.Add(this.butSaveAs);
            this.panelCommand.Controls.Add(this.butDefault);
            this.panelCommand.Controls.Add(this.butSave);
            this.panelCommand.Controls.Add(this.butDelete);
            this.panelCommand.Controls.Add(this.butChoice);
            this.panelCommand.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelCommand.Location = new System.Drawing.Point(0, 358);
            this.panelCommand.Name = "panelCommand";
            this.panelCommand.Size = new System.Drawing.Size(690, 44);
            this.panelCommand.TabIndex = 0;
            // 
            // butConfigDown
            // 
            this.butConfigDown.Location = new System.Drawing.Point(65, 9);
            this.butConfigDown.Name = "butConfigDown";
            this.butConfigDown.Size = new System.Drawing.Size(21, 29);
            this.butConfigDown.TabIndex = 7;
            this.butConfigDown.Text = "↓";
            this.butConfigDown.UseVisualStyleBackColor = true;
            this.butConfigDown.Click += new System.EventHandler(this.butConfigDown_Click);
            // 
            // butConfigUp
            // 
            this.butConfigUp.Location = new System.Drawing.Point(23, 9);
            this.butConfigUp.Name = "butConfigUp";
            this.butConfigUp.Size = new System.Drawing.Size(21, 29);
            this.butConfigUp.TabIndex = 6;
            this.butConfigUp.Text = "↑";
            this.butConfigUp.UseVisualStyleBackColor = true;
            this.butConfigUp.Click += new System.EventHandler(this.butConfigUp_Click);
            // 
            // butGetWidth
            // 
            this.butGetWidth.Location = new System.Drawing.Point(596, 7);
            this.butGetWidth.Name = "butGetWidth";
            this.butGetWidth.Size = new System.Drawing.Size(81, 31);
            this.butGetWidth.TabIndex = 5;
            this.butGetWidth.Text = "获取宽度(&G)";
            this.butGetWidth.UseVisualStyleBackColor = true;
            this.butGetWidth.Click += new System.EventHandler(this.butGetWidth_Click);
            // 
            // butSaveAs
            // 
            this.butSaveAs.Location = new System.Drawing.Point(335, 7);
            this.butSaveAs.Name = "butSaveAs";
            this.butSaveAs.Size = new System.Drawing.Size(81, 31);
            this.butSaveAs.TabIndex = 4;
            this.butSaveAs.Text = "另存为(&A)";
            this.butSaveAs.UseVisualStyleBackColor = true;
            this.butSaveAs.Click += new System.EventHandler(this.butSaveAs_Click);
            // 
            // butDefault
            // 
            this.butDefault.Location = new System.Drawing.Point(509, 7);
            this.butDefault.Name = "butDefault";
            this.butDefault.Size = new System.Drawing.Size(81, 31);
            this.butDefault.TabIndex = 3;
            this.butDefault.Text = "设为默认(&D)";
            this.butDefault.UseVisualStyleBackColor = true;
            this.butDefault.Click += new System.EventHandler(this.butDefault_Click);
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(248, 7);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(81, 31);
            this.butSave.TabIndex = 2;
            this.butSave.Text = "保存(&S)";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // butDelete
            // 
            this.butDelete.Location = new System.Drawing.Point(422, 6);
            this.butDelete.Name = "butDelete";
            this.butDelete.Size = new System.Drawing.Size(81, 31);
            this.butDelete.TabIndex = 1;
            this.butDelete.Text = "删除(&R)";
            this.butDelete.UseVisualStyleBackColor = true;
            this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
            // 
            // butChoice
            // 
            this.butChoice.Location = new System.Drawing.Point(161, 7);
            this.butChoice.Name = "butChoice";
            this.butChoice.Size = new System.Drawing.Size(81, 31);
            this.butChoice.TabIndex = 0;
            this.butChoice.Text = "选择(&C)";
            this.butChoice.UseVisualStyleBackColor = true;
            this.butChoice.Click += new System.EventHandler(this.butChoice_Click);
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.splitTop);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(690, 358);
            this.panelTop.TabIndex = 1;
            // 
            // splitTop
            // 
            this.splitTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitTop.Location = new System.Drawing.Point(0, 0);
            this.splitTop.Name = "splitTop";
            // 
            // splitTop.Panel1
            // 
            this.splitTop.Panel1.Controls.Add(this.listConfig);
            // 
            // splitTop.Panel2
            // 
            this.splitTop.Panel2.Controls.Add(this.panelConfigTop);
            this.splitTop.Panel2.Controls.Add(this.splitter1);
            this.splitTop.Panel2.Controls.Add(this.panelConfigBottom);
            this.splitTop.Size = new System.Drawing.Size(690, 358);
            this.splitTop.SplitterDistance = 112;
            this.splitTop.TabIndex = 0;
            // 
            // listConfig
            // 
            this.listConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listConfig.FullRowSelect = true;
            this.listConfig.HideSelection = false;
            this.listConfig.Location = new System.Drawing.Point(0, 0);
            this.listConfig.MultiSelect = false;
            this.listConfig.Name = "listConfig";
            this.listConfig.Size = new System.Drawing.Size(112, 358);
            this.listConfig.TabIndex = 0;
            this.listConfig.UseCompatibleStateImageBehavior = false;
            this.listConfig.View = System.Windows.Forms.View.Details;
            this.listConfig.SelectedIndexChanged += new System.EventHandler(this.listConfig_SelectedIndexChanged);
            // 
            // panelConfigTop
            // 
            this.panelConfigTop.AutoScroll = true;
            this.panelConfigTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelConfigTop.Location = new System.Drawing.Point(0, 0);
            this.panelConfigTop.Name = "panelConfigTop";
            this.panelConfigTop.Size = new System.Drawing.Size(574, 190);
            this.panelConfigTop.TabIndex = 2;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 190);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(574, 10);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panelConfigBottom
            // 
            this.panelConfigBottom.Controls.Add(this.gridColumn);
            this.panelConfigBottom.Controls.Add(this.panel1);
            this.panelConfigBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelConfigBottom.Location = new System.Drawing.Point(0, 200);
            this.panelConfigBottom.Name = "panelConfigBottom";
            this.panelConfigBottom.Size = new System.Drawing.Size(574, 158);
            this.panelConfigBottom.TabIndex = 0;
            // 
            // gridColumn
            // 
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.gridColumn.DisplayLayout.Appearance = appearance1;
            ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn1.Header.Caption = "序号";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 58;
            ultraGridColumn2.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn2.Header.Caption = "字段名";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 84;
            ultraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn3.Header.Caption = "原始名称";
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn4.Header.Caption = "新名称";
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn5.Header.Caption = "宽度";
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Width = 47;
            ultraGridColumn6.Header.Caption = "是否隐藏";
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn6.Width = 62;
            ultraGridColumn7.Header.Caption = "排序";
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn7.Width = 36;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7});
            this.gridColumn.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.gridColumn.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.gridColumn.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.gridColumn.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gridColumn.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.gridColumn.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gridColumn.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.gridColumn.DisplayLayout.MaxColScrollRegions = 1;
            this.gridColumn.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gridColumn.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.gridColumn.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.gridColumn.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.gridColumn.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.gridColumn.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.gridColumn.DisplayLayout.Override.CellAppearance = appearance8;
            this.gridColumn.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.gridColumn.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.gridColumn.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.gridColumn.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.gridColumn.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.gridColumn.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.gridColumn.DisplayLayout.Override.RowAppearance = appearance11;
            this.gridColumn.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.gridColumn.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.gridColumn.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.gridColumn.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.gridColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridColumn.Location = new System.Drawing.Point(0, 0);
            this.gridColumn.Name = "gridColumn";
            this.gridColumn.Size = new System.Drawing.Size(529, 158);
            this.gridColumn.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.butDown);
            this.panel1.Controls.Add(this.butUp);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(529, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(45, 158);
            this.panel1.TabIndex = 0;
            // 
            // butDown
            // 
            this.butDown.AutoSize = true;
            this.butDown.Location = new System.Drawing.Point(11, 93);
            this.butDown.Name = "butDown";
            this.butDown.Size = new System.Drawing.Size(27, 22);
            this.butDown.TabIndex = 1;
            this.butDown.Text = "↓";
            this.butDown.UseVisualStyleBackColor = true;
            this.butDown.Click += new System.EventHandler(this.butDown_Click);
            // 
            // butUp
            // 
            this.butUp.AutoSize = true;
            this.butUp.Location = new System.Drawing.Point(11, 33);
            this.butUp.Name = "butUp";
            this.butUp.Size = new System.Drawing.Size(27, 25);
            this.butUp.TabIndex = 0;
            this.butUp.Text = "↑";
            this.butUp.UseVisualStyleBackColor = true;
            this.butUp.Click += new System.EventHandler(this.butUp_Click);
            // 
            // frmColumnConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 402);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelCommand);
            this.Name = "frmColumnConfig";
            this.Text = "配置";
            this.Load += new System.EventHandler(this.frmColumnConfig_Load);
            this.panelCommand.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.splitTop.Panel1.ResumeLayout(false);
            this.splitTop.Panel2.ResumeLayout(false);
            this.splitTop.ResumeLayout(false);
            this.panelConfigBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridColumn)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelCommand;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.SplitContainer splitTop;
        private System.Windows.Forms.ListView listConfig;
        private System.Windows.Forms.Panel panelConfigTop;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panelConfigBottom;
        private Infragistics.Win.UltraWinGrid.UltraGrid gridColumn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button butDefault;
        private System.Windows.Forms.Button butSave;
        private System.Windows.Forms.Button butDelete;
        private System.Windows.Forms.Button butChoice;
        private System.Windows.Forms.Button butSaveAs;
        private System.Windows.Forms.Button butDown;
        private System.Windows.Forms.Button butUp;
        private System.Windows.Forms.Button butGetWidth;
        private System.Windows.Forms.Button butConfigDown;
        private System.Windows.Forms.Button butConfigUp;
    }
}