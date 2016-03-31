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
            _Title = "���Ź��ʲ�ѯ";
            _IsAppendSQL = false;
            _IsWhere = false;


            //base.AddCondition("period", "�·�", "period", "A.CheckDate>='{0:yyyy-MM-dd}' and A.CheckDate<'{1:yyyy-MM-dd}'");
            base.AddCondition("startDate", "��ʼʱ��", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "����ʱ��", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "����", "Enum:0=ȫ��,1=A��,2=B��,3=C��", "(0={0} or A.Class={0})");
            base.AddCondition("ClassType", "���", "Enum:2=ȫ��,0=�װ�,1=ҹ��", "(2={0} or A.ClassType={0})");
            base.AddCondition("Factory", "����", "Dict:P_Factory", "A.DepartmentID in (Select ID from P_Department where FactoryID='{0}')");
            base.AddCondition("Department", "����", "Dict:P_Department", "A.DepartmentID='{0}'");
            base.AddCondition("Process", "����", "Dict:P_Process", "A.MachineID in ( Select ID from P_Machine where ProcessID='{0}')");
            base.AddCondition("MachineID", "�豸", "Dict:P_Machine", "A.MachineID='{0}'");
            base.AddCondition("MaterialID", "��Ʒ", "Dict:P_Material", "B.MaterialID='{0}'");
            base.AddCondition("MachineName", "��ʾ�豸", "Boolean", ",E.MachineName", "");
            base.AddCondition("MaterialName", "��ʾ��Ʒ", "Boolean", ",F.MaterialName", "");

            base.AddColumn("DepartmentName", "����");
            base.AddColumn("MachineName", "�豸");
            base.AddColumn("MaterialName", "����");
            base.AddColumn("Amount", "ʵ�ʹ���", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("TotalAmount", "һ��Ʒ", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Amount2ndClass", "����Ʒ", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Amount3rdClass", "�乫Ʒ", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("AmountMaterialWaster1", "�Ϸ�", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("AmountMaterialWaster2", "ԭ���Ϸ�", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("amountMachineWaster1", "����", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("AmountMachineWaster2", "�豸����", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

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
