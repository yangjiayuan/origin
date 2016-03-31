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
            _Title = "工资查询";
            _IsAppendSQL = false;
            _IsWhere = false;

            //base.AddCondition("period", "月份", "period", "B.Date>='{0:yyyy-MM-dd}' and B.Date<'{1:yyyy-MM-dd}'");
            base.AddCondition("startDate", "开始时间", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "结束时间", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "班组", "Enum:0=全部,1=A班,2=B班,3=C班", "(0={0} or A.Class={0})");
            base.AddCondition("ClassType", "班次", "Enum:2=全部,0=白班,1=夜班", "(2={0} or A.ClassType={0})");
            base.AddCondition("Factory", "工厂", "Dict:P_Factory", "A.DepartmentID in (Select ID from P_Department where FactoryID='{0}')");
            base.AddCondition("Department", "部门", "Dict:P_Department", "A.DepartmentID='{0}'");
            base.AddCondition("Process", "工序", "Dict:P_Process", "A.MachineID in ( Select ID from P_Machine where ProcessID='{0}')");
            base.AddCondition("MachineID", "设备", "Dict:P_Machine", "A.MachineID='{0}'");
            base.AddCondition("Money", "大于等于", "Number:18,2", " having Sum(Amount)>={0}");


            base.AddColumn("Code", "工号"); 
            base.AddColumn("Name", "姓名");
            base.AddColumn("DepartmentName", "部门");
            base.AddColumn("Amount", "金额", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

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
