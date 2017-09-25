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

    public class CStorageOutActual : ToolDetailForm
    {
        private ToolStripButton toolPrint;
        private ToolStripButton toolPreview;
        private ToolStripButton toolInvoice;

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

        public override bool  AllowNew
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

        public override void InsertToolStrip(ToolStrip toolStrip, ToolDetailForm.enuInsertToolStripType insertType)
        {
            base.InsertToolStrip(toolStrip, insertType);
            int i = toolStrip.Items.Count;


            i = i - 3;
            toolInvoice = new ToolStripButton();
            toolInvoice.Font = new System.Drawing.Font("SimSun", 10.5F);
            toolInvoice.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolInvoice.Name = "toolAddNewItem";
            toolInvoice.Size = new System.Drawing.Size(39, 34);
            toolInvoice.Text = "开票";
            toolInvoice.Image = UI.clsResources.Query;
            toolInvoice.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolInvoice.Click += new EventHandler(toolInvoice_Click);
            toolStrip.Items.Insert(i, toolInvoice);


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
        //    Guid StorageOutDocID;
        //    DataSet DS;

        //    if (this._BrowserForm.grid.ActiveRow == null)
        //        return;

        //    if (_BrowserForm.grid.Selected.Rows.Count == 0 && _BrowserForm.grid.ActiveRow != null)
        //        _BrowserForm.grid.ActiveRow.Selected = true;

        //    UltraGridRow row = _BrowserForm.grid.Selected.Rows[0];
        //    StorageOutDocID = (Guid)row.Cells["ID"].Value;
        //    DS = GetData(StorageOutDocID);
        //    FrmPrintViewer PV = new FrmPrintViewer("提货单", AppDomain.CurrentDomain.BaseDirectory + "PrintTemplate\\PrintStorageOut.rdlc", DS);
        //    PV.Text = "提货单";
        //    PV.MdiParent = _mdiForm;
        //    PV.Show();
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
  

        }

        void _CreateGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {

        }


        void grid_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            decimal Quantity = 0;
            decimal Price = 0;

            switch (e.Cell.Column.Key)
            {
                case "Price":

                    break;

                case "Quantity":

                    break;

                case "Amount":

                    break;
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



        private void toolInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                Guid StorageOutDocID;


                if (this._BrowserForm.grid.ActiveRow == null)
                    return;

                if (_BrowserForm.grid.Selected.Rows.Count == 0 && _BrowserForm.grid.ActiveRow != null)
                    _BrowserForm.grid.ActiveRow.Selected = true;

                UltraGridRow row = _BrowserForm.grid.Selected.Rows[0];
                StorageOutDocID = (Guid)row.Cells["ID"].Value;

                string SQL = string.Format("Insert Into D_PurchaseInvoice (StorageOutID,Code,DocumentDate,Customer,IssueDate,Vendor,QuantityActual,PaymentTerm,ShippingType,Bill,POL,POD,DEST,Notes,Priceterms) Select StorageOutID,Code,DocumentDate,Customer,IssueDate,Vendor,QuantityActual,PaymentTerm,ShippingType,Bill,POL,POD,DEST,Notes,Priceterms from V_StorageOutByVendor Where storageoutID= '{0}'", StorageOutDocID);
                CSystem.Sys.Svr.cntMain.Excute(string.Format(SQL));

                SQL = string.Format("Insert Into D_PurchaseInvoiceItem (ID,MainID,LineNumber,SalesOrder,Vendor ,Style ,Material ,Color ,Quantity ,Measure ,QuantityActual ,Price ,Amount) Select ID,MainID,LineNumber,SalesOrder,Vendor ,Style ,Material ,Color ,Quantity ,Measure ,QuantityActual ,Price ,Amount From D_StorageOutFactory Where MainID= '{0}'", StorageOutDocID);
                CSystem.Sys.Svr.cntMain.Excute(string.Format(SQL));
                
                CSystem.Sys.Svr.cntMain.StoreProcedure("SP_D_PurchaseInvoiceItem","D_PurchaseInvoiceItem");
                
            }
            catch (Exception ex)
            {
                Msg.Error(string.Format("生产供应商开票资料出错！具体的错误信息如下：%n {0}", ex.Message));
            }
        }


        //public override string BeforeSaving(DataSet ds, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran)
        //{
            //try
            //{
            //    string returnmessage;

            //    returnmessage = "";
 
            //    decimal total = 0;
            //    foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_StorageOutFactory"].Rows)
            //        if (dr.RowState != DataRowState.Deleted)
            //            total += (decimal)dr["QuantityActual"];
            //    _ControlMap["QuantityActual"].Text = total.ToString();

            //    return returnmessage;
            //}
            //catch (Exception ex)
            //{
            //    return ex.Message;
            //}
        //}

        public override void AfterSaved(DataSet ds, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction tran)
        {
            //string StorageoutID = this.
            //string SQL= string.Format("Select StorageOutID,Code,DocumentDate,Customer,IssueDate,Vendor,QuantityActual,PaymentTerm,ShippingType,Bill,POL,POD,DEST,Notes,Priceterms from V_StorageOutByVendor Where storageoutID= '{0}'", ID);

            //DataTable dtPurchaseInvoice =new DataTable("D_PurchaseInvoice");
            //DataSet  dsStorageOutActual= CSystem.Sys.Svr.cntMain.Select("V_StorageOutByVendor")
            //dtPurchaseInvoice= ds.Tables["V_StorageOutByVendor"].Copy();
            //CSystem.Sys.Svr.cntMain.Update(dtPurchaseInvoice, conn, tran);
            base.AfterSaved(ds, conn, tran);
        }

        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            try
            {

                string Filter;

                _mdiForm = mdiForm;
                this.MainTableName = "V_StorageOutFactory";

                SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                SortedList<string, object> Value = new SortedList<string, object>();
                Value.Add("DocumentDate", CSystem.Sys.Svr.SystemTime.Date);
                defaultValue.Add("V_StorageOutFactory", Value);

                COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);

                this._MainCOMFields = mainTableDefine;
                this._DetailCOMFields = detailTableDefines;

                frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);

                frm.DefaultValue = defaultValue;
                return frm;
            }
            catch (Exception ex)
            {
                Msg.Warning(string.Format("实际装运单加载失败！%n {0}", ex.Message));
            }
            return null;
        }


        //public override bool Check(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
        //{
        //}


        //public override bool UnCheck(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
        //{
        //}
    
    
    }
}

