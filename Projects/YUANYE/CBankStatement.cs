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
using System.Collections;


namespace YUANYE
{
    public class CBankStatement : ToolDetailForm
    {

        private Form _mdiForm;
        private string AccountNo; 
        private DateTime DateRangeLow;
	    private DateTime DateRangeHigh;
        private int TotalNumber;
	    private int NumberofDebited;
	    private decimal DebitedAmount;
	    private int NumberofCredited;
	    private decimal CreditedAmount;
        private int ConfirmStatus;


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
            Guid BankStatementID;

            DictionaryEntry ParamAccountNo;
            DictionaryEntry ParamDateRangeLow;
            DictionaryEntry ParamDateRangeHigh;

            int TotalNumber_Actual=0;
            int NumberofDebited_Actual =0;
            decimal DebitedAmount_Actual =0;
            int NumberofCredited_Actual =0;
            decimal CreditedAmount_Actual =0;
            string DCSign;

            if (this._BrowserForm.grid.ActiveRow == null)
                return;

            if (_BrowserForm.grid.Selected.Rows.Count == 0 && _BrowserForm.grid.ActiveRow != null)
                _BrowserForm.grid.ActiveRow.Selected = true;

            DataTable dt = _BrowserForm.MainDataSet.Tables[0].Clone();

            UltraGridRow SelectedRow = _BrowserForm.grid.Selected.Rows[0];
            BankStatementID = (Guid)SelectedRow.Cells["ID"].Value;
            this.AccountNo =(string)SelectedRow.Cells["AccountNo"].Value;
            this.DateRangeLow= (DateTime)SelectedRow.Cells["DateRangeLow"].Value;
            this.DateRangeHigh= (DateTime)SelectedRow.Cells["DateRangeHigh"].Value;

             //ParamAccountNo = new DictionaryEntry("AccountNo",string.Format("'{0}%'",this.AccountNo.Trim()));
             //ParamDateRangeLow = new DictionaryEntry("DateRangeLow", string.Format("'{0}'",this.DateRangeLow.ToShortDateString()));
             //ParamDateRangeHigh = new DictionaryEntry("DateRangeHigh",string.Format("'{0}'",this.DateRangeHigh.ToShortDateString()));
            ParamAccountNo = new DictionaryEntry("AccountNo", string.Format("{0}%", this.AccountNo.Trim()));
             ParamDateRangeLow = new DictionaryEntry("DateRangeLow",  this.DateRangeLow.ToShortDateString());
             ParamDateRangeHigh = new DictionaryEntry("DateRangeHigh",this.DateRangeHigh.ToShortDateString());

             DataSet dsBankTransaction = CSystem.Sys.Svr.cntMain.StoreProcedure("SP_D_BankTransaction_Statement", "D_BankStatement_Check", ParamAccountNo, ParamDateRangeLow, ParamDateRangeHigh);
             foreach (DataRow DRTransaction in dsBankTransaction.Tables["D_BankStatement_Check"].Rows)
             {
                 DCSign = (string)DRTransaction["DCSign"];
                 TotalNumber_Actual += (int)DRTransaction["Number"];

                 if (DCSign == "Debit")
                 {
                     NumberofDebited_Actual = (int)DRTransaction["Number"];
                     DebitedAmount_Actual = (decimal)DRTransaction["Amount"];
 
                 }
                 else
                 {
                     NumberofCredited_Actual = (int)DRTransaction["Number"];
                     CreditedAmount_Actual = (decimal)DRTransaction["Amount"];
                 }
             }

             if ((NumberofCredited_Actual == this.NumberofCredited) && (CreditedAmount_Actual == this.CreditedAmount))
             {
                 if ((NumberofDebited_Actual == this.NumberofDebited) && (DebitedAmount_Actual == this.DebitedAmount))
                 {

                 }
                 else
                 {

                 }
             }
             else
             { 

             }



            //        if (dt.Rows.Count > 0)
            //        {
            //            cntMain.Update(dt, conn, sqlTran);
            //            _MainDataSet.Merge(dt);
            //            RefreshCheckStatus();
            //        }

            //        sqlTran.Commit();
            //    }
            //    catch (Exception ex)
            //    {
            //        Msg.Warning(ex.ToString());
            //        sqlTran.Rollback();
            //    }
            //    finally
            //    {
            //        conn.Close();
            //    }
            //    if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
            //    {
            //        toolRefrash_Click(this, EventArgs.Empty);
            //        RefreshCheckStatus();
            //    }
            //}
            //if (PrimaryKey != null)
            //{
            //    foreach (UltraGridRow row in this.grid.Rows)
            //    {
            //        if ((Guid)row.Cells[PrimaryKey].Value == MainID)
            //        {
            //            row.Activated = true;
            //            return;
            //        }
            //    }
            //}

            //try
            //{
 
                 
            //}
            //catch (Exception ex)
            //{
            //    Msg.Warning(string.Format("银行交易对账程序执行错误！具体错误信息如下%n {0}", ex.Message));
            //}
            
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

                    this.MainTableName = "D_BankStatement";
                    SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                    SortedList<string, object> Value = new SortedList<string, object>();
                    defaultValue.Add("D_BankStatement", Value);
                

                    COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                    FormTitle = "银行交易核对";
                

                    mainTableDefine.Property.Title = string.Format(FormTitle);
                    this._MainCOMFields = mainTableDefine;
                    frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
                    frm.DefaultValue = defaultValue;
                    return frm;
                }
                catch (Exception ex)
                {
                    Msg.Warning(string.Format("银行交易核对程序加载失败！%n {0}", ex.Message));
                }
                return null;

            }
    }

}
