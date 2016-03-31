using System;
using System.Collections.Generic;
using System.Text;
using Base;
using UI;
using System.Data;
using Infragistics.Win.UltraWinEditors;

namespace OH
{
    public class clsRptOperatorWage:BaseReport
    {
        public override bool Initialize()
        {
            _SQL = "select A.ID,A.Date,U2.Code WorkerCode,U2.Name WorkerName,C.Name DepartmentName,A.NO WorkOrderCode,A.Class,A.ClassType,D.Name MachineName," +
                        "U1.Name CheckerName,A.Memo WorkOrderNotes,B.Rate,B.Ratio,B.Amount " +
                    "from D_WorkOrder A,D_workOrderOperator B,P_Department C,P_Machine D,P_Operator U1,P_Operator U2,P_Station G,P_OperatorProperty H " +
                    "where A.ID=B.MainID and A.OperatorID=U1.ID and B.OperatorID=U2.ID and U2.StationID=G.ID and U2.OperatorPropertyID=H.ID "+
                        " and C.ID=A.DepartmentID and A.MachineID=D.ID";
            _IsAppendSQL = true;
            _IsWhere = false;
            _Title = "个人工资查询";

            base.AddCondition("startDate", "开始时间", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "结束时间", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "班组", "Enum:0=全部,1=A班,2=B班,3=C班", "(0={0} or A.Class={0})");
            base.AddCondition("ClassType", "班次", "Enum:2=全部,0=白班,1=夜班", "(2={0} or A.ClassType={0})");
            base.AddCondition("Factory", "工厂", "Dict:P_Factory", "A.DepartmentID in (Select ID from P_Department where FactoryID='{0}')");
            base.AddCondition("Department", "部门", "Dict:P_Department", "A.DepartmentID='{0}'");
            base.AddCondition("Process", "工序", "Dict:P_Process", "A.MachineID in ( Select ID from P_Machine where ProcessID='{0}')");
            base.AddCondition("MachineID", "设备", "Dict:P_Machine", "A.MachineID='{0}'");

            base.AddCondition("OperatorID", "人员", "Dict:P_Operator", "B.OperatorID='{0}'");
            //base.AddCondition("period", "月份", "period", "A.Date>='{0:yyyy-MM-dd}' and A.Date<'{1:yyyy-MM-dd}'");

            //日期、工号、姓名、部门、工票号、
            //班别、设备、检验员、备注、分配比例、调整金额、金额 个人备注
            base.AddColumn("Date", "日期", "date", Infragistics.Win.HAlign.Default, "yyyy-MM-dd", true);
            base.AddColumn("WorkerCode", "工号");
            base.AddColumn("WorkerName", "姓名");
            base.AddColumn("DepartmentName", "部门");
            base.AddColumn("WorkOrderCode", "工票号");
            base.AddColumn("Class", "班组", "Enum:0=全部,1=A班,2=B班,3=C班", Infragistics.Win.HAlign.Default, "", true, RptSummaryType.None);
            base.AddColumn("ClassType", "班次", "Enum:2=全部,0=白班,1=夜班", Infragistics.Win.HAlign.Default, "", true, RptSummaryType.None);
            base.AddColumn("MachineName", "设备");
            base.AddColumn("CheckerName", "检验员");
            base.AddColumn("WorkOrderNotes", "备注");
            base.AddColumn("Rate", "分配比例");
            base.AddColumn("Ratio", "扣减比例");
            base.AddColumn("Amount", "金额", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            return base.Initialize();
        }
        public override string BeforeSelectForm(ReportCondition condition)
        {
            string s = "";
            ReportCondition rc;
            switch (condition.Name)
            {
                case "OperatorID":
                    return "P_Operator.IsChecker=0";
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
            if (row.Band.Columns.Exists("ID"))
            {
                COMFields _MainTableDefine = CSystem.Sys.Svr.LDI.GetFields("D_WorkOrder");
                List<COMFields> _DetailTableDefine = CSystem.Sys.Svr.Properties.DetailTableDefineList("D_WorkOrder");

                DataSet ds = CSystem.Sys.Svr.cntMain.Select(_MainTableDefine.QuerySQL + " where D_WorkOrder.ID='" + row.Cells["ID"].Value + "'", _MainTableDefine.OrinalTableName);

                frmDetail form = new frmDetail();
                if (form != null)
                {
                    form.toolDetailForm = new clsWorkOrderDetailForm();
                    bool bShowData = form.ShowData(_MainTableDefine, _DetailTableDefine, new DataSetEventArgs(ds), false, frm.MdiParent);
                }
            }
            return true;
        }
    }
}
