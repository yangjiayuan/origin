//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Text;
//using System.Windows.Forms;
//using UI.Report;
//using System.Data.SqlClient;

//namespace UI
//{
//    public partial class FrmCRStorageIn: Form
//    {
//        CRStorageIn RStorageData;
//        StorageData DSStorageData;
//        string SQL;

//        public FrmCRStorageIn()
//        {
//            InitializeComponent();
//        }

 

//        public void FillData()
//        {
//            DataSet DS;
//            RStorageData = new CRStorageIn();
//            DSStorageData = new StorageData();

//            try
//            {
//                SQL = "Select * From V_NormalStorageIn";
//                DS = CSystem.Sys.Svr.cntMain.Select(SQL,"DTStorageIn");
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);
//            }
//            finally
//            {
//            }

//            if (DS != null)
//            {
//                DSStorageData.Merge(DS.Tables[0]);
//                RStorageData.SetDataSource(DSStorageData);
//                CRViewer.ReportSource = RStorageData;
//            }
//        }

//        private void CRViewer_Load(object sender, EventArgs e)
//        {
//            this.FillData();
//        }

//        private void FrmCRStorageData_Load(object sender, EventArgs e)
//        {

//        }
//    }
//}
