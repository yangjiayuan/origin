using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Base
{
    public class CCodeRules:SortedList<string,List<CCodeRule>>
    {
        private BaseServer mSvr;
        public CCodeRules(BaseServer svr)
        {
            mSvr = svr;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                DataSet ds = mSvr.cntMain.Select("Select * from S_CodeRule order by TableName,OrderNumber");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    CCodeRule codeRule = new CCodeRule();
                    codeRule.TableName = dr["TableName"] as string;
                    codeRule.DataTableName = dr["DataTableName"] as string;
                    codeRule.OrderNumber = (int)dr["OrderNumber"];
                    codeRule.SequenceExpression = (string)dr["SequenceExpression"];
                    codeRule.GroupSQLExpression = (string)dr["GroupSQLExpression"];
                    codeRule.GroupExpression = dr["GroupExpression"] as string;
                    codeRule.GroupParameters = ((string)dr["GroupParameters"]).Split(',');
                    codeRule.CodeField = (string)dr["CodeField"];
                    codeRule.CodeExpression = dr["CodeExpression"] as string;
                    codeRule.CodeParameters = ((string)dr["CodeParameters"]).Split(',');
                    codeRule.SquenceLength = (int)dr["SquenceLength"];
                    codeRule.Roize = (int)dr["Roize"] == 1;
                    codeRule.InitialValue = (int)dr["InitialValue"];
                    codeRule.SequenceField = dr["SequenceField"] as string;
                    if (this.ContainsKey(codeRule.TableName))
                        this[codeRule.TableName].Add(codeRule);
                    else
                    {
                        List<CCodeRule> list = new List<CCodeRule>();
                        list.Add(codeRule);
                        this.Add(codeRule.TableName, list);
                    }
                }
            }
            catch { }
        }
    }
    public class CCodeRule
    {
        public string DataTableName;
        public string TableName;
        public int OrderNumber;
        public string SequenceExpression;
        public string GroupSQLExpression;
        public string GroupExpression;
        public string[] GroupParameters;
        public string CodeField;
        public string CodeExpression;
        public string[] CodeParameters;
        public int SquenceLength;
        public bool Roize;
        public int InitialValue;
        public string SequenceField;
    }
}
