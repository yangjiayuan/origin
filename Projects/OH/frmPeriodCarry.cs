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
                        DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"ϵͳ\ϵͳ״̬"), conn, tran);
                        if (ds.Tables[0].Rows.Count != 1)
                        {
                            Msg.Error("������Ϣ�����ݶ�ʧ!");
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
                            ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"ϵͳ\��ǰ�����"), conn, tran);
                            if (ds.Tables[0].Rows.Count != 1)
                            {
                                Msg.Error("������Ϣ�����ݶ�ʧ!");
                                return false;
                            }
                            currDate = DateTime.Parse((string)ds.Tables[0].Rows[0]["Value"]);
                        }
                        DateTime lastDate = currDate.AddMonths(-1);
                        //if (CSystem.Sys.Svr.SystemTime < lastDate)
                        //{
                        //    Msg.Information("���ѽ����½�!");
                        //    return false;
                        //}
                        bool Rtn = true;
                        //�ֿ��½�
                        clsPeriodCarry periodCarry = new clsPeriodCarry();
                        Rtn = periodCarry.ReturnWarehouse(currDate, lastDate, conn, tran);
                        /*
                         * ���ⲿ�ִ����Ƶ������Ĵ��ڣ������½��н���
                        //���Ͽ����½�
                        if (Rtn)
                            Rtn = periodCarry.CloseCheckStandard(currDate, nextDate, conn, tran);
                        //���������Ա�Ľ���
                        if (Rtn)
                            Rtn = periodCarry.ComputeReward(currDate, nextDate, conn, tran);
                         * */
                        if (Rtn)
                        {
                            //����ϵͳ״̬
                            if (systemStatus == 0)
                                CSystem.Sys.Svr.cntMain.Excute(string.Format("Update S_BaseInfo set Value='1' where ID='{0}'", @"ϵͳ\ϵͳ״̬"), conn, tran);
                            //����ϵͳ����
                            CSystem.Sys.Svr.cntMain.Excute(string.Format("Update S_BaseInfo set Value='{1}' where ID='{0}'", @"ϵͳ\��ǰ�����", lastDate), conn, tran);
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
                Msg.Error("��ȡ������Ϣʧ��:" + ex.Message);
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
                        DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"ϵͳ\ϵͳ״̬"), conn, tran);
                        if (ds.Tables[0].Rows.Count != 1)
                        {
                            Msg.Error("������Ϣ�����ݶ�ʧ!");
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
                            ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"ϵͳ\��ǰ�����"), conn, tran);
                            if (ds.Tables[0].Rows.Count != 1)
                            {
                                Msg.Error("������Ϣ�����ݶ�ʧ!");
                                return false;
                            }
                            currDate = DateTime.Parse((string)ds.Tables[0].Rows[0]["Value"]);
                        }
                        DateTime nextDate = currDate.AddMonths(1);
                        if (CSystem.Sys.Svr.SystemTime < nextDate)
                        {
                            Msg.Information("���ѽ����½�!");
                            return false;
                        }
                        bool Rtn = true;
                        //�ֿ��½�
                        clsPeriodCarry periodCarry = new clsPeriodCarry();
                        Rtn = periodCarry.CloseWarehouse(currDate, nextDate, conn, tran);
                        /*
                         * ���ⲿ�ִ����Ƶ������Ĵ��ڣ������½��н���
                        //���Ͽ����½�
                        if (Rtn)
                            Rtn = periodCarry.CloseCheckStandard(currDate, nextDate, conn, tran);
                        //���������Ա�Ľ���
                        if (Rtn)
                            Rtn = periodCarry.ComputeReward(currDate, nextDate, conn, tran);
                         * */
                        if (Rtn)
                        {
                            //����ϵͳ״̬
                            if (systemStatus == 0)
                                CSystem.Sys.Svr.cntMain.Excute(string.Format("Update S_BaseInfo set Value='1' where ID='{0}'", @"ϵͳ\ϵͳ״̬"), conn, tran);
                            //����ϵͳ����
                            CSystem.Sys.Svr.cntMain.Excute(string.Format("Update S_BaseInfo set Value='{1}' where ID='{0}'", @"ϵͳ\��ǰ�����", nextDate), conn, tran);
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
                Msg.Error("��ȡ������Ϣʧ��:" + ex.Message);
                return false;
            }
        }
        
        private void butRefrash_Click(object sender, EventArgs e)
        {
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"ϵͳ\ϵͳ״̬"));
            if (ds.Tables[0].Rows.Count != 1)
            {
                Msg.Error("������Ϣ�����ݶ�ʧ!");
                return;
            }
            int systemStatus = int.Parse((string)ds.Tables[0].Rows[0]["Value"]);
            DateTime currDate;
            if (systemStatus == 0)
            {
                txtNotes.Text = "��ǰϵͳ��û�г�ʼ��!\r\n";
                txtNotes.AppendText(string.Format("�������ִ����ĩ��ת��ʾ\"������ʼ��\",���ĵ�һ������ڼ佫��{0:yyyy-MM}.",DateTime.Now));
                
            }
            else
            {
                ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"ϵͳ\��ǰ�����"));
                if (ds.Tables[0].Rows.Count != 1)
                {
                    Msg.Error("������Ϣ�����ݶ�ʧ!");
                    return;
                }
                currDate = DateTime.Parse((string)ds.Tables[0].Rows[0]["Value"]);
                DateTime nextDate = currDate.AddMonths(1);
                txtNotes.Text = string.Format("��ǰ�������{0:yyyy-MM},��һ�������{1:yyyy-MM}", currDate, nextDate);
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
                Msg.Error("�½�ʧ��!");
            }
            else
            {
                Msg.Information("�½�ɹ�!");
                this.Close();
            }
        }

        private void butTurnBack_Click(object sender, EventArgs e)
        {

            if (!PeriodTurnBack())
            {
                Msg.Error("���½�ʧ��!");
            }
            else
            {
                Msg.Information("���½�ɹ�!");
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