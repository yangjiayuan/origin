using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Base
{
    public class CPreferences
    {
        private string _FileName;
        private bool _Loaded;
        private XmlDocument _Doc;
        private XmlNode _Root;
        public string GetValue(string key)
        {
            LoadData();
            XmlNode node = _Root.SelectSingleNode(key);
            if (node == null)
                return null;
            else
            {
                return ((XmlElement)node).GetAttribute("Value");
            }
        }
        public void SetValue(string key, string value)
        {
            LoadData();
            XmlNode node = _Root.SelectSingleNode(key);
            if (node == null)
            {
                XmlElement ele = _Doc.CreateElement(key);
                _Root.AppendChild(ele);
                ele.SetAttribute("Value", value);
            }
            else
            {
                XmlElement ele = node as XmlElement;
                ele.SetAttribute("Value", value);
            }
            
        }
        private void LoadData()
        {
            if (_Loaded)
                return;
            _Doc = new XmlDocument();
            _FileName = AppDomain.CurrentDomain.BaseDirectory + "Preferences.xml";
            if (File.Exists(_FileName))
            {
                _Doc.Load(_FileName);
                _Root = _Doc.SelectSingleNode("Preferences");
            }
            else
            {
                _Root = _Doc.CreateElement("Preferences") as XmlNode;
                _Doc.AppendChild(_Root);
            }
            _Loaded = true;
        }
        ~CPreferences()
        {
             if (_Loaded)
                _Doc.Save(_FileName);
        }
    }
}
