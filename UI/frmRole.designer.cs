namespace UI
{
    partial class frmRole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRole));
            this.toolMain = new System.Windows.Forms.ToolStrip();
            this.toolNew = new System.Windows.Forms.ToolStripButton();
            this.toolEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolUp = new System.Windows.Forms.ToolStripButton();
            this.toolDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolClose = new System.Windows.Forms.ToolStripButton();
            this.tvRights = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ChkBoxDisable = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtDescription = new System.Windows.Forms.TextBox();
            this.TxtName = new System.Windows.Forms.TextBox();
            this.toolMain.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolMain
            // 
            this.toolMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolNew,
            this.toolEdit,
            this.toolStripSeparator1,
            this.toolSave,
            this.toolStripSeparator4,
            this.toolUp,
            this.toolDown,
            this.toolStripSeparator3,
            this.toolClose});
            this.toolMain.Location = new System.Drawing.Point(0, 0);
            this.toolMain.Name = "toolMain";
            this.toolMain.Size = new System.Drawing.Size(746, 37);
            this.toolMain.TabIndex = 2;
            this.toolMain.Text = "toolStrip1";
            // 
            // toolNew
            // 
            this.toolNew.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolNew.Image = ((System.Drawing.Image)(resources.GetObject("toolNew.Image")));
            this.toolNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolNew.Name = "toolNew";
            this.toolNew.Size = new System.Drawing.Size(39, 34);
            this.toolNew.Text = "新增";
            this.toolNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolEdit
            // 
            this.toolEdit.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolEdit.Image = ((System.Drawing.Image)(resources.GetObject("toolEdit.Image")));
            this.toolEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolEdit.Name = "toolEdit";
            this.toolEdit.Size = new System.Drawing.Size(39, 34);
            this.toolEdit.Text = "修改";
            this.toolEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 37);
            // 
            // toolSave
            // 
            this.toolSave.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolSave.Image = ((System.Drawing.Image)(resources.GetObject("toolSave.Image")));
            this.toolSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSave.Name = "toolSave";
            this.toolSave.Size = new System.Drawing.Size(39, 34);
            this.toolSave.Text = "保存";
            this.toolSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolSave.Click += new System.EventHandler(this.toolSave_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 37);
            // 
            // toolUp
            // 
            this.toolUp.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolUp.Image = ((System.Drawing.Image)(resources.GetObject("toolUp.Image")));
            this.toolUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolUp.Name = "toolUp";
            this.toolUp.Size = new System.Drawing.Size(39, 34);
            this.toolUp.Text = "上翻";
            this.toolUp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolDown
            // 
            this.toolDown.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolDown.Image = ((System.Drawing.Image)(resources.GetObject("toolDown.Image")));
            this.toolDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolDown.Name = "toolDown";
            this.toolDown.Size = new System.Drawing.Size(39, 34);
            this.toolDown.Text = "下翻";
            this.toolDown.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 37);
            // 
            // toolClose
            // 
            this.toolClose.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolClose.Image = ((System.Drawing.Image)(resources.GetObject("toolClose.Image")));
            this.toolClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolClose.Name = "toolClose";
            this.toolClose.Size = new System.Drawing.Size(39, 34);
            this.toolClose.Text = "关闭";
            this.toolClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolClose.Click += new System.EventHandler(this.toolClose_Click);
            // 
            // tvRights
            // 
            this.tvRights.CheckBoxes = true;
            this.tvRights.Location = new System.Drawing.Point(12, 143);
            this.tvRights.Name = "tvRights";
            this.tvRights.Size = new System.Drawing.Size(722, 327);
            this.tvRights.TabIndex = 3;
            this.tvRights.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvRights_AfterCheck);
            this.tvRights.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvRights_BeforeExpand);
            this.tvRights.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvRights_BeforeExpand);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ChkBoxDisable);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.TxtDescription);
            this.groupBox1.Controls.Add(this.TxtName);
            this.groupBox1.Location = new System.Drawing.Point(12, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(721, 93);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "基本";
            // 
            // ChkBoxDisable
            // 
            this.ChkBoxDisable.AutoSize = true;
            this.ChkBoxDisable.Location = new System.Drawing.Point(63, 77);
            this.ChkBoxDisable.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ChkBoxDisable.Name = "ChkBoxDisable";
            this.ChkBoxDisable.Size = new System.Drawing.Size(48, 16);
            this.ChkBoxDisable.TabIndex = 4;
            this.ChkBoxDisable.Text = "禁用";
            this.ChkBoxDisable.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "描述";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "名称";
            // 
            // TxtDescription
            // 
            this.TxtDescription.Location = new System.Drawing.Point(64, 53);
            this.TxtDescription.Name = "TxtDescription";
            this.TxtDescription.Size = new System.Drawing.Size(149, 21);
            this.TxtDescription.TabIndex = 1;
            // 
            // TxtName
            // 
            this.TxtName.Location = new System.Drawing.Point(63, 17);
            this.TxtName.Name = "TxtName";
            this.TxtName.Size = new System.Drawing.Size(150, 21);
            this.TxtName.TabIndex = 0;
            // 
            // frmRole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 481);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tvRights);
            this.Controls.Add(this.toolMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRole";
            this.Text = "角色";
            this.Load += new System.EventHandler(this.frmRole_Load);
            this.toolMain.ResumeLayout(false);
            this.toolMain.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ToolStrip toolMain;
        protected internal System.Windows.Forms.ToolStripButton toolNew;
        protected internal System.Windows.Forms.ToolStripButton toolEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        protected internal System.Windows.Forms.ToolStripButton toolSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolUp;
        private System.Windows.Forms.ToolStripButton toolDown;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolClose;
        private System.Windows.Forms.TreeView tvRights;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtDescription;
        private System.Windows.Forms.TextBox TxtName;
        private System.Windows.Forms.CheckBox ChkBoxDisable;
    }
}