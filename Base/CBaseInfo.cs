using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Base
{
    public class CBaseInfo:SortedList<String,CBaseInfoValue>
    {
        private BaseServer mSvr;

        public CBaseInfo(BaseServer svr)
        {
            mSvr=svr;
            loadData();
        }
        private void loadData()
        {
            DataSet ds = mSvr.cntMain.Select("Select * from S_BaseInfo");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CBaseInfoValue baseInfoValue = new CBaseInfoValue(mSvr);
                baseInfoValue.ID = dr["ID"] as string;
                baseInfoValue.Name = dr["Name"] as string;
                baseInfoValue.Value = dr["Value"] as string;
                baseInfoValue.Enable = (int)dr["Enable"] == 1;
                baseInfoValue.Visible = (int)dr["Visible"] == 1;
                baseInfoValue.Notes = dr["Notes"] as string;
                baseInfoValue.ExtendInfo = dr["ExtendInfo"] as string;
                this.Add(dr["ID"] as string, baseInfoValue);
            }
        }
        public void Reload()
        {
            this.Clear();
            loadData();
        }
    }
    public class CBaseInfoValue
    {
        private BaseServer mSvr;
        public string ID;
        public string Name;
        public string Value;
        public bool Visible;
        public bool Enable;
        public string ValueType;
        public string Notes;
        public string ExtendInfo;
        public CBaseInfoValue(BaseServer svr)
        {
            mSvr = svr;
        }
        public string GetValue()
        {
            return Value;
        }
        public bool GetBoolean()
        {
            try
            {
                return bool.Parse(Value);
            }
            catch
            {
                return false;
            }
        }
        public int GetInteger()
        {
            try
            {
                return int.Parse(Value);
            }
            catch
            {
                return 0;
            }
        }
        public decimal GetDecimal()
        {
            try
            {
                return decimal.Parse(Value);
            }
            catch
            {
                return new decimal(0);
            }
        }
        public string GetDictCode()
        {
            try
            {
                if (ValueType.StartsWith("Dict"))
                {
                    string[] d = ValueType.Split(':');
                    if (d.Length == 2)
                    {
                        Guid v = new Guid(Value);
                        string sql = string.Format("Select Code,Name where {0} where ID='{1}'", d[1], v);
                        DataSet ds = mSvr.cntMain.Select(sql);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            return ds.Tables[0].Rows[0]["Code"] as string;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
        public string GetDictName()
        {
            try
            {
                if (ValueType.StartsWith("Dict"))
                {
                    string[] d = ValueType.Split(':');
                    if (d.Length == 2)
                    {
                        Guid v = new Guid(Value);
                        string sql = string.Format("Select Code,Name where {0} where ID='{1}'", d[1], v);
                        DataSet ds = mSvr.cntMain.Select(sql);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            return ds.Tables[0].Rows[0]["Name"] as string;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
    }
}
