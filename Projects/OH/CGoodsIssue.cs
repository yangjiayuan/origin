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
    public class CGoodsIssue : ToolDetailForm
    {
        private enuDocumentOut DocumentOutType;
        private ToolStripButton toolGIConfirm;
 
        private Form _mdiForm;

        public CGoodsIssue()
        {

        }
        public override bool AllowInsertRowInGrid(string TableName)
        {
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
        public override bool AllowNew
        {
            get
            {
                return false;
            }
        }

        public override bool AllowInsertRowInToolBar
        {
            get
            {
                return false;
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
            //grid.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(grid_AfterCellUpdate);
        }

        public override void InsertToolStrip(ToolStrip toolStrip, ToolDetailForm.enuInsertToolStripType insertType)
        {
            base.InsertToolStrip(toolStrip, insertType);
            int i = toolStrip.Items.Count;

            i = i - 10;
            toolGIConfirm = new ToolStripButton();
            toolGIConfirm.Font = new System.Drawing.Font("SimSun", 10.5F);
            toolGIConfirm.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolGIConfirm.Name = "toolPreview";
            toolGIConfirm.Size = new System.Drawing.Size(39, 34);
            toolGIConfirm.Text = "发货确认";
            toolGIConfirm.Image = UI.clsResources.Finish;
            toolGIConfirm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolGIConfirm.Click += new EventHandler(toolGIConfirm_Click);
            toolStrip.Items.Insert(i, toolGIConfirm);
             
        }

        private void toolGIConfirm_Click(object sender, EventArgs e)
        {
            string tableName = "D_StorageOut";
            List<COMFields> detailTable = CSystem.Sys.Svr.Properties.DetailTableDefineList(tableName);
            COMFields mainTable = CSystem.Sys.Svr.LDI.GetFields(tableName);
            string where =  string.Format("D_StorageOut.CheckStatus=1 and D_StorageOut.ID Not In (Select Document from D_StorageData)");

            frmBrowser frm = new frmBrowser(mainTable, detailTable, where, enuShowStatus.None);
            frm.AllowDoubleClick = false;
            DataRow mainRow = frm.ShowSelect("Code",null, null);
            if (mainRow != null)
            {
                Guid StorageOutID = (Guid)mainRow["ID"];
                DocumentOutType = (enuDocumentOut)mainRow["DocumentType"];

                try
                {
                    Boolean Result;

                    switch (DocumentOutType)
                    {
                        case enuDocumentOut.SalesOut:
                            Result = UpdateStorageData(StorageOutID, enuStorageType.Normal, true, mainRow);
                            break;
                        case enuDocumentOut.CustomerConsignOut:
                            Result = UpdateStorageData(StorageOutID, enuStorageType.Normal, true, mainRow);
                            if (Result == true)
                                Result = UpdateStorageData(StorageOutID, enuStorageType.CustomerConsign, false, mainRow);
                            break;
                        case enuDocumentOut.VendorConsignOut:
                            Result = UpdateStorageData(StorageOutID, enuStorageType.VendorConsign, true, mainRow);
                            break;
                        case enuDocumentOut.CustomerConsignSales:
                            Result = UpdateStorageData(StorageOutID, enuStorageType.CustomerConsign, true, mainRow);
                            break;
                        default:
                            Result = UpdateStorageData(StorageOutID, enuStorageType.Normal, true, mainRow);
                            break;
                    }
                   
                }
                catch (Exception ex)
                {
                    Msg.Error(string.Format("物流发货确认出错！具体的错误信息如下：%n {0}", ex.Message));
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
                                    drOutNew[dc.ColumnName] = mainRow["CustomerStorageLocation"];
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

        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            try
            { 
                string Filter;

                _mdiForm = mdiForm;
                this.MainTableName = "D_StorageData";
                Filter = string.Format("D_StorageData.DocumentType in (101,102,103,104)");

                COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);
                this._MainCOMFields = mainTableDefine;
                this._DetailCOMFields = detailTableDefines;

                frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm, Filter);
                return frm;
            }
            catch (Exception ex)
            {
                Msg.Warning(string.Format("物流发货程序加载失败！%n {0}", ex.Message));
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
            }
            return _DetailForm.MainDataSet;
        }
         
    }
}
