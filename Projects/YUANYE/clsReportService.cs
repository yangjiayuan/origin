using System;
using System.Collections.Generic;
using System.Text;
using Base;
using UI;
using System.Data;

namespace YUANYE
{
    public class clsReportService : IMenuAction
    {
        #region IMenuAction Members

        public System.Windows.Forms.Form GetForm(CRightItem right, System.Windows.Forms.Form mdiForm)
        {

            string ReportCode = right.Code;

            MDIMain mdi = (MDIMain)mdiForm;

            //string ReportTitle = "产品入库报表";
            //Uri ReportServerUrl = new Uri("http://127.0.0.1/Reportserver");
            //string ReportPaht = "/RStorageIn";

            if (mdi.CheckFormTag(ReportCode))
            {
                frmReportViewer frmReport = null;
                clsReportItem ReportItem = new clsReportItem();
                if (ReportItem.Initialization(ReportCode))
                {
                    frmReport = (frmReportViewer)(new frmReportViewer(ReportItem.ReportTitle, ReportItem.ReportServerUrl, ReportItem.ReportPath));
                    frmReport.Tag = ReportCode;
                    frmReport.Text = ReportItem.ReportTitle;
                    return frmReport;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
        #endregion
    }

    public class clsReportItem
    {
        public string ReportTitle;
        public Uri ReportServerUrl;
        public string ReportPath;

        public clsReportItem()
        {
        }

        public Boolean Initialization(string ReportCode)
        {
            try
            {
                COMFields ReportList = CSystem.Sys.Svr.LDI.GetFields("S_ReportList");
                String SQL = ReportList.QuerySQLWithClause(string.Format("Code= '{0}'", ReportCode));
                DataSet DSReportList = new DataSet();
                DSReportList = CSystem.Sys.Svr.cntMain.Select(SQL);
                if (DSReportList.Tables[0].Rows.Count == 1)
                {
                    DataRow ReportItem = DSReportList.Tables[0].Rows[0];
                    ReportTitle = (string)ReportItem["Name"];
                    ReportServerUrl = new Uri((string)ReportItem["ServerUrl"]);
                    ReportPath = (string)ReportItem["Path"];
                }

                return true;
            }
            catch (Exception Excp)
            {

                if (Excp.InnerException != null)
                    Msg.Error(string.Format("{0}\r\n{1}\r\n{2}\r\n{3}", Excp.Message, Excp.InnerException.Message, Excp.InnerException.StackTrace, Excp.StackTrace));
                else
                {
                    Msg.Error(string.Format("{0}\r\n{1}\r\n", Excp.Message, Excp.StackTrace));
                }
                return false;
            }
        
        }
    }
}