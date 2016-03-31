using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace Base
{   
    public class CProperties
    {
        bool mbInited;
        DBConnection mCnt;
        string msTableName;
        BaseServer mSvr;

        SortedList<string, CTableProperty> mTableCollection;

        public CProperties()
        {
            mTableCollection = new SortedList<string, CTableProperty>();
        }

        public void Init(BaseServer Server, DBConnection cntDB, String sTableName)
        {
            mSvr = Server;
            mCnt = cntDB;
            msTableName = sTableName;
            mbInited = true;
        }
        public void ReadFromDB()
        {
            string SQL;
            CTableProperty xTC;
           

            SQL = string.Format("Select * From {0}", msTableName);
            DataSet ds = mCnt.Select(SQL);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string SKey = (string)dr["ID"];
                xTC = new CTableProperty();
                xTC.ID = (string)dr["ID"];
                xTC.TableName = (string)dr["TableName"];
                xTC.Title = (string)dr["Title"];
                xTC.AutoLoad = (Int32)dr["AutoLoad"]==1?true:false;
                xTC.Visible = (Int32)dr["Visible"]==1?true:false;
                
                if (Convert.IsDBNull(dr["ParentTableName"]) == true)
                    xTC.ParentTableName = "";
                else
                    xTC.ParentTableName = (string)dr["ParentTableName"];

                xTC.TableType = (CTableProperty.enuTableType)dr["TableType"];

                if (Convert.IsDBNull(dr["OrderBy"]))
                    xTC.OrderBy=0;
                else
                    xTC.OrderBy=(int)dr["OrderBy"];
                if (Convert.IsDBNull(dr["GroupBy"]))
                    xTC.GroupBy=0;
                else
                    xTC.GroupBy=(int)dr["GroupBy"];
                if (ds.Tables[0].Columns.Contains("HistoryTable"))
                    xTC.HistoryTable = (int)dr["HistoryTable"] == 1;
                else
                    xTC.HistoryTable = false;

                mTableCollection.Add(SKey, xTC);

            }
        }
        public IList<CTableProperty> Tables
        {
            get 
            {
                List<CTableProperty> Result=new List<CTableProperty>();
                foreach (CTableProperty CT in mTableCollection.Values)
                    Result.Add(CT);
                Result.Sort(new PropertyCompare());
                return Result;
            }

          
        }
        private class PropertyCompare : IComparer<CTableProperty>
        {

            #region IComparer<CTableProperty> Members

            public int Compare(CTableProperty x, CTableProperty y)
            {
                if (x.GroupBy == y.GroupBy)
                    return x.OrderBy.CompareTo(y.OrderBy);
                else
                    return x.GroupBy.CompareTo(y.GroupBy);
            }

            #endregion
        }
        public CTableProperty this[string Key]
        {
            get
            {
                return mTableCollection[Key];
            }
        }
        public int Count
        {
            get
            {
                return mTableCollection.Count;
            }
        }
        public bool ContainsTable(string TableName) 
        {
            return mTableCollection.ContainsKey(TableName);
        }
        public List<string> DetailTableList(string ParentTableName)
        {
            List<CTableProperty> ResultTable = new List<CTableProperty>();
            foreach (CTableProperty TP in Tables)
                if (TP.ParentTableName == ParentTableName)
                    ResultTable.Add(TP);
            ResultTable.Sort(new PropertyCompare());

            List<string> ReturnList = new List<string>();
            foreach (CTableProperty table in ResultTable)
                ReturnList.Add(table.TableName);
            return ReturnList;
        }
        public List<COMFields> DetailTableDefineList(string ParentTableName)
        {
            List<CTableProperty> ResultTable=new List<CTableProperty>();
            foreach (CTableProperty TP in Tables)
                if (TP.ParentTableName == ParentTableName)
                    ResultTable.Add(TP);
            ResultTable.Sort(new PropertyCompare());

            List<COMFields> ReturnList = new List<COMFields>();
            foreach (CTableProperty table in ResultTable)
                ReturnList.Add(mSvr.LDI.GetFields(table.TableName));
            return ReturnList;
        }
        public List<COMFields> DetailTableDefineListClone(string ParentTableName)
        {
            List<COMFields> ReturnList = new List<COMFields>();
            foreach (CTableProperty TP in Tables)
                if (TP.ParentTableName == ParentTableName)
                    ReturnList.Add(mSvr.LDI.GetFields(TP.TableName).Clone());
            return ReturnList;
        }
    }

}

