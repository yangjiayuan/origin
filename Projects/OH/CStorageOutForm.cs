using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Infragistics.Win.UltraWinGrid;
using Base;
using UI;
using System.Windows.Forms;
using System.Collections;

namespace OH
{
    public enum enuDocumentOut : int { SalesOut=101,VendorConsignOut=102,CustomerConsignOut=103,CustomerConsignSales=104,TransferOut=105 }
 
    
    public class CStorageOutForm : ToolDetailForm
    {
        private enuDocumentOut DocumentOutType;
        private enuStorageType StorageType;

        private ToolStripButton toolPrint;
        private ToolStripButton toolPreview;
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
                default:
                    break;
            }

        }

        void _CreateGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {
            CCreateGrid createGrid = (CCreateGrid)sender;
            switch (e.Field.FieldName)
            {
                case "MaterialName":
                case "MaterialCode":
                     createGrid.Grid.ActiveRow.Cells["Package"].Value = createGrid.Grid.ActiveRow.Cells["MPackage"].Value;
                     createGrid.Grid.ActiveRow.Cells["PackageName"].Value = createGrid.Grid.ActiveRow.Cells["MPackageName"].Value;
                     createGrid.Grid.ActiveRow.Cells["Measure"].Value = createGrid.Grid.ActiveRow.Cells["MUNIT"].Value;
                     createGrid.Grid.ActiveRow.Cells["MeasureName"].Value = createGrid.Grid.ActiveRow.Cells["MUNITName"].Value;
                     break;
                case "BatchNo":
                     decimal QuantityBalance = StorageQuantityBalance(e.GridRow);
                     createGrid.Grid.ActiveRow.Cells["QuantityBalance"].Value = QuantityBalance;
                     break;
                default:
                     break;
            
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

                    Price = 0;
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
                    ////计算单价
                    //decimal Quantity = 0;
                    //if (e.Cell.Row.Cells["Quantity"].Value != DBNull.Value)
                    //    Quantity = (decimal)e.Cell.Row.Cells["Quantity"].Value;
                    //if (Quantity != 0)
                    //    e.Cell.Row.Cells["Price"].Value = Math.Round((decimal)e.Cell.Value / Quantity, 4);

                    break;
            }

        }

        public override string GetPrefix()
        {
            return "CK";
        }

        public override System.Data.DataSet InsertRowsInGrid(List<Base.COMFields> detailTableDefine)
        {
            //选择采购订单

            if (this._DetailForm.MainDataSet.Tables["D_StorageOut"].Rows[0]["Customer"] == DBNull.Value)
            {
                Msg.Information("请先选择出库单的客户！");
                return null;
            }

            if (this._DetailForm.MainDataSet.Tables["D_StorageOut"].Rows[0]["StorageLocation"] == DBNull.Value)
            {
                Msg.Information("请先选择出库单的库存地点！");
                return null;
            }

            Guid CustomerID = (Guid)this._DetailForm.MainDataSet.Tables["D_StorageOut"].Rows[0]["Customer"];
            getSalesOrder(null, CustomerID);

            return this._DetailForm.MainDataSet;
        }


        private void getSalesOrder(String SONumber, Guid CustomerID)
        {
            string FilterCondition;
            switch (DocumentOutType)
            {
                case enuDocumentOut.SalesOut:
                    FilterCondition = string.Format("D_SalesOrder.CheckStatus=1 and D_SalesOrder.OrderType=0 and D_SalesOrder.Customer='{0}'", CustomerID);
                    break;
                case enuDocumentOut.CustomerConsignOut:
                    FilterCondition = string.Format("D_SalesOrder.CheckStatus=1 and D_SalesOrder.OrderType=1 and D_SalesOrder.Customer='{0}'", CustomerID);
                    break;
                case enuDocumentOut.VendorConsignOut:
                    FilterCondition = string.Format("D_SalesOrder.CheckStatus=1 and D_SalesOrder.Customer='{0}'", CustomerID);
                    break;
                default:
                    FilterCondition = string.Format("D_SalesOrder.CheckStatus=1 and D_SalesOrder.Customer='{0}'", CustomerID);
                    break;

            }
            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_SalesOrder"), null,FilterCondition, enuShowStatus.None);

            DataRow dr = frm.ShowSelect("Code", SONumber, null);
            if (dr != null)
            {

                COMFields FieldsDetail = CSystem.Sys.Svr.LDI.GetFields("D_SalesOrderItem");

                string SQLDetail = FieldsDetail.QuerySQLWithClause(" MainID = '" + dr["ID"] + "'");

                DataSet dsPOItems = CSystem.Sys.Svr.cntMain.Select(SQLDetail, "D_SalesOrderItem");

                foreach (DataRow drItems in dsPOItems.Tables["D_SalesOrderItem"].Rows)
                {
                    DataRow newDR = this._DetailForm.MainDataSet.Tables["D_StorageOutDetail"].NewRow();
                    newDR["SalesOrder"] = dr["ID"];
                    newDR["SONumber"] = dr["Code"];
                    newDR["Material"] = drItems["Material"];
                    newDR["MaterialCode"] = drItems["MaterialCode"];
                    newDR["MaterialName"] = drItems["MaterialName"];
                    newDR["Measure"] = drItems["Measure"];
                    newDR["MeasureName"] = drItems["MeasureName"];
                    newDR["Package"] = drItems["Package"];
                    newDR["PackageName"] = drItems["PackageName"];
                    newDR["Quantity"] = drItems["Quantity"];
                    newDR["Price"] = drItems["Price"];
                    newDR["Amount"] = drItems["Amount"];
                    this._DetailForm.MainDataSet.Tables["D_StorageOutDetail"].Rows.Add(newDR);
                }

            }
        }

        public override string BeforeSaving(DataSet ds, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran)
        {
            try
            {
                string returnmessage;

                returnmessage = "";

                 switch (DocumentOutType)
                {
                     case enuDocumentOut.CustomerConsignSales:
                        if (this._DetailForm.MainDataSet.Tables["D_StorageOut"].Rows[0]["StorageLocation"] == DBNull.Value)
                            this._DetailForm.MainDataSet.Tables["D_StorageOut"].Rows[0]["StorageLocation"] = this._DetailForm.MainDataSet.Tables["D_StorageOut"].Rows[0]["CustomerStorageLocation"];
                        break;
                     default:
                        break;
                 }

                 returnmessage = this.CheckQuantity();
 
                decimal total = 0;
                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_StorageOutDetail"].Rows)
                    if (dr.RowState != DataRowState.Deleted)
                        total += (decimal)dr["Amount"];
                _ControlMap["Amount"].Text = total.ToString();

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
                this.MainTableName = "D_StorageOut";
                Filter = string.Format("D_StorageOut.DocumentType={0}", (int)DocumentOutType);

                SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                SortedList<string, object> Value = new SortedList<string, object>();
                Value.Add("DocumentDate", CSystem.Sys.Svr.SystemTime.Date);
                Value.Add("DocumentType", (int)DocumentOutType);

                defaultValue.Add("D_StorageOut", Value);

                COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);

                switch (DocumentOutType)
                {
                    case enuDocumentOut.SalesOut:
                        mainTableDefine.Property.Title = string.Format("[销售出库单]");
                        StorageType = enuStorageType.Normal;
                        //_AllowInsertRowInGrid = false;
                        _AllowInsertRowInGrid = true;
                        _AllowInsertRowInToolBar = true;
                        break;
                    case enuDocumentOut.CustomerConsignOut:
                        mainTableDefine.Property.Title = string.Format("[客户寄售出库单]");
                        StorageType = enuStorageType.Normal;
                         //_AllowInsertRowInGrid = false;
                        _AllowInsertRowInGrid = true;
                        _AllowInsertRowInToolBar = true;               
                        break;
                    case enuDocumentOut.VendorConsignOut:
                        mainTableDefine.Property.Title = string.Format("[供应商寄售出库单]");
                        StorageType = enuStorageType.VendorConsign;
                        //_AllowInsertRowInGrid = false;
                        _AllowInsertRowInGrid = true;
                        _AllowInsertRowInToolBar = true;
                        break;
                    case enuDocumentOut.CustomerConsignSales:
                        mainTableDefine.Property.Title = string.Format("[客户寄售领用]");
                        mainTableDefine["StorageLocationName"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["StorageLocationContact"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["StorageLocationAddress"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["StorageLocationTel"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["CompanyName"].Visible = COMField.Enum_Visible.NotVisible;
                        detailTableDefines[0]["SONumber"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["CustomerStorageLocationName"].Visible = COMField.Enum_Visible.VisibleInDetail;
                        detailTableDefines[0]["MaterialCode"].Enable = true;
                        detailTableDefines[0]["MaterialName"].Enable = true;
                        detailTableDefines[0]["MeasureName"].Enable = true;
                        detailTableDefines[0]["PackageName"].Enable = true;
                        StorageType = enuStorageType.CustomerConsign;
                        _AllowInsertRowInGrid = true;
                        _AllowInsertRowInToolBar = false;
                        break;
                    default:
                        StorageType = enuStorageType.Normal;
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
                Msg.Warning(string.Format("出库单据加载失败！%n {0}", ex.Message));
            }
            return null;


        }

        public decimal StorageQuantityBalance(UltraGridRow ActiveGridRow)
        {
            try 
            {
                decimal QuantityBalance;
                Guid StorageLocation ;
                int StorageType;

                if (this._DetailForm.MainDataSet.Tables["D_StorageOut"].Rows[0]["Customer"] == DBNull.Value)
                {
                    Msg.Information("请选择出库单的所对应的客户名称后再继续操作！");
                    return 0;
                }

                if (ActiveGridRow.Cells["Material"].Value == DBNull.Value)
                {
                    Msg.Information("请选择出库单的所对应的物料名称后再继续操作！");
                    return 0;

                }

                   switch (DocumentOutType)
                {
                       case enuDocumentOut.CustomerConsignSales:
                        StorageType = (int)enuStorageType.CustomerConsign;
                        StorageLocation = (Guid)this._DetailForm.MainDataSet.Tables["D_StorageOut"].Rows[0]["CustomerStorageLocation"];
                        break;
                    default:
                        StorageLocation = (Guid)this._DetailForm.MainDataSet.Tables["D_StorageOut"].Rows[0]["StorageLocation"];
                        StorageType = (int)enuStorageType.Normal;
                        break;

                }

               
                Guid Material = (Guid)ActiveGridRow.Cells["Material"].Value;
                Guid Package = (Guid)ActiveGridRow.Cells["Package"].Value;
                Guid Batch = (Guid)ActiveGridRow.Cells["Batch"].Value;
                Guid Measure = (Guid)ActiveGridRow.Cells["Measure"].Value;

                DictionaryEntry ParamStorageType = new DictionaryEntry("StorageType", StorageType);
                DictionaryEntry ParamStorageLocation = new DictionaryEntry("StorageLocation", StorageLocation);
                DictionaryEntry ParamMaterial = new DictionaryEntry("MaterialID", Material);
                DictionaryEntry ParamPackage = new DictionaryEntry("PackageID", Package);
                DictionaryEntry ParamBatch = new DictionaryEntry("BatchID", Batch);
                DictionaryEntry ParamMeasure= new DictionaryEntry("MeasureID", Measure);

                DataSet dsBalance = CSystem.Sys.Svr.cntMain.StoreProcedure("StorageBalanceEx", "Balance",ParamStorageType,ParamStorageLocation,ParamMaterial,ParamPackage,ParamBatch,ParamMeasure);
                if (dsBalance.Tables["Balance"].Rows.Count > 0)
                {
                    DataRow DRBalance = dsBalance.Tables["Balance"].Rows[0];
                    QuantityBalance = (decimal)DRBalance["QuantityBalance"];
                }
                else
                {
                    QuantityBalance = 0;
                }

                return QuantityBalance;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public string CheckQuantity()
        {
            try
            {
                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_StorageOutDetail"].Rows)
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        int LineNumber = (int)dr["LineNumber"];
                        string MaterialName = (string)dr["MaterialName"];
                        decimal Quantity = (decimal)dr["Quantity"];
                        decimal QuantityBalance = Convert.ToDecimal(dr["QuantityBalance"]);
                        if (Quantity > QuantityBalance)
                            return string.Format("第{2}行物料[{0}]的出库数量大于当前库存的可用数量，库存数量是{1}", MaterialName, QuantityBalance, LineNumber);
                    }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public override void AfterDataBind()
        {
            UltraGrid grid = _GridMap["D_StorageOutDetail"];
            DataTable dt = _DetailForm.MainDataSet.Tables["D_StorageOutDetail"];
          
            foreach (UltraGridRow row in grid.Rows)
            {
               if (row.Cells["Batch"].Value != DBNull.Value )
                {
                    decimal QuantityBalance = StorageQuantityBalance(row);
                    row.Cells["QuantityBalance"].Value = QuantityBalance;
                }
 
            }
        }


        private bool UpdateStorageData(Guid StorageOutDocument, enuStorageType TargetStorageType, Boolean Negate, DataRow mainRow)
        {
            DataSet dsOutDetail = CSystem.Sys.Svr.cntMain.Select("Select * from D_StorageOutDetail where MainID='" + StorageOutDocument + "'", "D_StorageOutDetail");
            if (dsOutDetail.Tables["D_StorageOutDetail"].Rows.Count > 0)
            {
                DataSet dsOutNew = CSystem.Sys.Svr.cntMain.Select("Select * from D_StorageData where 1=0", "D_StorageData");
                foreach (DataRow drStorageOut in dsOutDetail.Tables["D_StorageOutDetail"].Rows)
                {
                    DataRow drOutNew = dsOutNew.Tables["D_StorageData"].NewRow();
                    foreach (DataColumn dc in dsOutNew.Tables["D_StorageData"].Columns)
                    {
                        switch (dc.ColumnName)
                        {
                            case "ID":
                                break;
                            case "StorageLocation":
                                if ((DocumentOutType == enuDocumentOut.CustomerConsignOut) && (TargetStorageType == enuStorageType.CustomerConsign))
                                    drOutNew[dc.ColumnName] = mainRow["Customer"];
                                else
                                    drOutNew[dc.ColumnName] = mainRow[dc.ColumnName];
                                break;
                            case "DocumentDate":
                            case "DocumentType":
                                drOutNew[dc.ColumnName] = mainRow[dc.ColumnName];
                                break;
                            case "StorageType":
                                drOutNew[dc.ColumnName] = TargetStorageType;
                                break;
                            case "Document":
                                drOutNew[dc.ColumnName] = StorageOutDocument;
                                break;
                            case "Quantity":
                            case "Amount":
                                decimal Original = (decimal)drStorageOut[dc.ColumnName];
                                if (Negate == true)
                                    drOutNew[dc.ColumnName] = 0 - Original;
                                else
                                    drOutNew[dc.ColumnName] = Original;
                                break;

                            default:
                                if (dsOutDetail.Tables["D_StorageOutDetail"].Columns.Contains(dc.ColumnName))
                                {
                                    drOutNew[dc.ColumnName] = drStorageOut[dc.ColumnName];
                                }
                                break;
                        }
                    }
                    dsOutNew.Tables["D_StorageData"].Rows.Add(drOutNew);
                }
                CSystem.Sys.Svr.cntMain.Update(dsOutNew.Tables["D_StorageData"]);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Check(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
        {

            try
            {
                Boolean Result;

                switch (DocumentOutType)
                {
                    case enuDocumentOut.CustomerConsignSales:
                        Result = UpdateStorageData(ID, enuStorageType.CustomerConsign, true, mainRow);
                        break;
                    default:
                        Result = true;
                        break;
                }
                return Result;
            }
            catch (Exception ex)
            {
                Msg.Error(string.Format("出库单复核出错！具体的错误信息如下：%n {0}", ex.Message));
                return false;
            }

        }
        public override bool UnCheck(Guid ID, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction sqlTran, DataRow mainRow)
        {
            try
            {
                Guid StorageOutDocument;

                StorageOutDocument = ID;

                string SQL = string.Format("Delete From D_StorageData Where DocumentType >10 And Document= '{0}'", StorageOutDocument);
                CSystem.Sys.Svr.cntMain.Excute(string.Format(SQL));
                return true;

            }
            catch (Exception ex)
            {
                Msg.Error(string.Format("出库单反复核出错！具体的错误信息如下：%n {0}", ex.Message));
                return false;
            }
        }
    }
}
