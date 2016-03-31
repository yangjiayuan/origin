namespace OH
{
    partial class frmCalculateMaterialCheck
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
            this.butCalc = new System.Windows.Forms.Button();
            this.butCancel = new System.Windows.Forms.Button();
            this.lableNotes = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // butCalc
            // 
            this.butCalc.Location = new System.Drawing.Point(45, 56);
            this.butCalc.Name = "butCalc";
            this.butCalc.Size = new System.Drawing.Size(99, 39);
            this.butCalc.TabIndex = 0;
            this.butCalc.Text = "计算(&O)";
            this.butCalc.UseVisualStyleBackColor = true;
            this.butCalc.Click += new System.EventHandler(this.butCalc_Click);
            // 
            // butCancel
            // 
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(218, 56);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(99, 39);
            this.butCancel.TabIndex = 1;
            this.butCancel.Text = "取消(&C)";
            this.butCancel.UseVisualStyleBackColor = true;
            // 
            // lableNotes
            // 
            this.lableNotes.AutoSize = true;
            this.lableNotes.Location = new System.Drawing.Point(42, 20);
            this.lableNotes.Name = "lableNotes";
            this.lableNotes.Size = new System.Drawing.Size(65, 12);
            this.lableNotes.TabIndex = 2;
            this.lableNotes.Text = "没有计算过";
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Location = new System.Drawing.Point(8, 113);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResult.Size = new System.Drawing.Size(335, 169);
            this.txtResult.TabIndex = 3;
            // 
            // frmCalculateMaterialCheck
            // 
            this.AcceptButton = this.butCalc;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.butCancel;
            this.ClientSize = new System.Drawing.Size(353, 289);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.lableNotes);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butCalc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmCalculateMaterialCheck";
            this.Text = "计算物料考核";
            this.Load += new System.EventHandler(this.frmCalculateMaterialCheck_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button butCalc;
        private System.Windows.Forms.Button butCancel;
        private System.Windows.Forms.Label lableNotes;
        private System.Windows.Forms.TextBox txtResult;
    }
}