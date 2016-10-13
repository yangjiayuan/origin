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
