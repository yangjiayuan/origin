using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Infragistics.Win.UltraWinGrid;
using Base;
using UI;
using System.Windows.Forms;
using System.Collections;

namespace YUANYE
{
    //public enum enuDocumentOut : int { SalesOut=101,VendorConsignOut=102,CustomerConsignOut=103,CustomerConsignSales=104,TransferOut=105 }
    //public enum enuStorageType : int { Normal = 1, VendorConsign = 2, CustomerConsign = 3 };

    public class clsPurchaseInvoice : ToolDetailForm
    {
        private enuDocumentOut DocumentOutType;
     

        private ToolStripButton toolPrint;
        private ToolStripButton toolPreview;
        private ToolStripButton toolAddNewItem;

        private Form _mdiForm;

        private bool _AllowInsertRowInGrid;
        private bool _AllowInsertRowInToolBar;

        public override bool AllowInsertRowInGrid(string TableName)
        {
            return _AllowInsertRowInGrid;
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
                return _AllowInsertRowInToolBar;
            }
        }
        public override bool AllowCheck
        {
            get
            {
                return true ;
            }
        }

        public override bool AutoCode
        {
            get
            {
                return true;
            }
        }
        public override void NewGrid(Base.COMFields fields, Infragistics.Win.UltraWinGrid.UltraGrid grid)
        {

            grid.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(grid_AfterCellUpdate);

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
            Guid StorageOutDocID;
            DataSet DS;

            if (this._BrowserForm.grid.ActiveRow == null)
                return;

            if (_BrowserForm.grid.Selected.Rows.Count == 0 && _BrowserForm.grid.ActiveRow != null)
                _BrowserForm.grid.ActiveRow.Selected = true;

            UltraGridRow row = _BrowserForm.grid.Selected.Rows[0];
            StorageOutDocID = (Guid)row.Cells["ID"].Value;
            DS = GetData(StorageOutDocID);
            FrmPrintViewer PV = new FrmPrintViewer("提货单", AppDomain.CurrentDomain.BaseDirectory + "PrintTemplate\\PrintStorageOut.rdlc", DS);
            PV.Text = "提货单";
            PV.MdiParent = _mdiForm;
            PV.Show();
        }

        private DataSet GetData(Guid MainID)
        {
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(this.MainCOMFields.QuerySQLWithClause(this.MainTableName + ".ID='" + MainID + "'"), this.MainTableName);
            CSystem.Sys.Svr.cntMain.Select(this.DetailCOMFields[0].QuerySQLWithClause(this.DetailCOMFields[0].GetTableName(false) + ".MainID='" + MainID + "'"), this.DetailCOMFields[0].GetTableName(false), ds);
            return ds;
        }

        public override void SetCreateGrid(CCreateGrid createGrid)
        {
            base.SetCreateGrid(createGrid);
            createGrid.AfterSelectForm += new AfterSelectFormEventHandler(_CreateGrid_AfterSelectForm);
            createGrid.BeforeSelectForm += new BeforeSelectFormEventHandler(_CreateGrid_BeforeSelectForm);
        }

        void _CreateGrid_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            //string[] s = e.Field.ValueType.Split(':');
            //CCreateGrid CreatedGrid = (CCreateGrid)sender;
            //UltraGridRow ActiveRow = CreatedGrid.Grid.ActiveCell.Row;
            //Guid MaterialID = Guid.NewGuid();

            //switch (s[1])
            //{
            //    case "P_Batch":
            //        if (ActiveRow != null)
            //        {
            //            if (ActiveRow.Cells["Material"].Value.ToString().Length > 0)
            //            {
            //                MaterialID = (Guid)ActiveRow.Cells["Material"].Value;
            //                e.Where = string.Format("P_Batch.Material = '{0}'", MaterialID);
            //            }
            //        }
            //        break;
            //    default:
            //        break;
            //}

        }

        void _CreateGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {
            //CCreateGrid createGrid = (CCreateGrid)sender;
            //switch (e.Field.FieldName)
            //{
            //    case "MaterialName":
            //    case "MaterialCode":
            //         createGrid.Grid.ActiveRow.Cells["Measure"].Value = createGrid.Grid.ActiveRow.Cells["MUNIT"].Value;
            //         createGrid.Grid.ActiveRow.Cells["MeasureName"].Value = createGrid.Grid.ActiveRow.Cells["MUNITName"].Value;
            //         break;
            //    default:
            //         break;
            
            //}


        }


        void grid_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
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
                        Price = (decimal)e.Cell.Row.Cells["Price"].Value;
                        e.Cell.Row.Cells["Amount"].Value = Math.Round(Price * (decimal)e.Cell.Value, 2);
                    }
                    break;

                //case "Amount":
                //    UltraGrid grid2 = (UltraGrid)sender;

                //    //如果需要在订单的表头级别记录下订单的总金额，则需要此功能来自动汇总计算.
                //    decimal AmountTotal = 0;
                //    foreach (UltraGridRow row in grid2.Rows)
                //        if (row.Cells["Amount"].Value != DBNull.Value)
                //            AmountTotal += (decimal)row.Cells["Amount"].Value;
                //    _ControlMap["Amount"].Text = AmountTotal.ToString();

                //    break;
            }

        }
        public override string AutoCodeFormat
        {
            get
            {
                return "yyyy";
            }
        }

        public override string GetPrefix()
        {
            return "INV";
        }



        //private void toolAddNewItem_Click(object sender, EventArgs e)
        //{
        //    //选择采购订单

        //    if (this._DetailForm.MainDataSet.Tables["D_StorageOut"].Rows[0]["Customer"] == DBNull.Value)
        //    {
        //        Msg.Information("请首先在出货通知单中选择客户名称后再进行此操作！");

        //    }
        //    else
        //    {
        //        Guid CustomerID = (Guid)this._DetailForm.MainDataSet.Tables["D_StorageOut"].Rows[0]["Customer"];
        //        getSalesOrder(null, CustomerID);
        //    }

        //    //return this._DetailForm.MainDataSet;      
        //}

        //private void getSalesOrder(String SONumber, Guid CustomerID)
        //{
        //    string FilterCondition;


        //    FilterCondition = string.Format("D_SalesOrder.CheckStatus=1 and D_SalesOrder.Shipped=0 and D_SalesOrder.Customer='{0}'", CustomerID);
       
           
        //    frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_SalesOrder"), null,FilterCondition, enuShowStatus.None);

        //    DataRow dr = frm.ShowSelect("Code", SONumber, null);

        //    if (dr != null)
        //    {

        //        COMFields FieldsDetail = CSystem.Sys.Svr.LDI.GetFields("D_SalesOrderItem");

        //        string SQLDetail = FieldsDetail.QuerySQLWithClause(" MainID = '" + dr["ID"] + "'");

        //        DataSet dsShippingItems = CSystem.Sys.Svr.cntMain.Select(SQLDetail, "D_SalesOrderItem");

        //        foreach (DataRow drItems in dsShippingItems.Tables["D_SalesOrderItem"].Rows)
        //        {
        //            DataRow newDR = this._DetailForm.MainDataSet.Tables["D_StorageOutDetail"].NewRow();
        //            newDR["SalesOrder"] = dr["ID"];
        //            newDR["SONumber"] = dr["Code"];
        //            newDR["CPONumber"] = dr["CPONumber"];
        //            newDR["Style"] = drItems["Style"];
        //            newDR["Destination"] = drItems["Destination"];
        //            newDR["Material"] = drItems["Material"];
        //            newDR["MaterialName"] = drItems["MaterialName"];
        //            newDR["MaterialCode"] = drItems["MaterialCode"];
        //            newDR["EComposition"] = drItems["EComposition"];
        //            newDR["Color"] = drItems["Colour"];
        //            newDR["Measure"] = drItems["Measure"];
        //            newDR["MeasureName"] = drItems["MeasureName"];
        //            newDR["Quantity"] = drItems["Quantity"];
        //            newDR["Price"] = drItems["Price"];
        //            newDR["Amount"] = drItems["Amount"];
        //            this._DetailForm.MainDataSet.Tables["D_StorageOutDetail"].Rows.Add(newDR);
        //        }

        //    }
        //}

        public override string BeforeSaving(DataSet ds, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran)
        {
            try
            {
                string returnmessage;

                returnmessage = "";
 
                //returnmessage = this.CheckQuantity();
 
                decimal total = 0;
                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_StorageOutFactory"].Rows)
                    if (dr.RowState != DataRowState.Deleted)
                        total += (decimal)dr["Amount"];
                //_ControlMap["Amount"].Text = total.ToString();
                
                return returnmessage;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            try
            {

                string Filter;

                _mdiForm = mdiForm;

                if (right.Paramters == null)
                    return null;
                string[] s = right.Paramters.Split(new char[] { ',' });
                if (s.Length > 0)
                    DocumentOutType = (enuDocumentOut)int.Parse(s[0]);
                else
                    return null;
                this.MainTableName = "V_PurchaseInvoice";

                SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                SortedList<string, object> Value = new SortedList<string, object>();
                Value.Add("DocumentDate", CSystem.Sys.Svr.SystemTime.Date);
                defaultValue.Add("V_PurchaseInvoice", Value);

                COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);

                //Value.Add("StorageType", (int)StorageType);
                this._MainCOMFields = mainTableDefine;
                this._DetailCOMFields = detailTableDefines;
                detailTableDefines[0]["Price"].Visible = COMField.Enum_Visible.VisibleInDetail;
                detailTableDefines[0]["Amount"].Visible = COMField.Enum_Visible.VisibleInDetail;
                frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);

                frm.DefaultValue = defaultValue;
                return frm;
            }
            catch (Exception ex)
            {
                Msg.Warning(string.Format("采购发票加载失败！%n {0}", ex.Message));
            }
            return null;
        }

        
    }
}
