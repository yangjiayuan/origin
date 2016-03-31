//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using Infragistics.Win.UltraWinGrid;
//using Base;
//using UI;
//using System.Windows.Forms;

//namespace OH
//{
//    public enum enuPayInvoice : int { StandPurchase = 1,  ConsignPurchase = 4 }

//    public class clsPayInvoice:ToolDetailForm
//    {
//        private enuPayInvoice PayInvoiceType;

//        public override bool AllowInsertRowInGrid(string TableName)
//        {
//            if (TableName == "D_PayInvoiceCleaning")
//                return true;
//            else
//                return false;
//        }
//        public override bool AllowDeleteRowInToolBar
//        {
//            get
//            {
//                return true;
//            }
//        }
//        public override bool AllowUpdateInGrid(string TableName)
//        {
//                return true;
//        }

//        public override bool AllowInsertRowInToolBar
//        {
//            get
//            {
//                return true;
//            }
//        }
//        public override bool AllowCheck
//        {
//            get
//            {
//                return false;
//            }
//        }

//        public override bool AutoCode
//        {
//            get
//            {
//                return false;
//            }
//        }
//        public override void NewGrid(Base.COMFields fields, Infragistics.Win.UltraWinGrid.UltraGrid grid)
//        {

//             grid.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(grid_AfterCellUpdate);

//        }

//        //private bool deleting = false;
//        //void grid_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
//        //{
//        //    if (deleting)
//        //        return;
//        //    deleting = true;
//        //    e.DisplayPromptMsg = true;
//        //    if (Msg.Question("确定要取消当前的入库单?") != System.Windows.Forms.DialogResult.Yes)
//        //        e.Cancel = true;
//        //    else
//        //    {
//        //        SortedList<Guid,Guid> listWarehouseIn=new SortedList<Guid,Guid>();
//        //        foreach (UltraGridRow row in e.Rows)
//        //            if (!listWarehouseIn.ContainsKey((Guid)row.Cells["WarehouseInID"].Value))
//        //                listWarehouseIn.Add((Guid)row.Cells["WarehouseInID"].Value, (Guid)row.Cells["WarehouseInID"].Value);
//        //        for (int i = _GridMap["V_PayWarehouseIn"].Rows.Count - 1; i >= 0; i--)
//        //        {
//        //            UltraGridRow row = _GridMap["V_PayWarehouseIn"].Rows[i];
//        //            if (listWarehouseIn.ContainsKey((Guid)row.Cells["WarehouseInID"].Value))
//        //                row.Delete(false);
//        //        }
//        //        e.Cancel = true;
//        //        for (int i = _DetailForm.MainDataSet.Tables["D_PayWarehouseIn"].Rows.Count - 1; i >= 0; i--)
//        //        {
//        //            DataRow row = _DetailForm.MainDataSet.Tables["D_PayWarehouseIn"].Rows[i];
//        //            if (row.RowState!= DataRowState.Deleted && listWarehouseIn.ContainsKey((Guid)row["WarehouseInID"]))
//        //                row.Delete();
//        //        }
//        //    }
//        //    deleting = false;
//        //}

//        void grid_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
//        {
//            UltraGrid grid = (UltraGrid)sender;
//            string key = e.Cell.Column.Key;
//            switch (key)
//            {
//                case "InvoiceAmount":
//                    UltraGrid grid2 = (UltraGrid)sender;

//                    //如果需要在订单的表头级别记录下订单的总金额，则需要此功能来自动汇总计算.
//                    decimal AmountTotal = 0;
//                    foreach (UltraGridRow row in grid2.Rows)
//                        if (row.Cells["InvoiceAmount"].Value != DBNull.Value)
//                            AmountTotal += (decimal)row.Cells["InvoiceAmount"].Value;
//                    _ControlMap["Amount"].Text = AmountTotal.ToString();

//                    break;

//            }
//        }

//        //public override string GetPrefix()
//        //{
//        //    return "CGFP";
//        //}

//        public override System.Data.DataSet InsertRowsInGrid(List<Base.COMFields> detailTableDefine)
//        {
//            //选择采购订单

//            if (this._DetailForm.MainDataSet.Tables["D_PayInvoice"].Rows[0]["Vendor"] == DBNull.Value)
//            {
//                Msg.Information("请先选择发票的供应商!");
//                return null;
//            }
//            Guid VendorID = (Guid)this._DetailForm.MainDataSet.Tables["D_PayInvoice"].Rows[0]["Vendor"];
//            getPurchaseOrder(null,VendorID);

//            return this._DetailForm.MainDataSet;
//        }


//        private void getPurchaseOrder(String PONumber,Guid VendorID)
//        {
//            //frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_PurchaseOrder"), null, string.Format("D_PurchaseOrder.CheckStatus=1 and D_PurchaseOrder.InvoiceStatus<2 and D_PurchaseOrder.Vendor='{0}'", VendorID), enuShowStatus.None);
          
//            //DataRow dr = frm.ShowSelect("Code", PONumber, null);
//            //if (dr != null)
//            //{
//            //    DataRow newDR = this._DetailForm.MainDataSet.Tables["D_PayInvoicePO"].NewRow();
//            //    newDR["POID"] = dr["ID"];
//            //    newDR["PONumber"] = dr["Code"];
//            //    newDR["Amount"] = dr["Amount"];
//            //    newDR["InvoiceAmount"] = dr["Amount"];
//            //    this._DetailForm.MainDataSet.Tables["D_PayInvoicePO"].Rows.Add(newDR);
//            //}

//            frmBrowser frm;
//            if (PayInvoiceType  == enuPayInvoice.StandPurchase)
//                frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_PurchaseOrder"), null, string.Format("D_PurchaseOrder.CheckStatus=1 and D_PurchaseOrder.InvoiceStatus<2 and D_PurchaseOrder.Vendor='{0}'", VendorID), enuShowStatus.None);
//            else
//                frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_StorageIn"), null, string.Format("D_StorageIn.CheckStatus=1 and D_StorageIn.DocumentType={1} and D_StorageIn.Vendor='{0}'", VendorID, (int)PayInvoiceType), enuShowStatus.None);


//            DataRow dr = frm.ShowSelect("Code", PONumber, null);
//            if (dr != null)
//            {
//                DataRow newDR = this._DetailForm.MainDataSet.Tables["D_PayInvoicePO"].NewRow();

//                if (PayInvoiceType == enuPayInvoice.StandPurchase)
//                {
//                    newDR["POID"] = dr["ID"];
//                    newDR["PONumber"] = dr["Code"];
//                }
//                else
//                {
//                    newDR["StorageIn"] = dr["ID"];
//                    newDR["StorageInNumber"] = dr["Code"];
//                }
//                newDR["Amount"] = dr["Amount"];
//                newDR["InAmount"] = dr["Amount"];
//                newDR["InvoiceAmount"] = dr["Amount"];
//                this._DetailForm.MainDataSet.Tables["D_PayInvoicePO"].Rows.Add(newDR);
//            }
//        }

//        public override string BeforeSaving(DataSet ds, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran)
//        {
//            try
//            {

//                decimal total = 0;
//                decimal Tax = 0;
//                decimal NetAmount;

//                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_PayInvoicePO"].Rows)
//                    if (dr.RowState != DataRowState.Deleted)
//                        total += (decimal)dr["InvoiceAmount"];
//                _ControlMap["Amount"].Text = total.ToString();

//                Tax = (decimal)this._DetailForm.MainDataSet.Tables["D_PayInvoice"].Rows[0]["Tax"];
//                NetAmount = (decimal)this._DetailForm.MainDataSet.Tables["D_PayInvoice"].Rows[0]["Money"];

//                if ((NetAmount + Tax) != total)
//                    return "发票金额扣除税额后与税前金额不相等，请重新输入！";
//                else
//                    return "";
//            }
//            catch (Exception ex)
//            {
//                return ex.Message;
//            }
//        }

//        public override DataSet DeleteRowsInGrid(List<COMFields> DetailTableDefine)
//        {
//            if (_DetailForm.ActiveControl.GetType() == typeof(UltraGrid))
//            {
//                UltraGrid grid = _DetailForm.ActiveControl as UltraGrid;
//                grid.PerformAction(UltraGridAction.DeleteRows);
//                string TableName = null;
//                foreach (string n in _DetailForm.GridMap.Keys)
//                    if (_DetailForm.GridMap[n] == grid)
//                        TableName = n;
//                if (TableName == "D_PayInvoicePO")
//                { 
//                    this.UpdataTotalAmount();
//                }
//            }
//            return _DetailForm.MainDataSet;

//        }

//        public void UpdataTotalAmount()
//        {
//            try
//            {
//                decimal Total = 0;

//                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_PayInvoicePO"].Rows)
//                    if (dr.RowState != DataRowState.Deleted)
//                        Total += (decimal)dr["InvoiceAmount"];
//                _ControlMap["Amount"].Text = Total.ToString();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        public override Form GetForm(CRightItem right, Form mdiForm)
//        {
//            try
//            {

//                string Filter;


//                if (right.Paramters == null)
//                    return null;
//                string[] s = right.Paramters.Split(new char[] { ',' });
//                if (s.Length > 0)
//                    PayInvoiceType = (enuPayInvoice)int.Parse(s[0]);
//                else
//                    return null;
//                this.MainTableName = "D_PayInvoice";
//                Filter = string.Format("D_PayInvoice.DocumentType={0}", (int)PayInvoiceType);

//                SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
//                SortedList<string, object> Value = new SortedList<string, object>();
//                Value.Add("InvoiceDate", CSystem.Sys.Svr.SystemTime.Date);
//                Value.Add("DocumentType", (int)PayInvoiceType);

//                defaultValue.Add("D_PayInvoice", Value);

//                COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
//                List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);

//                switch (PayInvoiceType)
//                {
//                    case enuPayInvoice.StandPurchase:
//                        mainTableDefine.Property.Title = string.Format("[采购发票]");
//                        detailTableDefines[0]["StorageInNumber"].Visible = COMField.Enum_Visible.NotVisible;
//                        detailTableDefines[0]["InAmount"].Visible = COMField.Enum_Visible.NotVisible;
//                        detailTableDefines[0]["InAmount"].Mandatory = false;
//                        break;
//                    case enuPayInvoice.ConsignPurchase:
//                        mainTableDefine.Property.Title = string.Format("[采购发票-寄售]");
//                        detailTableDefines[0]["PONumber"].Visible = COMField.Enum_Visible.NotVisible;
//                        detailTableDefines[0]["Amount"].Visible = COMField.Enum_Visible.NotVisible;
//                        detailTableDefines[0]["Amount"].Mandatory = false;
//                        break;
//                    default:

//                        break;

//                }


//                this._MainCOMFields = mainTableDefine;
//                this._DetailCOMFields = detailTableDefines;

//                frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm, Filter);


//                frm.DefaultValue = defaultValue;
//                return frm;
//            }
//            catch (Exception ex)
//            {
//                Msg.Warning(string.Format("采购发票加载失败！%n {0}", ex.Message));
//            }
//            return null;
//        }

//        public override bool Check(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
//        {
//            return true;
//        }

//        public override bool UnCheck(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
//        {
//            return true;
//        }
//    }
//}
