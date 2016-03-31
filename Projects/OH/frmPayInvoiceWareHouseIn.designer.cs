namespace OH
{
    partial class frmPayInvoiceWareHouseIn
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton4 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton5 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton6 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BuyOrderCode");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WarehouseName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WarehouseInCode");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BillDate");
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            this.panelCommand = new System.Windows.Forms.Panel();
            this.butCancel = new System.Windows.Forms.Button();
            this.butOk = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtWarehouse = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWarehouseIn = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBuyOrder = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.panelGrid = new System.Windows.Forms.Panel();
            this.grid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.butDelete = new System.Windows.Forms.Button();
            this.panelCommand.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWarehouse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWarehouseIn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyOrder)).BeginInit();
            this.panelGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // 
            // panelCommand
            // 
            this.panelCommand.Controls.Add(this.butDelete);
            this.panelCommand.Controls.Add(this.butCancel);
            this.panelCommand.Controls.Add(this.butOk);
            this.panelCommand.Controls.Add(this.label3);
            this.panelCommand.Controls.Add(this.txtWarehouse);
            this.panelCommand.Controls.Add(this.label2);
            this.panelCommand.Controls.Add(this.txtWarehouseIn);
            this.panelCommand.Controls.Add(this.label1);
            this.panelCommand.Controls.Add(this.txtBuyOrder);
            this.panelCommand.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCommand.Location = new System.Drawing.Point(0, 0);
            this.panelCommand.Name = "panelCommand";
            this.panelCommand.Size = new System.Drawing.Size(579, 75);
            this.panelCommand.TabIndex = 0;
            // 
            // butCancel
            // 
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(480, 41);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(87, 24);
            this.butCancel.TabIndex = 27;
            this.butCancel.Text = "取消(&C)";
            this.butCancel.UseVisualStyleBackColor = true;
            // 
            // butOk
            // 
            this.butOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.butOk.Location = new System.Drawing.Point(356, 41);
            this.butOk.Name = "butOk";
            this.butOk.Size = new System.Drawing.Size(87, 23);
            this.butOk.TabIndex = 26;
            this.butOk.Text = "确定(&O)";
            this.butOk.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(214, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 25;
            this.label3.Text = "仓库";
            // 
            // txtWarehouse
            // 
            appearance16.TextHAlignAsString = "Center";
            appearance16.TextVAlignAsString = "Top";
            editorButton4.Appearance = appearance16;
            editorButton4.Text = "";
            this.txtWarehouse.ButtonsRight.Add(editorButton4);
            this.txtWarehouse.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtWarehouse.Location = new System.Drawing.Point(260, 3);
            this.txtWarehouse.Name = "txtWarehouse";
            this.txtWarehouse.Size = new System.Drawing.Size(120, 23);
            this.txtWarehouse.TabIndex = 24;
            this.txtWarehouse.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtWarehouse_EditorButtonClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(402, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 23;
            this.label2.Text = "入库单";
            // 
            // txtWarehouseIn
            // 
            appearance17.TextHAlignAsString = "Center";
            appearance17.TextVAlignAsString = "Top";
            editorButton5.Appearance = appearance17;
            editorButton5.Text = "";
            this.txtWarehouseIn.ButtonsRight.Add(editorButton5);
            this.txtWarehouseIn.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtWarehouseIn.Location = new System.Drawing.Point(449, 3);
            this.txtWarehouseIn.Name = "txtWarehouseIn";
            this.txtWarehouseIn.Size = new System.Drawing.Size(120, 23);
            this.txtWarehouseIn.TabIndex = 22;
            this.txtWarehouseIn.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtWarehouseIn_EditorButtonClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 21;
            this.label1.Text = "采购订单";
            // 
            // txtBuyOrder
            // 
            appearance18.TextHAlignAsString = "Center";
            appearance18.TextVAlignAsString = "Top";
            editorButton6.Appearance = appearance18;
            editorButton6.Text = "";
            this.txtBuyOrder.ButtonsRight.Add(editorButton6);
            this.txtBuyOrder.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtBuyOrder.Location = new System.Drawing.Point(71, 3);
            this.txtBuyOrder.Name = "txtBuyOrder";
            this.txtBuyOrder.Size = new System.Drawing.Size(120, 23);
            this.txtBuyOrder.TabIndex = 20;
            this.txtBuyOrder.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtBuyOrder_EditorButtonClick);
            // 
            // panelGrid
            // 
            this.panelGrid.Controls.Add(this.grid);
            this.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrid.Location = new System.Drawing.Point(0, 75);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Size = new System.Drawing.Size(579, 258);
            this.panelGrid.TabIndex = 1;
            // 
            // grid
            // 
            appearance19.BackColor = System.Drawing.SystemColors.Window;
            appearance19.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grid.DisplayLayout.Appearance = appearance19;
            ultraGridColumn5.Header.Caption = "采购订单号";
            ultraGridColumn5.Header.VisiblePosition = 0;
            ultraGridColumn6.Header.Caption = "仓库";
            ultraGridColumn6.Header.VisiblePosition = 1;
            ultraGridColumn6.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(103, 0);
            ultraGridColumn7.Header.Caption = "入库单号";
            ultraGridColumn7.Header.VisiblePosition = 2;
            ultraGridColumn7.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(110, 0);
            ultraGridColumn8.Header.Caption = "入库日期";
            ultraGridColumn8.Header.VisiblePosition = 3;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8});
            ultraGridBand2.UseRowLayout = true;
            this.grid.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance20.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance20.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance20.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance20.BorderColor = System.Drawing.SystemColors.Window;
            this.grid.DisplayLayout.GroupByBox.Appearance = appearance20;
            appearance21.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance21;
            this.grid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance22.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance22.BackColor2 = System.Drawing.SystemColors.Control;
            appearance22.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance22.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grid.DisplayLayout.GroupByBox.PromptAppearance = appearance22;
            this.grid.DisplayLayout.MaxColScrollRegions = 1;
            this.grid.DisplayLayout.MaxRowScrollRegions = 1;
            appearance23.BackColor = System.Drawing.SystemColors.Window;
            appearance23.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grid.DisplayLayout.Override.ActiveCellAppearance = appearance23;
            appearance24.BackColor = System.Drawing.SystemColors.Highlight;
            appearance24.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grid.DisplayLayout.Override.ActiveRowAppearance = appearance24;
            this.grid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.grid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance25.BackColor = System.Drawing.SystemColors.Window;
            this.grid.DisplayLayout.Override.CardAreaAppearance = appearance25;
            appearance26.BorderColor = System.Drawing.Color.Silver;
            appearance26.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grid.DisplayLayout.Override.CellAppearance = appearance26;
            this.grid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grid.DisplayLayout.Override.CellPadding = 0;
            appearance27.BackColor = System.Drawing.SystemColors.Control;
            appearance27.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance27.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance27.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance27.BorderColor = System.Drawing.SystemColors.Window;
            this.grid.DisplayLayout.Override.GroupByRowAppearance = appearance27;
            appearance28.TextHAlignAsString = "Left";
            this.grid.DisplayLayout.Override.HeaderAppearance = appearance28;
            this.grid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance29.BackColor = System.Drawing.SystemColors.Window;
            appearance29.BorderColor = System.Drawing.Color.Silver;
            this.grid.DisplayLayout.Override.RowAppearance = appearance29;
            appearance30.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grid.DisplayLayout.Override.TemplateAddRowAppearance = appearance30;
            this.grid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(0, 0);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(579, 258);
            this.grid.TabIndex = 6;
            this.grid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.grid.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.grid_BeforeRowsDeleted);
            // 
            // butDelete
            // 
            this.butDelete.Location = new System.Drawing.Point(216, 41);
            this.butDelete.Name = "butDelete";
            this.butDelete.Size = new System.Drawing.Size(87, 24);
            this.butDelete.TabIndex = 28;
            this.butDelete.Text = "删除(&D)";
            this.butDelete.UseVisualStyleBackColor = true;
            this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
            // 
            // frmPayInvoiceWareHouseIn
            // 
            this.AcceptButton = this.butOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.butCancel;
            this.ClientSize = new System.Drawing.Size(579, 333);
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelCommand);
            this.Name = "frmPayInvoiceWareHouseIn";
            this.Text = "选择采购订单或入库单";
            this.Load += new System.EventHandler(this.frmPayInvoiceWareHouseIn_Load);
            this.panelCommand.ResumeLayout(false);
            this.panelCommand.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWarehouse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWarehouseIn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyOrder)).EndInit();
            this.panelGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelCommand;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBuyOrder;
        private System.Windows.Forms.Button butCancel;
        private System.Windows.Forms.Button butOk;
        private System.Windows.Forms.Label label3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtWarehouse;
        private System.Windows.Forms.Label label2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtWarehouseIn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelGrid;
        private Infragistics.Win.UltraWinGrid.UltraGrid grid;
        private System.Windows.Forms.Button butDelete;
    }
}