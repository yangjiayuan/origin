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
    public class  CShipping : ToolDetailForm
    {
        public override bool AutoCode
        {
            get
            {
                return true;
            }
        }
 
        public override void NewControl(COMField f, System.Windows.Forms.Control ctl)
        {

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


        public override void NewGrid(COMFields fields, UltraGrid grid)
        {
            grid.AfterCellUpdate += new CellEventHandler(grid_AfterCellUpdate);
            grid.AfterRowInsert += new RowEventHandler(grid_AfterRowInsert);
        }

        void grid_AfterRowInsert(object sender, RowEventArgs e)
        {
            //try
            //{
            //    e.Row.Cells["Date"].Value = ((UltraDateTimeEditor)_ControlMap["DocumentDate"]).DateTime;
            //}
            //catch { }
        }

        void grid_AfterCellUpdate(object sender, CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {

                case "Amount":
                    UltraGrid grid2 = (UltraGrid)sender;

                    //如果需要在订单的表头级别记录下订单的总金额，则需要此功能来自动汇总计算.
                    decimal AmountTotal = 0;
                    foreach (UltraGridRow row in grid2.Rows)
                        if (row.Cells["Amount"].Value != DBNull.Value)
                            AmountTotal += (decimal)row.Cells["Amount"].Value;
                    _ControlMap["Amount"].Text = AmountTotal.ToString();

                    break;

                case "Rate":
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

        public override void SetCreateDetail(CCreateDetailForm createDetailForm)
        {
            base.SetCreateDetail(createDetailForm);
        }
  
        public override bool NewData(DataSet ds, COMFields mainTableDefine, List<COMFields> detailTableDefine)
        {
            string FilterCondition;


            FilterCondition = string.Format("D_StorageOut.CheckStatus=1 and D_StorageOut.ID not in (Select StorageOutID from D_Shipping)");// and  D_StorageOut.ID not in (Select );

            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_StorageOut"), null, FilterCondition, enuShowStatus.None);

            DataRow dr = frm.ShowSelect("Code",null, null);

            if (dr != null)
            {
                DataRow newMainDR = ds.Tables["D_Shipping"].Rows[0];
                newMainDR["Code"] = dr["Code"];
                newMainDR["StorageOutID"] = dr["ID"];
                newMainDR["StorageOutNo"] = dr["Code"];
                newMainDR["Customer"] = dr["Customer"];
                newMainDR["CustomerName"] = dr["CustomerName"];
                newMainDR["Priceterms"] = dr["Priceterms"];
                newMainDR["ShippingType"] = dr["ShippingType"];
                newMainDR["POL"] = dr["POL"];
                newMainDR["POD"] = dr["POD"];
                newMainDR["DEST"] = dr["DEST"];
                newMainDR["Bill"] = dr["Bill"];
                newMainDR["Currency"] = dr["Currency"];
                newMainDR["CurrencyName"] = dr["CurrencyName"];

                COMFields FieldsDetail = CSystem.Sys.Svr.LDI.GetFields("D_StorageOutDetail");
                string SQLDetail = FieldsDetail.QuerySQLWithClause(" MainID = '" + dr["ID"] + "'");
                DataSet dsShippingItems = CSystem.Sys.Svr.cntMain.Select(SQLDetail, "D_StorageoutDetail");

                foreach (DataRow drItems in dsShippingItems.Tables["D_StorageOutDetail"].Rows)
                {
                    DataRow newDR = ds.Tables["D_ShippingDetail"].NewRow();

                    newDR["LineNumber"] = drItems["LineNumber"];
                    newDR["CPONumber"] = drItems["CPONumber"];
                    newDR["SalesOrder"] = drItems["SalesOrder"];
                    newDR["SONumber"] = drItems["SONumber"];
                    newDR["Style"] = drItems["Style"];
                    newDR["Material"] = drItems["Material"];
                    newDR["MaterialName"] = drItems["MaterialName"];
                    newDR["MaterialEnglishName"] = drItems["MaterialEnglishName"];
                    newDR["HSCode"] = drItems["HSCode"];
                    newDR["Measure"] = drItems["Measure"];
                    newDR["MeasureName"] = drItems["MeasureName"];
                    newDR["Quantity"] = drItems["Quantity"];
                    newDR["Price"] = drItems["Price"];
                    newDR["Amount"] = drItems["Amount"];
                    newDR["CTN"] = drItems["CTN"];
                    newDR["GW"] = drItems["GW"];
                    newDR["NW"] = drItems["NW"];
                    newDR["CBM"] = drItems["CBM"];

                    ds.Tables["D_ShippingDetail"].Rows.Add(newDR);
                }


                return true;
            }
            else
            {
                return false;
            }
           
        }




        public override string BeforeSaving(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                decimal total = 0;

                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_ShippingDetail"].Rows)
                    if (dr.RowState != DataRowState.Deleted)
                        total += (decimal)dr["Amount"];
                _ControlMap["Amount"].Text = total.ToString();

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
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
            frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
            SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
            SortedList<string, object> Value = new SortedList<string, object>();
            Value.Add("DocumentDate", CSystem.Sys.Svr.SystemTime.Date);
            defaultValue.Add("D_Shipping", Value);
            frm.DefaultValue = defaultValue;
            return frm;
        }
    }
}
