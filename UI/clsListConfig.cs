using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace UI
{
    public class clsListConfig
    {
        private string _FileName;
        private System.Xml.XmlDocument doc;
        private XmlNode _Root;
        private XmlElement _Element;
        private XmlElement _EleConditions;
        private XmlElement _EleColumns;
        private SortedList<string, clsColumn> _Columns=new SortedList<string,clsColumn>();
        private SortedList<string, clsCondition> _Conditions=new SortedList<string,clsCondition>();

        public string Name
        {
            get { return _Element.GetAttribute("Name"); }
            set { _Element.SetAttribute("Name", value); }
        }
        public string SQL
        {
            get { return _Element.GetAttribute("SQL"); }
            set { _Element.SetAttribute("SQL", value); }
        }
        public string OrderBy
        {
            get { return _Element.GetAttribute("OrderBy"); }
            set { _Element.SetAttribute("OrderBy", value); }
        }
        public string FileName
        {
            set { _FileName = value; }
            get { return _FileName; }
        }
        public clsListConfig(string FileName,bool IsNew)
        {
            _FileName = FileName;
            doc = new System.Xml.XmlDocument();
            if (IsNew)
            {
                
                _Element = doc.CreateElement("ListConfig");
                _Root = (XmlNode)_Element;
                doc.AppendChild(_Root);
                _EleColumns = doc.CreateElement("Columns");
                _Root.AppendChild(_EleColumns);
                _EleConditions = doc.CreateElement("Conditions");
                _Root.AppendChild(_EleConditions);
            }
            else
            {
                doc.Load(_FileName);
                _Root = doc.SelectSingleNode("ListConfig");
                _Element = (XmlElement)_Root;
                _EleColumns = (XmlElement)_Root.SelectSingleNode("Columns");
                XmlNodeList nodes = _EleColumns.SelectNodes("Column");
                foreach (XmlNode node in nodes)
                {
                    XmlElement xml = (XmlElement)node;
                    clsColumn col = new clsColumn(xml);
                    _Columns.Add(col.FieldName, col);
                }
                _EleConditions = (XmlElement)_Root.SelectSingleNode("Conditions");
                nodes = _EleConditions.SelectNodes("Condition");
                foreach (XmlNode node in nodes)
                {
                    XmlElement xml = (XmlElement)node;
                    clsCondition con = new clsCondition(xml);
                    _Conditions.Add(con.Key, con);
                }
            }
        }
        public SortedList<string, clsCondition> Conditions
        {
            get { return _Conditions; }
        }
        public SortedList<string, clsColumn> Columns
        {
            get { return _Columns; }
        }
        public clsCondition NewCondition(string Key)
        {
            XmlElement xml = doc.CreateElement("Condition");
            xml.SetAttribute("Key", Key);
            _EleConditions.AppendChild((XmlNode)xml);
            clsCondition con = new clsCondition(xml);
            _Conditions.Add(Key, con);
            return con;
        }
        public clsColumn NewColumn(string FieldName)
        {
            XmlElement xml = doc.CreateElement("Column");
            xml.SetAttribute("FieldName", FieldName);
            _EleColumns.AppendChild((XmlNode)xml);
            clsColumn col = new clsColumn(xml);
            _Columns.Add(FieldName, col);
            return col;
        }
        public bool Save()
        {
            if (_FileName != null & _FileName.Length > 0)
            {
                doc.Save(_FileName);
                return true;
            }
            else
                return false;
        }
    }
    public class clsColumn
    {
        private XmlElement _Element;

        public clsColumn(XmlElement element)
        {
            _Element = element;
        }
        public string FieldName
        {
            get { return _Element.GetAttribute("FieldName"); }
            set { _Element.SetAttribute("FieldName",value); }
        }
        public int LineNumber
        {
            get { return int.Parse(_Element.GetAttribute("LineNumber")); }
            set { _Element.SetAttribute("LineNumber", value.ToString()); }
        }
        public string OldDescription
        {
            get { return _Element.GetAttribute("OldDescription"); }
            set { _Element.SetAttribute("OldDescription", value); }
        }
        public string NewDescription
        {
            get { return _Element.GetAttribute("NewDescription"); }
            set { _Element.SetAttribute("NewDescription", value); }
        }
        public int Width
        {
            get
            {
                try { return int.Parse(_Element.GetAttribute("Width")); }
                catch
                {
                    return 0;
                }
            }
            set { _Element.SetAttribute("Width", value.ToString()); }
        }
        public bool Hidden
        {
            get
            {
                try { return bool.Parse(_Element.GetAttribute("Hidden")); }
                catch
                {
                    return false;
                }
            }
            set { _Element.SetAttribute("Hidden", value.ToString()); }
        }

        /// <summary>
        /// -1 ½µÐò;0 ²»±¨Ðò;1 ÉýÐò
        /// </summary>
        public int OrderBy
        {
            get
            {
                try { return int.Parse(_Element.GetAttribute("OrderBy")); }
                catch
                {
                    return 0;
                }
            }
            set { _Element.SetAttribute("OrderBy", value.ToString()); }
        }
    }
    public class clsCondition
    {
        private XmlElement _Element;
        public clsCondition(XmlElement element)
        {
            _Element = element;
        }
        
        public string Key
        {
            get { return _Element.GetAttribute("Key"); }
            set { _Element.SetAttribute("Key", value); }
        }
        public SortedList<string, string> Values
        {
            get
            {
                SortedList<string, string> sl = new SortedList<string, string>();
                XmlNodeList nodes = _Element.SelectNodes("Value");
                foreach (XmlNode node in nodes)
                {
                    XmlElement xml = (XmlElement)node;
                    sl.Add(xml.GetAttribute("Name"), xml.GetAttribute("Value"));
                }
                return sl;
            }
        }
        public void SetValue(string Name, string Value)
        {
            XmlNode node = _Element.SelectSingleNode(string.Format("Value[@Name='{0}']",Name));
            if (node == null)
            {
                XmlElement xml = _Element.OwnerDocument.CreateElement("Value");
                _Element.AppendChild(xml);
                xml.SetAttribute("Name", Name);
                xml.SetAttribute("Value", Value);
            }
            else
            {
                XmlElement xml = (XmlElement)node;
                xml.SetAttribute("Value", Value);
            }
        }
        public string GetValue(string Name)
        {
            XmlNode node = _Element.SelectSingleNode(string.Format("Value[@Name='{0}']", Name));
            if (node == null)
                return null;
            else
            {
                XmlElement xml = (XmlElement)node;
                return xml.GetAttribute("Value");
            }
        }
    }
}
