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


    public class clsAccountReceivable : ToolDetailForm
    {

        private ToolStripButton toolPrint;
        private ToolStripButton toolARCleaning;

        private Form _mdiForm;
        private int SalesOrderType;

        private DataSet _MainDataSet;
        private COMFields _MainTableDefine;
        private ToolDetailForm _ToolDetailForm;
        private List<COMFields> _DetailTableDefine;


        public override bool AutoCode
        {
            get
            {
                return false;
            }
        }
        //public override string GetPrefix()
        //{
  
        //}
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

        public override void SetCreateDetail(CCreateDetailForm createDetailForm)
        {
            base.SetCreateDetail(createDetailForm);
            createDetailForm.BeforeSelectForm += new BeforeSelectFormEventHandler(createDetailForm_BeforeSelectForm);
            //createDetailForm.AfterSelectForm += new AfterSelectFormEventHandler(createDetailForm_AfterSelectForm);
        }


        void createDetailForm_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            string[] s = e.Field.ValueType.Split(':');
            Guid Customer;


            if (s[1] == "D_StorageOut")
            {
                if (_DetailForm.MainDataSet.Tables["D_SOA"].Rows[0]["Customer"].ToString().Length > 0)
                    Customer = (Guid)this._DetailForm.MainDataSet.Tables["D_SOA"].Rows[0]["Customer"];
                else
                    Customer = Guid.Empty;

                e.Where = string.Format("D_StorageOut.Customer = '{0}'", Customer);
            }
        }


        void _CreateGrid_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {

        }

        void _CreateGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
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

            //i = i - 2;
            //toolPreview = new ToolStripButton();
            //toolPreview.Font = new System.Drawing.Font("SimSun", 10.5F);
            //toolPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            //toolPreview.Name = "toolPreview";
            //toolPreview.Size = new System.Drawing.Size(39, 34);
            //toolPreview.Text = "打印";
            //toolPreview.Image = UI.clsResources.Preview;
            //toolPreview.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            //toolPreview.Click += new EventHandler(toolPrint_Click);
            //toolStrip.Items.Insert(i, toolPreview);

            i = i - 2;
            toolARCleaning = new ToolStripButton();
            toolARCleaning.Font = new System.Drawing.Font("SimSun", 10.5F);
            toolARCleaning.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolARCleaning.Name = "toolARCleared";
            toolARCleaning.Size = new System.Drawing.Size(39, 34);
            toolARCleaning.Text = "已清账";
            toolARCleaning.Image = UI.clsResources.Preview;
            toolARCleaning.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolARCleaning.Click += new EventHandler(toolARCleared_Click);
            toolStrip.Items.Insert(i, toolARCleaning);

        }

        public override string BeforeSaving(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                Guid StorageOutID;
                decimal InvoiceAmount = 0;
                decimal Balance = 0;
                decimal TransferCharge = 0;
                decimal DeductMoney = 0;
                decimal Rate = 0;
                decimal LCAmount = 0;
                decimal ChargeLimit =0 ;
                decimal DeductLimit = 0;
               
               

                

                StorageOutID = (Guid)this._DetailForm.MainDataSet.Tables["V_AccountReceivable"].Rows[0]["ID"];
                TransferCharge = (decimal)this._DetailForm.MainDataSet.Tables["V_AccountReceivable"].Rows[0]["TransferCharge"];
                DeductMoney = (decimal)this._DetailForm.MainDataSet.Tables["V_AccountReceivable"].Rows[0]["DeductMoney"];
                InvoiceAmount = (decimal)this._DetailForm.MainDataSet.Tables["V_AccountReceivable"].Rows[0]["InvoiceAmount"];
                Balance = (decimal)this._DetailForm.MainDataSet.Tables["V_AccountReceivable"].Rows[0]["Balance"];
                ChargeLimit = InvoiceAmount/100;
                DeductLimit = InvoiceAmount / 100;

                //if ((TransferCharge > 50) || (TransferCharge > ChargeLimit))
                //    return "转账手续费超过允许数额的上限（大于发票金额的百分之一或者大于50美元),请重新确认！";


                //if (Balance != (DeductMoney + TransferCharge ))
                //    return "清账后应收账款的余额不为0，请重新输入！";


                string SQL = string.Format("Delete D_ARCleaning Where StorageOut = '{0}' ", StorageOutID);
                CSystem.Sys.Svr.cntMain.Excute(string.Format(SQL));
                
                SQL = string.Format("Insert into D_ARCleaning (ID,StorageOut,TransferCharge ,DeductMoney ,LCAmount ,Rate) Values ('{0}','{1}','{2}','{3}','{4}','{5}') ", StorageOutID, StorageOutID, TransferCharge, DeductMoney, LCAmount, Rate);
                CSystem.Sys.Svr.cntMain.Excute(string.Format(SQL));

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
 

        public override string  Saving(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
 	         return base.Saving(ds, conn, tran);
        }
    
        private void toolPrint_Click(object sender, EventArgs e)
        {
            //PrintNew();
        }

        private void toolARCleared_Click(object sender, EventArgs e)
        {
            
        }

        public override Form GetForm(CRightItem right, Form mdiForm)
        {

            try
            {
                string Filter;
                string FormTitle;

                _mdiForm = mdiForm;

                //if (right.Paramters == null)
                //    return null;
                //string[] s = right.Paramters.Split(new char[] { ',' });
                //if (s.Length > 0)
                //    SalesOrderType = int.Parse(s[0]);
                //else
                //    return null;

                this.MainTableName = "V_AccountReceivable";
                //Filter = string.Format("V_AccountReceivable.DocumentStatus =0 ");
                Filter = string.Format("V_AccountReceivable.Balance > 0 and DocumentStatus =0 ");

                SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                SortedList<string, object> Value = new SortedList<string, object>();
                defaultValue.Add("V_AccountReceivable", Value);

                COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);
                FormTitle = "应收账款";
                mainTableDefine.Property.Title = string.Format(FormTitle);
                this._MainCOMFields = mainTableDefine;
                this._DetailCOMFields = detailTableDefines;

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
