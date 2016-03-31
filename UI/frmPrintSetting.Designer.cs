namespace UI
{
    partial class frmPrintSetting
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrintSetting));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkOverprint = new System.Windows.Forms.CheckBox();
            this.numericPageRow = new System.Windows.Forms.NumericUpDown();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnHorizontal = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnVertical = new System.Windows.Forms.Button();
            this.numPageHeight = new System.Windows.Forms.NumericUpDown();
            this.labHeight = new System.Windows.Forms.Label();
            this.numPageWidth = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.cmbPrintName = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cmbPageSize = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnCanel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.numTop = new System.Windows.Forms.NumericUpDown();
            this.numLeft = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtPageNum = new System.Windows.Forms.TextBox();
            this.radPageNum = new System.Windows.Forms.RadioButton();
            this.radAll = new System.Windows.Forms.RadioButton();
            this.grpTemplate = new System.Windows.Forms.GroupBox();
            this.rabNoUse = new System.Windows.Forms.RadioButton();
            this.rabUse = new System.Windows.Forms.RadioButton();
            this.cmbTemplate = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numRight = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numBottom = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPageRow)).BeginInit();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPageHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPageWidth)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLeft)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.grpTemplate.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBottom)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "上(&T) ";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkOverprint);
            this.groupBox4.Controls.Add(this.numericPageRow);
            this.groupBox4.Controls.Add(this.groupBox8);
            this.groupBox4.Controls.Add(this.numPageHeight);
            this.groupBox4.Controls.Add(this.labHeight);
            this.groupBox4.Controls.Add(this.numPageWidth);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label26);
            this.groupBox4.Controls.Add(this.cmbPrintName);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.cmbPageSize);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(409, 192);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "打印设置";
            // 
            // chkOverprint
            // 
            this.chkOverprint.AutoSize = true;
            this.chkOverprint.Location = new System.Drawing.Point(281, 160);
            this.chkOverprint.Name = "chkOverprint";
            this.chkOverprint.Size = new System.Drawing.Size(72, 16);
            this.chkOverprint.TabIndex = 20;
            this.chkOverprint.Text = "是否套打";
            this.chkOverprint.UseVisualStyleBackColor = true;
            // 
            // numericPageRow
            // 
            this.numericPageRow.Location = new System.Drawing.Point(281, 133);
            this.numericPageRow.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericPageRow.Name = "numericPageRow";
            this.numericPageRow.Size = new System.Drawing.Size(83, 21);
            this.numericPageRow.TabIndex = 19;
            this.numericPageRow.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.label6);
            this.groupBox8.Controls.Add(this.label5);
            this.groupBox8.Controls.Add(this.btnHorizontal);
            this.groupBox8.Controls.Add(this.btnVertical);
            this.groupBox8.Location = new System.Drawing.Point(177, 26);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(228, 93);
            this.groupBox8.TabIndex = 17;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "方  向";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(174, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "横向(&S)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(70, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "纵向(&P)";
            // 
            // btnHorizontal
            // 
            this.btnHorizontal.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnHorizontal.ImageIndex = 3;
            this.btnHorizontal.ImageList = this.imageList1;
            this.btnHorizontal.Location = new System.Drawing.Point(125, 24);
            this.btnHorizontal.Name = "btnHorizontal";
            this.btnHorizontal.Size = new System.Drawing.Size(86, 56);
            this.btnHorizontal.TabIndex = 5;
            this.btnHorizontal.UseVisualStyleBackColor = true;
            this.btnHorizontal.Click += new System.EventHandler(this.btnHorizontal_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "verticalIs");
            this.imageList1.Images.SetKeyName(1, "verticalNo");
            this.imageList1.Images.SetKeyName(2, "horizontalIs");
            this.imageList1.Images.SetKeyName(3, "horizontalNo");
            // 
            // btnVertical
            // 
            this.btnVertical.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVertical.ImageIndex = 1;
            this.btnVertical.ImageList = this.imageList1;
            this.btnVertical.Location = new System.Drawing.Point(21, 24);
            this.btnVertical.Name = "btnVertical";
            this.btnVertical.Size = new System.Drawing.Size(98, 56);
            this.btnVertical.TabIndex = 4;
            this.btnVertical.UseVisualStyleBackColor = true;
            this.btnVertical.Click += new System.EventHandler(this.btnVertical_Click);
            // 
            // numPageHeight
            // 
            this.numPageHeight.Location = new System.Drawing.Point(71, 155);
            this.numPageHeight.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numPageHeight.Name = "numPageHeight";
            this.numPageHeight.Size = new System.Drawing.Size(95, 21);
            this.numPageHeight.TabIndex = 16;
            this.numPageHeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numPageHeight.ValueChanged += new System.EventHandler(this.numPageHeight_ValueChanged);
            // 
            // labHeight
            // 
            this.labHeight.AutoSize = true;
            this.labHeight.Location = new System.Drawing.Point(15, 164);
            this.labHeight.Name = "labHeight";
            this.labHeight.Size = new System.Drawing.Size(47, 12);
            this.labHeight.TabIndex = 15;
            this.labHeight.Text = "高度(&H)";
            // 
            // numPageWidth
            // 
            this.numPageWidth.Location = new System.Drawing.Point(71, 131);
            this.numPageWidth.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numPageWidth.Name = "numPageWidth";
            this.numPageWidth.Size = new System.Drawing.Size(95, 21);
            this.numPageWidth.TabIndex = 14;
            this.numPageWidth.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numPageWidth.ValueChanged += new System.EventHandler(this.numPageWidth_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(192, 136);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "每张打印份数:";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(15, 136);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(47, 12);
            this.label26.TabIndex = 13;
            this.label26.Text = "宽度(&W)";
            // 
            // cmbPrintName
            // 
            this.cmbPrintName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrintName.FormattingEnabled = true;
            this.cmbPrintName.Location = new System.Drawing.Point(21, 45);
            this.cmbPrintName.Name = "cmbPrintName";
            this.cmbPrintName.Size = new System.Drawing.Size(145, 20);
            this.cmbPrintName.TabIndex = 7;
            this.cmbPrintName.SelectedIndexChanged += new System.EventHandler(this.cmbPrintName_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(22, 28);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 12);
            this.label13.TabIndex = 6;
            this.label13.Text = "打印机(&C)";
            // 
            // cmbPageSize
            // 
            this.cmbPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPageSize.FormattingEnabled = true;
            this.cmbPageSize.Location = new System.Drawing.Point(21, 88);
            this.cmbPageSize.Name = "cmbPageSize";
            this.cmbPageSize.Size = new System.Drawing.Size(145, 20);
            this.cmbPageSize.TabIndex = 1;
            this.cmbPageSize.SelectedIndexChanged += new System.EventHandler(this.cmbPageSize_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 71);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "纸张大小(&R)";
            // 
            // btnCanel
            // 
            this.btnCanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCanel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCanel.Location = new System.Drawing.Point(316, 11);
            this.btnCanel.Name = "btnCanel";
            this.btnCanel.Size = new System.Drawing.Size(79, 30);
            this.btnCanel.TabIndex = 4;
            this.btnCanel.Text = "取消(&C)";
            this.btnCanel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(231, 10);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(79, 30);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "确定(&O)";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 421);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(423, 46);
            this.panel1.TabIndex = 7;
            // 
            // numTop
            // 
            this.numTop.Location = new System.Drawing.Point(47, 19);
            this.numTop.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numTop.Name = "numTop";
            this.numTop.Size = new System.Drawing.Size(51, 21);
            this.numTop.TabIndex = 1;
            this.numTop.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // numLeft
            // 
            this.numLeft.Location = new System.Drawing.Point(47, 46);
            this.numLeft.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numLeft.Name = "numLeft";
            this.numLeft.Size = new System.Drawing.Size(51, 21);
            this.numLeft.TabIndex = 5;
            this.numLeft.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "左(&L) ";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(423, 419);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.grpTemplate);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(415, 394);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "打印设置";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtPageNum);
            this.groupBox3.Controls.Add(this.radPageNum);
            this.groupBox3.Controls.Add(this.radAll);
            this.groupBox3.Location = new System.Drawing.Point(214, 213);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(199, 73);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "打印范围";
            // 
            // txtPageNum
            // 
            this.txtPageNum.Enabled = false;
            this.txtPageNum.Location = new System.Drawing.Point(81, 44);
            this.txtPageNum.Name = "txtPageNum";
            this.txtPageNum.Size = new System.Drawing.Size(113, 21);
            this.txtPageNum.TabIndex = 2;
            // 
            // radPageNum
            // 
            this.radPageNum.AutoSize = true;
            this.radPageNum.Location = new System.Drawing.Point(11, 46);
            this.radPageNum.Name = "radPageNum";
            this.radPageNum.Size = new System.Drawing.Size(71, 16);
            this.radPageNum.TabIndex = 1;
            this.radPageNum.Text = "页码范围";
            this.radPageNum.UseVisualStyleBackColor = true;
            // 
            // radAll
            // 
            this.radAll.AutoSize = true;
            this.radAll.Checked = true;
            this.radAll.Location = new System.Drawing.Point(12, 19);
            this.radAll.Name = "radAll";
            this.radAll.Size = new System.Drawing.Size(47, 16);
            this.radAll.TabIndex = 0;
            this.radAll.TabStop = true;
            this.radAll.Text = "全部";
            this.radAll.UseVisualStyleBackColor = true;
            // 
            // grpTemplate
            // 
            this.grpTemplate.Controls.Add(this.rabNoUse);
            this.grpTemplate.Controls.Add(this.rabUse);
            this.grpTemplate.Controls.Add(this.cmbTemplate);
            this.grpTemplate.Location = new System.Drawing.Point(4, 302);
            this.grpTemplate.Name = "grpTemplate";
            this.grpTemplate.Size = new System.Drawing.Size(406, 69);
            this.grpTemplate.TabIndex = 5;
            this.grpTemplate.TabStop = false;
            this.grpTemplate.Text = "模板选择";
            // 
            // rabNoUse
            // 
            this.rabNoUse.AutoSize = true;
            this.rabNoUse.Location = new System.Drawing.Point(68, 34);
            this.rabNoUse.Name = "rabNoUse";
            this.rabNoUse.Size = new System.Drawing.Size(59, 16);
            this.rabNoUse.TabIndex = 2;
            this.rabNoUse.TabStop = true;
            this.rabNoUse.Text = "不使用";
            this.rabNoUse.UseVisualStyleBackColor = true;
            // 
            // rabUse
            // 
            this.rabUse.AutoSize = true;
            this.rabUse.Checked = true;
            this.rabUse.Location = new System.Drawing.Point(13, 34);
            this.rabUse.Name = "rabUse";
            this.rabUse.Size = new System.Drawing.Size(47, 16);
            this.rabUse.TabIndex = 1;
            this.rabUse.TabStop = true;
            this.rabUse.Text = "使用";
            this.rabUse.UseVisualStyleBackColor = true;
            // 
            // cmbTemplate
            // 
            this.cmbTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTemplate.FormattingEnabled = true;
            this.cmbTemplate.Location = new System.Drawing.Point(145, 32);
            this.cmbTemplate.Name = "cmbTemplate";
            this.cmbTemplate.Size = new System.Drawing.Size(251, 20);
            this.cmbTemplate.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numRight);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numLeft);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numBottom);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numTop);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 214);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(201, 72);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "页边距";
            // 
            // numRight
            // 
            this.numRight.Location = new System.Drawing.Point(143, 46);
            this.numRight.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numRight.Name = "numRight";
            this.numRight.Size = new System.Drawing.Size(50, 21);
            this.numRight.TabIndex = 7;
            this.numRight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(103, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "右(&R) ";
            // 
            // numBottom
            // 
            this.numBottom.Location = new System.Drawing.Point(143, 20);
            this.numBottom.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numBottom.Name = "numBottom";
            this.numBottom.Size = new System.Drawing.Size(50, 21);
            this.numBottom.TabIndex = 3;
            this.numBottom.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(103, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "下(&B) ";
            // 
            // frmPrintSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCanel;
            this.ClientSize = new System.Drawing.Size(423, 467);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Name = "frmPrintSetting";
            this.Text = "打印设置";
            this.Load += new System.EventHandler(this.frmPrintSetting_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPageRow)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPageHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPageWidth)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLeft)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.grpTemplate.ResumeLayout(false);
            this.grpTemplate.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBottom)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown numericPageRow;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnHorizontal;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnVertical;
        private System.Windows.Forms.NumericUpDown numPageHeight;
        private System.Windows.Forms.Label labHeight;
        private System.Windows.Forms.NumericUpDown numPageWidth;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.ComboBox cmbPrintName;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmbPageSize;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnCanel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown numTop;
        private System.Windows.Forms.NumericUpDown numLeft;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtPageNum;
        private System.Windows.Forms.RadioButton radPageNum;
        private System.Windows.Forms.RadioButton radAll;
        private System.Windows.Forms.GroupBox grpTemplate;
        private System.Windows.Forms.RadioButton rabNoUse;
        private System.Windows.Forms.RadioButton rabUse;
        private System.Windows.Forms.ComboBox cmbTemplate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numRight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numBottom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkOverprint;
    }
}