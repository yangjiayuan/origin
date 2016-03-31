using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Base;
namespace UI
{
    public partial class frmAskName : Form
    {
        private string _Name;
        public frmAskName()
        {
            InitializeComponent();
        }

        private void butOk_Click(object sender, EventArgs e)
        {
            _Name = txtName.Text;
            if (_Name.Length == 0)
                Msg.Information("«Î ‰»Î√˚≥∆£°");
            else
                this.DialogResult = DialogResult.OK;
        }
        public string GetName()
        {
            if (this.ShowDialog() == DialogResult.OK)
            {
                return _Name;
            }
            return null;
        }
    }
}