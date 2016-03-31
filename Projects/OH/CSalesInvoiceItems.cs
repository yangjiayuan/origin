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
    public class CSalesInvoiceItems : ToolDetailForm
    {
        private enuDocumentOut DocumentOutType;
        private ToolStripButton toolInvoiceConfirm;
        private enuSalesInvoice SalesInvoiceType;
        private Form _mdiForm;

        //class CSalesInvoiceItems
        //{
        //}

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
            toolInvoiceConfirm = new ToolStripButton();
            toolInvoiceConfirm.Font = new System.Drawing.Font("SimSun", 10.5F);
            toolInvoiceConfirm.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolInvoiceConfirm.Name = "toolInvoiceConfirm";
            toolInvoiceConfirm.Size = new System.Drawing.Size(39, 34);
            toolInvoiceConfirm.Text = "发票确认";
            toolInvoiceConfirm.Image = UI.clsResources.Finish;
            toolInvoiceConfirm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolInvoiceConfirm.Click += new EventHandler(toolInvoiceConfirm_Click);
            toolStrip.Items.Insert(i, toolInvoiceConfirm);
             
        }

        private void toolInvoiceConfirm_Click(object sender, EventArgs e)
        {
            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_StorageOut"), null, string.Format("D_StorageOut.CheckStatus=1 and D_StorageOut.DocumentType ={0} and D_StorageOut.ID Not In (Select StorageOut from D_SalesInvoiceItems)",(int)SalesInvoiceType), enuShowStatus.None);

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
                            Result = UpdateSalesInvoiceItems(StorageOutID, mainRow);
                            break;
                        default:
                            Result =  UpdateSalesInvoiceItems(StorageOutID,mainRow);
                            break;
                    }
                    
                }
                catch (Exception ex)
                {
                    Msg.Error(string.Format("销售发票确认出错！具体的错误信息如下：%n {0}", ex.Message));
                }

            }             
        }


        private bool UpdateSalesInvoiceItems(Guid StorageOutDocument, DataRow mainRow)
        {
            DataSet dsOutDetail = CSystem.Sys.Svr.cntMain.Select("Select * from D_StorageOutDetail where MainID='" + StorageOutDocument + "'", "D_StorageOutDetail");
            if (dsOutDetail.Tables["D_StorageOutDetail"].Rows.Count > 0)
            {
                DataSet dsInvoiceItemNew = CSystem.Sys.Svr.cntMain.Select("Select * from D_SalesInvoiceItems where 1=0", "D_SalesInvoiceItems");
                foreach (DataRow drStorageOut in dsOutDetail.Tables["D_StorageOutDetail"].Rows)
                {
                    DataRow drInvoiceNew = dsInvoiceItemNew.Tables["D_SalesInvoiceItems"].NewRow();
                    foreach (DataColumn dc in dsInvoiceItemNew.Tables["D_SalesInvoiceItems"].Columns)
                    {
                        switch (dc.ColumnName)
                        {
                            case "ID":
                                break;
                            case "InvoiceDate":
                            case "InvoiceNumber":
                                break;
                            case "DocumentType":
                                drInvoiceNew[dc.ColumnName] = mainRow[dc.ColumnName];
                                break;
                            case "Customer":
                                drInvoiceNew[dc.ColumnName] = mainRow[dc.ColumnName];
                                break;
                            case "StorageOut":
                                drInvoiceNew[dc.ColumnName] = StorageOutDocument;
                                break;
                            case "InvoiceAmount":
                                drInvoiceNew[dc.ColumnName] = drStorageOut["Amount"];
                                break;
                            case"CreatedBy":
                                drInvoiceNew[dc.ColumnName] = CSystem.Sys.Svr.User;
                                break;
                            case "CreateDate":
                                drInvoiceNew[dc.ColumnName] = CSystem.Sys.Svr.SystemTime;
                                break;
                            default:
                                if (dsOutDetail.Tables["D_StorageOutDetail"].Columns.Contains(dc.ColumnName))
                                {
                                    drInvoiceNew[dc.ColumnName] = drStorageOut[dc.ColumnName];
                                }
                                break;
                        }
                    }
                    dsInvoiceItemNew.Tables["D_SalesInvoiceItems"].Rows.Add(drInvoiceNew);
                }
                CSystem.Sys.Svr.cntMain.Update(dsInvoiceItemNew.Tables["D_SalesInvoiceItems"]);
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
                
                if (right.Paramters == null)
                    return null;
                string[] s = right.Paramters.Split(new char[] { ',' });
                if (s.Length > 0)
                    SalesInvoiceType = (enuSalesInvoice)int.Parse(s[0]);
                else
                    return null;
                this.MainTableName = "D_SalesInvoiceItems";

                Filter = string.Format("D_SalesInvoiceItems.DocumentType={0}", (int)SalesInvoiceType);

                SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                SortedList<string, object> Value = new SortedList<string, object>();

                Value.Add("InvoiceDate", CSystem.Sys.Svr.SystemTime.Date);
                Value.Add("DocumentType", (int)SalesInvoiceType);

                defaultValue.Add("D_SalesInvoiceItems", Value);

                COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);

                switch (SalesInvoiceType)
                {
                    case enuSalesInvoice.SalesOut:
                        mainTableDefine.Property.Title = string.Format("[销售发票]");
                        break;
                    case enuSalesInvoice.CustomerConsignSales:
                        mainTableDefine.Property.Title = string.Format("[销售发票-寄售]");
                        break;
                    default:
                        break;

                }
     
                this._MainCOMFields = mainTableDefine;
                this._DetailCOMFields = detailTableDefines;

                frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm, Filter);


                frm.DefaultValue = defaultValue;
                return frm;

            }
            catch (Exception ex)
            {
                Msg.Warning(string.Format("销售发票维护程序加载失败！%n {0}", ex.Message));
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
