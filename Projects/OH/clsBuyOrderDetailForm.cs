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
    public class clsBuyOrderDetailForm : ToolDetailForm
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
            return "CG";
        }
        public override void NewControl(COMField f, System.Windows.Forms.Control ctl)
        {
            if (f.FieldName == "TaxRate")
            {
                UltraCurrencyEditor curr = ctl as UltraCurrencyEditor;
                curr.ValueChanged += new EventHandler(curr_ValueChanged);
            }
        }

        void curr_ValueChanged(object sender, EventArgs e)
        {
            if (!_DetailForm.Showed)
                return;
            UltraCurrencyEditor curr= sender as UltraCurrencyEditor;
            if (curr==null) 
                return;
            foreach (UltraGridRow row in this._GridMap["D_BuyOrderBill"].Rows)
            {
                if (row.Cells["Money"].Value != null && row.Cells["Money"].Value != DBNull.Value && (decimal)row.Cells["Money"].Value!=0)
                {
                    row.Cells["Amount"].Value = (decimal)row.Cells["Money"].Value * curr.Value;
                }
            }
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
            if (s[1] == "P_Material")
            {
                //将产成品放开，只过滤半成品
                //e.Where = "P_Material.MaterialType <> 1 and P_Material.MaterialType <> 2";
                e.Where = "P_Material.MaterialType <> 1";
            }
        }

        void _CreateGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {
            CCreateGrid createGrid = (CCreateGrid)sender;
            if (e.Field.FieldName == "MaterialCode" || e.Field.FieldName == "MaterialName")
            {
                if ((int)e.Row["NeedLength"] == 1)
                    createGrid.Grid.ActiveRow.Cells["Length"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                else
                    createGrid.Grid.ActiveRow.Cells["Length"].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
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
                e.Row.Cells["ConsignDate"].Value = ((UltraDateTimeEditor)_ControlMap["ConsignDate"]).DateTime;
            }
            catch { }
        }

        void grid_AfterCellUpdate(object sender, CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "Quantity1":
                    if ((int)e.Cell.Row.Cells["QuantityCheckMethod"].Value == 1)
                    {
                        decimal length = 1;
                        if (e.Cell.Row.Cells["Length"].Activation == Activation.AllowEdit && e.Cell.Row.Cells["Length"].Value != DBNull.Value)
                            length = (decimal)e.Cell.Row.Cells["Length"].Value;
                        decimal conv = (decimal)e.Cell.Row.Cells["ConvertQuotiety"].Value;
                        if (conv == 0)
                            conv = 1;
                        e.Cell.Row.Cells["Quantity2"].Value = Math.Round((decimal)e.Cell.Value * length * conv, 4);
                    }
                    break;
                case "Quantity2":
                    if ((int)e.Cell.Row.Cells["QuantityCheckMethod"].Value == 2)
                    {
                        decimal length = 1;
                        if (e.Cell.Row.Cells["Length"].Activation == Activation.AllowEdit && e.Cell.Row.Cells["Length"].Value != DBNull.Value)
                            length = (decimal)e.Cell.Row.Cells["Length"].Value;
                        decimal conv = (decimal)e.Cell.Row.Cells["ConvertQuotiety"].Value;
                        if (conv == 0)
                            conv = 1;
                        e.Cell.Row.Cells["Quantity1"].Value = Math.Round((decimal)e.Cell.Value * length * conv, 4);
                    }
                    break;
                case "Price":
                    //decimal qty = 0;
                    //if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 1)
                    //    qty = (decimal)e.Cell.Row.Cells["Quantity1"].Value;
                    //else if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 2)
                    //    qty = (decimal)e.Cell.Row.Cells["Quantity2"].Value;
                    //e.Cell.Row.Cells["Amount"].Value = Math.Round((decimal)e.Cell.Value * qty,4);
                    break;
                case "Money":
                    //计算总金额
                    UltraGrid grid = (UltraGrid)sender;
                    decimal total = 0;
                    foreach (UltraGridRow row in grid.Rows)
                        if (row.Cells["Money"].Value!=DBNull.Value)
                            total += (decimal)row.Cells["Money"].Value;
                    _ControlMap["Money"].Text = total.ToString();
                    break;
                case "Amount":
                    UltraGrid grid2 = (UltraGrid)sender;
                    decimal total2 = 0;
                    foreach (UltraGridRow row in grid2.Rows)
                        if (row.Cells["Amount"].Value!=DBNull.Value)
                            total2 += (decimal)row.Cells["Amount"].Value;
                    _ControlMap["Amount"].Text = total2.ToString();
                    //计算不含税金额
                    e.Cell.Row.Cells["Money"].Value = Math.Round((decimal)e.Cell.Value / (1 + ((UltraCurrencyEditor)this._ControlMap["TaxRate"]).Value / 100), 2);
                    //计算单价
                    decimal qty2 = 0;
                    if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 1)
                        qty2 = (decimal)e.Cell.Row.Cells["Quantity1"].Value;
                    else if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 2)
                        qty2 = (decimal)e.Cell.Row.Cells["Quantity2"].Value;
                    else
                        break;
                    //取采购系数
                    decimal rate2 = (decimal)e.Cell.Row.Cells["BuyConvertQuotiety"].Value; ;
                    if (rate2 == 0) rate2 = 1;
                    if (qty2 != 0)
                        e.Cell.Row.Cells["Price"].Value = Math.Round((decimal)e.Cell.Value / qty2, 4);
                    if (qty2 != 0)
                        e.Cell.Row.Cells["BuyPrice"].Value = Math.Round((decimal)e.Cell.Value / (qty2 / rate2), 4);

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
            detailTableDefine[0]["FinishQuantity1"].Visible = COMField.Enum_Visible.NotVisible;
            detailTableDefine[0]["FinishQuantity2"].Visible = COMField.Enum_Visible.NotVisible;
            return true;
        }
        public override string BeforeSaving(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            string s = null;
            foreach (DataRow dr in ds.Tables[this._DetailForm.DetailTableDefine[0].OrinalTableName].Rows)
            {
                if (dr.RowState!= DataRowState.Deleted && (int)dr["NeedLength"] == 1 && (dr["Length"] == DBNull.Value || (decimal)dr["Length"] == 0))
                    s = s + (string)dr["MaterialName"] + " 的长度没有输入！";
            }
            return s;
        }
        public override void InsertToolStrip(System.Windows.Forms.ToolStrip toolStrip,ToolDetailForm.enuInsertToolStripType insertType)
        {
            base.InsertToolStrip(toolStrip,insertType);
            int i = toolStrip.Items.Count;
            if (insertType == enuInsertToolStripType.Detail)
            {
                ToolStripButton toolFinish = new ToolStripButton();
                toolFinish.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolFinish.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolFinish.Name = "toolSplit";

                toolFinish.Text = "完成";
                toolFinish.Image = clsResources.Finish;
                toolFinish.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolFinish.Click += new EventHandler(toolFinish_Click);

                toolStrip.Items.Insert(i - 2, toolFinish);
            }
        }
        public override bool UnCheck(Guid ID, SqlConnection conn, SqlTransaction sqlTran, DataRow mainRow)
        {
            //检测有没有入库
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select count(*) from D_WarehouseIn where BuyOrderID='{0}'", ID), conn, sqlTran);
            if ((int)ds.Tables[0].Rows[0][0] > 0)
            {
                Msg.Error("该采购订单已月入库,不能反复核!");
                return false;
            }
            return base.UnCheck(ID, conn, sqlTran, mainRow);
        }

        void toolFinish_Click(object sender, EventArgs e)
        {
            if (!_DetailForm.IsChecked) return;
            if (Msg.Question("是否确认当前采购订单已完成?") != DialogResult.Yes)
                return;
            using (SqlConnection conn = new SqlConnection(CSystem.Sys.Svr.cntMain.ConnectionString))
            {
                SqlTransaction tran = null;
                try
                {
                    conn.Open();
                    tran = conn.BeginTransaction();

                    DataRow dr = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0];
                    if ((int)dr["ConsignStatus"] == 1)
                    {
                        if ((int)dr["Finished"] == 0)
                        {
                            DataSet ds2 = CSystem.Sys.Svr.cntMain.Select(string.Format("Select count(*) from D_WarehouseIn B where B.BuyOrderID='{0}' and B.InvoiceStatus=0", dr["ID"]), conn, tran);
                            if ((int)ds2.Tables[0].Rows[0][0] == 0)
                                dr["InvoiceStatus"] = 2;
                            dr["Finished"] = 1;
                        }
                        else
                        {
                            if ((int)dr["InvoiceStatus"] > 0)
                            {
                                DataSet ds2 = CSystem.Sys.Svr.cntMain.Select(string.Format("Select count(*) from D_WarehouseIn B where B.BuyOrderID='{0}' and B.InvoiceStatus=1", dr["ID"]), conn, tran);
                                if ((int)ds2.Tables[0].Rows[0][0] > 0)
                                    dr["InvoiceStatus"] = 1;
                            }
                            dr["Finished"] = 0;
                        }
                        CSystem.Sys.Svr.cntMain.Update(_DetailForm.MainDataSet.Tables[_DetailForm.MainTable]);
                    }
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    Msg.Warning(ex.ToString());
                    if (tran != null)
                        tran.Rollback();
                }
                finally
                {
                    conn.Close();
                }
                _DetailForm.ChangeData();
            }
        }
        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
            SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
            SortedList<string, object> Value = new SortedList<string, object>();
            Value.Add("BillDate", CSystem.Sys.Svr.SystemTime.Date);
            Value.Add("ConsignDate", CSystem.Sys.Svr.SystemTime.Date);
            defaultValue.Add("D_BuyOrder", Value);
            frm.DefaultValue = defaultValue;
            return frm;
        }
    }
}
