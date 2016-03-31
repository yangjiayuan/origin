using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections;


namespace Base
{
    public class DBConnection
    {
        //public static DBConnection Connection = new DBConnection();
        private List<string> _Periods;
        private string connString = null;
        public void SetPeriods(DateTime begin, DateTime end)
        {
            _Periods=new List<string>();
            for (int i = begin.Year; i < end.Year; i++)
                _Periods.Add(i.ToString());
        }


        /* 这个功能做也能够做，当是做起来太复杂了。暂时不处理，改为由具体SQL的地方来处理，可以视具体情况提供一些工具类。
         * 1.分离出from后的内容，终止条件是 where,group,order,unioun,遇到(,必须要找到),确定字符串后，进行表处理；
         * 2.遇到直接是字符串，即认为是表，同里需要分析出是否有别名；遇到"(",找到对应的")",将该字符进行sql处理。
         * 3.也就是有两个处理函数，１是SQL处理函数；２是表名处理函数。两个都要进行递归调用。
         
        public string ProcessPeriod(string sql)
        {
            StringBuilder sb = new StringBuilder();
            int i = -1;
        }
        private string ProcessTable(string tableSegment)
        {
        }
         * 
         */
        public DBConnection(string ConnectionString)
        {
            connString = ConnectionString;
        }
        public string ConnectionString
        {
            get { return connString; }
        }
        /*public DataSet GetPageData(string tableName, string fieldList, string from, string primaryKey, string where, string order, int pageSize, int pageIndex, ref int recorderCount, ref int pageCount)
        {
        Retry:
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("P_viewPage", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@TableName", SqlDbType.VarChar).Value = from;
                        cmd.Parameters.Add("@FieldList", SqlDbType.VarChar).Value = fieldList;
                        cmd.Parameters.Add("@PrimaryKey", SqlDbType.VarChar).Value = primaryKey;
                        cmd.Parameters.Add("@Where", SqlDbType.VarChar).Value = where;
                        cmd.Parameters.Add("@Order", SqlDbType.VarChar).Value = order;
                        cmd.Parameters.Add("@RecorderCount", SqlDbType.Int).Value = recorderCount;
                        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
                        cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                        SqlParameter paraRecorderCount = cmd.Parameters.Add("@TotalCount", SqlDbType.Int);
                        paraRecorderCount.Value = recorderCount;
                        paraRecorderCount.Direction = ParameterDirection.InputOutput;

                        SqlParameter paraPageCount = cmd.Parameters.Add("@TotalPageCount", SqlDbType.Int);
                        paraPageCount.Value = pageCount;
                        paraPageCount.Direction = ParameterDirection.InputOutput;

                        SqlDataAdapter dataAdapter = new SqlDataAdapter();
                        dataAdapter.SelectCommand = cmd;
                        //cmd.ExecuteNonQuery();
                        DataSet ds = new DataSet();
                        dataAdapter.Fill(ds, tableName);
                        if (primaryKey != null && primaryKey.Length > 0)
                        {
                            try
                            {
                                if (ds.Tables[tableName].Columns.Contains(primaryKey))
                                    ds.Tables[tableName].PrimaryKey = new DataColumn[] { ds.Tables[tableName].Columns[primaryKey] };
                            }
                            catch { }
                        }
                        pageCount = (int)paraPageCount.Value;
                        recorderCount = (int)paraRecorderCount.Value;
                        return ds;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2)
                {
                    if (Msg.Question("联接不上服务器，请与系统管理员！\r\n是否重新联接服务器？") == System.Windows.Forms.DialogResult.Yes)
                        goto Retry;
                    else
                        System.Windows.Forms.Application.Exit(new System.ComponentModel.CancelEventArgs(true));
                }
                else
                    Msg.Warning(ex.ToString());
            }
            return null;
        }*/

        public DataSet StoreProcedure(string StoreProcedureName,string TableName,params DictionaryEntry[] ParamName)
        {
            Retry:
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(StoreProcedureName, conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        foreach (DictionaryEntry paramV in ParamName)
                        {
                            cmd.Parameters.AddWithValue(paramV.Key.ToString(), paramV.Value);
                        }

                        SqlDataAdapter dataAdapter = new SqlDataAdapter();
                        dataAdapter.SelectCommand = cmd;
                        cmd.ExecuteNonQuery();
                        DataSet ds = new DataSet();
                        dataAdapter.Fill(ds, TableName);
                        return ds;
                        }
                      catch (Exception ex)
                      {
                        throw ex;
                      }
                      finally
                     {
                        conn.Close();
                     }
                  }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2)
                    {
                        if (Msg.Question("联接不上服务器，请与系统管理员！\r\n是否重新联接服务器？") == System.Windows.Forms.DialogResult.Yes)
                            goto Retry;
                        else
                            System.Windows.Forms.Application.Exit(new System.ComponentModel.CancelEventArgs(true));
                    }
                    else
                        Msg.Warning(ex.ToString());
                }
                return null;
    }
 
        public DataSet Select(string sql)
        {
            return Select(sql, "");
        }
        public DataSet Select(string sql, string tableName)
        {
            return Select(sql, tableName, new DataSet());
        }
        public DataSet Select(string sql, string tableName, DataSet ds)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    try
                    {
                        return Select(sql, tableName, ds, conn, null);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {       
                    if ((ex.Number == 53) ||(ex.Number == 4060)  || (ex.Number ==18456))
                    {
                        Msg.Error(string.Format("无法连接到数据库服务器，请与系统管理员联系！\r\n {0}",ex.ToString()));
                        System.Windows.Forms.Application.Exit(new System.ComponentModel.CancelEventArgs(true)); 
                    }

            }
            return null;
        }
        public DataSet Select(string sql, SqlConnection conn, SqlTransaction tran)
        {
            return Select(sql, null, conn, tran);
        }
        public DataSet Select(string sql, string tableName, SqlConnection conn, SqlTransaction tran)
        {
            return Select(sql, tableName, null, conn, tran);
        }
        public DataSet Select(string sql, string tableName, DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            SqlDataAdapter ad = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandTimeout = conn.ConnectionTimeout;
            if (tran != null)
                cmd.Transaction = tran;
            ad.SelectCommand = cmd;
            DataSet ds2 = null;
            if (ds != null && ds.Tables.Count == 0)
                ds2 = ds;
            else
                ds2 = new DataSet();
            if (tableName == null || tableName.Length == 0)
                ad.Fill(ds2);
            else
            {
                ad.Fill(ds2, tableName);
                if (ds != ds2 && ds != null)
                {
                    if (ds.Tables.Contains(tableName))
                    {
                        ds.Tables[tableName].Rows.Clear();
                    }
                    ds.Merge(ds2);
                }
            }
            if (ds == null)
                return ds2;
            else
                return ds;
        }
        public int Excute(string sql, SqlConnection conn, SqlTransaction tran)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Transaction = tran;
            cmd.CommandTimeout = conn.ConnectionTimeout;
            return cmd.ExecuteNonQuery();
        }
        public int Excute(string sql)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandTimeout = conn.ConnectionTimeout;
                    cmd.CommandText = sql;
                    return cmd.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                }
            }
           
        }
        public void Update(DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    string sql = "select * from " + dt.TableName + " where 1=0";
                    SqlDataAdapter ad = new SqlDataAdapter(sql, conn);
                    SqlCommandBuilder builder = new SqlCommandBuilder(ad);
                    builder.ConflictOption = ConflictOption.CompareRowVersion;
                    ad.Update(dt);
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public void Update(DataTable dt, SqlConnection conn, SqlTransaction tran)
        {
            string sql = "select * from " + dt.TableName + " where 1=0";
            SqlDataAdapter ad = new SqlDataAdapter(sql, conn);
            ad.SelectCommand.Transaction = tran;
            SqlCommandBuilder builder = new SqlCommandBuilder(ad);
            builder.ConflictOption = ConflictOption.OverwriteChanges;
            //ad.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            ad.InsertCommand = builder.GetInsertCommand();
            ad.InsertCommand.Transaction = tran;

            //SqlCommand delCmd = new SqlCommand(string.Format("delete from {0} where ID=@ID", dt.TableName));
            //delCmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier, 0, "ID");
            //ad.DeleteCommand = delCmd;
            ad.DeleteCommand = builder.GetDeleteCommand();
            ad.DeleteCommand.Transaction = tran;

            ad.UpdateCommand = builder.GetUpdateCommand();
            //string s = ad.UpdateCommand.CommandText;
            //s = s.Substring(0, sql.IndexOf("WHERE")) + " WHERE ID=@ID";
            //int count =ad.UpdateCommand.Parameters.Count;
            //for (int i = count - 1; i > count / 2; i++)
            //    ad.UpdateCommand.Parameters.RemoveAt(i);
            //ad.UpdateCommand.Parameters.Add("@ID", SqlDbType.UniqueIdentifier, 0, "ID"); 
            //ad.UpdateCommand.CommandText = s;
            ad.UpdateCommand.Transaction = tran;
            ad.Update(dt);
        }
        public SqlDataAdapter GetAdapter(string sql)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    SqlDataAdapter ad = new SqlDataAdapter(sql, conn);
                    SqlCommandBuilder builder = new SqlCommandBuilder(ad);
                    return ad;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public string DBServerName()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    string DataSource = conn.DataSource;
                    string Database = conn.Database;
                    return string.Format("服务器:{0} 数据库:{1}", DataSource, Database);
                }
                finally
                {
                    conn.Close();
                }
            }

        }


    }
}
