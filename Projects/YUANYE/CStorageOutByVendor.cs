//using System;
//using System.Collections.Generic;
//using System.Text;
//using Infragistics.Win.UltraWinGrid;
//using System.Data;
//using System.Data.SqlClient;
//using Infragistics.Win.UltraWinEditors;
//using Base;
//using UI;
//using System.Windows.Forms;
//using System.Collections;


//namespace YUANYE
//{
//    public class CStorageOutByVendor : ToolDetailForm
//    {

//        private Form _mdiForm;
//        private string AccountNo; 
//        private DateTime DateRangeLow;
//        private DateTime DateRangeHigh;


//        public override bool AutoCode
//        {
//            get
//            {
//                return true;
//            }
//        }
 
//        public override void NewControl(COMField f, System.Windows.Forms.Control ctl)
//        {

//        }
//        public override bool AllowCheck
//        {
//            get
//            {
//                return false;
//            }
//        }
//        void curr_ValueChanged(object sender, EventArgs e)
//        {
//            if (!_DetailForm.Showed)
//                return;
//            UltraCurrencyEditor curr = sender as UltraCurrencyEditor;
//            if (curr == null)
//                return;

//        }
//        public override void SetCreateGrid(CCreateGrid createGrid)
//        {
//            base.SetCreateGrid(createGrid);
//            createGrid.AfterSelectForm += new AfterSelectFormEventHandler(_CreateGrid_AfterSelectForm);
//            createGrid.BeforeSelectForm += new BeforeSelectFormEventHandler(_CreateGrid_BeforeSelectForm);
//        }


//        void _CreateGrid_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
//        {

//        }

//        void _CreateGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
//        {

//        }

 

//        public override void NewGrid(COMFields fields, UltraGrid grid)
//        {
//            grid.AfterCellUpdate += new CellEventHandler(grid_AfterCellUpdate);
//            grid.AfterRowInsert += new RowEventHandler(grid_AfterRowInsert);
//        }

//        void grid_AfterRowInsert(object sender, RowEventArgs e)
//        {

//        }

//        void grid_AfterCellUpdate(object sender, CellEventArgs e)
//        {   
//        }

//        public override bool AllowDeleteRowInToolBar
//        {
//            get
//            {
//                return true;
//            }
//        }
//        public override bool AllowInsertRowInGrid(string TableName)
//        {
//            return true;
//        }
//        public override bool AllowUpdateInGrid(string TableName)
//        {
//            return true;
//        }
//        public override bool NewData(DataSet ds, COMFields mainTableDefine, List<COMFields> detailTableDefine)
//        {

//            return true;
//        }

//        public override void InsertToolStrip(ToolStrip toolStrip, ToolDetailForm.enuInsertToolStripType insertType)
//        {

//        }


//        public override bool UnCheck(Guid ID, SqlConnection conn, SqlTransaction sqlTran, DataRow mainRow)
//        {
//            return base.UnCheck(ID, conn, sqlTran, mainRow);
//        }

//        public override Form GetForm(CRightItem right, Form mdiForm)
//        {

//            try
//                {
//                    string FormTitle;

//                    _mdiForm = mdiForm;

//                    this.MainTableName = "V_StorageOutByVendor";
//                    SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
//                    SortedList<string, object> Value = new SortedList<string, object>();
//                    defaultValue.Add("V_StorageOutByVendor", Value);
                

//                    COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
//                    FormTitle = "供应商发货统计";
                

//                    mainTableDefine.Property.Title = string.Format(FormTitle);
//                    this._MainCOMFields = mainTableDefine;

//                    frmReport frm = (frmReport)base.GetForm(right, mdiForm);
//                    //frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
//                    //frm.DefaultValue = defaultValue;
//                    return frm;
//                }
//                catch (Exception ex)
//                {
//                    Msg.Warning(string.Format("供应商发货统计程序加载失败,错误信息如下：\r\n {0}", ex.Message));
//                }
//                return null;

//            }
//    }

//}


using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinEditors;
using UI;

namespace YUANYE
{
    public class CStorageOutByVendor : BaseReport
    {
        public override bool Initialize()
        {

            _SQL =
               "SELECT V.StorageOutID,V.Code,V.DocumentDate,V.Customer,P_Customer.Name CustomerName,IssueDate,Vendor,P_Vendor.Name VendorName, " +
               "QuantityActual,PaymentTerm,P_Payment.Name PaymentName, ShippingType,Bill,POL,POD,DEST,Notes,Priceterms " +
               "FROM YUANYE.dbo.V_StorageOutByVendor V " +
               "LEFT JOIN P_Customer ON V.Customer= P_Customer.ID " +
               "Left Join P_Vendor ON V.Vendor =P_Vendor.ID " +
               "Left Join P_Payment on V.PaymentTerm = P_Payment.ID " +
               "${Where} " +
               "Order by V.Code Desc";

            _Title = "供应商发货统计";
            _IsAppendSQL = false;
            _IsWhere = true;

 
            base.AddCondition("IssueDate", "发货时间", "Date", "V.IssueDate <='{0:yyyy-MM-dd}'");
            base.AddCondition("Customer", "客户", "Dict:P_Customer", "V.Customer='{0}'");
            base.AddCondition("Vendor", "供应商", "Dict:P_Vendor", "V.Vendor='{0}'");
            //base.AddCondition("endDate", "结束时间", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            //base.AddCondition("Class", "班组", "Enum:0=全部,1=A班,2=B班,3=C班", "(0={0} or A.Class={0})");
            //base.AddCondition("MachineName", "显示设备", "Boolean", ",E.MachineName", "");
 

            base.AddColumn("Code", "发票号");
            base.AddColumn("CustomerName", "客户", "String", Infragistics.Win.HAlign.Left, "", true);
            base.AddColumn("IssueDate", "发货时间");
            base.AddColumn("VendorName", "供应商","String",Infragistics.Win.HAlign.Left,"",true);
            base.AddColumn("QuantityActual", "实际发货数量", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("PaymentName", "付款条件");
            base.AddColumn("ShippingType", "运输方式", "Enum: 0 = 海运, 1 = 空运",Infragistics.Win.HAlign.Right,"",true);
            base.AddColumn("DEST", "目的地");
            base.AddColumn("NOTES", "备注");

            return base.Initialize();
        }

        public override string BeforeSelectForm(ReportCondition condition)
        {
            string s = "";
            ReportCondition rc;
            switch (condition.Name)
            {
                case "Customer":
                    rc = base.GetCondition("Customer");
                    if (rc != null)
                    {
                        UltraTextEditor txt = rc.Control as UltraTextEditor;
                        if (txt != null && txt.Tag != null)
                        {
                            return string.Format("P_Customer.ID = '{0}'", (Guid)txt.Tag);
                        }
                    }
                    break;
                case "Vendor":
                    rc = base.GetCondition("Vendor");
                    if (rc != null)
                    {
                        UltraTextEditor txt = rc.Control as UltraTextEditor;
                        if (txt != null && txt.Tag != null)
                        {
                            return string.Format("P_Vendor.ID = '{0}'", (Guid)txt.Tag);
                        }
                    }
                    break;
            }
            return "";
        }
    }
}
