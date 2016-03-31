using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Base
{
    public class COperatorItem
    {
        public Guid ID;
        public string Code;
        public string Name;
        public Guid DepartmentID;
        public string DepartmentName;
        public bool Disabled;
        public Guid RoleID;
        public SortedList<string, int> moRight;
        public SortedList<string, string> moDataRight;
        public SortedList<string, CMenuRight> moMenuRight;

        public COperatorItem()
        {
            moRight = new SortedList<string, int>();
            moDataRight = new SortedList<string, string>();
            moMenuRight = new SortedList<string, CMenuRight>();
        }
    }

    public class CMenuRight
    {
        public string TableName;
        public bool AllowCreate;
        public bool AllowEdit;
        public bool AllowDelete;
        public bool AllowCheck;
    }


    public class COperator
    {
        private COperatorItem _LogonUser;
        private DBConnection mCnt;
        private BaseServer mSvr;
        private string msTableName;
        private string msRightTabelName;
        private string msDataRightName;
        private string msMenuRightName;


        public void Initialization(BaseServer Svr, DBConnection cntDB)
        {
            mSvr = Svr;
            mCnt = cntDB;
            msTableName = "P_User";
            msRightTabelName = "P_RoleRight";
            msDataRightName = "P_UserDataRight";
            msMenuRightName = "P_UserMenuRight";

            _LogonUser = new COperatorItem();
        }

        public COperatorItem LogonUser
        {
            get { return _LogonUser; }
        }
        public void Logon(Guid OperatorID, string Code, string Name, Guid DepartmentID, string DepartmentName, Guid RoleID)
        {

            _LogonUser.ID = OperatorID;
            _LogonUser.Code = Code;
            _LogonUser.Name = Name;
            _LogonUser.DepartmentID = DepartmentID;
            _LogonUser.DepartmentName = DepartmentName;
            _LogonUser.RoleID = RoleID;
            LoadOperatorRight(_LogonUser.RoleID, _LogonUser.moRight);
            LoadDataRight(_LogonUser.ID, _LogonUser.moDataRight);
            LoadMenuRight(LogonUser.ID, _LogonUser.moMenuRight);

        }
        /*public bool Logon(Guid OperatorID)
        {
            string WhereClause = string.Format("P_User.ID='{0}'", OperatorID);
            string SQL = mSvr.LDI.GetFields(msTableName).QuerySQLWithClause(WhereClause);
            DataSet ds = mSvr.cntMain.Select(SQL, msTableName);
            DataTable dt = ds.Tables[msTableName];


            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];


                _LogonUser.ID = OperatorID;
                _LogonUser.Code = (string)dr["Code"];
                _LogonUser.Name = (string)dr["Name"];
                if (dr["DepartmentID"]!=DBNull.Value)
                    _LogonUser.DepartmentID = (Guid)dr["DepartmentID"];
                if ((int)dr["Disable"] == 0)
                    _LogonUser.Disabled = false;
                else
                    _LogonUser.Disabled = true;
                if (dr["RoleID"].Equals(DBNull.Value) == true)
                    _LogonUser.RoleID = mSvr.NullID;
                else
                    _LogonUser.RoleID = (Guid)dr["RoleID"];

                LoadOperatorRight(_LogonUser.RoleID,_LogonUser.moRight);

            }
            return true;
        }*/

        private bool LoadOperatorRight(Guid RoleID, SortedList<string, int> moReturn)
        {
              

            string WhereClause = string.Format("MainID='{0}'", RoleID);
            string SQL = mSvr.LDI.GetFields(msRightTabelName).QuerySQLWithClause(WhereClause);
            DataSet ds = mSvr.cntMain.Select(SQL, msRightTabelName);
            DataTable dt = ds.Tables[msRightTabelName];

            foreach (DataRow DR in dt.Rows)
            {
                if (moReturn.ContainsKey((string)DR["RightID"]) == false)
                    moReturn.Add((string)DR["RightID"], (int)DR["RightValue"]);
            }

            return true;

        }

        private bool LoadMenuRight(Guid UserID, SortedList<string, CMenuRight> moReturn)
        {
            string WhereClause = string.Format("MainID='{0}'", UserID);
            string SQL = mSvr.LDI.GetFields(msMenuRightName).QuerySQLWithClause(WhereClause);
            DataSet ds = mSvr.cntMain.Select(SQL, msDataRightName);
            DataTable dt = ds.Tables[msDataRightName];

            foreach (DataRow DR in dt.Rows)
            {
                if (moReturn.ContainsKey((string)DR["TableName"]) == false)

                {
                    CMenuRight mr = new CMenuRight();
                    mr.TableName = (string)DR["TableName"];
                    mr.AllowCreate= (int)DR["New"] == 1;
                    mr.AllowDelete = (int)DR["DeleteRight"]==1;
                    mr.AllowEdit = (int)DR["Edit"]==1;
                    mr.AllowCheck = (int)DR["CheckRight"]==1;
                    moReturn.Add((string)DR["TableName"], mr);
                }
                    
            }

            return true;

        }

        public CMenuRight GetMenuRightValue(string TableName)
        {
            if (_LogonUser.RoleID == new Guid("{00000000-0000-0000-0000-000000000000}"))
                return null;
            else
            {
                if (LogonUser.moMenuRight.ContainsKey(TableName.Trim()))
                {
                    return LogonUser.moMenuRight[TableName.Trim()];
                }
                return null;
            }
        }

        private bool LoadDataRight(Guid UserID, SortedList<string, string> moReturn)
        {


            string WhereClause = string.Format("MainID='{0}'", UserID);
            string SQL = mSvr.LDI.GetFields(msDataRightName).QuerySQLWithClause(WhereClause);
            DataSet ds = mSvr.cntMain.Select(SQL, msDataRightName);
            DataTable dt = ds.Tables[msDataRightName];

            foreach (DataRow DR in dt.Rows)
            {
                if (moReturn.ContainsKey((string)DR["TableName"]) == false)
                    moReturn.Add((string)DR["TableName"],(string)DR["RightValue"]);
            }

            return true;

        }

        public string GetDataRightValue(string TableName)
        {
            if (_LogonUser.RoleID == new Guid("{00000000-0000-0000-0000-000000000000}"))
                return string.Empty;
            else
            {
                if (LogonUser.moDataRight.ContainsKey(TableName.Trim()))
                {
                    return LogonUser.moDataRight[TableName.Trim()];
                }
                return string.Empty;
            }
        }



        public bool DataRightControl(string TableName)
        {
            if (_LogonUser.RoleID == new Guid("{00000000-0000-0000-0000-000000000000}"))
                return false;
            else
            {
                if (LogonUser.moDataRight.ContainsKey(TableName.Trim()))
                {
                    return true;
                }
                return false;
            }
        }






        /// <summary>
        /// 以位的形式表示权限，第1位是新增，第２位是修改，第３位是删除，第４位是复核，其他位为扩展位,通过RightExpression定义。
        /// 其中第４位比较特别，必须在RightExpression中定义，且Key为Check
        /// 该属性比较特别，他实际上来之于P_OperatorRight,而不是P_Right.但是去通过Right来调用
        /// 默认值为-1,表示没有权限控制
        /// </summary>
        /// <param name="right">参数为菜单项</param>
        /// <returns>返回具体的值</returns>
        public int GetRightValue(CRightItem right)
        {
            if (_LogonUser.RoleID == new Guid("{00000000-0000-0000-0000-000000000000}"))
                return -1;
            else
            {
                if (LogonUser.moRight.ContainsKey(right.ID.Trim()))
                {
                    return LogonUser.moRight[right.ID.Trim()];
                }
                return 0;
            }
        }


        
        public bool RightValidate(CRightItem right)
        {
            if (_LogonUser.RoleID==new Guid("{00000000-0000-0000-0000-000000000000}"))
                return true;
            else
            {
                if (LogonUser.moRight.ContainsKey(right.ID.Trim()))
                {
                    //mSvr.Right[right.ID].RightValue = LogonUser.moRight[right.ID.Trim()];
                    return true;
                }
                return false;
            }
        }
    }
}