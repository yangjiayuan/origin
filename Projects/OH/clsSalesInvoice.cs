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
    public enum enuSalesInvoice : int { SalesOut = 101, VendorConsignOut =102, CustomerConsignSales = 104}
    public class clsSalesInvoice : ToolDetailForm
    {
        private enuSalesInvoice SalesInvoiceType;

        public override bool AllowInsertRowInGrid(string TableName)
        {
            if (TableName == "D_SalesInvoiceCleaning")
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
        public override void NewGrid(Base.COMFields fields, Infragistics.Win.UltraWinGrid.UltraGrid grid)
        {

            grid.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(grid_AfterCellUpdate);

        }

     
        //void grid_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        //{
        //}

        void grid_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            UltraGrid grid = (UltraGrid)sender;
            string key = e.Cell.Column.Key;
            switch (key)
            {
                case "InvoiceAmount":
                    UltraGrid grid2 = (UltraGrid)sender;

                    //如果需要在订单的表头级别记录下订单的总金额，则需要此功能来自动汇总计算.
                    decimal AmountTotal = 0;
                    foreach (UltraGridRow row in grid2.Rows)
                        if (row.Cells["InvoiceAmount"].Value != DBNull.Value)
                            AmountTotal += (decimal)row.Cells["InvoiceAmount"].Value;
                    _ControlMap["Amount"].Text = AmountTotal.ToString();

                    break;
     
            }
        }

        //public override string GetPrefix()
        //{
        //    return "";
        //}

        public override System.Data.DataSet InsertRowsInGrid(List<Base.COMFields> detailTableDefine)
        {
            //选择销售订单 或者 寄售领用单

            if (this._DetailForm.MainDataSet.Tables["D_SalesInvoice"].Rows[0]["Customer"] == DBNull.Value)
            {
                Msg.Information("请先选择开票的客户!");
                return null;
            }

            if (this._DetailForm.MainDataSet.Tables["D_SalesInvoice"].Rows[0]["Company"] == DBNull.Value)
            {
                Msg.Information("请先选择开票的公司名称!");
                return null;
            }

            Guid CustomerID = (Guid)this._DetailForm.MainDataSet.Tables["D_SalesInvoice"].Rows[0]["Customer"];
            Guid CompanyID = (Guid)this._DetailForm.MainDataSet.Tables["D_SalesInvoice"].Rows[0]["Company"];
            getStorageOutDocument(null, CustomerID,CompanyID);
            this.UpdataTotalAmount();
            return this._DetailForm.MainDataSet;
        }


        //private void getSalesOrder(String SONumber, Guid CustomerID,Guid CompanyID)
        //{
        //    frmBrowser frm ;
        //    if (SalesInvoiceType == enuSalesInvoice.SalesOut)
        //        frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_SalesOrder"), null, string.Format("D_SalesOrder.CheckStatus=1 and D_SalesOrder.InvoiceStatus<2 and D_SalesOrder.Customer='{0}' and D_SalesOrder.Company='{1}'", CustomerID,CompanyID), enuShowStatus.None);
        //    else
        //        frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_StorageOut"), null, string.Format("D_StorageOut.CheckStatus=1 and D_StorageOut.DocumentType={1} and D_StorageOut.Customer='{0}'", CustomerID, (int)SalesInvoiceType), enuShowStatus.None);
          
            
        //    DataRow dr = frm.ShowSelect("Code", SONumber, null);
        //    if (dr != null)
        //    {

        //        if (SalesInvoiceType == enuSalesInvoice.SalesOut)
        //        {

                   

        //            COMFields FieldsDetail = CSystem.Sys.Svr.LDI.GetFields("D_SalesOrderItem");
        //            string SQLDetail = FieldsDetail.QuerySQLWithClause(" MainID = '" + dr["ID"] + "'");
        //            DataSet dsSalesOrderItems = CSystem.Sys.Svr.cntMain.Select(SQLDetail, "D_SalesOrderItem");

        //            foreach (DataRow drItems in dsSalesOrderItems.Tables["D_SalesOrderItem"].Rows)
        //            { 
        //                DataRow newDR = this._DetailForm.MainDataSet.Tables["D_SalesInvoiceSO"].NewRow();
        //                newDR["SOID"] = dr["ID"];
        //                newDR["SONumber"] = dr["Code"];
        //                newDR["Material"] = drItems["Material"];
        //                newDR["MaterialCode"] = drItems["MaterialCode"];
        //                newDR["MaterialName"] = drItems["MaterialName"];
        //                newDR["Measure"] = drItems["Measure"];
        //                newDR["MeasureName"] = drItems["MeasureName"];
        //                newDR["Package"] = drItems["Package"];
        //                newDR["PackageName"] = drItems["PackageName"];
        //                newDR["Quantity"] = drItems["Quantity"];
        //                newDR["Price"] = drItems["Price"];
        //                newDR["Amount"] = drItems["Amount"];
        //                newDR["InvoiceAmount"] = dr["Amount"];

        //                this._DetailForm.MainDataSet.Tables["D_SalesInvoiceSO"].Rows.Add(newDR);
        //            }
        //        }
        //        else
        //        {
        //            DataRow newDR = this._DetailForm.MainDataSet.Tables["D_SalesInvoiceSO"].NewRow();

        //            COMFields FieldsDetail = CSystem.Sys.Svr.LDI.GetFields("D_StorageOutDetail");
        //            string SQLDetail = FieldsDetail.QuerySQLWithClause(" MainID = '" + dr["ID"] + "'");
        //            DataSet dsSalesOrderItems = CSystem.Sys.Svr.cntMain.Select(SQLDetail, "D_StorageOutDetail");

        //            foreach (DataRow drItems in dsSalesOrderItems.Tables["D_StorageOutDetail"].Rows)
        //            {
        //                newDR["StorageOut"] = dr["ID"];
        //                newDR["StorageOutNumber"] = dr["Code"];
        //                newDR["Material"] = drItems["Material"];
        //                newDR["MaterialCode"] = drItems["MaterialCode"];
        //                newDR["MaterialName"] = drItems["MaterialName"];
        //                newDR["Measure"] = drItems["Measure"];
        //                newDR["MeasureName"] = drItems["MeasureName"];
        //                newDR["Package"] = drItems["Package"];
        //                newDR["PackageName"] = drItems["PackageName"];
        //                newDR["Quantity"] = drItems["Quantity"];
        //                newDR["Price"] = drItems["Price"];
        //                newDR["OutAmount"] = dr["Amount"];
        //                newDR["InvoiceAmount"] = dr["Amount"];

        //                this._DetailForm.MainDataSet.Tables["D_SalesInvoiceSO"].Rows.Add(newDR);
        //            }
        //        }  
        //    }
        //}

        private void getStorageOutDocument (String SONumber, Guid CustomerID, Guid CompanyID)
        {
            frmBrowser frm;
            frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_StorageOut"), null, string.Format("D_StorageOut.CheckStatus=1 and D_StorageOut.DocumentType={1} and D_StorageOut.Customer='{0}'", CustomerID, (int)SalesInvoiceType), enuShowStatus.None);


            DataRow dr = frm.ShowSelect("Code", SONumber, null);
            if (dr != null)
            {

                    DataRow newDR = this._DetailForm.MainDataSet.Tables["D_SalesInvoiceSO"].NewRow();

                    COMFields FieldsDetail = CSystem.Sys.Svr.LDI.GetFields("D_StorageOutDetail");
                    string SQLDetail = FieldsDetail.QuerySQLWithClause(" MainID = '" + dr["ID"] + "'");
                    DataSet dsSalesOrderItems = CSystem.Sys.Svr.cntMain.Select(SQLDetail, "D_StorageOutDetail");

                    foreach (DataRow drItems in dsSalesOrderItems.Tables["D_StorageOutDetail"].Rows)
                    {
                        newDR["StorageOut"] = dr["ID"];
                        newDR["StorageOutNumber"] = dr["Code"];
                        newDR["Material"] = drItems["Material"];
                        newDR["MaterialCode"] = drItems["MaterialCode"];
                        newDR["MaterialName"] = drItems["MaterialName"];
                        newDR["Measure"] = drItems["Measure"];
                        newDR["MeasureName"] = drItems["MeasureName"];
                        newDR["Package"] = drItems["Package"];
                        newDR["PackageName"] = drItems["PackageName"];
                        newDR["Quantity"] = drItems["Quantity"];
                        newDR["Price"] = drItems["Price"];
                        newDR["OutAmount"] = dr["Amount"];
                        newDR["InvoiceAmount"] = dr["Amount"];

                        this._DetailForm.MainDataSet.Tables["D_SalesInvoiceSO"].Rows.Add(newDR);
                    }
            }
        }

        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            try
            {

                string Filter;


                if (right.Paramters == null)
                    return null;
                string[] s = right.Paramters.Split(new char[] { ',' });
                if (s.Length > 0)
                    SalesInvoiceType = (enuSalesInvoice)int.Parse(s[0]);
                else
                    return null;
                this.MainTableName = "D_SalesInvoice";
                Filter = string.Format("D_SalesInvoice.DocumentType={0}", (int)SalesInvoiceType);

                SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                SortedList<string, object> Value = new SortedList<string, object>();
                Value.Add("InvoiceDate", CSystem.Sys.Svr.SystemTime.Date);
                Value.Add("DocumentType", (int)SalesInvoiceType);

                defaultValue.Add("D_SalesInvoice", Value);

                COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);

                //switch (SalesInvoiceType)
                //{
                //    case enuSalesInvoice.SalesOut:
                //        mainTableDefine.Property.Title = string.Format("[销售发票]");
                //        detailTableDefines[0]["StorageOutNumber"].Visible = COMField.Enum_Visible.NotVisible;
                //        detailTableDefines[0]["OutAmount"].Visible = COMField.Enum_Visible.NotVisible;
                //        detailTableDefines[0]["OutAmount"].Mandatory = false;
                //        break;
                //    case enuSalesInvoice.CustomerConsignSales:
                //        mainTableDefine.Property.Title = string.Format("[销售发票-寄售]");
                //        detailTableDefines[0]["SONumber"].Visible = COMField.Enum_Visible.NotVisible;
                //        detailTableDefines[0]["Amount"].Visible = COMField.Enum_Visible.NotVisible;
                //        detailTableDefines[0]["Amount"].Mandatory = false;
                //        break;
                //    default:
 
                //        break;

                //}
                mainTableDefine.Property.Title = string.Format("[销售发票]");
                detailTableDefines[0]["SONumber"].Visible = COMField.Enum_Visible.NotVisible;
                detailTableDefines[0]["Amount"].Visible = COMField.Enum_Visible.NotVisible;
                detailTableDefines[0]["Amount"].Mandatory = false;
 
                this._MainCOMFields = mainTableDefine;
                this._DetailCOMFields = detailTableDefines;

                frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm, Filter);


                frm.DefaultValue = defaultValue;
                return frm;
            }
            catch (Exception ex)
            {
                Msg.Warning(string.Format("销售发票加载失败！%n {0}", ex.Message));
            }
            return null;
        }
        public override string BeforeSaving(DataSet ds, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran)
        {

            try
            {

                decimal total = 0;
                decimal Tax = 0;
                decimal NetAmount;

                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_SalesInvoiceSO"].Rows)
                    if (dr.RowState != DataRowState.Deleted)
                        total += (decimal)dr["InvoiceAmount"];
                _ControlMap["Amount"].Text = total.ToString();

                //Tax = (decimal)this._DetailForm.MainDataSet.Tables["D_SalesInvoice"].Rows[0]["Tax"];
                //NetAmount = (decimal)this._DetailForm.MainDataSet.Tables["D_SalesInvoice"].Rows[0]["Money"];

                //if ((NetAmount + Tax) != total)
                //    return "发票金额扣除税额后与税前金额不相等，请重新输入！";
                //else
                //    return "";
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void UpdataTotalAmount()
        {
            try
            {
                decimal Total = 0;


                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_SalesInvoiceSO"].Rows)
                    if (dr.RowState != DataRowState.Deleted)
                        Total += (decimal)dr["InvoiceAmount"];
                _ControlMap["Amount"].Text = Total.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public override DataSet DeleteRowsInGrid(List<COMFields> DetailTableDefine)
        {
            if (_DetailForm.ActiveControl.GetType() == typeof(UltraGrid))
            {
                UltraGrid grid = _DetailForm.ActiveControl as UltraGrid;
                string TableName = null;
                foreach (string n in _DetailForm.GridMap.Keys)
                    if (_DetailForm.GridMap[n] == grid)
                        TableName = n;
                if (TableName == "D_SalesInvoiceSO")
                {
                    grid.PerformAction(UltraGridAction.DeleteRows);
                    this.UpdataTotalAmount();
                }
            }
            return _DetailForm.MainDataSet;
        }

        //public override bool Check(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
        //{
        //    return true;
        //}

        //public override bool UnCheck(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
        //{
        //    return true;
        //}
    }
}
