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
        private ToolStripButton toolPreview;

        private Form _mdiForm;
        private int SalesOrderType;

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

            i = i - 2;
            toolPreview = new ToolStripButton();
            toolPreview.Font = new System.Drawing.Font("SimSun", 10.5F);
            toolPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolPreview.Name = "toolPreview";
            toolPreview.Size = new System.Drawing.Size(39, 34);
            toolPreview.Text = "打印";
            toolPreview.Image = UI.clsResources.Preview;
            toolPreview.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolPreview.Click += new EventHandler(toolPrint_Click);
            toolStrip.Items.Insert(i, toolPreview);

        }

        private void toolPrint_Click(object sender, EventArgs e)
        {
            //PrintNew();
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
                Filter = string.Format("V_AccountReceivable.Balance > 0 and DocumentStatus =0 ");

                SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                SortedList<string, object> Value = new SortedList<string, object>();
                //Value.Add("DocumentDate", CSystem.Sys.Svr.SystemTime.Date);
                //Value.Add("OrderType", (int)SalesOrderType);

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
