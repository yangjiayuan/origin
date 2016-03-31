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
    public class CGoodsReceive : ToolDetailForm
    {
        private enuDocumentIn DocumentInType;
        private ToolStripButton toolGRConfirm;
 
        private Form _mdiForm;

        public CGoodsReceive()
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
            toolGRConfirm = new ToolStripButton();
            toolGRConfirm.Font = new System.Drawing.Font("SimSun", 10.5F);
            toolGRConfirm.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolGRConfirm.Name = "toolPreview";
            toolGRConfirm.Size = new System.Drawing.Size(39, 34);
            toolGRConfirm.Text = "收货确认";
            toolGRConfirm.Image = UI.clsResources.Finish;
            toolGRConfirm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolGRConfirm.Click += new EventHandler(toolGRConfirm_Click);
            toolStrip.Items.Insert(i, toolGRConfirm);
             
        }

        private void toolGRConfirm_Click(object sender, EventArgs e)
        {
            string tableName= "D_StorageIn";
            List<COMFields> detailTable = CSystem.Sys.Svr.Properties.DetailTableDefineList(tableName);
            COMFields mainTable = CSystem.Sys.Svr.LDI.GetFields(tableName);
            string where = string.Format("D_StorageIn.CheckStatus=1 and D_StorageIn.ID Not In (Select Document from D_StorageData)");
            frmBrowser frm = new frmBrowser(mainTable, detailTable, where, enuShowStatus.None);
            frm.AllowDoubleClick = false;
            DataRow mainRow = frm.ShowSelect("Code",null, null);
            if (mainRow != null)
            {
                Guid StorageInID = (Guid)mainRow["ID"];
                DocumentInType = (enuDocumentIn)mainRow["DocumentType"];

                try
                {
                    Boolean Result;

                    switch (DocumentInType)
                    {

                        case enuDocumentIn.ConsignPurchase:
                            Result = UpdateStorageData(StorageInID, enuStorageType.Normal, false,mainRow);
                            if (Result == true)
                                Result = UpdateStorageData(StorageInID, enuStorageType.VendorConsign, true,mainRow);
                            break;
                        case enuDocumentIn.ConsignIn:
                            Result = UpdateStorageData(StorageInID, enuStorageType.VendorConsign, false,mainRow);
                            break;
                        case enuDocumentIn.PurchaseIn:
                            Result = UpdateStorageData(StorageInID, enuStorageType.Normal, false, mainRow);
                            break;
                        default:
                            Result = UpdateStorageData(StorageInID, enuStorageType.Normal, false, mainRow);
                            break;

                    }
                   
                }
                catch (Exception ex)
                {
                    Msg.Error(string.Format("物流收货确认出错！具体的错误信息如下：%n {0}", ex.Message));
                }

            }             
        }

 

        private bool UpdateStorageData(Guid StorageInDocument, enuStorageType TargetStorageType, Boolean Negate,DataRow mainRow)
        {

            DataSet dsInDetail = CSystem.Sys.Svr.cntMain.Select("Select * from D_StorageInDetail where MainID='" + StorageInDocument + "'", "D_StorageInDetail");
            if (dsInDetail.Tables["D_StorageInDetail"].Rows.Count > 0)
            {
                DataSet dsInNew = CSystem.Sys.Svr.cntMain.Select("Select * from D_StorageData where 1=0", "D_StorageData");
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
                CSystem.Sys.Svr.cntMain.Update(dsInNew.Tables["D_StorageData"]);
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
                Filter = string.Format("D_StorageData.DocumentType in (1,2,4)");

                COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);
                this._MainCOMFields = mainTableDefine;
                this._DetailCOMFields = detailTableDefines;

                frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm, Filter);
                return frm;
            }
            catch (Exception ex)
            {
                Msg.Warning(string.Format("物流收货程序加载失败！%n {0}", ex.Message));
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
