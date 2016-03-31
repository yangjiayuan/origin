using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace Base
{
    public class CRightItem
    {
        public delegate System.Windows.Forms.Form GetFormEventHandler(CRightItem right);
        public GetFormEventHandler GetFormHandler;
        private BaseServer mSvr;

        public string ID;
        public string Code;
        public string Name;
        public string PID;
        public bool IsCustom;
        public string Paramters;
        public bool NeedSeparator;
        public bool NeedRightControl;
        public string RightExpression;

        private string _ActionName;
        private CRight _Right;
        private IMenuAction _MenuAction;

        public CRightItem(BaseServer svr)
        {
            mSvr = svr;
        }
        public int RightValue
        {
            get
            {
                return mSvr.Operator.GetRightValue(this);
            }
        }
        public CRight Right
        {
            set { _Right = value; }
        }
        public string ActionName
        {
            set { _ActionName = value; }
            get { return _ActionName; }
        }
        public bool HasAction
        {
            get { return _ActionName != null && _ActionName.Length > 0; }
        }
        //public IMenuAction MenuAction
        //{
        //    set { _MenuAction = value; }
        //    get { return _MenuAction; }
        //}
        
        public System.Windows.Forms.Form GetForm(System.Windows.Forms.Form mdiForm)
        {
            if (GetFormHandler != null)
            {
                return GetFormHandler(this);
            }
            if (_MenuAction != null)
                return _MenuAction.GetForm(this, mdiForm);
            else if (_ActionName!=null && _ActionName.Length>0)
            {
                string[] a = _ActionName.Split(new char[] { '!' });
                if (a.Length >= 2)
                {
                    Assembly ass = _Right.GetAssembly(a[0]);
                    try
                    {
                        _MenuAction = ass.CreateInstance(a[1]) as IMenuAction;
                        if (_MenuAction == null)
                            Msg.Warning(string.Format("菜单项({1})创建对象({0})不成功！", a[1], this.Name));
                        else
                            return _MenuAction.GetForm(this, mdiForm);
                    }
                    catch (Exception ex)
                    {
                        Msg.Warning(string.Format("菜单项({1})创建对象({0})失败！\n {2}", a[1], this.Name,ex.Message));
                    }
                }
            }
            return null;
        }
    }


    public class CRight
    {
        private SortedList<string,CRightItem> moRight;
        private DBConnection mCnt;
        private BaseServer mSvr;
        private string msTableName;
        private SortedList<string, Assembly> DLLFiles;

        public void Initialization(BaseServer Svr, DBConnection cntDB)
        {
            mSvr = Svr;
            mCnt = cntDB;
            msTableName = "P_Right";
            moRight=new SortedList<string,CRightItem>();

            this.Load();
        }
        public Assembly GetAssembly(string name)
        {
            Assembly ass = null;
            if (DLLFiles.ContainsKey(name))
                ass = DLLFiles[name];
            else
            {
                string fileName = name;
                System.IO.FileInfo f = new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + fileName);
                if (f.Exists)
                {
                    try
                    {
                        ass = Assembly.LoadFile(f.FullName);
                        if (ass != null)
                        {
                            DLLFiles.Add(name, ass);
                        }
                        else
                        {
                            Msg.Warning(string.Format("定制库({0})装载失败！", name));
                        }
                    }
                    catch (Exception ex)
                    {
                        Msg.Warning(string.Format("定制库({0})装载失败！/r/d{1}", name, ex.ToString()));
                    }
                }
                else
                {
                    Msg.Warning(string.Format("定制库({0})不存在！", name));
                }
            }
            return ass;
        }

        private void Load()
        {
            //string WhereClause = string.Format("");
            string SQL = mSvr.LDI.GetFields(msTableName).QuerySQL;
            DataSet ds = mSvr.cntMain.Select(SQL, msTableName);
            DataTable dt = ds.Tables[msTableName];
            CRightItem xRightItem;
            DLLFiles = new SortedList<string, Assembly>();

            foreach (DataRow dr in dt.Rows)
            {
                string SKey = (string)dr["ID"];
                SKey= SKey.Trim();
                xRightItem  = new CRightItem(mSvr);

                xRightItem.ID = SKey;
                xRightItem.Code = (string)dr["Code"];
                xRightItem.Name = (string)dr["Name"];
                xRightItem.PID = dr["PID"].ToString().Trim();
                xRightItem.IsCustom = (int)dr["IsCustom"] == 1;
                xRightItem.NeedSeparator = (int)dr["NeedSeparator"] == 1;
                xRightItem.Paramters = dr["Parameters"] as string;
                xRightItem.ActionName = dr["Action"] as string;
                xRightItem.Right = this;
                xRightItem.RightExpression = dr["RightExpression"] as string;
                xRightItem.NeedRightControl = (int)dr["NeedRightControl"] == 1;
                
                if (moRight.ContainsKey(SKey))
                {
                    Msg.Warning("菜单项重复:"+SKey);
                }
                else
                    moRight.Add(SKey, xRightItem);
            }
        }

        public IList<CRightItem> Rights
        {
            get
            {
                List<CRightItem> T  = new List<CRightItem>();
                foreach (CRightItem RI in moRight.Values)
                    T.Add(RI);
                T.Sort(new RightCompare());
                return T;
            }
        }

        public CRightItem this[string RightID]
        {
            get
            {
                return moRight[RightID];
            }
        }

        public System.Windows.Forms.Form GetForm(string code)
        {
            CRightItem right = null;
            foreach (KeyValuePair<string, CRightItem> kv in moRight)
            {
                if (kv.Value.Code == code)
                {
                    if (kv.Value.RightValue != 0)
                        return kv.Value.GetForm(null);
                    else
                        right = kv.Value;
                }
            }
            if (right != null)
                return right.GetForm(null);
            else
                return null;
        }
        public System.Windows.Forms.Form GetForm(string action,string parameters)
        {
            CRightItem right = null;
            foreach (KeyValuePair<string, CRightItem> kv in moRight)
            {
                if (kv.Value.ActionName != null && kv.Value.ActionName.Length > 0)
                {
                    string className = kv.Value.ActionName.Substring(kv.Value.ActionName.LastIndexOf('.') + 1);
                    if (className == action && (kv.Value.Paramters==parameters ||((kv.Value.Paramters==null || kv.Value.Paramters.Length==0) && (parameters==null || parameters.Length==0))))
                    {
                        if (kv.Value.RightValue != 0)
                            return kv.Value.GetForm(null);
                        else
                            right = kv.Value;
                    }
                }
            }
            return right.GetForm(null);
        }
        public CRightItem GetRight(string action, string parameters)
        {
            CRightItem right = null;
            foreach (KeyValuePair<string, CRightItem> kv in moRight)
            {
                if (kv.Value.ActionName != null && kv.Value.ActionName.Length > 0)
                {
                    string className = kv.Value.ActionName.Substring(kv.Value.ActionName.LastIndexOf('.') + 1);
                    if (className == action && (kv.Value.Paramters == parameters || ((kv.Value.Paramters == null || kv.Value.Paramters.Length == 0) && (parameters == null || parameters.Length == 0))))
                    {
                        if (kv.Value.RightValue != 0)
                            return kv.Value;
                        else
                            right = kv.Value;
                    }
                }
            }
            return right;
        }
        private class RightCompare : IComparer<CRightItem>
        {
            #region IComparer Members

            public int Compare(CRightItem x, CRightItem y)
            {
                return x.ID.CompareTo(y.ID);
            }

            #endregion
        }
    }


}
