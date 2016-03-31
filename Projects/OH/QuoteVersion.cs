using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Base;

namespace OH
{
    public partial class QuoteVersion : Form
    {
        string _Version = "";
        Boolean _DefaultVersion = false;
        public bool _GetDefaultVersion
        {
            get
            {
                return _DefaultVersion;
            }
            set
            {
                this.chkDefault.Checked  = value;
            }
        }
        public string _GetVersion
        {
            get
            {
                return _Version;
            }
            set
            {
                txtVersion.Text = value;
            }
        }
        public DateTime _startDate;
        public DateTime _endDate;
        public QuoteVersion()
        {
            InitializeComponent();
            this.startDate.Value = DateTime.Now;
            this.endDate.Value = DateTime.Now.AddMonths(1);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string notes = this.txtVersion.Text;
            if (notes.Length == 0)
            {
                Msg.Error("请输入版本信息");
                return;
            }
            else
            {
                _Version = this.txtVersion.Text;
                _DefaultVersion = this.chkDefault.Checked;
                _startDate = this.startDate.Value;
                _endDate = this.endDate.Value;
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}