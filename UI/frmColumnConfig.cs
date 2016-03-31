using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Base;

namespace UI
{
    public partial class frmColumnConfig : Form
    {
        private const int TopValue = 10;
        private const int LeftValue = 10;
        private const int LabelWith = 70;
        private const int DataWith = 110;
        private const int SpaceWith = 20;
        private const int ControlHeight = 25;
        private const int X1 = 10;
        private const int X2 = 80;
        private const int X3 = 130;
        private const int X4 = 250;
        private const int X5 = 300;
        private const int Y1 = 0;
        private const int Y2 = 4;

        private string _Path;
        private COMFields _Fields;
        private COMFields _FieldsDetail;
        private UltraGrid _browserGrid;
        private DataTable _dtcolumns;
        private SortedList<string, CheckBox> _ControlMap1 = new SortedList<string, CheckBox>();
        private SortedList<string, ComboBox> _ControlMap2 = new SortedList<string, ComboBox>();
        private SortedList<string, Control> _ControlMap3 = new SortedList<string, Control>();
        private SortedList<string, ComboBox> _ControlMap4 = new SortedList<string, ComboBox>();
        private SortedList<string, Control> _ControlMap5 = new SortedList<string, Control>();
        public frmColumnConfig()
        {
            InitializeComponent();
        }
        public COMFields Fields
        {
            set { _Fields = value; }
        }
        public UltraGrid BrowserGrid
        {
            set { _browserGrid = value; }
        }
        public string Path
        {
            set{_Path=value;}
        }

        private void frmColumnConfig_Load(object sender, EventArgs e)
        {
            _dtcolumns = new DataTable();
            _dtcolumns.Columns.Add("LineNumber", typeof(int));
            _dtcolumns.Columns.Add("FieldName", typeof(string));
            _dtcolumns.Columns.Add("OldDescription", typeof(string));
            _dtcolumns.Columns.Add("NewDescription", typeof(string));
            _dtcolumns.Columns.Add("Width", typeof(int));
            _dtcolumns.Columns.Add("Hidden", typeof(bool));
            _dtcolumns.Columns.Add("OrderBy", typeof(int));

            int y = 4;
            for (int fi = 0; fi < 2; fi++)
            {
                COMFields fs = null;
                if (fi == 0)
                    fs = _Fields;
                else if (_FieldsDetail == null)
                    break;
                else
                    fs = _FieldsDetail;
                foreach (COMField f in fs.Fields)
                {
                    if ((f.Visible & COMField.Enum_Visible.VisibleInBrower) != COMField.Enum_Visible.VisibleInBrower) continue;

                    string fieldName = null;
                    if (fi == 1)
                        fieldName = "D_" + f.FieldName;
                    else
                        fieldName = f.FieldName;
                    //设置列的数据源
                    DataRow dr = _dtcolumns.NewRow();
                    dr["LineNumber"] = f.ColOrder;
                    dr["FieldName"] = fieldName;
                    dr["OldDescription"] = f.FieldTitle;
                    dr["NewDescription"] = f.FieldTitle;
                    dr["Width"] = 50;
                    dr["Hidden"] = false;
                    dr["OrderBy"] = 0;
                    _dtcolumns.Rows.Add(dr);

                    //设置条件
                    string[] s = f.ValueType.Split(':');
                    CheckBox chk = new CheckBox();
                    chk.Left = 10;
                    chk.Top = y + 4;
                    chk.AutoSize = true;
                    chk.CheckedChanged += new EventHandler(chk_CheckedChanged);
                    panelConfigTop.Controls.Add(chk);
                    _ControlMap1.Add(fieldName, chk);
                    Panel panelControl = new Panel();
                    panelControl.Left = 20;
                    panelControl.Top = y;
                    panelControl.Width = this.panelConfigTop.Width - 50;
                    panelControl.Height = ControlHeight;
                    panelControl.Enabled = false;
                    panelConfigTop.Controls.Add(panelControl);
                    chk.Tag = panelControl;
                    y += ControlHeight;
                    switch (s[0].ToLower())
                    {
                        case "string":
                            Label ls = new Label();
                            ls.Left = X1;
                            ls.Top = Y2;
                            ls.Width = LabelWith;
                            ls.Text = f.FieldTitle;
                            ls.AutoSize = true;
                            ls.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                            TextBox t = new TextBox();
                            t.Left = X2;
                            t.Top = Y1;
                            t.Width = DataWith;
                            panelControl.Controls.Add(t);
                            panelControl.Controls.Add(ls);

                            _ControlMap3.Add(fieldName, t);
                            break;
                        case "number":
                            Label ln = new Label();
                            ln.Left = X1;
                            ln.Top = Y2;
                            ln.Width = LabelWith;
                            ln.Text = f.FieldTitle;
                            ln.AutoSize = true;
                            ln.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            ComboBox cmbN1 = new ComboBox();
                            cmbN1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                            cmbN1.FormattingEnabled = true;
                            cmbN1.Items.AddRange(new object[] {
                                                        "=",
                                                        ">",
                                                        "<",
                                                        ">=",
                                                        "<=",
                                                        "<>"});
                            cmbN1.Location = new System.Drawing.Point(X2, Y1);
                            cmbN1.Size = new System.Drawing.Size(43, 20);

                            UltraCurrencyEditor nu1 = new UltraCurrencyEditor();
                            nu1.Left = X3;
                            nu1.Top = Y1;
                            nu1.Width = DataWith;
                            string[] d = s[1].Split(',');
                            nu1.FormatString = "#,##0." + new string('0', int.Parse(d[1]));
                            nu1.MaskInput = "nnn,nnn,nnn." + new string('n', int.Parse(d[1]));

                            ComboBox cmbN2 = new ComboBox();
                            cmbN2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                            cmbN2.FormattingEnabled = true;
                            cmbN2.Items.AddRange(new object[] {
                                                        "=",
                                                        ">",
                                                        "<",
                                                        ">=",
                                                        "<=",
                                                        "<>"});
                            cmbN2.Location = new System.Drawing.Point(X4, Y2);
                            cmbN2.Size = new System.Drawing.Size(43, 20);

                            UltraCurrencyEditor nu2 = new UltraCurrencyEditor();
                            nu2.Left = X5;
                            nu2.Top = Y2;
                            nu2.Width = DataWith;
                            nu2.FormatString = "#,##0." + new string('0', int.Parse(d[1]));
                            nu2.MaskInput = "nnn,nnn,nnn." + new string('n', int.Parse(d[1]));

                            panelControl.Controls.Add(ln);
                            panelControl.Controls.Add(cmbN1);
                            panelControl.Controls.Add(nu1);
                            panelControl.Controls.Add(cmbN2);
                            panelControl.Controls.Add(nu2);

                            _ControlMap2.Add(fieldName, cmbN1);
                            _ControlMap3.Add(fieldName, nu1);
                            _ControlMap4.Add(fieldName, cmbN2);
                            _ControlMap5.Add(fieldName, nu2);
                            break;
                        case "int":
                            Label li = new Label();
                            li.Left = X1;
                            li.Top = Y2;
                            li.Width = LabelWith;
                            li.Text = f.FieldTitle;
                            li.AutoSize = true;
                            li.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            ComboBox cmbI1 = new ComboBox();
                            cmbI1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                            cmbI1.FormattingEnabled = true;
                            cmbI1.Items.AddRange(new object[] {
                                                        "=",
                                                        ">",
                                                        "<",
                                                        ">=",
                                                        "<=",
                                                        "<>"});
                            cmbI1.Location = new System.Drawing.Point(X2, Y1);
                            cmbI1.Size = new System.Drawing.Size(43, 20);
                            NumericUpDown nud1 = new NumericUpDown();
                            nud1.Left = X3;
                            nud1.Top = Y1;
                            nud1.Width = DataWith;

                            ComboBox cmbI2 = new ComboBox();
                            cmbI2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                            cmbI2.FormattingEnabled = true;
                            cmbI2.Items.AddRange(new object[] {
                                                        "=",
                                                        ">",
                                                        "<",
                                                        ">=",
                                                        "<=",
                                                        "<>"});
                            cmbI2.Location = new System.Drawing.Point(X4, Y2);
                            cmbI2.Size = new System.Drawing.Size(43, 20);
                            NumericUpDown nud2 = new NumericUpDown();
                            nud2.Left = X5;
                            nud2.Top = Y2;
                            nud2.Width = DataWith;

                            panelControl.Controls.Add(li);
                            panelControl.Controls.Add(cmbI1);
                            panelControl.Controls.Add(nud1);
                            panelControl.Controls.Add(cmbI2);
                            panelControl.Controls.Add(nud2);

                            _ControlMap2.Add(fieldName, cmbI1);
                            _ControlMap3.Add(fieldName, nud1);
                            _ControlMap4.Add(fieldName, cmbI2);
                            _ControlMap5.Add(fieldName, nud2);
                            break;
                        case "enum":
                            Label le = new Label();
                            le.Left = X1;
                            le.Top = Y2;
                            le.Width = LabelWith;
                            le.Text = f.FieldTitle;
                            le.AutoSize = true;
                            le.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                            ComboBox cmbOpt1 = new ComboBox();
                            cmbOpt1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                            cmbOpt1.FormattingEnabled = true;
                            cmbOpt1.Items.AddRange(new object[] {
                                                        "=",
                                                        ">",
                                                        "<",
                                                        ">=",
                                                        "<=",
                                                        "<>"});
                            cmbOpt1.Location = new System.Drawing.Point(X2, Y1);
                            cmbOpt1.Size = new System.Drawing.Size(43, 20);

                            ComboBox cmbOpt2 = new ComboBox();
                            cmbOpt2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                            cmbOpt2.FormattingEnabled = true;
                            cmbOpt2.Items.AddRange(new object[] {
                                                        "=",
                                                        ">",
                                                        "<",
                                                        ">=",
                                                        "<=",
                                                        "<>"});
                            cmbOpt2.Location = new System.Drawing.Point(X4, Y2);
                            cmbOpt2.Size = new System.Drawing.Size(43, 20);

                            ComboBox cb1 = new ComboBox();
                            cb1.Left = X3;
                            cb1.Top = Y1;
                            cb1.Width = DataWith;
                            ComboBox cb2 = new ComboBox();
                            cb2.Left = X5;
                            cb2.Top = Y2;
                            cb2.Width = DataWith;

                            string[] enu = s[1].Split(',');
                            for (int i = 0; i < enu.Length; i++)
                            {
                                string[] v = enu[i].Split('=');
                                if (v.Length >= 2)
                                {
                                    IntItem iItem = new IntItem(int.Parse(v[0]), v[1]);
                                    cb1.Items.Add(iItem);
                                    cb2.Items.Add(iItem);
                                }
                            }

                            cb1.SelectedIndexChanged += new EventHandler(cmb_SelectedIndexChanged);
                            cb2.SelectedIndexChanged += new EventHandler(cmb_SelectedIndexChanged);
                            panelControl.Controls.Add(le);
                            panelControl.Controls.Add(cmbOpt1);
                            panelControl.Controls.Add(cb1);
                            panelControl.Controls.Add(cmbOpt2);
                            panelControl.Controls.Add(cb2);

                            _ControlMap2.Add(fieldName, cmbOpt1);
                            _ControlMap3.Add(fieldName, cb1);
                            _ControlMap4.Add(fieldName, cmbOpt2);
                            _ControlMap5.Add(fieldName, cb2);
                            break;
                        case "boolean":
                            CheckBox chkBox = new CheckBox();
                            chkBox.Text = f.FieldTitle;
                            chkBox.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            chkBox.Left = X1;
                            chkBox.Top = Y1;
                            chkBox.Width = DataWith + LabelWith;

                            panelControl.Controls.Add(chkBox);
                            _ControlMap3.Add(fieldName, chkBox);
                            break;
                        case "dict":
                        case "data":
                            Label ld = new Label();
                            ld.Left = X1;
                            ld.Top = Y2;
                            ld.Width = LabelWith;
                            ld.Text = f.FieldTitle;
                            ld.AutoSize = true;
                            ld.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                            UltraTextEditor ute = new UltraTextEditor();
                            ute.Left = X2;
                            ute.Top = Y1;
                            ute.Width = DataWith;
                            EditorButton editButton = new EditorButton();
                            ute.ButtonsRight.Add(editButton);
                            ute.EditorButtonClick += new EditorButtonEventHandler(Dict_EditorButtonClick);
                            ute.Tag = f;
                            panelControl.Controls.Add(ute);
                            panelControl.Controls.Add(ld);
                            _ControlMap3.Add(fieldName, ute);
                            break;
                        case "date":
                            Label ldate = new Label();
                            ldate.Left = X1;
                            ldate.Top = Y2;
                            ldate.Width = LabelWith;
                            ldate.Text = f.FieldTitle;
                            ldate.AutoSize = true;
                            ldate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            ComboBox cmbd1 = new ComboBox();
                            cmbd1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                            cmbd1.FormattingEnabled = true;
                            cmbd1.Items.AddRange(new object[] {
                                                        "=",
                                                        ">",
                                                        "<",
                                                        ">=",
                                                        "<=",
                                                        "<>"});
                            cmbd1.Location = new System.Drawing.Point(X2, Y1);
                            cmbd1.Size = new System.Drawing.Size(43, 20);

                            UltraDateTimeEditor udt1 = new UltraDateTimeEditor();
                            udt1.Left = X3;
                            udt1.Top = Y1;
                            udt1.Width = DataWith;

                            ComboBox cmbd2 = new ComboBox();
                            cmbd2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                            cmbd2.FormattingEnabled = true;
                            cmbd2.Items.AddRange(new object[] {
                                                        "=",
                                                        ">",
                                                        "<",
                                                        ">=",
                                                        "<=",
                                                        "<>"});
                            cmbd2.Location = new System.Drawing.Point(X4, Y2);
                            cmbd2.Size = new System.Drawing.Size(43, 20);

                            UltraDateTimeEditor udt2 = new UltraDateTimeEditor();
                            udt2.Left = X5;
                            udt2.Top = Y2;
                            udt2.Width = DataWith;

                            panelControl.Controls.Add(ldate);
                            panelControl.Controls.Add(cmbd1);
                            panelControl.Controls.Add(udt1);
                            panelControl.Controls.Add(cmbd2);
                            panelControl.Controls.Add(udt2);

                            _ControlMap2.Add(fieldName, cmbd1);
                            _ControlMap3.Add(fieldName, udt1);
                            _ControlMap4.Add(fieldName, cmbd2);
                            _ControlMap5.Add(fieldName, udt2);
                            break;
                    }
                }
            }
            gridColumn.SetDataBinding(_dtcolumns, null, true);
            Infragistics.Win.ValueList list = gridColumn.DisplayLayout.ValueLists.Add("VL_OrderBy");
            list.ValueListItems.Add(1, "升序");
            list.ValueListItems.Add(-1, "降序");
            list.ValueListItems.Add(0, "　");
            gridColumn.DisplayLayout.Bands[0].Columns["Orderby"].ValueList = list;

            //读取指定目录里的文件名
            if (CheckPath(_Path))
            {
                listConfig.Columns.Add("名称", listConfig.Width);
                listConfig.Columns.Add("Tag", 0);

                bool hasDefalut = false;
                string[] files = Directory.GetFiles(_Path,"*.xml",SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    ListViewItem lvi = new ListViewItem();
                    string fileName = files[i].Substring(files[i].LastIndexOf('\\') + 1);
                    int j = fileName.IndexOf('_');
                    if (fileName.EndsWith(".default.xml"))
                    {
                        lvi.SubItems[0].Text = fileName.Substring(j + 1, fileName.Length - ".default.xml".Length - j - 1);
                        lvi.SubItems.Add(fileName);
                        lvi.Font = new Font(lvi.Font, FontStyle.Bold);
                        listConfig.Items.Add(lvi);
                        lvi.Selected = true;
                        hasDefalut = false;
                    }
                    else if (fileName.EndsWith(".xml"))
                    {
                        lvi.SubItems[0].Text = fileName.Substring(j + 1, fileName.Length - ".xml".Length - j - 1);
                        lvi.SubItems.Add(fileName);
                        listConfig.Items.Add(lvi);
                    }
                }
                if (!hasDefalut)
                    butGetWidth_Click(null, EventArgs.Empty);
            }
        }
        private bool CheckPath(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    return true;
                else
                {
                    int i = path.LastIndexOf('\\');
                    if (i > -1)
                    {
                        if (CheckPath(path.Substring(0, i)))
                            Directory.CreateDirectory( path);
                        else
                            return false;
                    }
                    else
                        Directory.CreateDirectory(path);
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }
        void chk_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            Panel pan = (Panel)chk.Tag;
            pan.Enabled = chk.Checked;
        }
        private void cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCmbTagByInt(sender);
        }
        public static void SetCmbItemByInt(ComboBox cmb, int i)
        {
            for (int j = 0; j < cmb.Items.Count; j++)
            {
                IntItem item = (IntItem)cmb.Items[j];
                if (item.Int == i)
                {
                    cmb.SelectedIndex = j;
                    cmb.Tag = i;
                    return;
                }
            }
        }
        public static int SetCmbTagByInt(object sender)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb != null)
            {
                int i = cmb.SelectedIndex;
                if (i > -1)
                {
                    IntItem item = (IntItem)cmb.Items[i];
                    cmb.Tag = item.Int;
                    return item.Int;
                }
            }
            return -1;
        }

        void Dict_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            UltraTextEditor txt = (UltraTextEditor)sender;
            COMField field = (COMField)txt.Tag;
            string[] s = field.ValueType.Split(':');
            List<COMFields> detailTable = CSystem.Sys.Svr.Properties.DetailTableDefineList(s[1]);
            string where = "";

            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields(s[1]), detailTable, where, enuShowStatus.None);
            string other;
            if (s[0].ToLower() == "dict")
                other = s[1] + ".Disable=0";
            else
                other = "";

            DataRow dr = frm.ShowSelect("", "", other);
            if (dr != null)
            {
                txt.Text = dr[field.RFieldName] as string;
            }
        }

        private void butGetWidth_Click(object sender, EventArgs e)
        {
            foreach (UltraGridColumn col in _browserGrid.DisplayLayout.Bands[0].Columns)
            {
                DataRow[] drs = _dtcolumns.Select("FieldName='"+col.Key+"'");
                if (drs.Length == 1)
                {
                    drs[0]["Width"] = col.Width;
                }
            }
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            string fileName = _Path;
            if (listConfig.SelectedItems.Count == 1)
            {
                ListViewItem lvi = listConfig.SelectedItems[0];
                fileName += lvi.SubItems[1].Text;
            }
            else
            {
                bool exists = true;
                string Name = null;
                while (exists)
                {
                    Name = (new frmAskName()).GetName();
                    if (Name == null)
                        return;
                    //检查名称是否已使用
                    exists = false;
                    foreach (ListViewItem l in listConfig.Items)
                        if (l.SubItems[0].Text == Name)
                        {
                            exists = true;
                            break;
                        }
                }
                ListViewItem lvi = new ListViewItem(Name);
                Name = string.Format("{0}_{1}.xml", listConfig.Items.Count, Name);
                lvi.SubItems.Add(Name);
                listConfig.Items.Add(lvi);

                fileName += Name;
            }
            clsListConfig config = GetListConfig(fileName);
            //保存
            config.Save();
        }
        private clsListConfig GetListConfig(string fileName)
        {
            //构造出真实的文件名
            clsListConfig config = new clsListConfig(fileName,true);
            //保存列
            gridColumn.UpdateData();
            StringBuilder sbOrderby = new StringBuilder();
            foreach (DataRow dr in _dtcolumns.Rows)
            {
                clsColumn col = config.NewColumn((string)dr["FieldName"]);
                col.LineNumber = (int)dr["LineNumber"];
                col.Hidden = (bool)dr["Hidden"];
                col.NewDescription = (string)dr["NewDescription"];
                col.OldDescription = (string)dr["OldDescription"];
                col.OrderBy = (int)dr["OrderBy"];
                col.Width = (int)dr["Width"];
                switch (col.OrderBy)
                {
                    case -1:
                        if (col.FieldName.StartsWith("D_"))
                            sbOrderby.AppendFormat(",{0} DESC",_FieldsDetail[col.FieldName].FullFieldName);
                        else
                            sbOrderby.AppendFormat(",{0} DESC",_Fields[col.FieldName].FullFieldName);
                        break;
                    case 1:
                        if (col.FieldName.StartsWith("D_"))
                            sbOrderby.AppendFormat(",{0}",_FieldsDetail[col.FieldName].FullFieldName);
                        else
                            sbOrderby.AppendFormat(",{0}",_Fields[col.FieldName].FullFieldName);
                        break;
                }
            }
            if (sbOrderby.Length > 0)
            {
                sbOrderby.Remove(0, 1);
                config.OrderBy = sbOrderby.ToString();
            }

            //保存条件
            StringBuilder sbSQL = new StringBuilder();
            for (int fi = 0; fi < 2; fi++)
            {
                COMFields fs = null;
                if (fi == 0)
                    fs = _Fields;
                else if (_FieldsDetail == null)
                    break;
                else
                    fs = _FieldsDetail;
                foreach (COMField field in fs.Fields)
                {
                    if ((field.Visible & COMField.Enum_Visible.VisibleInBrower) != COMField.Enum_Visible.VisibleInBrower)
                        continue;
                    string fieldName = null;
                    if (fi == 1)
                            fieldName = "D_" + field.FieldName;
                        else
                            fieldName = field.FieldName;
                    if (!_ControlMap1[fieldName].Checked)
                        continue;
                    clsCondition condition = config.NewCondition(fieldName);
                    string[] s = field.ValueType.Split(':');
                    //sbSQL.Append(" and ");
                    switch (s[0].ToLower())
                    {
                        case "dict":
                        case "data":
                            //condition.SetValue("Text", ((UltraTextEditor)_ControlMap3[fieldName]).Text);
                            //sbSQL.AppendFormat(" {0} like '%{1}%' ", field.FullFieldName, ((UltraTextEditor)_ControlMap3[fieldName]).Text);
                            //break;
                        case "string":
                            condition.SetValue("Text", _ControlMap3[fieldName].Text);
                            sbSQL.AppendFormat(" and {0} like '%{1}%' ", field.FullFieldName, _ControlMap3[fieldName].Text);
                            break;
                        case "number":
                            condition.SetValue("Index", _ControlMap2[fieldName].SelectedIndex.ToString());
                            condition.SetValue("Value", ((UltraCurrencyEditor)_ControlMap3[fieldName]).Value.ToString());
                            condition.SetValue("Index2", _ControlMap4[fieldName].SelectedIndex.ToString());
                            condition.SetValue("Value2", ((UltraCurrencyEditor)_ControlMap5[fieldName]).Value.ToString());
                            if (_ControlMap2[fieldName].Text.Length > 0)
                            {
                                sbSQL.AppendFormat(" and {0} {1} {2} ", field.FullFieldName, _ControlMap2[fieldName].Text, ((UltraCurrencyEditor)_ControlMap3[fieldName]).Value);
                                if (_ControlMap4[fieldName].Text.Length > 0)
                                    sbSQL.AppendFormat(" and {0} {1} {2} ", field.FullFieldName, _ControlMap4[fieldName].Text, ((UltraCurrencyEditor)_ControlMap5[fieldName]).Value);
                            }
                            break;
                        case "int":
                            condition.SetValue("Index", _ControlMap2[fieldName].SelectedIndex.ToString());
                            condition.SetValue("Value", ((NumericUpDown)_ControlMap3[fieldName]).Value.ToString());
                            condition.SetValue("Index2", _ControlMap4[fieldName].SelectedIndex.ToString());
                            condition.SetValue("Value2", ((NumericUpDown)_ControlMap5[fieldName]).Value.ToString());
                            if (_ControlMap2[fieldName].Text.Length > 0)
                            {
                                sbSQL.AppendFormat(" and {0} {1} {2} ", field.FullFieldName, _ControlMap2[fieldName].Text, ((NumericUpDown)_ControlMap3[fieldName]).Value);
                                if (_ControlMap4[fieldName].Text.Length > 0)
                                    sbSQL.AppendFormat(" and {0} {1} {2} ", field.FullFieldName, _ControlMap4[fieldName].Text, ((NumericUpDown)_ControlMap5[fieldName]).Value);
                            }
                            break;
                        case "enum":
                            condition.SetValue("Index1", (_ControlMap2[fieldName]).SelectedIndex.ToString());
                            condition.SetValue("Index", ((ComboBox)_ControlMap3[fieldName]).SelectedIndex.ToString());
                            condition.SetValue("Index3", (_ControlMap4[fieldName]).SelectedIndex.ToString());
                            condition.SetValue("Index4", ((ComboBox)_ControlMap5[fieldName]).SelectedIndex.ToString());
                            if (_ControlMap2[fieldName].Text.Length > 0)
                            {
                                ComboBox cmb1 = (ComboBox)_ControlMap3[fieldName];
                                sbSQL.AppendFormat(" and {0} = {1} ", field.FullFieldName, ((IntItem)cmb1.Items[cmb1.SelectedIndex]).Int);
                                if (_ControlMap4[fieldName].Text.Length > 0)
                                {
                                    ComboBox cmb2 = (ComboBox)_ControlMap5[fieldName];
                                    sbSQL.AppendFormat(" and {0} = {2} ", field.FullFieldName, ((IntItem)cmb2.Items[cmb2.SelectedIndex]).Int);
                                }
                            }
                            break;
                        case "boolean":
                            condition.SetValue("Checked", ((CheckBox)_ControlMap3[fieldName]).Checked.ToString());
                            sbSQL.AppendFormat(" and {0} = {1} ", field.FullFieldName, ((CheckBox)_ControlMap3[fieldName]).Checked ? 1 : 0);
                            break;
                        case "date":
                            condition.SetValue("Index", _ControlMap2[fieldName].SelectedIndex.ToString());
                            condition.SetValue("Value", ((UltraDateTimeEditor)_ControlMap3[fieldName]).DateTime.ToString());
                            condition.SetValue("Index2", _ControlMap4[fieldName].SelectedIndex.ToString());
                            condition.SetValue("Value2", ((UltraDateTimeEditor)_ControlMap5[fieldName]).DateTime.ToString());

                            if (_ControlMap2[fieldName].Text.Length > 0)
                            {
                                sbSQL.Append(" and " + getDateSQL(field.FullFieldName, _ControlMap2[fieldName].Text, ((UltraDateTimeEditor)_ControlMap3[fieldName]).DateTime));
                                if (_ControlMap4[fieldName].Text.Length > 0)
                                    sbSQL.Append(" and " + getDateSQL(field.FullFieldName, _ControlMap4[fieldName].Text,((UltraDateTimeEditor)_ControlMap5[fieldName]).DateTime));
                            }
                            break;
                    }
                }
            }
            if (sbSQL.Length > 0)
            {
                sbSQL.Remove(0, 4);
                config.SQL = sbSQL.ToString();
            }
            return config;
        }
        private string getDateSQL(string field, string opt, DateTime dat)
        {
            if (opt == "=")
            {
                return field + ">='" + dat + "' and " + field + "<'" + dat.AddDays(1) + "'";
            }
            else if (opt == "<>")
            {
                return field + ">'" + dat.AddDays(1) + "' and " + field + "<'" + dat + "'";
            }
            else
            {
                if (opt == "<=")
                {
                    dat = dat.AddDays(1);
                    opt = "<";
                }
                return field + opt + "'" + dat + "'";
            }
        }

        private void listConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listConfig.SelectedItems.Count == 0)
                return;
            string fileName = _Path;
            fileName += listConfig.SelectedItems[0].SubItems[1].Text;
            //清除上次的值
            foreach (CheckBox chk in _ControlMap1.Values)
                chk.Checked = false;
            clsListConfig config = new clsListConfig(fileName,false);
            foreach (clsCondition con in config.Conditions.Values)
            {
                COMField field = null;
                if (con.Key.StartsWith("D_"))
                    field = _FieldsDetail[con.Key.Substring(2)];
                else
                    field = _Fields[con.Key];
                _ControlMap1[con.Key].Checked = true;
                string[] s = field.ValueType.Split(':');
                try
                {
                    switch (s[0].ToLower())
                    {
                        case "dict":
                        case "data":
                        case "string":
                            _ControlMap3[con.Key].Text = con.GetValue("Text");
                            break;
                        case "number":
                            _ControlMap2[con.Key].SelectedIndex = int.Parse(con.GetValue("Index"));
                            ((UltraCurrencyEditor)_ControlMap3[con.Key]).Value = decimal.Parse(con.GetValue("Value"));
                            _ControlMap4[con.Key].SelectedIndex = int.Parse(con.GetValue("Index2"));
                            ((UltraCurrencyEditor)_ControlMap5[con.Key]).Value = decimal.Parse(con.GetValue("Value2"));
                            break;
                        case "int":
                            _ControlMap2[con.Key].SelectedIndex = int.Parse(con.GetValue("Index"));
                            ((NumericUpDown)_ControlMap3[con.Key]).Value = decimal.Parse(con.GetValue("Value"));
                            _ControlMap4[con.Key].SelectedIndex = int.Parse(con.GetValue("Index2"));
                            ((NumericUpDown)_ControlMap5[con.Key]).Value = decimal.Parse(con.GetValue("Value2"));
                            break;
                        case "enum":
                            ((ComboBox)_ControlMap3[con.Key]).SelectedIndex = int.Parse(con.GetValue("Index"));
                            _ControlMap2[con.Key].SelectedIndex = int.Parse(con.GetValue("Index1"));
                            _ControlMap4[con.Key].SelectedIndex = int.Parse(con.GetValue("Index3"));
                            ((ComboBox)_ControlMap5[con.Key]).SelectedIndex = int.Parse(con.GetValue("Index4"));
                            break;
                        case "boolean":
                            ((CheckBox)_ControlMap3[con.Key]).Checked = bool.Parse(con.GetValue("Checked"));
                            break;
                        case "date":
                            _ControlMap2[con.Key].SelectedIndex = int.Parse(con.GetValue("Index"));
                            ((UltraDateTimeEditor)_ControlMap3[con.Key]).DateTime = DateTime.Parse(con.GetValue("Value"));
                            _ControlMap4[con.Key].SelectedIndex = int.Parse(con.GetValue("Index2"));
                            ((UltraDateTimeEditor)_ControlMap5[con.Key]).DateTime = DateTime.Parse(con.GetValue("Value2"));
                            break;
                    }
                }
                catch { }
            }
            foreach (clsColumn col in config.Columns.Values)
            {
                DataRow[] drs = _dtcolumns.Select("FieldName='" + col.FieldName + "'");
                if (drs.Length == 1)
                {
                    drs[0]["LineNumber"] = col.LineNumber;
                    drs[0]["Hidden"] = col.Hidden;
                    drs[0]["NewDescription"] = col.NewDescription;
                    drs[0]["OldDescription"] = col.OldDescription;
                    drs[0]["OrderBy"] = col.OrderBy;
                    drs[0]["Width"] = col.Width;
                }
            }
            gridColumn.DisplayLayout.Bands[0].SortedColumns.Clear();
            gridColumn.DisplayLayout.Bands[0].SortedColumns.Add("LineNumber", false);
        }
        private clsListConfig _Return=null;
        private void butChoice_Click(object sender, EventArgs e)
        {
            _Return = GetListConfig(null);
            this.DialogResult = DialogResult.Yes;
        }
        public clsListConfig ShowSelect(UltraGrid grid,string path,COMFields fields,COMFields fieldsDetail)
        {
            _browserGrid = grid;
            _Path = path;
            _Fields = fields;
            _FieldsDetail = fieldsDetail;
            if (this.ShowDialog() == DialogResult.Yes)
                return _Return;
            else
                return null;
        }
        private void butSaveAs_Click(object sender, EventArgs e)
        {
            string fileName = _Path;
           
            bool exists = true;
            string Name = null;
            while (exists)
            {
                Name = (new frmAskName()).GetName();
                if (Name == null)
                    return;
                //检查名称是否已使用
                exists = false;
                foreach (ListViewItem l in listConfig.Items)
                    if (l.SubItems[0].Text == Name)
                    {
                        exists = true;
                        break;
                    }
            }
            ListViewItem lvi = new ListViewItem(Name);
            Name = string.Format("{0}_{1}.xml", listConfig.Items.Count, Name);
            lvi.SubItems.Add(Name);
            listConfig.Items.Add(lvi);

            fileName += Name;

            clsListConfig config = GetListConfig(fileName);
            //保存
            config.Save();
        }

        private void butUp_Click(object sender, EventArgs e)
        {
            UltraGridRow row = gridColumn.ActiveRow;
            if (row == null)
                return;
            if (row.Index == 0)
                return;
            gridColumn.DisplayLayout.Bands[0].SortedColumns.Clear();
            gridColumn.Rows[row.Index - 1].Cells["LineNumber"].Value=(int)gridColumn.Rows[row.Index - 1].Cells["LineNumber"].Value +1;
            row.Cells["LineNumber"].Value = (int)row.Cells["LineNumber"].Value - 1;
            gridColumn.DisplayLayout.Bands[0].SortedColumns.Add("LineNumber", false);
        }

        private void butDown_Click(object sender, EventArgs e)
        {
            UltraGridRow row = gridColumn.ActiveRow;
            if (row == null)
                return;
            if (row.Index == gridColumn.Rows.Count - 2)
                return;
            gridColumn.DisplayLayout.Bands[0].SortedColumns.Clear();
            gridColumn.Rows[row.Index + 1].Cells["LineNumber"].Value = (int)gridColumn.Rows[row.Index + 1].Cells["LineNumber"].Value - 1;
            row.Cells["LineNumber"].Value = (int)row.Cells["LineNumber"].Value + 1;
            gridColumn.DisplayLayout.Bands[0].SortedColumns.Add("LineNumber", false);
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            if (listConfig.SelectedItems.Count == 1)
            {
                string filePath = _Path;
                ListViewItem lvi = listConfig.SelectedItems[0];
                filePath += lvi.SubItems[1].Text;
                File.Delete(filePath);
                lvi.Remove();
            }
        }

        private void butDefault_Click(object sender, EventArgs e)
        {
            if (listConfig.SelectedItems.Count == 1)
            {
                string oldFileName=null;
                string newFileName =null;
                string filePath = _Path;
                ListViewItem lvi = listConfig.SelectedItems[0];
                int i = lvi.SubItems[1].Text.LastIndexOf(".default.xml");
                if (i>-1)
                {
                    oldFileName = lvi.SubItems[1].Text;
                    newFileName = lvi.SubItems[1].Text.Substring(0, i) + ".xml";
                    lvi.Font = new Font(lvi.Font, FontStyle.Regular);
                }
                else
                {
                    foreach (ListViewItem l in listConfig.Items)
                    {
                        i = l.SubItems[1].Text.LastIndexOf(".default.xml");
                        if (i > -1)
                        {
                            oldFileName = l.SubItems[1].Text;
                            newFileName = l.SubItems[1].Text.Substring(0, i) + ".xml";
                            l.SubItems[1].Text = newFileName;
                            l.Font = new Font(l.Font, FontStyle.Regular);
                            File.Move(filePath + oldFileName, filePath + newFileName);
                            break;
                        }
                    }
                    lvi.Font = new Font(lvi.Font, FontStyle.Bold);
                    oldFileName = lvi.SubItems[1].Text;
                    newFileName = oldFileName.Substring(0, oldFileName.Length - 4) + ".default.xml";
                }
                lvi.SubItems[1].Text = newFileName;
                File.Move(filePath + oldFileName, filePath + newFileName);
            }
        }

        private void butConfigUp_Click(object sender, EventArgs e)
        {
            if (listConfig.SelectedItems.Count == 1)
            {
                ListViewItem lvi = listConfig.SelectedItems[0];
                int index = lvi.Index;
                if (index == 0)
                    return;
                string filePath = _Path;
                string oldFileName = listConfig.Items[index-1].SubItems[1].Text;
                string newFileName = index.ToString() + oldFileName.Substring(oldFileName.IndexOf("_"));
                listConfig.Items[index - 1].SubItems[1].Text = newFileName;
                File.Move(filePath + oldFileName, filePath + newFileName);
                lvi.Remove();
                listConfig.Items.Insert(index - 1, lvi);
                oldFileName = lvi.SubItems[1].Text;
                newFileName = (index-1).ToString() + oldFileName.Substring(oldFileName.IndexOf("_"));
                lvi.SubItems[1].Text = newFileName;
                File.Move(filePath + oldFileName, filePath + newFileName);
            }
        }

        private void butConfigDown_Click(object sender, EventArgs e)
        {
            if (listConfig.SelectedItems.Count == 1)
            {
                ListViewItem lvi = listConfig.SelectedItems[0];
                int index = lvi.Index;
                if (index == listConfig.Items.Count - 2)
                    return;
                string filePath = _Path;
                string oldFileName = listConfig.Items[index + 1].SubItems[1].Text;
                string newFileName = index.ToString() + oldFileName.Substring(oldFileName.IndexOf("_"));
                listConfig.Items[index - 1].SubItems[1].Text = newFileName;
                File.Move(filePath + oldFileName, filePath + newFileName);
                lvi.Remove();
                listConfig.Items.Insert(index + 1, lvi);
                oldFileName = lvi.SubItems[1].Text;
                newFileName = (index+1).ToString() + oldFileName.Substring(oldFileName.IndexOf("_"));
                lvi.SubItems[1].Text = newFileName;
                File.Move(filePath + oldFileName, filePath + newFileName);
            }
        }
        
    }
}