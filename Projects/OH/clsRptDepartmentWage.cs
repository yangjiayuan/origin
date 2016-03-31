using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinEditors;
using UI;

namespace OH
{
    public class clsRptDepartmentWage:BaseReport
    {
        public override bool Initialize()
        {
            _SQL = "select D.DepartmentName ${MachineName} ${MaterialName}," +
                        "sum(C.Amount) Amount," +
                        "sum(C.TotalAmount) TotalAmount," +
                        "sum(Amount2ndClass) Amount2ndClass," +
                        "sum(Amount3rdClass) Amount3rdClass," +
                        "sum(AmountMaterialWaster1) AmountMaterialWaster1," +
                        "sum(AmountMaterialWaster2) AmountMaterialWaster2," +
                        "sum(amountMachineWaster1) amountMachineWaster1," +
                        "sum(AmountMachineWaster2) AmountMachineWaster2 " +
                    "from D_WorkOrder A,d_WorkOrderProduct B,D_WorkOrderDetail C,"+
                        "(select ID,Name DepartmentName from P_Department) D,"+
                        "(select ID,Name MachineName from P_Machine) E,"+
                        "(select ID,Name MaterialName from P_Material) F " +
                    "where A.ID=B.MainID and A.ID=C.MainID and B.Tag=C.Tag and A.DepartmentID=D.ID and A.MachineID=E.ID and B.MaterialID=F.ID "+
                    "and A.CalculateStatus=1 ${Where} " +
                    "Group By D.DepartmentName ${MachineName} ${MaterialName}";
            _Title = "部门工资查询";
            _IsAppendSQL = false;
            _IsWhere = false;


            //base.AddCondition("period", "月份", "period", "A.CheckDate>='{0:yyyy-MM-dd}' and A.CheckDate<'{1:yyyy-MM-dd}'");
            base.AddCondition("startDate", "开始时间", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "结束时间", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "班组", "Enum:0=全部,1=A班,2=B班,3=C班", "(0={0} or A.Class={0})");
            base.AddCondition("ClassType", "班次", "Enum:2=全部,0=白班,1=夜班", "(2={0} or A.ClassType={0})");
            base.AddCondition("Factory", "工厂", "Dict:P_Factory", "A.DepartmentID in (Select ID from P_Department where FactoryID='{0}')");
            base.AddCondition("Department", "部门", "Dict:P_Department", "A.DepartmentID='{0}'");
            base.AddCondition("Process", "工序", "Dict:P_Process", "A.MachineID in ( Select ID from P_Machine where ProcessID='{0}')");
            base.AddCondition("MachineID", "设备", "Dict:P_Machine", "A.MachineID='{0}'");
            base.AddCondition("MaterialID", "产品", "Dict:P_Material", "B.MaterialID='{0}'");
            base.AddCondition("MachineName", "显示设备", "Boolean", ",E.MachineName", "");
            base.AddCondition("MaterialName", "显示产品", "Boolean", ",F.MaterialName", "");

            base.AddColumn("DepartmentName", "部门");
            base.AddColumn("MachineName", "设备");
            base.AddColumn("MaterialName", "物料");
            base.AddColumn("Amount", "实际工资", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("TotalAmount", "一等品", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Amount2ndClass", "二等品", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Amount3rdClass", "充公品", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("AmountMaterialWaster1", "料废", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("AmountMaterialWaster2", "原料料废", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("amountMachineWaster1", "报废", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("AmountMachineWaster2", "设备报废", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            return base.Initialize();
        }
        public override string BeforeSelectForm(ReportCondition condition)
        {
            string s = "";
            ReportCondition rc;
            switch (condition.Name)
            {
                case "MachineID":
                    rc = base.GetCondition("Department");
                    if (rc != null)
                    {
                        UltraTextEditor txt = rc.Control as UltraTextEditor;
                        if (txt != null && txt.Tag != null)
                        {
                            s = string.Format("P_Machine.DepartmentID = '{0}'", (Guid)txt.Tag);
                        }
                    }
                    rc = base.GetCondition("Process");
                    if (rc != null)
                    {
                        UltraTextEditor txt = rc.Control as UltraTextEditor;
                        if (txt != null && txt.Tag != null)
                        {
                            s = string.Format("P_Machine.ProcessID = '{0}'", (Guid)txt.Tag);
                        }
                    }
                    break;
                case "Department":
                    rc = base.GetCondition("Factory");
                    if (rc != null)
                    {
                        UltraTextEditor txt = rc.Control as UltraTextEditor;
                        if (txt != null && txt.Tag != null)
                        {
                            return string.Format("P_Department.FactoryID = '{0}'", (Guid)txt.Tag);
                        }
                    }
                    break;
                case "Process":
                    rc = base.GetCondition("Factory");
                    if (rc != null)
                    {
                        UltraTextEditor txt = rc.Control as UltraTextEditor;
                        if (txt != null && txt.Tag != null)
                        {
                            return string.Format("P_Process.FactoryID = '{0}'", (Guid)txt.Tag);
                        }
                    }
                    break;
            }
            return "";
        }
    }
}
