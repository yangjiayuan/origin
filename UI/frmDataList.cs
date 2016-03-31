using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Base;
using Infragistics.Excel;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win;

namespace UI
{
    public partial class frmDataList : Form
    {
        private COMFields _TableDefine;
        private DataSet DataList;
        private System.Windows.Forms.Button butFilter;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.ComboBox cmbFilter;
        private System.Windows.Forms.ComboBox cmbValueList;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor dictValue;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor datValue;
        private System.Windows.Forms.ComboBox cmbOperator;
        private int FilterMethod = 1;


        public delegate bool BeforeExportEventHandler(Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter exporter, UltraGrid grid, Worksheet sheet, ref int startRow, ref int startColumn);
        public event BeforeExportEventHandler BeforeExport;
        public delegate void AfterExportEventHandler(UltraGrid grid, Worksheet sheet);
        public event AfterExportEventHandler AfterExport;
        public event BeforeSelectFormEventHandler BeforeSelectForm;
        public event AfterSelectFormEventHandler AfterSelectForm;

        public frmDataList()
        {
            InitializeComponent();
        }
        public frmDataList(COMFields tableDefine):this()
        {
            _TableDefine = tableDefine;
            toolSave.Enabled = false;
            toolCopyLine.Enabled = false;
            
            DataList = CSystem.Sys.Svr.cntMain.Select(tableDefine.QuerySQL, tableDefine.OrinalTableName);
            CCreateGrid createGrid = new CCreateGrid(tableDefine, grid, DataList.Tables[0].DefaultView, COMField.Enum_Visible.VisibleAll,null);
            createGrid.AfterSelectForm += new AfterSelectFormEventHandler(createGrid_AfterSelectForm);
            createGrid.BeforeSelectForm += new BeforeSelectFormEventHandler(createGrid_BeforeSelectForm);

            grid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            grid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;

            InitFilterControl(panelFilter);
            SetFilterMethod(1);
            load();
        }
        private void load()
        {
            //根据相关对应设置列头属性
            cmbFilter.Items.Clear();
            cmbFilter.Items.Add(new DataItem("All", "<全部>"));
            foreach (Base.COMField f in _TableDefine.Fields)
            {
                if ((f.Visible & COMField.Enum_Visible.VisibleInBrower) == COMField.Enum_Visible.VisibleInBrower)
                {
                    cmbFilter.Items.Add(f);
                }
            }
        }
        void InitFilterControl(Panel p)
        {
            // 
            // butFilter
            // 
            this.butFilter = new Button();
            this.butFilter.Location = new System.Drawing.Point(320, 14);
            this.butFilter.Name = "butFilter";
            this.butFilter.Size = new System.Drawing.Size(60, 25);
            this.butFilter.TabIndex = 12;
            this.butFilter.Text = "过滤(&F)";
            this.butFilter.UseVisualStyleBackColor = true;
            // 
            // txtFilter
            // 
            this.txtFilter = new TextBox();
            this.txtFilter.Location = new System.Drawing.Point(147, 14);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(154, 21);
            this.txtFilter.TabIndex = 11;
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
            this.cmbFilter.TabIndex = 9;
            this.cmbFilter.TabStop = false;
            // 
            // cmbValueList
            // 
            this.cmbValueList = new ComboBox();
            this.cmbValueList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbValueList.FormattingEnabled = true;
            this.cmbValueList.Location = new System.Drawing.Point(148, 14);
            this.cmbValueList.Name = "cmbValueList";
            this.cmbValueList.Size = new System.Drawing.Size(153, 20);
            this.cmbValueList.TabIndex = 13;
            // 
            // cmbOperator
            // 
            this.cmbOperator = new ComboBox();
            this.cmbOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOperator.FormattingEnabled = true;
            this.cmbOperator.Items.AddRange(new object[] {
            "=",
            ">",
            "<",
            ">=",
            "<=",
            "<>"});
            this.cmbOperator.Location = new System.Drawing.Point(147, 14);
            this.cmbOperator.Name = "cmbOperator";
            this.cmbOperator.Size = new System.Drawing.Size(43, 20);
            this.cmbOperator.TabIndex = 14;
            // 
            // datValue
            // 
            this.datValue = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.datValue.DateTime = System.DateTime.Now.Date;
            this.datValue.Location = new System.Drawing.Point(193, 14);
            this.datValue.Name = "datValue";
            this.datValue.Size = new System.Drawing.Size(108, 21);
            this.datValue.TabIndex = 15;
            this.datValue.Value = System.DateTime.Now.Date;

            this.dictValue = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.dictValue.Name = "dictValue";
            this.dictValue.Location = new System.Drawing.Point(147, 14);
            this.dictValue.Size = new System.Drawing.Size(154, 21);
            this.dictValue.TabIndex = 16;
            EditorButton editButton = new EditorButton();
            dictValue.ButtonsRight.Add(editButton);
            dictValue.EditorButtonClick += new EditorButtonEventHandler(Dict_EditorButtonClick);

            p.Controls.Add(this.datValue);
            p.Controls.Add(this.cmbOperator);
            p.Controls.Add(this.butFilter);

            p.Controls.Add(this.txtFilter);
            p.Controls.Add(lab);
            p.Controls.Add(this.cmbFilter);
            p.Controls.Add(this.cmbValueList);
            p.Controls.Add(this.dictValue);

            this.cmbFilter.SelectedIndexChanged += new EventHandler(cmbFilter_SelectedIndexChanged);
            this.butFilter.Click += new EventHandler(butFilter_Click);

        }
        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilter.SelectionStart = 0;
            txtFilter.SelectionLength = txtFilter.Text.Length;
            if (cmbFilter.SelectedIndex > -1 && cmbValueList != null)
            {
                COMField item = cmbFilter.Items[cmbFilter.SelectedIndex] as COMField;
                if (item == null)
                {
                    DataList.Tables[0].DefaultView.RowFilter = "";
                }
                else if (grid.DisplayLayout.Bands[0].Columns.Exists(item.FieldName))
                {
                    UltraGridColumn col = grid.DisplayLayout.Bands[0].Columns[item.FieldName];
                    if (col.ValueList != null)
                    {
                        ValueList vl = (ValueList)col.ValueList;
                        cmbValueList.Items.Clear();
                        foreach (Infragistics.Win.ValueListItem i in vl.ValueListItems)
                            cmbValueList.Items.Add(i);
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
            if (cmbValueList != null) cmbValueList.Visible = false;
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
                    txtFilter.Visible = true;
                    txtFilter.Width = cmbValueList.Width;
                    txtFilter.Left = cmbValueList.Left;
                    cmbValueList.Visible = false;
                    cmbOperator.Visible = false;
                    datValue.Visible = false;
                    dictValue.Visible = false;
                    break;
                case 2:
                    txtFilter.Visible = false;
                    cmbValueList.Visible = true;
                    cmbOperator.Visible = false; ;
                    datValue.Visible = false;
                    dictValue.Visible = false;
                    break;
                case 3:
                    txtFilter.Visible = true;
                    cmbOperator.Visible = true;
                    cmbOperator.SelectedIndex = 0;
                    txtFilter.Width = datValue.Width;
                    txtFilter.Left = datValue.Left;
                    datValue.Visible = false;
                    cmbValueList.Visible = false;
                    dictValue.Visible = false;
                    break;
                case 4:
                    txtFilter.Visible = false;
                    cmbOperator.Visible = true;
                    cmbOperator.SelectedIndex = 0;
                    datValue.Visible = true;
                    cmbValueList.Visible = false;
                    dictValue.Visible = false;
                    break;
                case 5:
                    txtFilter.Visible = false;
                    cmbOperator.Visible = false;
                    //cmbOperator.SelectedIndex = 0;
                    datValue.Visible = false;
                    cmbValueList.Visible = false;
                    dictValue.Visible = true;
                    break;
            }
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
                            DataList.Tables[0].DefaultView.RowFilter = item.FieldName + filterDict;
                        }
                        catch { }
                        //DBSelect.WhereFilter = item.FullFieldName + filterDict;
                        break;
                    case 1:
                        string filter = txtFilter.Text;
                        filter = filter.Replace("*", "[*]");
                        filter = filter.Replace("%", "[%]");
                        filter = " like '%" + filter + "%'";
                        try
                        {
                            DataList.Tables[0].DefaultView.RowFilter = item.FieldName + filter;
                        }
                        catch { }
                        //DBSelect.WhereFilter = item.FullFieldName + filter;
                        break;
                    case 2:
                        if (cmbValueList.SelectedIndex > -1)
                        {
                            ValueListItem vli = cmbValueList.Items[cmbValueList.SelectedIndex] as ValueListItem;
                            DataList.Tables[0].DefaultView.RowFilter = item.FieldName + " = '" + vli.DataValue.ToString() + "'";
                            //DBSelect.WhereFilter = item.FullFieldName + " = '" + vli.DataValue.ToString() + "'";
                        }
                        else
                        {
                            DataList.Tables[0].DefaultView.RowFilter = "";
                            //DBSelect.WhereFilter = "";
                        }
                        break;
                    case 3:
                        decimal d = 0;
                        try
                        {
                            d = decimal.Parse(txtFilter.Text);
                        }
                        catch { }
                        string opt3 = cmbOperator.Text;
                        if (opt3.Length > 0 && d != 0)
                        {
                            DataList.Tables[0].DefaultView.RowFilter = item.FieldName + opt3 + d;
                            //DBSelect.WhereFilter = item.FullFieldName + opt3 + d;
                        }
                        break;
                    case 4:
                        DateTime dat = DateTime.MinValue;
                        try
                        {
                            dat = (DateTime)datValue.Value;
                        }
                        catch { }
                        string opt = cmbOperator.Text;
                        if (opt.Length > 0 && dat != DateTime.MinValue)
                        {
                            if (opt == "=")
                            {
                                DataList.Tables[0].DefaultView.RowFilter = item.FieldName + ">='" + dat + "' and " + item.FieldName + "<'" + dat.AddDays(1) + "'";
                                //DBSelect.WhereFilter = item.FullFieldName + ">='" + dat + "' and " + item.FullFieldName + "<'" + dat.AddDays(1) + "'";
                            }
                            else if (opt == "<>")
                            {
                                DataList.Tables[0].DefaultView.RowFilter = item.FieldName + ">'" + dat.AddDays(1) + "' and " + item.FieldName + "<'" + dat + "'";
                                //DBSelect.WhereFilter = item.FullFieldName + ">'" + dat.AddDays(1) + "' and " + item.FullFieldName + "<'" + dat + "'";
                            }
                            else
                            {
                                if (opt == "<=")
                                {
                                    dat = dat.AddDays(1);
                                    opt = "<";
                                }
                                DataList.Tables[0].DefaultView.RowFilter = item.FieldName + opt + "'" + dat + "'";
                                //DBSelect.WhereFilter = item.FullFieldName + opt + "'" + dat + "'";
                            }
                        }
                        break;
                }
                //if (grid.Rows.Count < DBSelect.PageSize)
                //    getNextPage(true);
            }
            else
            {
                DataList.Tables[0].DefaultView.RowFilter = "";
            }
            //RefrashCheckStatus();
        }

        void createGrid_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            if (BeforeSelectForm != null)
                BeforeSelectForm(sender, e);
        }

        void createGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {
            if (AfterSelectForm != null)
                AfterSelectForm(sender, e);
        }
        private void toolEdit_Click(object sender, EventArgs e)
        {
            toolSave.Enabled = true;
            toolEdit.Enabled = false;
            toolCopyLine.Enabled = true;
            grid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            grid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            grid.PerformAction(UltraGridAction.ExitEditMode);
            this.BindingContext[DataList, _TableDefine.OrinalTableName].EndCurrentEdit();
            CSystem.Sys.Svr.cntMain.Update(DataList.Tables[0]);
            DataList.AcceptChanges();
            toolSave.Enabled = false;
            toolEdit.Enabled = true;
            toolCopyLine.Enabled = false;
            grid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            grid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
        }

        private void toolExport_Click(object sender, EventArgs e)
        {
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


        private void toolClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolCopyLine_Click(object sender, EventArgs e)
        {
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
}