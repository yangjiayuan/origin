namespace OH
{
    partial class frmPeriodCarry
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
            this.labDescription = new System.Windows.Forms.Label();
            this.butPeriodCarry = new System.Windows.Forms.Button();
            this.butClose = new System.Windows.Forms.Button();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.butRefrash = new System.Windows.Forms.Button();
            this.butTurnBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labDescription
            // 
            this.labDescription.AutoSize = true;
            this.labDescription.Location = new System.Drawing.Point(26, 21);
            this.labDescription.Name = "labDescription";
            this.labDescription.Size = new System.Drawing.Size(59, 12);
            this.labDescription.TabIndex = 0;
            this.labDescription.Text = "系统提示:";
            // 
            // butPeriodCarry
            // 
            this.butPeriodCarry.Location = new System.Drawing.Point(97, 137);
            this.butPeriodCarry.Name = "butPeriodCarry";
            this.butPeriodCarry.Size = new System.Drawing.Size(81, 30);
            this.butPeriodCarry.TabIndex = 1;
            this.butPeriodCarry.Text = "月末结转(&P)";
            this.butPeriodCarry.UseVisualStyleBackColor = true;
            this.butPeriodCarry.Click += new System.EventHandler(this.butPeriodCarry_Click);
            // 
            // butClose
            // 
            this.butClose.Location = new System.Drawing.Point(277, 137);
            this.butClose.Name = "butClose";
            this.butClose.Size = new System.Drawing.Size(81, 30);
            this.butClose.TabIndex = 2;
            this.butClose.Text = "关闭(&C)";
            this.butClose.UseVisualStyleBackColor = true;
            this.butClose.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(28, 50);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ReadOnly = true;
            this.txtNotes.Size = new System.Drawing.Size(330, 65);
            this.txtNotes.TabIndex = 3;
            // 
            // butRefrash
            // 
            this.butRefrash.Location = new System.Drawing.Point(28, 137);
            this.butRefrash.Name = "butRefrash";
            this.butRefrash.Size = new System.Drawing.Size(56, 30);
            this.butRefrash.TabIndex = 4;
            this.butRefrash.Text = "刷新(&R)";
            this.butRefrash.UseVisualStyleBackColor = true;
            this.butRefrash.Click += new System.EventHandler(this.butRefrash_Click);
            // 
            // butTurnBack
            // 
            this.butTurnBack.Location = new System.Drawing.Point(184, 137);
            this.butTurnBack.Name = "butTurnBack";
            this.butTurnBack.Size = new System.Drawing.Size(81, 30);
            this.butTurnBack.TabIndex = 5;
            this.butTurnBack.Text = "反结转(&B)";
            this.butTurnBack.UseVisualStyleBackColor = true;
            this.butTurnBack.Click += new System.EventHandler(this.butTurnBack_Click);
            // 
            // frmPeriodCarry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 200);
            this.Controls.Add(this.butTurnBack);
            this.Controls.Add(this.butRefrash);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.butClose);
            this.Controls.Add(this.butPeriodCarry);
            this.Controls.Add(this.labDescription);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmPeriodCarry";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "月末结转处理";
            this.Load += new System.EventHandler(this.frmPeriodCarry_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labDescription;
        private System.Windows.Forms.Button butPeriodCarry;
        private System.Windows.Forms.Button butClose;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Button butRefrash;
        private System.Windows.Forms.Button butTurnBack;
    }
}