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
using System.Reflection;


namespace YUANYE
{
    public class CTxnConsole: ToolDetailForm
    {
        private ToolStripButton toolRun;
        private ITransaction _Tranaction;
        private SortedList<string, Assembly> DLLFiles;
        private Form mymdiForm;

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
  
        public override void InsertToolStrip(System.Windows.Forms.ToolStrip toolStrip, ToolDetailForm.enuInsertToolStripType insertType)
        {
            base.InsertToolStrip(toolStrip, insertType);
            int i = toolStrip.Items.Count;

            toolRun = new ToolStripButton();
            toolRun.Font = new System.Drawing.Font("SimSun", 10.5F);
            toolRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolRun.Name = "toolRun";
            toolRun.Size = new System.Drawing.Size(39, 34);
            toolRun.Text = "运行";
            toolRun.Image = UI.clsResources.Finish;
            toolRun.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolRun.Click += new EventHandler(toolRun_Click);
            toolStrip.Items.Insert(i, toolRun);

        }


        private void toolRun_Click(object sender, EventArgs e)
        {
            string TransactionName;
            string TransactionProgram;
            Form TransactionWindow;
            
            
            TransactionName = (string) this._DetailForm.MainDataSet.Tables["S_Transaction"].Rows[0]["Name"];

            if (this._DetailForm.MainDataSet.Tables["S_Transaction"].Rows[0]["Action"] != DBNull.Value)
            {
                string ActionName = (string)this._DetailForm.MainDataSet.Tables["S_Transaction"].Rows[0]["Action"];
                string[] a = ActionName.Split(new char[] { '!' });

                try
                {
                    Assembly ass = GetAssembly(a[0]);
                    _Tranaction = ass.CreateInstance(a[1]) as ITransaction;
                    if (_Tranaction == null)
                        Msg.Warning(string.Format("事务代码({1})创建对象({0})不成功！", a[1], TransactionName));
                    else
                    {
                        TransactionWindow = _Tranaction.GetForm(TransactionName,mymdiForm);
                        //if (CheckFormTag(frm.Tag))
                        //{
                            TransactionWindow.MdiParent = mymdiForm;
                            TransactionWindow.Show();
                        //}
                    }
                }
                catch (Exception ex)
                {
                    Msg.Warning(string.Format("事务代码({1})创建对象({0})失败！\n {2}", a[1], TransactionName, ex.Message));
                }
            }
        }


        public Assembly GetAssembly(string name)
        {
            Assembly ass = null;
            if (DLLFiles.ContainsKey(name))
                ass = DLLFiles[name];
            else
            {
                string fileName = name;
                System.IO.FileInfo f = new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + fileName);
                if (f.Exists)
                {
                    try
                    {
                        ass = Assembly.LoadFile(f.FullName);
                        if (ass != null)
                        {
                            DLLFiles.Add(name, ass);
                        }
                        else
                        {
                            Msg.Warning(string.Format("定制库({0})装载失败！", name));
                        }
                    }
                    catch (Exception ex)
                    {
                        Msg.Warning(string.Format("定制库({0})装载失败！/r/d{1}", name, ex.ToString()));
                    }
                }
                else
                {
                    Msg.Warning(string.Format("定制库({0})不存在！", name));
                }
            }
            return ass;
        }

        public override bool UnCheck(Guid ID, SqlConnection conn, SqlTransaction sqlTran, DataRow mainRow)
        {
            return base.UnCheck(ID, conn, sqlTran, mainRow);
        }

    
 
        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            mymdiForm = mdiForm;
            frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
            DLLFiles = new SortedList<string, Assembly>();

            SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
            SortedList<string, object> Value = new SortedList<string, object>();
            defaultValue.Add("S_Transaction", Value);
            frm.DefaultValue = defaultValue;
            return frm;
        }
    }
}
