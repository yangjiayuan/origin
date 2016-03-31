//using System;
//using System.Collections.Generic;
//using System.Text;
//using Base;
//using UI;
//using System.Data;

//namespace YUANYE
//{
//    public class CPrint : IMenuAction
//    {
//        #region IMenuAction Members

//        public System.Windows.Forms.Form GetForm(String ReportTitle, string ReportPaht, DataSet ReportData, System.Windows.Forms.Form mdiForm)
//        {

//            string ReportCode = ReportTitle;

//            MDIMain mdi = (MDIMain)mdiForm;

//            if (mdi.CheckFormTag(ReportCode))
//            {
//                FrmPrintViewer frmPrint = null;
//                frmPrint = (FrmPrintViewer)(new FrmPrintViewer(ReportTitle,ReportPaht,ReportData));
//                return frmPrint;
 
//            }
//            return null;
//        }
//        #endregion
//    }
//}