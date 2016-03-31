using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Base;
using System.Data.SqlClient;
using UI;

namespace OH
{
    public partial class frmPeriodCarry : Form,IMenuAction
    {
        public frmPeriodCarry()
        {
            InitializeComponent();
        }

        private void frmPeriodCarry_Load(object sender, EventArgs e)
        {
            butRefrash_Click(sender, e);
        }
        private bool PeriodTurnBack()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(CSystem.Sys.Svr.cntMain.ConnectionString))
                {
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction();
                        DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"系统\系统状态"), conn, tran);
                        if (ds.Tables[0].Rows.Count != 1)
                        {
                            Msg.Error("基本信息表数据丢失!");
                            return false;
                        }
                        int systemStatus = int.Parse((string)ds.Tables[0].Rows[0]["Value"]);
                        DateTime currDate;
                        if (systemStatus == 0)
                        {
                            currDate = DateTime.Now;
                            currDate = new DateTime(currDate.Year, currDate.Month, 1);
                        }
                        else
                        {
                            ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"系统\当前会计期"), conn, tran);
                            if (ds.Tables[0].Rows.Count != 1)
                            {
                                Msg.Error("基本信息表数据丢失!");
                                return false;
                            }
                            currDate = DateTime.Parse((string)ds.Tables[0].Rows[0]["Value"]);
                        }
                        DateTime lastDate = currDate.AddMonths(-1);
                        //if (CSystem.Sys.Svr.SystemTime < lastDate)
                        //{
                        //    Msg.Information("您已进行月结!");
                        //    return false;
                        //}
                        bool Rtn = true;
                        //仓库月结
                        clsPeriodCarry periodCarry = new clsPeriodCarry();
                        Rtn = periodCarry.ReturnWarehouse(currDate, lastDate, conn, tran);
                        /*
                         * 将这部分代码移到单独的窗口，不在月结中进行
                        //物料考核月结
                        if (Rtn)
                            Rtn = periodCarry.CloseCheckStandard(currDate, nextDate, conn, tran);
                        //计算出各人员的奖金
                        if (Rtn)
                            Rtn = periodCarry.ComputeReward(currDate, nextDate, conn, tran);
                         * */
                        if (Rtn)
                        {
                            //更新系统状态
                            if (systemStatus == 0)
                                CSystem.Sys.Svr.cntMain.Excute(string.Format("Update S_BaseInfo set Value='1' where ID='{0}'", @"系统\系统状态"), conn, tran);
                            //更新系统日期
                            CSystem.Sys.Svr.cntMain.Excute(string.Format("Update S_BaseInfo set Value='{1}' where ID='{0}'", @"系统\当前会计期", lastDate), conn, tran);
                        }
                        if (Rtn)
                            tran.Commit();
                        else
                            tran.Rollback();
                        return Rtn;
                    }
                    catch (Exception ex)
                    {
                        if (tran != null)
                            tran.Rollback();
                        Msg.Error(ex.ToString());
                        return false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Msg.Error("获取基本信息失败:" + ex.Message);
                return false;
            }

        }
        private bool PeriodForword()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(CSystem.Sys.Svr.cntMain.ConnectionString))
                {
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction();
                        DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"系统\系统状态"), conn, tran);
                        if (ds.Tables[0].Rows.Count != 1)
                        {
                            Msg.Error("基本信息表数据丢失!");
                            return false;
                        }
                        int systemStatus = int.Parse((string)ds.Tables[0].Rows[0]["Value"]);
                        DateTime currDate;
                        if (systemStatus == 0)
                        {
                            currDate = DateTime.Now;
                            currDate = new DateTime(currDate.Year, currDate.Month, 1);
                        }
                        else
                        {
                            ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"系统\当前会计期"), conn, tran);
                            if (ds.Tables[0].Rows.Count != 1)
                            {
                                Msg.Error("基本信息表数据丢失!");
                                return false;
                            }
                            currDate = DateTime.Parse((string)ds.Tables[0].Rows[0]["Value"]);
                        }
                        DateTime nextDate = currDate.AddMonths(1);
                        if (CSystem.Sys.Svr.SystemTime < nextDate)
                        {
                            Msg.Information("您已进行月结!");
                            return false;
                        }
                        bool Rtn = true;
                        //仓库月结
                        clsPeriodCarry periodCarry = new clsPeriodCarry();
                        Rtn = periodCarry.CloseWarehouse(currDate, nextDate, conn, tran);
                        /*
                         * 将这部分代码移到单独的窗口，不在月结中进行
                        //物料考核月结
                        if (Rtn)
                            Rtn = periodCarry.CloseCheckStandard(currDate, nextDate, conn, tran);
                        //计算出各人员的奖金
                        if (Rtn)
                            Rtn = periodCarry.ComputeReward(currDate, nextDate, conn, tran);
                         * */
                        if (Rtn)
                        {
                            //更新系统状态
                            if (systemStatus == 0)
                                CSystem.Sys.Svr.cntMain.Excute(string.Format("Update S_BaseInfo set Value='1' where ID='{0}'", @"系统\系统状态"), conn, tran);
                            //更新系统日期
                            CSystem.Sys.Svr.cntMain.Excute(string.Format("Update S_BaseInfo set Value='{1}' where ID='{0}'", @"系统\当前会计期", nextDate), conn, tran);
                        }
                        if (Rtn)
                            tran.Commit();
                        else
                            tran.Rollback();
                        return Rtn;
                    }
                    catch (Exception ex)
                    {
                        if (tran != null)
                            tran.Rollback();
                        Msg.Error(ex.ToString());
                        return false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Msg.Error("获取基本信息失败:" + ex.Message);
                return false;
            }
        }
        
        private void butRefrash_Click(object sender, EventArgs e)
        {
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"系统\系统状态"));
            if (ds.Tables[0].Rows.Count != 1)
            {
                Msg.Error("基本信息表数据丢失!");
                return;
            }
            int systemStatus = int.Parse((string)ds.Tables[0].Rows[0]["Value"]);
            DateTime currDate;
            if (systemStatus == 0)
            {
                txtNotes.Text = "当前系统还没有初始化!\r\n";
                txtNotes.AppendText(string.Format("如果现在执行月末结转表示\"结束初始化\",您的第一个会计期间将是{0:yyyy-MM}.",DateTime.Now));
                
            }
            else
            {
                ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"系统\当前会计期"));
                if (ds.Tables[0].Rows.Count != 1)
                {
                    Msg.Error("基本信息表数据丢失!");
                    return;
                }
                currDate = DateTime.Parse((string)ds.Tables[0].Rows[0]["Value"]);
                DateTime nextDate = currDate.AddMonths(1);
                txtNotes.Text = string.Format("当前会计期是{0:yyyy-MM},下一会计期是{1:yyyy-MM}", currDate, nextDate);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void butPeriodCarry_Click(object sender, EventArgs e)
        {
            if (!PeriodForword())
            {
                Msg.Error("月结失败!");
            }
            else
            {
                Msg.Information("月结成功!");
                this.Close();
            }
        }

        private void butTurnBack_Click(object sender, EventArgs e)
        {

            if (!PeriodTurnBack())
            {
                Msg.Error("反月结失败!");
            }
            else
            {
                Msg.Information("反月结成功!");
                this.Close();
            }
        }

        #region IMenuAction Members

        public Form GetForm(CRightItem right, Form mdiForm)
        {
            this.ShowDialog();
            return null;
        }

        #endregion
    }
}