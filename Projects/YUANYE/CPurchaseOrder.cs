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
    public class CPurchaseOrder : ToolDetailForm
    {
        private int PurchaseOrderType;

        public override bool AutoCode
        {
            get
            {
                return false;
            }
        }
        public override string GetPrefix()
        {
            return "NTYY";
        }

        public override bool AllowCheck
        {
            get
            {
                return true;
            }
        }
        public override void NewControl(COMField f, System.Windows.Forms.Control ctl)
        {

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
            try
            {
                e.Row.Cells["DelvDate"].Value = ((UltraDateTimeEditor)_ControlMap["DelvDate"]).DateTime;
            }
            catch { }
        }

        void grid_AfterCellUpdate(object sender, CellEventArgs e)
        {   
            decimal Quantity = 0;
            decimal Price = 0;
            //decimal Amount = 0;
            switch (e.Cell.Column.Key)
            {
               
                case "Price":
                    if (e.Cell.Row.Cells["Quantity"].Value != DBNull.Value)
                    {
                        Quantity = (decimal)e.Cell.Row.Cells["Quantity"].Value;
                        Price = (decimal)e.Cell.Value;
                        e.Cell.Row.Cells["Amount"].Value = Math.Round(Price * Quantity, 2);

                    }
                    break;

                case "Quantity":
                    
                    if ((e.Cell.Row.Cells["Price"].Value != DBNull.Value) && (e.Cell.Value != DBNull.Value))
                    {
                        Price  = (decimal)e.Cell.Row.Cells["Price"].Value;
                        e.Cell.Row.Cells["Amount"].Value = Math.Round(Price * (decimal)e.Cell.Value, 2);
                    }
                    break;

                case "Amount":
                    UltraGrid grid2 = (UltraGrid)sender;

                    //如果需要在订单的表头级别记录下订单的总金额，则需要此功能来自动汇总计算.
                    decimal AmountTotal = 0;
                    foreach (UltraGridRow row in grid2.Rows)
                        if (row.Cells["Amount"].Value != DBNull.Value)
                            AmountTotal += (decimal)row.Cells["Amount"].Value;
                    _ControlMap["Amount"].Text = AmountTotal.ToString();

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
            string FilterCondition;

            FilterCondition = string.Format("D_SalesOrder.CheckStatus=1 and D_SalesOrder.OrderType={0} ", PurchaseOrderType);

            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_SalesOrder"), null, FilterCondition, enuShowStatus.None);

            DataRow dr = frm.ShowSelect("Code", null, null);

            if (dr != null)
            {
                DataRow newMainDR = ds.Tables["D_PurchaseOrder"].Rows[0];
                newMainDR["Code"] = dr["Code"];
                newMainDR["SalesOrder"] = dr["ID"];
                newMainDR["OrderType"] = PurchaseOrderType;
                newMainDR["SOCode"] = dr["Code"];
                newMainDR["CustomerName"] = dr["CustomerName"];
                newMainDR["Company"] = dr["Company"];
                newMainDR["CompanyName"] = dr["CompanyName"];
                newMainDR["DelvDate"] = dr["DelvDate"];
                newMainDR["CPONumber"] = dr["CPONumber"];

                if (PurchaseOrderType == 1)
                {
                    newMainDR["Buyer"] = dr["SalesPerson"];
                    newMainDR["BuyerName"] = dr["SalesPersonName"];
                    newMainDR["PaymentType"] = dr["PaymentType"];
                    newMainDR["PaymentTypeName"] = dr["PaymentTypeName"];
                    newMainDR["Currency"] = dr["Currency"];
                    newMainDR["CurrencyName"] = dr["CurrencyName"];
                    newMainDR["Amount"] = dr["Amount"];
                }


                COMFields FieldsDetail = CSystem.Sys.Svr.LDI.GetFields("D_SalesOrderItem");
                string SQLDetail = FieldsDetail.QuerySQLWithClause(" MainID = '" + dr["ID"] + "'");
                DataSet dsSalesOrderItems = CSystem.Sys.Svr.cntMain.Select(SQLDetail, "D_SalesOrderItem");

                foreach (DataRow drItems in dsSalesOrderItems.Tables["D_SalesOrderItem"].Rows)
                {
                    DataRow newDR = ds.Tables["D_PurchaseOrderItem"].NewRow();

                    newDR["LineNumber"] = drItems["LineNumber"];
                    newDR["Style"] = drItems["Style"];
                    newDR["Colour"] = drItems["Colour"];
                    newDR["Material"] = drItems["Material"];
                    newDR["MaterialName"] = drItems["MaterialName"];
                    newDR["MaterialEnglishName"] = drItems["MaterialEnglishName"];
                    newDR["Measure"] = drItems["Measure"];
                    newDR["MeasureName"] = drItems["MeasureName"];
                    newDR["Quantity"] = drItems["Quantity"];
                    newDR["DelvDate"] = drItems["DelvDate"];
                    if (PurchaseOrderType == 1)
                    {
                        newDR["Price"] = drItems["Price"];
                        newDR["Amount"] = drItems["Amount"];
                    }
                    ds.Tables["D_PurchaseOrderItem"].Rows.Add(newDR);
                }


                return true;
            }
            else
            {
                return false;
            }

        }


        public override string BeforeSaving(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            string s = null;

            return s;
        }
        public override void InsertToolStrip(System.Windows.Forms.ToolStrip toolStrip, ToolDetailForm.enuInsertToolStripType insertType)
        {
            //base.InsertToolStrip(toolStrip, insertType);
            //int i = toolStrip.Items.Count;
            //if (insertType == enuInsertToolStripType.Detail)
            //{
            //    ToolStripButton toolFinish = new ToolStripButton();
            //    toolFinish.Font = new System.Drawing.Font("SimSun", 10.5F);
            //    toolFinish.ImageTransparentColor = System.Drawing.Color.Magenta;
            //    toolFinish.Name = "toolSplit";

            //    toolFinish.Text = "完成";
            //    toolFinish.Image = clsResources.Finish;
            //    toolFinish.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            //    toolFinish.Click += new EventHandler(toolFinish_Click);

            //    toolStrip.Items.Insert(i - 2, toolFinish);
            //}
        }
        public override bool UnCheck(Guid ID, SqlConnection conn, SqlTransaction sqlTran, DataRow mainRow)
        {
            return base.UnCheck(ID, conn, sqlTran, mainRow);
        }

        void toolFinish_Click(object sender, EventArgs e)
        {
        }
        public override Form GetForm(CRightItem right, Form mdiForm)
        {

            string Filter;

            if (right.Paramters == null)
                return null;
            string[] s = right.Paramters.Split(new char[] { ',' });
            if (s.Length > 0)
                PurchaseOrderType = int.Parse(s[0]);
            else
                return null;
            this.MainTableName = "D_PurchaseOrder";
            Filter = string.Format("D_PurchaseOrder.OrderType={0} ", (int)PurchaseOrderType);

            frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm,Filter);
            SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
            SortedList<string, object> Value = new SortedList<string, object>();
            Value.Add("DocumentDate", CSystem.Sys.Svr.SystemTime.Date);
            Value.Add("DelvDate", CSystem.Sys.Svr.SystemTime.Date);
            defaultValue.Add("D_PurchaseOrder", Value);
            frm.DefaultValue = defaultValue;
            return frm;
        }

        private void toolAddNewItem_Click(object sender, EventArgs e)
        {
            //选择采购订单

            if (this._DetailForm.MainDataSet.Tables["D_StorageOut"].Rows[0]["Customer"] == DBNull.Value)
            {
                Msg.Information("请首先在出货通知单中选择客户名称后再进行此操作！");

            }
            else
            {
                Guid CustomerID = (Guid)this._DetailForm.MainDataSet.Tables["D_StorageOut"].Rows[0]["Customer"];
                getSalesOrder(null, CustomerID);
            }

            //return this._DetailForm.MainDataSet;      
        }

        private void getSalesOrder(String SONumber, Guid CustomerID)
        {
            string FilterCondition;


            FilterCondition = string.Format("D_SalesOrder.CheckStatus=1 and D_SalesOrder.Shipped=0 and D_SalesOrder.Customer='{0}'", SONumber);


            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_SalesOrder"), null, FilterCondition, enuShowStatus.None);

            DataRow dr = frm.ShowSelect("Code", SONumber, null);

            if (dr != null)
            {

                COMFields FieldsDetail = CSystem.Sys.Svr.LDI.GetFields("D_SalesOrderItem");

                string SQLDetail = FieldsDetail.QuerySQLWithClause(" MainID = '" + dr["ID"] + "'");

                DataSet dsShippingItems = CSystem.Sys.Svr.cntMain.Select(SQLDetail, "D_SalesOrderItem");

                foreach (DataRow drItems in dsShippingItems.Tables["D_SalesOrderItem"].Rows)
                {
                    DataRow newDR = this._DetailForm.MainDataSet.Tables["D_StorageOutDetail"].NewRow();
                    newDR["SalesOrder"] = dr["ID"];
                    newDR["SONumber"] = dr["Code"];
                    newDR["Material"] = drItems["Material"];
                    newDR["MaterialName"] = drItems["MaterialName"];
                    newDR["Measure"] = drItems["Measure"];
                    newDR["MeasureName"] = drItems["MeasureName"];
                    newDR["Quantity"] = drItems["Quantity"];
                    newDR["Price"] = drItems["Price"];
                    newDR["Amount"] = drItems["Amount"];
                    this._DetailForm.MainDataSet.Tables["D_StorageOutDetail"].Rows.Add(newDR);
                }

            }
        }


    }
}
