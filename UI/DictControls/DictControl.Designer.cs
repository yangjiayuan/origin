namespace UI
{
    partial class DictControl
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
            this.ultraComboEditor1 = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.ultraComboEditor1)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraComboEditor1
            // 
            this.ultraComboEditor1.AutoSize = false;
            this.ultraComboEditor1.DisplayMember = "Name";
            this.ultraComboEditor1.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.ultraComboEditor1.Location = new System.Drawing.Point(0, 0);
            this.ultraComboEditor1.Name = "ultraComboEditor1";
            this.ultraComboEditor1.Size = new System.Drawing.Size(197, 20);
            this.ultraComboEditor1.TabIndex = 0;
            this.ultraComboEditor1.Text = "ultraComboEditor1";
            this.ultraComboEditor1.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ultraComboEditor1.ValueMember = "code";
            this.ultraComboEditor1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ultraComboEditor1_KeyDown);
            this.ultraComboEditor1.ValueChanged += new System.EventHandler(this.ultraComboEditor1_ValueChanged);
            // 
            // DictControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraComboEditor1);
            this.Name = "DictControl";
            this.Size = new System.Drawing.Size(197, 20);
            ((System.ComponentModel.ISupportInitialize)(this.ultraComboEditor1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraComboEditor ultraComboEditor1;
    }
}
