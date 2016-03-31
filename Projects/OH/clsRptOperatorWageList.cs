using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinEditors;
using UI;

namespace OH
{
    public class clsRptOperatorWageList:BaseReport 
    {
        public override bool Initialize()
        {
            _SQL = "select C.ID,C.Code,C.Name,D.Name DepartmentName,sum(Amount) Amount " +
                    "from D_WorkOrderOperator B,D_WorkOrder A,P_Operator C,P_Department D " +
                    "where B.MainID=A.ID and B.OperatorID=C.ID and C.DepartmentID=D.ID ${Where} " +
                    "group by C.ID,C.Code,C.Name,D.Name ${Money} Order By C.Code";
            _Title = "���ʲ�ѯ";
            _IsAppendSQL = false;
            _IsWhere = false;

            //base.AddCondition("period", "�·�", "period", "B.Date>='{0:yyyy-MM-dd}' and B.Date<'{1:yyyy-MM-dd}'");
            base.AddCondition("startDate", "��ʼʱ��", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "����ʱ��", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "����", "Enum:0=ȫ��,1=A��,2=B��,3=C��", "(0={0} or A.Class={0})");
            base.AddCondition("ClassType", "���", "Enum:2=ȫ��,0=�װ�,1=ҹ��", "(2={0} or A.ClassType={0})");
            base.AddCondition("Factory", "����", "Dict:P_Factory", "A.DepartmentID in (Select ID from P_Department where FactoryID='{0}')");
            base.AddCondition("Department", "����", "Dict:P_Department", "A.DepartmentID='{0}'");
            base.AddCondition("Process", "����", "Dict:P_Process", "A.MachineID in ( Select ID from P_Machine where ProcessID='{0}')");
            base.AddCondition("MachineID", "�豸", "Dict:P_Machine", "A.MachineID='{0}'");
            base.AddCondition("Money", "���ڵ���", "Number:18,2", " having Sum(Amount)>={0}");


            base.AddColumn("Code", "����"); 
            base.AddColumn("Name", "����");
            base.AddColumn("DepartmentName", "����");
            base.AddColumn("Amount", "���", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

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

        public override bool Strike(System.Windows.Forms.Form frm, Infragistics.Win.UltraWinGrid.UltraGridRow row)
        {
            BaseReport rpt = new clsRptOperatorWage();
            frmReport frmNew = new frmReport(rpt, frm.MdiParent);

            UltraDateTimeEditor startDate = (UltraDateTimeEditor)rpt.GetCondition("startDate").Control;
            startDate.Value = ((UltraDateTimeEditor)this.GetCondition("startDate").Control).Value;
            UltraDateTimeEditor endDate = (UltraDateTimeEditor)rpt.GetCondition("endDate").Control;
            endDate.Value = ((UltraDateTimeEditor)this.GetCondition("endDate").Control).Value;

            UltraTextEditor OperatorID = (UltraTextEditor)rpt.GetCondition("OperatorID").Control;
            OperatorID.Tag = row.Cells["ID"].Value;
            OperatorID.Text = row.Cells["Name"].Value as string;
            frmNew.Show();
            frmNew.Query();
            return true;
        }
    }
}
