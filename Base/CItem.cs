using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Base
{
    public delegate void DataSetEventHandler(object sender, DataSetEventArgs e);
    public class DataSetEventArgs : EventArgs
    {
        private bool _HasNext;
        private bool _HasPrevious;
        private DataSet _ds;
        public DataSetEventArgs(DataSet ds, bool hasNext, bool hasPrevious)
        {
            _ds = ds;
            _HasNext = hasNext;
            _HasPrevious = hasPrevious;
        }
        public DataSetEventArgs(DataSet ds)
            : this(ds, false, false)
        {
        }
        public DataSet Data
        {
            get { return _ds; }
        }
        public bool HasNext
        {
            get { return _HasNext; }
            set { _HasNext = value; }
        }
        public bool HasPrevious
        {
            get { return _HasPrevious; }
            set { _HasPrevious = value; }
        }
    }
    public delegate void DataTableEventHandler(object sender, DataTableEventArgs e);
    public class DataTableEventArgs : EventArgs
    {
        private bool _HasNext;
        private bool _HasPrevious;
        private DataTable _dt;
        public DataTableEventArgs(DataTable dt, bool hasNext, bool hasPrevious)
        {
            _dt = dt;
            _HasNext = hasNext;
            _HasPrevious = hasPrevious;
        }
        public DataTableEventArgs(DataTable dt)
            : this(dt, false, false)
        {
        }
        public DataTable Data
        {
            get
            {
                return _dt;
            }
        }
        public bool HasNext
        {
            get { return _HasNext; }
            set { _HasNext = value; }
        }
        public bool HasPrevious
        {
            get { return _HasPrevious; }
            set { _HasPrevious = value; }
        }
    }

    public class IntItem
    {
        private int _Int;
        private string _Name;
        public IntItem(int i, string name)
        {
            _Int = i;
            _Name = name;
        }
        public int Int
        {
            get { return _Int; }
            set { _Int = value; }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public override string ToString()
        {
            return _Name;
        }
    }
    public class DataItem
    {
        public enum ShowStyle { Code, Name, All };
        private string _Name;
        private string _Code;
        private Guid _ID;
        private object _Tag;
        private ShowStyle _ShowStyle = ShowStyle.Name;
        public DataItem(string code, string name)
        {
            _Name = name;
            _Code = code;
        }
        public DataItem(Guid id, string code, string name)
        {
            _ID = id;
            _Name = name;
            _Code = code;
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }
        public Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public object Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }
        public ShowStyle ItemShowStyle
        {
            set { _ShowStyle = value; }
        }
        public override string ToString()
        {
            switch (_ShowStyle)
            {
                case DataItem.ShowStyle.Name:
                    return _Name;
                case DataItem.ShowStyle.Code:
                    return _Code;
                case DataItem.ShowStyle.All:
                    return _Code + "-" + _Name;
            }
            return _Name;
        }
    }
}
