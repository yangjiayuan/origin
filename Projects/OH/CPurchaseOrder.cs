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
namespace OH
{
    public class CPurchaseOrder : ToolDetailForm
    {
        public override bool AutoCode
        {
            get
            {
                return true;
            }
        }
        public override string GetPrefix()
        {
            return "PO";
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
            //if (f.FieldName == "TaxRate")
            //{
            //    UltraCurrencyEditor curr = ctl as UltraCurrencyEditor;
            //    curr.ValueChanged += new EventHandler(curr_ValueChanged);
            //}
        }
 

        void curr_ValueChanged(object sender, EventArgs e)
        {
            if (!_DetailForm.Showed)
                return;
            UltraCurrencyEditor curr = sender as UltraCurrencyEditor;
            if (curr == null)
                return;
            //foreach (UltraGridRow row in this._GridMap["D_PurchaseOrder"].Rows)
            //{
            //    if (row.Cells["Money"].Value != null && row.Cells["Money"].Value != DBNull.Value && (decimal)row.Cells["Money"].Value != 0)
            //    {
            //        row.Cells["Amount"].Value = (decimal)row.Cells["Money"].Value * curr.Value;
            //    }
            //}
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
            if (s[1] == "P_Package")
            {
                CCreateGrid CreatedGrid = (CCreateGrid)sender;
                UltraGridRow ActiveRow = CreatedGrid.Grid.ActiveCell.Row;
                Guid MaterialID= Guid.NewGuid();
                if (ActiveRow != null) 
                {
                    System.Console.WriteLine("Material" + ActiveRow.Cells["Material"].Value);
                    if (ActiveRow.Cells["Material"].Value.ToString().Length > 0)
                    {
                        MaterialID = (Guid)ActiveRow.Cells["Material"].Value;
                        e.Where = string.Format("P_Package.ID in (Select Package from P_MaterialPackage where MainID ='{0}')", MaterialID);
                    }
                 }
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
                    ////计算单价
                    
                    //if (e.Cell.Row.Cells["Quantity"].Value!= DBNull.Value)
                    //    Quantity = (decimal)e.Cell.Row.Cells["Quantity"].Value;
                    //if (Quantity != 0)
                    //    e.Cell.Row.Cells["Price"].Value = Math.Round((decimal)e.Cell.Value / Quantity, 4);

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
            //detailTableDefine[0]["FinishQuantity1"].Visible = COMField.Enum_Visible.NotVisible;
            //detailTableDefine[0]["FinishQuantity2"].Visible = COMField.Enum_Visible.NotVisible;
            return true;
        }
        public override string BeforeSaving(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            string s = null;
            //foreach (DataRow dr in ds.Tables[this._DetailForm.DetailTableDefine[0].OrinalTableName].Rows)
            //{
            //    if (dr.RowState != DataRowState.Deleted && (int)dr["NeedLength"] == 1 && (dr["Length"] == DBNull.Value || (decimal)dr["Length"] == 0))
            //        s = s + (string)dr["MaterialName"] + " 的长度没有输入！";
            //}
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
            ////检测有没有入库
            //DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select count(*) from D_WarehouseIn where BuyOrderID='{0}'", ID), conn, sqlTran);
            //if ((int)ds.Tables[0].Rows[0][0] > 0)
            //{
            //    Msg.Error("该采购订单已月入库,不能反复核!");
            //    return false;
            //}
            return base.UnCheck(ID, conn, sqlTran, mainRow);
        }

        void toolFinish_Click(object sender, EventArgs e)
        {
            //if (!_DetailForm.IsChecked) return;
            //if (Msg.Question("是否确认当前采购订单已完成?") != DialogResult.Yes)
            //    return;
            //using (SqlConnection conn = new SqlConnection(CSystem.Sys.Svr.cntMain.ConnectionString))
            //{
            //    SqlTransaction tran = null;
            //    try
            //    {
            //        conn.Open();
            //        tran = conn.BeginTransaction();

            //        DataRow dr = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0];
            //        if ((int)dr["ConsignStatus"] == 1)
            //        {
            //            if ((int)dr["Finished"] == 0)
            //            {
            //                DataSet ds2 = CSystem.Sys.Svr.cntMain.Select(string.Format("Select count(*) from D_WarehouseIn B where B.BuyOrderID='{0}' and B.InvoiceStatus=0", dr["ID"]), conn, tran);
            //                if ((int)ds2.Tables[0].Rows[0][0] == 0)
            //                    dr["InvoiceStatus"] = 2;
            //                dr["Finished"] = 1;
            //            }
            //            else
            //            {
            //                if ((int)dr["InvoiceStatus"] > 0)
            //                {
            //                    DataSet ds2 = CSystem.Sys.Svr.cntMain.Select(string.Format("Select count(*) from D_WarehouseIn B where B.BuyOrderID='{0}' and B.InvoiceStatus=1", dr["ID"]), conn, tran);
            //                    if ((int)ds2.Tables[0].Rows[0][0] > 0)
            //                        dr["InvoiceStatus"] = 1;
            //                }
            //                dr["Finished"] = 0;
            //            }
            //            CSystem.Sys.Svr.cntMain.Update(_DetailForm.MainDataSet.Tables[_DetailForm.MainTable]);
            //        }
            //        tran.Commit();
            //    }
            //    catch (Exception ex)
            //    {
            //        Msg.Warning(ex.ToString());
            //        if (tran != null)
            //            tran.Rollback();
            //    }
            //    finally
            //    {
            //        conn.Close();
            //    }
            //    _DetailForm.ChangeData();
            //}
        }
        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
            SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
            SortedList<string, object> Value = new SortedList<string, object>();
            Value.Add("DocumentDate", CSystem.Sys.Svr.SystemTime.Date);
            Value.Add("DelvDate", CSystem.Sys.Svr.SystemTime.Date);
            defaultValue.Add("D_PurchaseOrder", Value);
            frm.DefaultValue = defaultValue;
            return frm;
        }
    }
}
