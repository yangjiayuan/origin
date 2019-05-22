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
    public class clsARConfirm : ToolDetailForm
    {

        private Form _mdiForm;
        private int RevenueType;


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
            string[] s = e.Field.ValueType.Split(':');
            CCreateGrid CreatedGrid = (CCreateGrid)sender;
            UltraGridRow ActiveRow = CreatedGrid.Grid.ActiveCell.Row;
            if (this._DetailForm.MainDataSet.Tables["D_ARConfirm"].Rows[0]["Customer"] == DBNull.Value)
            {
                Msg.Information("请先选择收款对应的客户名称后再进行此操作!");
                e.Where = string.Format(" 0=1 ");
            }
            else
            {
                Guid Customer = (Guid)this._DetailForm.MainDataSet.Tables["D_ARConfirm"].Rows[0]["Customer"];
                switch (s[1])
                {
                    case "V_AccountReceivable":
                        if (ActiveRow != null)
                        {
                            e.Where = string.Format("(V_AccountReceivable.Customer = '{0}' or V_AccountReceivable.Customer in (Select Customer from P_CustomerDeliveryto Where MainID = '{0}')) and V_AccountReceivable.Balance > 0 and DocumentStatus =0", Customer);
                        }
                        break;
                    case "D_SalesOrder":
                        if (ActiveRow != null)
                        {
                            //e.Where = string.Format("D_SalesOrder.Customer = '{0}'", Customer);
                            e.Where = string.Format("(D_SalesOrder.Customer = '{0}' or D_SalesOrder.Customer in (Select Customer from P_CustomerDeliveryto Where MainID = '{0}'))", Customer);
                        }
                        break;
                    default:
                        break;
                }
            }

        }

        void _CreateGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {
            CCreateGrid createGrid = (CCreateGrid)sender;
            if ((e.Field.FieldName == "StorageOut") || (e.Field.FieldName == "StorageOutNo"))
            {
                createGrid.Grid.ActiveRow.Cells["Amount"].Value = (decimal) e.Row["Balance"]; 
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

                e.Row.Cells["Rate"].Value = _ControlMap["Rate"].Text;
            }
            catch { }
        }

        void grid_AfterCellUpdate(object sender, CellEventArgs e)
        {   
            decimal Rate = 0;
            decimal Amount = 0;

            switch (e.Cell.Column.Key)
            {

                case "Amount":

                    Rate = (decimal)this._DetailForm.MainDataSet.Tables["D_ARConfirm"].Rows[0]["Rate"];
                    e.Cell.Row.Cells["Rate"].Value =Rate;
                    Amount = (decimal)e.Cell.Value;
                    e.Cell.Row.Cells["LCAmount"].Value = Math.Round(Amount * Rate, 2);
 
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
       
            try
            {

                decimal Amount = 0;
                decimal Rate = 0;
                decimal LCAmount;
                decimal DetailAmount = 0;
                decimal TotalDetailAmount =0;


                
                Rate = (decimal)this._DetailForm.MainDataSet.Tables["D_ARConfirm"].Rows[0]["Rate"];

                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_ARConfirmItems"].Rows)
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DetailAmount = (decimal)dr["Amount"];
                        dr["rate"] = Rate;
                        dr["LCAmount"] = DetailAmount * Rate;
                        TotalDetailAmount += DetailAmount;
                    }

                Amount = (decimal)this._DetailForm.MainDataSet.Tables["D_ARConfirm"].Rows[0]["Amount"];

                if ( (TotalDetailAmount != Amount) & (TotalDetailAmount !=0) )
                    return "收款确认明细的汇总金额银行收款信息中的金额不一致，请重新输入！";


                if (TotalDetailAmount == Amount)
                {
                    LCAmount = Amount * Rate;
                    _ControlMap["LCAmount"].Text = LCAmount.ToString();
                    _ControlMap["ConfirmStatus"].Text = string.Format("是");
                    this._DetailForm.MainDataSet.Tables["D_ARConfirm"].Rows[0]["ConfirmStatus"]=1;
                }
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

            try
                {
                    string Filter;
                    string FormTitle;

                    _mdiForm = mdiForm;

                    if (right.Paramters == null)
                        return null;

                    string[] s = right.Paramters.Split(new char[] { ',' });
                    if (s.Length > 0)
                        RevenueType = int.Parse(s[0]);
                    else
                        return null;
                    
                    

                    this.MainTableName = "D_ARConfirm";
                    SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                    SortedList<string, object> Value = new SortedList<string, object>();
                    Value.Add("DocumentDate", CSystem.Sys.Svr.SystemTime.Date);
                    defaultValue.Add("D_ARConfirm", Value);
                

                    COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                    //List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);

                    switch (RevenueType)
                    {
                        case 0:
                            FormTitle = "收款确认";
                            Filter = string.Format("D_ARConfirm.RevenueType={0}", (int)RevenueType);
                            break;
                        case 1:
                            FormTitle = "收入明细";
                            Filter = string.Empty;
                            break;
                        default:
                            FormTitle = "收入明细";
                            Filter = string.Empty;
                            break;
                    }

                    mainTableDefine.Property.Title = string.Format(FormTitle);
                    this._MainCOMFields = mainTableDefine;
                    //this._DetailCOMFields = detailTableDefines;

                    frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm, Filter);
                    frm.DefaultValue = defaultValue;
                    return frm;
                }
                catch (Exception ex)
                {
                    Msg.Warning(string.Format("加载失败！%n {0}", ex.Message));
                }
                return null;

            }
    }

}
