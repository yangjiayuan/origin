using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace Base
{
    
    public class COMFieldManager
    {

        //'------------------------------------------------------------------------------'
        //'〖方法〗读取字段数据
        //'〖参数〗clsFields      ：字段集合
        //'　　　　TableName      ：主表名称
        //'------------------------------------------------------------------------------'
        private string msTableName;
        private bool mbInited;
        private DBConnection mCnt;
        private BaseServer mSvr;
        private SortedList<string, COMFields> moTables= new SortedList<string,COMFields>();

        private void LoadFields()
        {
            string OriginalTableName;
            COMFields clsFields;

            foreach (CTableProperty CT in mSvr.Properties.Tables)
             {
                 //Debug.Assert(CT.TableName != "S_ReportList");
                 OriginalTableName =CT.TableName;
                 clsFields = LoadFieldsbyTable(OriginalTableName);
                 clsFields.Property = CT;
                 if (moTables.ContainsKey(CT.ID)==false)
                    moTables.Add(CT.ID, clsFields);
             }   
        }

        private COMFields LoadFieldsbyTable(string TableName) 
        {
            COMFields clsFields;
            string SQL;
            COMField xField;
 

            clsFields = new COMFields(TableName,mSvr);

            SQL = "Select S_TableField.*,S_TableRelation.LeftTable,S_TableRelation.Righttable, " +
                 "S_TableRelation.LeftField,S_TableRelation.RightField,S_TableRelation.JoinType " +
                 "From S_TableField " +
                 "Left Join S_TableRelation on S_TableField.TableRelation=S_TableRelation.ID " +
                 "Where S_TableField.TableName='" + TableName + "' "+
                 "Order by S_TableField.ColOrder";

            DataSet ds = mCnt.Select(SQL);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                int IJoinType;
                string SKey = (string)dr["ID"];
                xField = new COMField();

                xField.FieldID = SKey;
                xField.TableName = (string)dr["TableName"];
                xField.FieldName = (string)dr["FieldName"];
                xField.FieldTitle = (string)dr["FieldTitle"];
                xField.ColOrder = (int)dr["ColOrder"];
                xField.Mandatory = (int)dr["Mandatory"] == 1 ? true : false;
                xField.ValueType = (string)dr["ValueType"];
                xField.Format = Convert.IsDBNull(dr["Format"]) ? "" : (string)dr["Format"];
                xField.DefaultValue = Convert.IsDBNull(dr["DefaultValue"]) ? "" : (string)dr["DefaultValue"];
                xField.Visible = (COMField.Enum_Visible)dr["Visible"];
                xField.IsPrimaryKey = ((int)dr["IsPrimaryKey"]) == 1;
                xField.OrderIndex = (int)dr["OrderBy"];
                xField.OrderType = (COMField.Enum_OrderType)dr["OrderType"];
                xField.Enable = (int)dr["Enable"] == 1 ? true : false;
                xField.FieldType = (int)dr["FieldType"];
                xField.RelationPath = Convert.IsDBNull(dr["RelationPath"]) ? "" : (string)dr["RelationPath"];
                xField.TableRelation = Convert.IsDBNull(dr["TableRelation"]) ? "" : (string)dr["TableRelation"];
                xField.RFieldName = Convert.IsDBNull(dr["FactFieldName"]) ? "" : (string)dr["FactFieldName"];
                xField.LeftTableName = Convert.IsDBNull(dr["LeftTable"]) ? "" : (string)dr["LeftTable"];
                xField.RightTableName = Convert.IsDBNull(dr["RightTable"]) ? "" : (string)dr["RightTable"];
                xField.LeftFieldName = Convert.IsDBNull(dr["LeftField"]) ? "" : (string)dr["LeftField"];
                xField.RightFieldName = Convert.IsDBNull(dr["RightField"]) ? "" : (string)dr["RightField"];
                xField.ShowSummary = (int)dr["ShowSummary"] == 1 ? true : false;
                xField.NewLine = (int)dr["NewLine"] == 1 ? true : false;
                xField.Width = (int)dr["Width"];
                if (ds.Tables[0].Columns.Contains("Height"))
                    xField.Height = dr["Height"] == System.DBNull.Value ? 1 : (int)dr["Height"];
                else
                    xField.Height =1;
                xField.GroupName = dr["GroupName"] as string;

                if (Convert.IsDBNull(dr["Jointype"]) == true)
                    xField.JoinType = "";
                else
                {
                    IJoinType = (int)dr["JoinType"];
                    xField.JoinType = IJoinType != 0 ? "Left Join" : "Inner Join";
                }

                clsFields.Add(xField);

            }

            return clsFields;
        }
        public void Initialization(BaseServer Svr,DBConnection cntDB, String sTableName)
        {
            mSvr = Svr;
            mCnt = cntDB;
            msTableName = sTableName;

            mbInited = true;
            this.LoadFields();
        }

        public COMFields GetFields(string OriginalTableName)
        {
            if (mSvr.Properties.ContainsTable(OriginalTableName))
                return moTables[OriginalTableName];
            else
                throw new Exception(string.Format("Table name of '{0}' is not defined in S_Table!",OriginalTableName));
                return null;
        }

    }
}