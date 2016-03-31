using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OH
{
    public partial class clsUpdateAsignDate : Form
    {
        public bool _isUpdateAsign = false;
        public int _intAsign = 0;
        public DateTime _AsignDate;
        public clsUpdateAsignDate()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.cbAsign.Enabled = checkBox1.Checked;
        }

        private void clsUpdateAsignDate_Load(object sender, EventArgs e)
        {
            this.cbAsign.Enabled = false;
            this.cbAsign.SelectedIndex = 0;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _isUpdateAsign = checkBox1.Checked;
            _intAsign = this.cbAsign.SelectedIndex;
            _AsignDate = this.dtAsign.Value;
        }
    }
}