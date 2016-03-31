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
    public enum enuSOBilling : int { StandSales = 0, ConsignSales =1 }

    public class clsSOBilling : ToolDetailForm
    {
        private enuSOBilling SOBillingType;

        public override bool AllowInsertRowInGrid(string TableName)
        {
            if ((TableName == "D_BillingItem") || ( TableName == "D_RevenueItem"))
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
        public override bool AllowNew
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

        //public override DataSet DeleteRowsInGrid(List<COMFields> DetailTableDefine)
        //{
        //    if (_DetailForm.ActiveControl.GetType() == typeof(UltraGrid))
        //    {
        //        UltraGrid grid = _DetailForm.ActiveControl as UltraGrid;
        //        grid.PerformAction(UltraGridAction.DeleteRows);
        //        string TableName = null;
        //        foreach (string n in _DetailForm.GridMap.Keys)
        //            if (_DetailForm.GridMap[n] == grid)
        //                TableName = n;
        //        if (TableName == "D_PayInvoicePO")
        //        { 
        //            this.UpdataTotalAmount();
        //        }
        //    }
        //    return _DetailForm.MainDataSet;

        //}

        //public void UpdataTotalAmount()
        //{
        //    try
        //    {
        //        decimal Total = 0;

        //        foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_PayInvoicePO"].Rows)
        //            if (dr.RowState != DataRowState.Deleted)
        //                Total += (decimal)dr["InvoiceAmount"];
        //        _ControlMap["Amount"].Text = Total.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

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
                    SOBillingType = (enuSOBilling)int.Parse(s[0]);
                else
                    return null;
                if (SOBillingType == enuSOBilling.StandSales)
                {
                    this.MainTableName = "V_SOBilling";
                    Filter = string.Format("V_SOBilling.OrderType={0}", (int)SOBillingType);
                    SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                    SortedList<string, object> Value = new SortedList<string, object>();
                    Value.Add("DocumentType", (int)SOBillingType);

                    defaultValue.Add("V_SOBilling", Value);

                    COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                    List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);
                    this._MainCOMFields = mainTableDefine;
                    this._DetailCOMFields = detailTableDefines;

                    frm = (frmBrowser)base.GetForm(right, mdiForm, Filter);
                    frm.DefaultValue = defaultValue;
                }
                else
                {
                    this.MainTableName = "V_SOBilling_C";
                    SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                    SortedList<string, object> Value = new SortedList<string, object>();
                    Value.Add("DocumentType", (int)SOBillingType);

                    defaultValue.Add("V_SOBilling_C", Value);

                    COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                    List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);

                    detailTableDefines[2]["SONumber"].Visible = COMField.Enum_Visible.NotVisible;
                    detailTableDefines[2]["QuantityBalance"].Visible = COMField.Enum_Visible.NotVisible;
                    this._MainCOMFields = mainTableDefine;
                    this._DetailCOMFields = detailTableDefines;

                     frm = (frmBrowser)base.GetForm(right, mdiForm);
                    frm.DefaultValue = defaultValue;
                }
             
                return frm;
            }
            catch (Exception ex)
            {
                Msg.Warning(string.Format("销售发票加载失败！%n {0}", ex.Message));
            }
            return null;
        }

    }
}
