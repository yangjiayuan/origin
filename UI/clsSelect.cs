using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace UI
{
    public class clsSelect
    {
        private int _PageIndex = 1;
        private int _PageSize = 200;
        private bool _EOF = false;
        private string _FieldList;
        private string _TableName;
        private string _From1;
        private string _From2;
        private string _Where;
        private string _WhereOther;
        private string _WhereFilter;
        private string _PrimaryKey;
        private string _Order;
        private bool _IsHistory = false;
        public clsSelect(string tableName,string fieldList, string from1, string from2, string where, string primaryKey,string order)
        {
            _TableName = tableName;
            _FieldList = fieldList;
            _From1 = from1;
            _From2 = from2;
            _Where = where;
            _Order = order;
            _PrimaryKey = primaryKey;
        }
        public bool IsHistory
        {
            get { return _IsHistory; }
            set { _IsHistory = value; }
        }
        public int PageSize
        {
            get { return _PageSize; }
            set { _PageSize = value; }
        }
        public string Where
        {
            get { return _Where; }
            set { _Where = value; }
        }
        public string WhereOther
        {
            get { return _WhereOther; }
            set { _WhereOther = value; }
        }
        public string WhereFilter
        {
            get { return _WhereFilter; }
            set
            {
                if (_WhereFilter != value)
                {
                    _PageIndex = 1;
                    _WhereFilter = value;
                }
            }
        }
        public string OrderBy
        {
            get { return _Order; }
            set { _Order = value; }
        }
        public string SelectSql()
        {
            string w = "";
            if (_Where != null && _Where.Length > 0)
                w = w + " and " + _Where;
            if (_WhereOther != null && _WhereOther.Length > 0)
                w = w + " and " + _WhereOther;
            if (_WhereFilter != null && _WhereFilter.Length > 0)
                w = w + " and " + _WhereFilter;
            if (w.Length == 0)
                return string.Format("Select {0} from {1}", _FieldList, _IsHistory ? _From2 : _From1);
            else
                return string.Format("Select {0} from {1} where {2}", _FieldList, _IsHistory ? _From2 : _From1, w.Substring(4));
        }
        private string WhereSQL
        {
            get
            {
                string w = "";
                if (_Where != null && _Where.Length > 0)
                    w = w + " and " + _Where;
                if (_WhereOther != null && _WhereOther.Length > 0)
                    w = w + " and " + _WhereOther;
                if (_WhereFilter != null && _WhereFilter.Length > 0)
                    w = w + " and " + _WhereFilter;
                if (w.Length>0)
                    return w.Substring(5);
                else
                    return w;
            }
        }
        public DataSet RegetPageData()
        {
            _EOF = false;
            _PageIndex = 1;
            return NextPageData();
        }
        public DataSet NextPageData()
        {
            if (!_EOF)
            {
                string where = WhereSQL;
                if (where != null && where.Length > 0)
                    where = " WHERE " + where;
                string sql = string.Format("SELECT TOP {0} * FROM ( SELECT ROW_NUMBER() OVER (ORDER BY {5}) AS RowNumber,{2} FROM {3} {4}) A WHERE RowNumber > {0}*({1}-1)", _PageSize, _PageIndex, _FieldList, _IsHistory ? _From2 : _From1, where, _Order);
                DataSet ds = CSystem.Sys.Svr.cntMain.Select(sql, _TableName);
                if (_PrimaryKey != null && _PrimaryKey.Length > 0)
                {
                    try
                    {
                        if (ds.Tables[_TableName].Columns.Contains(_PrimaryKey))
                            ds.Tables[_TableName].PrimaryKey = new DataColumn[] { ds.Tables[_TableName].Columns[_PrimaryKey] };
                    }
                    catch { }
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    _PageIndex++;

                    _EOF = ds.Tables[0].Rows.Count < _PageSize; 
                }
                else
                    _EOF = true;
                //DataSet ds = CSystem.Sys.Svr.cntMain.GetPageData(_TableName, _FieldList,_From, _PrimaryKey, Where, _Order, _PageSize, _PageIndex, ref _RecorderCount,ref _PageCount);
                //_PageIndex++;
                return ds;
            }
            return null;
        }
    }
}
