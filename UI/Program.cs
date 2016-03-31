using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using Base;
using System.Diagnostics;

namespace UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                CSystem.Sys = new CSystem();
                if (CSystem.Sys.Svr.Inited == false)
                {
                    MessageBox.Show("Error");
                }

                frmLogin frm = new frmLogin();
                MDIMain frmMain;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    frmMain = new MDIMain();
                    CSystem.Sys.MdiMain = frmMain;
                    frmMain.UpdateMenuItem();
                    Application.Run(frmMain);
                }
            }
            catch (SqlException SQLExcp)
            {
                MessageBox.Show(SQLExcp.Message);
            }
            catch (Exception Excp)
            {
                if (Excp.InnerException != null)
                    MessageBox.Show(string.Format("{0}\r\n{1}\r\n{2}\r\n{3}", Excp.Message, Excp.InnerException.Message, Excp.InnerException.StackTrace, Excp.StackTrace),"系统错误",MessageBoxButtons.OK,MessageBoxIcon.Stop);
                else
                {
                    MessageBox.Show(string.Format("{0}\r\n{1}\r\n", Excp.Message, Excp.StackTrace),"系统错误",MessageBoxButtons.OK,MessageBoxIcon.Stop);
                }

            }

        }

    }

    public class CSystem
    {
        public static CSystem Sys;
        MDIMain mdiMain;
        BaseServer mSvr;
        bool Inited;
        public CSystem()
        {
            string connString;
            Trace.Assert(Inited == false, "Csystem is already Initialized");
            connString = ConfigurationManager.AppSettings["ConnectionString"];
            System.Console.WriteLine("Connection" + ConfigurationManager.AppSettings["MainTitle"]);
            mSvr = new BaseServer();
            mSvr.Initialize(connString, "AppCode", "MachineCode");
            Inited = true;
        }

        public BaseServer Svr
        {
            get { return mSvr; }
        }
        public MDIMain MdiMain
        {
            get { return mdiMain; }
            set { mdiMain = value; }
        }

        public void DisplayErrorMessage(Exception Excp)
        {
            MessageBox.Show(Excp.Message + "\n" + Excp.InnerException.Message);
        }

    }
    public enum enuShowStatus : int { None = 0, ShowDetail = 1, ShowCheck = 2, ShowInsert = 4 };

    //public class Dialog
    //{
    //    private string _DialogCaption;
    //    public Dialog()
    //    {
    //        _DialogCaption = "进销存管理系统";
    //    }


    //    public static DialogResult Question(object Message)
    //    { 
            
    //    }
    //}
}
