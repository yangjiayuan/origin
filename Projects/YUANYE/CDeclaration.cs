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
    public class CDeclaration : ToolDetailForm
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
            decimal Rate = 0;
            decimal Amount = 0;
            //decimal Amount = 0;
            switch (e.Cell.Column.Key)
            {

                case "Amount":
                    if (e.Cell.Row.Cells["Rate"].Value != DBNull.Value)
                    {
                        Rate = (decimal)e.Cell.Row.Cells["Rate"].Value;
                        Amount = (decimal)e.Cell.Value;
                        e.Cell.Row.Cells["LCAmount"].Value = Math.Round(Amount * Rate, 2);

                    }
                    break;

                case "Rate":

                    if ((e.Cell.Row.Cells["Amount"].Value != DBNull.Value) && (e.Cell.Value != DBNull.Value))
                    {
                        Amount = (decimal)e.Cell.Row.Cells["Amount"].Value;
                        e.Cell.Row.Cells["LCAmount"].Value = Math.Round(Amount * (decimal)e.Cell.Value, 2);
                    }
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

            return true;
        }




        public override string BeforeSaving(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            string s = null;
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
            frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
            SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
            SortedList<string, object> Value = new SortedList<string, object>();
            Value.Add("DocumentDate", CSystem.Sys.Svr.SystemTime.Date);
            defaultValue.Add("D_Declaration", Value);
            frm.DefaultValue = defaultValue;
            return frm;
        }
    }
}
