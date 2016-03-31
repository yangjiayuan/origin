using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using Base;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using Infragistics.Excel;

namespace UI
{
    public partial class frmReport : Form
    {

        private DataSet _MainDataSet;
        private COMFields _MainTableDefine;
        private ToolDetailForm _ToolDetailForm;
        private List<COMFields> _DetailTableDefine;

        public class BeforeQueryEventArgs : EventArgs
        {
            private string _SQL;
            private bool _Cancel = false;
            private bool _IsAppendSQL;
            private bool _Changed = false;
            private DataSet _Data = null;
            public BeforeQueryEventArgs(string sql, bool isAppendSQL)
            {
                _SQL = sql;
                _IsAppendSQL = isAppendSQL;
            }
            public string SQL
            {
                get { return _SQL; }
                set { _Changed = _SQL != value; _SQL = value; }
            }
            public DataSet Data
            {
                get { return _Data; }
                set { _Data = value; }
            }
            public bool Cancel
            {
                get { return _Cancel; }
                set { _Cancel = true; }
            }
            public bool IsAppendSQL
            {
                get { return _IsAppendSQL; }
                set { _Changed = _IsAppendSQL != value; _IsAppendSQL = value; }
            }
            public bool Changed
            {
                get { return _Changed; }
                set { _Changed = value; }
            }
        }
        public class BeforeConditionQueryEventArgs : EventArgs
        {
            private string _SQL;
            private bool _Handled = false;
            private ReportCondition _Condition;
            private bool _Cancel = false;
            public BeforeConditionQueryEventArgs(string sql, ReportCondition rc)
            {
                _SQL = sql;
                _Condition = rc;
            }
            public ReportCondition Condition
            {
                get
                {
                    return _Condition;
                }
            }
            public bool Handled
            {
                get { return _Handled; }
                set { _Handled = value; }
            }
            public bool Cancel
            {
                get { return _Cancel; }
                set { _Cancel = value; }
            }

            public string SQL
            {
                get { return _SQL; }
                set { _SQL = value; }
            }
        }
        public class BeforeConditionCreateEventArgs : EventArgs
        {
            private Control _Control;
            private bool _NewControl = false;
            private bool _HasCaption = true;
            private ReportCondition _Condition;
            public BeforeConditionCreateEventArgs(ReportCondition rc)
            {
                _Condition = rc;
            }
            public ReportCondition Condition
            {
                get
                {
                    return _Condition;
                }
            }
            public bool NewControl
            {
                get { return _NewControl; }
                set { _NewControl = value; }
            }
            public bool HasCaption
            {
                get { return _HasCaption; }
                set { _HasCaption = value; }
            }
            public Control Control
            {
                get { return _Control; }
                set { _Control = value; }
            }
        }
        public class BeforeSelectFormEventArgs : EventArgs
        {
            private ReportCondition _Condition;
            private bool _Cancel;
            public BeforeSelectFormEventArgs(ReportCondition rc)
            {
                _Condition = rc;
            }
            public bool Cancel
            {
                set { _Cancel = value; }
                get { return _Cancel; }
            }
            public ReportCondition Condition
            {
                get { return _Condition; }
            }
        }
        public delegate void BeforeQueryEventHandler(object sender, BeforeQueryEventArgs e);
        public event BeforeQueryEventHandler BeforeQuery;
        //public delegate void BeforeSelectFormEventHandler(object sender, BeforeSelectFormEventArgs e);
        //public event BeforeSelectFormEventHandler BeforeSelectForm;
        public delegate bool BeforeExportEventHandler(Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter exporter, UltraGrid grid, Worksheet sheet, ref int startRow, ref int startColumn);
        public event BeforeExportEventHandler BeforeExport;
        public delegate void AfterExportEventHandler(UltraGrid grid, Worksheet sheet);
        public event AfterExportEventHandler AfterExport;

        private ToolStripButton toolExit;
        private const int TOP_VALUE = 10;
        private const int LEFT_VALUE = 10;
        private const int LABEL_WITH = 70;
        private const int DATA_WITH = 110;
        private const int SPACE_WITH = 20;
        private const int CONTROL_HEIGHT = 25;

        //private string _Report.SQL;
        //private List<ReportColumn> _Columns = new List<ReportColumn>();
        //private List<ReportCondition> _Report.Conditions = new List<ReportCondition>();
        //private bool _ShowConditionForm;
        private Form ConditionForm = null;
        //private string _Title;
        //private bool _IsWhere = true;
        public UltraGrid grid;
        private UltraGrid detailGrid = null;
        //private bool _Report.IsAppendSQL = false;

        private BaseReport _Report;
        private Form _MDIForm;

        public frmReport()
        {
            InitializeComponent();
        }
        public frmReport(BaseReport rpt,Form mdi):this()
        {
            _Report = rpt;
            _MDIForm = mdi;
            _Report.InitControl = InitializeControl;
            this.Tag = _Report;
            //_Report.Columns.Clear();
            //_Report.Conditions.Clear();
            //_Report.DetailButtons.Clear();
            //_MDIForm = mdi;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case (Keys.Control | Keys.W):
                    {
                        toolExit.PerformClick();
                        return true ;
                    }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void getControl(BeforeConditionCreateEventArgs e)
        {
            string[] s = e.Condition.DataType.Split(':');
            switch (s[0].ToLower())
            {
                case "string":
                    TextBox t = new TextBox();
                    e.Control = t;
                    break;
                case "number":
                    UltraCurrencyEditor nu = new UltraCurrencyEditor();
                    string[] d = s[1].Split(',');
                    nu.FormatString = "#,##0." + new string('0', int.Parse(d[1]));
                    nu.MaskInput = "nnn,nnn,nnn." + new string('n', int.Parse(d[1]));
                    e.Control = nu;
                    break;
                case "int":
                    NumericUpDown nud = new NumericUpDown();
                    nud.Maximum = int.MaxValue;
                    e.Control = nud;
                    break;
                case "enum":
                    ComboBox cb = new ComboBox();
                    string[] es = s[1].Split(',');
                    for (int i = 0; i < es.Length; i++)
                    {
                        string[] v = es[i].Split('=');
                        if (v.Length >= 2)
                        {
                            IntItem iItem = new IntItem(int.Parse(v[0]), v[1]);
                            cb.Items.Add(iItem);
                        }
                    }
                    cb.SelectedIndexChanged += new EventHandler(cmb_SelectedIndexChanged);
                    e.Control = cb;
                    break;
                case "boolean":
                    CheckBox chk = new CheckBox();
                    chk.Text = e.Condition.Description;
                    chk.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    chk.AutoSize = true;
                    e.Control = chk;
                    e.HasCaption = false;
                    break;
                case "dict":
                case "data":
                    UltraTextEditor ute = new UltraTextEditor();
                    //ute.ReadOnly = true;
                    EditorButton editButton = new EditorButton();
                    ute.ButtonsRight.Add(editButton);
                    ute.KeyDown += new KeyEventHandler(ute_KeyDown);
                    ute.EditorButtonClick += new EditorButtonEventHandler(Dict_EditorButtonClick);
                    ute.TextChanged += new EventHandler(Dict_TextChanged);
                    e.Control = ute;
                    break;
                case "custom":
                    UltraTextEditor utec = new UltraTextEditor();
                    EditorButton editButton2 = new EditorButton();
                    utec.ButtonsRight.Add(editButton2);
                    e.Control = utec;
                    break;
                case "date":
                    UltraDateTimeEditor udt = new UltraDateTimeEditor();
                    e.Control = udt;
                    break;
                case "period":
                    UltraDateTimeEditor udtPeriod = new UltraDateTimeEditor();
                    udtPeriod.FormatString = "yyyy年MM月";
                    e.Control = udtPeriod;
                    break;
            }
        }
        
        private void InitializeControl(object sender, EventArgs eArg)
        {
            Form mdiParent = this._MDIForm;
            this.SuspendLayout();
            this.Text = _Report.Title;
            Panel pCondition = null;
            int x = 0;
            int y = 0;
            if (!_Report.ShowConditionForm)
            {
                pCondition = new Panel();
                pCondition.Dock = DockStyle.Top;
                pCondition.Width = 800;

                x = LEFT_VALUE;
                y = TOP_VALUE;
                int y2 = TOP_VALUE;
                int columns = (pCondition.Width - LEFT_VALUE) / (LABEL_WITH + DATA_WITH + SPACE_WITH);
                int columnIndex = 1;
                GroupBox grp = null;

                int lines = (_Report.Conditions.Count / columns) + (_Report.Conditions.Count % columns == 0 ? 0 : 1);
                pCondition.Height = (lines + 1) * CONTROL_HEIGHT;

                foreach (ReportCondition rc in _Report.Conditions)
                {
                    BeforeConditionCreateEventArgs e = new BeforeConditionCreateEventArgs(rc);
                    _Report.BeforeNewControl(e);
                    if (!e.NewControl)
                        getControl(e);
                    _Report.AfterNewControl(e);
                    //处理占位多个控件位置的控件,如果放在行末,要判断宽度是否超过
                    if (rc.Width > 1 && (columnIndex + rc.Width - 1) > columns)
                    {
                        columnIndex = 1;
                        x = LEFT_VALUE;
                        y += CONTROL_HEIGHT;
                    }

                    if (rc.GroupName != null && rc.GroupName.Length > 0 && (grp == null || grp.Text != rc.GroupName))
                    {
                        //处理分组
                        if (columnIndex != 1)
                        {
                            columnIndex = 1;
                            x = LEFT_VALUE;
                            y += CONTROL_HEIGHT;
                        }
                        grp = new GroupBox();
                        grp.Text = rc.GroupName;
                        grp.Width = pCondition.Width - LEFT_VALUE;
                        grp.Left = 0;
                        grp.Top = y;
                        grp.Height = CONTROL_HEIGHT * 2;
                        y = grp.Top + CONTROL_HEIGHT + TOP_VALUE;
                        y2 = TOP_VALUE * 2;
                        pCondition.Controls.Add(grp);
                    }
                    else
                    {
                        if (grp != null && (rc.GroupName == null || rc.GroupName.Length == 0))
                        {
                            grp = null;
                            if (columnIndex != 1)
                            {
                                columnIndex = 1;
                                x = LEFT_VALUE;
                                y += CONTROL_HEIGHT;
                            }
                        }
                        if (rc.NewLine && columnIndex != 1)
                        {
                            //处理换行的问题
                            columnIndex = 1;
                            x = LEFT_VALUE;
                            y += CONTROL_HEIGHT;
                        }
                    }

                    Label l = null;
                    if (e.HasCaption)
                    {
                        l = new Label();
                        l.Left = x;
                        l.Top = y + 4;
                        l.Width = LABEL_WITH;
                        l.Text = rc.Description;
                        l.AutoSize = true;
                        l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                        x += LABEL_WITH;

                        if (grp == null)
                            pCondition.Controls.Add(l);
                        else
                        {
                            l.Top = y2 + 4;
                            grp.Controls.Add(l);
                        }

                        if (rc.Mandatory)
                        {
                            l.ForeColor = System.Drawing.Color.Blue;
                            l.Font = new Font(l.Font, FontStyle.Bold);
                        }
                    }

                    Control ctl = e.Control;
                    ctl.Left = x;
                    ctl.Top = y;
                    rc.Control = ctl;

                    if (e.HasCaption)
                        ctl.Width = DATA_WITH;
                    else
                        ctl.Width = DATA_WITH + LABEL_WITH;
                    //当占多个控件位置时
                    if (rc.Width > 1)
                    {
                        ctl.Width = ctl.Width + (DATA_WITH + LABEL_WITH + SPACE_WITH) * (rc.Width - 1);
                        columnIndex += (rc.Width - 1);
                        x += (DATA_WITH + LABEL_WITH + SPACE_WITH) * (rc.Width - 1);
                    }

                    if (grp == null)
                        pCondition.Controls.Add(ctl);
                    else
                    {
                        ctl.Top = y2;
                        grp.Controls.Add(ctl);
                    }

                    if (l != null)
                        l.SendToBack();

                    if (columnIndex == columns)
                    {
                        columnIndex = 1;
                        x = LEFT_VALUE;
                        y += CONTROL_HEIGHT;
                        y2 += CONTROL_HEIGHT;
                        if (grp != null)
                            grp.Height += CONTROL_HEIGHT;
                    }
                    else
                    {
                        columnIndex++;
                        if (e.HasCaption)
                            x += (DATA_WITH + SPACE_WITH);
                        else
                            x += (DATA_WITH + LABEL_WITH + SPACE_WITH);
                    }

                    /*Label l = null;
                    if (e.HasCaption)
                    {
                        l = new Label();
                        l.Left = x;
                        l.Top = y + 4;
                        l.Width = LABEL_WITH;
                        l.Text = rc.Description;
                        l.AutoSize = true;
                        l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                        x += LABEL_WITH;
                        pCondition.Controls.Add(l);
                    }

                    Control ctl = e.Control;
                    ctl.Left = x;
                    ctl.Top = y;
                    rc.Control = ctl;

                    if (e.HasCaption)
                        ctl.Width = DATA_WITH;
                    else
                        ctl.Width = DATA_WITH + LABEL_WITH;
                    pCondition.Controls.Add(ctl);
                    if (l != null)
                        l.SendToBack();

                    if (columnIndex == columns)
                    {
                        columnIndex = 1;
                        x = LEFT_VALUE;
                        y += CONTROL_HEIGHT;
                    }
                    else
                    {
                        columnIndex++;
                        if (e.HasCaption)
                            x += (DATA_WITH + SPACE_WITH);
                        else
                            x += (DATA_WITH + LABEL_WITH + SPACE_WITH);
                    }*/

                    /*string[] s = rc.DataType.Split(':');
                    switch (s[0].ToLower())
                    {
                        case "string":
                            Label ls = new Label();
                            ls.Left = x;
                            ls.Top = y + 4;
                            ls.Width = LABEL_WITH;
                            ls.Text = rc.Description;
                            ls.AutoSize = true;
                            ls.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            x += LABEL_WITH;
                            TextBox t = new TextBox();
                            t.Left = x;
                            t.Top = y;
                            t.Width = DATA_WITH;
                            pCondition.Controls.Add(t);
                            pCondition.Controls.Add(ls);
                            rc.Control = t;
                            if (columnIndex == columns)
                            {
                                columnIndex = 1;
                                x = LEFT_VALUE;
                                y += CONTROL_HEIGHT;
                            }
                            else
                            {
                                columnIndex++;
                                x += (DATA_WITH + SPACE_WITH);
                            }
                            break;
                        case "number":
                            Label ln = new Label();
                            ln.Left = x;
                            ln.Top = y + 4;
                            ln.Width = LABEL_WITH;
                            ln.Text = rc.Description;
                            ln.AutoSize = true;
                            ln.SendToBack();
                            ln.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            x += LABEL_WITH;
                            UltraCurrencyEditor nu = new UltraCurrencyEditor();
                            nu.Left = x;
                            nu.Top = y;
                            nu.Width = DATA_WITH;
                            string[] d = s[1].Split(',');
                            nu.FormatString = "#,##0." + new string('0', int.Parse(d[1])) + ";" + "-#,##0." + new string('0', int.Parse(d[1])) + "; ";
                            nu.MaskInput = "nnn,nnn,nnn." + new string('n', int.Parse(d[1]));
                            pCondition.Controls.Add(nu);
                            pCondition.Controls.Add(ln);
                            rc.Control = nu;
                            if (columnIndex == columns)
                            {
                                columnIndex = 1;
                                x = LEFT_VALUE;
                                y += CONTROL_HEIGHT;
                            }
                            else
                            {
                                columnIndex++;
                                x += (DATA_WITH + SPACE_WITH);
                            }
                            break;
                        case "int":
                            Label li = new Label();
                            li.Left = x;
                            li.Top = y + 4;
                            li.Width = LABEL_WITH;
                            li.Text = rc.Description;
                            li.AutoSize = true;
                            li.SendToBack();
                            li.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            x += LABEL_WITH;
                            NumericUpDown nud = new NumericUpDown();
                            nud.Left = x;
                            nud.Top = y;
                            nud.Width = DATA_WITH;
                            pCondition.Controls.Add(nud);
                            pCondition.Controls.Add(li);
                            rc.Control = nud;
                            if (columnIndex == columns)
                            {
                                columnIndex = 1;
                                x = LEFT_VALUE;
                                y += CONTROL_HEIGHT;
                            }
                            else
                            {
                                columnIndex++;
                                x += (DATA_WITH + SPACE_WITH);
                            }
                            break;
                        case "enum":
                            Label le = new Label();
                            le.Left = x;
                            le.Top = y + 4;
                            le.Width = LABEL_WITH;
                            le.Text = rc.Description;
                            le.AutoSize = true;
                            le.SendToBack();
                            le.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            x += LABEL_WITH;
                            ComboBox cb = new ComboBox();
                            cb.Left = x;
                            cb.Top = y;
                            cb.Width = DATA_WITH;
                            string[] e = s[1].Split(',');
                            for (int index = 0; index < e.Length; index++)
                            {
                                string[] v = e[index].Split('=');
                                if (v.Length >= 2)
                                {
                                    IntItem iItem = new IntItem(int.Parse(v[0]), v[1]);
                                    cb.Items.Add(iItem);
                                }
                            }
                            cb.SelectedIndexChanged += new EventHandler(cmb_SelectedIndexChanged);
                            pCondition.Controls.Add(cb);
                            pCondition.Controls.Add(le);
                            rc.Control = cb;
                            if (columnIndex == columns)
                            {
                                columnIndex = 1;
                                x = LEFT_VALUE;
                                y += CONTROL_HEIGHT;
                            }
                            else
                            {
                                columnIndex++;
                                x += (DATA_WITH + SPACE_WITH);
                            }
                            break;
                        case "boolean":
                            CheckBox chk = new CheckBox();
                            chk.Text = rc.Description;
                            chk.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            chk.Left = x;
                            chk.Top = y;
                            chk.Width = DATA_WITH + LABEL_WITH;
                            pCondition.Controls.Add(chk);
                            rc.Control = chk;
                            if (columnIndex == columns)
                            {
                                columnIndex = 1;
                                x = LEFT_VALUE;
                                y += CONTROL_HEIGHT;
                            }
                            else
                            {
                                columnIndex++;
                                x += (DATA_WITH + SPACE_WITH + LABEL_WITH);
                            }
                            break;
                        case "dict":
                        case "data":
                            Label ld = new Label();
                            ld.Left = x;
                            ld.Top = y + 4;
                            ld.Width = LABEL_WITH;
                            ld.Text = rc.Description;
                            ld.AutoSize = true;
                            ld.SendToBack();
                            ld.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            x += LABEL_WITH;
                            UltraTextEditor ute = new UltraTextEditor();
                            ute.Left = x;
                            ute.Top = y;
                            ute.Width = DATA_WITH;
                            //ute.ReadOnly = true;
                            EditorButton editButton = new EditorButton();
                            ute.ButtonsRight.Add(editButton);
                            ute.KeyDown += new KeyEventHandler(ute_KeyDown);
                            ute.EditorButtonClick += new EditorButtonEventHandler(Dict_EditorButtonClick);
                            ute.TextChanged += new EventHandler(Dict_TextChanged);
                            //ute.Tag = rc;
                            pCondition.Controls.Add(ute);
                            pCondition.Controls.Add(ld);
                            rc.Control = ute;
                            if (columnIndex == columns)
                            {
                                columnIndex = 1;
                                x = LEFT_VALUE;
                                y += CONTROL_HEIGHT;
                            }
                            else
                            {
                                columnIndex++;
                                x += (DATA_WITH + SPACE_WITH);
                            }
                            break;
                        case "date":
                            Label ldate = new Label();
                            ldate.Left = x;
                            ldate.Top = y + 4;
                            ldate.Width = LABEL_WITH;
                            ldate.Text = rc.Description;
                            ldate.AutoSize = true;
                            ldate.SendToBack();
                            ldate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            x += LABEL_WITH;
                            UltraDateTimeEditor udt = new UltraDateTimeEditor();
                            udt.Left = x;
                            udt.Top = y;
                            udt.Width = DATA_WITH;
                            pCondition.Controls.Add(udt);
                            pCondition.Controls.Add(ldate);
                            rc.Control = udt;
                            if (columnIndex == columns)
                            {
                                columnIndex = 1;
                                x = LEFT_VALUE;
                                y += CONTROL_HEIGHT;
                            }
                            else
                            {
                                columnIndex++;
                                x += (DATA_WITH + SPACE_WITH);
                            }
                            break;
                        case "period":
                            Label ldatePeriod = new Label();
                            ldatePeriod.Left = x;
                            ldatePeriod.Top = y + 4;
                            ldatePeriod.Width = LABEL_WITH;
                            ldatePeriod.Text = rc.Description;
                            ldatePeriod.AutoSize = true;
                            ldatePeriod.SendToBack();
                            ldatePeriod.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                            x += LABEL_WITH;
                            UltraDateTimeEditor udtPeriod = new UltraDateTimeEditor();
                            udtPeriod.FormatString = "yyyy年MM月";
                            udtPeriod.Left = x;
                            udtPeriod.Top = y;
                            udtPeriod.Width = DATA_WITH;
                            pCondition.Controls.Add(udtPeriod);
                            pCondition.Controls.Add(ldatePeriod);
                            rc.Control = udtPeriod;
                            if (columnIndex == columns)
                            {
                                columnIndex = 1;
                                x = LEFT_VALUE;
                                y += CONTROL_HEIGHT;
                            }
                            else
                            {
                                columnIndex++;
                                x += (DATA_WITH + SPACE_WITH);
                            }
                            break;
                    }*/
                }
            }
            //设置详细的按钮
            if (_Report.DetailButtons.Count > 0)
            {
                int columns = (pCondition.Width - LEFT_VALUE) / (LABEL_WITH + SPACE_WITH);
                int lines = (_Report.DetailButtons.Count / columns) + (_Report.DetailButtons.Count % columns == 0 ? 0 : 1);
                pCondition.Height = pCondition.Height + (lines + 1) * CONTROL_HEIGHT;
                
                x = LEFT_VALUE;
                if (y == 0)
                    y = TOP_VALUE;
                else
                    y += CONTROL_HEIGHT;
                int columnIndex = 1;
                foreach (ReportDetialButton rdb in _Report.DetailButtons)
                {
                    Button but = new Button();
                    but.Left = x;
                    but.Top = y;
                    but.Width = LABEL_WITH;
                    but.Text = rdb.Text;
                    pCondition.Controls.Add(but);
                    rdb.Button = but;
                    but.Tag = rdb;
                    if (columnIndex == columns)
                    {
                        columnIndex = 1;
                        x = LEFT_VALUE;
                        y += CONTROL_HEIGHT;
                    }
                    else
                    {
                        columnIndex++;
                        x += (LABEL_WITH + SPACE_WITH);
                    }
                    but.Click += new EventHandler(but_Click);
                }
            }
            
            //工具条
            ToolStrip toolReport = new System.Windows.Forms.ToolStrip();
            ToolStripButton toolSearch = new System.Windows.Forms.ToolStripButton();
            ToolStripSeparator toolSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            ToolStripButton toolExport = new System.Windows.Forms.ToolStripButton();
            ToolStripSeparator toolSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolExit = new System.Windows.Forms.ToolStripButton();
            toolReport.SuspendLayout();
            // 
            // toolReport
            // 
            toolReport.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                toolSearch,
                toolSeparator1,
                toolExport,
                toolSeparator2,
                toolExit});
            toolReport.Location = new System.Drawing.Point(0, 0);
            toolReport.Name = "toolReport";
            toolReport.Size = new System.Drawing.Size(792, 35);
            toolReport.TabIndex = 0;
            // 
            // toolSearch
            // 
            toolSearch.Image = global::UI.Properties.Resources.Query;
            toolSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolSearch.Name = "toolSearch";
            toolSearch.Size = new System.Drawing.Size(39, 35);
            toolSearch.Text = "查询";
            toolSearch.Font = new System.Drawing.Font("SimSun",(float)10.5);
            toolSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolSearch.Click += new System.EventHandler(toolSearch_Click);
            // 
            // toolExport
            // 
            toolExport.Image = global::UI.Properties.Resources.ExportToExcal;
            toolExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolExport.Name = "toolExport";
            toolExport.Size = new System.Drawing.Size(39, 35);
            toolExport.Text = "导出";
            toolExport.Font = new System.Drawing.Font("SimSun", (float)10.5);
            toolExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolExport.Click += new System.EventHandler(toolExport_Click);
            toolReport.PerformLayout();
            // 
            // toolExit
            // 
            toolExit.Image = global::UI.Properties.Resources.Close;
            toolExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolExit.Name = "toolExit";
            toolExit.Size = new System.Drawing.Size(39, 35);
            toolExit.Text = "退出";
            toolExit.Font = new System.Drawing.Font("SimSun", (float)10.5);
            toolExit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            toolExit.Click += new System.EventHandler(toolExit_Click);
            toolReport.PerformLayout();

            _Report.InsertToolStrip(toolReport);

            //Grid
            Panel pGrid = new Panel();
            pGrid.Dock = DockStyle.Fill;

            //grid
            grid = new UltraGrid();
            pGrid.Controls.Add(grid);
            grid.Dock = DockStyle.Fill;
            grid.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
            grid.UpdateMode = UpdateMode.OnCellChangeOrLostFocus;
            grid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            grid.DoubleClickRow += new DoubleClickRowEventHandler(grid_DoubleClickRow);
            grid.DisplayLayout.ScrollBounds = ScrollBounds.ScrollToFill;
            grid.DisplayLayout.Override.AllowColMoving = AllowColMoving.WithinBand;


            //增加spliter 和明细Grid
            if (_Report.DetailButtons.Count > 0)
            {
                Splitter splitter = new Splitter();
                splitter.Dock = DockStyle.Bottom;
                pGrid.Controls.Add(splitter);
                detailGrid = new UltraGrid();
                detailGrid.Dock = DockStyle.Bottom;
                detailGrid.Height = this.Height / 5 * 2;
                detailGrid.DoubleClickRow += new DoubleClickRowEventHandler(grid_DoubleClickRow);
                detailGrid.DisplayLayout.Override.AllowColMoving = AllowColMoving.WithinBand;
                pGrid.Controls.Add(detailGrid);
            }
            

            // 
            // frmReport
            // 
            this.Controls.Add(pGrid);
            if (pCondition!=null)
                this.Controls.Add(pCondition);
            this.Controls.Add(toolReport);
            toolReport.ResumeLayout(false);
            this.ResumeLayout(false);

            this.MdiParent = mdiParent;
        }


        void but_Click(object sender, EventArgs e)
        {
            if (grid.ActiveRow == null) return;
            Button but= (Button) sender;
            ReportDetialButton rdb = (ReportDetialButton)but.Tag;
            string sql = _Report.DetailButtonClick(rdb, grid.ActiveRow);
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(sql);
            setGrid(rdb.Columns, detailGrid, ds);
        }

        //void grid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        //{
        //    _Report.Strike(this,e.Row);
        //}


        public virtual void grid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (_ToolDetailForm != null && _ToolDetailForm.OnlyDisplay)
                return;

            toolView_Click(sender, EventArgs.Empty);
        }


        public virtual void toolView_Click(object sender, EventArgs e)
        {
            if (this.grid.ActiveRow != null)
            {
                DataSet ds;

                //if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                //{
                //    ds = GetMainDataSet((Guid)grid.ActiveRow.Cells["ID"].Value);
                //}
                //else
                //{
                //    if (_MainDataSet.Tables[0].Columns.Contains("ID"))
                //    {
                //        DataRow[] drs = _MainDataSet.Tables[0].Select("ID = '" + grid.ActiveRow.Cells["ID"].Value + "'");
                //        ds = _MainDataSet.Clone();
                //        ds.Tables[0].ImportRow(drs[0]);
                //    }
                //    else if (_MainDataSet.Tables[0].TableName.Equals("S_Table"))
                //    {
                //        DataRow[] drs = _MainDataSet.Tables[0].Select("TableName = '" + grid.ActiveRow.Cells["TableName"].Value + "'");
                //        ds = _MainDataSet.Clone();
                //        ds.Tables[0].ImportRow(drs[0]);
                //    }
                //    else
                //    {
                //        DataRow[] drs = _MainDataSet.Tables[0].Select("ID = '" + grid.ActiveRow.Cells["ID"].Value + "'");
                //        ds = _MainDataSet.Clone();
                //        ds.Tables[0].ImportRow(drs[0]);
                //    }
                //}


                DataRow[] drs = _MainDataSet.Tables[0].Select("ID = '" + grid.ActiveRow.Cells["ID"].Value + "'");
                ds = _MainDataSet.Clone();
                ds.Tables[0].ImportRow(drs[0]);

                frmDetail frm = GetDetailForm();
                if (frm != null)
                {
                    if (this.MdiParent != null)
                        frm.MdiParent = this.MdiParent;
                    //frm.DefaultValue = _DefaultValue;
                    //frm.CopyLastValue = _CopyLastValue;
                    frm.toolDetailForm = _ToolDetailForm;
                    //frm.SelectForm = _SelectForm;
                    //frm.DetailFormHandler = _DetailFormHandler;
                    //frm.Changed += new DataTableEventHandler(frm_Changed);
                    //frm.PageDown += new DataTableEventHandler(frm_PageDown);
                    //frm.PageUp += new DataTableEventHandler(frm_PageUp);

                    bool bShowData = frm.ShowData(_MainTableDefine, _DetailTableDefine, new DataSetEventArgs(ds, this.grid.ActiveRow.HasNextSibling(), this.grid.ActiveRow.HasPrevSibling()), false, this.MdiParent);
                    if (bShowData && this.MdiParent != null)
                    {
                        //frm.FormClosed += new FormClosedEventHandler(frm_FormClosed);
                        //SetEnabled(false);
                    }
                }
            }
        }

        public virtual DataSet GetMainDataSet(Guid ID)
        {
            return CSystem.Sys.Svr.cntMain.Select(string.Format("{0} where {2}.ID='{1}'", _MainTableDefine.GetQuerySQL(false), ID, _MainTableDefine.OrinalTableName), _MainTableDefine.OrinalTableName);
        }

        public virtual frmDetail GetDetailForm()
        {
            frmDetail frm = new frmDetail();
            //frm.Path = this.FullPath;
            frm.IsHistory = true;
            return frm;
        }

 

        private bool Setting = false;
        void Dict_TextChanged(object sender, EventArgs e)
        {
            UltraTextEditor txt = (UltraTextEditor)sender;
            if (txt.Text == "" && !Setting)
            {
                txt.Tag = null;
            }
        }
        void ute_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                FindData(sender,true);
            else if (e.Control && e.KeyCode == Keys.L)
                FindData(sender,false);
        }

        void Dict_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            FindData(sender,false);
        }
        void FindData(object sender, bool IsKeyDown)
        {
            UltraTextEditor txt = (UltraTextEditor)sender;
            ReportCondition condition = null;
            foreach (ReportCondition rc in _Report.Conditions)
            {
                if (rc.Control == txt)
                {
                    condition = rc;
                    break;
                }
            }
            string[] s = condition.DataType.Split(':');
            List<COMFields> detailTable = CSystem.Sys.Svr.Properties.DetailTableDefineList(s[1]);

            BeforeSelectFormEventArgs e = new BeforeSelectFormEventArgs(condition);
            string where = _Report.BeforeSelectForm(e);
            if (e.Cancel)
                return;

            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields(s[1]), detailTable, where, enuShowStatus.None);
            string other;
            string field = null;
            if (s[0].ToLower() == "dict")
            {
                if (CSystem.Sys.Svr.LDI.GetFields(s[1]).FieldNameList(false).Contains("Disable"))
                    other = s[1] + ".Disable=0";
                else
                    other = "";
                if (CSystem.Sys.Svr.LDI.GetFields(s[1]).FieldNameList(false).Contains("Name"))
                    field = string.Format("{0}.{1}", s[1], "Name");
            }
            else
            {
                other = "";
                if (CSystem.Sys.Svr.LDI.GetFields(s[1]).FieldNameList(false).Contains("Code"))
                    field = string.Format("{0}.{1}", s[1], "Code");
            }
            string text = null;
            if (IsKeyDown)
                text = txt.Text;
            DataRow dr = frm.ShowSelect(field, text, other);
            //DataRow dr = frm.ShowSelect("", "", other);
            if (dr != null)
            {
                txt.Tag = dr["ID"];
                if (dr.Table.Columns.Contains("Name"))
                    txt.Text = dr["Name"] as string;
                else if (dr.Table.Columns.Contains("Code"))
                    txt.Text = dr["Code"] as string;
                _Report.AfterSelectForm(condition, dr);
                //Setting = true;
                //if (AfterSelectForm != null)
                //    AfterSelectForm(sender, new AfterSelectFormEventArgs(field, dr));
                //Setting = false;
            }
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
        public static void SetCmbItemByInt(ComboBox cmb)
        {
            if (cmb.SelectedItem != null)
            {
                IntItem dt = cmb.SelectedItem as IntItem;
                if (dt != null)
                {
                    SetCmbItemByInt(cmb, dt.Int);
                }
            }
            if (cmb.Tag == null) return;
            if (cmb.Tag == DBNull.Value) return;
            int i = (int)cmb.Tag;
            SetCmbItemByInt(cmb, i);
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

        //查询前,产生事件,可以去修改SQL,或某个控件的条件等
        private void toolSearch_Click(object sender, EventArgs e)
        {
            Query();
        }

        public void Query()
        {
            //检测一些必填的字段
            string sResult="";
            foreach (ReportCondition rc in _Report.Conditions)
            {
                if (rc.Mandatory)
                {
                    string[] s = rc.DataType.Split(':');
                    switch (s[0].ToLower())
                    {
                        case "string":
                            if (rc.Control.GetType() == typeof(TextBox) && ((TextBox)rc.Control).Text.Length == 0)
                                sResult += string.Format("[{0}]没有输入值!\r\n", rc.Description);
                            break;
                        case "enum":
                            if (rc.Control.GetType() == typeof(ComboBox) && ((ComboBox)rc.Control).SelectedIndex< 0)
                                sResult += string.Format("[{0}]没有输入值!\r\n", rc.Description);
                            break;
                        case "dict":
                        case "data":
                            if (rc.Control.GetType() == typeof(UltraTextEditor) && ((UltraTextEditor)rc.Control).Tag == null)
                                sResult += string.Format("[{0}]没有输入值!\r\n", rc.Description);
                            break;
                        case "date":
                        case "period": 
                            if (rc.Control.GetType() == typeof(UltraDateTimeEditor) && ((UltraDateTimeEditor)rc.Control).Value == null)
                                sResult += string.Format("[{0}]没有输入值!\r\n", rc.Description);
                            break;
                    }
                }
            }
            if (sResult.Length > 0)
            {
                Msg.Warning(sResult);
                return;
            }

            string sql = _Report.SQL;
            bool isAppend = _Report.IsAppendSQL;
           
            BeforeQueryEventArgs bqe=new BeforeQueryEventArgs(_Report.SQL, _Report.IsAppendSQL);
            _Report.BeforeQuery(this, bqe);
            DataSet ds = null;
            if (!bqe.Cancel)
            {
                if (bqe.Changed)
                {
                    sql = bqe.SQL;
                    isAppend = bqe.IsAppendSQL;
                }

                StringBuilder sb = new StringBuilder();
                foreach (ReportCondition rc in _Report.Conditions)
                {
                    BeforeConditionQueryEventArgs bcqe = new BeforeConditionQueryEventArgs(sql, rc);
                    _Report.BeforeCondition(bcqe);
                    if (bcqe.Cancel)
                        return;
                    if (bcqe.Handled)
                    {
                        sql = bcqe.SQL;
                        continue;
                    }
                    string[] s = rc.DataType.Split(':');
                    switch (s[0].ToLower())
                    {
                        case "string":
                            TextBox txt = (TextBox)rc.Control;
                            if (txt.Text.Length > 0)
                            {
                                if (rc.ConditionSQL != null && rc.ConditionSQL.Length > 0)
                                {
                                    sb.Append(" and ");
                                    sb.AppendFormat(rc.ConditionSQL, txt.Text);
                                }
                                sql = sql.Replace("${" + rc.Name + "}", txt.Text);
                            }
                            else
                                sql = sql.Replace("${" + rc.Name + "}", "");
                            break;
                        case "enum":
                            ComboBox cb = (ComboBox)rc.Control;
                            if (cb.SelectedItem != null)
                            {
                                if (rc.ConditionSQL != null && rc.ConditionSQL.Length > 0)
                                {
                                    sb.Append(" and ");
                                    sb.AppendFormat(rc.ConditionSQL, ((IntItem)cb.SelectedItem).Int);
                                }
                                sql = sql.Replace("${" + rc.Name + "}", ((IntItem)cb.SelectedItem).Int.ToString());
                            }
                            else
                                sql = sql.Replace("${" + rc.Name + "}", "");
                            break;
                        case "boolean":
                            //bool类型比较特别，不参加条件，只进行替换。所以如果需要条件中反映需要写到ＳＱＬ中去
                            CheckBox chk = (CheckBox)rc.Control;
                            if (chk.Checked)
                            {
                                sql = sql.Replace("${" + rc.Name + "}", rc.ConditionSQL);
                                //if (rc.ConditionSQL != null && rc.ConditionSQL.Length > 0)
                                //{
                                //    sb.Append(" and ");
                                //    sb.AppendFormat(rc.ConditionSQL);
                                //}
                            }
                            else
                            {
                                sql = sql.Replace("${" + rc.Name + "}", rc.ConditionSQL2);
                                //if (rc.ConditionSQL2 != null && rc.ConditionSQL2.Length > 0)
                                //{
                                //    sb.Append(" and ");
                                //    sb.AppendFormat(rc.ConditionSQL2);
                                //}
                            }
                            break;
                        case "dict":
                        case "data":
                            UltraTextEditor ute = (UltraTextEditor)rc.Control;
                            if (ute.Tag != null)
                            {
                                if (rc.ConditionSQL != null && rc.ConditionSQL.Length > 0)
                                {
                                    sb.Append(" and ");
                                    sb.AppendFormat(rc.ConditionSQL, (Guid)ute.Tag);
                                }
                                sql = sql.Replace("${" + rc.Name + "}", ((Guid)ute.Tag).ToString());
                            }
                            else
                                sql = sql.Replace("${" + rc.Name + "}", "");
                            break;
                        case "date":
                            UltraDateTimeEditor udte2 = (UltraDateTimeEditor)rc.Control;
                            if (sql.IndexOf("${" + rc.Name + "}") >= 0)
                                sql = sql.Replace("${" + rc.Name + "}", string.Format(rc.ConditionSQL, udte2.DateTime));
                            else if (rc.ConditionSQL != null && rc.ConditionSQL.Length > 0)
                            {
                                sb.Append(" and ");
                                sb.AppendFormat(rc.ConditionSQL, udte2.DateTime);
                            }
                            break;
                        case "period":
                            UltraDateTimeEditor udte = (UltraDateTimeEditor)rc.Control;
                            DateTime beginDate = new DateTime(udte.DateTime.Year, udte.DateTime.Month, 1);
                            DateTime endDate = beginDate.AddMonths(1);
                            if (rc.ConditionSQL != null && rc.ConditionSQL.Length > 0)
                            {
                                sb.Append(" and ");
                                sb.AppendFormat(rc.ConditionSQL, beginDate, endDate);
                            }
                            sql = sql.Replace("${" + rc.Name + "}", string.Format(rc.ConditionSQL, beginDate, endDate));
                            sql = sql.Replace("${" + rc.Name + ".begin}", string.Format("{0:yyyy-MM-dd}", beginDate));
                            sql = sql.Replace("${" + rc.Name + ".end}", string.Format("{0:yyyy-MM-dd}", endDate));
                            break;
                        case "int":
                            NumericUpDown nud = (NumericUpDown)rc.Control;
                            if (nud.Value == 0)
                                sql = sql.Replace("${" + rc.Name + "}", "");
                            else
                            {
                                if (sql.IndexOf("${" + rc.Name + "}") >= 0)
                                    sql = sql.Replace("${" + rc.Name + "}", string.Format(rc.ConditionSQL, nud.Value));
                                else if (rc.ConditionSQL != null && rc.ConditionSQL.Length > 0)
                                {
                                    sb.Append(" and ");
                                    sb.AppendFormat(rc.ConditionSQL, nud.Value);
                                }
                            }
                            break;
                        case "number":
                            UltraCurrencyEditor uce = (UltraCurrencyEditor)rc.Control;
                            if (uce.Value == 0)
                                sql = sql.Replace("${" + rc.Name + "}", "");
                            else
                            {
                                if (sql.IndexOf("${" + rc.Name + "}") >= 0)
                                    sql = sql.Replace("${" + rc.Name + "}", string.Format(rc.ConditionSQL, uce.Value));
                                else
                                    if (rc.ConditionSQL != null && rc.ConditionSQL.Length > 0)
                                    {
                                        sb.Append(" and ");
                                        sb.AppendFormat(rc.ConditionSQL, uce.Value);
                                    }
                            }
                            break;
                    }
                }
                //设置报表表格
                if (sb.Length > 0)
                {
                    sb.Remove(0, 5);
                    if (isAppend)
                        sql = string.Format("{0} {1} {2}", sql, _Report.Where, sb.ToString());
                    else
                        sql = sql.Replace("${Where}", string.Format("{0} {1}", _Report.Where, sb.ToString()));
                }
                else
                    sql = sql.Replace("${Where}", "");
                ds = CSystem.Sys.Svr.cntMain.Select(sql);
            }
            else if (bqe.Data == null)
                return;
            else
                ds = bqe.Data;

            //设置表格
            setGrid(_Report.Columns, grid, ds);
        }
        private void setGrid(List<ReportColumn> columns,UltraGrid grid,DataSet ds)
        {
            //设置表格
            grid.SetDataBinding(ds, null, true);
            foreach (UltraGridColumn column in grid.DisplayLayout.Bands[0].Columns)
            {
                column.Hidden = true;
            }
            int i = 0;
            bool NeedSummary = false;
             grid.DisplayLayout.Bands[0].UseRowLayout =true;
            grid.DisplayLayout.Bands[0].Summaries.Clear();
            foreach (ReportColumn rc in columns)
            {
                if (rc.Visible)
                {
                    if (!grid.DisplayLayout.Bands[0].Columns.Exists(rc.Name))
                    {
                        if (!rc.isLabel)
                            continue;
                        else
                        {
                            UltraGridColumn lableColumn = grid.DisplayLayout.Bands[0].Columns.Add(rc.Name);
                            lableColumn.Header.Caption = rc.Description;
                            lableColumn.CellActivation = Activation.Disabled;
                            lableColumn.SortIndicator = SortIndicator.Disabled;
                            lableColumn.RowLayoutColumnInfo.AllowCellSizing = RowLayoutSizing.None;
                            lableColumn.RowLayoutColumnInfo.LabelPosition=LabelPosition.LabelOnly;
                            lableColumn.RowLayoutColumnInfo.OriginX = rc.OriginXY[0];
                            lableColumn.RowLayoutColumnInfo.OriginY = rc.OriginXY[1];
                            lableColumn.RowLayoutColumnInfo.SpanX = rc.SpanXY[0];
                            lableColumn.RowLayoutColumnInfo.SpanY = rc.SpanXY[1];
                            continue;
                        }
                    }
                        
                    UltraGridColumn column = grid.DisplayLayout.Bands[0].Columns[rc.Name];
                    column.Hidden = false;
                    column.Header.Caption = rc.Description;
                    column.Header.VisiblePosition = i;
                    column.Header.Fixed = rc.Fixed;
                    if (rc.MergeCells)
                    {
                        column.MergedCellEvaluationType = MergedCellEvaluationType.MergeSameText;
                        column.MergedCellStyle = MergedCellStyle.Always;
                    }
                    if (rc.OriginXY == null)
                    {
                        column.RowLayoutColumnInfo.OriginX = i * 2;
                        column.RowLayoutColumnInfo.OriginY = 0;
                    }
                    else
                    {
                        column.RowLayoutColumnInfo.OriginX = rc.OriginXY[0];
                        column.RowLayoutColumnInfo.OriginY = rc.OriginXY[1];
                    }
                    column.RowLayoutColumnInfo.SpanX = rc.SpanXY[0];
                    column.RowLayoutColumnInfo.SpanY =rc.SpanXY[1];
                    if (rc.Format != null && rc.Format.Length > 0)
                        column.Format = rc.Format;
                    column.CellAppearance.TextHAlign = rc.Align;
                    
                    //grid.DisplayLayout.Bands[0].Columns.Add(column);
                    i++;
                    if (rc.DataType == null || rc.DataType.Length == 0)
                        continue;
                    string[] s = rc.DataType.Split(':');
                    switch (s[0].ToLower())
                    {
                        case "boolean":
                            //column.e = typeof(bool);
                            if (!grid.DisplayLayout.ValueLists.Exists("VL_Boolean"))
                            {
                                Infragistics.Win.ValueList list = grid.DisplayLayout.ValueLists.Add("VL_Boolean");
                                list.ValueListItems.Add(0, "否");
                                list.ValueListItems.Add(1, "是");
                            }
                            column.ValueList = grid.DisplayLayout.ValueLists["VL_Boolean"];
                            //column.Editor = new  Infragistics.Win.CheckEditor();
                            break;
                        case "int":
                            column.CellAppearance.TextHAlign = HAlign.Right;
                            column.Format = "#,##0;-#,##0; ";
                            break;
                        case "number":
                            column.CellAppearance.TextHAlign = HAlign.Right;
                            string[] d = s[1].Split(',');
                            column.Format = "#,##0." + new string('0', int.Parse(d[1])) + ";" + "-#,##0." + new string('0', int.Parse(d[1])) + "; ";
                            break;
                        case "enum":
                            {
                                string key = "VL_" + rc.Name;
                                if (!grid.DisplayLayout.ValueLists.Exists(key))
                                {
                                    string[] v = s[1].Split(',');
                                    Infragistics.Win.ValueList list = grid.DisplayLayout.ValueLists.Add("VL_" + rc.Name);
                                    for (int j = 0; j < v.Length; j++)
                                    {
                                        string[] n = v[j].Split('=');
                                        list.ValueListItems.Add(int.Parse(n[0]), n[1]);
                                    }
                                    column.ValueList = list;
                                }
                                else
                                    column.ValueList = grid.DisplayLayout.ValueLists[key];
                                break;
                            }
                    }
                    //设置合计行
                    if (rc.Summary > RptSummaryType.None)
                    {
                        NeedSummary = true;

                        SummarySettings ss = grid.DisplayLayout.Bands[0].Summaries.Add((SummaryType)rc.Summary, column, SummaryPosition.UseSummaryPositionColumn);
                        if (column.Format != null && column.Format.Length > 0)
                            ss.DisplayFormat = "{0:" + column.Format + "}";
                    }
                }
            }
            if (NeedSummary)
            {
                grid.DisplayLayout.Override.AllowRowSummaries = AllowRowSummaries.False;
                grid.DisplayLayout.Override.SummaryValueAppearance.TextHAlign = HAlign.Right;
                grid.DisplayLayout.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
            }
            grid.DisplayLayout.UseFixedHeaders = true;
            foreach (UltraGridColumn ugc in grid.DisplayLayout.Bands[0].Columns)
                ugc.PerformAutoResize();
        }
        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private bool CheckControl(Control a, Control b)
        {
            if (a == null || b == null)
                return false;
            if (a == b)
                return true;
            else
                return CheckControl(a, b.Parent);
        }
        private void toolExport_Click(object sender, EventArgs e)
        {
            UltraGrid grid;
            if (CheckControl(this.detailGrid,this.ActiveControl))
                grid = this.detailGrid;
            else
                grid = this.grid;
            if (grid != null)
            {
                if (grid.Rows.Count == 0)
                {
                    Msg.Information("没有数据不能导出！");
                    return;
                }
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.RestoreDirectory = true;
                sfd.Filter = "Excel文件(*.xls)|*.xls";
                sfd.DefaultExt = "xls";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Infragistics.Excel.Workbook w = new Infragistics.Excel.Workbook();
                    Infragistics.Excel.Worksheet ws = w.Worksheets.Add(this.Text);
                    int row = 0;
                    int column = 0;
                    Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter gridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter();
                    if (BeforeExport != null)
                    {
                        if (!BeforeExport(gridExcelExporter, grid, ws, ref row, ref column))
                        {
                            Msg.Information("不支持导出！");
                            return;
                        }
                    }

                    gridExcelExporter.CellExported += new Infragistics.Win.UltraWinGrid.ExcelExport.CellExportedEventHandler(gridExcelExporter_CellExported);
                    gridExcelExporter.Export(grid, ws, row, column);

                    if (AfterExport != null)
                        AfterExport(grid, ws);
                    Infragistics.Excel.BIFF8Writer.WriteWorkbookToFile(w, sfd.FileName);
                }
            }
        }
        void gridExcelExporter_CellExported(object sender, Infragistics.Win.UltraWinGrid.ExcelExport.CellExportedEventArgs e)
        {
            if (e.Value != null && e.Value.GetType() == typeof(DateTime))
            {
                Infragistics.Excel.IWorksheetCellFormat format = e.CurrentWorksheet.Rows[e.CurrentRowIndex].Cells[e.CurrentColumnIndex].CellFormat;
                format.FormatString = "yyyy-MM-dd";
            }
        }

        private void frmReport_Load(object sender, EventArgs e)
        {
            if (!_Report.Inited)
                _Report.Initialize();
            else if (_Report.Closed)
                _Report.Reshow();
            //InitializeControl(_MDIForm);
        }

        private void frmReport_FormClosed(object sender, FormClosedEventArgs e)
        {
            _Report.Closed = true;
        }

    }

}