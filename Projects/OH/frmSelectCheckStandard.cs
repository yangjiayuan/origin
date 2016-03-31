using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Base;
using UI;

namespace OH
{
    public partial class frmSelectCheckStandard : Form,IMenuAction
    {
        DataSet ds;
        public frmSelectCheckStandard()
        {
            InitializeComponent();
        }
        private void butOK_Click(object sender, EventArgs e)
        {
            ds.Tables[0].Rows[0]["Value"] = txtCheckStandard.Tag;
            CSystem.Sys.Svr.cntMain.Update(ds.Tables[0]);
        }

        private void frmSelectCheckStandard_Load(object sender, EventArgs e)
        {
            ds = CSystem.Sys.Svr.cntMain.Select(@"Select * from S_BaseInfo where ID='物料考核\考核版本'","S_BaseInfo");
            if (ds.Tables[0].Rows.Count != 1)
            {
                Msg.Error("读取当前的考核版本失败！");
                return;
            }
            else
            {
                try
                {
                    Guid id = new Guid(ds.Tables[0].Rows[0]["Value"] as string);
                    CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from C_CheckStandard where ID='{0}'", id), "C_CheckStandard", ds);
                    if (ds.Tables["C_CheckStandard"].Rows.Count == 1)
                    {
                        txtCheckStandard.Text = (string)ds.Tables["C_CheckStandard"].Rows[0]["Name"];
                        txtCheckStandard.Tag = ds.Tables["C_CheckStandard"].Rows[0]["ID"];
                    }
                }
                catch { }
            }
        }

        private void txtCheckStandard_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("C_CheckStandard"), CSystem.Sys.Svr.Properties.DetailTableDefineList("C_CheckStandard"), null, enuShowStatus.None);
            frm.toolDetailForm = new clsCheckStandard();
            DataRow dr = frm.ShowSelect("", "", "");
            if (dr != null)
            {
                txtCheckStandard.Text = dr["Name"] as string;
                txtCheckStandard.Tag = dr["ID"];

            }
        }

        #region IMenuAction Members

        public Form GetForm(CRightItem right, Form mdiForm)
        {
            this.ShowDialog();
            return null;
        }

        #endregion
    }
}