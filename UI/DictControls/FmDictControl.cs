using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using Base;

namespace UI
{
    public class FmDictControl : ICustomControlType
    {
        #region ICustomControlType Members
        String _Name;
        public string Name
        {
            get { return _Name; }
        }
        public string ControlTypeName
        {
            get { return "FmDictControl"; }
        }
        public string DisplayName
        {
            get { return "×Öµä¿Ø¼þ"; }
        }
        public System.Windows.Forms.Control CreateRealControl(string fieldName)
        {
            return new DictControl();
        }
        private DataTable _Source=null;
        public DataTable getSource
        {
            get { return _Source;}
        }
        public void SetDataBinding(Control control, COMField field, string fieldName)
        {
            //((FiCustomDictControl)control).CustomControl.OnlyBindID  = true;
            ((DictControl)control).CustomControl.DataBindings.Clear();
            String sql = "select code,name from " + field.RightTableName + " order by code ";
            DataSet ds = new DataSet();
            ds = CSystem.Sys.Svr.cntMain.Select(sql);
            _Name = fieldName;
            _Source = ds.Tables[0];
            ((DictControl)control).CustomControl.DataSource = _Source;
            ((DictControl)control).CustomControl.Tag = _Source;
            //((DictControl)control).CustomControl.DataBindings.Add("EntityId", dataSource, fieldName);
            //((FiCustomDictControl)control).DataBindings.Add("EntityNode", dataSource, fieldName);
            //((FiCustomDictControl)control).DataBindings.Add("EntityName", dataSource, fieldName);
        }
        public void SetControlChangedEventHandler(System.Windows.Forms.Control control, EventHandler handler)
        {
            ((DictControl)control).CustomControl.ValueChanged += handler;
        }
        #endregion
    }
}
