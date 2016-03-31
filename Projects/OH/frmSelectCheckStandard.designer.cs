namespace OH
{
    partial class frmSelectCheckStandard
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            this.txtCheckStandard = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.butOK = new System.Windows.Forms.Button();
            this.butCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.txtCheckStandard)).BeginInit();
            this.SuspendLayout();
            // 
            // txtCheckStandard
            // 
            this.txtCheckStandard.ButtonsRight.Add(editorButton1);
            this.txtCheckStandard.Location = new System.Drawing.Point(54, 33);
            this.txtCheckStandard.Name = "txtCheckStandard";
            this.txtCheckStandard.Size = new System.Drawing.Size(182, 21);
            this.txtCheckStandard.TabIndex = 43;
            this.txtCheckStandard.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtCheckStandard_EditorButtonClick);
            // 
            // butOK
            // 
            this.butOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.butOK.Location = new System.Drawing.Point(54, 78);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(78, 30);
            this.butOK.TabIndex = 44;
            this.butOK.Text = "确定(&O)";
            this.butOK.UseVisualStyleBackColor = true;
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // butCancel
            // 
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(158, 78);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(78, 30);
            this.butCancel.TabIndex = 45;
            this.butCancel.Text = "取消(&C)";
            this.butCancel.UseVisualStyleBackColor = true;
            // 
            // frmSelectCheckStandard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.butCancel;
            this.ClientSize = new System.Drawing.Size(284, 143);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.txtCheckStandard);
            this.Name = "frmSelectCheckStandard";
            this.Text = "选择物料考核标准";
            this.Load += new System.EventHandler(this.frmSelectCheckStandard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtCheckStandard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCheckStandard;
        private System.Windows.Forms.Button butOK;
        private System.Windows.Forms.Button butCancel;

    }
}