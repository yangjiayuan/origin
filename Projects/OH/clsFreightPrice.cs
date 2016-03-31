using System;
using System.Collections.Generic;
using System.Text;
using UI;
using System.Data;
using Base;

namespace OH
{
    class clsFreightPrice : ToolDetailForm
    {
        private Guid _CustomerID;
        private string _CustmerCode;
        private string _CustmerName;
        private decimal _FreightPrice;
        private decimal _WeightRate;
        frmBrowser frmSalesOrderBill = null;
        public Infragistics.Win.UltraWinGrid.UltraGrid grid;

        public override bool AutoCode
        {
            get
            {
                return true;
            }
            set
            {
                base.AutoCode = value;
            }
        }
        public override bool AllowInsertRowInGrid(string TableName)
        {
            if (TableName == "D_FreightPriceBill")
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
            if (TableName == "D_FreightPriceBill")
                return true;
            else
                return false;
        }
        public override bool AllowInsertRowInToolBar
        {
            get
            {
                return true;
            }
        }
        public override System.Windows.Forms.Form GetForm(Base.CRightItem right, System.Windows.Forms.Form mdiForm)
        {
            this.MainTableName = "D_FreightPrice";
            COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
            List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);               
            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("P_Customer"), null, null, enuShowStatus.None);
            DataRow dr = frm.ShowSelect(null, null, null);
            if (dr != null)
            {
                _CustomerID = (Guid)dr["ID"];
                _CustmerCode = dr["Code"] as string;
                _CustmerName = dr["Name"] as string;
                SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                SortedList<string, object> Value = new SortedList<string, object>();
                Value.Add("BillDate", CSystem.Sys.Svr.SystemTime.Date);

                Value.Add("CustomerID", _CustomerID);
                //dr["CustomerCode"] = _CustomerCode;
                Value.Add("CustomerName", _CustmerName);
                Value.Add("FreightPrice", dr["FreightPrice"]);
                Value.Add("WeightRate", dr["WeightRate"]);
                defaultValue.Add(this.MainTableName, Value);
                _FreightPrice = VarConverTo.ConvertToDecimal(dr["FreightPrice"]);
                _WeightRate = VarConverTo.ConvertToDecimal(dr["WeightRate"]);
                this._MainCOMFields = mainTableDefine;
                this._DetailCOMFields = detailTableDefines;
                frmSalesOrderBill = new frmBrowser(_MainCOMFields, _DetailCOMFields, "D_FreightPrice.deleted=0" + "", enuShowStatus.None);
                frmSalesOrderBill.DefaultValue = defaultValue;
                this.SetBrowerForm(frmSalesOrderBill);
                frmSalesOrderBill.toolDetailForm = this;
                grid = _BrowserForm.MainGrid;
                //_DetailForm = ;
                //this.SetDetailForm(frmSalesOrderBill);
                //this.setToolButtons();
            }
            return frmSalesOrderBill;
        }
        public override System.Data.DataSet InsertRowsInGrid(List<Base.COMFields> detailTableDefine)
        {
            DataSet ds = _DetailForm.MainDataSet;
            Guid strCustomerId = _CustomerID;
            frmBrowser frmCustomer = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_SalesOrder"), null, "D_SalesOrder.CustomerID='" + strCustomerId + "' and InvoiceStatus=0 ", enuShowStatus.None);
            DataRow[] drs = frmCustomer.ShowSelectRows(null, null, null);
            //frm.ShowDialog();
            if (drs != null)
            {
                foreach (DataRow dr in drs)
                {
                    if (dr.RowState == DataRowState.Deleted)
                        continue;
                    DataRow[] drs_V = ds.Tables["D_FreightPriceBill"].Select(string.Format("SalesOrderID='{0}'", dr["ID"]));
                    for (int i = 0; i < drs_V.Length; i++)
                        drs_V[i].Delete();
                    DataRow newDrV = ds.Tables["D_FreightPriceBill"].NewRow();
                    newDrV["SalesOrderID"] = dr["ID"];
                    newDrV["SalesOrderCode"] = dr["Code"];
                    newDrV["ConsignDate"] = dr["ConsignDate"];
                    newDrV["ContractCode"] = dr["ContractCode"];
                    newDrV["Weight"] = dr["Weight"];
                    ds.Tables["D_FreightPriceBill"].Rows.Add(newDrV);
                }
            }
            //合计出库单金额
            decimal total = 0;
            foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_FreightPriceBill"].Rows)
                if (dr.RowState != DataRowState.Deleted)
                    total += (decimal)dr["Weight"];
            _ControlMap["TotalWeight"].Text = total.ToString();
            _ControlMap["TotalFreightPrice"].Text = string.Format("{0:#,##0.00}",total * _FreightPrice * _WeightRate);
            _ControlMap["NowFreightPrice"].Text = string.Format("{0:#,##0.00}", total * _FreightPrice * _WeightRate);
            return ds;
        }
    }
}
