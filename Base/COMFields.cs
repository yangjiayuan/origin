using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Base
{
    public class COMFields
    {
        private SortedList<string, COMField> moFlds;
        private string mOriginalTableName;
        private string mPrimaryKey;
        private CTableProperty _Property;
        private BaseServer mSvr;

        /// <summary>
        /// 
        /// </summary>
        public string OrinalTableName
        {
            get { return mOriginalTableName; }
        }
        public override string ToString()
        {
            return mOriginalTableName;
        }
        public CTableProperty Property
        {
            get { return _Property; }
            set { _Property = value; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OriginalTableName"></param>
        public COMFields(string OriginalTableName, BaseServer svr)
        {
            mSvr = svr;
            moFlds = new SortedList<string, COMField>();
            mOriginalTableName = OriginalTableName;
        }
        public IList<COMField> Fields
        {
            get
            {
                List<COMField> a = new List<COMField>();
                foreach (COMField f in moFlds.Values)
                    a.Add(f);
                a.Sort(new FieldCompare());
                return a;
            }
        }
        public COMField this[string fieldName]
        {
            get
            {
                return moFlds[fieldName];
            }
        }
        private class FieldCompare : IComparer<COMField>
        {
            #region IComparer Members

            public int Compare(COMField x, COMField y)
            {
                return x.ColOrder.CompareTo(y.ColOrder);
            }

            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FldObj"></param>
        public void Add(COMField FldObj)
        {
            string sERR_SOURCE = "COMFields.Add{0}{Table{1}}";
            string sFld;             //字段集合

            sFld = FldObj.FieldName;
            moFlds.Add(sFld, FldObj);
            if (FldObj.IsPrimaryKey)
                mPrimaryKey = FldObj.FieldName;
        }
        //public COMFields GetFields(string OriginalTableName)
        //{
        //    COMFields FldsResult = new COMFields();
        //    foreach (KeyValuePair<String,COMField> kvPair in moFlds)
        //    {
        //        COMField cf = new COMField();
        //        cf = (COMField)kvPair.Value;
        //        if (cf.TableName == OriginalTableName)
        //            FldsResult.Add(cf);
        //    }
        //    return FldsResult;
        //}
        public int Count
        {
            get { return moFlds.Count; }
        }
        /// <summary>
        /// 返回字段列表
        /// </summary>
        /// <param name="OriginalTableName">所需字段列表的主表名称</param>
        /// <param name="WithTablePrefix">是否将表名作为字段的前缀，如果是关联表，前缀为关联名称</param>
        /// <returns></returns>
        public List<string> FieldNameList(bool WithTablePrefix)
        {
            //取一下关系树
            if (WithTablePrefix)
                GetRelationTree();

            List<string> FldNamesResult = new List<string>();
            foreach (KeyValuePair<String, COMField> kvPair in moFlds)
            {
                COMField cf = new COMField();
                cf = (COMField)kvPair.Value;
                if (WithTablePrefix == true)
                {
                    if (cf.TableRelation.Trim() == string.Empty || cf.FieldType==0)
                    {
                        //如果没有关系且为虚字段,需要用默认值来显示
                        if (cf.FieldType == 1)
                        {
                            if (cf.DefaultValue.Length > 0)
                            {
                                if (cf.ValueType.StartsWith("String"))
                                    FldNamesResult.Add(string.Format("'{0}' as {1}", cf.DefaultValue, cf.FieldName));
                                else if (cf.ValueType.StartsWith("Dict") || cf.ValueType.StartsWith("Data"))
                                    FldNamesResult.Add(string.Format("convert(uniqueidentifier,'{0}') as {1}", cf.DefaultValue, cf.FieldName));
                                else if (cf.ValueType.StartsWith("Date"))
                                    FldNamesResult.Add(string.Format("convert(DateTime,'{0}') as {1}", cf.DefaultValue, cf.FieldName));
                                else
                                    FldNamesResult.Add(string.Format("{0} as {1}", cf.DefaultValue, cf.FieldName));
                            }
                            else
                            {
                                FldNamesResult.Add(string.Format("null as {1}", cf.DefaultValue, cf.FieldName));
                            }
                        }
                        else
                            FldNamesResult.Add(string.Format("{0}.{1}", cf.TableName, cf.FieldName));
                    }
                    else
                    {
                        string RelationPath = cf.RelationPath;
                        if (RelationPath != null && RelationPath.Length > 0)
                        {
                            if (!RelationPath.EndsWith(cf.TableRelation))
                                RelationPath = RelationPath + "." + cf.TableRelation;
                        }
                        else
                            RelationPath = cf.TableRelation;
                        FldNamesResult.Add(string.Format("{0}.{1} as {2}", pathRelations[RelationPath].Alia, cf.RFieldName, cf.FieldName));
                    }
                }
                else
                {
                    if (cf.FieldType == 0)
                    {
                        //只有实字段，可以显示
                        FldNamesResult.Add(((COMField)kvPair.Value).FieldName);
                    }
                    else if (cf.TableRelation.Trim() == string.Empty && cf.FieldType == 1)
                    {
                        //如果没有关系且为虚字段,需要用默认值来显示
                        if (cf.DefaultValue.Length > 0)
                        {
                            if (cf.ValueType.StartsWith("String") || cf.ValueType.StartsWith("Dict") || cf.ValueType.StartsWith("Data") || cf.ValueType.StartsWith("Date"))
                                FldNamesResult.Add(string.Format("{0} as {1}", cf.DefaultValue, cf.FieldName));
                        }
                        else
                        {
                            FldNamesResult.Add(string.Format("null as {1}", cf.DefaultValue, cf.FieldName));
                        }
                    }
                    
                }

            }
            return FldNamesResult;

        }

        private List<CRelation> treeRelation;
        private SortedList<string, CRelation> pathRelations;
        //获得关系树,如果没有,就生成
        private List<CRelation> GetRelationTree()
        {
            if (treeRelation != null)
                return treeRelation;
            treeRelation = new List<CRelation>();
            pathRelations = new SortedList<string, CRelation>();
            SortedList<string, string> colCheckPah = new SortedList<string, string>();
            SortedList<string, string> colCheckSubPah = new SortedList<string, string>();
            foreach (KeyValuePair<String, COMField> kvPair in moFlds)
            {
                COMField cf = new COMField();
                cf = (COMField)kvPair.Value;
                if (cf.TableRelation.Trim().Length > 0)
                {
                    string RelationPath = cf.RelationPath;
                    if (RelationPath != null && RelationPath.Length > 0)
                    {
                        if (!RelationPath.EndsWith(cf.TableRelation))
                            RelationPath = RelationPath + "." + cf.TableRelation;
                    }
                    else
                        RelationPath = cf.TableRelation;
                    if (!colCheckPah.ContainsKey(RelationPath))
                    {
                        colCheckPah.Add(RelationPath, RelationPath);
                        string[] path = RelationPath.Split('.');
                        pathRelations.Add(RelationPath, AddRel(path, 0, treeRelation, colCheckSubPah));
                    }
                }
            }
            return treeRelation;
        }
        private CRelation AddRel(string[] path, int i, List<CRelation> list,SortedList<string, string> subPath)
        {
            if (path.Length <= i)
                return null;
            //检测一下有没有然后再增加
            bool exist=false;
            CRelation rel = null;
            foreach (CRelation r in list)
            {
                if (r.RelationName == path[i])
                {
                    exist = true;
                    rel = r;
                    break;
                }
            }
            if (!exist)
            {
                if (!mSvr.Relations.ContainsRelation(path[i]))
                    throw new Exception(string.Format("关系路径中的[{0}]不存在！", path[i]));
                rel = mSvr.Relations[path[i]].Clone();
                if (subPath.ContainsKey(rel.Alia))
                {
                    for (int j = 0; i < 100; j++)
                        if (!subPath.ContainsKey(rel.RelationName + j))
                        {
                            rel.Alia = rel.RelationName + j;
                            break;
                        }
                }
                subPath.Add(rel.Alia, rel.Alia);
                if (rel.JoinType == "Left Join")
                    list.Add(rel);
                else
                    list.Insert(0, rel);
            }
            if (path.Length > i + 1)
            {
                return AddRel(path, i + 1, rel.Children, subPath);
            }
            else
                return rel;
        
        }

        public string GetTableRelation(bool isHistory)
        {
            return GetTableRelation(OrinalTableName, null, GetRelationTree(), isHistory);
        }

        private string GetTableRelation(string baseTable,string alia,List<CRelation> list,bool isHistory)
        {
            //List<CRelation> tree = GetRelationTree();
            string sql = getTableName(baseTable, isHistory);

            if (isHistory)
            {
                if (alia == null || alia.Length == 0)
                    sql = sql + " " + baseTable;
                else
                    sql = sql + " " + alia;
            }
            else
            {
                if (alia != null && alia.Length > 0)
                    sql = sql + " " + alia;
            }

            if (alia == null || alia.Length == 0)
                alia = baseTable;
            foreach (CRelation rel in list)
            {
                if (rel.ChildrenCount > 0)
                    sql += string.Format(" {5} ({0}) On {1}.{2}={3}.{4}", GetTableRelation(rel.RightTable, rel.Alia, rel.Children,isHistory), rel.Alia, rel.RightField, alia, rel.LeftField, rel.JoinType);
                else
                    sql += string.Format(" {0} {1} {2} On {3}.{4}={2}.{5}", rel.JoinType, getTableName(rel.RightTable, isHistory), rel.Alia, alia, rel.LeftField, rel.RightField);
            }
            return sql;
        }
        public string GetTableName(bool isHistory)
        {
            if (isHistory)
            {
                if (this.Property.HistoryTable)
                    return this.OrinalTableName + "_His";
                else
                    return this.OrinalTableName;
            }
            else
                return this.OrinalTableName;

        }
        private string getTableName(string tableName,bool isHistory)
        {
            if (isHistory)
            {
                if (mSvr.Properties[tableName].HistoryTable)
                    return tableName + "_His";
                else
                    return tableName;
            }
            else
                return tableName;
        }
        /// <summary>
        /// TableRelation
        /// </summary>
        public string TableRelation
        {
            get
            {
                return GetTableRelation(false);
            }
        }
        /// <summary>
        /// PrimaryKey
        /// </summary>
        public string PrimaryKey
        {
            get { return mPrimaryKey; }
        }
        public List<COMField> SortColumns
        {
            get
            {
                string OrderByClause;
                List<COMField> Flds = new List<COMField>();

                OrderByClause = "";
                foreach (COMField Fld in moFlds.Values)
                    if (Fld.OrderIndex > 0)
                        Flds.Add(Fld);

                Flds.Sort(new SortIndexCompare());
                return Flds;
            }
        }
        /// <summary>
        /// OrderBy
        /// </summary>
        public string OrderBy
        {
            get
            {
                string OrderByClause;
                List<COMField> Flds = new List<COMField>();

                OrderByClause = "";
                foreach (COMField Fld in moFlds.Values)
                    if (Fld.OrderIndex > 0)
                        Flds.Add(Fld);

                Flds.Sort(new SortIndexCompare());
                foreach (COMField IndexFld in Flds)
                    OrderByClause += string.Format(",{0} {1}", IndexFld.FullFieldName, IndexFld.OrderType == 0 ? "ASC" : "DESC");
                if (OrderByClause.Length > 0)
                    OrderByClause = OrderByClause.Substring(1);

                return OrderByClause;
            }
        }
        private class SortIndexCompare : IComparer<COMField>
        {
            #region IComparer<COMField> Members

            public int Compare(COMField x, COMField y)
            {
                return x.OrderIndex.CompareTo(y.OrderIndex);
            }

            #endregion
        }
        public string FieldsName
        {
            get
            {
                string Result;
                Result = "";
                foreach (string FieldName in FieldNameList(true))
                    Result += string.Format("{0}, ", FieldName);
                Result = Result.Trim();
                Result = Result.Remove(Result.Length - 1);
                return Result;
            }

        }
        public string GetFieldNames(bool WithTablePrefix)
        {
            string Result;
            Result = "";
            foreach (string FieldName in FieldNameList(WithTablePrefix))
                Result += string.Format("{0}, ", FieldName);
            Result = Result.Trim();
            Result = Result.Remove(Result.Length - 1);
            return Result;
        }
        public string QuerySQL
        {
            get
            {
                string result;
                result = string.Format("Select {0} From {1}", FieldsName, TableRelation);
                return result;
            }
        }
        public string GetQuerySQL(bool isHistory)
        {
            return string.Format("Select {0} From {1}", FieldsName, GetTableRelation(isHistory));
        }
        public string QuerySQLWithClause(string WhereClause)
        {
            return string.Format("{0} Where {1}", QuerySQL, WhereClause); 
        }

        public COMFields Clone()
        {
            COMFields FieldsCopy = new COMFields(mOriginalTableName, mSvr);

            foreach (COMField CF in moFlds.Values)
                FieldsCopy.Add(CF.Clone);
            FieldsCopy.Property = this._Property.Clone();

            return FieldsCopy;
        }

        /*
         * Create Table D_WorkPlane(
        ID uniqueidentifier  not null default newid(),
        Code varchar(50)  not null,
        CustomerID uniqueidentifier  not null,
        ConsignDate datetime  null,
        Notes varchar(50)   null,
        CreatedBy uniqueidentifier  not null,
        BillDate datetime not null,
        CreateDate datetime not null,
        CheckStatus int   null,
        CheckedBy uniqueidentifier   null,
        CheckDate datetime  null,
        CONSTRAINT PK_D_WorkPlane PRIMARY KEY CLUSTERED
        (
        ID Asc
        )WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
        ) ON [PRIMARY]

         */
        public string CreateTable(string Tag)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Create Table {0}{1} (",this.OrinalTableName,Tag);
            foreach (COMField f in this.Fields)
            {
                if (f.FieldType == 1)
                    continue;
                sb.Append(f.FieldName);
                string[] s = f.ValueType.Split(':');
                switch (s[0].ToLower())
                {
                    case "string":
                        sb.AppendFormat(" varchar({0}) ", s[1]);
                        break;
                    case "number":
                        sb.AppendFormat(" decimal({0}) ", s[1]);
                        break;
                    case "int":
                    case "enum":
                    case "boolean":
                        sb.Append(" int ");
                        break;
                    case "dict":
                    case "data":
                        sb.Append(" uniqueidentifier ");
                        break;
                    case "date":
                        sb.Append(" datetime ");
                        break;
                    case "guid":
                        sb.Append(" uniqueidentifier ");
                        break;
                    case "custom":
                        sb.Append(" varchar(50) ");
                        break;
                }
                if (string.Compare(f.FieldName, "ID", false)==0)
                    sb.AppendLine(" not null default newid(),");
                else if (f.Mandatory || string.Compare(f.FieldName, "MainID", false)==0 || string.Compare(f.FieldName, "LineNumber", false)==0)
                    sb.AppendLine("not null,");
                else
                    sb.AppendLine("null,");
            }
            sb.AppendFormat("CONSTRAINT PK_{0}{1} PRIMARY KEY CLUSTERED", this.OrinalTableName, Tag);
            sb.AppendLine("(");
            sb.AppendLine("ID Asc");
            sb.AppendLine(")WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]");
            sb.AppendLine(") ON [PRIMARY]");
            return sb.ToString();
        }
    }
}
