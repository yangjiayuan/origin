using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Base;
using Infragistics.Win;
using System.Drawing;

namespace UI
{
    public class CCreateDetailForm
    {
        public class FieldControlEventArgs
        {
            private COMField field;
            private DataTable dataTable;

            public bool NewControl = false;
            public Control FieldControl = null;
            public bool HasCaption = true;
            public string ValueBindName;

            public FieldControlEventArgs(COMField f,DataTable d)
            {
                field = f;
                dataTable = d;
            }
            public COMField Field
            {
                get { return field; }
            }
            public DataTable Data
            {
                get { return dataTable; }
            }
        }
        private const int TOP_VALUE = 10;
        private const int LEFT_VALUE = 10;
        private int _Caption_Width = 70;
        private int _Data_Width = 110;
        private const int SPACE_WITH = 20;
        private const int CONTROL_HEIGHT = 25;
        int maxColumnHeight = 21;

        //private string _TableName;
        private COMFields _Fields;
        private Control _ContainerControl;
        private DataTable _DataTable;
        private SortedList<string,Control> _ControlMap;
        private SortedList<string, Label> _LabelMap;
        private ToolDetailForm _DetailForm;

        public event BeforeSelectFormEventHandler BeforeSelectForm; 
        public event AfterSelectFormEventHandler AfterSelectForm;

        public delegate void NewControlEventHandler(object sender, FieldControlEventArgs e);
        public event NewControlEventHandler BeforeNewControl;
        public event NewControlEventHandler AfterNewControl;

        public int CaptionWidth
        {
            set { _Caption_Width = value; }
            get{return _Caption_Width;}
        }
        public int DataWidth
        {
            set { _Data_Width = value; }
            get { return _Data_Width; }
        }
        public int GetColumn(int width)
        {
            return (width - LEFT_VALUE) / (_Caption_Width + _Data_Width + SPACE_WITH);
        }
        private GroupBox GetGroupBox(COMField f)
        {
            if (f.GroupName != null && f.GroupName.Length > 0)
            {
                foreach (Control ctl in _ContainerControl.Controls)
                {
                    if (ctl.GetType() == typeof(GroupBox))
                    {
                        if (ctl.Text == f.GroupName)
                        {
                            return (GroupBox)ctl;
                        }
                    }
                }
                GroupBox grp = new GroupBox();
                grp.Text=f.GroupName;
                _ContainerControl.Controls.Add(grp);
                return grp;
            }
            return null;
        }
        public int Refrash()
        {
            int x = LEFT_VALUE;
            int y = -10;
            int y2 = -10;
            int columns = (_ContainerControl.Width - LEFT_VALUE) / (_Caption_Width + _Data_Width + SPACE_WITH);
            int columnIndex = 1;
            GroupBox grp = null;
            foreach (COMField f in _Fields.Fields)
            {
                if ((f.Visible & COMField.Enum_Visible.VisibleInDetail) != COMField.Enum_Visible.VisibleInDetail)
                {
                    if (_LabelMap.ContainsKey(f.FieldName))
                    {
                        if (f.GroupName != null && f.GroupName.Length > 0)
                            GetGroupBox(f).Controls.Remove(_LabelMap[f.FieldName]);
                        else
                            _ContainerControl.Controls.Remove(_LabelMap[f.FieldName]);
                    }
                    if (_ControlMap.ContainsKey(f.FieldName))
                    {
                        if (f.GroupName != null && f.GroupName.Length > 0)
                            GetGroupBox(f).Controls.Remove(_ControlMap[f.FieldName]);
                        else
                            _ContainerControl.Controls.Remove(_ControlMap[f.FieldName]);
                    }
                    continue;
                }

                FieldControlEventArgs fieldControl = new FieldControlEventArgs(f, _DataTable);
                if (_ControlMap.ContainsKey(f.FieldName))
                {
                    fieldControl.FieldControl = _ControlMap[f.FieldName];
                }
                else
                {
                    if (BeforeNewControl != null)
                    {
                        BeforeNewControl(this, fieldControl);
                    }
                    if (_DetailForm != null)
                        _DetailForm.BeforeNewControl(this, fieldControl);
                    if (!fieldControl.NewControl)
                    {
                        getControl(fieldControl);
                    }
                }

                if (f.GroupName != null && f.GroupName.Length > 0 && (grp == null || grp.Text != f.GroupName))
                {
                    //处理分组
                    //if (columnIndex != 1)
                    //{
                    if (grp != null)
                    {
                        grp.Height += maxColumnHeight + 4;
                    }
                    columnIndex = 1;
                    x = LEFT_VALUE;
                    y += maxColumnHeight + 4;
                    //}
                    grp = GetGroupBox(f);
                    grp.Width = _ContainerControl.Width - LEFT_VALUE;
                    grp.Left = 0;
                    grp.Top = y;
                    grp.Height = CONTROL_HEIGHT ;
                    y = grp.Top + CONTROL_HEIGHT + TOP_VALUE;
                    y2 = TOP_VALUE * 2;
                }
                else
                {
                    if (grp != null && (f.GroupName == null || f.GroupName.Length == 0))
                    {
                        grp.Height += maxColumnHeight + 4;
                        grp = null;
                        if (columnIndex != 1)
                        {
                            columnIndex = 1;
                            x = LEFT_VALUE;
                            y += CONTROL_HEIGHT * f.Height;
                        }
                    }
                    //新行
                    //第个位置
                    //处理占位多个控件位置的控件,如果放在行末,要判断宽度是否超过
                    if (f.NewLine || columnIndex == 1 || (f.Width > 1 && (columnIndex + f.Width - 1) > columns))
                    {
                        //处理换行的问题
                        columnIndex = 1;
                        x = LEFT_VALUE;
                        y += maxColumnHeight + 4;
                        y2 += maxColumnHeight + 4;
                        if (grp != null)
                            grp.Height += maxColumnHeight + 4;
                        maxColumnHeight = 21;
                    }
                }

                //if (f.Width > 1 && (columnIndex + f.Width - 1) > columns)
                //{
                //    columnIndex = 1;
                //    x = LEFT_VALUE;
                //    y += CONTROL_HEIGHT;
                //    y2 += CONTROL_HEIGHT;
                //    if (grp!=null)
                //        grp.Height += CONTROL_HEIGHT;
                //}

                Label l = null;
                if (fieldControl.HasCaption)
                {
                    if (_LabelMap.ContainsKey(f.FieldName))
                        l = _LabelMap[f.FieldName];
                    else
                        l = new Label();
                    l.Left = x;
                    l.Top = y + 4;
                    l.Width = _Caption_Width;
                    l.Text = f.FieldTitle;
                    l.AutoSize = true;
                    l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    x += _Caption_Width;

                    if (grp == null)
                    {
                        if (!_LabelMap.ContainsKey(f.FieldName))
                            _ContainerControl.Controls.Add(l);
                    }
                    else
                    {
                        l.Top = y2 + 4;
                        if (!_LabelMap.ContainsKey(f.FieldName))
                        grp.Controls.Add(l);
                    }

                    if (!_LabelMap.ContainsKey(f.FieldName))
                        _LabelMap.Add(f.FieldName, l);
                    if (f.Mandatory)
                    {
                        l.ForeColor = System.Drawing.Color.Blue;
                        l.Font = new Font(l.Font, FontStyle.Bold);
                    }
                }

                Control ctl = fieldControl.FieldControl;
                ctl.Left = x;
                ctl.Top = y;

                //将Comfield放到控件的Tag上
                ctl.Tag = f;
                if (f.Mandatory && ctl.GetType() == typeof(CheckBox))
                {
                    ctl.ForeColor = System.Drawing.Color.Blue;
                    ctl.Font = new Font(ctl.Font, FontStyle.Bold);
                }

                if (fieldControl.HasCaption)
                    ctl.Width = _Data_Width;
                else
                    ctl.Width = _Data_Width + _Caption_Width;
                //当占多个控件位置时
                if (f.Width > 1)
                {
                    ctl.Width = ctl.Width + (_Data_Width + _Caption_Width + SPACE_WITH) * (f.Width - 1);
                    columnIndex += (f.Width - 1);
                    x += (_Data_Width + _Caption_Width + SPACE_WITH) * (f.Width - 1);
                }
                if (f.Height > 1)
                {
                    if (ctl.GetType() == typeof(TextBox))
                        ((TextBox)ctl).Multiline = true;
                    int height = ctl.Height * f.Height;
                    ctl.Height = height;
                    if (maxColumnHeight < height)
                        maxColumnHeight = height;
                }
                SetEnabled(ctl, f.Enable);
                if (grp == null)
                {
                    if (!_ControlMap.ContainsKey(f.FieldName))
                        _ContainerControl.Controls.Add(ctl);
                }
                else
                {
                    ctl.Top = y2;
                    if (!_ControlMap.ContainsKey(f.FieldName))
                        grp.Controls.Add(ctl);
                }

                if (l != null)
                    l.SendToBack();

                if (columnIndex == columns)
                {
                    columnIndex = 1;
                    //x = LEFT_VALUE;
                    //y += CONTROL_HEIGHT;
                    //y2 += CONTROL_HEIGHT;
                    //if (grp != null)
                    //    grp.Height += CONTROL_HEIGHT;
                }
                else
                {
                    columnIndex++;
                    if (fieldControl.HasCaption)
                        x += (_Data_Width + SPACE_WITH);
                    else
                        x += (_Data_Width + _Caption_Width + SPACE_WITH);
                }
                if (!_ControlMap.ContainsKey(f.FieldName))
                {
                    ctl.DataBindings.Add(fieldControl.ValueBindName, _DataTable, f.FieldName, false, DataSourceUpdateMode.OnPropertyChanged);


                    if (AfterNewControl != null)
                        AfterNewControl(this, fieldControl);
                    if (_DetailForm != null)
                        _DetailForm.NewControl(f, ctl);

                    _ControlMap.Add(f.FieldName, ctl);
                }
            }
            if (grp != null)
                grp.Height += maxColumnHeight + 4;
            return y + CONTROL_HEIGHT;
        }
        public int CreateDetail(COMFields fields, Control control, DataTable dataTable,ToolDetailForm detailForm)
        {
            //_TableName = tableName;
            _DataTable = dataTable;
            _Fields = fields;
            _DetailForm = detailForm;
            _ContainerControl = control;
            _ControlMap = new SortedList<string, Control>();
            _LabelMap = new SortedList<string, Label>();
            if (_DetailForm != null)
                _DetailForm.SetControlMap(_ControlMap);
            int x = LEFT_VALUE;
            int y = -10;
            int y2 = -10;
            int columns = (control.Width-LEFT_VALUE) / (_Caption_Width + _Data_Width + SPACE_WITH);
            int columnIndex = 1;
            int maxColumnHeight = 21;
            GroupBox grp = null;
            foreach(COMField f in _Fields.Fields)
            {
                if ((f.Visible& COMField.Enum_Visible.VisibleInDetail) != COMField.Enum_Visible.VisibleInDetail) continue;
                FieldControlEventArgs fieldControl = new FieldControlEventArgs(f,dataTable);
                if (BeforeNewControl != null)
                {
                    BeforeNewControl(this, fieldControl);
                }
                if (_DetailForm != null)
                    _DetailForm.BeforeNewControl(this, fieldControl);
                if (!fieldControl.NewControl)
                {
                    getControl(fieldControl);
                }


                if (f.GroupName != null && f.GroupName.Length > 0 && (grp == null || grp.Text != f.GroupName))
                {
                    //处理分组
                    //if (columnIndex != 1)
                    //{
                    if (grp != null)
                    {
                        grp.Height += maxColumnHeight + 4;
                    }
                        columnIndex = 1;
                        x = LEFT_VALUE;
                        y += maxColumnHeight+4;
                        maxColumnHeight = 21;
                    //}
                    grp = new GroupBox();
                    grp.Text = f.GroupName;
                    grp.Width = control.Width - LEFT_VALUE;
                    grp.Left = 0;
                    grp.Top = y;
                    grp.Height = CONTROL_HEIGHT;
                    y = grp.Top + CONTROL_HEIGHT + TOP_VALUE;
                    y2 = TOP_VALUE*2;
                    control.Controls.Add(grp);
                }
                else
                {
                    if (grp != null && (f.GroupName == null || f.GroupName.Length == 0))
                    {
                        grp.Height += maxColumnHeight + 4;
                        grp = null;
                        if (columnIndex != 1)
                        {
                            columnIndex = 1;
                            //x = LEFT_VALUE;
                            //y += CONTROL_HEIGHT;
                        }
                    }
                    //新行
                    //第个位置
                    //处理占位多个控件位置的控件,如果放在行末,要判断宽度是否超过
                    if (f.NewLine || columnIndex == 1||(f.Width > 1 && (columnIndex + f.Width - 1) > columns))
                    {
                        //处理换行的问题
                        columnIndex = 1;
                        x = LEFT_VALUE;
                        y +=maxColumnHeight+4 ;
                        y2 += maxColumnHeight+4;
                        if (grp != null)
                            grp.Height += maxColumnHeight + 4;
                        maxColumnHeight = 21;
                    }
                }
                
                //if (f.Width > 1 && (columnIndex + f.Width - 1) > columns)
                //{
                //    columnIndex = 1;
                //    x = LEFT_VALUE;
                //    y += CONTROL_HEIGHT;
                //    y2 += CONTROL_HEIGHT;
                //    if (grp!=null)
                //        grp.Height += CONTROL_HEIGHT;
                //}

                Label l = null;
                if (fieldControl.HasCaption)
                {
                    l = new Label();
                    l.Left = x;
                    l.Top = y + 4;
                    l.Width = _Caption_Width;
                    l.Text = f.FieldTitle;
                    l.AutoSize = true;
                    l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    x += _Caption_Width;

                    if (grp == null)
                        control.Controls.Add(l);
                    else
                    {
                        l.Top = y2 + 4;
                        grp.Controls.Add(l);
                    }
                    
                    _LabelMap.Add(f.FieldName, l);
                    if (f.Mandatory)
                    {
                        l.ForeColor = System.Drawing.Color.Blue;
                        l.Font = new Font(l.Font, FontStyle.Bold);
                    }
                }

                Control ctl = fieldControl.FieldControl;
                ctl.Left = x;
                ctl.Top = y;
                
                //将Comfield放到控件的Tag上
                ctl.Tag = f;
                if (f.Mandatory && ctl.GetType()==typeof(CheckBox))
                {
                    ctl.ForeColor = System.Drawing.Color.Blue;
                    ctl.Font = new Font(ctl.Font, FontStyle.Bold);
                }
                if (fieldControl.HasCaption)
                    ctl.Width = _Data_Width;
                else
                    ctl.Width = _Data_Width + _Caption_Width;
                //当占多个控件位置时
                if (f.Width > 1)
                {
                    ctl.Width = ctl.Width + (_Data_Width + _Caption_Width + SPACE_WITH) * (f.Width - 1);
                    columnIndex += (f.Width - 1);
                    x += (_Data_Width + _Caption_Width + SPACE_WITH) * (f.Width - 1);
                }
                if (f.Height > 1)
                {
                    if (ctl.GetType() == typeof(TextBox))
                        ((TextBox)ctl).Multiline = true;
                    int height = ctl.Height * f.Height;
                    ctl.Height = height;
                    if (maxColumnHeight < height)
                        maxColumnHeight = height;
                }
                SetEnabled(ctl,f.Enable);
                if (grp == null)
                    control.Controls.Add(ctl);
                else
                {
                    ctl.Top = y2;
                    grp.Controls.Add(ctl);
                }

                if (l!=null)
                    l.SendToBack();
                _ControlMap.Add(f.FieldName, ctl);
                if (columnIndex == columns)
                {
                    columnIndex = 1;
                    //x = LEFT_VALUE;
                    //y += CONTROL_HEIGHT;
                    //y2 += CONTROL_HEIGHT;
                    //if (grp != null)
                    //    grp.Height += CONTROL_HEIGHT;
                }
                else
                {
                    columnIndex++;
                    if (fieldControl.HasCaption)
                        x += (_Data_Width + SPACE_WITH);
                    else
                        x += (_Data_Width + _Caption_Width + SPACE_WITH);
                }
                if (grp != null && grp.Text == f.GroupName)
                {
                }
                ctl.DataBindings.Add(fieldControl.ValueBindName, dataTable, f.FieldName, false, DataSourceUpdateMode.OnPropertyChanged);

                if (AfterNewControl != null)
                    AfterNewControl(this, fieldControl);
                if (_DetailForm != null)
                    _DetailForm.NewControl(f, ctl);
            }
            if (grp != null)
                grp.Height += maxColumnHeight + 4;
            return y + CONTROL_HEIGHT;
        }

        private void SetEnabled(Control ctl, bool IsEdit)
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
                ctl.Enabled = IsEdit;
            }
        }

        private void getControl(FieldControlEventArgs fieldControl)
        {
            COMField f = fieldControl.Field;
            DataTable dataTable = fieldControl.Data;
            string[] s = fieldControl.Field.ValueType.Split(':');
            switch (s[0].ToLower())
            {
                case "string":
                    TextBox t = new TextBox();
                    t.MaxLength = int.Parse(s[1]);
                    t.ReadOnly = !f.Enable;
                    fieldControl.ValueBindName = "Text";
                    fieldControl.FieldControl = t;
                    break;
                case "number":
                    UltraCurrencyEditor nu = new UltraCurrencyEditor(); 
                    string[] d = s[1].Split(',');
                    nu.FormatString = "#,##0." + new string('0', int.Parse(d[1]));
                    nu.MaskInput = "-nnn,nnn,nnn." + new string('n', int.Parse(d[1]));
                    nu.ReadOnly = !f.Enable;
                    if (dataTable.Rows[0][f.FieldName] == DBNull.Value)
                        dataTable.Rows[0][f.FieldName] = 0;
                    fieldControl.ValueBindName = "Value";
                    fieldControl.FieldControl = nu;
                    break;
                case "int":
                    NumericUpDown nud = new NumericUpDown();
                    nud.Maximum = int.MaxValue;
                    nud.ReadOnly = !f.Enable;
                    fieldControl.ValueBindName = "Value";
                    fieldControl.FieldControl = nud;
                    break;
                case "enum":
                    ComboBox cb = new ComboBox();
                    string[] e = s[1].Split(',');
                    for (int i = 0; i < e.Length; i++)
                    {
                        string[] v = e[i].Split('=');
                        if (v.Length >= 2)
                        {
                            IntItem iItem = new IntItem(int.Parse(v[0]), v[1]);
                            cb.Items.Add(iItem);
                        }
                    }
                    //处理空和默认值
                    if (dataTable.Rows[0][f.FieldName] == DBNull.Value)
                    {
                        if (f.DefaultValue == null || f.DefaultValue == "")
                            dataTable.Rows[0][f.FieldName] = 0;
                        else
                            dataTable.Rows[0][f.FieldName] = int.Parse(f.DefaultValue);
                    }
                    SetCmbItemByInt(cb, (int)dataTable.Rows[0][f.FieldName]);
                    cb.SelectedIndexChanged += new EventHandler(cmb_SelectedIndexChanged);
                    fieldControl.ValueBindName = "Tag";
                    fieldControl.FieldControl = cb;
                    break;
                case "boolean":
                    CheckBox chk = new CheckBox();
                    chk.Text = f.FieldTitle;
                    chk.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    chk.AutoSize = true;
                    fieldControl.ValueBindName = "Checked";
                    fieldControl.FieldControl = chk;
                    fieldControl.HasCaption = false;
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
                    ute.Tag = f;
                    fieldControl.ValueBindName = "Text";
                    fieldControl.FieldControl = ute;
                    break;
                case "custom":
                    UltraTextEditor utec = new UltraTextEditor();
                    EditorButton editButton2 = new EditorButton();
                    utec.ButtonsRight.Add(editButton2);
                    utec.Tag = f;
                    fieldControl.ValueBindName = "Text";
                    fieldControl.FieldControl = utec;
                    break;
                case "date":
                    UltraDateTimeEditor udt = new UltraDateTimeEditor();
                    fieldControl.ValueBindName = "Value";
                    fieldControl.FieldControl = udt;
                    break;
            }
        }

        void ute_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                FindData(sender,true);
            else if (e.Control && e.KeyCode == Keys.L)
                FindData(sender,false);
        }

        public SortedList<string,Control> ControlMap
        {
            get { return _ControlMap; }
        }
        public SortedList<string, Label> LabelMap
        {
            get { return _LabelMap; }
        }
        public DataTable DataTable
        {
            set { _DataTable = value; }
        }
        private bool Setting = false;
        private bool TextChanging = false;
        void Dict_TextChanged(object sender, EventArgs e)
        {
            if (TextChanging)
                return;
            TextChanging = true;
            UltraTextEditor txt = (UltraTextEditor)sender;
            COMField field = (COMField)txt.Tag;
            if (txt.Text == "" && !Setting)
            {
                if (_DataTable.Rows.Count == 0 || _DataTable.Rows[0].RowState == DataRowState.Deleted)
                    return;
                foreach (COMField f in _Fields.Fields)
                {
                    if (f.TableRelation == field.TableRelation)
                    {
                        try
                        {
                            _DataTable.Rows[0][f.FieldName] = DBNull.Value;
                        }
                        catch { }
                        if (_ControlMap.ContainsKey(f.FieldName))
                        {
                            Control ctl = _ControlMap[f.FieldName];
                            if (ctl.GetType() == typeof(TextBox) || ctl.GetType() == typeof(UltraTextEditor))
                                ctl.Text = "";
                            else if (ctl.GetType() == typeof(UltraCurrencyEditor))
                                ((UltraCurrencyEditor)ctl).Value = 0;
                            else if (ctl.GetType() == typeof(NumericUpDown))
                                ((NumericUpDown)ctl).Value = 0;
                            else if (ctl.GetType() == typeof(ComboBox))
                                ((ComboBox)ctl).Tag = null;
                            else if (ctl.GetType() == typeof(CheckBox))
                                ((CheckBox)ctl).Checked = false;
                            else if (ctl.GetType() == typeof(UltraDateTimeEditor))
                                ((UltraDateTimeEditor)ctl).Value = DBNull.Value;
                        }
                    }
                }
            }
            TextChanging = false;
        }

        void Dict_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            FindData(sender,false);
        }
        void FindData(object sender,bool IsKeyDown)
        {
            UltraTextEditor txt = (UltraTextEditor)sender;
            COMField field = (COMField)txt.Tag;
            string[] s = field.ValueType.Split(':');

            
            string where = "";
            if (BeforeSelectForm != null)
            {
                BeforeSelectFormEventArgs bsfe = new BeforeSelectFormEventArgs(field);
                BeforeSelectForm(this, bsfe);
                if (bsfe.Cancel)
                    return;
                where = bsfe.Where;
            }

            frmBrowser frm = frmBrowser.GetForm(s,where);

            //frm.Where = where;
            string other;
            if (s[0].ToLower() == "dict")
            {
                if (frm.MainTableDefine.FieldNameList(false).Contains("Disable"))
                    other = s[1] + ".Disable=0";
                else
                    other = "";
            }
            else
                other = "";

            string text = null;
            if (IsKeyDown)
                text = txt.Text;
            DataRow dr = frm.ShowSelect(string.Format("{0}.{1}", s[1], field.RFieldName), text, other);
            //DataRow dr = frm.ShowSelect("", "", other);
            if (dr != null)
            {
                Setting = true;
                //移到下面去
                //if (AfterSelectForm != null)
                //    AfterSelectForm(sender, new AfterSelectFormEventArgs(field, dr));

                SetRelateionData(dr, field, frm.MainTableDefine);
                //必须到这个地方才能产生AfterSelectForm事件,因为这时候才完成了对外部调用控件的赋值。 
                if (AfterSelectForm != null)
                    AfterSelectForm(sender, new AfterSelectFormEventArgs(field, dr));

                Setting = false;
            }
        }
        public void SetRelateionData(DataRow dr, COMField field,COMFields sourceMainTableDefine)
        {
            foreach (COMField f in _Fields.Fields)
            {
                if (f.TableRelation == field.TableRelation)
                {
                    _DataTable.Rows[0][f.FieldName] = dr[f.RFieldName];
                    if (_ControlMap.ContainsKey(f.FieldName))
                    {
                        Control ctl = _ControlMap[f.FieldName];
                        if (ctl.GetType() == typeof(TextBox) || ctl.GetType() == typeof(UltraTextEditor))
                            ctl.Text = dr[f.RFieldName] as string;
                        else if (ctl.GetType() == typeof(UltraCurrencyEditor))
                            ((UltraCurrencyEditor)ctl).Value = (decimal)dr[f.RFieldName];
                        else if (ctl.GetType() == typeof(NumericUpDown))
                            ((NumericUpDown)ctl).Value = (int)dr[f.RFieldName];
                        else if (ctl.GetType() == typeof(ComboBox))
                            ((ComboBox)ctl).Tag = dr[f.RFieldName];
                        else if (ctl.GetType() == typeof(CheckBox))
                            ((CheckBox)ctl).Checked = (int)dr[f.RFieldName] == 1;
                        else if (ctl.GetType() == typeof(UltraDateTimeEditor))
                            ((UltraDateTimeEditor)ctl).Value = dr[f.RFieldName];
                    }
                }
                if (f.RelationPath != null && f.RelationPath.Length > 0 && f.RelationPath.StartsWith(field.TableRelation))
                {
                    if (f.RelationPath == field.TableRelation)
                    {
                        foreach (COMField f2 in sourceMainTableDefine.Fields)
                        {
                            if (f2.TableRelation == f.TableRelation && f2.RFieldName == f.RFieldName)
                            {
                                _DataTable.Rows[0][f.FieldName] = dr[f2.FieldName];
                                if (_ControlMap.ContainsKey(f.FieldName))
                                {
                                    Control ctl = _ControlMap[f.FieldName];
                                    if (ctl.GetType() == typeof(TextBox) || ctl.GetType() == typeof(UltraTextEditor))
                                        ctl.Text = dr[f2.FieldName] as string;
                                    else if (ctl.GetType() == typeof(UltraCurrencyEditor))
                                        ((UltraCurrencyEditor)ctl).Value = (decimal)dr[f2.FieldName];
                                    else if (ctl.GetType() == typeof(NumericUpDown))
                                        ((NumericUpDown)ctl).Value = (int)dr[f2.FieldName];
                                    else if (ctl.GetType() == typeof(ComboBox))
                                        ((ComboBox)ctl).Tag = dr[f2.FieldName];
                                    else if (ctl.GetType() == typeof(CheckBox))
                                        ((CheckBox)ctl).Checked = (bool)dr[f2.FieldName];
                                    else if (ctl.GetType() == typeof(UltraDateTimeEditor))
                                        ((UltraDateTimeEditor)ctl).Value = dr[f2.FieldName];
                                }
                            }
                        }
                    }
                    else
                    {
                        //需要通过查询取得数据
                        string leftTable;
                        string leftField;
                        string rightField;
                        string rightTable;
                        string from = CSystem.Sys.Svr.Relations.GetRelationFrom(f.RelationPath + "." + f.TableRelation, 2, out leftTable, out rightTable, out leftField, out rightField);
                        DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select {5}.{1} from {2} where {0}.{3}='{4}'", leftTable, f.RFieldName, from, rightField, dr[leftField], rightTable));
                        _DataTable.Rows[0][f.FieldName] = ds.Tables[0].Rows[0][f.RFieldName];
                        if (_ControlMap.ContainsKey(f.FieldName))
                        {
                            Control ctl = _ControlMap[f.FieldName];
                            if (ctl.GetType() == typeof(TextBox) || ctl.GetType() == typeof(UltraTextEditor))
                                ctl.Text = ds.Tables[0].Rows[0][f.RFieldName] as string;
                            else if (ctl.GetType() == typeof(UltraCurrencyEditor))
                                ((UltraCurrencyEditor)ctl).Value = (decimal)ds.Tables[0].Rows[0][f.RFieldName];
                            else if (ctl.GetType() == typeof(NumericUpDown))
                                ((NumericUpDown)ctl).Value = (int)ds.Tables[0].Rows[0][f.RFieldName];
                            else if (ctl.GetType() == typeof(ComboBox))
                                ((ComboBox)ctl).Tag = ds.Tables[0].Rows[0][f.RFieldName];
                            else if (ctl.GetType() == typeof(CheckBox))
                                ((CheckBox)ctl).Checked = (bool)ds.Tables[0].Rows[0][f.RFieldName];
                            else if (ctl.GetType() == typeof(UltraDateTimeEditor))
                                ((UltraDateTimeEditor)ctl).Value = ds.Tables[0].Rows[0][f.RFieldName];
                        }
                    }
                }
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
        
    }
    public delegate void BeforeSelectFormEventHandler(object sender, BeforeSelectFormEventArgs e);
    public class BeforeSelectFormEventArgs : EventArgs
    {
        private COMField _Field;
        private string _Where;
        private bool _Cancel = false;
        private UltraGridRow _GridRow;
        public string Where
        {
            set { _Where = value; }
            get { return _Where; }
        }
        public COMField Field
        {
            get { return _Field; }
        }
        public UltraGridRow GridRow
        {
            get { return _GridRow; }
        }
        public BeforeSelectFormEventArgs(COMField field)
        {
            _Field = field;
        }
        public BeforeSelectFormEventArgs(COMField field,UltraGridRow gridRow)
        {
            _Field = field;
            _GridRow = gridRow;
        }
        public bool Cancel
        {
            get { return _Cancel; }
            set { _Cancel = value; }
        }
    }
    public delegate void AfterSelectFormEventHandler(object sender, AfterSelectFormEventArgs e);
    public class AfterSelectFormEventArgs : EventArgs
    {
        private COMField _Field;
        private DataRow _DataRow;
        private UltraGridRow _GridRow;
        public COMField Field
        {
            get { return _Field; }
        }
        public DataRow Row
        {
            get { return _DataRow; }
        }
        public UltraGridRow GridRow
        {
            get { return _GridRow; }
        }
        public AfterSelectFormEventArgs(COMField field,DataRow dr,UltraGridRow gr)
        {
            _Field = field;
            _DataRow = dr;
            _GridRow = gr;
        }
        public AfterSelectFormEventArgs(COMField field, DataRow dr)
        {
            _Field = field;
            _DataRow = dr;
            _GridRow = null;
        }
    }
    public class CCreateGrid
    {
        public event BeforeSelectFormEventHandler BeforeSelectForm;
        public event AfterSelectFormEventHandler AfterSelectForm;

        private COMFields _Fields;
        private ToolDetailForm _ToolDetailForm;
        private UltraGrid _Grid;
        public UltraGrid Grid
        {
            get { return _Grid; }
        }
        public COMFields Fields
        {
            get { return _Fields; }
        }
        public CCreateGrid(COMFields fields, UltraGrid grid, object dataTable, COMField.Enum_Visible visible)
            : this(fields, grid, dataTable, visible, null)
        {
        }
        public CCreateGrid(COMFields fields, UltraGrid grid, object dataTable, COMField.Enum_Visible visible, ToolDetailForm toolDetailForm)
            : this(fields, null,grid, dataTable, visible, toolDetailForm)
        {
        }
        public  CCreateGrid(COMFields fields,COMFields detailFields, UltraGrid grid, object dataTable,COMField.Enum_Visible visible,ToolDetailForm toolDetailForm)
        {
            _Grid = grid;
            _Fields = fields;
            _ToolDetailForm = toolDetailForm;
            grid.Tag = fields;
            grid.UpdateMode = UpdateMode.OnCellChangeOrLostFocus;
            grid.DisplayLayout.ScrollBounds = ScrollBounds.ScrollToFill;
            grid.SetDataBinding(dataTable, "", false);
            foreach (UltraGridColumn column in grid.DisplayLayout.Bands[0].Columns)
            {
                column.Hidden=true;
            }
            int i = 0;
            bool NeedSummary = false;
            for (int fi = 0; fi < 2; fi++)
            {
                COMFields fs = null;
                if (fi == 0)
                    fs = fields;
                else if (detailFields == null)
                    break;
                else
                    fs = detailFields;
                foreach (COMField f in fs.Fields)
                {
                    if ((f.Visible & visible) == visible)
                    {
                        string fieldName = null;
                        if (fi == 1)
                            fieldName = "D_" + f.FieldName;
                        else
                            fieldName = f.FieldName;
                        UltraGridColumn column = grid.DisplayLayout.Bands[0].Columns[fieldName];
                        Console.WriteLine(fieldName);
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
                            case "date":
                                if (f.Format.Length > 0)
                                    column.Format = f.Format;
                                break;
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
                                column.MaskInput = "-nnn,nnn,nnn,nnn";
                                column.CellClickAction = CellClickAction.EditAndSelectText;
                                break;
                            case "number":
                                column.CellAppearance.TextHAlign = HAlign.Right;
                                string[] d = s[1].Split(',');
                                column.Format = "#,##0." + new string('0', int.Parse(d[1])) + ";-#,##0." + new string('0', int.Parse(d[1])) + "; ";
                                column.MaskInput = "-nnn,nnn,nnn,nnn." + new string('n', int.Parse(d[1]));
                                column.CellClickAction = CellClickAction.EditAndSelectText;
                                break;
                            case "enum":
                                {
                                    string key = "VL_" + f.FieldName;
                                    if (!grid.DisplayLayout.ValueLists.Exists(key))
                                    {
                                        string[] v = s[1].Split(',');
                                        Infragistics.Win.ValueList list = grid.DisplayLayout.ValueLists.Add("VL_" + f.FieldName);
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
                                    EditorWithText editor = new EditorWithText();
                                    editor.Tag = f;
                                    EditorButton but = new EditorButton();
                                    editor.ButtonsRight.Add(but);
                                    editor.KeyDown += new KeyEventHandler(editor_KeyDown);
                                    editor.EditorButtonClick += new EditorButtonEventHandler(editor_EditorButtonClick);
                                    editor.ValueChanged += new EventHandler(editor_ValueChanged);
                                    column.Editor = editor;
                                    break;
                                }
                            case "custom":
                                {
                                    EditorWithText editor = new EditorWithText();
                                    editor.Tag = f;
                                    EditorButton but = new EditorButton();
                                    editor.ButtonsRight.Add(but);
                                    //editor.KeyDown += new KeyEventHandler(editor_KeyDown);
                                    //editor.EditorButtonClick += new EditorButtonEventHandler(editor_EditorButtonClick);
                                    //editor.ValueChanged += new EventHandler(editor_ValueChanged);
                                    column.Editor = editor;
                                    break;
                                }
                        }
                        if (_ToolDetailForm != null)
                            _ToolDetailForm.NewColumn(f, column);
                        //设置合计行
                        if (f.ShowSummary && detailFields==null)
                        {
                            NeedSummary = true;
                            //如果合计行已经存在的情况下，则不需要
                            if (!(grid.DisplayLayout.Bands[0].Summaries.Exists(column.Key)))
                            {
                                SummarySettings ss = grid.DisplayLayout.Bands[0].Summaries.Add(column.Key,SummaryType.Sum, column, SummaryPosition.UseSummaryPositionColumn);
                                if (column.Format != null && column.Format.Length > 0)
                                    ss.DisplayFormat = "{0:" + column.Format + "}";
                            }
                        }
                        i++;
                    }
                }
            }
            if (NeedSummary)
            {
                grid.DisplayLayout.Override.AllowRowSummaries = AllowRowSummaries.False;
                grid.DisplayLayout.Override.SummaryValueAppearance.TextHAlign = HAlign.Right;
                grid.DisplayLayout.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
            }
            //增加对于字段类列的处理
            //grid.BeforeEnterEditMode += new System.ComponentModel.CancelEventHandler(grid_BeforeEnterEditMode);
            grid.AfterCellUpdate += new CellEventHandler(grid_AfterCellUpdate);
        }

        void editor_ValueChanged(object sender, EventArgs e)
        {
            Infragistics.Win.EditorWithText but = (Infragistics.Win.EditorWithText)sender;
            if (but.Value.ToString() == "")
            {
                if (_Grid.ActiveRow == null)
                    return;
                COMField f = (COMField)but.Tag;
                foreach (COMField field in _Fields.Fields)
                {
                    if (field.TableRelation == f.TableRelation)
                    {
                        _Grid.ActiveRow.Cells[field.FieldName].Value = DBNull.Value;
                    }
                    if (field.RelationPath != null && field.RelationPath.Length > 0 && field.RelationPath.StartsWith(f.TableRelation))
                    {
                        _Grid.ActiveRow.Cells[field.FieldName].Value = DBNull.Value;
                    }
                }
            }
        }

        void grid_AfterCellUpdate(object sender, CellEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Cell.Value);
            EditorWithText txt = null;
            UltraGrid grid = (UltraGrid)sender;
            if (grid.ActiveCell == null)
                return;
            if (grid.ActiveCell.Editor != null && grid.ActiveCell.Editor.GetType() == typeof(EditorWithText))
                txt = (EditorWithText)grid.ActiveCell.Editor;
            if (txt == null && (grid.ActiveCell.Column.Editor != null && grid.ActiveCell.Column.Editor.GetType() == typeof(EditorWithText)))
                txt = (EditorWithText)grid.ActiveCell.Column.Editor;
            if (txt != null)
            {
                COMField f = (COMField)txt.Tag;
                //需要增加代码，当不是ＩＤ字段的值更新时，检测ＩＤ字段是否有值，如果没有，将取消前更新。
                //如果做得好一些，可以做一次查询，找出ＩＤ值.
            }
        }

        void grid_BeforeEnterEditMode(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EditorWithText txt = null;
            UltraGrid grid = (UltraGrid)sender;
            if (grid.ActiveCell == null)
                return;
            if (grid.ActiveCell.Editor != null && grid.ActiveCell.Editor.GetType() == typeof(EditorWithText))
                txt = (EditorWithText)grid.ActiveCell.Editor;
            if (txt == null || (grid.ActiveCell.Column.Editor != null && grid.ActiveCell.Column.Editor.GetType() == typeof(EditorWithText)))
                txt = (EditorWithText)grid.ActiveCell.Column.Editor;
            if (txt != null)
            {
                e.Cancel = true;
            }
        }

        void editor_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            SelecForm(sender,false);
        }

        void editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter||(e.KeyCode== Keys.L && e.Control))
            {
                SelecForm(sender,true);
            }
        }

        private void SelecForm(object sender, bool IsKeyDown)
        {
            EditorWithText but = sender as EditorWithText;
            string text = null;
            if (IsKeyDown)
                text = but.TextBox.Text;
            COMField f = (COMField)but.Tag;
            string where = "";
            if (BeforeSelectForm != null)
            {
                BeforeSelectFormEventArgs e = new BeforeSelectFormEventArgs(f, Grid.ActiveRow);
                BeforeSelectForm(this, e);
                if (e.Cancel)
                    return;
                where = e.Where;
            }
            string[] s = f.ValueType.Split(':');

            frmBrowser frm = frmBrowser.GetForm(s,where);
            if (frm == null)
                return;
            
            string other;
            if (s[0].ToLower() == "dict")
            {
                if (frm.MainTableDefine.FieldNameList(false).Contains("Disable"))
                    other = s[1] + ".Disable=0";
                else
                    other = "";
            }
            else
                other = "";
            //if (s[0].ToLower() == "dict")
            //{
            //    if (mainTable.FieldNameList(false).IndexOf("Disable") > -1)
            //        other = s[1] + ".Disable=0";
            //    else
            //        other = "";
            //}
            //else
            //    other = "";

            DataRow[] drs = frm.ShowSelectRows(string.Format("{0}.{1}", s[1], f.RFieldName), text, other);
            if (drs != null)
            {
                but.Value = drs[0][f.RFieldName];
                UltraGridRow row = Grid.ActiveRow;
                for (int i = 0; i < drs.Length; i++)
                {
                    if (i > 0)
                        row = Grid.DisplayLayout.Bands[0].AddNew();
                    foreach (COMField field in _Fields.Fields)
                    {
                        if (field.TableRelation == f.TableRelation)
                        {
                            row.Cells[field.FieldName].Value = drs[i][field.RFieldName];
                        }
                        if (field.RelationPath != null && field.RelationPath.Length > 0 && field.RelationPath.StartsWith(f.TableRelation))
                        {
                            if (field.RelationPath == f.TableRelation)
                            {
                                foreach (COMField f2 in frm.MainTableDefine.Fields)
                                {
                                    if (f2.TableRelation == field.TableRelation && f2.RFieldName == field.RFieldName)
                                        row.Cells[field.FieldName].Value = drs[i][f2.FieldName];
                                }
                            }
                            else
                            {
                                //需要通过查询取得数据
                                string leftTable;
                                string leftField;
                                string rightTable;
                                string rightField;
                                string from = CSystem.Sys.Svr.Relations.GetRelationFrom(field.RelationPath + "." + field.TableRelation, 2, out leftTable, out rightTable, out leftField, out rightField);
                                DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select {5}.{1} from {2} where {0}.{3}='{4}'", leftTable, field.RFieldName, from, rightField, drs[i][leftField], rightTable));
                                row.Cells[field.FieldName].Value = ds.Tables[0].Rows[0][field.RFieldName];
                            }
                        }
                    }
                        
                    if (AfterSelectForm != null)
                    {
                        AfterSelectForm(this, new AfterSelectFormEventArgs(f, drs[i], row));
                    }
                }
                if (drs.Length > 1)
                    Grid.DisplayLayout.Bands[0].AddNew();
                Grid.UpdateData();
                if (drs.Length == 1)
                    Grid.PerformAction(UltraGridAction.NextCellByTab);
            }
        }
    }
}
