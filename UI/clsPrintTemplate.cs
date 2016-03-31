using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.ComponentModel;
using Base;
using System.IO;

namespace UI
{
    public class clsPrintTemplate:IPrintParent
    {
        private string _FileName;
        private bool _IsNew;

        private XmlDocument _Doc;
        private XmlElement _Xml;
        private clsPrintRange _PageHeader;
        private clsPrintRange _PageFooter;
        private clsPrintMainBody _MainBody;
        private clsPrintColumns _Columns;
        public clsPrintTemplate()
        {
            _IsNew = true;
            _Doc = new XmlDocument();
            _Xml = _Doc.CreateElement("PrintTemplate");
            _Doc.AppendChild(_Xml);
        }
        public clsPrintTemplate(string fileName)
        {
            _Doc = new XmlDocument();
            _IsNew = false;
            _FileName = fileName;
            _Doc.Load(fileName);
            _Xml = (XmlElement)_Doc.SelectSingleNode("PrintTemplate");
        }
        public void Save(string fileName)
        {
            _Doc.Save(fileName);
        }
        [Browsable(false)]
        public bool IsNew
        {
            get { return _IsNew; }
        }
        public string Tag
        {
            get { return _Xml.GetAttribute("Tag"); }
            set { _Xml.SetAttribute("Tag", value); }
        }
        public clsPrintRange NewPageHeader()
        {
            if (_PageHeader == null)
            {
                XmlElement xml = _Xml.SelectSingleNode("PageHeader") as XmlElement;
                if (xml == null)
                {
                    xml = _Xml.OwnerDocument.CreateElement("PageHeader");
                    _Xml.AppendChild(xml);
                }
                _PageHeader = new clsPrintRange(xml, this, this);
                return _PageHeader;
            }
            else
                return _PageHeader;
        }
        [Browsable(false)]
        public clsPrintRange PageHeader
        {
            get
            {
                if (_PageHeader == null)
                {
                    XmlElement xml = _Xml.SelectSingleNode("PageHeader") as XmlElement;
                    if (xml != null)
                        _PageHeader = new clsPrintRange(xml, this, this);
                    return _PageHeader;
                }
                else
                    return _PageHeader;
            }
        }
        public clsPrintRange NewPageFooter()
        {
            if (_PageFooter == null)
            {
                XmlElement xml = _Xml.SelectSingleNode("PageFooter") as XmlElement;
                if (xml == null)
                {
                    xml = _Xml.OwnerDocument.CreateElement("PageFooter");
                    _Xml.AppendChild(xml);
                }
                _PageFooter = new clsPrintRange(xml, this, this);
                return _PageFooter;
            }
            else
                return _PageFooter;
        }
        [Browsable(false)]
        public clsPrintRange PageFooter
        {
            get
            {
                if (_PageFooter == null)
                {
                    XmlElement xml = _Xml.SelectSingleNode("PageFooter") as XmlElement;
                    if (xml != null)
                        _PageFooter = new clsPrintRange(xml, this, this);
                    return _PageFooter;
                }
                else
                    return _PageFooter;
            }
        }
        public clsPrintMainBody NewMainBody()
        {
            if (_MainBody == null)
            {
                XmlElement xml = _Xml.SelectSingleNode("MainBody") as XmlElement;
                if (xml == null)
                {
                    xml = _Xml.OwnerDocument.CreateElement("MainBody");
                    _Xml.AppendChild(xml);
                }

                _MainBody = new clsPrintMainBody(xml, this);
                return _MainBody;
            }
            else
                return _MainBody;
        }
        [Browsable(false)]
        public clsPrintMainBody MainBody
        {
            get
            {
                if (_MainBody == null)
                {
                    XmlElement xml = _Xml.SelectSingleNode("MainBody") as XmlElement;
                    if (xml != null)
                        _MainBody = new clsPrintMainBody(xml, this);
                    return _MainBody;
                }
                else
                    return _MainBody;
            }
        }
        public clsPrintColumns Columns
        {
            get
            {
                if (_Columns == null)
                {
                    XmlElement xml = _Xml.SelectSingleNode("Columns") as XmlElement;
                    if (xml == null)
                    {
                        xml = _Xml.OwnerDocument.CreateElement("Columns");
                        _Xml.AppendChild(xml);
                    }
                    _Columns = new clsPrintColumns(xml,this);
                    return _Columns;
                }
                else
                    return _Columns;
            }
        }

        public string OuterXml
        {
            get
            {
                if (_Doc != null)
                    return _Doc.OuterXml;
                else
                    return "";
            }
        }
        #region IPrintParent Members

        public void ResetRowCount()
        {
            
        }

        #endregion
    }
    public class clsPrintMainBody : List<clsPrintTable>
    {
        private XmlElement _Xml;
        private clsPrintTemplate _Template;

        public clsPrintMainBody(XmlElement xml,clsPrintTemplate template)
        {
            _Xml = xml;
            _Template = template;
            foreach (XmlNode n in xml.ChildNodes)
            {
                XmlElement x = (XmlElement)n;
                this.Add(new clsPrintTable(x, this, _Template));
            }
        }
        public clsPrintTable NewChild()
        {
            XmlElement xml = _Xml.OwnerDocument.CreateElement("Table");
            _Xml.AppendChild(xml);
            clsPrintTable v = new clsPrintTable(xml, this, _Template);
            this.Add(v);
            return v;
        }
    }
    public interface IPrintParent
    {
        void ResetRowCount();
    }
    public class clsPrintTable:IPrintParent
    {
        private XmlElement _Xml;
        private clsPrintRange _TableHeader;
        private clsPrintRange _TableBody;
        private clsPrintMainBody _Body;
        private clsPrintTemplate _Template;

        public clsPrintTable(XmlElement xml,clsPrintMainBody body,clsPrintTemplate template)
        {
            _Xml = xml;
            _Body = body;
            _Template = template;
        }
        public string OuterXml
        {
            get { return _Xml.OuterXml; }
        }
        [Browsable(false)]
        public clsPrintRange TableHeader
        {
            get
            {
                if (_TableHeader == null)
                {
                    XmlElement xml = _Xml.SelectSingleNode("TableHeader") as XmlElement;
                    if (xml == null)
                    {
                        xml = _Xml.OwnerDocument.CreateElement("TableHeader");
                        _Xml.AppendChild(xml);
                    }
                    _TableHeader = new clsPrintRange(xml,(IPrintParent)this,_Template);
                    //_TableHeader.RowCount = 1;
                    return _TableHeader;
                }
                else
                    return _TableHeader;
            }
        }
        [Browsable(false)]
        public clsPrintRange TableBody
        {
            get
            {
                if (_TableBody == null)
                {
                    XmlElement xml = _Xml.SelectSingleNode("TableBody") as XmlElement;
                    if (xml == null)
                    {
                        xml = _Xml.OwnerDocument.CreateElement("TableBody");
                        _Xml.AppendChild(xml);
                    }
                    _TableBody = new clsPrintRange(xml, this, _Template);
                    //_TableBody.RowCount = 1;
                    return _TableBody;
                }
                else
                    return _TableBody;
            }
        }

        [Browsable(false)]
        public int RowCount
        {
            get { return clsXMLTools.GetInt(_Xml, "RowCount"); }
            set { _Xml.SetAttribute("RowCount", value.ToString()); }
        }
        public void ResetRowCount()
        {
            int c = 0;
            if (_TableBody != null)
                c = TableBody.RowCount;
            if (_TableHeader != null)
                c += _TableHeader.RowCount;
            RowCount = c;
        }
        [TypeConverter(typeof(TableFieldConverter))]
        public string TableName
        {
            get { return _Xml.GetAttribute("TableName"); }
            set { _Xml.SetAttribute("TableName", value); }
        }
        public int MaxLinePerPage
        {
            get { return clsXMLTools.GetInt(_Xml, "MaxLinePerPage"); }
            set { _Xml.SetAttribute("MaxLinePerPage", value.ToString()); }
        }
        public bool FullEmptyLine
        {
            get { return clsXMLTools.GetBool(_Xml, "FullEmptyLine"); }
            set { _Xml.SetAttribute("FullEmptyLine", value.ToString()); }
        }
        public bool ShowHeaderInPerPage
        {
            get { return clsXMLTools.GetBool(_Xml, "ShowHeaderInPerPage"); }
            set { _Xml.SetAttribute("ShowHeaderInPerPage", value.ToString()); }
        }
    }
    public class clsPrintRange:IPrintParent
    {
        private XmlElement _Xml;
        private clsPrintCells _Cells;
        private clsPrintRows _Rows;
        private IPrintParent _Parent;
        private clsPrintTemplate _Template;
        public clsPrintRange(XmlElement xml,IPrintParent parent,clsPrintTemplate template)
        {
            _Xml = xml;
            _Parent = parent;
            _Template = template;
        }
        [Browsable(false)]
        public IPrintParent Parent
        {
            get { return _Parent; }
        }
        public string OuterXml
        {
            get { return _Xml.OuterXml; }
        }
        [TypeConverter(typeof(TableFieldConverter))]
        public string DefaultTableName
        {
            get
            {
                return this.Cells.DefaultTableName;
            }
            set
            {
                this.Cells.DefaultTableName = value;
            }
        }
        [Browsable(false)]
        public clsPrintCells Cells
        {
            get
            {
                if (_Cells == null)
                {
                    XmlElement xml = _Xml.SelectSingleNode("Cells") as XmlElement;
                    if (xml == null)
                    {
                        xml = _Xml.OwnerDocument.CreateElement("Cells");
                        _Xml.AppendChild(xml);
                    }
                    _Cells = new clsPrintCells(xml,this);
                    return _Cells;
                }
                else
                    return _Cells;
            }
        }
        [Browsable(false)]
        public clsPrintRows Rows
        {
            get
            {
                if (_Rows == null)
                {
                    XmlElement xml = _Xml.SelectSingleNode("Rows") as XmlElement;
                    if (xml == null)
                    {
                        xml = _Xml.OwnerDocument.CreateElement("Rows");
                        _Xml.AppendChild((XmlNode)xml);
                    }
                    _Rows = new clsPrintRows(xml, this, _Template);
                    return _Rows;
                }
                else
                    return _Rows;
            }
        }
        public int RowCount
        {
            get { return clsXMLTools.GetInt(_Xml,"RowCount"); }
            set
            {
                _Xml.SetAttribute("RowCount", value.ToString());
                _Parent.ResetRowCount();
            }
        }

        #region IPrintParent Members

        public void ResetRowCount()
        {
            this.RowCount = this.Rows.Count;
            _Parent.ResetRowCount();
        }

        #endregion
    }

    public class clsPrintColumnCompare : IComparer<clsPrintColumn>
    {
        #region IComparer<clsPrintColumn> Members

        public int Compare(clsPrintColumn x, clsPrintColumn y)
        {
            return  (x.Index.CompareTo(y.Index));
        }

        #endregion
    }
    public class clsPrintColumns : List<clsPrintColumn>
    {
        private XmlElement _Xml;
        private clsPrintTemplate _Parent;
        public clsPrintColumns(XmlElement xml,clsPrintTemplate parent)
        {
            _Xml = xml;
            _Parent = parent;
            foreach (XmlNode n in xml.ChildNodes)
            {
                XmlElement x = (XmlElement)n;
                this.Add(new clsPrintColumn(x));
            }
            this.Sort(new clsPrintColumnCompare());
        }
        public string OuterXml
        {
            get { return _Xml.OuterXml; }
        }

        public void Remove(int i)
        {
            clsPrintColumn col=null;
            foreach (clsPrintColumn c in this)
            {
                if (c.Index == i)
                {
                    col = c;
                    break;
                }
            }
            if (col != null)
            {
                foreach (clsPrintColumn c in this)
                {
                    if (c.Index > i)
                    {
                        c.Index--;
                    }
                }

                _Xml.RemoveChild(col.XML);
                this.Remove(col);

                if (_Parent.PageHeader != null)
                    RemoveColumn(i, _Parent.PageHeader);
                if (_Parent.MainBody != null)
                    foreach (clsPrintTable table in _Parent.MainBody)
                    {
                        RemoveColumn(i, table.TableHeader);
                        RemoveColumn(i, table.TableBody);
                    }
                if (_Parent.PageFooter != null)
                    RemoveColumn(i, _Parent.PageFooter);
            }
            _Parent.ResetRowCount();
        }
        private void RemoveColumn(int i, clsPrintRange range)
        {
            List<clsPrintCell> list = new List<clsPrintCell>();
            foreach (clsPrintCell cell in range.Cells)
            {
                if (cell.StartColumn == i && cell.EndColumn == i)
                    list.Add(cell);
            }
            foreach (clsPrintCell cell in range.Cells)
            {
                if ((cell.StartColumn <= i && cell.EndColumn>i) ||  (cell.StartColumn< i && cell.EndColumn==i))
                {
                    cell.EndColumn--;
                }
                else if (cell.StartColumn >= i)
                {
                    cell.EndColumn--;
                    cell.StartColumn--;
                }
            }
            foreach (clsPrintCell cell in list)
            {
                range.Cells.Remove(cell);
            }
        }
        public clsPrintColumn NewChild()
        {
            return InsertChild(this.Count);
        }
        public clsPrintColumn InsertChild(int i)
        {
            if (i > this.Count)
                i = this.Count;
            //改变后续的列
            foreach(clsPrintColumn col in this)
            {
                if (col.Index >= i)
                    col.Index++;
            }
            XmlElement xml = _Xml.OwnerDocument.CreateElement("Column");
            _Xml.AppendChild(xml);
            clsPrintColumn v = new clsPrintColumn(xml);
            v.Index = i;
            this.Add(v);
            //再找出所有后续的单元格，将他们后移或如果是跨列的，增加其宽度
            if (_Parent.PageHeader!=null)
                InsertColumn(i, _Parent.PageHeader);
            if (_Parent.MainBody!=null)
                foreach (clsPrintTable table in _Parent.MainBody)
                {
                    InsertColumn(i, table.TableHeader);
                    InsertColumn(i, table.TableBody);
                }
            if (_Parent.PageFooter!=null)
                InsertColumn(i, _Parent.PageFooter);

            return v;
        }
        private void InsertColumn(int i,clsPrintRange range)
        {
            List<int> list=new List<int>();
            foreach (clsPrintCell cell in range.Cells)
            {
                if (cell.StartColumn >= i)
                {
                    cell.StartColumn++;
                    cell.EndColumn++;
                }
                else if (cell.StartColumn < i && cell.EndColumn >= i)
                {
                    cell.EndColumn++;
                    for(int r =cell.StartRow;r<cell.EndRow;r++)
                        list.Add(r);
                }
            }
            foreach (clsPrintRow row in range.Rows)
            {
                if (!list.Contains(row.Index))
                {
                    clsPrintCell cell = range.Cells.NewChild();
                    cell.StartRow = row.Index;
                    cell.EndRow = row.Index;
                    cell.StartColumn = i;
                    cell.EndColumn = i;
                }
            }
        }
    }
    public class clsPrintColumn
    {
        private XmlElement _Xml;
        public clsPrintColumn(XmlElement xml)
        {
            _Xml = xml;
        }
        public string OuterXml
        {
            get { return _Xml.OuterXml; }
        }
        [Browsable(false)]
        public XmlElement XML
        {
            get { return _Xml; }
        }
        public int Index
        {
            get { return clsXMLTools.GetInt(_Xml, "Index"); }
            set { _Xml.SetAttribute("Index", value.ToString()); }
        }
        public int Width
        {
            get { return clsXMLTools.GetInt(_Xml, "Width"); }
            set { _Xml.SetAttribute("Width", value.ToString()); }
        }
    }
    public class clsPrintRowCompare : IComparer<clsPrintRow>
    {
        #region IComparer<clsPrintRow> Members

        public int Compare(clsPrintRow x, clsPrintRow y)
        {
            return x.Index.CompareTo(y.Index);
        }

        #endregion
    }
    public class clsPrintRows : List<clsPrintRow>
    {
        private XmlElement _Xml;
        private IPrintParent _Parent;
        private clsPrintTemplate _Template;

        public clsPrintRows(XmlElement xml,IPrintParent parent,clsPrintTemplate template)
        {
            _Xml = xml;
            _Parent = parent;
            _Template = template;
            foreach (XmlNode n in xml.ChildNodes)
            {
                XmlElement x = (XmlElement)n;
                this.Add(new clsPrintRow(x));
            }
            this.Sort(new clsPrintRowCompare());
        }
        public string OuterXml
        {
            get { return _Xml.OuterXml; }
        }
        private void RemoveRow(int i, clsPrintRange range)
        {
            List<clsPrintCell> list = new List<clsPrintCell>();
            foreach (clsPrintCell cell in range.Cells)
            {
                if (cell.StartRow == i && cell.EndRow == i)
                    list.Add(cell);
            }
            foreach (clsPrintCell cell in range.Cells)
            {
                if ((cell.StartRow <= i && cell.EndRow > i) || (cell.StartRow < i && cell.EndRow == i))
                {
                    cell.EndRow--;
                }
                else if (cell.StartRow >= i)
                {
                    cell.StartRow--;
                    cell.EndRow--;
                }
            }
            foreach (clsPrintCell cell in list)
            {
                range.Cells.Remove(cell);
            }
        }
        public void Remove(int i)
        {
            clsPrintRow row = null;
            foreach (clsPrintRow r in this)
            {
                if (r.Index==i)
                {
                    row = r;
                    break;
                }
            }
            if (row != null)
            {
                foreach (clsPrintRow r in this)
                {
                    if (r.Index > i)
                        r.Index--;
                }

                _Xml.RemoveChild(row.XML);
                this.Remove(row);

                RemoveRow(i, (clsPrintRange)_Parent);
            }
            _Parent.ResetRowCount();
        }
        public clsPrintRow NewChild()
        {
            return InsertChild(this.Count);
        }
        public clsPrintRow InsertChild(int i)
        {
            if (i > this.Count)
                i = this.Count;
            //改变后续的列
            foreach (clsPrintRow row in this)
            {
                if (row.Index >= i)
                    row.Index++;
            }
            XmlElement xml = _Xml.OwnerDocument.CreateElement("Row");
            _Xml.AppendChild(xml);
            clsPrintRow v = new clsPrintRow(xml);
            v.Index = i;
            v.Height = 30;
            this.Add(v);
            //再找到后续的行,下移或跨行的增加高度
            List<int> list = new List<int>();
            clsPrintRange range = (clsPrintRange)_Parent;
            foreach (clsPrintCell cell in range.Cells)
            {
                if (cell.StartRow >= i)
                {
                    cell.StartRow++;
                    cell.EndRow++;
                }
                else if (cell.StartRow < i && cell.EndRow >= i)
                {
                    cell.EndRow++;
                    for (int c = cell.StartColumn; c < cell.EndRow; c++)
                        list.Add(c);
                }
            }
            foreach(clsPrintColumn col in _Template.Columns)
            {
                if (!list.Contains(col.Index))
                {
                    clsPrintCell cell = range.Cells.NewChild();
                    cell.StartRow = i;
                    cell.EndRow = i;
                    cell.StartColumn = col.Index;
                    cell.EndColumn = col.Index;
                }
            }
            _Parent.ResetRowCount();
            return v;
        }
    }
    public class clsPrintRow
    {
        private XmlElement _Xml;
        public clsPrintRow(XmlElement xml)
        {
            _Xml = xml;
        }
        [Browsable(false)]
        public XmlElement XML
        {
            get { return _Xml; }
        }
        public int Index
        {
            get { return clsXMLTools.GetInt(_Xml, "Index"); }
            set { _Xml.SetAttribute("Index", value.ToString()); }
        }
        public int Height
        {
            get { return clsXMLTools.GetInt(_Xml, "Height"); }
            set { _Xml.SetAttribute("Height", value.ToString()); }
        }
    }
    public class clsPrintCells : List<clsPrintCell>
    {
        private XmlElement _Xml;
        private clsPrintRange _Parent;

        public clsPrintCells(XmlElement xml,clsPrintRange parent)
        {
            _Xml = xml;
            _Parent = parent;
            foreach (XmlNode n in xml.ChildNodes)
            {
                XmlElement x = (XmlElement)n;
                this.Add(new clsPrintCell(x,this));
            }
        }
        [Browsable(false)]
        public clsPrintRange Parent
        {
            get { return _Parent; }
        }
        public string OuterXml
        {
            get { return _Xml.OuterXml; }
        }
        public void Remove(clsPrintCell cell)
        {
            _Xml.RemoveChild(cell.XML);
            base.Remove(cell);
        }
        [TypeConverter(typeof(TableFieldConverter))]
        public string DefaultTableName
        {
            get { return _Xml.GetAttribute("DefaultTableName"); }
            set { _Xml.SetAttribute("DefaultTableName", value); }
        }
        public clsPrintCell NewChild()
        {
            XmlElement xml = _Xml.OwnerDocument.CreateElement("Cell");
            _Xml.AppendChild(xml);
            clsPrintCell v = new clsPrintCell(xml,this);
            this.Add(v);
            return v;
        }
    }
    public class clsPrintCell
    {
        public enum enuGetType : int { Value = 0, Caption = 1 };
        
        private XmlElement _Xml;
        private clsPrintCells _Parent;
        public clsPrintCell(XmlElement xml,clsPrintCells parent)
        {
            _Xml = xml;
            _Parent = parent;
        }
        [Browsable(false)]
        public clsPrintCells Parent
        {
            get { return _Parent; }
        }
        public string OuterXml
        {
            get { return _Xml.OuterXml; }
        }
        [Browsable(false)]
        public XmlElement XML
        {
            get{return _Xml;}
        }
        [Description("开始行"),Category("行")]
        public int StartRow
        {
            get { return clsXMLTools.GetInt(_Xml, "StartRow"); }
            set { _Xml.SetAttribute("StartRow", value.ToString()); }
        }
        [Description("结束行"), Category("行")]
        public int EndRow
        {
            get { return clsXMLTools.GetInt(_Xml, "EndRow"); }
            set { _Xml.SetAttribute("EndRow", value.ToString()); }
        }
        [Description("开始列"), Category("列")]
        public int StartColumn
        {
            get { return clsXMLTools.GetInt(_Xml, "StartColumn"); }
            set { _Xml.SetAttribute("StartColumn", value.ToString()); }
        }
        [Description("结束列"), Category("列")]
        public int EndColumn
        {
            get { return clsXMLTools.GetInt(_Xml, "EndColumn"); }
            set { _Xml.SetAttribute("EndColumn", value.ToString()); }
        }
        [Description("格式:/r/n数字型，如#,##0.00/r/n日期型，yyyy-MM-dd/r/n时间，如hh:mm:ss"), Category("数据"), TypeConverter(typeof(FormatConverter))]
        public string Format
        {
            get { return _Xml.GetAttribute("Format"); }
            set { _Xml.SetAttribute("Format", value); }
        }
        [Description("是否图片"), Category("图片")]
        public bool IsImage
        {
            get { return clsXMLTools.GetBool(_Xml,"IsImage"); }
            set { _Xml.SetAttribute("IsImage", value.ToString()); }
        }
        [Description("图片内容"), Category("图片序列化")]
        public string Image
        {
            get { return _Xml.GetAttribute("Image"); }
            set { _Xml.SetAttribute("Image", value); }
        }
        [Description("文本内容"), Category("数据")]
        public string Text
        {
            get { return _Xml.GetAttribute("Text"); }
            set { _Xml.SetAttribute("Text", value); }
        }
        [Description("字段"), Category("数据"),TypeConverter(typeof(TableFieldConverter))]
        public string Field
        {
            get { return _Xml.GetAttribute("Field"); }
            set { _Xml.SetAttribute("Field", value); }
        }
        [Description("字段值或字段名称"), Category("数据")]
        public enuGetType ValueOrCatpion
        {
            get { return clsXMLTools.GetInt(_Xml, "ValueOrCatpion") == 0 ? enuGetType.Value : enuGetType.Caption; }
            set { _Xml.SetAttribute("ValueOrCatpion", (value == enuGetType.Value ? 0 : 1).ToString()); }
        }
        [Description("是否为空"), Category("数据")]
        public bool IsNull
        {
            get { return clsXMLTools.GetBool(_Xml, "IsNull"); }
            set { _Xml.SetAttribute("IsNull", value.ToString()); }
        }
        private RectangleBorder _RectangleBorder;
        [Description("边线宽度"), Category("格式")]
        public RectangleBorder BoderStyle
        {
            get
            {
                if (_RectangleBorder == null)
                {
                    string s = _Xml.GetAttribute("BoderStyle");
                    int t = 0;
                    int b = 0;
                    int l = 0;
                    int r = 0;
                    if (s != null && s.Length > 0)
                    {
                        string[] v = s.Split(',');
                        try
                        {
                            if (v.Length > 0)
                                t = int.Parse(v[0]);
                            if (v.Length > 1)
                                b = int.Parse(v[1]);
                            if (v.Length > 2)
                                l = int.Parse(v[2]);
                            if (v.Length > 3)
                                r = int.Parse(v[3]);
                        }
                        catch { }
                    }
                    _RectangleBorder = new RectangleBorder(t, b, l, r);
                    
                }
                _Xml.SetAttribute("BoderStyle", _RectangleBorder.ToString());
                return _RectangleBorder;
            }
            set { _Xml.SetAttribute("BoderStyle", value.ToString()); }
        }
        [Description("对齐方式"), Category("格式")]
        public System.Drawing.ContentAlignment Alignment
        {
            get { return clsXMLTools.GetAlign(_Xml, "Alignment"); }
            set { _Xml.SetAttribute("Alignment", value.ToString()); }
        }
        [Description("字体"), Category("数据")]
        public System.Drawing.Font Font
        {
            get
            {
                XmlElement xml = _Xml.SelectSingleNode("Font") as XmlElement;
                if (xml == null)
                {
                    xml = _Xml.OwnerDocument.CreateElement("Font");
                    _Xml.AppendChild(xml);
                    System.Drawing.Font font = new System.Drawing.Font("SimSun", 9);
                    xml.SetAttribute("Name", font.Name);
                    xml.SetAttribute("Size", font.Size.ToString());
                    xml.SetAttribute("Bold", font.Bold.ToString());
                    xml.SetAttribute("Italic", font.Italic.ToString());
                    xml.SetAttribute("Strikeout", font.Strikeout.ToString());
                    xml.SetAttribute("Underline", font.Underline.ToString());
                    return font;
                }
                else
                {
                    string name = xml.GetAttribute("Name");
                    float size = clsXMLTools.GetFloat(xml, "Size",9);
                    bool Bold = clsXMLTools.GetBool(xml, "Bold");
                    bool Italic = clsXMLTools.GetBool(xml, "Italic");
                    bool Strikeout = clsXMLTools.GetBool(xml, "Strikeout");
                    bool Underline = clsXMLTools.GetBool(xml, "Underline");
                    System.Drawing.FontStyle style= System.Drawing.FontStyle.Regular;
                    style = Bold ? style | System.Drawing.FontStyle.Bold : style;
                    style = Italic ? style | System.Drawing.FontStyle.Italic : style;
                    style = Strikeout ? style | System.Drawing.FontStyle.Strikeout : style;
                    style = Underline ? style | System.Drawing.FontStyle.Underline : style;

                    return new System.Drawing.Font(name, size, style);
                }
            }
            set
            {
                XmlElement xml = _Xml.SelectSingleNode("Font") as XmlElement;
                if (xml == null)
                {
                    xml = _Xml.OwnerDocument.CreateElement("Font");
                    _Xml.AppendChild(xml);
                }
                xml.SetAttribute("Name", value.Name);
                xml.SetAttribute("Size", value.Size.ToString());
                xml.SetAttribute("Bold", value.Bold.ToString());
                xml.SetAttribute("Italic", value.Italic.ToString());
                xml.SetAttribute("Strikeout", value.Strikeout.ToString());
                xml.SetAttribute("Underline", value.Underline.ToString());
            }
        }
    }

    public class clsXMLTools
    {
        public static int GetInt(XmlElement xml, string key, int defaultValue)
        {
            string v = xml.GetAttribute(key);
            if (v == null || v.Length == 0)
                return defaultValue;
            else
            {
                try
                {
                    return int.Parse(v);
                }
                catch
                {
                    return defaultValue;
                }
            }
        }
        public static int GetInt(XmlElement xml, string key)
        {
            return GetInt(xml, key, 0);
        }
        public static float GetFloat(XmlElement xml, string key, float defaultValue)
        {
            string v = xml.GetAttribute(key);
            if (v == null || v.Length == 0)
                return defaultValue;
            else
            {
                try
                {
                    return float.Parse(v);
                }
                catch
                {
                    return defaultValue;
                }
            }
        }
        public static float GetFloat(XmlElement xml, string key)
        {
            return GetFloat(xml, key, 0);
        }
        public static bool GetBool(XmlElement xml, string key, bool defaultValue)
        {
            string v = xml.GetAttribute(key);
            if (v == null || v.Length==0)
                return defaultValue;
            else
            {
                try
                {
                    return bool.Parse(v);
                }
                catch
                {
                    return defaultValue;
                }
            }
        }
        public static bool GetBool(XmlElement xml, string key)
        {
            return GetBool(xml, key, false);
        }
        public static System.Drawing.ContentAlignment GetAlign(XmlElement xml, string key, System.Drawing.ContentAlignment defaultValue)
        {
            string v = xml.GetAttribute(key);
            if (v == null || v.Length == 0)
                return defaultValue;
            else
            {
                try
                {
                    switch (v)
                    {
                        case "BottomLeft":
                            return System.Drawing.ContentAlignment.BottomLeft;
                        case "BottomCenter":
                            return System.Drawing.ContentAlignment.BottomCenter;
                        case "BottomRight":
                            return System.Drawing.ContentAlignment.BottomRight;
                        case "MiddleCenter":
                            return System.Drawing.ContentAlignment.MiddleCenter;
                        case "MiddleLeft":
                            return System.Drawing.ContentAlignment.MiddleLeft;
                        case "MiddleRight":
                            return System.Drawing.ContentAlignment.MiddleRight;
                        case "TopCenter":
                            return System.Drawing.ContentAlignment.TopCenter;
                        case "TopRight":
                            return System.Drawing.ContentAlignment.TopRight;
                        default:
                            return System.Drawing.ContentAlignment.TopLeft;
                    }
                }
                catch
                {
                    return defaultValue;
                }
            }
        }
        public static System.Drawing.ContentAlignment GetAlign(XmlElement xml, string key)
        {
            return GetAlign(xml, key, System.Drawing.ContentAlignment.TopLeft);
        }
    }
    public class RectangleBorderConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(RectangleBorder))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType==typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value.GetType() == typeof(RectangleBorder))
            {
                RectangleBorder rb = (RectangleBorder)value;
                return string.Format("{0},{1},{2},{3}", rb.Top, rb.Bottom, rb.Left, rb.Right);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string v = (string)value;
                string[] s = v.Split(',');
                return new RectangleBorder(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]));
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
    [TypeConverterAttribute(typeof(RectangleBorderConverter))]
    public class RectangleBorder
    {
        private int _Top;
        private int _Bottom;
        private int _Left;
        private int _Right;

        public int Top
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Bottom
        {
            get { return _Bottom; }
            set { _Bottom = value; }
        }
        public int Left
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Right
        {
            get { return _Right; }
            set { _Right = value; }
        }
        public RectangleBorder(int top, int bottom, int left, int right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", Top, Bottom, Left, Right);
        }
    }
    public class FormatConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new String[] { "{0:#,##0.00}", "{0:#,##0}", "{0:yyyy-MM-dd}", "{0:hh:mm:ss}" });
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }
    public class TableFieldConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context.Instance.GetType() == typeof(clsPrintCell))
            {
                clsPrintCell cell = (clsPrintCell)context.Instance;
                string tableName = null;
                //StandardValuesCollection svc = new StandardValuesCollection();
                if (cell.Parent.DefaultTableName != null && cell.Parent.DefaultTableName.Length > 0)
                {
                    tableName = cell.Parent.DefaultTableName;
                }
                else if (cell.Parent.Parent.Parent.GetType()==typeof(clsPrintTable))
                {
                    clsPrintTable table = (clsPrintTable)cell.Parent.Parent.Parent;
                    tableName = table.TableName;
                }
                if (tableName != null)
                {
                    List<string> list = new List<string>();
                    try
                    {
                        COMFields fields = CSystem.Sys.Svr.LDI.GetFields(tableName);
                        foreach (COMField f in fields.Fields)
                            list.Add(f.FieldName);
                    }
                    catch { }
                    StandardValuesCollection svc = new StandardValuesCollection(list);
                    return svc;
                }
            }
            else if (context.Instance.GetType() == typeof(clsPrintMainBody) || context.Instance.GetType() == typeof(clsPrintRange)||context.Instance.GetType() == typeof(clsPrintTable))
            {
                List<string> list = new List<string>();
                foreach (CTableProperty prop in CSystem.Sys.Svr.Properties.Tables)
                    list.Add(prop.TableName);
                StandardValuesCollection svc = new StandardValuesCollection(list);
                return svc;
            }
            return base.GetStandardValues(context);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}