using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using System.Data;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinEditors;
using Base;
using UI;
using System.Windows.Forms;
namespace YUANYE
{
    //public enum enuPaymentType : int { Vendor = 0,  Employee= 1, Prepayment= 2 };

    public class clsPaymentAnalysis : ToolDetailForm
    {

        private ToolStripButton toolPrint;
        private ToolStripButton toolPreview;

        private Form _mdiForm;
        private enuPaymentType PaymentType;

        public override bool AutoCode
        {
            get
            {
                return true;
            }
        }
        public override string GetPrefix()
        {
            return "P";
        }
        public override void NewControl(COMField f, System.Windows.Forms.Control ctl)
        {

        }
        public override bool AllowCheck
        {
            get
            {
                return true;
            }
        }
        void curr_ValueChanged(object sender, EventArgs e)
        {
            if (!_DetailForm.Showed)
                return;
            UltraCurrencyEditor curr = sender as UltraCurrencyEditor;
            if (curr == null)
                return;

        }
        public override void SetCreateGrid(CCreateGrid createGrid)
        {
            base.SetCreateGrid(createGrid);
            createGrid.AfterSelectForm += new AfterSelectFormEventHandler(_CreateGrid_AfterSelectForm);
            createGrid.BeforeSelectForm += new BeforeSelectFormEventHandler(_CreateGrid_BeforeSelectForm);
        }

        public override void SetCreateDetail(CCreateDetailForm createDetailForm)
        {
            base.SetCreateDetail(createDetailForm);
            createDetailForm.BeforeSelectForm+= new BeforeSelectFormEventHandler(createDetailForm_BeforeSelectForm);
            //createDetailForm.AfterSelectForm += new AfterSelectFormEventHandler(createDetailForm_AfterSelectForm);
        }


        void createDetailForm_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            string[] s = e.Field.ValueType.Split(':');
            if (s[1] == "P_Vendor") 
            {
                switch (PaymentType)
                {
                    case enuPaymentType.Vendor:
                        e.Where = string.Format("P_Vendor.VendorType = '{0}'", 1);
                        break;

                    case enuPaymentType.Employee:
                        e.Where = string.Format("P_Vendor.VendorType = '{0}'", 0);
                        break;

                    case enuPaymentType.Prepayment:

                        break;

                    default:

                        break;

                }
            }
        }


        void _CreateGrid_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {

        }

        void _CreateGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {

        }


        public override void NewGrid(COMFields fields, UltraGrid grid)
        {
            grid.AfterCellUpdate += new CellEventHandler(grid_AfterCellUpdate);
            grid.AfterRowInsert += new RowEventHandler(grid_AfterRowInsert);
        }

        void grid_AfterRowInsert(object sender, RowEventArgs e)
        {
            //try
            //{
            //    e.Row.Cells["Date"].Value = ((UltraDateTimeEditor)_ControlMap["DocumentDate"]).DateTime;
            //}
            //catch { }
        }

        void grid_AfterCellUpdate(object sender, CellEventArgs e)
        {
             switch (e.Cell.Column.Key)
            {

                case "Amount":
                    UltraGrid grid2 = (UltraGrid)sender;

                    //如果需要在订单的表头级别记录下订单的总金额，则需要此功能来自动汇总计算.
                    decimal AmountTotal = 0;
                    foreach (UltraGridRow row in grid2.Rows)
                        if (row.Cells["Amount"].Value != DBNull.Value)
                            AmountTotal += (decimal)row.Cells["Amount"].Value;
                    _ControlMap["Amount"].Text = AmountTotal.ToString();

                    break;

                case "Rate":
                    break;
            }
 
        }
        

        public override bool AllowDeleteRowInToolBar
        {
            get
            {
                return true;
            }
        }
        public override bool AllowInsertRowInGrid(string TableName)
        {
            return true;
        }
        public override bool AllowUpdateInGrid(string TableName)
        {
            return true;
        }
        public override bool NewData(DataSet ds, COMFields mainTableDefine, List<COMFields> detailTableDefine)
        {

            return true;
        }




        public override string BeforeSaving(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                decimal total = 0;
   
                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_PaymentItems"].Rows)
                    if (dr.RowState != DataRowState.Deleted)
                        total += (decimal)dr["Amount"];
                _ControlMap["Amount"].Text = total.ToString();
 
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public override void InsertToolStrip(ToolStrip toolStrip, ToolDetailForm.enuInsertToolStripType insertType)
        {
            base.InsertToolStrip(toolStrip, insertType);
            int i = toolStrip.Items.Count;

            i = i - 2;
            toolPreview = new ToolStripButton();
            toolPreview.Font = new System.Drawing.Font("SimSun", 10.5F);
            toolPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolPreview.Name = "toolPreview";
            toolPreview.Size = new System.Drawing.Size(39, 34);
            toolPreview.Text = "打印";
            toolPreview.Image = UI.clsResources.Preview;
            toolPreview.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolPreview.Click += new EventHandler(toolPrint_Click);
            toolStrip.Items.Insert(i, toolPreview);

            toolPrint = new ToolStripButton();
            toolPrint.Font = new System.Drawing.Font("SimSun", 10.5F);
            toolPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolPrint.Name = "toolPrint";
            toolPrint.Size = new System.Drawing.Size(39, 34);
            toolPrint.Text = "预览";
            toolPrint.Image = UI.clsResources.Print;
            toolPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolPrint.Click += new EventHandler(toolPreview_Click);
            toolStrip.Items.Insert(i, toolPrint);

        }
        private void toolPreview_Click(object sender, EventArgs e)
        {
            PrintNew();
        }

        private void toolPrint_Click(object sender, EventArgs e)
        {
            PrintNew();
        }


        private void PrintNew()
        {
            Guid StorageInDocID;
            DataSet DS;

            if (this._BrowserForm.grid.ActiveRow == null)
                return;

            if (_BrowserForm.grid.Selected.Rows.Count == 0 && _BrowserForm.grid.ActiveRow != null)
                _BrowserForm.grid.ActiveRow.Selected = true;

            UltraGridRow row = _BrowserForm.grid.Selected.Rows[0];
            StorageInDocID = (Guid)row.Cells["ID"].Value;
            DS = GetData(StorageInDocID);
            FrmPrintViewer PV = new FrmPrintViewer("付款申请单", AppDomain.CurrentDomain.BaseDirectory + "PrintTemplate\\PrintPayment.rdlc", DS);
            PV.Text = "付款申请单";
            PV.MdiParent = _mdiForm;
            PV.Show();
        }

        private DataSet GetData(Guid MainID)
        {
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(this.MainCOMFields.QuerySQLWithClause(this.MainTableName + ".ID='" + MainID + "'"), this.MainTableName);
            CSystem.Sys.Svr.cntMain.Select(this.DetailCOMFields[0].QuerySQLWithClause(this.DetailCOMFields[0].GetTableName(false) + ".MainID='" + MainID + "'"), this.DetailCOMFields[0].GetTableName(false), ds);
            return ds;
        }


        public override bool UnCheck(Guid ID, SqlConnection conn, SqlTransaction sqlTran, DataRow mainRow)
        {
            return base.UnCheck(ID, conn, sqlTran, mainRow);
        }

        public override Form GetForm(CRightItem right, Form mdiForm)
        {
             
             try
            {
                string Filter;
                string FormTitle;

                _mdiForm = mdiForm;

                if (right.Paramters == null)
                    return null;
                string[] s = right.Paramters.Split(new char[] { ',' });
                if (s.Length > 0)
                    PaymentType = (enuPaymentType)int.Parse(s[0]);
                else
                    return null;

                this.MainTableName = "V_PLByIssue_Payment";
                Filter = string.Format("V_PLByIssue_Payment.Paymenttype={0}", (int)PaymentType);

                SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                SortedList<string, object> Value = new SortedList<string, object>();
                Value.Add("DocumentDate", CSystem.Sys.Svr.SystemTime.Date);
                Value.Add("PaymentType", (int)PaymentType);

                defaultValue.Add("V_PLByIssue_Payment", Value);

                COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);

                switch (PaymentType)
                {
                    case enuPaymentType.Vendor:
                        FormTitle = "供应商付款分析";
                        break;

                    case enuPaymentType.Employee:
                        FormTitle = "员工报销分析";
                        break;

                    case enuPaymentType.Prepayment:

                        FormTitle = "预付款分析";
                        break;
 
                    default:
                        FormTitle = "供应商付款分析";
                        break;

                }

                mainTableDefine.Property.Title = string.Format(FormTitle);
                this._MainCOMFields = mainTableDefine;
                this._DetailCOMFields = detailTableDefines;

                frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm, Filter);


                frm.DefaultValue = defaultValue;
                return frm;
            }
            catch (Exception ex)
            {
                Msg.Warning(string.Format("加载失败！%n {0}", ex.Message));
            }
            return null;

        }
    }
}
