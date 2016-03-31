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
    public partial class frmSelectDetailForm : Form
    {
        private string _DetailTable;
        private string _DetailTitle;
        private COMFields _Fields;
        private List<COMFields> _ListDetail;
        public frmSelectDetailForm()
        {
            InitializeComponent();
        }
        public frmSelectDetailForm(List<COMFields> listDetail):this()
        {
            _ListDetail = listDetail;
            foreach (COMFields fields in listDetail)
            {
                cmbDetail.Items.Add(fields.Property);
            }
        }
        public COMFields Fields
        {
            get { return _Fields; }
        }
        private void butOK_Click(object sender, EventArgs e)
        {
            if (cmbDetail.SelectedIndex > -1)
            {
                CTableProperty property = (CTableProperty)cmbDetail.Items[cmbDetail.SelectedIndex];
                _DetailTitle = property.Title;
                _DetailTable = property.TableName;
                foreach (COMFields fields in _ListDetail)
                {
                    if (fields.OrinalTableName == _DetailTable)
                    {
                        _Fields = fields;
                        break;
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}