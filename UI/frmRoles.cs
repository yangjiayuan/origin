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
    public partial class frmRoles : UI.frmBrowser
    {
        public frmRoles(COMFields mainTable, List<COMFields> detailTable, string where,enuShowStatus showStatus):base(mainTable,detailTable, where,showStatus)
        {
            InitializeComponent();
        }
        public override void grid_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            if (this.ParentForm==null)
                toolSelect_Click(sender, EventArgs.Empty);
            else
                toolEdit_Click(sender, EventArgs.Empty);
        }

        public override void frm_Changed(object sender, DataTableEventArgs e)
        {
            base.frm_Changed(sender, e);
        }

        public override void toolEdit_Click(object sender, EventArgs e)
        {
            if (base.MainGrid.ActiveRow != null)
            {

                    DataSet ds; 
                    frmRole frmDetail = new frmRole();

                    frmDetail.Changed += new DataTableEventHandler(frm_Changed);
                    //frmRole.PageDown += new DataTableEventHandler(frm_PageDown);
                    //frmRole.PageUp += new DataTableEventHandler(frm_PageUp);
                    bool bShowData = false;
                    Guid MainID = (Guid)base.MainGrid.ActiveRow.Cells["ID"].Value;
                    ds = base.GetMainDataSet(MainID);
                    bShowData = frmDetail.ShowData(MainTableDefine, new DataSetEventArgs(ds), true, this.MdiParent);

                    frmDetail.Show();
                    if (bShowData && this.MdiParent != null)
                    {
                        frmDetail.FormClosed += new FormClosedEventHandler(base.frm_FormClosed);
                        SetEnabled(false);
                    }
                    base.toolRefrash_Click(sender, e);
                    
                }
        }
        public override void toolView_Click(object sender, EventArgs e)
        {
            if (base.MainGrid.ActiveRow != null)
            {

                DataSet ds;
                frmRole frmDetail = new frmRole();

                frmDetail.Changed += new DataTableEventHandler(frm_Changed);
                //frmRole.PageDown += new DataTableEventHandler(frm_PageDown);
                //frmRole.PageUp += new DataTableEventHandler(frm_PageUp);
                bool bShowData = false;
                Guid MainID = (Guid)base.MainGrid.ActiveRow.Cells["ID"].Value;
                ds = base.GetMainDataSet(MainID);
                bShowData = frmDetail.ShowData(MainTableDefine, new DataSetEventArgs(ds),false, this.MdiParent);

                frmDetail.Show();
                if (bShowData && this.MdiParent != null)
                {
                    frmDetail.FormClosed += new FormClosedEventHandler(base.frm_FormClosed);
                    SetEnabled(false);
                }

            }
        }

        public override void toolNew_Click(object sender, EventArgs e)
        {
            frmRole frmDetail = new frmRole();

            frmDetail.Changed += new DataTableEventHandler(frm_Changed);
            //frmRole.PageDown += new DataTableEventHandler(frm_PageDown);
            //frmRole.PageUp += new DataTableEventHandler(frm_PageUp);
            bool bShowData = false;

            DataSet ds = base.GetMainDataSet();  
            bShowData = frmDetail.ShowData(MainTableDefine, new DataSetEventArgs(ds), true, this.MdiParent);
            
            frmDetail.Show();
            if (bShowData && this.MdiParent != null)
            {
                frmDetail.FormClosed += new FormClosedEventHandler(base.frm_FormClosed);
                SetEnabled(false);
            }

        }
    }
}

