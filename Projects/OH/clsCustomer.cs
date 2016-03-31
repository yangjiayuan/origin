using System;
using System.Collections.Generic;
using System.Text;
using Base;
using UI;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using System.Data;

namespace OH
{
    public class clsCustomer : ToolDetailForm
    {
        //�½��汾��ѡ��汾������Ĭ�ϰ汾
        private ToolStripButton toolNewVersion;
        private ToolStripButton toolSelectVersion;
        private ToolStripButton toolSetDefaultVersion;
        private ToolStripButton toolCopyVersion;

        public override bool AutoCode
        {
            get
            {
                return true;
            }
            set
            {
                base.AutoCode = value;
            }
        }
        public override bool DateInCode
        {
            get
            {
                return false;
            }
            set
            {
                base.DateInCode = value;
            }
        }
        public override string GetPrefix()
        {
            return base.GetPrefix();
        }
        public override void InsertToolStrip(System.Windows.Forms.ToolStrip toolStrip, ToolDetailForm.enuInsertToolStripType insertType)
        {
            int i = toolStrip.Items.Count-1;

            if (insertType == enuInsertToolStripType.Browser)
            {
                toolNewVersion = new ToolStripButton();
                toolNewVersion.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolNewVersion.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolNewVersion.Name = "toolNewVersion";
                toolNewVersion.Size = new System.Drawing.Size(39, 34);
                toolNewVersion.Text = "�½��汾";

                toolNewVersion.Image = global::OH.Properties.Resources.New2;
                toolNewVersion.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolNewVersion.Click += new EventHandler(toolNewVersion_Click);
                toolStrip.Items.Insert(i++, toolNewVersion);


                toolSelectVersion = new ToolStripButton();
                toolSelectVersion.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolSelectVersion.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolSelectVersion.Name = "toolSelectVersion";
                toolSelectVersion.Size = new System.Drawing.Size(39, 34);
                toolSelectVersion.Text = "ѡ��汾";

                toolSelectVersion.Image = global::OH.Properties.Resources.Select;
                toolSelectVersion.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolSelectVersion.Click += new EventHandler(toolSelectVersion_Click);
                toolStrip.Items.Insert(i++, toolSelectVersion);


                toolSetDefaultVersion = new ToolStripButton();
                toolSetDefaultVersion.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolSetDefaultVersion.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolSetDefaultVersion.Name = "toolSetDefaultVersion";
                toolSetDefaultVersion.Size = new System.Drawing.Size(39, 34);
                toolSetDefaultVersion.Text = "����Ĭ�ϰ汾";

                toolSetDefaultVersion.Image = global::OH.Properties.Resources.Set;
                toolSetDefaultVersion.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolSetDefaultVersion.Click += new EventHandler(toolSetDefaultVersion_Click);
                toolStrip.Items.Insert(i++, toolSetDefaultVersion);

                toolCopyVersion = new ToolStripButton();
                toolCopyVersion.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolCopyVersion.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolCopyVersion.Name = "toolCopyVersion";
                toolCopyVersion.Size = new System.Drawing.Size(39, 34);
                toolCopyVersion.Text = "���ư汾";

                toolCopyVersion.Image = global::OH.Properties.Resources.Copy;
                toolCopyVersion.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolCopyVersion.Click += new EventHandler(toolCopyVersion_Click);
                toolStrip.Items.Insert(i++, toolCopyVersion);

                ToolStripSeparator tss = new ToolStripSeparator();
                toolStrip.Items.Insert(i++, tss);
            }
        }
        private void toolCopyVersion_Click(object sender, EventArgs e)
        {
            //ѡ��ԭ�汾
            //��Ч�ڣ���ԭ�����յ�֮������죻���ÿ�ʼ����������
        }
        private void toolNewVersion_Click(object sender, EventArgs e)
        {
            UltraGridRow row = this._BrowserForm.grid.ActiveRow;
            if (row!=null)
            {
                QuoteVersion version = new QuoteVersion();
                if (version.ShowDialog() == DialogResult.OK)
                {
                    string notes = version._GetVersion;
                    Boolean defaultVserion = version._GetDefaultVersion;
                    frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("P_Quote"), CSystem.Sys.Svr.Properties.DetailTableDefineList("P_Quote"), "1=0", enuShowStatus.None);
                    clsQuote cls = new clsQuote();
                    cls.CustomerID = (Guid)row.Cells["ID"].Value;
                    cls.CustomerName = (string)row.Cells["Name"].Value;
                    cls.QuoteVersionNote = notes;
                    cls.NewVerion = defaultVserion;
                    cls.StartDate = version._startDate;
                    cls.EndDate = version._endDate;
                    Guid id = Guid.NewGuid();
                    cls.QuoteVersionID = id;
                    frm.toolDetailForm = cls;
                    this.MainTableName = "P_Quote";
                    SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                    SortedList<string, object> Value = new SortedList<string, object>();
                    Value.Add("QuoteVersionNotes", notes);
                    Value.Add("QuoteVersionID", id);
                    defaultValue.Add(this.MainTableName, Value);
                    frm.DefaultValue = defaultValue;
                    string text = string.Format("{0}[{1}]", "����ͼ��", (string)row.Cells["Name"].Value);
                    frm.Text = text;
                    frm.MdiParent = this._BrowserForm.MdiParent;
                    frm.Show();
                    if (defaultVserion)
                    {
                        CSystem.Sys.Svr.cntMain.Excute(string.Format("Update P_Customer set QuoteVersionID='{0}' where ID='{1}'", id, row.Cells["ID"].Value));
                        row.Cells["QuoteVersionID"].Value = id;
                        row.Cells["QuoteVersion"].Value = notes;
                    }
                }

                //string notes = Microsoft.VisualBasic.Interaction.InputBox("������汾˵��(Ĭ����������Ϊ��ǰ�汾)", "ϵͳ", "", 200, 100);
                //if (notes.Length > 0)
                //{
                //    frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("P_Quote"), CSystem.Sys.Svr.Properties.DetailTableDefineList("P_Quote"), "1=0", enuShowStatus.None);
                //    clsQuote cls = new clsQuote();
                //    cls.CustomerID = (Guid)row.Cells["ID"].Value;
                //    cls.CustomerName = (string)row.Cells["Name"].Value;
                //    cls.QuoteVersionNote = notes;
                //    cls.NewVerion = true;
                //    Guid id = Guid.NewGuid();
                //    cls.QuoteVersionID =id;
                //    frm.toolDetailForm = cls;
                //    this.MainTableName = "P_Quote";
                //    SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                //    SortedList<string, object> Value = new SortedList<string, object>();
                //    Value.Add("QuoteVersionNotes", notes);
                //    Value.Add("QuoteVersionID",id );
                //    defaultValue.Add(this.MainTableName, Value);
                //    frm.DefaultValue = defaultValue;
                //    string text = string.Format("{0}[{1}]", "����ͼ��", (string)row.Cells["Name"].Value);
                //    frm.Text = text;
                //    frm.MdiParent = this._BrowserForm.MdiParent;
                //    frm.Show();
                //}
            }
        }

        private void toolSelectVersion_Click(object sender, EventArgs e)
        {
            UltraGridRow row = this._BrowserForm.grid.ActiveRow;
            if (row != null)
            {
                //����� ��
                frmBrowser frmVersion = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("P_QuoteVersion"), null, string.Format("P_QuoteVersion.MainID='{0}'", row.Cells["ID"].Value), enuShowStatus.None);
                DataRow dr = frmVersion.ShowSelect(null, null, null);
                if (dr != null)
                {
                    //�ٵ�������
                    frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("P_Quote"), CSystem.Sys.Svr.Properties.DetailTableDefineList("P_Quote"), string.Format("P_Quote.QuoteVersionID='{0}'", dr["ID"]), enuShowStatus.None);
                    clsQuote cls = new clsQuote();
                    cls.CustomerID = (Guid)row.Cells["ID"].Value;
                    cls.CustomerName = (string)row.Cells["Name"].Value;
                    cls.QuoteVersionNote = dr["Notes"] as string;
                    cls.QuoteVersionID = (Guid)dr["ID"];
                    frm.toolDetailForm = cls;
                    Guid id = (Guid)dr["ID"];
                    cls.QuoteVersionID = id;
                    this.MainTableName = "P_Quote";
                    SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                    SortedList<string, object> Value = new SortedList<string, object>();
                    Value.Add("QuoteVersionNotes", dr["Notes"] as string);
                    Value.Add("QuoteVersionID", id);
                    defaultValue.Add(this.MainTableName, Value);
                    frm.DefaultValue = defaultValue;
                    string text = string.Format("{0}[{1}]", "����ͼ��", (string)row.Cells["Name"].Value);
                    frm.Text = text;
                    frm.MdiParent = this._BrowserForm.MdiParent;
                    frm.Show();
                }
            }
        }
        public override string BeforeSaving(DataSet ds, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction tran)
        {
            string s = null;
            foreach (DataRow dr in ds.Tables[_MainTableName].Rows)
            {
                if (dr.RowState != DataRowState.Deleted && (dr["MaxQuantity"] == DBNull.Value || VarConverTo.ConvertToDecimal(dr["MaxQuantity"]) == 0))
                    s = s + " ÿ���������û�����룡";
                if (dr.RowState != DataRowState.Deleted && (dr["MaxWeight"] == DBNull.Value || VarConverTo.ConvertToDecimal(dr["MaxWeight"]) == 0))
                    s = s + " ÿ���������û�����룡";
            }
            return s;
        }
        private void toolSetDefaultVersion_Click(object sender, EventArgs e)
        {
            UltraGridRow row = this._BrowserForm.grid.ActiveRow;
            if (row != null)
            {
                //ѡ��汾����
                frmBrowser frmVersion = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("P_QuoteVersion"), null, string.Format("P_QuoteVersion.MainID='{0}'", row.Cells["ID"].Value), enuShowStatus.None);
                DataRow dr = frmVersion.ShowSelect(null, null, null);
                if (dr != null)
                {
                    CSystem.Sys.Svr.cntMain.Excute(string.Format("Update P_Customer set QuoteVersionID='{0}' where ID='{1}'", dr["ID"], row.Cells["ID"].Value));
                    row.Cells["QuoteVersionID"].Value = dr["ID"];
                    row.Cells["QuoteVersion"].Value = dr["Notes"];
                }
            }
        }


    }
}
