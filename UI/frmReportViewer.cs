using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace UI
{
    public partial class frmReportViewer : Form
    {
        private Uri _ReportServerUrl;
        private string _ReportPath;

        public frmReportViewer(String ReportTitle,Uri ReportServerUrl, string ReportPaht)
        {
            _ReportServerUrl = ReportServerUrl;
            _ReportPath = ReportPaht;
            this.Tag = ReportTitle;
            this.Text = ReportTitle;
            InitializeComponent();
        }

        private void frmReportViewer_Load(object sender, EventArgs e)
        {
            string ReportServerCredentialsUser;
            string ReportServerCredentialsPassword;

            ReportServerCredentialsUser = CSystem.Sys.Svr.BaseInfo["ReportServerCredentialsUser"].Value.ToString();
            ReportServerCredentialsPassword = CSystem.Sys.Svr.BaseInfo["ReportServerCredentialsPassword"].Value.ToString();
            this.reportViewer.ProcessingMode = ProcessingMode.Remote;
            this.reportViewer.ServerReport.ReportServerUrl= _ReportServerUrl;
            this.reportViewer.ServerReport.ReportServerCredentials.NetworkCredentials = new System.Net.NetworkCredential(ReportServerCredentialsUser, ReportServerCredentialsPassword);
            this.reportViewer.ServerReport.ReportPath= _ReportPath;
            this.reportViewer.RefreshReport();
        }
    }
}
