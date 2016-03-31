using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Transactions;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Excel;
using Base;

namespace UI
{
    public partial class frmDetail : Form
    {
        private const string fieldMainID = "MainID";
        private const string fieldLineNumber = "LineNumber";
        
        private bool _IsNew = false;
        private bool _IsEdit = false;
        private bool _HasUp = false;
        private bool _HasDown = false;
        private bool _CanPage = true;
        private bool _Showing = false;
        private DataSet _MainDataSet;
        private DataSet BackupDataSet;
        //private bool _AutoCode = false;
        private Guid _MainID;
        private string _Path;
        private bool _IsHistory;

        private COMFields _MainTableDefine;
        private List<COMFields> _DetailTableDefine;
        private CCreateDetailForm _CreateForm;
        //private COMFields _MainTableDefine;
        private SortedList<string, SortedList<string, object>> _DefaultValue;
        private SortedList<string, string> _CopyLastValue;
        //private ISelectForm toolDetailForm;
        //private IDetailFormHandler toolDetailForm;
        private ToolDetailForm _ToolDetailForm;
        private SortedList<string, UltraGrid> _GridMap = new SortedList<string, UltraGrid>();
        private SortedList<string, Control> _ControlMap = null;
    
        public event DataTableEventHandler Changed;
        public event DataTableEventHandler PageUp;
        public event DataTableEventHandler PageDown;

        public Panel panelMain;
        public Panel panelDetail;

        public bool Showed = false;
        public frmDetail()
        {
            InitializeComponent();
        }
        public bool IsHistory
        {
            set { _IsHistory = value; }
        }
        public Guid MainID
        {
            get { return _MainID; }
        }
        public SortedList<string, UltraGrid> GridMap
        {
            get { return _GridMap; }
        }
        public SortedList<string, SortedList<string, object>> DefaultValue
        {
            set
            {
                _DefaultValue = value;
            }
        }
        public SortedList<string, string> CopyLastValue
        {
            set
            {
                _CopyLastValue = value;
            }
        }
        public ToolDetailForm toolDetailForm
        {
            set { _ToolDetailForm = value; }
            get
            {
                if (_ToolDetailForm == null)
                {
                    _ToolDetailForm = new ToolDetailForm();
                }
                return _ToolDetailForm;
            }
        }
        //public ISelectForm SelectForm
        //{
        //    set { toolDetailForm = value; }
        //}
        //public IDetailFormHandler DetailFormHandler
        //{
        //    set { toolDetailForm = value; }
        //}
        //public bool AutoCode
        //{
        //    set { _AutoCode = value; }
        //}
        //protected virtual string GetPrefix()
        //{
        //    return "";
        //}
        public string GetCode(string prefix)
        {
            string p = prefix;
            string AutoCodeFormat;

            AutoCodeFormat = toolDetailForm.AutoCodeFormat;
            if (toolDetailForm.DateInCode)
                p += CSystem.Sys.Svr.SystemTime.ToString(AutoCodeFormat);
            string f = "0".PadLeft(toolDetailForm.LengthOfCode,'0');
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select Max(Code) from {0} where Code like '{1}%'", _MainTableDefine.OrinalTableName, p));
            if (ds.Tables[0].Rows[0][0] == DBNull.Value)
            {
                return p + string.Format("{0:" + f + "}", 1);
            }
            else
            {
                string v = (string)ds.Tables[0].Rows[0][0];
                string s = "";
                for (int j = p.Length; j < v.Length && j < p.Length + toolDetailForm.LengthOfCode; j++)
                    if (v[j] >= '0' && v[j] <= '9')
                        s += v[j];
                //string s = ((string)ds.Tables[0].Rows[0][0]).Substring(p.Length);
                int i = int.Parse(s);
                return p + string.Format("{0:" + f + "}", i + 1);
            }
        }
        public string GetCode(string prefix, SqlConnection conn, SqlTransaction tran)
        {
            string p = prefix;
            if (toolDetailForm.DateInCode)
                p+=CSystem.Sys.Svr.SystemTime.ToString("yyyyMM");
            string f = "0".PadLeft(toolDetailForm.LengthOfCode, '0');
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select Max(Code) from {0} where Code like '{1}%'", _MainTableDefine.OrinalTableName, prefix), conn, tran);
            if (ds.Tables[0].Rows[0][0] == DBNull.Value || ((string)ds.Tables[0].Rows[0][0]).Length<8)
            {
                return p + string.Format("{0:" + f + "}", 1);
            }
            else
            {
                string v=(string)ds.Tables[0].Rows[0][0];
                string s="";
                for (int j = p.Length; j < v.Length && j < p.Length + toolDetailForm.LengthOfCode; j++)
                    if (v[j] >= '0' && v[j] <= '9')
                        s += v[j];
                //string s = ((string)ds.Tables[0].Rows[0][0]).Substring(p.Length);
                int i = int.Parse(s);
                return p + string.Format("{0:" + f + "}", i + 1);
            }
        }
        public bool CanPage
        {
            set { _CanPage = value; }
        }
        public bool IsChecked
        {
            get
            {
                if (_MainTableDefine.OrinalTableName == "D_ProjectProcess" || _MainTableDefine.OrinalTableName == "D_GuaranteeFee" || _MainTableDefine.OrinalTableName == "D_Repayment")
                {
                    if (MainDataSet.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("BillStatus"))
                        return ((int)MainDataSet.Tables[_MainTableDefine.OrinalTableName].Rows[0]["BillStatus"] == 1);
                    else
                        return false;
                }
                else
                {
                    if (MainDataSet.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("CheckStatus"))
                        return ((int)MainDataSet.Tables[_MainTableDefine.OrinalTableName].Rows[0]["CheckStatus"] == 1);
                    else
                        return false;
                }
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool needSend = !(ActiveControl is Button);
            if (needSend)
            {
                if (ActiveControl is Infragistics.Win.EmbeddableTextBoxWithUIPermissions)
                {
                    UltraGrid grid = ActiveControl.Parent as UltraGrid;
                    if (grid != null)
                    {
                        if (grid.ActiveCell != null)
                        {
                            Infragistics.Win.EditorWithText edit = grid.ActiveCell.Editor as Infragistics.Win.EditorWithText;
                            if (edit == null) edit = grid.ActiveCell.Column.Editor as Infragistics.Win.EditorWithText;
                            if (edit != null)
                            {
                                needSend = edit.ButtonsRight.Count == 0;
                            }
                        }
                    }
                    else
                    {
                        UltraTextEditor edit = ActiveControl.Parent as UltraTextEditor;
                        if (edit != null)
                        {
                            needSend = edit.ButtonsRight.Count == 0;
                        }
                    }
                }
            }
            if (needSend && (keyData == Keys.Enter || keyData == (Keys.Enter | Keys.Shift)))
            {
                if (keyData == Keys.Enter)
                    SendKeys.Send("{tab}");
                else
                    SendKeys.Send("+{tab}");
                return true;
            }
            else if (keyData == (Keys.Control | Keys.W))
            {
                toolClose.PerformClick();
                return true;
            }
             else
                return base.ProcessCmdKey(ref msg, keyData);
        }
        public bool IsEdit
        {
            get { return _IsEdit; }
            set
            {
                _IsEdit = value;
                if (_ToolDetailForm != null)
                    _ToolDetailForm.EditChanged(_IsEdit);
            }
        }
        public bool IsNew
        {
            get { return _IsNew; }
        }
        public DataSet MainDataSet
        {
            get { return _MainDataSet; }
        }

        public SortedList<string, Control> ControlMap
        {
            get { return _ControlMap; }
        }
        //protected virtual void InsertToolStrip(ToolStrip toolStrip)
        //{
        //}
        public string MainTable
        {
            get { return _MainTableDefine.OrinalTableName; }
        }
        public List<COMFields> DetailTableDefine
        {
            get { return _DetailTableDefine; }
        }
        public COMFields GetDetailTableDefine(string TableName)
        {
            foreach (COMFields f in _DetailTableDefine)
                if (string.Compare(f.OrinalTableName, TableName, true)==0)
                    return f;
            return null;
        }
        protected virtual bool NewData(DataRow dr)
        {
            return true;
        }
        private void RemoveDataBinding(SortedList<string, Control> controlMap)
        {
            foreach (Control ctl in controlMap.Values)
                ctl.DataBindings.Clear();
        }
        private void AddDataBinding(SortedList<string, Control> controlMap,DataTable dataTable)
        {
            foreach (KeyValuePair<string, Control> o in controlMap)
            {
                Type t = o.Value.GetType();
                if (t==typeof(TextBox))
                    o.Value.DataBindings.Add("Text", dataTable, o.Key, false, DataSourceUpdateMode.OnPropertyChanged);
                else if (t == typeof(UltraTextEditor))
                    o.Value.DataBindings.Add("Text", dataTable, o.Key, false, DataSourceUpdateMode.OnPropertyChanged);
                else if (t == typeof(UltraCurrencyEditor))
                    o.Value.DataBindings.Add("Value", dataTable, o.Key, false, DataSourceUpdateMode.OnPropertyChanged);
                else if (t == typeof(NumericUpDown))
                    o.Value.DataBindings.Add("Value", dataTable, o.Key, false, DataSourceUpdateMode.OnPropertyChanged);
                else if (t == typeof(ComboBox))
                {
                    o.Value.DataBindings.Add("Tag", dataTable, o.Key, false, DataSourceUpdateMode.OnPropertyChanged);
                    try
                    {
                        ComboBox cmb = (ComboBox)o.Value;
                        if (dataTable.Rows[0][o.Key] == DBNull.Value)
                            cmb.SelectedIndex = 0;
                        for (int i = 0; i < cmb.Items.Count; i++)
                        {
                            IntItem item = (IntItem)cmb.Items[i];
                            if (item.Int == (int)dataTable.Rows[0][o.Key])
                            {
                                cmb.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    catch { }
                }
                else if (t == typeof(CheckBox))
                    o.Value.DataBindings.Add("Checked", dataTable, o.Key, false, DataSourceUpdateMode.OnPropertyChanged);
                else if (t == typeof(UltraDateTimeEditor))
                    o.Value.DataBindings.Add("Value", dataTable, o.Key, false, DataSourceUpdateMode.OnPropertyChanged);
            }
        }
        
        protected virtual bool NewData(DataSet ds)
        {
            IsEdit = true;
            _IsNew = true;
            DataRow dr = ds.Tables[_MainTableDefine.OrinalTableName].NewRow();
            _MainID = System.Guid.NewGuid();
            dr["ID"] = _MainID;
            if (ds.Tables[MainTable].Columns.Contains("Createdby"))
                dr["Createdby"] = CSystem.Sys.Svr.User;
            if (ds.Tables[MainTable].Columns.Contains("CreatedbyName"))
                dr["CreatedbyName"] = CSystem.Sys.Svr.UserName;
            if (ds.Tables[MainTable].Columns.Contains("CreateDate"))
                dr["CreateDate"] = CSystem.Sys.Svr.SystemTime;
            
            if (NewData(dr))
            {
                setDefaultValue(dr, ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]);
                RemoveDataBinding(_ControlMap);
                ds.Tables[_MainTableDefine.OrinalTableName].Rows.Add(dr);
                ds.Tables[_MainTableDefine.OrinalTableName].Rows.RemoveAt(0);


                if (toolDetailForm.NewData(ds, this._MainTableDefine, this._DetailTableDefine))
                {
                    AddDataBinding(_ControlMap, ds.Tables[_MainTableDefine.OrinalTableName]);
                    foreach (DataTable dt in ds.Tables)
                        if (dt.TableName != _MainTableDefine.OrinalTableName)
                            dt.Rows.Clear();
                    if (_DefaultValue != null)
                        foreach (string tableName in _DefaultValue.Keys)
                            if (MainDataSet.Tables.Contains(tableName))
                            {
                                foreach (string fieldName in _DefaultValue[tableName].Keys)
                                    if (MainDataSet.Tables[tableName].Columns.Contains(fieldName))
                                    {
                                        foreach (DataRow dr2 in MainDataSet.Tables[tableName].Rows)
                                            dr2[fieldName] = _DefaultValue[tableName][fieldName];
                                    }
                            }
                }
            }

            if (!CodeRule(dr) && toolDetailForm.AutoCode && ds.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("Code"))
                dr["Code"] = GetCode(toolDetailForm.GetPrefix());
            return true;
        }
        protected virtual bool InitData(DataSet ds)
        {
            if (ds.Tables[_MainTableDefine.OrinalTableName].Rows.Count == 0)
            {
                _IsNew = true;
                DataRow dr = ds.Tables[_MainTableDefine.OrinalTableName].NewRow();
                if (ds.Tables[MainTable].Columns.Contains("ID"))
                {
                    _MainID = System.Guid.NewGuid();
                    dr["ID"] = _MainID;
                }
                //Add by jiayuan 2012/11/21 增加对S_Table和S_TableField的处理
                if (_MainTableDefine.OrinalTableName.Equals("S_Table"))
                    dr["TableName"] = "New Table";

                if (ds.Tables[MainTable].Columns.Contains("Createdby"))
                    dr["Createdby"] = CSystem.Sys.Svr.User;
                if (ds.Tables[MainTable].Columns.Contains("CreatedbyName"))
                    dr["CreatedbyName"] = CSystem.Sys.Svr.UserName;
                if (ds.Tables[MainTable].Columns.Contains("CreateDate"))
                    dr["CreateDate"] = CSystem.Sys.Svr.SystemTime;

                ds.Tables[_MainTableDefine.OrinalTableName].Rows.Add(dr);

                setDefaultValue(dr, null);
                if (_DefaultValue != null)
                    foreach (string tableName in _DefaultValue.Keys)
                        if (MainDataSet.Tables.Contains(tableName))
                        {
                            foreach (string fieldName in _DefaultValue[tableName].Keys)
                                if (MainDataSet.Tables[tableName].Columns.Contains(fieldName))
                                {
                                    foreach (DataRow dr2 in MainDataSet.Tables[tableName].Rows)
                                        dr2[fieldName] = _DefaultValue[tableName][fieldName];
                                }
                        }
                if (_DetailTableDefine!=null)
                    foreach (COMFields d in _DetailTableDefine)
                    {
                        string sql = d.QuerySQL + " where 0=1";
                        CSystem.Sys.Svr.cntMain.Select(sql, d.OrinalTableName, ds);
                    }

                if (!CodeRule(dr) && toolDetailForm.AutoCode && ds.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("Code"))
                    dr["Code"] = GetCode(toolDetailForm.GetPrefix());

                return toolDetailForm.NewData(ds, this._MainTableDefine, this._DetailTableDefine);
            }
            else
            {
                COMField IDField = new COMField();

                if (CSystem.Sys.Svr.LDI.GetFields(_MainTableDefine.OrinalTableName).FieldsName.Contains("ID"))
                    IDField = CSystem.Sys.Svr.LDI.GetFields(_MainTableDefine.OrinalTableName)["ID"];

                if (IDField.ValueType.Contains("String"))
                //if ((_MainTableDefine.OrinalTableName.Equals("S_Table")) || (_MainTableDefine.OrinalTableName.Equals("S_TableField")) ||(_MainTableDefine.OrinalTableName.Equals("S_BaseInfo")))
                {
                    string _MainKey ;
                    if (_MainTableDefine.OrinalTableName.Equals("S_Table"))
                        _MainKey = (String)ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["TableName"];
                    else
                         _MainKey = (String)ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["ID"];

                    CodeRule(ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]);
                    if (ds.Tables[_MainTableDefine.OrinalTableName].Rows[0].RowState != DataRowState.Added)
                    {
                        if (_DetailTableDefine!=null)
                            foreach (COMFields d in _DetailTableDefine)
                            {
                                string sql = d.GetQuerySQL(_IsHistory) + string.Format(" where {0}.{1}='{2}' ", d.OrinalTableName, "MainID", _MainID);
                                string orderBy = d.OrderBy;
                                if (orderBy.Length > 0)
                                    sql = sql + " Order by " + orderBy;
                                CSystem.Sys.Svr.cntMain.Select(sql, d.OrinalTableName, ds);
                            }
                    }
                    else
                    {
                        _IsNew = true;
                        if (!CodeRule(ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]) && toolDetailForm.AutoCode && ds.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("Code"))
                        {
                            if (ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["Code"] == DBNull.Value)
                                ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["Code"] = GetCode(toolDetailForm.GetPrefix());
                        }
                    }
                
                }
                else
                {
                    _MainID = (Guid)ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["ID"];
                    CodeRule(ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]);
                    if (ds.Tables[_MainTableDefine.OrinalTableName].Rows[0].RowState != DataRowState.Added)
                    {
                        if (_DetailTableDefine!=null)
                            foreach (COMFields d in _DetailTableDefine)
                            {
                                string sql = d.GetQuerySQL(_IsHistory) + string.Format(" where {0}.{1}='{2}' ", d.OrinalTableName, "MainID", _MainID);
                                string orderBy = d.OrderBy;
                                if (orderBy.Length > 0)
                                    sql = sql + " Order by " + orderBy;
                                CSystem.Sys.Svr.cntMain.Select(sql, d.OrinalTableName, ds);
                            }
                    }
                    else
                    {
                        _IsNew = true;
                        if (!CodeRule(ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]) && toolDetailForm.AutoCode && ds.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("Code"))
                        {
                            if (ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["Code"] == DBNull.Value)
                                ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["Code"] = GetCode(toolDetailForm.GetPrefix());
                        }
                    }
                }

            }
            return true;
        }
        public void setDefaultValue(DataRow dr,DataRow LastDr)
        {
            foreach (COMField f in _MainTableDefine.Fields)
            {
                if (f.FieldName == "Code") continue;
                string[] s = f.ValueType.Split(':');
                if (_CopyLastValue != null && _CopyLastValue.ContainsKey(f.FieldName) && LastDr != null)
                    dr[f.FieldName] = LastDr[f.FieldName];
                else
                {
                    switch (s[0].ToLower())
                    {
                        case "boolean":
                            if (f.DefaultValue.Length == 0)

                                dr[f.FieldName] = 0;
                            else
                                dr[f.FieldName] = int.Parse(f.DefaultValue);

                            break;
                        case "int":
                        case "enum":
                            if (f.DefaultValue.Length == 0)
                                dr[f.FieldName] = 0;
                            else
                                dr[f.FieldName] = int.Parse(f.DefaultValue);
                            break;
                        case "number":
                            if (f.DefaultValue.Length == 0)
                                dr[f.FieldName] = 0;

                            else
                                dr[f.FieldName] = decimal.Parse(f.DefaultValue);

                            break;
                        case "string":
                            if (f.DefaultValue.Length == 0)
                                dr[f.FieldName] = "";

                            else
                                dr[f.FieldName] = f.DefaultValue;
                            break;
                    }
                }
            }
        }
        public void setDefaultValueWhenNull(COMFields TableDefine, DataRow dr,SortedList<string, object> defaultValue)
        {
            foreach (COMField f in TableDefine.Fields)
            {
                if (dr[f.FieldName] == DBNull.Value)
                {
                    if (defaultValue != null && defaultValue.ContainsKey(f.FieldName))
                    {

                        dr[f.FieldName] = defaultValue[f.FieldName];
                    }
                    else
                    {
                        string[] s = f.ValueType.Split(':');
                        switch (s[0].ToLower())
                        {
                            case "boolean":
                                if (dr[f.FieldName] == DBNull.Value && f.DefaultValue.Length != 0)
                                    dr[f.FieldName] = int.Parse(f.DefaultValue);
                                break;
                            case "int":
                            case "enum":
                                if (dr[f.FieldName] == DBNull.Value && f.DefaultValue.Length != 0)
                                    dr[f.FieldName] = int.Parse(f.DefaultValue);
                                break;
                            case "number":
                                if (dr[f.FieldName] == DBNull.Value && f.DefaultValue.Length != 0)
                                    dr[f.FieldName] = decimal.Parse(f.DefaultValue);
                                break;
                            case "string":
                                if (dr[f.FieldName] == DBNull.Value && f.DefaultValue.Length != 0)
                                    dr[f.FieldName] = f.DefaultValue;
                                break;
                        }
                    }
                }
            }
        }
        protected virtual void DataBinding()
        {
            toolDetailForm.Initialization();
            //COMFields _MainTableDefine = CSystem.Sys.Svr.LDI.GetFields(_MainTableDefine);
            int ShowedFieldCount = getShowedFieldCount(_MainTableDefine);
            if (DetailTableDefine!=null && DetailTableDefine.Count == 0)
            {
                if (ShowedFieldCount > 21)
                {
                    if (MdiParent == null)
                    {
                        this.Width = 800;
                        this.Height = 600;
                    }
                    else
                    {
                        this.Width = MdiParent.Width;
                    }
                }
            }
            else
            {
                if (MdiParent == null)
                {
                    this.Width = 800;
                    this.Height = 600;
                }
                else
                {
                    this.Width = MdiParent.Width;
                }
            }
            _CreateForm = new CCreateDetailForm();
            _CreateForm.CaptionWidth = toolDetailForm.CaptionWidth;
            _CreateForm.DataWidth = toolDetailForm.DataWidth;

            //设置主表
            panelMain = new Panel();
            panelMain.Top = toolMain.Height+1;
            panelMain.Left = 0;
            panelMain.Width = this.Width;
            if (DetailTableDefine!=null && DetailTableDefine.Count == 0)
            {
                panelMain.Height = this.Height - toolMain.Height - 1;
                panelMain.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            }
            else
            {
                if (ShowedFieldCount > 21)
                {
                    panelMain.Height = this.Height / 2;
                    panelMain.AutoScroll = true;
                }
                else
                {
                    panelMain.Height = (((ShowedFieldCount - 1) /_CreateForm.GetColumn(this.Width)) + 1) * 25 + 10;
                    //panelMain.BorderStyle = BorderStyle.FixedSingle;
                }
            }
            this.Controls.Add(panelMain);


            panelMain.Height = _CreateForm.CreateDetail(_MainTableDefine, this.panelMain, MainDataSet.Tables[_MainTableDefine.OrinalTableName], toolDetailForm);
            _ControlMap = _CreateForm.ControlMap;
            toolDetailForm.SetCreateDetail(_CreateForm); 

            //设从表界面
            if (DetailTableDefine != null && _DetailTableDefine.Count > 0)
            {
                //toolCopyLine.Visible = true;
                panelDetail = new Panel();
                panelDetail.Top = panelMain.Top + panelMain.Height;
                panelDetail.Height = this.Height - panelDetail.Top - 10;
                panelDetail.Width = this.Width - 8;
                panelDetail.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                this.Controls.Add(panelDetail);
                if (_DetailTableDefine.Count > 1)
                {
                    toolDetailForm.SetDetailForm(this);
                    //取得从表的分组情况
                    int group = 0;
                    int lastGroup = -1;
                    SortedList<int, TabControl> tabControls = new SortedList<int, TabControl>();
                    foreach (COMFields fields in _DetailTableDefine)
                        if (fields.Property.GroupBy != lastGroup)
                        {
                            lastGroup = fields.Property.GroupBy;
                            group++;
                            TabControl tab = new TabControl();
                            tabControls.Add(lastGroup, tab);
                            panelDetail.Controls.Add(tab);
                            //如果设为Fill将会造成其他的Detail不能显示
                            //tab.Dock = DockStyle.Fill;
                        }
                    //当只有一个TabControl时
                    //贷款系统的某个界面，出现没有显示全的问题，设为Fill就解决了。
                    if (tabControls.Count == 1)
                        tabControls.Values[0].Dock = DockStyle.Fill;
                    else
                        panelDetail.Resize += new EventHandler(panelDetail_Resize);
                    foreach (COMFields fields in _DetailTableDefine)
                    {
                        //ToolDetailForm检测是否需要用Grid显示
                        if (toolDetailForm.IsShow(fields.OrinalTableName))
                        {
                            string title = fields.Property.Title;
                            TabPage tPage = new TabPage(title);
                            tabControls[fields.Property.GroupBy].Controls.Add(tPage);
                            UltraGrid grid = new UltraGrid();
                            grid.Dock = DockStyle.Fill;
                            if (toolDetailForm.AllowSort(fields.OrinalTableName))
                                grid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
                            grid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
                            tPage.Controls.Add(grid);
                            CCreateGrid createGrid = new CCreateGrid(fields, grid, MainDataSet.Tables[fields.OrinalTableName], COMField.Enum_Visible.VisibleInDetail, toolDetailForm);

                            grid.AfterRowInsert += new RowEventHandler(grid_AfterRowInsert);
                            toolDetailForm.SetCreateGrid(createGrid);
                            toolDetailForm.NewGrid(fields, grid);

                            _GridMap.Add(fields.OrinalTableName, grid);
                            SetColumn(grid, fields);
                            grid.AfterColPosChanged += new AfterColPosChangedEventHandler(grid_AfterColPosChanged);
                        }
                    }
                }
                else
                {

                    COMFields fields = _DetailTableDefine[0];
                    if (toolDetailForm.IsShow(fields.OrinalTableName))
                    {
                        UltraGrid grid = new UltraGrid();
                        grid.Dock = DockStyle.Fill;
                        if (toolDetailForm.AllowSort(fields.OrinalTableName))
                            grid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
                        grid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
                        panelDetail.Controls.Add(grid);
                        CCreateGrid createGrid = new CCreateGrid(fields, grid, MainDataSet.Tables[fields.OrinalTableName], COMField.Enum_Visible.VisibleInDetail, toolDetailForm);

                        grid.AfterRowInsert+=new RowEventHandler(grid_AfterRowInsert);
                        toolDetailForm.SetCreateGrid(createGrid);
                        toolDetailForm.SetDetailForm(this);


                        toolDetailForm.NewGrid(fields, grid);

                        _GridMap.Add(fields.OrinalTableName, grid);
                        SetColumn(grid, fields);
                        grid.AfterColPosChanged += new AfterColPosChangedEventHandler(grid_AfterColPosChanged);
                    }
                }
                toolDetailForm.SetGridMap(_GridMap);
                if (panelDetail.Controls.Count > 0)
                    this.Controls.Add(panelDetail);
            }
            else
            {
                toolCopyLine.Visible = false;
                toolDetailForm.SetDetailForm(this);
            }
        }

        void grid_AfterRowInsert(object sender, RowEventArgs e)
        {
            UltraGrid grid=(UltraGrid)sender;
            COMFields fields=(COMFields)grid.Tag;
            //设置默认值
            if (fields.Property.TableType != CTableProperty.enuTableType.View)
            {

                SortedList<string, object> defaultValue = null;
                if (_DefaultValue != null && _DefaultValue.ContainsKey(fields.OrinalTableName))
                    defaultValue = _DefaultValue[fields.OrinalTableName];
                if (defaultValue == null)
                    return;
                foreach (COMField f in fields.Fields)
                {
                    if (e.Row.Cells[f.FieldName].Value == DBNull.Value)
                    {
                        if (defaultValue != null && defaultValue.ContainsKey(f.FieldName))
                        {

                            e.Row.Cells[f.FieldName].Value = defaultValue[f.FieldName];
                        }
                    }
                }
            }
        }
        public string Path
        {
            set { _Path = value; }
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
                            Directory.CreateDirectory(path);
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
        private string FullPath
        {
            get { return _Path + "\\Detail\\"; }
        }
        void grid_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            UltraGrid grid = (UltraGrid)sender;
            //保存各列的宽度及顺序
            if (CheckPath(FullPath))
            {
                COMFields fields=(COMFields)grid.Tag;
                string fileName=FullPath+fields.OrinalTableName+".xml";
                clsListConfig config = new clsListConfig(fileName, true);
                ColumnsCollection cols = grid.DisplayLayout.Bands[0].Columns;
                foreach (UltraGridColumn col in cols)
                {
                    clsColumn c = config.NewColumn(col.Key);
                    c.NewDescription = col.Header.Caption;
                    c.OldDescription = col.Header.Caption;
                    c.Width = col.Width;
                    c.Hidden = col.Hidden;
                    c.LineNumber = col.Header.VisiblePosition;
                }
                config.Save();
            }
        }
        private void SetColumn(UltraGrid grid,COMFields fields)
        {
            string fileName=FullPath+fields.OrinalTableName+".xml";
            if (File.Exists(fileName))
            {
                clsListConfig config = new clsListConfig(fileName, false);
                //设置列
                List<clsColumn> listColumn = new List<clsColumn>();
                listColumn.AddRange(config.Columns.Values);
                listColumn.Sort(new ColumnCompare());
                ColumnsCollection cols = grid.DisplayLayout.Bands[0].Columns;
                int i = 0;
                foreach (clsColumn col in listColumn)
                {
                    try
                    {
                        if (cols.Exists(col.FieldName))
                        {
                            cols[col.FieldName].Header.VisiblePosition = i;
                            cols[col.FieldName].Header.Caption = col.NewDescription;
                            cols[col.FieldName].Width = col.Width;
                            cols[col.FieldName].Hidden = col.Hidden;
                        }
                    }
                    catch { }
                    i++;
                }
            }
        }
        private class ColumnCompare : IComparer<clsColumn>
        {
            #region IComparer<clsColumn> Members

            public int Compare(clsColumn x, clsColumn y)
            {
                return x.LineNumber.CompareTo(y.LineNumber);
            }

            #endregion
        }

        void panelDetail_Resize(object sender, EventArgs e)
        {
            if (panelDetail.Controls.Count == 0)
                return;
            int height = panelDetail.Height / panelDetail.Controls.Count;
            for (int i = 0; i < panelDetail.Controls.Count; i++)
            {
                Control ctl = panelDetail.Controls[i];
                ctl.Top = i * height;
                ctl.Height = height;
                ctl.Width = panelDetail.Width;
            }
        }
        private int getShowedFieldCount(COMFields fields)
        {
            int count=0;
            foreach (COMField f in fields.Fields)
                if ((f.Visible& COMField.Enum_Visible.VisibleInDetail)== COMField.Enum_Visible.VisibleInDetail)
                    count++;
            return count;
        }
        protected virtual void AfterDataBind()
        {
            //检索出所有ComboBox,重新刷一下
            foreach (KeyValuePair<string,Control> v in _ControlMap)
            {
                if (v.Value.GetType() == typeof(ComboBox))
                {
                    ComboBox cmb = (ComboBox)v.Value;
                    CCreateDetailForm.SetCmbItemByInt(cmb, (int)_MainDataSet.Tables[_MainTableDefine.OrinalTableName].Rows[0][v.Key]);
                    //cmb.DataBindings.Clear();
                    //cmb.DataBindings.Add("Checked", MainDataSet.Tables[MainTable], "IsTop", false, DataSourceUpdateMode.OnPropertyChanged);
                }
            }
            //更新复核状态
            if (IsChecked)
                toolCheck.Text = "取消复核";
            else
                toolCheck.Text = "复核";

            if (_ToolDetailForm != null)
                _ToolDetailForm.AfterDataBind();
        }
        private void SetToolBar()
        {
            if (_IsHistory)
            {
                toolNew.Visible = false;
                toolInsertRow.Visible = false;
                toolDeleteRow.Visible = false;
                toolEdit.Visible = false;
                toolDelete.Visible = false;
                toolCheck.Visible = false;
            }
            else
            {
                toolNew.Visible = toolDetailForm.AllowNew;
                toolContinue.Visible = toolDetailForm.AllowNew;
                toolInsertRow.Visible = toolDetailForm.AllowInsertRowInToolBar;
                toolDeleteRow.Visible = toolDetailForm.AllowDeleteRowInToolBar;
                toolEdit.Visible = toolDetailForm.AllowEdit;
                toolDelete.Visible = toolDetailForm.AllowEdit;
                toolCheck.Visible = toolDetailForm.AllowCheck;
            }
        }
        public bool ShowData(COMFields mainTableDefine,List<COMFields> detailTableDefine, DataSetEventArgs data, bool IsEdit,Form MDIParent,string FormCaption)
        {
            if (FormCaption == null)
                this.Text = mainTableDefine.Property.Title;
            else
                this.Text = FormCaption;

            _MainTableDefine = mainTableDefine;
            _DetailTableDefine = detailTableDefine;
            _HasDown = data.HasNext;
            _HasUp = data.HasPrevious;
            _MainDataSet = data.Data;
            if (!InitData(data.Data))
            {
                return false;
            }
            BackupDataSet = _MainDataSet.Copy();

            this.MdiParent = MDIParent;

            //根据字段设置相关控件和绑定
            DataBinding();

            toolDetailForm.InsertToolStrip(this.toolMain, ToolDetailForm.enuInsertToolStripType.Detail);
            SetToolBar();
            this.updateTool(IsEdit);
            if (MDIParent == null)
                this.ShowDialog();
            else
            {
                //this.MdiParent = MDIParent;
                this.Show();
            }
            return true;
        }

        public bool ShowData(COMFields mainTableDefine, List<COMFields> detailTableDefine, DataSetEventArgs data, bool IsEdit, Form MDIParent)
        {
            return ShowData(mainTableDefine, detailTableDefine, data, IsEdit, MDIParent, null);
        }

        public void RefreshControl()
        {
            //先清除
            this.Controls.Remove(panelMain);
            if (panelDetail != null)
                this.Controls.Remove(panelDetail);
            //for (int i = this.panelMain.Controls.Count - 1; i >= 0; i--)
            //    this.panelMain.Controls.RemoveAt(i);
            //for (int i = this.panelDetail.Controls.Count - 1; i >= 0; i--)
            //    this.panelDetail.Controls.RemoveAt(i);

            _CreateForm.ControlMap.Clear();
            _CreateForm.LabelMap.Clear();
            _GridMap.Clear();

            //再重新绑定
            DataBinding();

            toolDetailForm.InsertToolStrip(this.toolMain, ToolDetailForm.enuInsertToolStripType.Detail);
            SetToolBar();
            this.updateTool(IsEdit);

        }
        public void updateTool(bool Edit)
        {
            IsEdit = Edit;
            updateTool();
            IsEdit = Edit;
        }
        private void updateTool()
        {
            AfterEditModeChanged(IsEdit);

            CMenuRight mr;
            bool UserAllowCreate;
            bool UserAllowEdit;
            bool UserAllowDelete;
            bool UserAllowCheck;

            //2014/07/07 增加了对菜单操作权限的控制
            string MenuRightTableName = _MainTableDefine.GetTableName(false);
            mr = CSystem.Sys.Svr.Operator.GetMenuRightValue(MenuRightTableName);
            if (mr != null)
            {
                UserAllowCreate = mr.AllowCreate;
                UserAllowEdit = mr.AllowEdit;
                UserAllowDelete = mr.AllowDelete;
                UserAllowCheck = mr.AllowCheck;
            }
            else
            {
                UserAllowCreate = false;
                UserAllowEdit = false;
                UserAllowDelete = false;
                UserAllowCheck = false;

            }

            if (IsChecked)
                toolCheck.Text = "取消复核";
            else
                toolCheck.Text = "复核";

            int rightValue = 0;
            if (toolDetailForm.Right != null)
                rightValue = toolDetailForm.Right.RightValue;
            
            if (rightValue == -1)
            {
                toolNew.Enabled = !IsEdit;
                toolCancel.Enabled = IsEdit;
                toolSave.Enabled = IsEdit;
                toolContinue.Enabled = IsEdit;
                toolEdit.Enabled = !IsEdit;
                toolDeleteRow.Enabled = IsEdit;
                toolInsertRow.Enabled = IsEdit;
                toolClose.Enabled = true;
                toolDelete.Enabled = !IsEdit && !IsChecked;
                toolUp.Enabled = !IsEdit && _HasUp && _CanPage;
                toolDown.Enabled = !IsEdit && _HasDown && _CanPage;
                toolCopyLine.Enabled = IsEdit;

            }
            else
            {
                //toolNew.Enabled = (rightValue & 1) == 1 && !IsEdit;
                //toolEdit.Enabled = (rightValue & 1 << 1) == 1 << 1 && !IsEdit;
                //toolDelete.Enabled = (rightValue & 1 << 2) == 1 << 2 && !IsEdit && !IsChecked;
                //toolCheck.Visible = (rightValue & 1 << 3) == 1 << 3 && toolDetailForm.AllowCheck;


                toolCheck.Visible = UserAllowCheck && _ToolDetailForm.AllowCheck;
                toolEdit.Visible = UserAllowEdit && _ToolDetailForm.AllowEdit;
                toolNew.Visible = UserAllowCreate && _ToolDetailForm.AllowNew;
                toolDelete.Visible = UserAllowDelete && _ToolDetailForm.AllowEdit;
      

                toolCancel.Enabled = IsEdit;
                toolSave.Enabled = IsEdit;
                toolContinue.Enabled = IsEdit;
                toolDeleteRow.Enabled = IsEdit;
                toolInsertRow.Enabled = IsEdit;
                toolClose.Enabled = true;
                toolUp.Enabled = !IsEdit && _HasUp && _CanPage;
                toolDown.Enabled = !IsEdit && _HasDown && _CanPage;
                toolCopyLine.Enabled = IsEdit;


                //设置扩展工具条的权限
                if (toolDetailForm.Right == null)
                    return;
                string exp = toolDetailForm.Right.RightExpression;
                if (exp != null && exp.Length > 0)
                {
                    string[] s = exp.Split(',');
                    int j = 1;
                    for (int i = 0; i < s.Length; i++)
                    {
                        string[] s2 = s[i].Split('=');
                        if (i == 0 && s2.Length == 2 && s2[0] == "Check")
                        {
                            j = 0;
                            continue;
                        }
                        if (s2.Length == 2 && this.toolMain.Items.ContainsKey(s2[0]))
                        {
                            this.toolMain.Items[s2[0]].Enabled = (rightValue & 1 << (3 + i + j)) == 1 << (3 + i + j);
                        }
                    }
                }

            }
        }
        /// <summary>
        /// 设其他的控件的Enabled或ReadOnly属性
        /// </summary>
        /// <param name="IsEdit"></param>
        protected virtual void AfterEditModeChanged(bool IsEdit)
        {
            ChangeEditMode(this.panelMain.Controls, IsEdit && !IsChecked);
            if (panelDetail != null)
                SetGridEditMode(panelDetail, IsEdit);
            _ToolDetailForm.AfterEditModeChanged(IsEdit);
        }
        protected void SetGridEditMode(Control control, bool isEdit)
        {
            foreach (KeyValuePair<string, UltraGrid> o in _GridMap)
            {
                if (IsEdit && toolDetailForm.AllowEntryEditMode(o.Key))
                {
                    if (toolDetailForm.AllowUpdateInGrid(o.Key))
                        o.Value.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
                    else
                    {
                        o.Value.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
                        o.Value.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
                    }
                    if (toolDetailForm.AllowInsertRowInGrid(o.Key))
                    {
                        if (toolDetailForm.ShowAddRowButton(o.Key))
                        {
                            o.Value.DisplayLayout.AddNewBox.Hidden = false;
                            o.Value.DisplayLayout.AddNewBox.Prompt = "";
                            o.Value.DisplayLayout.Bands[0].AddButtonCaption = "新增行";
                            
                            o.Value.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.Default;
                        }
                        else
                            o.Value.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
                    }
                    else
                        o.Value.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
                }
                else
                {
                    o.Value.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
                    o.Value.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
                }
            }
        }
        protected void ApplyGridData(Control control)
        {
            if (control == null) return;
            foreach (Control ctl in control.Controls)
                if (ctl.GetType() == typeof(UltraGrid))
                {
                    UltraGrid grid = ((UltraGrid)ctl);
                    int i = 1;
                    foreach (UltraGridRow dr in grid.Rows)
                    {
                        if (dr.IsDeleted) continue;
                        if (grid.DisplayLayout.Bands[0].Columns.Exists(fieldMainID) && (dr.Cells[fieldMainID].Value == null || dr.Cells[fieldMainID].Value == DBNull.Value || _MainID.CompareTo(dr.Cells[fieldMainID].Value) != 0))
                        {
                            dr.Cells[fieldMainID].Value = _MainID;
                        }
                        if (grid.DisplayLayout.Bands[0].Columns.Exists(fieldLineNumber) && (dr.Cells[fieldLineNumber].Value == null || dr.Cells[fieldLineNumber].Value == DBNull.Value))
                        {
                            dr.Cells[fieldLineNumber].Value = i;
                        }
                        i++;
                    }
                    ((UltraGrid)ctl).UpdateData();

                }
                else
                    ApplyGridData(ctl);
        }

        /// <summary>
        /// 改Panel里的控件的ReadOnly属性
        /// </summary>
        /// <param name="p"></param>
        /// <param name="isEdit"></param>
        protected void ChangeEditMode(Control.ControlCollection ctls,bool IsEdit)
        {
            foreach (Control ctl in ctls)
            {
                COMField f = ctl.Tag as COMField;
                if (f == null)
                {
                    if (ctl.GetType() == typeof(TextBox))
                    {
                        ((TextBox)ctl).ReadOnly = !IsEdit;
                    }
                    else if (ctl.GetType() == typeof(ComboBox))
                    {
                        ((ComboBox)ctl).Enabled = IsEdit;
                    }
                    else if (ctl.GetType() == typeof(NumericUpDown))
                    {
                        ((NumericUpDown)ctl).ReadOnly = !IsEdit;
                        ((NumericUpDown)ctl).Enabled = IsEdit;
                    }
                    else if (ctl.GetType() == typeof(UltraCurrencyEditor))
                    {
                        ((UltraCurrencyEditor)ctl).ReadOnly = !IsEdit;
                    }
                    else if (ctl.GetType() == typeof(UltraTextEditor))
                    {
                        ((UltraTextEditor)ctl).ReadOnly = !IsEdit;
                        ((UltraTextEditor)ctl).Enabled = IsEdit;
                    }
                    else if (ctl.GetType() == typeof(UltraDateTimeEditor))
                    {
                        ((UltraDateTimeEditor)ctl).ReadOnly = !IsEdit;
                    }
                    else if (ctl.GetType() == typeof(UltraNumericEditor))
                    {
                        ((UltraNumericEditor)ctl).ReadOnly = !IsEdit;
                    }
                    else if (ctl.GetType() != typeof(Label))
                    {
                        if (ctl.Controls.Count > 0)
                            ChangeEditMode(ctl.Controls, IsEdit);
                        else
                            ctl.Enabled = IsEdit;
                    }
                }
                else
                {
                    if (ctl.GetType() == typeof(TextBox))
                    {
                        ((TextBox)ctl).ReadOnly = !(IsEdit && f.Enable);
                    }
                    else if (ctl.GetType() == typeof(ComboBox))
                    {
                        ((ComboBox)ctl).Enabled = (IsEdit && f.Enable);
                    }
                    else if (ctl.GetType() == typeof(NumericUpDown))
                    {
                        ((NumericUpDown)ctl).ReadOnly = !(IsEdit && f.Enable);
                        ((NumericUpDown)ctl).Enabled = (IsEdit && f.Enable);
                    }
                    else if (ctl.GetType() == typeof(UltraCurrencyEditor))
                    {
                        ((UltraCurrencyEditor)ctl).ReadOnly = !(IsEdit && f.Enable);
                    }
                    else if (ctl.GetType() == typeof(UltraTextEditor))
                    {
                        ((UltraTextEditor)ctl).ReadOnly = !(IsEdit && f.Enable);
                        ((UltraTextEditor)ctl).Enabled = (IsEdit && f.Enable);
                    }
                    else if (ctl.GetType() == typeof(UltraDateTimeEditor))
                    {
                        ((UltraDateTimeEditor)ctl).ReadOnly = !(IsEdit && f.Enable);
                    }
                    else if (ctl.GetType() == typeof(UltraNumericEditor))
                    {
                        ((UltraNumericEditor)ctl).ReadOnly = !(IsEdit && f.Enable);
                    }
                    else if (ctl.GetType() != typeof(Label))
                    {
                        if (ctl.Controls.Count > 0)
                            ChangeEditMode(ctl.Controls, (IsEdit && f.Enable));
                        else
                            ctl.Enabled = (IsEdit && f.Enable);
                    }
                }
            }
        }

        private void toolEdit_Click(object sender, EventArgs e)
        {
            if (MainDataSet.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("CheckStatus"))
            {
                if ((int)MainDataSet.Tables[_MainTableDefine.OrinalTableName].Rows[0]["CheckStatus"] == 0)
                    updateTool(true);
            }
            else
                updateTool(true);
        }
        public void SaveTagData(Panel p)
        {
            SaveTagData(p.Controls);
        }
        public void SaveTagData(Control.ControlCollection controls)
        {
            foreach (Control ctl in controls)
            {
                if (ctl.GetType() == typeof(TextBox) || ctl.GetType() == typeof(ComboBox) || ctl.GetType() == typeof(UltraTextEditor) || ctl.GetType() == typeof(UltraDateTimeEditor))
                {
                    foreach (Binding b in ctl.DataBindings)
                        b.WriteValue();
                }
                else if (ctl.GetType() == typeof(Panel) || ctl.GetType() == typeof(GroupBox))
                {
                    SaveTagData(ctl.Controls);
                }
            }
        }
        protected virtual string BeforeSaving(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            /*
            SaveTagData(this.panelMain);
            this.BindingContext[MainDataSet, _MainTableDefine.OrinalTableName].EndCurrentEdit();
            foreach(COMFields d in _DetailTableDefine)
                this.BindingContext[MainDataSet, d.OrinalTableName].EndCurrentEdit();
            if (IsNew)
            {
                if (MainDataSet.Tables[MainTable].Columns.Contains("Createdby"))
                    MainDataSet.Tables[MainTable].Rows[0]["Createdby"] = CSystem.Sys.Svr.User;
                if (MainDataSet.Tables[MainTable].Columns.Contains("CreatedbyName"))
                    MainDataSet.Tables[MainTable].Rows[0]["CreatedbyName"] = CSystem.Sys.Svr.UserName;
                if (MainDataSet.Tables[MainTable].Columns.Contains("CreateDate"))
                    MainDataSet.Tables[MainTable].Rows[0]["CreateDate"] = CSystem.Sys.Svr.SystemTime;
            }
            ApplyGridData(panelDetail);
            Guid id = (Guid)MainDataSet.Tables[MainTable].Rows[0]["ID"];

            //设置Detail表的默认值
            foreach (COMFields fields in _DetailTableDefine)
            {
                if (fields.Property.TableType != CTableProperty.enuTableType.View)
                {
                    DataTable dt = MainDataSet.Tables[fields.OrinalTableName];
                    SortedList<string, object> defaultValue = null;
                    if (_DefaultValue!=null && _DefaultValue.ContainsKey(fields.OrinalTableName))
                        defaultValue = _DefaultValue[fields.OrinalTableName];
                    int i = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted)
                            continue;
                        setDefaultValueWhenNull(fields, dr, defaultValue);
                        if (!_GridMap.ContainsKey(fields.OrinalTableName))
                        {
                            if (dr[MainID] == null || dr[MainID] == DBNull.Value || id.CompareTo(dr[MainID]) != 0)
                            {
                                dr[MainID] = id;
                            }
                            if (dr[LineNumber] == null || dr[LineNumber] == DBNull.Value || i.CompareTo(dr[LineNumber]) != 0)
                            {
                                dr[LineNumber] = i;
                            }
                            i++;
                        }
                    }
                }
            }
            //设置主表默认值
            SortedList<string, object> defaultValue2 = null;
            if (_DefaultValue!=null && _DefaultValue.ContainsKey(MainTable))
                defaultValue2 = _DefaultValue[MainTable];
            foreach (DataRow dr in MainDataSet.Tables[MainTable].Rows)
            {
                setDefaultValueWhenNull(_MainTableDefine, dr, defaultValue2);
            }
            */
            /*if (_DefaultValue!=null)
                foreach(string tableName in _DefaultValue.Keys)
                    if (MainDataSet.Tables.Contains(tableName))
                    {
                        foreach(string fieldName in _DefaultValue[tableName].Keys)
                            if (MainDataSet.Tables[tableName].Columns.Contains(fieldName))
                            {
                                foreach (DataRow dr in MainDataSet.Tables[tableName].Rows)
                                    if (dr[fieldName]==null || dr[fieldName]==DBNull.Value)
                                        dr[fieldName] = _DefaultValue[tableName][fieldName];
                            }
                    }
            */
            return toolDetailForm.BeforeSaving(ds,conn,tran);
        }
protected virtual bool Saving(SqlConnection conn,SqlTransaction tran)
        {
            try
            {
                _ToolDetailForm.Saving(MainDataSet, conn, tran);
                foreach (COMFields d in _DetailTableDefine)
                    this.BindingContext[MainDataSet, d.OrinalTableName].EndCurrentEdit();
                foreach (COMFields d in _DetailTableDefine)
                    if (MainDataSet.Tables[d.OrinalTableName].Rows.Count > 0 && d.Property.TableType!= CTableProperty.enuTableType.View)
                    {
                        foreach (DataRow dr in MainDataSet.Tables[d.OrinalTableName].Rows)
                            CodeRule(conn, tran, dr, false);
                        CSystem.Sys.Svr.cntMain.Update(MainDataSet.Tables[d.OrinalTableName], conn, tran);
                    }
                return true;
            }
            catch (Exception e)
            {
                Msg.Error(e.ToString());
                return false;
            }
        }
        public void UpdateData()
        {
            //退出编辑状态,这样可以保证Update事件被执行
            foreach (UltraGrid grid in _GridMap.Values)
            {
                grid.PerformAction(UltraGridAction.ExitEditMode);
                grid.UpdateData();
            }

            this.SelectNextControl(this.ActiveControl, true, true, true, true);

            SaveTagData(this.panelMain);
            this.BindingContext[MainDataSet, _MainTableDefine.OrinalTableName].EndCurrentEdit();
            foreach (COMFields d in _DetailTableDefine)
                this.BindingContext[MainDataSet, d.OrinalTableName].EndCurrentEdit();

            ApplyGridData(panelDetail);
        }
        private bool Save()
        {
             //退出编辑状态,这样可以保证Update事件被执行
            foreach (UltraGrid grid in _GridMap.Values)
            {
                grid.PerformAction(UltraGridAction.ExitEditMode);
                grid.UpdateData();
            }

           this.SelectNextControl(this.ActiveControl, true, true, true, true);
            SaveTagData(this.panelMain);
            
            this.BindingContext[MainDataSet, _MainTableDefine.OrinalTableName].EndCurrentEdit();

            if (_DetailTableDefine!=null)
                foreach (COMFields d in _DetailTableDefine)
                    this.BindingContext[MainDataSet, d.OrinalTableName].EndCurrentEdit();
            if (IsNew)
            {
                if (MainDataSet.Tables[MainTable].Columns.Contains("Createdby"))
                    MainDataSet.Tables[MainTable].Rows[0]["Createdby"] = CSystem.Sys.Svr.User;
                if (MainDataSet.Tables[MainTable].Columns.Contains("CreatedbyName"))
                    MainDataSet.Tables[MainTable].Rows[0]["CreatedbyName"] = CSystem.Sys.Svr.UserName;
                if (MainDataSet.Tables[MainTable].Columns.Contains("CreateDate"))
                    MainDataSet.Tables[MainTable].Rows[0]["CreateDate"] = CSystem.Sys.Svr.SystemTime;
            }
            ApplyGridData(panelDetail);
            //Guid id = (Guid)MainDataSet.Tables[MainTable].Rows[0]["ID"];

            //设置Detail表的默认值
            foreach (COMFields fields in _DetailTableDefine)
            {
                if (fields.Property.TableType != CTableProperty.enuTableType.View)
                {
                    DataTable dt = MainDataSet.Tables[fields.OrinalTableName];
                    SortedList<string, object> defaultValue = null;
                    if (_DefaultValue != null && _DefaultValue.ContainsKey(fields.OrinalTableName))
                        defaultValue = _DefaultValue[fields.OrinalTableName];
                    int i = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted)
                            continue;
                        if (dr.RowState == DataRowState.Added && dr["ID"] == DBNull.Value)
                            dr["ID"] = Guid.NewGuid();
                        setDefaultValueWhenNull(fields, dr, defaultValue);
                        if (!_GridMap.ContainsKey(fields.OrinalTableName))
                        {
                            if ((dt.Columns.Contains(fieldMainID)) && (dr[fieldMainID] == null || dr[fieldMainID] == DBNull.Value || _MainID.CompareTo(dr[fieldMainID]) != 0))
                            {
                                dr[fieldMainID] = _MainID;
                            }
                            if ((dt.Columns.Contains(fieldLineNumber)) && (dr[fieldLineNumber] == null || dr[fieldLineNumber] == DBNull.Value || i.CompareTo(dr[fieldLineNumber]) != 0))
                            {
                                dr[fieldLineNumber] = i;
                            }
                            i++;
                        }
                    }
                }
            }
            //设置主表默认值
            SortedList<string, object> defaultValue2 = null;
            if (_DefaultValue != null && _DefaultValue.ContainsKey(MainTable))
                defaultValue2 = _DefaultValue[MainTable];
            foreach (DataRow dr in MainDataSet.Tables[MainTable].Rows)
            {
                setDefaultValueWhenNull(_MainTableDefine, dr, defaultValue2);
            }

            //检测必须有填的数据
            string sReson = "";
            foreach (KeyValuePair<string, Control> o in _ControlMap)
            {
                COMField field = _MainTableDefine[o.Key];
                DataRow dr =MainDataSet.Tables[_MainTableDefine.OrinalTableName].Rows[0];
                if (field.Mandatory && (dr[field.FieldName] == null || dr[field.FieldName] == DBNull.Value || "".Equals(dr[field.FieldName]) ))
                    sReson = sReson + _MainTableDefine.Property.Title + "[" + field.FieldTitle + "]没有输入值！";
            }
            foreach( COMFields fields in  _DetailTableDefine)
            {
                if (_GridMap.ContainsKey(fields.OrinalTableName))
                    foreach(UltraGridRow row in _GridMap[fields.OrinalTableName].Rows)
                        if (!row.IsAddRow )
                            foreach( COMField  f in fields.Fields)
                                if (f.Mandatory && (row.Cells[f.FieldName].Value==null || row.Cells[f.FieldName].Value==DBNull.Value))
                                    sReson = sReson + fields.Property.Title + "[" + f.FieldTitle + "]没有输入值！";
            }
            if (sReson.Length > 0)
            {
                Msg.Warning(sReson);
                return false;
            }
            using (SqlConnection conn = new SqlConnection(CSystem.Sys.Svr.cntMain.ConnectionString))
            {
                SqlTransaction sqlTran = null;
                try
                {
                    conn.Open();
                    sqlTran = conn.BeginTransaction();

                    string ErrText = null;

                    COMField IDField = new COMField();
                    if (CSystem.Sys.Svr.LDI.GetFields(_MainTableDefine.OrinalTableName).FieldsName.Contains("ID"))
                        IDField = CSystem.Sys.Svr.LDI.GetFields(_MainTableDefine.OrinalTableName)["ID"];

                    if (MainDataSet.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("Code") && _MainTableDefine["Code"].FieldType==0)
                    {
                        DataRow dr = MainDataSet.Tables[_MainTableDefine.OrinalTableName].Rows[0];
                        if (!CodeRule(conn, sqlTran, dr))
                        {
                            if (toolDetailForm.AutoCode)
                            {
                                if (IsNew)
                                {
                                    DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select count(*) from {0} where Code='{1}'", _MainTableDefine.OrinalTableName, dr["Code"]), conn, sqlTran);
                                    if ((int)ds.Tables[0].Rows[0][0] > 0)
                                        dr["Code"] = GetCode(toolDetailForm.GetPrefix(), conn, sqlTran);
                                }
                            }
                            else
                            {
                                DataSet d= new DataSet();

                                if (IDField.ValueType.Contains("String"))
                                      d = CSystem.Sys.Svr.cntMain.Select(string.Format("Select count(*) from {2} where Code='{0}' and ID<>'{1}'", dr["Code"], ((string)MainDataSet.Tables[MainTable].Rows[0]["ID"]).ToString(), MainTable), conn, sqlTran);
                                else
                                      d = CSystem.Sys.Svr.cntMain.Select(string.Format("Select count(*) from {2} where Code='{0}' and ID<>'{1}'", dr["Code"], ((Guid)MainDataSet.Tables[MainTable].Rows[0]["ID"]).ToString(), MainTable), conn, sqlTran);
                                
                                if ((int)d.Tables[0].Rows[0][0] > 0)
                                        ErrText = ErrText + "代码已被使用，请修改代码后再保存！";
                                
                            }
                        }
                    }
                    //检测
                    ErrText = ErrText + BeforeSaving(_MainDataSet, conn, sqlTran);
                    if (ErrText!=null && ErrText.Length>0)
                    {
                        Msg.Information(ErrText);
                        sqlTran.Rollback();
                        return false;
                    }
                    //由于在BeforeSaving中可能会改变值，需要做End操作，否则可能有些值不能保存进去。
                    this.BindingContext[MainDataSet, _MainTableDefine.OrinalTableName].EndCurrentEdit();

                    CSystem.Sys.Svr.cntMain.Update(_MainDataSet.Tables[_MainTableDefine.OrinalTableName], conn, sqlTran);
                    if (!Saving(conn, sqlTran))
                    {
                        sqlTran.Rollback();
                        return false;
                    }

                    toolDetailForm.AfterSaved(_MainDataSet, conn, sqlTran);
                    sqlTran.Commit();
                    _IsNew = false;
                    IsEdit = false;
                    return true;
                }
                catch (Exception ex)
                {
                    Msg.Error(ex.ToString());
                    try
                    {
                        if (sqlTran != null)
                            sqlTran.Rollback();
                    }
                    catch { }
                }
                finally
                {
                    conn.Close();
                }
            }
            return false;
        }

        /*
         * 编码规则：
            １．定义规则，数据源表、序号、序列表达式（SQL表达式）、分组SQL表达式、分组表达式（按string.format规则）、分组参数字段列表、编码表达式（按Stirng.Format的规则），编码参数字段列表（字段在format中从1开始，0留给序列），序列位数，是否补零，开始值；可选项：序列字段。
            ２．获取时，先根据当前记录的值，按分组表达式和分组参数字段列表，计算出分组值；再根据分组值与分组ＳＱＬ表达式到数据源查询，获取序列表达式的最大值；如没有最大值，直接使用开始值，代入编码表达式计算出编码；如有最大值，该值转为整数，加１，再按编码表达式和编码参数字段列表，生成编码
            ３．保存和预测使用同一规则，其中预测时，在DataBinding之后；预测还需要考虑一种情况，Grid的行增加时，也需要预测，预测时需要考虑，目前Grid中已有多少新增的行。
            */
        public bool CodeRule(DataRow dr, bool force)
        {
            return CodeRule(null, null, dr, force);
        }
        public bool CodeRule(DataRow dr)
        {
            return CodeRule(null, null, dr,false);
        }
        public bool CodeRule(SqlConnection conn, SqlTransaction tran, DataRow dr)
        {
            return CodeRule(conn, tran, dr, false);
        }
        public bool CodeRule(SqlConnection conn,SqlTransaction tran,DataRow dr,bool force)
        {
            if (CSystem.Sys.Svr.CodeRules.ContainsKey(dr.Table.TableName))
            {
                foreach (CCodeRule rule in CSystem.Sys.Svr.CodeRules[dr.Table.TableName])
                {
                    if (!force && dr.RowState!= DataRowState.Added && dr[rule.CodeField] != DBNull.Value && (string)dr[rule.CodeField] != "")
                        continue;
                    if (toolDetailForm.CancelCodeRule(rule, IsNew,dr))
                        continue;
                    object[] obj = new  object[rule.GroupParameters.Length];
                    for (int i=0;i<rule.GroupParameters.Length;i++)
                    {
                        string p=rule.GroupParameters[i];
                        obj[i] = dr[p];
                    }
                    string group = string.Format(rule.GroupExpression, obj);
                    DataSet ds = null;
                    if (conn == null)
                        ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select max({0}) from {1} where {2}='{3}'", rule.SequenceExpression, rule.DataTableName, rule.GroupSQLExpression, group));
                    else
                        ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select max({0}) from {1} where {2}='{3}'", rule.SequenceExpression, rule.DataTableName, rule.GroupSQLExpression, group), conn, tran);
                    if (ds.Tables[0].Rows[0][0] == DBNull.Value)
                    {
                        obj = new object[rule.CodeParameters.Length+1];
                        int v = rule.InitialValue;
                        if (rule.SequenceField != null && rule.SequenceField.Length > 0)
                            dr[rule.SequenceField] = v;
                        obj[0] = v.ToString().PadLeft(rule.SquenceLength,'0');
                        for (int i = 0; i < rule.CodeParameters.Length; i++)
                        {
                            string p = rule.CodeParameters[i];
                            obj[i+1] = dr[p];
                        }
                        dr[rule.CodeField] = string.Format(rule.CodeExpression, obj);
                    }
                    else
                    {
                        obj = new object[rule.CodeParameters.Length + 1];
                        int v = Convert.ToInt32(ds.Tables[0].Rows[0][0]) + 1;
                        if (rule.SequenceField != null && rule.SequenceField.Length > 0)
                            dr[rule.SequenceField] = v;
                        obj[0] = v.ToString().PadLeft(rule.SquenceLength, '0');
                        for (int i = 0; i < rule.CodeParameters.Length; i++)
                        {
                            string p = rule.CodeParameters[i];
                            obj[i + 1] = dr[p];
                        }
                        dr[rule.CodeField] = string.Format(rule.CodeExpression, obj);
                    }
                }
                return true;
            }
            else
                return false;
        }
        public bool CodeRule(UltraGridRow row)
        {
            COMFields fields = this.Grid.Tag as COMFields;
            string tableName = fields.OrinalTableName;
            if (tableName!=null &&  CSystem.Sys.Svr.CodeRules.ContainsKey(tableName))
            {
                foreach (CCodeRule rule in CSystem.Sys.Svr.CodeRules[tableName])
                {
                    if (toolDetailForm.CancelCodeRule(rule, IsNew, row))
                        continue;
                    object[] obj = new object[rule.GroupParameters.Length];
                    for (int i = 0; i < rule.GroupParameters.Length; i++)
                    {
                        string p = rule.GroupParameters[i];
                        obj[i] = row.Cells[p].Value;
                    }
                    string group = string.Format(rule.GroupExpression, obj);
                    DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select max({0}) from {1} where {2}='{3}'", rule.SequenceExpression, rule.DataTableName, rule.GroupSQLExpression, group));
                    if (ds.Tables[0].Rows[0][0] == DBNull.Value)
                    {
                        obj = new object[rule.CodeParameters.Length + 1];
                        obj[0] = rule.InitialValue.ToString().PadLeft(rule.SquenceLength, '0');
                        for (int i = 0; i < rule.CodeParameters.Length; i++)
                        {
                            string p = rule.CodeParameters[i];
                            obj[i + 1] = row.Cells[p].Value;
                        }
                        row.Cells[rule.CodeField].Value = string.Format(rule.CodeExpression, obj);
                    }
                    else
                    {
                        obj = new object[rule.CodeParameters.Length + 1];
                        obj[0] = (Convert.ToInt32(ds.Tables[0].Rows[0][0]) + 1).ToString().PadLeft(rule.SquenceLength, '0');
                        for (int i = 0; i < rule.CodeParameters.Length; i++)
                        {
                            string p = rule.CodeParameters[i];
                            obj[i + 1] = row.Cells[p].Value;
                        }
                        row.Cells[rule.CodeField].Value = string.Format(rule.CodeExpression, obj);
                    }
                }
                return true;
            }
            else
                return false;
        }
        private void toolSave_Click(object sender, EventArgs e)
        {
            bool lastIsNew = _IsNew;
            if (Save())
            {
                if (Changed != null)
                {
                    DataTableEventArgs data = new DataTableEventArgs(_MainDataSet.Tables[_MainTableDefine.OrinalTableName]);
                    Changed(this, data);
                    _HasDown = data.HasNext;
                    _HasUp = data.HasPrevious;
                }
                if (lastIsNew)
                    this.Close();
                else
                    updateTool(false);
            }
        }

        private void toolContinue_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                if (Changed != null)
                {
                    DataTableEventArgs data = new DataTableEventArgs(_MainDataSet.Tables[_MainTableDefine.OrinalTableName]);
                    Changed(this, data);
                    _HasDown = data.HasNext;
                    _HasUp = data.HasPrevious;
                }
                if (BeforeAdd())
                {
                    if (NewData(_MainDataSet))
                        AfterDataBind();
                }
                else
                    updateTool();
            }
        }

        private void toolCancel_Click(object sender, EventArgs e)
        {
            if (_IsNew)
            {
                if (NewData(_MainDataSet))
                    AfterDataBind();
            }
            else
            {
                foreach (DataTable dt in _MainDataSet.Tables)
                {
                    dt.Rows.Clear();
                }
                _MainDataSet.Merge(BackupDataSet, false);

                updateTool(false);
            }
        }

        private void toolDelete_Click(object sender, EventArgs e)
        {
            if (Msg.Question("您的操作将删除整张单据，请慎重。是否继续？") != DialogResult.Yes)
                return;
            if (IsNew && IsChecked)
            {
                this.Close();
                return;
            }
            //Guid id = (Guid)_MainDataSet.Tables[_MainTable].Rows[0]["ID"];
            if (_MainDataSet.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("Disable"))
                _MainDataSet.Tables[_MainTableDefine.OrinalTableName].Rows[0]["Disable"]=1;
            else
                _MainDataSet.Tables[_MainTableDefine.OrinalTableName].Rows[0].Delete();
            DataTable bk = _MainDataSet.Tables[_MainTableDefine.OrinalTableName].Copy();
            using (SqlConnection conn = new SqlConnection(CSystem.Sys.Svr.cntMain.ConnectionString))
            {
                SqlTransaction sqlTran = null;
                try
                {
                    conn.Open();
                    sqlTran = conn.BeginTransaction();

                    if (_MainDataSet.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("CheckStatus"))
                    {
                        DataSet dsTemp = CSystem.Sys.Svr.cntMain.Select(string.Format("Select id from {0} where id='{1}' and CheckStatus=0", _MainTableDefine.OrinalTableName, _MainDataSet.Tables[_MainTableDefine.OrinalTableName].Rows[0]["ID"]), conn, sqlTran);
                        if (dsTemp.Tables[0].Rows.Count == 0)
                        {
                            sqlTran.Rollback();
                            return;
                        }
                    }
                    //CSystem.Sys.Svr.cntMain.Delete(string.Format("Delete from {0} where {1}='{2}'", DetailTable, MainID, id.ToString()), conn);
                    if (Deleting(conn,sqlTran, _MainID))
                    {
                        CSystem.Sys.Svr.cntMain.Update(_MainDataSet.Tables[_MainTableDefine.OrinalTableName], conn,sqlTran);
                        sqlTran.Commit();
                    }
                    else
                    {
                        sqlTran.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    if (sqlTran!=null)
                        sqlTran.Rollback();
                    Msg.Error(ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
            if (Changed != null)
            {
                Changed(this, new DataTableEventArgs(bk));
            }
            IsEdit = false;
            this.Close();
        }
        protected virtual bool Deleting(SqlConnection conn, SqlTransaction tran, Guid id)
        {
            _ToolDetailForm.Deleting(conn, tran, id);
            foreach (COMFields d in _DetailTableDefine)
                if (d.Property.TableType != CTableProperty.enuTableType.View)
                {
                    CSystem.Sys.Svr.cntMain.Excute(string.Format("Delete from {0} where MainID='{1}'", d.OrinalTableName, id), conn, tran);
                }
            return true;
        }

        private void toolClose_Click(object sender, EventArgs e)
        {
            if (IsEdit)
            {
                switch (Msg.Question("正在修改数据，是否保存？", MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        if (Save())
                        {
                            if (Changed != null)
                            {
                                Changed(this, new DataTableEventArgs(_MainDataSet.Tables[_MainTableDefine.OrinalTableName]));
                            }
                        }
                        else
                            return;
                        break;
                    case DialogResult.Cancel:
                        return;
                }
                IsEdit = false;
            }
            this.Close();
        }

        private void frmDetail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                toolSave_Click(sender, EventArgs.Empty);
            }
        }

        private void frmDetail_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsEdit && !_IsNew)
            {
                switch (Msg.Question("正在修改数据，是否保存？", MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        if (Save())
                        {
                            if (Changed != null)
                            {
                                Changed(this, new DataTableEventArgs(_MainDataSet.Tables[_MainTableDefine.OrinalTableName]));
                            }
                        }
                        else
                        {
                            e.Cancel = true;
                        }
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }
        protected virtual bool BeforeAdd()
        {
            return true;
        }
        private void toolNew_Click(object sender, EventArgs e)
        {
            if (BeforeAdd())
            {
                if (NewData(_MainDataSet))
                {
                    AfterDataBind();
                    updateTool(true);
                }
            }
        }

        private void frmDetail_Load(object sender, EventArgs e)
        {
            AfterDataBind();
        }
        public void ChangeData()
        {
            if (Changed != null)
            {
                DataTableEventArgs data = new DataTableEventArgs(MainDataSet.Tables[_MainTableDefine.OrinalTableName]);
                Changed(this, data);
                _HasDown = data.HasNext;
                _HasUp = data.HasPrevious;
            }
        }
        private void toolUp_Click(object sender, EventArgs e)
        {
            if (PageUp != null)
            {
                DataTableEventArgs data = new DataTableEventArgs(MainDataSet.Tables[_MainTableDefine.OrinalTableName].Clone());
                PageUp(sender, data);
                _HasUp = data.HasPrevious;
                _HasDown = data.HasNext;
                toolUp.Enabled = data.HasPrevious;
                toolDown.Enabled = data.HasNext;
                if (data.Data.Rows.Count == 1)
                {
                    Guid newID = (Guid)data.Data.Rows[0]["ID"];
                    MainDataSet.Merge(data.Data);
                    DataTable dt = MainDataSet.Tables[_MainTableDefine.OrinalTableName];
                    foreach (DataTable d in MainDataSet.Tables)
                        if (d != dt)
                            d.Rows.Clear();
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        if (newID.CompareTo((Guid)dt.Rows[i]["ID"]) != 0)
                            dt.Rows.RemoveAt(i);
                    }
                    InitData(MainDataSet);
                }
                AfterDataBind();
            }
        }

        private void toolDown_Click(object sender, EventArgs e)
        {
            if (PageDown != null)
            {
                DataTableEventArgs data = new DataTableEventArgs(MainDataSet.Tables[_MainTableDefine.OrinalTableName].Clone());
                PageDown(sender, data);
                _HasUp = data.HasPrevious;
                _HasDown = data.HasNext;
                toolUp.Enabled = data.HasPrevious;
                toolDown.Enabled = data.HasNext; 
                if (data.Data.Rows.Count == 1)
                {
                    Guid newID = (Guid)data.Data.Rows[0]["ID"];
                    MainDataSet.Merge(data.Data);
                    DataTable dt = MainDataSet.Tables[_MainTableDefine.OrinalTableName];
                    foreach (DataTable d in MainDataSet.Tables)
                        if (d != dt)
                            d.Rows.Clear();
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        if (newID.CompareTo((Guid)dt.Rows[i]["ID"]) != 0)
                            dt.Rows.RemoveAt(i);
                    }
                    InitData(MainDataSet);
                }
                AfterDataBind();
            }
        }
        public virtual UltraGrid Grid
        {
            get 
            {
                if (_GridMap.Count == 0)
                    return null;
                else
                {
                    if (_GridMap.Count == 1)
                        return _GridMap.Values[0];
                    else
                        return CheckUltraGrid(this.ActiveControl);
                }
            }
        }
        private UltraGrid CheckUltraGrid(Control a)
        {
            if (a == null)
                return null;
            else if (a.GetType() == typeof(UltraGrid))
                return (UltraGrid)a;
            else
                return CheckUltraGrid(a.Parent);
        }
        public virtual bool BeforeExport(Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter exporter, UltraGrid grid, Worksheet sheet, ref int startRow, ref int startColumn)
        {
            if (toolDetailForm != null)
                return toolDetailForm.BeforeExport(exporter, grid, sheet, ref startRow, ref startColumn);
            else
                return true;
        }
        public virtual void AfterExport(UltraGrid grid, Worksheet sheet)
        {
            if (toolDetailForm != null)
                toolDetailForm.AfterExport(grid, sheet);
        }
        private void toolExport_Click(object sender, EventArgs e)
        {
            UltraGrid grid = Grid;
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
                    Infragistics.Excel.Worksheet ws = w.Worksheets.Add(_MainTableDefine.Property.Title);
                    int row = 0;
                    int column = 0;
                    Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter gridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter();
                    if (!BeforeExport(gridExcelExporter, grid,ws,ref row,ref column))
                    {
                        Msg.Information("不支持导出！");
                        return;
                    }
                    
                    gridExcelExporter.CellExported += new Infragistics.Win.UltraWinGrid.ExcelExport.CellExportedEventHandler(gridExcelExporter_CellExported);
                    gridExcelExporter.Export(grid, ws, row, column);
                    
                    AfterExport(grid,ws);
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

        private void toolCheck_Click(object sender, EventArgs e)
        {
            if (IsEdit) return;
            using (SqlConnection conn = new SqlConnection(CSystem.Sys.Svr.cntMain.ConnectionString))
            {
                SqlTransaction sqlTran = null;
                try
                {
                    conn.Open();
                    sqlTran = conn.BeginTransaction();
                    DataRow dr = MainDataSet.Tables[MainTable].Rows[0];
                    Guid id = (Guid)dr["ID"];
                     int i=0;
                     string Statuskey = "CheckStatus";
                     string UserIdKey = "";
                    if (MainTable == "D_ProjectProcess")
                    {
                        i = CSystem.Sys.Svr.cntMain.Excute(string.Format("update {0} set BillStatus=1 where  id='{1}' and BillStatus={2}", MainTable, id, dr["BillStatus"]), conn, sqlTran);
                        Statuskey = "BillStatus";
                        UserIdKey = "ApplicationAuditId";
                    }
                    else if (MainTable == "D_GuaranteeFee"||MainTable=="D_Repayment")
                    {
                        i = CSystem.Sys.Svr.cntMain.Excute(string.Format("update {0} set BillStatus=1 where  id='{1}' and BillStatus={2}", MainTable, id, dr["BillStatus"]), conn, sqlTran);
                        Statuskey = "BillStatus";
                        UserIdKey = "AuditById";
                    }
                    else
                    {
                        i = CSystem.Sys.Svr.cntMain.Excute(string.Format("update {0} set CheckStatus=CheckStatus where  id='{1}' and CheckStatus={2}", MainTable, id, dr["CheckStatus"]), conn, sqlTran);
                        Statuskey = "CheckStatus";
                        UserIdKey = "CheckedBy";
                    }
                   
                    if (i != 1)
                    {
                        Msg.Warning("当前单据已被修改,请重新打开，再试！");
                        sqlTran.Rollback();
                        return;
                    }
                    //DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select CheckStatus from {0} where ID='{1}'", MainTable, id), conn, sqlTran);
                    //if (ds.Tables[0].Rows.Count == 1 && ds.Tables[0].Rows[0]["CheckStatus"].Equals(dr["CheckStatus"]))
                    {
                        if (dr.RowState == DataRowState.Unchanged)
                            dr.SetModified();

                        if ((int)dr[Statuskey] == 0)
                        {
                            if (Msg.Question("是否复核当前单据?") != DialogResult.Yes)
                                return;
                            if (toolDetailForm.Check(id, conn, sqlTran, dr))
                            {
                                dr[Statuskey] = 1;
                                dr[UserIdKey] = CSystem.Sys.Svr.User;
                                if (MainTable == "D_ProjectProcess")
                                {
                                    dr["ApplicationAuditName"] = CSystem.Sys.Svr.UserName;
                                   
                                }
                                else if (MainTable == "D_GuaranteeFee")
                                {
                                    dr["AuditDate"] = CSystem.Sys.Svr.SystemTime;
                                    dr["AuditByName"] = CSystem.Sys.Svr.UserName;
                                    switch ((int)dr["GuaranteeFeeType"])
                                    {
                                        case 0:
                                            CSystem.Sys.Svr.cntMain.Excute(string.Format("Update D_BuzGuarantee set BillStatus={0} where ID='{1}'", 9,dr["BuzGuaranteeID"].ToString()), conn, sqlTran);
                                            break;
                                        case 1:
                                            CSystem.Sys.Svr.cntMain.Excute(string.Format("Update D_BuzGuarantee set isReturnGuaranteeFee={0} where ID='{1}'", 1, dr["BuzGuaranteeID"].ToString()), conn, sqlTran);
                                            break;
                                        case 2:
                                            CSystem.Sys.Svr.cntMain.Excute(string.Format("Update D_BuzGuarantee set isCompensatory={0} where ID='{1}'", 1, dr["BuzGuaranteeID"].ToString()), conn, sqlTran);
                                            break;
                                    }
                                }
                                else if (MainTable != "D_Repayment")
                                    dr["CheckDate"] = CSystem.Sys.Svr.SystemTime;
                                toolCheck.Text = "取消复核";
                            }
                            else
                            {
                                sqlTran.Rollback();
                                return;
                            }
                        }
                        else
                        {
                            if (Msg.Question("是否反复核当前单据?") != DialogResult.Yes)
                                return;
                            if (toolDetailForm.UnCheck(id, conn, sqlTran, dr))
                            {
                                dr[Statuskey] = 0;
                                dr[UserIdKey] = DBNull.Value;
                                if (MainTable == "D_ProjectProcess")
                                {
                                    dr["ApplicationAuditName"] = DBNull.Value;   
                                }
                                else if (MainTable == "D_GuaranteeFee")
                                {
                                    dr["AuditDate"] = DBNull.Value ;
                                    dr["AuditByName"] = DBNull.Value ;
                                    switch ((int)dr["GuaranteeFeeType"])
                                    {
                                        case 0:
                                            CSystem.Sys.Svr.cntMain.Excute(string.Format("Update D_BuzGuarantee set BillStatus={0} where ID='{1}'", 8, dr["BuzGuaranteeID"].ToString()), conn, sqlTran);
                                            break;
                                        case 1:
                                            CSystem.Sys.Svr.cntMain.Excute(string.Format("Update D_BuzGuarantee set isReturnGuaranteeFee={0} where ID='{1}'", 0, dr["BuzGuaranteeID"].ToString()), conn, sqlTran);
                                            break;
                                        case 2:
                                            CSystem.Sys.Svr.cntMain.Excute(string.Format("Update D_BuzGuarantee set isCompensatory={0} where ID='{1}'", 0, dr["BuzGuaranteeID"].ToString()), conn, sqlTran);
                                            break;
                                    }
                                }
                                else if (MainTable != "D_Repayment")
                                    dr["CheckDate"] = DBNull.Value;
                                toolCheck.Text = "复核";
                            }
                            else
                            {
                                sqlTran.Rollback();
                                return;
                            }
                        }
                        CSystem.Sys.Svr.cntMain.Update(MainDataSet.Tables[MainTable], conn, sqlTran);
                        //RefreshCheck();
                        sqlTran.Commit();
                        this.ChangeData();
                    }
                    //else
                    //    Msg.Warning("当前单据已被修改,请重新打开，再试！");
                    //RefreshCheck();
                }
                catch (Exception ex)
                {
                    Msg.Warning(ex.ToString());
                    if (sqlTran != null)
                        sqlTran.Rollback();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 设置Showed值，主要是因为某些控件（如UltraCurrencyEditor）在显示时，会产生Changed事件，如果只是查看没有必要处理Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmDetail_Activated(object sender, EventArgs e)
        {
            Showed = true;
        }

        private void toolInsertRow_Click(object sender, EventArgs e)
        {
            DataSet ds = toolDetailForm.InsertRowsInGrid(DetailTableDefine);
            if (ds != null)
                _MainDataSet.Merge(ds);
        }

        private void toolDeleteRow_Click(object sender, EventArgs e)
        {
            DataSet ds = toolDetailForm.DeleteRowsInGrid(DetailTableDefine);
            if (ds != null)
                _MainDataSet.Merge(ds);
        }

        private void toolCopyLine_Click(object sender, EventArgs e)
        {
            if (this.ActiveControl != null && this.ActiveControl.GetType() == typeof(UltraGrid))
            {
                UltraGrid grid = (UltraGrid)this.ActiveControl;
                if (!_ToolDetailForm.AllowCopyLine(((DataTable)grid.DataSource).TableName))
                    return;
                if (grid.Selected.Rows.Count == 0 && grid.ActiveRow != null)
                    grid.ActiveRow.Selected = true;
                foreach (UltraGridRow row in grid.Selected.Rows)
                {
                    UltraGridRow newRow = grid.DisplayLayout.Bands[0].AddNew();
                    foreach (UltraGridCell cell in row.Cells)
                    {
                        try
                        {
                            newRow.Cells[cell.Column.Key].Activation = cell.Activation;
                            switch (cell.Column.Key)
                            {
                                case "ID":
                                    newRow.Cells[cell.Column.Key].Value = System.Guid.NewGuid();
                                    break;
                                case "LineNumber":
                                    newRow.Cells[cell.Column.Key].Value = grid.Rows.Count;
                                    break;
                                case "Tag":
                                    break;
                                default:
                                    newRow.Cells[cell.Column.Key].Value = cell.Value;
                                    break;
                            }
                        }
                        catch { }
                    }
                }
                grid.DisplayLayout.Bands[0].AddNew();
            }
        }

        private void frmDetail_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_ToolDetailForm!=null)
                _ToolDetailForm.SetDetailForm(null);
        }
    }
}