using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Infragistics.Win.UltraWinGrid;
using Base;
using UI;
using System.Windows.Forms;
using Infragistics.Win.Printing;
using System.Collections;

namespace OH
{
    public enum enuPurchaseOrder : int { Normal = 0, VendorConsign = 1}
    public enum enuDocumentIn : int {Initialization=0, PurchaseIn = 1, ConsignIn = 2, TransferIn = 3, ConsignPurchase =4}
    public enum enuStorageType : int { Normal = 1, VendorConsign = 2, CustomerConsign = 3};

    public class CStorageInForm : ToolDetailForm
    {
        private enuDocumentIn DocumentInType;
        private enuStorageType StorageType;
        private enuPurchaseOrder POType;


        private ToolStripButton toolPrint;
        private ToolStripButton toolPreview;

        private bool _AllowInsertRowInGrid;
        private bool _AllowInsertRowInToolBar;

        private Form _mdiForm;

        public CStorageInForm()
        {

        }
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
                return true;
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
            Guid StorageInDocID;
            DataSet DS;

            if (this._BrowserForm.grid.ActiveRow == null)
                return;

            if (_BrowserForm.grid.Selected.Rows.Count == 0 && _BrowserForm.grid.ActiveRow != null)
                _BrowserForm.grid.ActiveRow.Selected = true;

            UltraGridRow row = _BrowserForm.grid.Selected.Rows[0];
            StorageInDocID = (Guid)row.Cells["ID"].Value;     
            DS = GetData(StorageInDocID);
            FrmPrintViewer PV = new FrmPrintViewer("入库通知单", AppDomain.CurrentDomain.BaseDirectory + "PrintTemplate\\PrintStorageIn.rdlc", DS);
            PV.Text = "入库通知单";
            PV.MdiParent = _mdiForm;
            PV.Show();
        }



        //private void Print(bool IsPreview)
        //{
        //    if (this._BrowserForm.grid.ActiveRow == null)
        //        return;

        //    frmPrintSetting ps = new frmPrintSetting(AppDomain.CurrentDomain.BaseDirectory + "PrintTemplate\\StorageIn");
        //    if (ps.ShowDialog() == DialogResult.OK)
        //    {
        //        string tempFile = ps.TempateFile;
        //        clsPrintTemplate Template = new clsPrintTemplate(tempFile);

        //        if (_BrowserForm.grid.Selected.Rows.Count == 0 && _BrowserForm.grid.ActiveRow != null)
        //            _BrowserForm.grid.ActiveRow.Selected = true;

        //        int index = 1;
        //        int count = _BrowserForm.grid.Selected.Rows.Count;
        //        foreach (UltraGridRow row in _BrowserForm.grid.Selected.Rows)
        //        {
        //            Guid id = (Guid)row.Cells["ID"].Value;

        //            DataSet ds = GetData(id);
        //            clsPrintDocument doc = new clsPrintDocument(Template, ds, ps.PrintSetting);
        //            if (IsPreview)
        //            {
        //                if (count > 1 && Msg.Question(string.Format("将要预览第{0}领料单，共{1}张领料单，是否继续？", index, count)) != DialogResult.Yes)
        //                    return;
        //                index++;

        //                UltraPrintPreviewDialog pre = new UltraPrintPreviewDialog();
        //                pre.Document = doc;
        //                pre.ShowDialog();
        //            }
        //            else
        //                doc.Print();
        //        }
        //    }
        //}

        private DataSet GetData(Guid MainID)
        {
            //DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from V_StorageIn Where ID='{0}'", id), "V_StorageIn");

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
            string[] s = e.Field.ValueType.Split(':');
            CCreateGrid CreatedGrid = (CCreateGrid)sender;
            UltraGridRow ActiveRow = CreatedGrid.Grid.ActiveCell.Row;
            Guid MaterialID = Guid.NewGuid();

            switch (s[1])
            {
                case "P_Batch":
                    if (ActiveRow != null)
                    {
                        if (ActiveRow.Cells["Material"].Value.ToString().Length > 0)
                        {
                            MaterialID = (Guid)ActiveRow.Cells["Material"].Value;
                            e.Where = string.Format("P_Batch.Material = '{0}'", MaterialID);
                        }
                    }
                    break;
                case "P_Package":
                    if (ActiveRow != null) 
                    {
                        if (ActiveRow.Cells["Material"].Value.ToString().Length > 0)
                        {
                            MaterialID = (Guid)ActiveRow.Cells["Material"].Value;
                            e.Where = string.Format("P_Package.ID in (Select Package from P_MaterialPackage where MainID ='{0}')", MaterialID);
                        }
                    }
                    break;
                default:
                    break;
            }
 
        }

        void _CreateGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {
            CCreateGrid createGrid = (CCreateGrid)sender;
            if ((e.Field.FieldName == "MaterialName") || (e.Field.FieldName == "MaterialCode"))
            {
                createGrid.Grid.ActiveRow.Cells["Package"].Value = createGrid.Grid.ActiveRow.Cells["MPackage"].Value;
                createGrid.Grid.ActiveRow.Cells["PackageName"].Value = createGrid.Grid.ActiveRow.Cells["MPackageName"].Value;
            }
        }
        //private bool deleting = false;
        //void grid_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        //{
        //    if (deleting)
        //        return;
        //    deleting = true;
        //    e.DisplayPromptMsg = true;
        //    if (Msg.Question("确定要取消当前的入库单?") != System.Windows.Forms.DialogResult.Yes)
        //        e.Cancel = true;
        //    else
        //    {
        //        SortedList<Guid,Guid> listWarehouseIn=new SortedList<Guid,Guid>();
        //        foreach (UltraGridRow row in e.Rows)
        //            if (!listWarehouseIn.ContainsKey((Guid)row.Cells["WarehouseInID"].Value))
        //                listWarehouseIn.Add((Guid)row.Cells["WarehouseInID"].Value, (Guid)row.Cells["WarehouseInID"].Value);
        //        for (int i = _GridMap["V_PayWarehouseIn"].Rows.Count - 1; i >= 0; i--)
        //        {
        //            UltraGridRow row = _GridMap["V_PayWarehouseIn"].Rows[i];
        //            if (listWarehouseIn.ContainsKey((Guid)row.Cells["WarehouseInID"].Value))
        //                row.Delete(false);
        //        }
        //        e.Cancel = true;
        //        for (int i = _DetailForm.MainDataSet.Tables["D_PayWarehouseIn"].Rows.Count - 1; i >= 0; i--)
        //        {
        //            DataRow row = _DetailForm.MainDataSet.Tables["D_PayWarehouseIn"].Rows[i];
        //            if (row.RowState!= DataRowState.Deleted && listWarehouseIn.ContainsKey((Guid)row["WarehouseInID"]))
        //                row.Delete();
        //        }
        //    }
        //    deleting = false;
        //}

        void grid_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            decimal Quantity = 0;
            decimal Price = 0;

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

        public override string GetPrefix()
        {
            return "CGRK";
        }

        public override System.Data.DataSet InsertRowsInGrid(List<Base.COMFields> detailTableDefine)
        {
            //选择采购订单

            if (this._DetailForm.MainDataSet.Tables["D_StorageIn"].Rows[0]["Vendor"] == DBNull.Value)
            {
                Msg.Information("请先选择入库商品的供应商!");
                return null;
            }
            Guid VendorID = (Guid)this._DetailForm.MainDataSet.Tables["D_StorageIn"].Rows[0]["Vendor"];
            getPurchaseOrder(null, VendorID);
            this.UpdataTotalAmount();
            return this._DetailForm.MainDataSet;
        }

        public override bool NewData(DataSet ds, COMFields mainTableDefine, List<COMFields> detailTableDefine)
        {
            string FilterCondition;

            if (DocumentInType != enuDocumentIn.ConsignPurchase)
                return true;

            FilterCondition = string.Format("V_VendorConsignStorageData.Quantity >0 ");

            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("V_VendorConsignStorageData"), null, FilterCondition, enuShowStatus.None);

            DataRow dr = frm.ShowSelect("Code", null, null);

            if (dr != null)
            {
                DataRow newMainDR = ds.Tables["D_StorageIn"].Rows[0];
                newMainDR["StorageLocation"] = dr["StorageLocation"];
                newMainDR["StorageLocationName"] = dr["StorageLocationName"];
                newMainDR["Vendor"] = dr["Vendor"];
                newMainDR["VendorName"] = dr["VendorName"];

                    DataRow newDR = ds.Tables["D_StorageInDetail"].NewRow();

                    newDR["LineNumber"] = 1;
                    newDR["Material"] = dr["Material"];
                    newDR["MaterialName"] = dr["MaterialName"];
                    newDR["Measure"] = dr["Measure"];
                    newDR["MeasureName"] = dr["MeasureName"];
                    newDR["Package"] = dr["Package"];
                    newDR["PackageName"] = dr["PackageName"];
                    newDR["Batch"] = dr["Batch"];
                    newDR["BatchNo"] = dr["BatchNo"];
                    newDR["Quantity"] = dr["Quantity"];
                    newDR["Price"] = (decimal) dr["Amount"] / (decimal) dr["Quantity"];
                    newDR["Amount"] = dr["Amount"];

                    ds.Tables["D_StorageInDetail"].Rows.Add(newDR);
             


                return true;
            }
            else
            {
                return false;
            }
           
        }





        private void getPurchaseOrder(String PONumber, Guid VendorID)
        {

            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_PurchaseOrder"), null, string.Format("D_PurchaseOrder.CheckStatus=1 and D_PurchaseOrder.Vendor='{0}' and D_PurchaseOrder.OrderType={1} ", VendorID, (int)POType), enuShowStatus.None);

            DataRow dr = frm.ShowSelect("Code", PONumber, null);
            if (dr != null)
            {

                //COMFields FieldsDetail = CSystem.Sys.Svr.LDI.GetFields("D_PurchaseOrderItem");

                //string SQLDetail = FieldsDetail.QuerySQLWithClause(" MainID = '" + dr["ID"] +"'");

                //DataSet dsPOItems =  CSystem.Sys.Svr.cntMain.Select(SQLDetail, "D_PurchaseOrderItem");
                Guid PurchaseOrderID = (Guid) dr["ID"];
 
                DictionaryEntry ParamPO = new DictionaryEntry("PurchaseOrderID",PurchaseOrderID);

                DataSet dsPOItems = CSystem.Sys.Svr.cntMain.StoreProcedure("POItemsBalance", "D_PurchaseOrderItem", ParamPO);
                int LineNumber = 0;
                foreach (DataRow drItems in dsPOItems.Tables["D_PurchaseOrderItem"].Rows)
                {
                    LineNumber += 1;
                    DataRow newDR = this._DetailForm.MainDataSet.Tables["D_StorageInDetail"].NewRow();
                    newDR["LineNumber"] = LineNumber;
                    newDR["PurchaseOrder"] = dr["ID"];
                    newDR["PONumber"] = dr["Code"];
                    newDR["Material"] = drItems["Material"];
                    newDR["MaterialName"] = drItems["MaterialName"];
                    newDR["Measure"] = drItems["Measure"];
                    newDR["MeasureName"] = drItems["MeasureName"];
                    newDR["Package"] = drItems["Package"];
                    newDR["PackageName"] = drItems["PackageName"];
                    newDR["Quantity"] = drItems["QuantityBalance"];
                    newDR["Price"] = drItems["Price"];
                    newDR["Amount"] = (decimal)drItems["Price"] * (decimal)drItems["QuantityBalance"];
                    this._DetailForm.MainDataSet.Tables["D_StorageInDetail"].Rows.Add(newDR);
                }

            }
        }

        public override string BeforeSaving(DataSet ds, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran)
        {

            string returnmessage;

            returnmessage = "";
            this.UpdataTotalAmount();
            switch (DocumentInType)
            {
                case enuDocumentIn.PurchaseIn:
                case enuDocumentIn.ConsignIn:
                    returnmessage = this.CheckQuantity();
                    break;
                default:
                    break;
            }


            return  returnmessage;
        }

        public override void AfterSaved(DataSet ds, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction tran)
        {
            base.AfterSaved(ds, conn, tran);
        }


        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            try
            { 
                string Filter;

                _mdiForm = mdiForm;

                if (right.Paramters==null)
                    return null;
                string[] s = right.Paramters.Split(new char[] { ',' });
                if (s.Length > 0)
                    DocumentInType  = (enuDocumentIn)int.Parse(s[0]);
                else
                    return null;
                this.MainTableName = "D_StorageIn";
                Filter = string.Format("D_StorageIn.DocumentType={0}",(int)DocumentInType);

                SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                SortedList<string, object> Value = new SortedList<string, object>();
                Value.Add("DocumentDate", CSystem.Sys.Svr.SystemTime.Date);
                Value.Add("DocumentType", (int)DocumentInType);
                
                defaultValue.Add("D_StorageIn", Value);

                COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);

                switch (DocumentInType)
                {
                    case enuDocumentIn.Initialization:
                        mainTableDefine["StorageType"].Enable = true;
                        mainTableDefine.Property.Title = string.Format("期初库存入库单");
                        detailTableDefines[0]["PONumber"].Visible = COMField.Enum_Visible.NotVisible;
                        detailTableDefines[0]["MaterialName"].Enable = true;
                        detailTableDefines[0]["PackageName"].Enable = true;
                        StorageType = enuStorageType.Normal;
                        _AllowInsertRowInGrid = true;
                        _AllowInsertRowInToolBar = false;
                        break;
                    case enuDocumentIn.PurchaseIn:
                        mainTableDefine.Property.Title = string.Format("采购入库单");
                        StorageType = enuStorageType.Normal;
                        POType = enuPurchaseOrder.Normal;
                        //_AllowInsertRowInGrid = false;
                        _AllowInsertRowInGrid = true;
                        _AllowInsertRowInToolBar = true;
                        break;
                    case enuDocumentIn.ConsignPurchase:
                        mainTableDefine.Property.Title = string.Format("供应商寄售领用单");
                        detailTableDefines[0]["PONumber"].Visible = COMField.Enum_Visible.NotVisible;
                        detailTableDefines[0]["MaterialName"].Enable = true;
                        //detailTableDefines[0]["MeasureName"].Enable = true;
                        detailTableDefines[0]["PackageName"].Enable = true;

                        StorageType = enuStorageType.Normal;
                        _AllowInsertRowInGrid = true ;
                        _AllowInsertRowInToolBar = false;
                        break;
                    case enuDocumentIn.ConsignIn:
                        mainTableDefine.Property.Title = string.Format("供应商寄售入库单");
                        StorageType = enuStorageType.VendorConsign;
                        POType = enuPurchaseOrder.VendorConsign;
                        //_AllowInsertRowInGrid = false;
                        _AllowInsertRowInGrid = true;
                        _AllowInsertRowInToolBar = true;
                        break;
                    default:
                        StorageType = enuStorageType.Normal;
                        POType = enuPurchaseOrder.Normal;
                        //_AllowInsertRowInGrid = false;
                        _AllowInsertRowInGrid = true;
                        _AllowInsertRowInToolBar = true;
                        break;

                }

                Value.Add("StorageType", (int)StorageType);
                this._MainCOMFields = mainTableDefine;
                this._DetailCOMFields = detailTableDefines;

                frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm, Filter);


                frm.DefaultValue = defaultValue;
                return frm;
            }
            catch (Exception ex)
            {
                Msg.Warning(string.Format("入库单据加载失败！%n {0}", ex.Message));
            }
            return null;

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
                if (TableName == "D_StorageInDetail")
                {
                    grid.PerformAction(UltraGridAction.DeleteRows);
                    this.UpdataTotalAmount();
                }
            }
            return _DetailForm.MainDataSet;
        }


        public override bool Check(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
        {
            try
            {
                Boolean Result;
                 int StorageType;

                switch (DocumentInType)
                {
                    case enuDocumentIn.Initialization:

                        StorageType = (int)mainRow["StorageType"];
                        Result = UpdateStorageData(ID, (enuStorageType) StorageType, false, conn, sqlTran, mainRow);
                        break;

                    default:
                        Result = true;
                        break;

                }
                return Result;

            }
            catch (Exception ex)
            {
                Msg.Error(string.Format("单据复核出错！具体的错误信息如下：%n {0}", ex.Message));
                return false;
            }

        }

        private bool UpdateStorageData(Guid StorageInDocument, enuStorageType TargetStorageType, Boolean Negate, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
        {

            DataSet dsInDetail = CSystem.Sys.Svr.cntMain.Select("Select * from D_StorageInDetail where MainID='" + StorageInDocument + "'", "D_StorageInDetail", conn, sqlTran);
            if (dsInDetail.Tables["D_StorageInDetail"].Rows.Count > 0)
            {
                DataSet dsInNew = CSystem.Sys.Svr.cntMain.Select("Select * from D_StorageData where 1=0", "D_StorageData", conn, sqlTran);
                foreach (DataRow drStorageIn in dsInDetail.Tables["D_StorageInDetail"].Rows)
                {

                    DataRow drInNew = dsInNew.Tables["D_StorageData"].NewRow();
                    foreach (DataColumn dc in dsInNew.Tables["D_StorageData"].Columns)
                    {
                        switch (dc.ColumnName)
                        {
                            case "ID":
                                break;
                            case "StorageLocation":
                            //if ((DocumentInType == enuDocumentIn.ConsignPurchase) && (TargetStorageType == enuStorageType.VendorConsign))
                            //    drInNew[dc.ColumnName] = mainRow["Vendor"];
                            //else
                            //    drInNew[dc.ColumnName] = mainRow[dc.ColumnName];
                            //break;
                            case "DocumentDate":
                            case "DocumentType":
                                drInNew[dc.ColumnName] = mainRow[dc.ColumnName];
                                break;
                            case "StorageType":
                                drInNew[dc.ColumnName] = TargetStorageType;
                                break;
                            case "Document":
                                drInNew[dc.ColumnName] = StorageInDocument;
                                break;
                            case "Quantity":
                            case "Amount":
                                decimal Original = (decimal)drStorageIn[dc.ColumnName];
                                if (Negate == true)
                                    drInNew[dc.ColumnName] = 0 - Original;
                                else
                                    drInNew[dc.ColumnName] = Original;
                                break;

                            default:
                                if (dsInDetail.Tables["D_StorageInDetail"].Columns.Contains(dc.ColumnName))
                                {
                                    drInNew[dc.ColumnName] = drStorageIn[dc.ColumnName];
                                }
                                break;
                        }
                    }
                    dsInNew.Tables["D_StorageData"].Rows.Add(drInNew);
                }
                CSystem.Sys.Svr.cntMain.Update(dsInNew.Tables["D_StorageData"], conn, sqlTran);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdataTotalAmount()
        {
            try
            {
                decimal Total = 0;


                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_StorageInDetail"].Rows)
                    if (dr.RowState != DataRowState.Deleted)
                        Total += (decimal)dr["Amount"];
                _ControlMap["Amount"].Text = Total.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CheckQuantity()
        {
            try
            {
                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_StorageInDetail"].Rows)
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        Guid PurchaseOrderID = (Guid)dr["PurchaseOrder"];
                        Guid StorageInID = (Guid)dr["MainID"];
                        DictionaryEntry ParamPO = new DictionaryEntry("PurchaseOrderID", PurchaseOrderID);
                        DictionaryEntry ParamSI = new DictionaryEntry("StorageInID", StorageInID);

                        int LineNumber = (int)dr["LineNumber"];
                        string MaterialName = (string)dr["MaterialName"];
                        decimal Quantity = (decimal)dr["Quantity"];
                        decimal QuantityBalance=0;

                        DataSet dsBalance = CSystem.Sys.Svr.cntMain.StoreProcedure("POItemsBalanceEx", "Balance", ParamPO,ParamSI);
                            foreach (DataRow DRBalance in dsBalance.Tables["Balance"].Rows)
                                if ((DRBalance["Material"].Equals(dr["Material"])) && (DRBalance["Package"].Equals(dr["Package"])))
                                {

                                    QuantityBalance  = QuantityBalance + (decimal) DRBalance["QuantityBalance"];
                                }

                            if (Quantity > QuantityBalance)
                                return string.Format("第{2}行物料[{0}]的入库数量大于当前采购订单的可用数量，采购订单的可用数量是{1}", MaterialName, QuantityBalance, LineNumber);
                            break;
                    }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public override bool UnCheck(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
        {
            try
            {
                Guid StorageInDocument;

                StorageInDocument = ID;

                string SQL = string.Format("Delete From D_StorageData Where DocumentType <10 And Document= '{0}'", StorageInDocument);
                CSystem.Sys.Svr.cntMain.Excute(string.Format(SQL));
                return true;

            }
            catch (Exception ex)
            {
                Msg.Error(string.Format("入库单反复核出错！具体的错误信息如下：%n {0}", ex.Message));
                return false;
            }
        }
    }
}
