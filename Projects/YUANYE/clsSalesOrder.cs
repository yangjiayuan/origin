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
namespace YUANYE
{
    public class clsSalesOrder : ToolDetailForm
    {
        private Form _mdiForm;
        private int SalesOrderType;

        public override bool AutoCode
        {
            get
            {
                return true;
            }
        }
        public override string GetPrefix()
        {
            return "NTYY";
        }

        public override string AutoCodeFormat
        {
            get
            {
                return "yyyy";
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
        public override bool AllowCheck
        {
            get
            {
                return true;
            }
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
            if (s[1] == "P_Material")
            {
                CCreateGrid CreatedGrid = (CCreateGrid)sender;
                UltraGridRow ActiveRow = CreatedGrid.Grid.ActiveCell.Row;
                Guid Customer;

                if (_DetailForm.MainDataSet.Tables["D_SalesOrder"].Rows[0]["Customer"].ToString().Length > 0 )
                    Customer = (Guid)this._DetailForm.MainDataSet.Tables["D_SalesOrder"].Rows[0]["Customer"];
                else
                    Customer=Guid.Empty;

                if (ActiveRow != null)
                {
                    e.Where = string.Format("P_Material.Customer = '{0}'", Customer);
                }
            }
        }

        void _CreateGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {
            //CCreateGrid createGrid = (CCreateGrid)sender;
            //if ((e.Field.FieldName == "MaterialName") || (e.Field.FieldName == "MaterialCode"))
            //{
            //    createGrid.Grid.ActiveRow.Cells["Package"].Value = createGrid.Grid.ActiveRow.Cells["MPackage"].Value;
            //    createGrid.Grid.ActiveRow.Cells["PackageName"].Value = createGrid.Grid.ActiveRow.Cells["MPackageName"].Value;
            //}
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

        }
        public override bool UnCheck(Guid ID, SqlConnection conn, SqlTransaction sqlTran, DataRow mainRow)
        {
            return base.UnCheck(ID, conn, sqlTran, mainRow);
        }

        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            string Filter;

            _mdiForm = mdiForm;

            if (right.Paramters == null)
                return null;
            string[] s = right.Paramters.Split(new char[] { ',' });
            if (s.Length > 0)
                SalesOrderType = int.Parse(s[0]);
            else
                return null;
            this.MainTableName = "D_SalesOrder";
            Filter = string.Format("D_SalesOrder.OrderType={0}", (int)SalesOrderType);


 
            SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
            SortedList<string, object> Value = new SortedList<string, object>();
            Value.Add("DocumentDate", CSystem.Sys.Svr.SystemTime.Date);
            Value.Add("DelvDate", CSystem.Sys.Svr.SystemTime.Date);
            Value.Add("Company", this.MainCOMFields["Company"].DefaultValue);
            Value.Add("CompanyName", this.MainCOMFields["CompanyName"].DefaultValue);
            Value.Add("Currency", this.MainCOMFields["Currency"].DefaultValue);
            Value.Add("CurrencyName", this.MainCOMFields["CurrencyName"].DefaultValue);
            Value.Add("OrderType", (int)SalesOrderType);
            defaultValue.Add("D_SalesOrder", Value);

            frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm, Filter);
            frm.DefaultValue = defaultValue;
            return frm;
        }
    }
}
