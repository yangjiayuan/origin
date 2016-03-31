using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win;
using Infragistics.Excel;
using System.IO;
using Base;
using System.Data.OleDb;
using System.Data.Common;

namespace UI
{
    public partial class frmBrowser : Form
    {
        private const string _MainID = "MainID";

        private bool DataLoaded = false;
        private DataSet _MainDataSet;
        private bool IsSelectMode = false;
        private clsSelect DBSelect;
        private System.Windows.Forms.Button butFilter;
        private System.Windows.Forms.Button butDetail;
        private UltraGrid gridDetail;
        private System.Windows.Forms.TextBox txtFilter1;
        private System.Windows.Forms.TextBox txtFilter2;
        private System.Windows.Forms.ComboBox cmbFilter;
        private System.Windows.Forms.ComboBox cmbValueList1;
        private System.Windows.Forms.ComboBox cmbValueList2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor dictValue;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor datValue1;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor datValue2;
        private System.Windows.Forms.ComboBox cmbOperator1;
        private System.Windows.Forms.ComboBox cmbOperator2;
        private int FilterMethod = 1;
        //private bool _ShowDetail = false;
        private COMFields _MainTableDefine;
        private List<COMFields> _DetailTableDefine;
        private SortedList<string, SortedList<string, object>> _DefaultValue;
        private SortedList<string, string> _CopyLastValue;
        //private ISelectForm _SelectForm;
        //private IDetailFormHandler _DetailFormHandler;
        private ToolDetailForm _ToolDetailForm;
        private string _FieldList;
        private string _From;
        private string _Where;
        private string _Order;
        private string _PrimaryKey;
        private int _DetailIndex;
        private DBConnection cntMain;
        //private bool _ShowCheck = false;
        private enuShowStatus _ShowStatus;
        private bool _ShowDetail = false;
        private bool _IsHistory = false;
        private string BasePath = null;
        private string _ModulePath = null;
        private Panel panelLink = null;
        private String _FilterFieldName = "Code";
        private bool _AllowDoubleClick = true;


        public frmBrowser()
        {
            InitializeComponent();
            BasePath = Application.ExecutablePath;
            BasePath = BasePath.Substring(0, BasePath.LastIndexOf('\\') + 1);
        }

        public bool AllowDoubleClick
        {
            get { return _AllowDoubleClick; }
            set { _AllowDoubleClick = value; }
        }
        
        
        public bool IsHistory
        {
            get { return _IsHistory; }
            set { _IsHistory = value; }
        }
        public bool isDataLoaded
        {
            get { return DataLoaded; }
            set { DataLoaded = value; }
        }
        public String FilterFieldName
        {
            get { return _FilterFieldName; }
            set { _FilterFieldName = value; }
        }
        public enuShowStatus ShowStatus
        {
            get { return _ShowStatus; }
            set { _ShowStatus = value; }
        }
        public string Path
        {
            get
            {
                if (_ModulePath == null)
                    return _MainTableDefine.OrinalTableName;
                else
                    return _ModulePath;
            }
            set
            {
                _ModulePath = value;
                //if (_ModulePath != _MainTableDefine.OrinalTableName)
                //    AddLinkLabel();
            }
        }
        public string FullPath
        {
            get
            {
                if (Path.EndsWith("\\"))
                    return BasePath + Path;
                else
                    return BasePath + Path + "\\";
            }
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
            set { _CopyLastValue = value; }
        }
        public ToolDetailForm toolDetailForm
        {
            set
            {
                _ToolDetailForm = value;
                SetToolBarByRole();
                _ToolDetailForm.SetBrowerForm(this);
            }
            get
            {
                if (_ToolDetailForm == null)
                {
                    _ToolDetailForm = new ToolDetailForm();
                    _ToolDetailForm.SetBrowerForm(this);
                }
                return _ToolDetailForm;
            }
        }
        public void SetToolBarByRole()
        {
            int rightValue = -1;
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


            if (toolDetailForm.Right != null)
                rightValue = toolDetailForm.Right.RightValue;
            if (rightValue == -1)
            {
                if (toolDetailForm.OnlyDisplay)
                {
                    toolCheck.Visible = false;
                    toolUnCheck.Visible = false;
                    toolEdit.Visible = false;
                    toolCopy.Visible = false;
                    toolNew.Visible = false;
                    toolDelete.Visible = false;
                    toolView.Visible = false;
                    toolUndelete.Visible = false;
                    toolCheck.Visible = false;
                    toolUnCheck.Visible = false;
                }
                else
                {
                    toolCheck.Visible = _ToolDetailForm.AllowCheck;
                    toolUnCheck.Visible = _ToolDetailForm.AllowCheck;
                    toolEdit.Visible = _ToolDetailForm.AllowEdit;
                    toolCopy.Visible = _ToolDetailForm.AllowNew;
                    toolNew.Visible = _ToolDetailForm.AllowNew;
                    toolDelete.Visible = _ToolDetailForm.AllowEdit;
                    toolUndelete.Visible = _ToolDetailForm.AllowEdit;
                }
            }
            else
            {
                if (toolDetailForm.OnlyDisplay)
                {
                    toolCheck.Visible = false;
                    toolUnCheck.Visible = false;
                    toolEdit.Visible = false;
                    toolCopy.Visible = false;
                    toolNew.Visible = false;
                    toolDelete.Visible = false;
                    toolView.Visible = false;
                    toolUndelete.Visible = false;
                    toolCheck.Visible = false;
                    toolUnCheck.Visible = false;
                }
                else
                {
                    //toolCheck.Visible = (rightValue & 1 << 3) == 1 << 3 && _ToolDetailForm.AllowCheck;
                    //toolUnCheck.Visible = (rightValue & 1 << 3) == 1 << 3 && _ToolDetailForm.AllowCheck;
                    //toolEdit.Visible = (rightValue & 1 << 1) == 1 << 1 && _ToolDetailForm.AllowEdit;
                    //toolCopy.Visible = (rightValue & 1) == 1 && _ToolDetailForm.AllowNew;
                    //toolNew.Visible = (rightValue & 1) == 1 && _ToolDetailForm.AllowNew;
                    //toolDelete.Visible = (rightValue & 1 << 2) == 1 << 2 && _ToolDetailForm.AllowEdit;
                    //toolUndelete.Visible = (rightValue & 1 << 2) == 1 << 2 && _ToolDetailForm.AllowEdit;

                    toolCheck.Visible = UserAllowCheck && _ToolDetailForm.AllowCheck;
                    toolUnCheck.Visible = UserAllowCheck && _ToolDetailForm.AllowCheck;

                    toolEdit.Visible = UserAllowEdit && _ToolDetailForm.AllowEdit;

                    toolCopy.Visible = UserAllowCreate && _ToolDetailForm.AllowNew;
                    toolNew.Visible =  UserAllowCreate && _ToolDetailForm.AllowNew;

                    toolDelete.Visible = UserAllowDelete && _ToolDetailForm.AllowEdit;
                    toolUndelete.Visible =  UserAllowDelete && _ToolDetailForm.AllowEdit;
                }
                //设置扩展工具条的权限
                if (toolDetailForm.Right == null)
                    return;
                string exp = toolDetailForm.Right.RightExpression;
                if (exp != null && exp.Length > 0)
                {
                    string[] s = exp.Split(',');
                    int j = 0;
                    for (int i = 0; i < s.Length; i++)
                    {
                        string[] s2 = s[i].Split('=');
                        //这个逻辑实际上是有问题，但只能按权限部分将错就错
                        if (s2.Length == 2 && s2[0] != "Check" && j == 0)
                            j = 1;

                        if (s2.Length == 2 && this.toolMain.Items.ContainsKey(s2[0]))
                        {
                            this.toolMain.Items[s2[0]].Visible = (rightValue & 1 << (3 + i + j)) == 1 << (3 + i + j);
                        }
                    }
                }
            }
        }
        //public ISelectForm SelectForm
        //{
        //    set { _SelectForm = value; }
        //}
        //public IDetailFormHandler DetailFormHandler
        //{
        //    set { _DetailFormHandler = value; }
        //}
        public COMFields MainTableDefine
        {
            get { return _MainTableDefine; }
        }

        //public frmBrowser(COMFields mainTable, List<COMFields> detailTable, string where, enuShowStatus showStatus, int detailIndex, string FormCaption,Boolean AllowDoubleClick)
        //    : this()
        //{
        
        
        
        //}

        public frmBrowser(COMFields mainTable, List<COMFields> detailTable, string where, enuShowStatus showStatus)
            : this(mainTable, detailTable, where, showStatus, 0)
        {
        }

        public frmBrowser(COMFields mainTable, List<COMFields> detailTable, string where, enuShowStatus showStatus, int detailIndex, string FormCaption)
            : this()
        {

            string DataRight;

            if (FormCaption == null)
                this.Text = mainTable.Property.Title;
            else
                this.Text = FormCaption;

            cntMain = CSystem.Sys.Svr.cntMain;
            _MainTableDefine = mainTable;
            _DetailTableDefine = detailTable;
            _ShowStatus = showStatus;
            //_ShowCheck = showCheck;
            //_ShowDetail = showDetail;
            _FieldList = _MainTableDefine.FieldsName;
            _From = _MainTableDefine.TableRelation;
            string fromHistory = _MainTableDefine.GetTableRelation(true);
            _PrimaryKey = _MainTableDefine.PrimaryKey;
            _Order = _MainTableDefine.OrderBy;
         

            //2014/07/07 增加了对数据权限的过滤，根据主表定义的权限SQL来进行
            string DataRightTableName= _MainTableDefine.GetTableName(false);
            string DataRightSQL = CSystem.Sys.Svr.Operator.GetDataRightValue(DataRightTableName);

            if (DataRightSQL != string.Empty)
                _Where = DataRightSQL.Trim();


            if (_Where != null && _Where.Length > 0) 
               if (where != null && where.Length > 0)
                    _Where =  _Where  + " and " + where;
               else
                   _Where = DataRightSQL.Trim();
            else
                _Where = where;

            _DetailIndex = detailIndex;
            if ((showStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
            {
                StringBuilder sb = new StringBuilder();
                COMFields detail = _DetailTableDefine[detailIndex];
                foreach (COMField field in detail.Fields)
                {
                    sb.Append(",");
                    sb.Append(field.FullFieldName);
                    sb.Append(" D_");
                    sb.Append(field.FieldName);
                }
                string fieldlist = _MainTableDefine.FieldsName + sb.ToString();
                string from = string.Format("({0}) inner join ({1}) on {2}.ID={3}.MainID", _MainTableDefine.TableRelation, detail.TableRelation, _MainTableDefine.OrinalTableName, detail.OrinalTableName);
                fromHistory = string.Format("({0}) inner join ({1}) on {2}.ID={3}.MainID", _MainTableDefine.GetTableRelation(true), detail.GetTableRelation(true), _MainTableDefine.OrinalTableName, detail.OrinalTableName);
                string primaryKey = "D_ID";
                DBSelect = new clsSelect(_MainTableDefine.OrinalTableName, fieldlist, from, fromHistory, _Where, primaryKey, _Order);
            }
            else
                DBSelect = new clsSelect(_MainTableDefine.OrinalTableName, _FieldList, _From, fromHistory, _Where, _PrimaryKey, _Order);

            InitFilterControl(panelFilter);
            SetFilterMethod(1);
        }

        public frmBrowser(COMFields mainTable, List<COMFields> detailTable, string where, enuShowStatus showStatus, int detailIndex)
            : this(mainTable, detailTable, where, showStatus, 0, null)
        {
        }
        public string Where
        {
            set { DBSelect.Where = value; }
        }
        public DataSet MainDataSet
        {
            get { return _MainDataSet; }
        }
        public UltraGrid MainGrid
        {
            get { return grid; }
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

        void InitFilterControl(Panel p)
        {
            // 
            // butFilter
            // 
            this.butFilter = new Button();
            this.butFilter.Location = new System.Drawing.Point(460, 14);
            this.butFilter.Name = "butFilter";
            this.butFilter.Size = new System.Drawing.Size(60, 25);
            this.butFilter.TabIndex = 10;
            this.butFilter.Text = "过滤(&F)";
            this.butFilter.UseVisualStyleBackColor = true;
            // 
            // txtFilter
            // 
            this.txtFilter1 = new TextBox();
            this.txtFilter1.Location = new System.Drawing.Point(147, 14);
            this.txtFilter1.Name = "txtFilter";
            this.txtFilter1.Size = new System.Drawing.Size(460, 21);
            this.txtFilter1.TabIndex = 3;

            this.txtFilter2 = new TextBox();
            this.txtFilter2.Location = new System.Drawing.Point(147, 14);
            this.txtFilter2.Name = "txtFilter";
            this.txtFilter2.Size = new System.Drawing.Size(460, 21);
            this.txtFilter2.TabIndex = 5;
            // 
            // lab
            // 
            Label lab = new Label();
            lab.AutoSize = true;
            lab.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            lab.Location = new System.Drawing.Point(4, 15);
            lab.Name = "label1";
            lab.Size = new System.Drawing.Size(35, 14);
            lab.TabIndex = 10;
            lab.Text = "筛选";
            // 
            // cmbFilter
            // 
            this.cmbFilter = new ComboBox();
            this.cmbFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilter.FormattingEnabled = true;
            this.cmbFilter.Location = new System.Drawing.Point(42, 14);
            this.cmbFilter.Name = "cmbFilter";
            this.cmbFilter.Size = new System.Drawing.Size(100, 20);
            this.cmbFilter.TabIndex = 1;
            this.cmbFilter.TabStop = false;
            // 
            // cmbValueList
            // 
            this.cmbValueList1 = new ComboBox();
            this.cmbValueList1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbValueList1.FormattingEnabled = true;
            this.cmbValueList1.Location = new System.Drawing.Point(193, 14);
            this.cmbValueList1.Name = "cmbValueList";
            this.cmbValueList1.Size = new System.Drawing.Size(108, 20);
            this.cmbValueList1.TabIndex = 3;
            // 
            // cmbOperator
            // 
            this.cmbOperator1 = new ComboBox();
            this.cmbOperator1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOperator1.FormattingEnabled = true;
            this.cmbOperator1.Items.AddRange(new object[] {
            "=",
            ">",
            "<",
            ">=",
            "<=",
            "<>"});
            this.cmbOperator1.Location = new System.Drawing.Point(147, 14);
            this.cmbOperator1.Name = "cmbOperator";
            this.cmbOperator1.Size = new System.Drawing.Size(43, 20);
            this.cmbOperator1.TabIndex = 2;
            // 
            // datValue
            // 
            this.datValue1 = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.datValue1.DateTime = System.DateTime.Now.Date;
            this.datValue1.Location = new System.Drawing.Point(193, 14);
            this.datValue1.Name = "datValue";
            this.datValue1.Size = new System.Drawing.Size(108, 21);
            this.datValue1.TabIndex = 3;
            this.datValue1.Value = System.DateTime.Now.Date;

            // 
            // cmbValueList
            // 
            this.cmbValueList2 = new ComboBox();
            this.cmbValueList2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbValueList2.FormattingEnabled = true;
            this.cmbValueList2.Location = new System.Drawing.Point(350, 14);
            this.cmbValueList2.Name = "cmbValueList";
            this.cmbValueList2.Size = new System.Drawing.Size(108, 20);
            this.cmbValueList2.TabIndex = 5;
            // 
            // cmbOperator
            // 
            this.cmbOperator2 = new ComboBox();
            this.cmbOperator2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOperator2.FormattingEnabled = true;
            this.cmbOperator2.Items.AddRange(new object[] {
            "=",
            ">",
            "<",
            ">=",
            "<=",
            "<>"});
            this.cmbOperator2.Location = new System.Drawing.Point(305, 14);
            this.cmbOperator2.Name = "cmbOperator";
            this.cmbOperator2.Size = new System.Drawing.Size(43, 20);
            this.cmbOperator2.TabIndex = 4;
            // 
            // datValue
            // 
            this.datValue2 = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.datValue2.DateTime = System.DateTime.Now.Date;
            this.datValue2.Location = new System.Drawing.Point(350, 14);
            this.datValue2.Name = "datValue";
            this.datValue2.Size = new System.Drawing.Size(108, 21);
            this.datValue2.TabIndex = 5;
            this.datValue2.Value = System.DateTime.Now.Date;

            this.dictValue = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.dictValue.Name = "dictValue";
            this.dictValue.Location = new System.Drawing.Point(147, 14);
            this.dictValue.Size = new System.Drawing.Size(310, 21);
            this.dictValue.TabIndex = 6;
            EditorButton editButton = new EditorButton();
            dictValue.ButtonsRight.Add(editButton);
            dictValue.EditorButtonClick += new EditorButtonEventHandler(Dict_EditorButtonClick);

            if (_DetailTableDefine != null && _DetailTableDefine.Count > 0)
            {
                this.butDetail = new Button();
                this.butDetail.Location = new System.Drawing.Point(520, 14);
                this.butDetail.Name = "butDetail";
                this.butDetail.Size = new System.Drawing.Size(85, 25);
                this.butDetail.TabIndex = 11;
                this.butDetail.Text = "显示明细(&D)";
                this.butDetail.UseVisualStyleBackColor = true;
                p.Controls.Add(this.butDetail);
                this.butDetail.Click += new EventHandler(butDetail_Click);
            }

            //增加界面设置
            LinkLabel lLabel = new LinkLabel();
            lLabel.Text = "设置";
            lLabel.AutoSize = true;
            lLabel.Font = new Font(lLabel.Font.FontFamily, (float)10.5);
            lLabel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            lLabel.Location = new Point(p.Width - 30, 14);
            lLabel.Click += new EventHandler(lLabel_Click);
            p.Controls.Add(lLabel);

            panelLink = new Panel();
            panelLink.Location = new Point(610, 0);
            panelLink.Size = new Size(p.Width - 580, p.Height);
            panelLink.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;
            p.Controls.Add(panelLink);

            //增加快捷方式
            //AddLinkLabel();
            p.Controls.Add(this.datValue2);
            p.Controls.Add(this.cmbOperator2);
            p.Controls.Add(this.cmbValueList2);
            p.Controls.Add(this.txtFilter2);
            p.Controls.Add(this.datValue1);
            p.Controls.Add(this.cmbOperator1);
            p.Controls.Add(this.cmbValueList1);
            p.Controls.Add(this.butFilter);
            p.Controls.Add(this.txtFilter1);
            p.Controls.Add(lab);
            p.Controls.Add(this.cmbFilter);
            p.Controls.Add(this.dictValue);

            this.cmbFilter.SelectedIndexChanged += new EventHandler(cmbFilter_SelectedIndexChanged);
            this.butFilter.Click += new EventHandler(butFilter_Click);

        }

        private void AddLinkLabel()
        {
            panelLink.Controls.Clear();
            if (Directory.Exists(this.FullPath))
            {
                string[] files = Directory.GetFiles(this.FullPath, "*.xml", SearchOption.TopDirectoryOnly);
                int y = 0;
                for (int i = 0; i < files.Length; i++)
                {
                    //if (y > panelLink.Width-50)
                    //    break;
                    int j = files[i].LastIndexOf("_");
                    string fileName = null;
                    LinkLabel linkLabel = new LinkLabel();
                    linkLabel.Font = new Font(linkLabel.Font.FontFamily, (float)10.5);
                    if (files[i].EndsWith(".default.xml"))
                    {
                        clsListConfig config = new clsListConfig(files[i], false);
                        SetColumn(config);
                        fileName = files[i].Substring(j + 1, files[i].Length - j - 1 - ".default.xml".Length);
                        linkLabel.Font = new Font(linkLabel.Font, FontStyle.Bold);
                        linkLabel.LinkVisited = true;
                    }
                    else if (files[i].EndsWith(".xml"))
                    {
                        fileName = files[i].Substring(j + 1, files[i].Length - j - 1 - ".xml".Length);
                    }
                    else
                        continue;

                    linkLabel.Text = fileName;
                    linkLabel.Location = new Point(y, 14);
                    linkLabel.AutoSize = true;
                    linkLabel.Tag = files[i];
                    linkLabel.Click += new EventHandler(linkLabel_Click);
                    panelLink.Controls.Add(linkLabel);
                    y = y + linkLabel.Width + 20;
                }
            }
        }
        void linkLabel_Click(object sender, EventArgs e)
        {
            foreach (Control ctl in panelLink.Controls)
            {
                LinkLabel linkLabel = ctl as LinkLabel;
                linkLabel.LinkVisited = false;
            }
            LinkLabel linkLabel2 = (LinkLabel)sender;
            string fileName = (string)(linkLabel2).Tag;
            clsListConfig config = new clsListConfig(fileName, false);
            SetColumn(config);
            linkLabel2.LinkVisited = true;
        }

        void lLabel_Click(object sender, EventArgs e)
        {
            frmColumnConfig frm = new frmColumnConfig();
            clsListConfig config = null;
            if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                config = frm.ShowSelect(grid, FullPath, _MainTableDefine, _DetailTableDefine[_DetailIndex]);
            else
                config = frm.ShowSelect(grid, FullPath, _MainTableDefine, null);
            if (config != null)
                SetColumn(config);
            AddLinkLabel();
        }
        private void SetColumn(clsListConfig config)
        {
            //查询
            DBSelect.WhereOther = config.SQL;
            if (config.OrderBy.Length > 0)
                DBSelect.OrderBy = config.OrderBy;
            toolRefrash_Click(null, EventArgs.Empty);
            //设置列
            List<clsColumn> listColumn = new List<clsColumn>();
            listColumn.AddRange(config.Columns.Values);
            listColumn.Sort(new ColumnCompare());
            ColumnsCollection cols = grid.DisplayLayout.Bands[0].Columns;
            int i = 0;
            foreach (clsColumn col in listColumn)
            {
                cols[col.FieldName].Header.VisiblePosition = i;
                cols[col.FieldName].Header.Caption = col.NewDescription;
                cols[col.FieldName].Width = col.Width;
                cols[col.FieldName].Hidden = col.Hidden;
                if (col.OrderBy == 1)
                    cols[col.FieldName].SortIndicator = SortIndicator.Ascending;
                else if (col.OrderBy == -1)
                    cols[col.FieldName].SortIndicator = SortIndicator.Descending;
                else
                    cols[col.FieldName].SortIndicator = SortIndicator.None;
                i++;
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
        private string getDetailSQL(COMFields main, COMFields detail, string IDList)
        {

            StringBuilder sb = new StringBuilder();
            foreach (COMField field in main.Fields)
            {
                sb.Append("M.");
                sb.Append(field.FieldName);
                sb.Append(" M_");
                sb.Append(field.FieldName);
                sb.Append(",");
            }
            foreach (COMField field in detail.Fields)
            {
                sb.Append("D.");
                sb.Append(field.FieldName);
                sb.Append(" D_");
                sb.Append(field.FieldName);
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            return string.Format("Select {3} from ({0}) M ,({1}) D where M.ID=D.MainID and M.ID in ({2})", main.QuerySQL, detail.QuerySQL, IDList, sb.ToString());
        }
        private void SetDetailGrid(DataSet ds, UltraGrid detailGrid, COMFields main, COMFields detail)
        {
            detailGrid.UpdateMode = UpdateMode.OnCellChangeOrLostFocus;
            detailGrid.DisplayLayout.ScrollBounds = ScrollBounds.ScrollToFill;
            detailGrid.SetDataBinding(ds, null, false);
            foreach (UltraGridColumn column in detailGrid.DisplayLayout.Bands[0].Columns)
            {
                column.Hidden = true;
            }
            bool NeedSummary = false;
            detailGrid.DisplayLayout.Bands[0].Summaries.Clear();
            int i = 0;
            //根据主表设置
            NeedSummary = SetGridColumn(detailGrid, main, "M_", ref i);
            //根据辅表设置
            NeedSummary = NeedSummary || SetGridColumn(detailGrid, detail, "D_", ref i);

            if (NeedSummary)
            {
                detailGrid.DisplayLayout.Override.AllowRowSummaries = AllowRowSummaries.False;
                detailGrid.DisplayLayout.Override.SummaryValueAppearance.TextHAlign = HAlign.Right;
                detailGrid.DisplayLayout.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
            }
        }
        private bool SetGridColumn(UltraGrid detailGrid, COMFields fields, string pre, ref int i)
        {
            bool NeedSummary = false;
            foreach (COMField f in fields.Fields)
            {
                if ((f.Visible & COMField.Enum_Visible.VisibleInBrower) == COMField.Enum_Visible.VisibleInBrower)
                {
                    UltraGridColumn column = detailGrid.DisplayLayout.Bands[0].Columns[pre + f.FieldName];
                    column.Hidden = false;
                    column.Header.Caption = f.FieldTitle;
                    column.Header.VisiblePosition = i;
                    column.RowLayoutColumnInfo.OriginX = i * 2;
                    column.RowLayoutColumnInfo.OriginY = 0;
                    column.RowLayoutColumnInfo.SpanX = 2;
                    column.RowLayoutColumnInfo.SpanY = 2;
                    string[] s = f.ValueType.Split(':');
                    if (f.Format != null && f.Format.Length > 0)
                        column.Format = f.Format;
                    if (f.Enable)
                        column.CellActivation = Activation.AllowEdit;
                    else
                        column.CellActivation = Activation.NoEdit;

                    switch (s[0].ToLower())
                    {
                        case "boolean":
                            //column.e = typeof(bool);
                            if (!detailGrid.DisplayLayout.ValueLists.Exists("VL_Boolean"))
                            {
                                Infragistics.Win.ValueList list = detailGrid.DisplayLayout.ValueLists.Add("VL_Boolean");
                                list.ValueListItems.Add(0, "否");
                                list.ValueListItems.Add(1, "是");
                            }
                            column.ValueList = detailGrid.DisplayLayout.ValueLists["VL_Boolean"];
                            //column.Editor = new  Infragistics.Win.CheckEditor();
                            break;
                        case "int":
                            column.CellAppearance.TextHAlign = HAlign.Right;
                            column.Format = "#,##0;-#,##0; ";
                            column.MaskInput = "-nnn,nnn,nnn,nnn";
                            break;
                        case "number":
                            column.CellAppearance.TextHAlign = HAlign.Right;
                            string[] d = s[1].Split(',');
                            column.Format = "#,##0." + new string('0', int.Parse(d[1])) + ";-#,##0." + new string('0', int.Parse(d[1])) + "; ";
                            column.MaskInput = "-nnn,nnn,nnn,nnn." + new string('n', int.Parse(d[1]));
                            break;
                        case "enum":
                            {
                                string key = "VL_" + f.FieldName;
                                if (!detailGrid.DisplayLayout.ValueLists.Exists(key))
                                {
                                    string[] v = s[1].Split(',');
                                    Infragistics.Win.ValueList list = detailGrid.DisplayLayout.ValueLists.Add("VL_" + f.FieldName);
                                    for (int j = 0; j < v.Length; j++)
                                    {
                                        string[] n = v[j].Split('=');
                                        list.ValueListItems.Add(int.Parse(n[0]), n[1]);
                                    }
                                    column.ValueList = list;
                                }
                                break;
                            }
                        case "dict":
                        case "data":
                            {
                                //EditorWithText editor = new EditorWithText();
                                //editor.Tag = f;
                                //EditorButton but = new EditorButton();
                                //editor.ButtonsRight.Add(but);
                                //editor.KeyDown += new KeyEventHandler(editor_KeyDown);
                                //editor.EditorButtonClick += new EditorButtonEventHandler(editor_EditorButtonClick);
                                //column.Editor = editor;
                                break;
                            }
                    }
                    //设置合计行
                    if (f.ShowSummary)
                    {
                        NeedSummary = true;
                        SummarySettings ss = detailGrid.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum, column, SummaryPosition.UseSummaryPositionColumn);
                        if (column.Format != null && column.Format.Length > 0)
                            ss.DisplayFormat = "{0:" + column.Format + "}";
                    }
                    i++;
                }
            }
            return NeedSummary;
        }
        void butDetail_Click(object sender, EventArgs e)
        {
            if (!_ShowDetail)
            {
                //先判断出有几张明细表,如有多个,询问用户
                COMFields fields;
                if (_DetailTableDefine.Count == 1)
                    fields = _DetailTableDefine[0];
                else
                {
                    frmSelectDetailForm frm = new frmSelectDetailForm(_DetailTableDefine);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        fields = frm.Fields;
                    }
                    else
                        return;
                }
                //取ID列表
                if (grid.Selected.Rows.Count == 0)
                {
                    if (grid.ActiveRow != null)
                        grid.ActiveRow.Selected = true;
                    else
                        return;
                }
                StringBuilder IDList = new StringBuilder();
                foreach (UltraGridRow row in grid.Selected.Rows)
                {
                    IDList.Append("'");
                    IDList.Append(row.Cells["ID"].Value);
                    IDList.Append("',");
                }
                IDList.Remove(IDList.Length - 1, 1);
                string sql = getDetailSQL(_MainTableDefine, fields, IDList.ToString());
                DataSet ds = CSystem.Sys.Svr.cntMain.Select(sql, "Detail");
                //查询出数据
                if (gridDetail == null)
                {
                    gridDetail = new UltraGrid();
                    gridDetail.Dock = DockStyle.Fill;
                    panelGrid.Controls.Add(gridDetail);
                    grid.SendToBack();
                }
                else
                    gridDetail.Show();
                SetDetailGrid(ds, gridDetail, _MainTableDefine, fields);

                butDetail.Text = "返回(&D)";
                _ShowDetail = true;
            }
            else
            {
                gridDetail.Hide();
                butDetail.Text = "显示明细(&D)";
                _ShowDetail = false;
            }
        }
        public void toolUncheck_Click(object sender, EventArgs e)
        {
            if (Msg.Question("您确定要进行反复核操作吗?") != DialogResult.Yes)
                return;
            if (this.grid.Selected.Rows.Count == 0 && this.grid.ActiveRow != null)
            {
                this.grid.ActiveRow.Selected = true;
            }
            Guid MainID = CSystem.Sys.Svr.NullID;
            string PrimaryKey = null;
            if (_MainDataSet.Tables[0].PrimaryKey.Length > 0)
            {
                PrimaryKey = _MainDataSet.Tables[0].PrimaryKey[0].ColumnName;
                MainID = (Guid)this.grid.Selected.Rows[0].Cells[PrimaryKey].Value;
            }
            using (SqlConnection conn = new SqlConnection(cntMain.ConnectionString))
            {
                SqlTransaction sqlTran = null;
                try
                {
                    conn.Open();
                    sqlTran = conn.BeginTransaction();
                    if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                    {
                        SortedList<Guid, Guid> list = new SortedList<Guid, Guid>();
                        foreach (UltraGridRow row in this.grid.Selected.Rows)
                        {
                            Guid id = (Guid)row.Cells["ID"].Value;
                            if (row.Cells.Exists("CheckStatus"))
                            {
                                if ((int)row.Cells["CheckStatus"].Value == 1)
                                {
                                    if (!list.ContainsKey(id))
                                    {
                                        list.Add(id, id);
                                        cntMain.Excute(string.Format("update {0} set CheckStatus=CheckStatus where  id='{1}'", _MainTableDefine.OrinalTableName, id), conn, sqlTran);
                                    }
                                }
                            }
                        }
                        foreach (Guid id in list.Keys)
                        {
                            DataSet ds = cntMain.Select(string.Format("Select * from {0} where  id='{1}' and CheckStatus=1", _MainTableDefine.OrinalTableName, id), _MainTableDefine.OrinalTableName, conn, sqlTran);
                            if ((int)ds.Tables[0].Rows.Count == 1)
                            {
                                DataTable dt = ds.Tables[0];
                                DataRow dr = ds.Tables[0].Rows[0];
                                if (!UnCheck(id, conn, sqlTran, dr))
                                {
                                    sqlTran.Rollback();
                                    return;
                                }
                                if (dt.Columns.Contains("CheckStatus") && (int)dr["CheckStatus"] == 1)
                                {
                                    dr["CheckStatus"] = 0;
                                    if (dt.Columns.Contains("CheckedBy"))
                                        dr["CheckedBy"] = DBNull.Value;
                                    if (dt.Columns.Contains("CheckDate"))
                                        dr["CheckDate"] = DBNull.Value;
                                }
                                cntMain.Update(ds.Tables[0], conn, sqlTran);
                            }
                        }
                    }
                    else
                    {
                        DataTable dt = _MainDataSet.Tables[0].Clone();
                        foreach (UltraGridRow row in this.grid.Selected.Rows)
                        {
                            Guid id = (Guid)row.Cells["ID"].Value;
                            if (row.Cells.Exists("CheckStatus"))
                            {
                                cntMain.Excute(string.Format("update {0} set CheckStatus=CheckStatus where  id='{1}'", _MainTableDefine.OrinalTableName, id), conn, sqlTran);
                                DataSet ds = cntMain.Select(_MainTableDefine.QuerySQLWithClause(string.Format(" {1}.ID='{0}' and {1}.CheckStatus=1", id, _MainTableDefine.OrinalTableName)), conn, sqlTran);
                                if ((int)ds.Tables[0].Rows.Count == 1)
                                {
                                    dt.ImportRow(ds.Tables[0].Rows[0]);
                                    DataRow dr = dt.Rows[dt.Rows.Count - 1];
                                    if (!UnCheck(id, conn, sqlTran, dr))
                                    {
                                        sqlTran.Rollback();
                                        return;
                                    }
                                    if (dt.Columns.Contains("CheckStatus"))
                                        dr["CheckStatus"] = 0;
                                    if (dt.Columns.Contains("CheckedBy"))
                                        dr["CheckedBy"] = DBNull.Value;
                                    if (dt.Columns.Contains("CheckDate"))
                                        dr["CheckDate"] = DBNull.Value;
                                }
                            }
                        }
                        if (dt.Rows.Count > 0)
                        {
                            cntMain.Update(dt, conn, sqlTran);
                            _MainDataSet.Merge(dt);
                            RefreshCheckStatus();
                        }
                    }
                    sqlTran.Commit();
                }
                catch (Exception ex)
                {
                    Msg.Warning(ex.ToString());
                    sqlTran.Rollback();
                }
                finally
                {
                    conn.Close();
                }
                if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                {
                    toolRefrash_Click(this, EventArgs.Empty);
                    RefreshCheckStatus();
                }
            }
            if (PrimaryKey != null)
            {
                foreach (UltraGridRow row in this.grid.Rows)
                {
                    if ((Guid)row.Cells[PrimaryKey].Value == MainID)
                    {
                        row.Activated = true;
                        return;
                    }
                }
            }
        }
        protected Infragistics.Win.ValueList GetValueList(string table, string key)
        {
            if (this.grid.DisplayLayout.ValueLists.IndexOf(key) > -1)
                this.grid.DisplayLayout.ValueLists.Remove(key);
            Infragistics.Win.ValueList list = this.grid.DisplayLayout.ValueLists.Add(key);
            if (table != null && table.Length > 0)
            {
                try
                {
                    DataSet ds = cntMain.Select("Select * from " + table);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.ValueListItems.Add(dr["ID"], dr["Name"] as string);
                    }
                }
                catch { }
            }
            return list;
        }
        protected virtual bool UnCheck(Guid id, SqlConnection conn, SqlTransaction tran, DataRow mainRow)
        {
            if (_ToolDetailForm == null)
                return true;
            else
                return _ToolDetailForm.UnCheck(id, conn, tran, mainRow);
        }
        public void toolCheck_Click(object sender, EventArgs e)
        {
            if (Msg.Question("您确定要进行复核吗?") != DialogResult.Yes)
                return;
            if (this.grid.Selected.Rows.Count == 0 && this.grid.ActiveRow != null)
            {
                this.grid.ActiveRow.Selected = true;
            }
            Guid MainID = CSystem.Sys.Svr.NullID;
            string PrimaryKey = null;
            if (_MainDataSet.Tables[0].PrimaryKey.Length > 0)
            {
                PrimaryKey = _MainDataSet.Tables[0].PrimaryKey[0].ColumnName;
                MainID = (Guid)this.grid.Selected.Rows[0].Cells[PrimaryKey].Value;
            }
            using (SqlConnection conn = new SqlConnection(cntMain.ConnectionString))
            {
                SqlTransaction sqlTran = null;
                try
                {
                    conn.Open();
                    sqlTran = conn.BeginTransaction();
                    if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                    {
                        SortedList<Guid, Guid> list = new SortedList<Guid, Guid>();
                        foreach (UltraGridRow row in this.grid.Selected.Rows)
                        {
                            Guid id = (Guid)row.Cells["ID"].Value;
                            if (row.Cells.Exists("CheckStatus"))
                            {
                                if ((int)row.Cells["CheckStatus"].Value == 0)
                                {
                                    if (!list.ContainsKey(id))
                                    {
                                        list.Add(id, id);
                                        cntMain.Excute(string.Format("update {0} set CheckStatus=CheckStatus where  id='{1}'", _MainTableDefine.OrinalTableName, id), conn, sqlTran);
                                    }
                                }
                            }
                        }

                        foreach (Guid id in list.Keys)
                        {
                            DataSet ds = cntMain.Select(string.Format("Select * from {0} where  id='{1}' and CheckStatus=0", _MainTableDefine.OrinalTableName, id), _MainTableDefine.OrinalTableName, conn, sqlTran);
                            if ((int)ds.Tables[0].Rows.Count == 1)
                            {
                                DataTable dt = ds.Tables[0];
                                DataRow dr = ds.Tables[0].Rows[0];
                                if (!Check(id, conn, sqlTran, dr))
                                {
                                    sqlTran.Rollback();
                                    return;
                                }
                                if (dt.Columns.Contains("CheckStatus") && (int)dr["CheckStatus"] == 0)
                                {
                                    dr["CheckStatus"] = 1;
                                    if (dt.Columns.Contains("CheckedBy"))
                                        dr["CheckedBy"] = CSystem.Sys.Svr.User;
                                    if (dt.Columns.Contains("CheckedByName"))
                                        dr["CheckedByName"] = CSystem.Sys.Svr.UserName;
                                    if (dt.Columns.Contains("CheckDate"))
                                        dr["CheckDate"] = CSystem.Sys.Svr.SystemTime;
                                }
                                cntMain.Update(ds.Tables[0], conn, sqlTran);
                            }
                        }
                    }
                    else
                    {
                        DataTable dt = _MainDataSet.Tables[0].Clone();
                        foreach (UltraGridRow row in this.grid.Selected.Rows)
                        {
                            Guid id = (Guid)row.Cells["ID"].Value;
                            if (row.Cells.Exists("CheckStatus"))
                            {
                                cntMain.Excute(string.Format("update {0} set CheckStatus=CheckStatus where  id='{1}'", _MainTableDefine.OrinalTableName, id), conn, sqlTran);
                                DataSet ds = cntMain.Select(_MainTableDefine.QuerySQLWithClause(string.Format(" {1}.ID='{0}' and {1}.CheckStatus=0", id, _MainTableDefine.OrinalTableName)), conn, sqlTran);
                                if ((int)ds.Tables[0].Rows.Count == 1)
                                {
                                    dt.ImportRow(ds.Tables[0].Rows[0]);
                                    DataRow dr = dt.Rows[dt.Rows.Count - 1];
                                    if (!Check(id, conn, sqlTran, dr))
                                    {
                                        sqlTran.Rollback();
                                        return;
                                    }
                                    if (dt.Columns.Contains("CheckStatus"))
                                        dr["CheckStatus"] = 1;
                                    if (dt.Columns.Contains("CheckedBy"))
                                        dr["CheckedBy"] = CSystem.Sys.Svr.User;
                                    if (dt.Columns.Contains("CheckedByName"))
                                        dr["CheckedByName"] = CSystem.Sys.Svr.UserName;
                                    if (dt.Columns.Contains("CheckDate"))
                                        dr["CheckDate"] = CSystem.Sys.Svr.SystemTime;
                                }
                            }
                        }

                        if (dt.Rows.Count > 0)
                        {
                            cntMain.Update(dt, conn, sqlTran);
                            _MainDataSet.Merge(dt);
                            RefreshCheckStatus();
                        }
                    }
                    sqlTran.Commit();
                }
                catch (Exception ex)
                {
                    Msg.Warning(ex.ToString());
                    sqlTran.Rollback();
                }
                finally
                {
                    conn.Close();
                }
                if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                {
                    toolRefrash_Click(this, EventArgs.Empty);
                    RefreshCheckStatus();
                }
            }
            if (PrimaryKey != null)
            {
                foreach (UltraGridRow row in this.grid.Rows)
                {
                    if ((Guid)row.Cells[PrimaryKey].Value == MainID)
                    {
                        row.Activated = true;
                        return;
                    }
                }
            }
        }
        public void RefreshCheckStatus()
        {
            if (grid.DisplayLayout.Bands[0].Columns.Exists("CheckStatus") || grid.DisplayLayout.Bands[0].Columns.Exists("BillStatus") || grid.DisplayLayout.Bands[0].Columns.Exists("Deleted") || grid.DisplayLayout.Bands[0].Columns.Exists("IsCustomer"))
            {
                foreach (UltraGridRow row in grid.Rows)
                {
                    if (row.Cells.Exists("CheckStatus"))
                    {
                        if (row.Cells["CheckStatus"].Value != DBNull.Value && (int)row.Cells["CheckStatus"].Value == 1)
                            row.Appearance.BackColor = System.Drawing.Color.BurlyWood;
                        else
                            row.Appearance.BackColor = new System.Drawing.Color();
                    }
                    if (row.Cells.Exists("Deleted"))
                    {
                        if (row.Cells["Deleted"].Value != DBNull.Value && (int)row.Cells["Deleted"].Value == 1)
                            row.Appearance.BackColor = System.Drawing.Color.Red;
                    }
                    toolDetailForm.RefrashCheckStatus(row);
                }
            }
        }
        protected virtual bool Check(Guid id, SqlConnection conn, SqlTransaction tran, DataRow mainRow)
        {
            if (_ToolDetailForm == null)
                return true;
            else
                return _ToolDetailForm.Check(id, conn, tran, mainRow);
        }
        void grid_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {


            if (e.DisplayPromptMsg)
            {
                e.DisplayPromptMsg = false;
                if (Msg.Question("您确认要删除所选的记录吗？") == DialogResult.Yes)
                {
                    //获取Grid中的下一行的Index
                    int minIndex = this.grid.Rows.Count - 1;
                    foreach (UltraGridRow row in e.Rows)
                    {
                        if (minIndex > row.Index)
                            minIndex = row.Index;
                    }
                    using (SqlConnection conn = new SqlConnection(cntMain.ConnectionString))
                    {
                        SqlTransaction sqlTran = null;
                        try
                        {
                            conn.Open();
                            sqlTran = conn.BeginTransaction();
                            if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                            {
                                SortedList<Guid, Guid> list = new SortedList<Guid, Guid>();
                                foreach (UltraGridRow row in e.Rows)
                                {
                                    Guid id = (Guid)row.Cells["ID"].Value;
                                    if (row.Cells.Exists("CheckStatus"))
                                    {
                                        if ((int)row.Cells["CheckStatus"].Value == 0)
                                        {
                                            DataSet dsTemp = cntMain.Select(string.Format("Select id from {0} where id='{1}' and CheckStatus=0", _MainTableDefine.OrinalTableName, id), conn, sqlTran);
                                            if (!list.ContainsKey(id) && dsTemp.Tables[0].Rows.Count == 1)
                                                list.Add(id, id);
                                        }
                                    }
                                }
                                foreach (Guid id in list.Keys)
                                {
                                    if (!DeleteData(conn, sqlTran, id))
                                    {
                                        e.Cancel = true;
                                        sqlTran.Rollback();
                                        break;
                                    }
                                    foreach (COMFields d in _DetailTableDefine)
                                        cntMain.Excute(string.Format("Delete from {0} where {1}='{2}'", d.OrinalTableName, _MainID, id.ToString()), conn, sqlTran);
                                    cntMain.Excute(string.Format("Delete from {0} where  id='{1}'", _MainTableDefine.OrinalTableName, id), conn, sqlTran);
                                }
                            }
                            else
                            {
                                foreach (UltraGridRow row in e.Rows)
                                {
                                    if (row.Cells.Exists("CheckStatus"))
                                    {
                                        if ((int)row.Cells["CheckStatus"].Value == 1)
                                        {
                                            Msg.Warning("您要删除的数据中包含已复核的数据！");
                                            e.Cancel = true;
                                            sqlTran.Rollback();
                                            break;
                                        }
                                        else
                                        {
                                            DataSet dsTemp = cntMain.Select(string.Format("Select id from {0} where id='{1}' and CheckStatus=1", _MainTableDefine.OrinalTableName, row.Cells["ID"].Value), conn, sqlTran);
                                            if (dsTemp.Tables[0].Rows.Count == 1)
                                            {
                                                Msg.Warning("您要删除的数据中包含已复核的数据！");
                                                e.Cancel = true;
                                                sqlTran.Rollback();
                                                break;
                                            }
                                        }
                                    }
                                    DataRow[] drs = _MainDataSet.Tables[_MainTableDefine.OrinalTableName].Select("ID = '" + row.Cells["ID"].Value + "'");
                                    DataSet ds = _MainDataSet.Clone();
                                    drs[0].AcceptChanges();
                                    ds.Tables[_MainTableDefine.OrinalTableName].ImportRow(drs[0]);
                                    ds.Tables[_MainTableDefine.OrinalTableName].Rows[0].AcceptChanges();
                                    if (grid.DisplayLayout.Bands[0].Columns.Exists("Disable"))
                                        ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["Disable"] = 1;
                                    else
                                        ds.Tables[_MainTableDefine.OrinalTableName].Rows[0].Delete();

                                    //Add by Jiayuan 2012/11/25 对S_Table和S_TableField的删除处理
                                    COMField IDField = new COMField();

                                    if (CSystem.Sys.Svr.LDI.GetFields(_MainTableDefine.OrinalTableName).FieldsName.Contains("ID"))
                                        IDField = CSystem.Sys.Svr.LDI.GetFields(_MainTableDefine.OrinalTableName)["ID"];
                                    //if ((_MainTableDefine.OrinalTableName.Equals("S_TableField")) || (_MainTableDefine.OrinalTableName.Equals("S_BaseInfo")))
                                    if (IDField.ValueType.Contains("String"))
                                    {
                                        //nothing
                                    }
                                    else
                                    {
                                        Guid id = (Guid)drs[0]["ID"];
                                        //执行子类的一些删除操作,包括做一些检测
                                        if (!DeleteData(conn, sqlTran, id))
                                        {
                                            e.Cancel = true;
                                            sqlTran.Rollback();
                                            break;
                                        }
                                        if (!grid.DisplayLayout.Bands[0].Columns.Exists("Disable") && _DetailTableDefine.Count > 0)
                                        {
                                            foreach (COMFields d in _DetailTableDefine)
                                                if (d.Property.TableType != CTableProperty.enuTableType.View)
                                                    cntMain.Excute(string.Format("Delete from {0} where {1}='{2}'", d.OrinalTableName, _MainID, id.ToString()), conn, sqlTran);
                                        }
                                    }

                                    cntMain.Update(ds.Tables[_MainTableDefine.OrinalTableName], conn, sqlTran);
                                }
                            }
                            if (!e.Cancel)
                                sqlTran.Commit();
                        }
                        catch (Exception ex)
                        {
                            if (sqlTran != null)
                                sqlTran.Rollback();
                            Msg.Error(ex.ToString());
                            e.Cancel = true;
                        }
                        finally
                        {
                            conn.Close();
                        }
                        if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                        {
                            e.Cancel = true;
                            toolRefrash_Click(this, EventArgs.Empty);
                            RefreshCheckStatus();
                            if (minIndex < grid.Rows.Count)
                                grid.Rows[minIndex].Activate();
                        }
                        else
                        {
                            if (minIndex < grid.Rows.Count - e.Rows.Length)
                                grid.Rows[minIndex].Activate();
                            else if (minIndex > 0)
                                grid.Rows[minIndex - 1].Activate();
                        }
                    }
                }
                else
                    e.Cancel = true;
            }
            else
                e.Cancel = true;
        }

        public DataRow ShowSelect(string field, string value, string Other)
        {
            DBSelect.WhereOther = Other;
            if (value != null && value.Length > 0)
            {
                DBSelect.WhereFilter = string.Format("{0} like '{1}%'", field, value);
                _MainDataSet = cntMain.Select(DBSelect.SelectSql());
                if (_MainDataSet.Tables[0].Rows.Count == 1)
                    return _MainDataSet.Tables[0].Rows[0];
            }
            toolDelete.Visible = false;
            IsSelectMode = true;
            this.Width = 750;
            load();
            if (value != null && value.Length > 0)
            {
                txtFilter1.Text = value;
                if (field != null && field.Length > 0)
                {
                    for (int i = 1; i < cmbFilter.Items.Count; i++)
                    {
                        COMField item = (COMField)cmbFilter.Items[i];
                        if (item.FullFieldName.CompareTo(field) == 0)
                        {
                            cmbFilter.SelectedIndex = i;
                        }
                    }
                    butFilter_Click(null, EventArgs.Empty);
                }
            }
            if (this.ShowDialog() == DialogResult.OK)
                return Result;
            else
                return null;
        }
        public DataRow[] ShowSelectRows(string field, string value, string Other)
        {
            DBSelect.WhereOther = Other;
            if (value != null && value.Length > 0)
            {
                DBSelect.WhereFilter = string.Format("{0} like '{1}%'", field, value);
                _MainDataSet = cntMain.Select(DBSelect.SelectSql());
                if (_MainDataSet.Tables[0].Rows.Count == 1)
                    return new DataRow[] { _MainDataSet.Tables[0].Rows[0] };
            }
            toolDelete.Visible = false;
            IsSelectMode = true;
            this.Width = 750;
            load();
            if (value != null && value.Length > 0)
            {
                txtFilter1.Text = value;
                if (field != null && field.Length > 0)
                {
                    for (int i = 0; i < cmbFilter.Items.Count; i++)
                    {
                        COMField item = cmbFilter.Items[i] as COMField;
                        if (item != null && item.FullFieldName.CompareTo(field) == 0)
                        {
                            cmbFilter.SelectedIndex = i;
                        }
                    }
                    butFilter_Click(null, EventArgs.Empty);
                }
            }
            if (this.ShowDialog() == DialogResult.OK)
                return Results;
            else
                return null;
        }

        public void load()
        {
            if (DataLoaded) return;
            _MainDataSet = DBSelect.RegetPageData();
            if (_MainDataSet == null) return;
            if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                new CCreateGrid(_MainTableDefine, _DetailTableDefine[_DetailIndex], this.grid, _MainDataSet.Tables[_MainTableDefine.OrinalTableName].DefaultView, COMField.Enum_Visible.VisibleInBrower, null);
            else
                new CCreateGrid(_MainTableDefine, this.grid, _MainDataSet.Tables[_MainTableDefine.OrinalTableName].DefaultView, COMField.Enum_Visible.VisibleInBrower, null);
            //按排序规则设置列的排序
            List<COMField> sortColumns = _MainTableDefine.SortColumns;
            for (int i = 0; i < sortColumns.Count; i++)
            {
                COMField f = sortColumns[i];
                if (f.OrderType == COMField.Enum_OrderType.ASC)
                    this.grid.DisplayLayout.Bands[0].Columns[f.FieldName].SortIndicator = SortIndicator.Ascending;
                else
                    this.grid.DisplayLayout.Bands[0].Columns[f.FieldName].SortIndicator = SortIndicator.Descending;
            }
            //根据相关对应设置列头属性
            cmbFilter.Items.Clear();
            cmbFilter.Items.Add(new DataItem("All", "<全部>"));

            String tableName = this._MainTableDefine.GetTableName(false);
            string PreviousFilterFiledName;

            PreviousFilterFiledName = CSystem.Sys.Svr.Preferences.GetValue(tableName);
            if (string.IsNullOrEmpty(PreviousFilterFiledName) == false)
                this.FilterFieldName = PreviousFilterFiledName;

            foreach (Base.COMField f in _MainTableDefine.Fields)
            {
                if ((f.Visible & COMField.Enum_Visible.VisibleInBrower) == COMField.Enum_Visible.VisibleInBrower)
                {
                    cmbFilter.Items.Add(f);

                    if (f.FieldName == _FilterFieldName)
                        cmbFilter.SelectedIndex = cmbFilter.Items.Count - 1;
                }
            }


            if ((ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
            {
                foreach (Base.COMField f in _DetailTableDefine[_DetailIndex].Fields)
                {
                    if ((f.Visible & COMField.Enum_Visible.VisibleInBrower) == COMField.Enum_Visible.VisibleInBrower)
                    {
                        cmbFilter.Items.Add(f);
                    }
                }
            }

            RefreshCheckStatus();
            DataLoaded = true;
        }

        private void frmBrowser_Load(object sender, EventArgs e)
        {
            load();
            AddLinkLabel();
            if (!IsSelectMode)
                toolSelect.Visible = false;
            if (_ToolDetailForm != null)
                _ToolDetailForm.InsertToolStrip(this.toolMain, ToolDetailForm.enuInsertToolStripType.Browser);
            if ((_ShowStatus & enuShowStatus.ShowCheck) != enuShowStatus.ShowCheck)
            {
                toolCheck.Visible = false;
                toolUnCheck.Visible = false;
            }
            else if (_ToolDetailForm != null)
            {
                int rightValue = _ToolDetailForm.Right.RightValue;
                if (rightValue == -1)
                {
                    toolCheck.Visible = _ToolDetailForm.AllowCheck;
                    toolUnCheck.Visible = _ToolDetailForm.AllowCheck;
                }
                else
                {
                    toolCheck.Visible = (rightValue & 1 << 3) == 1 << 3 && _ToolDetailForm.AllowCheck;
                    toolUnCheck.Visible = (rightValue & 1 << 3) == 1 << 3 && _ToolDetailForm.AllowCheck;
                }
            }
            else
            {
                toolCheck.Visible = true;
                toolUnCheck.Visible = true;
            }
            toolStripHistory.Visible = this.MainTableDefine.Property.HistoryTable;
        }
        public virtual frmDetail GetDetailForm()
        {
            frmDetail frm = new frmDetail();
            frm.Path = this.FullPath;
            frm.IsHistory = _IsHistory;
            return frm;
        }
        public virtual frmDetail GetDetailForm(bool IsNew)
        {
            frmDetail frm = GetDetailForm();
            if (_IsHistory)
                frm.IsHistory = !IsNew;
            return frm;
        }
        public virtual void toolNew_Click(object sender, EventArgs e)
        {
            frmDetail frm = GetDetailForm(true);
            if (frm != null)
            {
                frm.DefaultValue = _DefaultValue;
                frm.CopyLastValue = _CopyLastValue;
                frm.toolDetailForm = _ToolDetailForm;
                //frm.SelectForm = _SelectForm;
                //frm.DetailFormHandler = _DetailFormHandler;
                frm.Changed += new DataTableEventHandler(frm_Changed);
                frm.PageDown += new DataTableEventHandler(frm_PageDown);
                frm.PageUp += new DataTableEventHandler(frm_PageUp);
                bool bShowData = false;
                if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                {
                    DataSet ds = GetMainDataSet();
                    bShowData = frm.ShowData(_MainTableDefine, _DetailTableDefine, new DataSetEventArgs(ds), true, this.MdiParent, this.Text + "[新增]");
                }
                else
                {
                    bShowData = frm.ShowData(_MainTableDefine, _DetailTableDefine, new DataSetEventArgs(_MainDataSet.Clone()), true, this.MdiParent, this.Text + "[新增]");
                }
                if (bShowData && this.MdiParent != null)
                {
                    frm.FormClosed += new FormClosedEventHandler(frm_FormClosed);
                    SetEnabled(false);
                }
            }
        }

        public void frm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetEnabled(true);
        }
        public void SetEnabled(bool enabled)
        {
            foreach (Control ctl in this.Controls)
                ctl.Enabled = enabled;
        }
        public virtual DataSet GetMainDataSet(Guid ID)
        {
            return cntMain.Select(string.Format("{0} where {2}.ID='{1}'", _MainTableDefine.GetQuerySQL(_IsHistory), ID, _MainTableDefine.OrinalTableName), _MainTableDefine.OrinalTableName);
        }
        public virtual DataSet GetMainDataSet()
        {
            return cntMain.Select(string.Format("{0} where 1=0", _MainTableDefine.QuerySQL), _MainTableDefine.OrinalTableName);
        }
        public virtual void toolView_Click(object sender, EventArgs e)
        {
            if (this.grid.ActiveRow != null)
            {
                DataSet ds;
                if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                {
                    ds = GetMainDataSet((Guid)grid.ActiveRow.Cells["ID"].Value);
                }
                else
                {
                    if (_MainDataSet.Tables[0].Columns.Contains("ID"))
                    {
                        DataRow[] drs = _MainDataSet.Tables[0].Select("ID = '" + grid.ActiveRow.Cells["ID"].Value + "'");
                        ds = _MainDataSet.Clone();
                        ds.Tables[0].ImportRow(drs[0]);
                    }
                    else if (_MainDataSet.Tables[0].TableName.Equals("S_Table"))
                    {
                        DataRow[] drs = _MainDataSet.Tables[0].Select("TableName = '" + grid.ActiveRow.Cells["TableName"].Value + "'");
                        ds = _MainDataSet.Clone();
                        ds.Tables[0].ImportRow(drs[0]);
                    }
                    else
                    {
                        DataRow[] drs = _MainDataSet.Tables[0].Select("ID = '" + grid.ActiveRow.Cells["ID"].Value + "'");
                        ds = _MainDataSet.Clone();
                        ds.Tables[0].ImportRow(drs[0]);
                    }
                }
                frmDetail frm = GetDetailForm(false);
                if (frm != null)
                {
                    if (this.MdiParent != null)
                        frm.MdiParent = this.MdiParent;
                    frm.DefaultValue = _DefaultValue;
                    frm.CopyLastValue = _CopyLastValue;
                    frm.toolDetailForm = _ToolDetailForm;
                    //frm.SelectForm = _SelectForm;
                    //frm.DetailFormHandler = _DetailFormHandler;
                    frm.Changed += new DataTableEventHandler(frm_Changed);
                    frm.PageDown += new DataTableEventHandler(frm_PageDown);
                    frm.PageUp += new DataTableEventHandler(frm_PageUp);

                    bool bShowData = frm.ShowData(_MainTableDefine, _DetailTableDefine, new DataSetEventArgs(ds, this.grid.ActiveRow.HasNextSibling(), this.grid.ActiveRow.HasPrevSibling()), false, this.MdiParent);
                    if (bShowData && this.MdiParent != null)
                    {
                        frm.FormClosed += new FormClosedEventHandler(frm_FormClosed);
                        SetEnabled(false);
                    }
                }
            }
        }

        public virtual void frm_Changed(object sender, DataTableEventArgs e)
        {
            if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
            {
                if (e.Data.Rows[0].RowState != DataRowState.Deleted)
                {
                    string s = DBSelect.WhereFilter;
                    if (e.Data.Columns.Contains("ID"))
                    {
                        DBSelect.WhereFilter = string.Format("{1}.ID = '{0}'", e.Data.Rows[0]["ID"], _MainTableDefine.OrinalTableName);
                        DataSet ds = cntMain.Select(DBSelect.SelectSql(), _MainTableDefine.OrinalTableName);
                        DBSelect.WhereFilter = s;
                        _MainDataSet.Tables[0].Merge(ds.Tables[0]);
                    }
                    RefreshCheckStatus();
                }
                else
                {
                    toolRefrash_Click(null, EventArgs.Empty);
                }
            }
            else
            {
                try
                {
                    e.Data.AcceptChanges();
                    _MainDataSet.Tables[0].Merge(e.Data);
                }
                catch { }
                RefreshCheckStatus();
            }
            if (e.Data.Rows[0].RowState != DataRowState.Deleted)
            {
                COMField IDField = new COMField();
                if (CSystem.Sys.Svr.LDI.GetFields(_MainTableDefine.OrinalTableName).FieldsName.Contains("ID"))
                    IDField = CSystem.Sys.Svr.LDI.GetFields(_MainTableDefine.OrinalTableName)["ID"];

                foreach (UltraGridRow row in grid.Rows)

                    // if (e.Data.TableName.Equals("S_Table"))
                    //{

                    //    if ((string)e.Data.Rows[0]["TableName"]== (string)row.Cells["TableName"].Value)
                    //    {
                    //        e.HasNext = row.HasNextSibling();
                    //        e.HasPrevious = row.HasPrevSibling();
                    //        row.Activate();
                    //        break;
                    //    }

                    //}
                    //else if((e.Data.TableName.Equals("S_TableField")) || (e.Data.TableName.Equals("S_BaseInfo")) )
                    //{
                    //    if ((string)e.Data.Rows[0]["ID"] == (string)row.Cells["ID"].Value)
                    //    {
                    //        e.HasNext = row.HasNextSibling();
                    //        e.HasPrevious = row.HasPrevSibling();
                    //        row.Activate();
                    //        break;
                    //    }

                    //}
                    if (IDField.ValueType.Contains("String"))
                    {
                        if ((string)e.Data.Rows[0]["ID"] == (string)row.Cells["ID"].Value)
                        {
                            e.HasNext = row.HasNextSibling();
                            e.HasPrevious = row.HasPrevSibling();
                            row.Activate();
                            break;
                        }
                    }
                    else
                    {
                        if ((Guid)e.Data.Rows[0]["ID"] == (Guid)row.Cells["ID"].Value)
                        {
                            e.HasNext = row.HasNextSibling();
                            e.HasPrevious = row.HasPrevSibling();
                            row.Activate();
                            break;
                        }

                    }


                RefreshCheckStatus();
            }
        }

        public virtual void toolEdit_Click(object sender, EventArgs e)
        {
            if (this.grid.ActiveRow != null)
            {
                if (!this.grid.DisplayLayout.Bands[0].Columns.Exists("CheckStatus") || (int)this.grid.ActiveRow.Cells["CheckStatus"].Value != 1)
                {
                    DataSet ds;
                    if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                    {
                        ds = cntMain.Select(string.Format("{0} where {2}.ID='{1}'", _MainTableDefine.QuerySQL, grid.ActiveRow.Cells["ID"].Value, _MainTableDefine.OrinalTableName), _MainTableDefine.OrinalTableName);
                    }
                    else
                    {
                        DataRow[] drs = _MainDataSet.Tables[0].Select("ID = '" + grid.ActiveRow.Cells["ID"].Value + "'");
                        ds = _MainDataSet.Clone();
                        ds.Tables[0].ImportRow(drs[0]);
                    }
                    frmDetail frm = GetDetailForm(false);
                    if (frm != null)
                    {
                        frm.DefaultValue = _DefaultValue;
                        frm.CopyLastValue = _CopyLastValue;
                        frm.toolDetailForm = _ToolDetailForm;
                        //frm.SelectForm = _SelectForm;
                        //frm.DetailFormHandler = _DetailFormHandler;
                        frm.Changed += new DataTableEventHandler(frm_Changed);
                        frm.PageDown += new DataTableEventHandler(frm_PageDown);
                        frm.PageUp += new DataTableEventHandler(frm_PageUp);

                        bool bShowData = frm.ShowData(_MainTableDefine, _DetailTableDefine, new DataSetEventArgs(ds, this.grid.ActiveRow.HasNextSibling(), this.grid.ActiveRow.HasPrevSibling()), true, this.MdiParent);
                        if (bShowData && this.MdiParent != null)
                        {
                            frm.FormClosed += new FormClosedEventHandler(frm_FormClosed);
                            SetEnabled(false);
                        }
                    }
                }
            }
        }

        void frm_PageUp(object sender, DataTableEventArgs e)
        {
            UltraGridRow row = grid.ActiveRow;
            if (row != null && row.Index > 0)
            {
                Guid lastid = (Guid)row.Cells["ID"].Value;
                Guid id = Guid.Empty;
                while (row.Index > 0)
                {
                    row.Selected = false;
                    row = grid.Rows[row.Index - 1];
                    row.Activate();
                    id = (Guid)row.Cells["ID"].Value;
                    if (id != lastid)
                        break;
                }
                if (id == lastid)
                {
                    e.HasPrevious = false;
                    e.HasNext = row.HasNextSibling();
                    return;
                }

                if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                {
                    DataSet ds = cntMain.Select(string.Format("{0} where {2}.ID='{1}'", _MainTableDefine.QuerySQL, grid.ActiveRow.Cells["ID"].Value, _MainTableDefine.OrinalTableName), _MainTableDefine.OrinalTableName);
                    e.Data.ImportRow(ds.Tables[0].Rows[0]);
                }
                else
                {
                    DataRow[] drs = _MainDataSet.Tables[0].Select("ID = '" + id + "'");
                    e.Data.ImportRow(drs[0]);
                }
                e.HasPrevious = row.HasPrevSibling();
                e.HasNext = row.HasNextSibling();
            }
        }

        void frm_PageDown(object sender, DataTableEventArgs e)
        {
            //这里需要加入对S_Table和S_TableField两个特殊类型的处理，暂时未实现。
            //Comment by Jiayuan 2012/11/23

            UltraGridRow row = grid.ActiveRow;
            if (row != null && row.Index < grid.Rows.Count - 1)
            {
                Guid lastid = (Guid)row.Cells["ID"].Value;
                Guid id = Guid.Empty;
                while (row.Index < grid.Rows.Count - 1)
                {
                    row.Selected = false;
                    row = grid.Rows[row.Index + 1];
                    row.Activate();
                    id = (Guid)row.Cells["ID"].Value;
                    if (id != lastid)
                        break;
                }
                if (id == lastid)
                {
                    e.HasNext = false;
                    e.HasPrevious = row.HasPrevSibling();
                    return;
                }
                if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                {
                    DataSet ds = cntMain.Select(string.Format("{0} where {2}.ID='{1}'", _MainTableDefine.QuerySQL, id, _MainTableDefine.OrinalTableName), _MainTableDefine.OrinalTableName);
                    e.Data.ImportRow(ds.Tables[0].Rows[0]);
                }
                else
                {
                    DataRow[] drs = _MainDataSet.Tables[0].Select("ID = '" + id + "'");
                    e.Data.ImportRow(drs[0]);
                }
                e.HasPrevious = row.HasPrevSibling();
                e.HasNext = row.HasNextSibling();
            }
        }
        protected virtual bool DeleteData(SqlConnection conn, SqlTransaction tran, Guid id)
        {
            if (_ToolDetailForm != null)
                _ToolDetailForm.Deleting(conn, tran, id);
            return true;
        }
        private void toolDelete_Click(object sender, EventArgs e)
        {
            if (this.grid.Selected.Rows.Count == 0 && this.grid.ActiveRow != null)
            {
                this.grid.ActiveRow.Selected = true;
            }
            if (this.grid.Selected.Rows.Count > 0)
                this.grid.PerformAction(UltraGridAction.DeleteRows);
        }

        private DataRow Result;
        private DataRow[] Results;
        protected void toolSelect_Click(object sender, EventArgs e)
        {
            //this.grid.Selected.Rows.Clear();
            if (this.grid.Selected.Rows.Count == 0)
            {
                if (this.grid.ActiveRow == null)
                    return;
                else
                    this.grid.ActiveRow.Selected = true;
            }
            StringBuilder sb = new StringBuilder();
            foreach (UltraGridRow row in grid.Selected.Rows)
            {
                if (sb.Length > 0)
                    sb.Append(" or ");
                sb.Append("ID='");
                sb.Append(row.Cells["ID"].Value);
                sb.Append("'");
            }
            DataRow[] dr = _MainDataSet.Tables[0].Select(sb.ToString());
            Result = dr[0];
            Results = dr;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void toolClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void toolRefrash_Click(object sender, EventArgs e)
        {
            DataLoaded = false;
            load();
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilter1.SelectionStart = 0;
            txtFilter1.SelectionLength = txtFilter1.Text.Length;

            String tableName = this._MainTableDefine.GetTableName(false);
            if (cmbFilter.SelectedItem.GetType() == typeof(Base.COMField))
            {
                Base.COMField SelectedFilterItem = (Base.COMField)cmbFilter.SelectedItem;
                string SelectedFilterFiledName = SelectedFilterItem.FieldName;
                CSystem.Sys.Svr.Preferences.SetValue(tableName, SelectedFilterFiledName);
            }


            if (cmbFilter.SelectedIndex > -1 && cmbValueList1 != null)
            {
                COMField item = cmbFilter.Items[cmbFilter.SelectedIndex] as COMField;
                if (item == null)
                {
                    _MainDataSet.Tables[0].DefaultView.RowFilter = "";
                }
                else if (grid.DisplayLayout.Bands[0].Columns.Exists(item.FieldName))
                {
                    UltraGridColumn col = grid.DisplayLayout.Bands[0].Columns[item.FieldName];
                    if (col.ValueList != null)
                    {
                        ValueList vl = (ValueList)col.ValueList;
                        cmbValueList1.Items.Clear();
                        cmbValueList2.Items.Clear();
                        foreach (Infragistics.Win.ValueListItem i in vl.ValueListItems)
                        {
                            cmbValueList1.Items.Add(i);
                            cmbValueList2.Items.Add(i);
                        }
                        SetFilterMethod(2);
                        return;
                    }
                    else
                    {
                        if (item.ValueType.StartsWith("Data") || item.ValueType.StartsWith("Dict"))
                        {
                            SetFilterMethod(5);
                            dictValue.Tag = item;
                        }
                        else if (col.DataType == typeof(decimal) || col.DataType == typeof(int))
                            SetFilterMethod(3);
                        else if (col.DataType == typeof(DateTime))
                            SetFilterMethod(4);
                        else
                            SetFilterMethod(1);
                    }
                }
            }
            if (cmbValueList1 != null) cmbValueList1.Visible = false;
            //txtFilter.Visible = true;
        }
        //查询状态：
        //1.用Like，只有txtFilter显示且大小如cmbValueList
        //2.用＝且为列表，只有cmbVlaueList;
        //3.用运算符，txt只有Date大小
        //4.用运算符，只有Date
        private void SetFilterMethod(int i)
        {
            FilterMethod = i;
            switch (i)
            {
                case 1:
                    txtFilter1.Visible = true;
                    txtFilter1.Width = dictValue.Width;
                    txtFilter1.Left = dictValue.Left;
                    txtFilter2.Visible = false;
                    cmbValueList1.Visible = false;
                    cmbOperator1.Visible = false;
                    datValue1.Visible = false;
                    cmbValueList2.Visible = false;
                    cmbOperator2.Visible = false;
                    datValue2.Visible = false;
                    dictValue.Visible = false;
                    break;
                case 2:
                    txtFilter1.Visible = false;
                    txtFilter2.Visible = false;
                    cmbValueList1.Visible = true;
                    cmbOperator1.Visible = true;
                    cmbOperator1.SelectedIndex = 0;
                    datValue1.Visible = false;
                    cmbValueList2.Visible = true;
                    cmbOperator2.Visible = true; ;
                    cmbOperator1.SelectedIndex = -1;
                    datValue2.Visible = false;
                    dictValue.Visible = false;
                    break;
                case 3:
                    txtFilter1.Visible = true;
                    cmbOperator1.Visible = true;
                    cmbOperator1.SelectedIndex = 0;
                    txtFilter1.Width = datValue1.Width;
                    txtFilter1.Left = datValue1.Left;
                    datValue1.Visible = false;
                    cmbValueList1.Visible = false;
                    dictValue.Visible = false;

                    txtFilter2.Visible = true;
                    cmbOperator2.Visible = true;
                    cmbOperator2.SelectedIndex = -1;
                    txtFilter2.Width = datValue2.Width;
                    txtFilter2.Left = datValue2.Left;
                    datValue2.Visible = false;
                    cmbValueList2.Visible = false;
                    break;
                case 4:
                    txtFilter1.Visible = false;
                    cmbOperator1.Visible = true;
                    cmbOperator1.SelectedIndex = 0;
                    datValue1.Visible = true;
                    cmbValueList1.Visible = false;

                    txtFilter2.Visible = false;
                    cmbOperator2.Visible = true;
                    cmbOperator2.SelectedIndex = -1;
                    datValue2.Visible = true;
                    cmbValueList2.Visible = false;

                    dictValue.Visible = false;
                    break;
                case 5:
                    txtFilter1.Visible = false;
                    cmbOperator1.Visible = false;
                    datValue1.Visible = false;
                    cmbValueList1.Visible = false;

                    txtFilter2.Visible = false;
                    cmbOperator2.Visible = false;
                    datValue2.Visible = false;
                    cmbValueList2.Visible = false;

                    dictValue.Visible = true;
                    break;
            }
        }
        //private bool filterTextChanged = false;
        private void butFilter_Click(object sender, EventArgs e)
        {
            //if (!filterTextChanged && txtFilter.Visible && FilterMethod == 1)
            //{
            //    if (grid.ActiveRow != null)
            //    {
            //        if (IsSelectMode)
            //            toolSelect_Click(sender, EventArgs.Empty);
            //        else
            //            toolView_Click(sender, EventArgs.Empty);
            //        return;
            //    }
            //}
            //else
            //    filterTextChanged = false;
            if (cmbFilter.SelectedIndex > 0)
            {
                string filterTable = "";
                string filterDB = "";
                COMField item = cmbFilter.Items[cmbFilter.SelectedIndex] as COMField;
                if (item == null) return;
                switch (FilterMethod)
                {
                    case 5:
                        string filterDict = dictValue.Text;
                        filterDict = filterDict.Replace("*", "[*]");
                        filterDict = filterDict.Replace("%", "[%]");
                        filterDict = " like '%" + filterDict + "%'";
                        try
                        {
                            if (_MainTableDefine.FieldsName.Contains(item.FieldName))
                                filterTable = item.FieldName + filterDict;
                            else
                                filterTable = "D_" + item.FieldName + filterDict;
                        }
                        catch { }
                        filterDB = item.FullFieldName + filterDict;
                        break;
                    case 1:
                        string filterTxt = txtFilter1.Text;
                        filterTxt = filterTxt.Replace("*", "[*]");
                        filterTxt = filterTxt.Replace("%", "[%]");
                        filterTxt = " like '%" + filterTxt + "%'";
                        try
                        {
                            if (_MainTableDefine.FieldsName.Contains(item.FieldName))
                                filterTable = item.FieldName + filterTxt;
                            else
                                filterTable = "D_" + item.FieldName + filterTxt;
                        }
                        catch { }
                        filterDB = item.FullFieldName + filterTxt;
                        break;
                    case 2:
                        if (cmbValueList1.SelectedIndex > -1 && cmbOperator1.SelectedIndex > -1)
                        {
                            ValueListItem vli = cmbValueList1.Items[cmbValueList1.SelectedIndex] as ValueListItem;
                            if (_MainTableDefine.FieldsName.Contains(item.FieldName))
                                filterTable = item.FieldName + cmbOperator1.Text + vli.DataValue.ToString();
                            else
                                filterTable = "D_" + item.FieldName + cmbOperator1.Text + vli.DataValue.ToString();
                            filterDB = item.FullFieldName + cmbOperator1.Text + vli.DataValue.ToString();
                        }
                        if (cmbValueList2.SelectedIndex > -1 && cmbOperator2.SelectedIndex > -1)
                        {
                            if (filterDB.Length > 0)
                            {
                                filterDB += " and ";
                                filterTable += " and ";
                            }
                            ValueListItem vli = cmbValueList2.Items[cmbValueList2.SelectedIndex] as ValueListItem;
                            if (_MainTableDefine.FieldsName.Contains(item.FieldName))
                                filterTable = item.FieldName + cmbOperator2.Text + vli.DataValue.ToString();
                            else
                                filterTable = "D_" + item.FieldName + cmbOperator2.Text + vli.DataValue.ToString();
                            filterDB = item.FullFieldName + cmbOperator2.Text + vli.DataValue.ToString();
                        }
                        break;
                    case 3:
                        if (cmbOperator1.Text.Length > 0 && IsNumber(txtFilter1.Text))
                        {
                            if (_MainTableDefine.FieldsName.Contains(item.FieldName))
                                filterTable = item.FieldName + cmbOperator1.Text + txtFilter1.Text;
                            else
                                filterTable = "D_" + item.FieldName + cmbOperator1.Text + txtFilter1.Text;
                            filterDB = item.FullFieldName + cmbOperator1.Text + txtFilter1.Text;
                        }
                        if (cmbOperator2.Text.Length > 0 && IsNumber(txtFilter2.Text))
                        {
                            if (filterDB.Length > 0)
                            {
                                filterDB += " and ";
                                filterTable += " and ";
                            }
                            if (_MainTableDefine.FieldsName.Contains(item.FieldName))
                                filterTable += item.FieldName + cmbOperator2.Text + txtFilter2.Text;
                            else
                                filterTable += "D_" + item.FieldName + cmbOperator2.Text + txtFilter2.Text;
                            filterDB += item.FullFieldName + cmbOperator2.Text + txtFilter2.Text;
                        }
                        break;
                    case 4:
                        string opt1 = cmbOperator1.Text;
                        string opt2 = cmbOperator2.Text;
                        if (opt1.Length > 0 && datValue1.Value != null && datValue1.Value != DBNull.Value)
                        {
                            if (_MainTableDefine.FieldsName.Contains(item.FieldName))
                                filterTable = getDateSQL(item.FieldName, opt1, datValue1.DateTime);
                            else
                                filterTable = getDateSQL("D_" + item.FieldName, opt1, datValue1.DateTime);

                            filterDB = getDateSQL(item.FullFieldName, opt1, datValue1.DateTime);
                        }
                        if (opt2.Length > 0 && datValue2.Value != null && datValue2.Value != DBNull.Value)
                        {
                            if (filterDB.Length > 0)
                            {
                                filterDB += " and ";
                                filterTable += " and ";
                            }
                            if (_MainTableDefine.FieldsName.Contains(item.FieldName))
                                filterTable += getDateSQL(item.FieldName, opt2, datValue2.DateTime);
                            else
                                filterTable += getDateSQL("D_" + item.FieldName, opt2, datValue2.DateTime);

                            filterDB += getDateSQL(item.FullFieldName, opt2, datValue2.DateTime);
                        }
                        break;
                }
                _MainDataSet.Tables[0].DefaultView.RowFilter = filterTable;
                DBSelect.WhereFilter = filterDB;

                if (grid.Rows.Count < DBSelect.PageSize)
                    getNextPage(true);
            }
            else
            {
                _MainDataSet.Tables[0].DefaultView.RowFilter = "";
            }
            RefreshCheckStatus();
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

        private bool IsNumber(string s)
        {
            try
            {
                decimal.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual void grid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (_ToolDetailForm != null && _ToolDetailForm.OnlyDisplay)
                return;


            if (IsSelectMode)
            {
                if (_AllowDoubleClick)
                    toolSelect_Click(sender, EventArgs.Empty);
                else
                    toolView_Click(sender, EventArgs.Empty);
            }
            else
                toolView_Click(sender, EventArgs.Empty);
        }

        private void getNextPage(bool reget)
        {
            DataSet ds = null;
            if (reget)
                ds = DBSelect.RegetPageData();
            else
                ds = DBSelect.NextPageData();
            if (ds != null)
            {
                _MainDataSet.Merge(ds);
                RefreshCheckStatus();
            }
        }
        private void grid_BeforeRowRegionScroll(object sender, BeforeRowRegionScrollEventArgs e)
        {
            if (e.NewState.FirstRow.Index >= this.grid.Rows.Count - DBSelect.PageSize - 1)
            {
                getNextPage(false);
            }
        }

        //private void txtFilter_TextChanged(object sender, EventArgs e)
        //{
        //    filterTextChanged = true;
        //}

        private void txtFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (grid.ActiveRow != null && grid.ActiveRow.Index < grid.Rows.Count - 1)
                {
                    grid.ActiveRow = grid.Rows[grid.ActiveRow.Index + 1];
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (grid.ActiveRow != null && grid.ActiveRow.Index > 0)
                {
                    grid.ActiveRow = grid.Rows[grid.ActiveRow.Index - 1];
                }
            }
        }

        private void toolBestWidth_Click(object sender, EventArgs e)
        {
            this.grid.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFree;
            foreach (UltraGridColumn col in this.grid.DisplayLayout.Bands[0].Columns)
            {
                col.PerformAutoResize(Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);
            }
            this.grid.Refresh();
        }
        private void toolBestHeight_Click(object sender, EventArgs e)
        {
            this.grid.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFree;
            foreach (UltraGridRow row in this.grid.Rows)
            {
                row.PerformAutoSize();
            }
            this.grid.Refresh();
        }
        private void toolExport_Click(object sender, EventArgs e)
        {
            UltraGrid dataGrid = null;
            if (_ShowDetail)
                dataGrid = gridDetail;
            else
                dataGrid = grid;
            if (dataGrid != null)
            {
                if (dataGrid.Rows.Count == 0)
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

                    gridExcelExporter.CellExported += new Infragistics.Win.UltraWinGrid.ExcelExport.CellExportedEventHandler(gridExcelExporter_CellExported);
                    gridExcelExporter.Export(dataGrid, ws, row, column);

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

        private void toolCopy_Click(object sender, EventArgs e)
        {
            if (grid.ActiveRow == null)
                return;

            DataSet ds;
            Guid ID;
            Guid MainID = Guid.NewGuid();
            COMField IDField = new COMField();

            if (CSystem.Sys.Svr.LDI.GetFields(_MainTableDefine.OrinalTableName).FieldsName.Contains("ID"))
                IDField = CSystem.Sys.Svr.LDI.GetFields(_MainTableDefine.OrinalTableName)["ID"];



            //if ((_MainTableDefine.OrinalTableName.Equals("S_TableField")) || (_MainTableDefine.OrinalTableName.Equals("S_BaseInfo")))
            if (IDField.ValueType.Contains("String"))
            {
                string TableFieldName = (string)grid.ActiveRow.Cells["ID"].Value;
                ds = CSystem.Sys.Svr.cntMain.Select(MainTableDefine.QuerySQLWithClause(string.Format("{0}.ID='{1}'", _MainTableDefine.OrinalTableName, TableFieldName)), _MainTableDefine.OrinalTableName);
            }
            else
            {
                ID = (Guid)grid.ActiveRow.Cells["ID"].Value;
                ds = CSystem.Sys.Svr.cntMain.Select(MainTableDefine.QuerySQLWithClause(string.Format("{0}.ID='{1}'", _MainTableDefine.OrinalTableName, ID)), _MainTableDefine.OrinalTableName);
                for (int i = 0; i < _DetailTableDefine.Count; i++)
                    CSystem.Sys.Svr.cntMain.Select(_DetailTableDefine[i].QuerySQLWithClause(string.Format("{0}.MainID='{1}'", _DetailTableDefine[i].OrinalTableName, ID)), _DetailTableDefine[i].OrinalTableName, ds);
                ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["ID"] = MainID;
            }

            if (ds.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("Createdby"))
                ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["Createdby"] = CSystem.Sys.Svr.User;
            if (ds.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("CreateDate"))
                ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["CreateDate"] = CSystem.Sys.Svr.SystemTime;
            if (ds.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("CheckStatus"))
                ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["CheckStatus"] = 0;
            if (ds.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("CheckedBy"))
                ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["CheckedBy"] = DBNull.Value;
            if (ds.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("CheckDate"))
                ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["CheckDate"] = DBNull.Value;
            if (ds.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("Code"))
                ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["Code"] = DBNull.Value;
            if (ds.Tables[_MainTableDefine.OrinalTableName].Columns.Contains("Status"))
                ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["Status"] = 0;

            ds.Tables[_MainTableDefine.OrinalTableName].Rows[0].AcceptChanges();
            ds.Tables[_MainTableDefine.OrinalTableName].Rows[0].SetAdded();

            for (int i = 0; i < _DetailTableDefine.Count; i++)
            {
                toolDetailForm.BeforeCopyDetailTable(_DetailTableDefine[i].OrinalTableName);
                foreach (DataRow dr in ds.Tables[_DetailTableDefine[i].OrinalTableName].Rows)
                {
                    ID = Guid.NewGuid();
                    toolDetailForm.BeforeSetRowNewIDWhenCopy(ID, (Guid)dr["ID"], _DetailTableDefine[i].OrinalTableName);
                    dr["ID"] = ID;
                    dr["MainID"] = MainID;
                    dr.AcceptChanges();
                    dr.SetAdded();
                }
            }
            toolDetailForm.AfterSetNewRowIDWhenCopy(ds);

            frmDetail frm = GetDetailForm(true);
            if (frm != null)
            {
                frm.DefaultValue = _DefaultValue;
                frm.CopyLastValue = _CopyLastValue;
                frm.toolDetailForm = _ToolDetailForm;
                //进行一些复制的后处理
                frm.toolDetailForm.CopyDetail(ds);
                //frm.SelectForm = _SelectForm;
                //frm.DetailFormHandler = _DetailFormHandler;
                frm.Changed += new DataTableEventHandler(frm_Changed);
                frm.PageDown += new DataTableEventHandler(frm_PageDown);
                frm.PageUp += new DataTableEventHandler(frm_PageUp);
                bool bShowData = frm.ShowData(_MainTableDefine, _DetailTableDefine, new DataSetEventArgs(ds), true, this.MdiParent);

                if (bShowData && this.MdiParent != null)
                {
                    frm.FormClosed += new FormClosedEventHandler(frm_FormClosed);
                    SetEnabled(false);
                }
            }

        }

        private void toolUndelete_Click(object sender, EventArgs e)
        {
            UltraGridRow row = grid.ActiveRow;
            if (row != null && grid.DisplayLayout.Bands[0].Columns.Exists("Disable"))
            {
                if (Msg.Question("您确定要恢复所删除数据吗?") == DialogResult.Yes)
                {
                    DataRow[] drs = _MainDataSet.Tables[_MainTableDefine.OrinalTableName].Select("ID = '" + row.Cells["ID"].Value + "'");
                    DataSet ds = _MainDataSet.Clone();
                    drs[0].AcceptChanges();
                    ds.Tables[_MainTableDefine.OrinalTableName].ImportRow(drs[0]);
                    ds.Tables[_MainTableDefine.OrinalTableName].Rows[0].AcceptChanges();
                    ds.Tables[_MainTableDefine.OrinalTableName].Rows[0]["Disable"] = 0;
                    Guid id = (Guid)drs[0]["ID"];

                    cntMain.Update(ds.Tables[_MainTableDefine.OrinalTableName]);

                    row.Cells["Disable"].Value = 0;
                }
            }
        }

        private void toolStripHistory_CheckedChanged(object sender, EventArgs e)
        {
            _IsHistory = toolStripHistory.Checked;
            DBSelect.IsHistory = _IsHistory;
            toolRefrash_Click(null, EventArgs.Empty);
            if (_IsHistory)
            {
                toolCheck.Enabled = false;
                toolUnCheck.Enabled = false;
                toolEdit.Enabled = false;
                toolNew.Enabled = false;
                toolDelete.Enabled = false;
            }
            else
            {
                SetToolBarByRole();
            }

        }
        public static frmBrowser GetForm(string s, string where)
        {
            string[] s1 = s.Split(':');
            return GetForm(s1, where);
        }
        public static frmBrowser GetForm(string[] s, string where)
        {
            string tableName = null;
            string menuName = null;
            string actionName = null;
            string parameters = null;
            for (int i = 1; i < s.Length; i++)
            {
                if (s[i].IndexOf('=') >= 0)
                {
                    string[] s2 = s[i].Split('=');
                    switch (s2[0])
                    {
                        case "Action":
                            if (s2.Length > 1)
                                actionName = s2[1];
                            break;
                        case "Menu":
                            if (s2.Length > 1)
                                menuName = s2[1];
                            break;
                        case "Para":
                            if (s2.Length > 1)
                                parameters = s2[1];
                            break;
                        case "Where":
                            if (s2.Length > 1)
                            {
                                if (where != null && where.Length > 0)
                                    where += " and ";
                                where += s2[1];
                                for (int j = 2; j < s2.Length; j++)
                                    where += "=" + s2[j];
                            }
                            break;
                    }
                }
                else
                {
                    tableName = s[i];
                    //break;
                }
            }
            return GetForm(tableName, menuName, actionName, parameters, where);
        }
        public static frmBrowser GetForm(string tableName, string menuName, string actionName, string parameters, string where)
        {
            frmBrowser frm = null;
            if (tableName != null && tableName.Length > 0)
            {
                List<COMFields> detailTable = CSystem.Sys.Svr.Properties.DetailTableDefineList(tableName);
                COMFields mainTable = CSystem.Sys.Svr.LDI.GetFields(tableName);
                frm = new frmBrowser(mainTable, detailTable, where, enuShowStatus.None);
                frm.Where = where;
            }
            else if (menuName != null && menuName.Length > 0)
            {
                frm = (frmBrowser)CSystem.Sys.Svr.Right.GetForm(menuName);
                frm.Where = where;
            }
            else if (actionName != null && actionName.Length > 0)
            {
                frm = (frmBrowser)CSystem.Sys.Svr.Right.GetForm(actionName, parameters);
                frm.Where = where;
            }
            return frm;
        }

        private void grid_AfterRowActivate(object sender, EventArgs e)
        {
            SetToolBarByRole();
        }

        private void toolImport_Click(object sender, EventArgs e)
        {
            UltraGrid dataGrid = null;
            if (_ShowDetail)
                dataGrid = gridDetail;
            else
                dataGrid = grid;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Filter = "Excel文件(*.xlsx)|*.xlsx|(*.xls)|*.xls";
            ofd.DefaultExt = "xlsx";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string path = ofd.FileName;
                    //For Office Excel 2010  please take a look to the followng link  http://social.msdn.microsoft.com/Forums/en-US/exceldev/thread/0f03c2de-3ee2-475f-b6a2-f4efb97de302/#ae1e6748-297d-4c6e-8f1e-8108f438e62e
                    string excelConnectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 8.0", path);

                    // Create Connection to Excel Workbook
                    using (OleDbConnection connection =
                                 new OleDbConnection(excelConnectionString))
                    {
                        OleDbCommand command = new OleDbCommand
                                ("Select * FROM [Sheet1$]", connection);

                        connection.Open();

                        // Create DbDataReader to Data Worksheet
                        using (DbDataReader dr = command.ExecuteReader())
                        {

                            // SQL Server Connection String
                            //string sqlConnectionString = @"Data Source=.\sqlexpress;Initial Catalog=ExcelDB;Integrated Security=True";
                            string sqlConnectionString = cntMain.ConnectionString;
                            String tableName = this._MainTableDefine.GetTableName(false);

                            //
                            cntMain.Excute(string.Format("Delete from T_{0}", tableName));

                            // Bulk Copy to SQL Server
                            using (SqlBulkCopy bulkCopy =
                                       new SqlBulkCopy(sqlConnectionString))
                            {
                                bulkCopy.DestinationTableName = string.Format("T_{0}", tableName);
                                bulkCopy.WriteToServer(dr); 
                                Msg.Information(string.Format("成功导入{0}数据到系统临时表{1}", this._MainTableDefine.Property.Title, bulkCopy.DestinationTableName));
                            }
                            string SPName = string.Format("SP_{0}",tableName);

                            CSystem.Sys.Svr.cntMain.StoreProcedure(SPName,tableName);

                        }
                    }
                }
                catch (Exception ex)
                {
                    Msg.Information(ex.Message);
                }
            }
            else
            {
 
            }
            //
        }

        private void toolFinished_Click(object sender, EventArgs e)
        {
            if (Msg.Question("您确定此单据已完成吗?") != DialogResult.Yes)
                return;
            if (this.grid.Selected.Rows.Count == 0 && this.grid.ActiveRow != null)
            {
                this.grid.ActiveRow.Selected = true;
            }
            Guid MainID = CSystem.Sys.Svr.NullID;
            string PrimaryKey = null;
            if (_MainDataSet.Tables[0].PrimaryKey.Length > 0)
            {
                PrimaryKey = _MainDataSet.Tables[0].PrimaryKey[0].ColumnName;
                MainID = (Guid)this.grid.Selected.Rows[0].Cells[PrimaryKey].Value;
            }
            using (SqlConnection conn = new SqlConnection(cntMain.ConnectionString))
            {
                SqlTransaction sqlTran = null;
                try
                {
                    conn.Open();
                    sqlTran = conn.BeginTransaction();
                    if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                    {
                        SortedList<Guid, Guid> list = new SortedList<Guid, Guid>();
                        foreach (UltraGridRow row in this.grid.Selected.Rows)
                        {
                            Guid id = (Guid)row.Cells["ID"].Value;
                            if (row.Cells.Exists("Finished"))
                            {
                                if ((int)row.Cells["Finished"].Value == 0)
                                {
                                    if (!list.ContainsKey(id))
                                    {
                                        list.Add(id, id);
                                        cntMain.Excute(string.Format("update {0} set Finished=Finished where  id='{1}'", _MainTableDefine.OrinalTableName, id), conn, sqlTran);
                                    }
                                }
                            }
                        }

                        foreach (Guid id in list.Keys)
                        {
                            DataSet ds = cntMain.Select(string.Format("Select * from {0} where  id='{1}' and Finished=0", _MainTableDefine.OrinalTableName, id), _MainTableDefine.OrinalTableName, conn, sqlTran);
                            if ((int)ds.Tables[0].Rows.Count == 1)
                            {
                                DataTable dt = ds.Tables[0];
                                DataRow dr = ds.Tables[0].Rows[0];
                                if (!Check(id, conn, sqlTran, dr))
                                {
                                    sqlTran.Rollback();
                                    return;
                                }
                                if (dt.Columns.Contains("Finished") && (int)dr["Finished"] == 0)
                                {
                                    dr["Finished"] = 1;
                                }
                                cntMain.Update(ds.Tables[0], conn, sqlTran);
                            }
                        }
                    }
                    else
                    {
                        DataTable dt = _MainDataSet.Tables[0].Clone();
                        foreach (UltraGridRow row in this.grid.Selected.Rows)
                        {
                            Guid id = (Guid)row.Cells["ID"].Value;
                            if (row.Cells.Exists("Finished"))
                            {
                                cntMain.Excute(string.Format("update {0} set Finished= Finished where  id='{1}'", _MainTableDefine.OrinalTableName, id), conn, sqlTran);
                                DataSet ds = cntMain.Select(_MainTableDefine.QuerySQLWithClause(string.Format(" {1}.ID='{0}' and {1}.Finished=0", id, _MainTableDefine.OrinalTableName)), conn, sqlTran);
                                if ((int)ds.Tables[0].Rows.Count == 1)
                                {
                                    dt.ImportRow(ds.Tables[0].Rows[0]);
                                    DataRow dr = dt.Rows[dt.Rows.Count - 1];
                                    if (!Check(id, conn, sqlTran, dr))
                                    {
                                        sqlTran.Rollback();
                                        return;
                                    }
                                    if (dt.Columns.Contains("Finished"))
                                        dr["Finished"] = 1;
                                }
                            }
                        }

                        if (dt.Rows.Count > 0)
                        {
                            cntMain.Update(dt, conn, sqlTran);
                            _MainDataSet.Merge(dt);
                            RefreshCheckStatus();
                        }
                    }
                    sqlTran.Commit();
                }
                catch (Exception ex)
                {
                    Msg.Warning(ex.ToString());
                    sqlTran.Rollback();
                }
                finally
                {
                    conn.Close();
                }
                if ((_ShowStatus & enuShowStatus.ShowDetail) == enuShowStatus.ShowDetail)
                {
                    toolRefrash_Click(this, EventArgs.Empty);
                    RefreshCheckStatus();
                }
            }
            if (PrimaryKey != null)
            {
                foreach (UltraGridRow row in this.grid.Rows)
                {
                    if ((Guid)row.Cells[PrimaryKey].Value == MainID)
                    {
                        row.Activated = true;
                        return;
                    }
                }
            }
        }

    //end of Class
    }
}