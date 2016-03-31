using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace Base
{
    public class CRelations
    {
        bool mbInited;
        DBConnection mCnt;
        string msTableName;
        BaseServer mSvr;

        SortedList<string, CRelation> mRelationCollection;

        public CRelations()
        {
            mRelationCollection = new SortedList<string, CRelation>();
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
            CRelation xRel;


            SQL = string.Format("Select * From {0}", msTableName);
            DataSet ds = mCnt.Select(SQL);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string SKey = (string)dr["ID"];
                xRel = new CRelation();
                xRel.RelationName = SKey;
                xRel.LeftField = (string)dr["LeftField"];
                xRel.LeftTable = (string)dr["LeftTable"];
                xRel.RightField = (string)dr["RightField"];
                xRel.RightTable = (string)dr["RightTable"];
                if (Convert.IsDBNull(dr["Jointype"]) == true)
                    xRel.JoinType = "Inner Join";
                else
                {
                    int IJoinType = (int)dr["JoinType"];
                    xRel.JoinType = IJoinType != 0 ? "Left Join" : "Inner Join";
                }
                mRelationCollection.Add(SKey, xRel);
            }
        }
        public IList<CRelation> Relations
        {
            get
            {
                List<CRelation> Result = new List<CRelation>();
                foreach (CRelation CT in mRelationCollection.Values)
                    Result.Add(CT);
                return Result;
            }


        }
        public CRelation this[string Key]
        {
            get
            {
                return mRelationCollection[Key];
            }
        }
        public int Count()
        {
            return mRelationCollection.Count;

        }
        public bool ContainsRelation(string RelationName)
        {
            return mRelationCollection.ContainsKey(RelationName);
        }
        public string GetRelationFrom(string relationPath, int startIndex,out string LeftTable,out string RightTable,out string LeftField,out string RightField)
        {
            LeftTable = null;
            LeftField = null;
            RightTable = null;
            RightField = null;
            string result = "";
            string[] path = relationPath.Split('.');
            if (startIndex == path.Length)
            {
                if (mRelationCollection.ContainsKey(path[path.Length - 1]))
                {
                    result = mRelationCollection[path[path.Length - 1]].RightTable;
                    LeftTable = mRelationCollection[path[path.Length - 1]].RightTable;
                    LeftField = mRelationCollection[path[path.Length - 1]].LeftField;
                    RightField = mRelationCollection[path[path.Length - 1]].RightField;
                    return result;
                }
                else
                    throw new Exception(string.Format("关系路径[{0}]的[{1}]不存在！", relationPath, path[path.Length - 1]));
            }
            if (mRelationCollection.ContainsKey(path[startIndex - 1]))
            {
                LeftField = mRelationCollection[path[startIndex-1]].LeftField;
                RightField = mRelationCollection[path[startIndex-1]].RightField;

                RightTable = mRelationCollection[path[startIndex]].LeftTable;
                result = mRelationCollection[path[startIndex]].LeftTable;
                LeftTable = RightTable;

            }
            else
                throw new Exception(string.Format("关系路径[{0}]的[{1}]不存在！", relationPath, path[startIndex-1]));
            
            for (int i = startIndex; i < path.Length; i++)
            {
                if (!mSvr.Relations.ContainsRelation(path[i]))
                    throw new Exception(string.Format("关系路径[{0}]的[{1}]不存在！", relationPath, path[i]));
                else
                {
                    CRelation Rel = mSvr.Relations[path[i]];
                    result += string.Format(" {0} {1} {2} On {3}.{4}={2}.{5}", Rel.JoinType, Rel.RightTable, Rel.RelationName, RightTable, Rel.LeftField, Rel.RightField);
                    RightTable = Rel.RelationName;
                }
            }
            return result;
        }

    }

}

