namespace UI
{
    partial class frmChangePassword
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.butOK = new System.Windows.Forms.Button();
            this.butCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOldPWD = new System.Windows.Forms.TextBox();
            this.txtRetry = new System.Windows.Forms.TextBox();
            this.txtNewPWD = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.butCancel);
            this.panel1.Controls.Add(this.butOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 142);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(359, 58);
            this.panel1.TabIndex = 0;
            // 
            // butOK
            // 
            this.butOK.Location = new System.Drawing.Point(69, 17);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(89, 29);
            this.butOK.TabIndex = 0;
            this.butOK.Text = "确定(&O)";
            this.butOK.UseVisualStyleBackColor = true;
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // butCancel
            // 
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(217, 17);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(89, 29);
            this.butCancel.TabIndex = 1;
            this.butCancel.Text = "取消(&C)";
            this.butCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "原密码";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "新密码";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "重输入";
            // 
            // txtOldPWD
            // 
            this.txtOldPWD.Location = new System.Drawing.Point(102, 20);
            this.txtOldPWD.Name = "txtOldPWD";
            this.txtOldPWD.PasswordChar = '*';
            this.txtOldPWD.Size = new System.Drawing.Size(204, 21);
            this.txtOldPWD.TabIndex = 4;
            // 
            // txtRetry
            // 
            this.txtRetry.Location = new System.Drawing.Point(102, 100);
            this.txtRetry.Name = "txtRetry";
            this.txtRetry.PasswordChar = '*';
            this.txtRetry.Size = new System.Drawing.Size(204, 21);
            this.txtRetry.TabIndex = 5;
            // 
            // txtNewPWD
            // 
            this.txtNewPWD.Location = new System.Drawing.Point(102, 61);
            this.txtNewPWD.Name = "txtNewPWD";
            this.txtNewPWD.PasswordChar = '*';
            this.txtNewPWD.Size = new System.Drawing.Size(204, 21);
            this.txtNewPWD.TabIndex = 6;
            // 
            // frmChangePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 200);
            this.Controls.Add(this.txtNewPWD);
            this.Controls.Add(this.txtRetry);
            this.Controls.Add(this.txtOldPWD);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "frmChangePassword";
            this.Text = "修改密码";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button butCancel;
        private System.Windows.Forms.Button butOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOldPWD;
        private System.Windows.Forms.TextBox txtRetry;
        private System.Windows.Forms.TextBox txtNewPWD;
    }
}