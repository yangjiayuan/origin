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
    public partial class FrmPrintViewer : Form
    {
        private string _ReportPath;
        private DataSet _ReportDataSet;

        public FrmPrintViewer(String ReportTitle, string ReportPaht, DataSet ReportData)
        {
            _ReportPath = ReportPaht;
            _ReportDataSet = ReportData;

            this.Tag = ReportTitle;
            this.Text = ReportTitle;
            InitializeComponent();
        }

        private void PrintViewer_Load(object sender, EventArgs e)
        {
            int i;
            ReportDataSource RDS;

            this.PrintViewer.ProcessingMode = ProcessingMode.Local;

            this.PrintViewer.LocalReport.ReportPath = _ReportPath;
            
            for ( i = 0; i < _ReportDataSet.Tables.Count; i++)
            {
                RDS = new ReportDataSource(_ReportDataSet.Tables[i].TableName, _ReportDataSet.Tables[i]);
                PrintViewer.LocalReport.DataSources.Add(RDS);
            }
           
            this.PrintViewer.RefreshReport();
        }

        private void FrmPrintViewer_Load(object sender, EventArgs e)
        {

        }

     }
    
}
