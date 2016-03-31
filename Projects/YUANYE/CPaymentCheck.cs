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
    public class CPaymentCheck : ToolDetailForm
    {

        private Form _mdiForm;
        private ToolStripButton toolVerify;

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
                return false;
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
            string PayeeAccountNumber;
            string[] s = e.Field.ValueType.Split(':');
            CCreateGrid CreatedGrid = (CCreateGrid)sender;
            UltraGridRow ActiveRow = CreatedGrid.Grid.ActiveCell.Row;
            // 1.根据收款人的账号和付款金额两个条件来同时进行过滤
            // 2.如果没有找到对应的记录,可能此付款是合并多个付款申请后的转账交易，这时候再尝试根据收款人的账号来进行过滤
            // 3.为减少返回记录的数量,在返回结果中剔除已经核对完成的付款申请

            if (this._DetailForm.MainDataSet.Tables["D_PaymentCheck"].Rows[0]["PayeeAccountNumber"] != DBNull.Value)
            {
                PayeeAccountNumber = (string)this._DetailForm.MainDataSet.Tables["D_PaymentCheck"].Rows[0]["PayeeAccountNumber"];
            }
            else
            {
                PayeeAccountNumber = "";
            }

            PayeeAccountNumber = PayeeAccountNumber.Trim();
             
            if (PayeeAccountNumber.Trim() == string.Empty)
            {
                e.Where = string.Format(" 0=1 ");
            }
            else
            {
                if (ActiveRow != null)
                {
                    e.Where = string.Format("D_Payment.Vendor in (Select ID from P_Vendor where BankAccount like'%{0}%')", PayeeAccountNumber);
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

        }

        void grid_AfterCellUpdate(object sender, CellEventArgs e)
        {   
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
                decimal DetailAmount = 0;
                decimal TotalDetailAmount =0;

                foreach (DataRow dr in this._DetailForm.MainDataSet.Tables["D_PaymentCheckItems"].Rows)
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DetailAmount = (decimal)dr["Amount"];
                        TotalDetailAmount += DetailAmount;
                    }

                Amount = (decimal)this._DetailForm.MainDataSet.Tables["D_PaymentCheck"].Rows[0]["TradeAmount"];

                if ( (TotalDetailAmount != Amount) & (TotalDetailAmount !=0) )
                    return "银行转账金额与对应的付款申请单汇总金额不一致，请重新确认！";


                if (TotalDetailAmount == Amount)
                {
                    _ControlMap["ConfirmStatus"].Text = string.Format("是");
                    this._DetailForm.MainDataSet.Tables["D_PaymentCheck"].Rows[0]["ConfirmStatus"] = 1;
                }
                return "";


            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public override void InsertToolStrip(ToolStrip toolStrip, ToolDetailForm.enuInsertToolStripType insertType)
        {
            base.InsertToolStrip(toolStrip, insertType);
            int i = toolStrip.Items.Count;

            i = i - 2;
            toolVerify = new ToolStripButton();
            toolVerify.Font = new System.Drawing.Font("SimSun", 10.5F);
            toolVerify.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolVerify.Name = "toolVerify";
            toolVerify.Size = new System.Drawing.Size(39, 34);
            toolVerify.Text = "对账";
            toolVerify.Image = UI.clsResources.Preview;
            toolVerify.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolVerify.Click += new EventHandler(toolVerify_Click);
            toolStrip.Items.Insert(i, toolVerify);

        }

        private void toolVerify_Click(object sender, EventArgs e)
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
                    //string Filter;
                    string FormTitle;

                    _mdiForm = mdiForm;

                    this.MainTableName = "D_PaymentCheck";
                    SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                    SortedList<string, object> Value = new SortedList<string, object>();
                    defaultValue.Add("D_PaymentCheck", Value);
                

                    COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                    FormTitle = "付款确认";
                    //Filter = string.Format("D_ARConfirm.RevenueType={0}", (int)RevenueType);

                    mainTableDefine.Property.Title = string.Format(FormTitle);
                    this._MainCOMFields = mainTableDefine;
                    frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
                    frm.DefaultValue = defaultValue;
                    return frm;
                }
                catch (Exception ex)
                {
                    Msg.Warning(string.Format("银行付款明细确认程序加载失败！%n {0}", ex.Message));
                }
                return null;

            }
    }

}
