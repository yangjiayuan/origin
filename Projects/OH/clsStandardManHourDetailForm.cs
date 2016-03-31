using System;
using System.Collections.Generic;
using System.Text;
using UI;

namespace OH
{
    public class clsStandardManHourDetailForm:ToolDetailForm
    {
        public override string BeforeSaving(System.Data.DataSet ds, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction tran)
        {
            string result = null;
            DateTime billDate = (DateTime)ds.Tables[_DetailForm.MainTable].Rows[0]["BillDate"];
            Guid id = (Guid)ds.Tables[_DetailForm.MainTable].Rows[0]["ID"];
            System.Data.DataSet data = CSystem.Sys.Svr.cntMain.Select(string.Format("Select Count(*) from D_StandardManHour where BillDate='{0}' and ID<>'{1}'", billDate, id), conn, tran);
            if ((int)data.Tables[0].Rows[0][0] > 0)
            {
                result = "不能重复输入相同时间的工时标准！";
            }

            return result;
        }
        public override bool AutoCode
        {
            get
            {
                return true;
            }
            set
            {
                base.AutoCode = value;
            }
        }
    }
}
