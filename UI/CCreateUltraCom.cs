using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using Base;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win;

namespace UI
{
    public class CCreateUltraCom
    {
        public COMFields _Fields;
        public DataTable _DataTable;
        private bool isKey = false;
        private Keys keys;
        public event BeforeSelectFormEventHandler BeforeSelectForm;
        public event AfterSelectFormEventHandler AfterSelectForm;
        public SortedList<string, Control> _ControlMap;
        public CCreateUltraCom()
        {
        }
        public int ShowRowCount = 20;
        public CCreateUltraCom(COMFields fields, DataTable dataTable,ToolDetailForm form,SortedList<string, Control> controlMap)
        {
            _DataTable = dataTable;
            _Fields = fields;
            _ControlMap = controlMap;
        }
        public UltraComboEditor createUltraComboEditor(COMField field)
        {
            UltraComboEditor ute = new UltraComboEditor();
            ute.DisplayMember = "name";
            ute.ValueMember = "code";
            ute.DropDownListWidth = 500;
            ute.DropDownListAlignment = Infragistics.Win.DropDownListAlignment.Left;
            ute.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            ute.UseOsThemes = DefaultableBoolean.False;
            //string where = "1=1";
            //String sql = "select lower(code) as code,name from " + field.RightTableName + " where  " + where + " order by code ";
            //DataSet ds = new DataSet();
            //ds = CSystem.Sys.Svr.cntMain.Select(sql);
            //ute.DataSource = ds;
            TextChanging = false;
            EditorButton editButton = new EditorButton();
            ute.ButtonsRight.Add(editButton);
            ute.KeyDown += new KeyEventHandler(ute_KeyDown);
            ute.EditorButtonClick += new EditorButtonEventHandler(Dict_EditorButtonClick);
            ute.TextChanged += new EventHandler(Dict_TextChanged);         
            ute.Tag = _Fields;
            ute.ValueChanged+=new EventHandler(ute_ValueChanged);
            //ute.SelectionChanged += new EventHandler(ute_SelectionChanged);
            TextChanging = true;
            return ute;
        }
        //void ute_SelectionChanged(object sender, EventArgs e)
        //{
        //    TextChanging = true;
        //}
        void ute_KeyDown(object sender, KeyEventArgs e)
        {
            isKey = true;
            keys = e.KeyCode;
            if (e.KeyCode == Keys.Enter)
                FindData(sender, true);
            else if (e.Control && e.KeyCode == Keys.L)
                FindData(sender, false);
        }
        private bool Setting = false;
        private bool TextChanging = false;
        void ute_ValueChanged(object sender, EventArgs e)
        {
            if (TextChanging)
            {
                UltraComboEditor ulcombox = (UltraComboEditor)sender;
                COMField fd = (COMField)ulcombox.Tag;
                if (ulcombox.SelectedIndex >= 0)
                {
                    string text = null;
                    text = (String)ulcombox.SelectedItem.DataValue;
                    COMFields fields = CSystem.Sys.Svr.LDI.GetFields(fd.RightTableName);
                    String sql = fields.QuerySQLWithClause(fd.RightTableName + ".code ='" + (String)ulcombox.Value + "'");
                    DataSet ds = new DataSet();
                    ds = CSystem.Sys.Svr.cntMain.Select(sql);
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        Setting = true;
                        SetRelateionData(ds.Tables[0].Rows[0], fd, CSystem.Sys.Svr.LDI.GetFields(fd.RightTableName));
                        if (AfterSelectForm != null)
                            AfterSelectForm(sender, new AfterSelectFormEventArgs(fd, (DataRow)ds.Tables[0].Rows[0]));
                        Setting = false;
                    }
                    UltraComboEditor txt = (UltraComboEditor)sender;
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
                                    if (ctl.GetType() == typeof(TextBox) || ctl.GetType() == typeof(UltraComboEditor))
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
            }
        }
        void Dict_TextChanged(object sender, EventArgs e)
        {
            UltraComboEditor ulcombox = (UltraComboEditor)sender;
            COMField fd = (COMField)ulcombox.Tag;
            if (keys != Keys.Up && keys != Keys.Down && keys != Keys.Enter && keys != Keys.RButton && keys != Keys.LButton && isKey)
            {
                if (fd.SourceDS == null || fd.SourceDS.Tables.Count == 0)
                {
                    string where = "1=1";
                    if (BeforeSelectForm != null)
                    {
                        BeforeSelectFormEventArgs bsfe = new BeforeSelectFormEventArgs(fd);
                        BeforeSelectForm(this, bsfe);
                        if (bsfe.Cancel)
                            return;
                        if (null != bsfe.Where)
                            where = bsfe.Where;
                    }
                    //COMFields fields = CSystem.Sys.Svr.LDI.GetFields(fd.RightTableName);
                    //String sql = fields.QuerySQLWithClause(fd.RightTableName + where);


                    String sql = "select lower(code) as code,name from " + fd.RightTableName + " where  " + where + " order by code ";
                    DataSet ds = new DataSet();
                    ds = CSystem.Sys.Svr.cntMain.Select(sql);
                    fd.SourceDS = ds;
                }
                DataTable dicTable = fd.SourceDS.Tables[0];
                DataRow[] allrow = dicTable.Select("code like '%" + ulcombox.Text.ToLower() + "%'", "code");
                DataTable dtNew = dicTable.Clone();
                int count = this.ShowRowCount;
                if (allrow.Length < this.ShowRowCount)
                {
                    count = allrow.Length;
                }
                for (int i = 0; i < count; i++)
                {
                    dtNew.ImportRow(allrow[i]);
                }
                ulcombox.DataSource = dtNew;
                ulcombox.CloseUp();
                ulcombox.DropDown();
                ulcombox.Focus();
                int len = ulcombox.Text.Length;
                try
                {
                    ulcombox.Editor.SelectionStart = len;
                }
                catch
                {
                }
            }
            else
            {
                TextChanging = true;
            }
            //else
            //{
            //    if (ulcombox.SelectedIndex > 0)
            //    {
            //        string text = null;
            //        text = (String)ulcombox.SelectedItem.DataValue;
            //        String sql = "select * from " + fd.RightTableName + " order by code ";
            //        DataSet ds = new DataSet();
            //        ds = CSystem.Sys.Svr.cntMain.Select(sql);
            //        if(ds.Tables[0].Rows.Count==1)
            //        {
            //            Setting = true;
            //            SetRelateionData(ds.Tables[0].Rows[0], fd, CSystem.Sys.Svr.LDI.GetFields(fd.RightTableName));
            //            Setting = false;
            //        }                    
            //    }
            //}
            isKey = false;
            //if (TextChanging)
            //    return;
            //TextChanging = true;
            //UltraComboEditor txt = (UltraComboEditor)sender;
            //COMField field = (COMField)txt.Tag;
            //if (txt.Text == "" && !Setting)
            //{
            //    if (_DataTable.Rows.Count == 0 || _DataTable.Rows[0].RowState == DataRowState.Deleted)
            //        return;
            //    foreach (COMField f in _Fields.Fields)
            //    {
            //        if (f.TableRelation == field.TableRelation)
            //        {
            //            try
            //            {
            //                _DataTable.Rows[0][f.FieldName] = DBNull.Value;
            //            }
            //            catch { }
            //            if (_ControlMap.ContainsKey(f.FieldName))
            //            {
            //                Control ctl = _ControlMap[f.FieldName];
            //                if (ctl.GetType() == typeof(TextBox) || ctl.GetType() == typeof(UltraComboEditor))
            //                    ctl.Text = "";
            //                else if (ctl.GetType() == typeof(UltraCurrencyEditor))
            //                    ((UltraCurrencyEditor)ctl).Value = 0;
            //                else if (ctl.GetType() == typeof(NumericUpDown))
            //                    ((NumericUpDown)ctl).Value = 0;
            //                else if (ctl.GetType() == typeof(ComboBox))
            //                    ((ComboBox)ctl).Tag = null;
            //                else if (ctl.GetType() == typeof(CheckBox))
            //                    ((CheckBox)ctl).Checked = false;
            //                else if (ctl.GetType() == typeof(UltraDateTimeEditor))
            //                    ((UltraDateTimeEditor)ctl).Value = DBNull.Value;
            //            }
            //        }
            //    }
            //}
            //TextChanging = false;
        }

        private void Enter()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void Dict_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            isKey = false;
            FindData(sender, false);
        }
        void FindData(object sender, bool IsKeyDown)
        {
            UltraComboEditor txt = (UltraComboEditor)sender;
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

            frmBrowser frm = frmBrowser.GetForm(s, where);

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
        public void SetRelateionData(DataRow dr, COMField field, COMFields sourceMainTableDefine)
        {
            foreach (COMField f in _Fields.Fields)
            {
                if (f.TableRelation == field.TableRelation)
                {
                    _DataTable.Rows[0][f.FieldName] = dr[f.RFieldName];
                    if (_ControlMap.ContainsKey(f.FieldName))
                    {
                        Control ctl = _ControlMap[f.FieldName];
                        if (ctl.GetType() == typeof(TextBox) || ctl.GetType() == typeof(UltraComboEditor))
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
                                    if (ctl.GetType() == typeof(TextBox) || ctl.GetType() == typeof(UltraComboEditor))
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
                            if (ctl.GetType() == typeof(TextBox) || ctl.GetType() == typeof(UltraComboEditor))
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
    }
}
