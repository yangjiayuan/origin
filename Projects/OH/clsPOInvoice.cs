using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Infragistics.Win.UltraWinGrid;
using Base;
using UI;
using System.Windows.Forms;

namespace OH
{
    public enum enuPOInvoice : int { StandPurchase =0, ConsignPurchase = 4 }

    public class clsPOInvoice : ToolDetailForm
    {
        private enuPOInvoice PayInvoiceType;

        public override bool AllowInsertRowInGrid(string TableName)
        {
            if ((TableName == "D_InvoiceItem") || ( TableName == "D_PaymentItem"))
                return true;
            else
                return false;
        }

        public override bool AllowDeleteRowInToolBar
        {
            get
            {
                return true;
            }
        }

        public override bool AllowUpdateInGrid(string TableName)
        {
                return true;
        }

        public override bool AllowInsertRowInToolBar
        {
            get
            {
                return true;
            }
        }
        public override bool AllowCheck
        {
            get
            {
                return false;
            }
        }

        public override bool AutoCode
        {
            get
            {
                return false;
            }
        }

        public override DataSet DeleteRowsInGrid(List<COMFields> DetailTableDefine)
        {
            if (_DetailForm.ActiveControl.GetType() == typeof(UltraGrid))
            {
                UltraGrid grid = _DetailForm.ActiveControl as UltraGrid;
                grid.PerformAction(UltraGridAction.DeleteRows);
                string TableName = null;
                foreach (string n in _DetailForm.GridMap.Keys)
                    if (_DetailForm.GridMap[n] == grid)
                        TableName = n;
                if (TableName == "D_PayInvoicePO")
                { 
                    this.UpdataTotalAmount();
                }
            }
            return _DetailForm.MainDataSet;

        }

        public void UpdataTotalAmount()
        {
            try
            {
                decimal Total = 0;

                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_PayInvoicePO"].Rows)
                    if (dr.RowState != DataRowState.Deleted)
                        Total += (decimal)dr["InvoiceAmount"];
                _ControlMap["Amount"].Text = Total.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            try
            {

                string Filter;
                frmBrowser frm;


                if (right.Paramters == null)
                    return null;
                string[] s = right.Paramters.Split(new char[] { ',' });
                if (s.Length > 0)
                    PayInvoiceType = (enuPOInvoice)int.Parse(s[0]);
                else
                    return null;


                if (PayInvoiceType == enuPOInvoice.StandPurchase)
                {
                    this.MainTableName = "V_POInvoice";
                    Filter = string.Format("V_POInvoice.OrderType={0}", (int)PayInvoiceType);

                    SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                    SortedList<string, object> Value = new SortedList<string, object>();
                    Value.Add("DocumentType", (int)PayInvoiceType);

                    defaultValue.Add("V_POInvoice", Value);

                    COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                    List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);
                    this._MainCOMFields = mainTableDefine;
                    this._DetailCOMFields = detailTableDefines;

                    frm = (frmBrowser)base.GetForm(right, mdiForm, Filter);
                    frm.DefaultValue = defaultValue;
                
                }
                else
                {
                    this.MainTableName = "V_POInvoice_C";
                    SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                    SortedList<string, object> Value = new SortedList<string, object>();
                    Value.Add("DocumentType", (int)PayInvoiceType);

                    defaultValue.Add("V_POInvoice_C", Value);

                    COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                    List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);

                    detailTableDefines[2]["PONumber"].Visible = COMField.Enum_Visible.NotVisible;

                    detailTableDefines[2]["Quantity"].Enable = false;
                    detailTableDefines[2]["Price"].Enable = false;
                    detailTableDefines[2]["BatchNo"].Enable = false;

                    this._MainCOMFields = mainTableDefine;
                    this._DetailCOMFields = detailTableDefines;

                    frm = (frmBrowser)base.GetForm(right, mdiForm);
                    frm.DefaultValue = defaultValue;
                
                }

                return frm;
            }
            catch (Exception ex)
            {
                Msg.Warning(string.Format("采购发票加载失败！%n {0}", ex.Message));
            }
            return null;
        }

        public override bool Check(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
        {
            return true;
        }

        public override bool UnCheck(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
        {
            return true;
        }
    }
}
