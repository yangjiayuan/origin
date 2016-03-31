using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using UI;

namespace OH
{
    public class clsRptReachRate:BaseReport
    {
       public override bool  Initialize()
       {
 	        _Title = "�����";
            _SQL = "select P.Name MachineName,Output.MachineID,Output.Class,Output.ManHour,Output.factManHour,Output.NormalDownTime,Output.MaintenanceDowntime,Output.Waitingtime,case (Output.ManHour-Output.NormalDownTime-Output.MaintenanceDowntime-Output.Waitingtime) when 0 then 0 else Output.factManHour/(Output.ManHour-Output.NormalDownTime-Output.MaintenanceDowntime-Output.Waitingtime) end Rate " +
                    "from P_Machine P,"+
                    "(select isnull(Data.Class,MaintenanceTime.Class) Class,isnull(Data.MachineID,MaintenanceTime.MachineID) MachineID,sum(isnull(ManHour,ManHour2)) ManHour,sum(isnull(Data.factManHour,ManHour2)) factManHour,sum(isnull(MaintenanceTime.NormalDownTime,0)) NormalDownTime ,sum(isnull(MaintenanceTime.MaintenanceDowntime,0)) MaintenanceDowntime,sum(isnull(MaintenanceTime.Waitingtime,0)) Waitingtime " +
                        "from (select C.MachineID,D.ClassType,D.Class,D.BillDate,SM.ManHour,sum(case C.OutputPerHour when 0 then 0 else D.qty/C.OutputPerHour end) factManHour from " +
                                "(Select A.BillDate,B.* from D_StandardOutput A,D_StandardOutputBill B where A.ID=B.MainID) C, " +
                                "(select A.MachineID,A.ClassType,A.Class,A.Date BillDate,B.MaterialID,D.StandardLevelID,sum(B.Quantity) qty "+   
                                    "from D_workorder A,D_WorkOrderProduct B,P_MachiningStandard D "+
                                    "where A.ID=B.MainID and B.MachineStandardID=D.ID "+
                                "group by B.MaterialID,A.ClassType,A.Class,A.Date,A.MachineID,D.StandardLevelID) D, "+
                                "(Select A.BillDate,B.* from D_StandardManHour A,D_StandardManHourBill B where A.ID=B.MainID) SM "+
                                "where datediff(wk,dateadd(day,-1,C.BillDate),C.BillDate)=datediff(wk,dateadd(day,-1,D.BillDate),D.BillDate) and C.MachineID=D.MachineID and C.MaterialID=D.MaterialID and C.StandardLevelID=D.StandardLevelID " +
                                    "and datediff(wk,dateadd(day,-1,SM.BillDate),SM.BillDate)=datediff(wk,dateadd(day,-1,D.BillDate),D.BillDate) and SM.MachineID=D.MachineID and SM.ClassType=D.ClassType " +
                                " and D.BillDate>=${startDate} and D.BillDate<=${endDate} " +
                            "group by C.MachineID,D.ClassType,D.Class,D.BillDate,SM.ManHour) Data " +
                        "full join  "+
                            "(select M.*, SM2.ManHour ManHour2 from "+
                                "(Select A.BillDate, B.* "+
                                    "from D_StandardManHour A, D_StandardManHourBill B "+
                                        "where A.ID = B.MainID) SM2,"+
                                "(select A.BillDate,B.MachineID,B.Class,B.ClassType,sum(B.NormalDownTime) NormalDownTime ,sum(B.MaintenanceDowntime) MaintenanceDowntime,sum(B.Waitingtime) Waitingtime " +
                                    "from D_MaintenanceTime A,D_MaintenanceTimeBill B "+
                                    "where A.ID=B.MainID and A.BillDate>=${startDate} and A.BillDate<=${endDate}" +
                                "group by A.BillDate,B.Class,B.ClassType,B.MachineID) M " +
                             "where datediff(wk,dateadd(day, -1, SM2.BillDate),SM2.BillDate) =datediff(wk, dateadd(day, -1, m.BillDate), m.BillDate) "+
                                    "and SM2.MachineID = m.MachineID and SM2.ClassType = m.ClassType) "+
                            "MaintenanceTime "+
                        "on MaintenanceTime.BillDate=Data.BillDate and MaintenanceTime.Class=Data.Class and MaintenanceTime.MachineID=Data.MachineID "+
                    "group by isnull(Data.Class,MaintenanceTime.Class),isnull(Data.MachineID,MaintenanceTime.MachineID)) Output " +
                    "where P.ID=Output.MachineID ${Where}";
            _IsAppendSQL = false;
            _IsWhere = false;
            //����,�豸,ʱ���,����
            base.AddCondition("ProcessID", "����", "Dict:P_Process", "Output.MachineID in (Select ID from P_Machine where ProcessID ='{0}')");
            base.AddCondition("MachineID", "�豸", "Dict:P_Machine", "Output.MachineID='{0}'");
            base.AddCondition("startDate", "��ʼʱ��", "Date", "'{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "����ʱ��", "Date", "'{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "����", "Enum:0=ȫ��,1=A��,2=B��,3=C��", "(0={0} or Output.Class={0})");
            //�豸,��׼��ʱ,����ͣ��ʱ��,ά��ͣ��ʱ��,���㹤ʱ,�����
            base.AddColumn("MachineName", "�豸");
            base.AddColumn("Class", "����", "Enum:1=A��,2=B��,3=C��", Infragistics.Win.HAlign.Left, "", true, RptSummaryType.None);
            base.AddColumn("ManHour", "��׼��ʱ", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("NormalDownTime", "����ͣ��ʱ��", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("MaintenanceDowntime", "ά��ͣ��ʱ��", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Waitingtime", "�ȴ�ʱ��", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("factManHour", "���㹤ʱ", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Rate", "�����", "number:10,4", Infragistics.Win.HAlign.Right, "#0.00", true, RptSummaryType.Average);

            return base.Initialize();
        }
        public override string BeforeSelectForm(ReportCondition condition)
        {
            if (condition.Name == "MachineID")
            {
                ReportCondition rc = base.GetCondition("ProcessID");
                if (rc != null)
                {
                    UltraTextEditor txt = rc.Control as UltraTextEditor;
                    if (txt != null && txt.Tag!=null)
                    {
                       return string.Format("P_Machine.ProcessID = '{0}'", (Guid)txt.Tag);
                    }
                }
            }
            return "";
        }
        public override bool Strike(Form frm, Infragistics.Win.UltraWinGrid.UltraGridRow row)
        {
            //��Ӧֵ
            //�豸��ʱ�����䡢����
            BaseReport rpt = new clsRptReachRateDetail();
            frmReport frmNew = new frmReport(rpt, frm.MdiParent);
            ReportCondition rc = rpt.GetCondition("MachineID");
            UltraTextEditor text = (UltraTextEditor)rc.Control;
            text.Tag = row.Cells["MachineID"].Value;
            text.Text = row.Cells["MachineName"].Value as string;
            UltraDateTimeEditor dateBegin = (UltraDateTimeEditor)rpt.GetCondition("startDate").Control;
            dateBegin.Value = ((UltraDateTimeEditor)this.GetCondition("startDate").Control).Value;
            UltraDateTimeEditor dateEnd = (UltraDateTimeEditor)rpt.GetCondition("endDate").Control;
            dateEnd.Value = ((UltraDateTimeEditor)this.GetCondition("endDate").Control).Value;
            ComboBox cmb=(ComboBox)rpt.GetCondition("Class").Control;
            cmb.SelectedIndex = (int)row.Cells["Class"].Value;
            frmNew.Show();
            frmNew.Query();
            return true;
        }
    }
}
