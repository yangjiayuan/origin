using System.Collections;
using System;
using UI;
using System.Data;
namespace YUANYE
{
    public class clsMaterial : ToolDetailForm
    {
        public override string BeforeSaving(DataSet ds, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction tran)
        {
            string s = null;
            Hashtable hs = new System.Collections.Hashtable(); 
            foreach (DataRow dr in ds.Tables[this._DetailForm.DetailTableDefine[0].OrinalTableName].Rows)
            {
                string tmp = (string)dr["CustomerName"];
                Guid _CustomerID = (Guid)dr["CustomerID"];
                if (hs.ContainsKey(_CustomerID))
                    s = s + tmp + " ";
                else
                    hs[_CustomerID] = tmp;
            }

            if (s != null)
                s = s + "的客户重复";
            return s;
        }
    }
}
