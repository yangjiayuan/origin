using System;
using System.Collections.Generic;
using System.Text;
using UI;
using Base;
using System.Windows.Forms;
using System.Data;
using Infragistics.Win.UltraWinGrid;
using System.Data.SqlClient;
using Infragistics.Win.Printing;

namespace OH
{
    class clsSendGoods : ToolDetailForm
    {
        private DBConnection cntMain;
        private string _Interface;
        private Guid _CustomerID;
        private string _CustmerCode;
        private string _CustmerName;
        frmBrowser frmSendGoods = null;
        public Infragistics.Win.UltraWinGrid.UltraGrid grid;
        private ToolStripButton toolPrint;
        private ToolStripButton toolPreview;

        public override bool AllowDeleteRowInToolBar
        {
            get
            {
                return true;
            }
        }
        public override bool AllowInsertRowInToolBar
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
        public override string GetPrefix()
        {
            return "SHD";
        }
        public override void NewGrid(Base.COMFields fields, Infragistics.Win.UltraWinGrid.UltraGrid grid)
        {
            if (fields.OrinalTableName == "D_SendGoodsBill")
            {
                grid.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(grid_AfterCellUpdate);
            }
        }
        void grid_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            UltraGrid grid = (UltraGrid)sender;
            decimal t = 0;
            string key = e.Cell.Column.Key;
            switch (key)
            {
              
            }
        }
        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            try
            {
                this.MainTableName = "D_SendGoods";
                cntMain = CSystem.Sys.Svr.cntMain;
                COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);
                string filter = "";
                //先选择客户
                frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("P_Customer"), null, null, enuShowStatus.None);
                DataRow dr = frm.ShowSelect(null, null, null);
                if (dr != null)
                {
                    _CustomerID = (Guid)dr["ID"];
                    _CustmerCode = dr["Code"] as string;
                    _CustmerName = dr["Name"] as string;
                    _Interface = dr["Interface"] as string;
                    string text = string.Format("{0}[{1}]", "发货单", _CustmerName);
                    mainTableDefine.Property.Title = text;
                    //mainTableDefine["CheckStatus"].Visible = COMField.Enum_Visible.VisibleInBrower;
                    //mainTableDefine["CheckedBy"].Visible = COMField.Enum_Visible.NotVisible;
                    //mainTableDefine["CheckedByName"].Visible = COMField.Enum_Visible.VisibleInBrower;
                    //mainTableDefine["CheckDate"].Visible = COMField.Enum_Visible.VisibleInBrower;
                    //mainTableDefine["PackagePlanTypeCheckStatus"].Visible = COMField.Enum_Visible.NotVisible;
                    //mainTableDefine["PackagePlanTypeCheckedBy"].Visible = COMField.Enum_Visible.NotVisible;
                    //mainTableDefine["PackagePlanTypeCheckedByName"].Visible = COMField.Enum_Visible.NotVisible;
                    //mainTableDefine["PackagePlanTypeCheckDate"].Visible = COMField.Enum_Visible.NotVisible;
                    //mainTableDefine["AllocationTypeCheckStatus"].Visible = COMField.Enum_Visible.NotVisible;
                    //mainTableDefine["AllocationTypeCheckedBy"].Visible = COMField.Enum_Visible.NotVisible;
                    //mainTableDefine["AllocationTypeCheckedByName"].Visible = COMField.Enum_Visible.NotVisible;
                    //mainTableDefine["AllocationTypeCheckDate"].Visible = COMField.Enum_Visible.NotVisible;

                    SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                    SortedList<string, object> Value = new SortedList<string, object>();
                    Value.Add("BillDate", CSystem.Sys.Svr.SystemTime.Date);

                    Value.Add("CustomerID", _CustomerID);
                    //dr["CustomerCode"] = _CustomerCode;
                    Value.Add("CustomerName", _CustmerName);
                    defaultValue.Add(this.MainTableName, Value);


                    this._MainCOMFields = mainTableDefine;
                    this._DetailCOMFields = detailTableDefines;
                    frmSendGoods = new frmBrowser(_MainCOMFields, _DetailCOMFields, "D_SendGoods.deleted=0" + filter, enuShowStatus.None);
                    frmSendGoods.DefaultValue = defaultValue;                    
                    this.SetBrowerForm(frmSendGoods);
                    frmSendGoods.toolDetailForm = this;
                    grid = _BrowserForm.MainGrid;
                    //_DetailForm = ;
                    //this.SetDetailForm(frmSalesOrderBill);
                }
            }
            catch (Exception ex)
            {
            }
            return frmSendGoods;
        }
        public override System.Data.DataSet InsertRowsInGrid(List<Base.COMFields> detailTableDefine)
        {
            DataSet ds = _DetailForm.MainDataSet;
            frmBrowser frmCustomer = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_SalesOrder"), null, "ConsignStatus=0 and D_SalesOrder.CustomerID='" + _CustomerID + "'  ", enuShowStatus.None);
            DataRow[] drs = frmCustomer.ShowSelectRows(null, null, null);
            //frm.ShowDialog();
            if (drs != null)
            {
                foreach (DataRow dr in drs)
                {
                    if (dr.RowState == DataRowState.Deleted)
                        continue;
                    DataRow[] drs_D = ds.Tables["D_SendGoodsBill"].Select(string.Format("SalesOrderID='{0}'", dr["ID"]));
                    for (int i = 0; i < drs_D.Length; i++)
                        drs_D[i].Delete();
                    DataRow newDrD = ds.Tables["D_SendGoodsBill"].NewRow();
                    newDrD["LineNumber"] = ds.Tables["D_SendGoodsBill"].Rows.Count + 1;
                    newDrD["SalesOrderID"] = dr["ID"];
                    newDrD["Code"] = dr["Code"];
                    newDrD["ElevatorCode"] = dr["ElevatorCode"];
                    newDrD["ContractCode"] = dr["ContractCode"];
                    newDrD["ProjectCode"] = dr["ProjectCode"];
                    //newDrD["ProjectName"] = dr["ProjectName"];
                    //newDrD["ConsignDate"] = dr["ConsignDate"];
                    newDrD["BoxCount"] = dr["BoxCount"];
                    newDrD["CalBoxCount"] = dr["CalBoxCount"];
                    newDrD["PackageQuantity"] = dr["PackageQuantity"];
                    newDrD["CalPackageQuantity"] = dr["CalPackageQuantity"];
                    newDrD["Notes"] = dr["Notes"];
                    newDrD["BillDate"] = dr["BillDate"];
                    //newDrD["CreateDate"] = dr["CreateDate"];
                    newDrD["Weight"] = dr["Weight"];
                    newDrD["NetWeight"] = dr["NetWeight"];
                    newDrD["Money"] = dr["Money"];
                    newDrD["Amount"] = dr["Amount"];
                    newDrD["Tax"] = dr["Tax"];
                    ds.Tables["D_SendGoodsBill"].Rows.Add(newDrD);
                }
            }
            return ds;
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
            toolPreview.Text = "出货单打印";
            toolPreview.Image = UI.clsResources.Preview;
            toolPreview.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolPreview.Click += new EventHandler(toolPrint_Click);
            toolStrip.Items.Insert(i, toolPreview);

            toolPrint = new ToolStripButton();
            toolPrint.Font = new System.Drawing.Font("SimSun", 10.5F);
            toolPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolPrint.Name = "toolPrint";
            toolPrint.Size = new System.Drawing.Size(39, 34);
            toolPrint.Text = "出货单预览";
            toolPrint.Image = UI.clsResources.Print;
            toolPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolPrint.Click += new EventHandler(toolPreview_Click);
            toolStrip.Items.Insert(i, toolPrint);

            toolPreview = new ToolStripButton();
            toolPreview.Font = new System.Drawing.Font("SimSun", 10.5F);
            toolPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolPreview.Name = "toolHeadPreview";
            toolPreview.Size = new System.Drawing.Size(39, 34);
            toolPreview.Text = "出门单打印";
            toolPreview.Image = UI.clsResources.Preview;
            toolPreview.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolPreview.Click += new EventHandler(toolHeadPrint_Click);
            toolStrip.Items.Insert(i, toolPreview);

            toolPrint = new ToolStripButton();
            toolPrint.Font = new System.Drawing.Font("SimSun", 10.5F);
            toolPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolPrint.Name = "toolHeadPrint";
            toolPrint.Size = new System.Drawing.Size(39, 34);
            toolPrint.Text = "出门单预览";
            toolPrint.Image = UI.clsResources.Print;
            toolPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolPrint.Click += new EventHandler(toolHeadPreview_Click);
            toolStrip.Items.Insert(i, toolPrint);
        }
        private void toolHeadPreview_Click(object sender, EventArgs e)
        {
            HeadPrint(true);
        }
        private void toolHeadPrint_Click(object sender, EventArgs e)
        {
            HeadPrint(false);
        }
        private void HeadPrint(bool IsPreview)
        {
            if (this._BrowserForm.grid.ActiveRow == null)
                return;

            frmPrintSetting ps = new frmPrintSetting(AppDomain.CurrentDomain.BaseDirectory + "PrintTemplate\\CMAllocation");
            if (ps.ShowDialog() == DialogResult.OK)
            {
                string tempFile = ps.TempateFile;
                clsPrintTemplate Template = new clsPrintTemplate(tempFile);

                if (_BrowserForm.grid.Selected.Rows.Count == 0 && _BrowserForm.grid.ActiveRow != null)
                    _BrowserForm.grid.ActiveRow.Selected = true;

                int index = 1;
                int count = _BrowserForm.grid.Selected.Rows.Count;
                foreach (UltraGridRow row in _BrowserForm.grid.Selected.Rows)
                {
                    Guid id = (Guid)row.Cells["ID"].Value;
                    DataSet ds = GetCMData(id);
                    DataRow dr = ds.Tables["D_SalesOrder"].Rows[0];
                    if (VarConverTo.ConvertToString(dr["CusCode"]).Length == 0)
                        dr["CusCode"] = dr["Code"];
                    clsPrintDocument doc = new clsPrintDocument(Template, ds, ps.PrintSetting);
                    if (IsPreview)
                    {
                        if (count > 1 && Msg.Question(string.Format("将要预览第{0}出门单，共{1}张出门单，是否继续？", index, count)) != DialogResult.Yes)
                            return;
                        index++;

                        UltraPrintPreviewDialog pre = new UltraPrintPreviewDialog();
                        pre.Document = doc;
                        pre.ShowDialog();
                    }
                    else
                        doc.Print();
                }
            }
        }

        private void toolPreview_Click(object sender, EventArgs e)
        {
            Print(true);
        }
        private void toolPrint_Click(object sender, EventArgs e)
        {
            Print(false);
        }
        private void Print(bool IsPreview)
        {
            if (this._BrowserForm.grid.ActiveRow == null)
                return;

            frmPrintSetting ps = new frmPrintSetting(AppDomain.CurrentDomain.BaseDirectory + "PrintTemplate\\Allocation");
            if (ps.ShowDialog() == DialogResult.OK)
            {
                string tempFile = ps.TempateFile;
                clsPrintTemplate Template = new clsPrintTemplate(tempFile);

                if (_BrowserForm.grid.Selected.Rows.Count == 0 && _BrowserForm.grid.ActiveRow != null)
                    _BrowserForm.grid.ActiveRow.Selected = true;

                int index = 1;
                int count = _BrowserForm.grid.Selected.Rows.Count;
                foreach (UltraGridRow row in _BrowserForm.grid.Selected.Rows)
                {
                    Guid id = (Guid)row.Cells["ID"].Value;
                    DataSet sendGoodDs = new DataSet();
                    DataSet ds =new DataSet();
                    CSystem.Sys.Svr.cntMain.Select(this._DetailCOMFields[0].QuerySQLWithClause(_DetailCOMFields[0].GetTableName(false) + ".MainID='" + id + "' order by LineNumber"), _DetailCOMFields[0].GetTableName(false), sendGoodDs);
                    for (int i = 0; i < sendGoodDs.Tables[0].Rows.Count; i++)
                    {
                        Guid salesOrderId = (Guid)sendGoodDs.Tables[0].Rows[i]["SalesOrderID"];
                        ds = GetData(salesOrderId);
                        DataRow dr = ds.Tables["D_SalesOrder"].Rows[0];
                        if (VarConverTo.ConvertToString(dr["CusCode"]).Length == 0)
                            dr["CusCode"] = dr["Code"];
                    }
                    clsPrintDocument doc = new clsPrintDocument(Template, ds, ps.PrintSetting);
                    if (IsPreview)
                    {
                        if (count > 1 && Msg.Question(string.Format("将要预览第{0}出货单，共{1}张出货单，是否继续？", index, count)) != DialogResult.Yes)
                            return;
                        index++;

                        UltraPrintPreviewDialog pre = new UltraPrintPreviewDialog();
                        pre.Document = doc;
                        pre.ShowDialog();
                    }
                    else
                        doc.Print();
                }
            }
        }
        private DataSet GetData(Guid id)
        {            
            COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields("D_SalesOrder");
            List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone("D_SalesOrder");
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(mainTableDefine.QuerySQLWithClause(mainTableDefine.GetTableName(false) + ".ID='" + id + "'"), "D_SalesOrder");
            CSystem.Sys.Svr.cntMain.Select(detailTableDefines[0].QuerySQLWithClause(detailTableDefines[0].GetTableName(false) + ".MainID='" + id + "' order by LineNumber"), detailTableDefines[0].GetTableName(false), ds);
            return ds;
        }
        private DataSet GetCMData(Guid id)
        {
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(_MainCOMFields.QuerySQLWithClause(this._MainCOMFields.GetTableName(false) + ".ID='" + id + "'"), this._MainCOMFields.GetTableName(false));
            CSystem.Sys.Svr.cntMain.Select(_DetailCOMFields[0].QuerySQLWithClause(_DetailCOMFields[0].GetTableName(false) + ".MainID='" + id + "' order by LineNumber"), _DetailCOMFields[0].GetTableName(false), ds);
            return ds;
        }
        public override DataSet DeleteRowsInGrid(List<COMFields> DetailTableDefine)
        {
            if (_DetailForm.ActiveControl.GetType() == typeof(UltraGrid))
            {
                UltraGrid grid = _DetailForm.ActiveControl as UltraGrid;
                if (grid.ActiveRow != null)
                {
                    Guid id = (Guid)grid.ActiveRow.Cells["SalesOrderID"].Value;
                    DataRow[] drs = _DetailForm.MainDataSet.Tables["D_SendGoodsBill"].Select("SalesOrderID='" + id + "'");
                    for (int i = drs.Length - 1; i >= 0; i--)
                    {
                        drs[i].Delete();
                        cntMain.Excute(string.Format("update {0} set ConsignStatus=0 where  id='{1}'", "D_SalesOrder", id));
                    }
                    return _DetailForm.MainDataSet;
                }
            }
            return null;
        }
        public override  bool Deleting(SqlConnection conn, SqlTransaction tran, Guid id)
        {
            DataSet sendGoodDs=new DataSet();
            CSystem.Sys.Svr.cntMain.Select(this._DetailCOMFields[0].QuerySQLWithClause(_DetailCOMFields[0].GetTableName(false) + ".MainID='" + id + "' order by LineNumber"), _DetailCOMFields[0].GetTableName(false), sendGoodDs);
            using (conn = new SqlConnection(cntMain.ConnectionString))
            {
                SqlTransaction sqlTran = null;
                try
                {
                    conn.Open();
                    sqlTran = conn.BeginTransaction();
                    foreach (DataRow dr in sendGoodDs.Tables["D_SendGoodsBill"].Rows)
                    {
                        Guid sid = (Guid)dr["SalesOrderID"];
                        cntMain.Excute(string.Format("update {0} set ConsignStatus=0 where  id='{1}'", "D_SalesOrder", sid), conn, sqlTran);
                    }
                    sqlTran.Commit();
                }
                catch (Exception ex)
                {
                    Msg.Warning(ex.ToString());
                    sqlTran.Rollback();
                }
                finally
                {
                    conn.Close();
                }
            }
            return true;
        }
        public override void AfterSaved(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            using (conn = new SqlConnection(cntMain.ConnectionString))
            {
                SqlTransaction sqlTran = null;
                try
                {
                    conn.Open();
                    sqlTran = conn.BeginTransaction();
                    foreach (DataRow dr in ds.Tables["D_SendGoodsBill"].Rows)
                    {
                        Guid id = (Guid)dr["SalesOrderID"];
                        cntMain.Excute(string.Format("update {0} set ConsignStatus=1,ConsignDate=CONVERT(varchar(10), '{2}', 23) where  id='{1}'", "D_SalesOrder", id,System.DateTime.Now), conn, sqlTran);
                    }
                    sqlTran.Commit();
                }
                catch (Exception ex)
                {
                    Msg.Warning(ex.ToString());
                    sqlTran.Rollback();
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}
