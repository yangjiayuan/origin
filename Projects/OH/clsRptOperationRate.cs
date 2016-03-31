using System;
using System.Collections.Generic;
using System.Text;
using UI;

namespace OH
{
    public class clsRptOperationRate:BaseReport
    {
        public override bool Initialize()
        {
            _Title = "��ת�ʡ��ڶ���";
            _SQL = "select P.Name,P.MaintainWorker," +
                "SM.ManHour StandardTime," +
		        "P.PowerTime*(datediff(day,'${startDate}','${endDate}')+1) PowerTime,"+
                "isnull(MT.NormalDownTime,0) NormalDownTime,isnull(MT.MaintenanceDowntime,0) MaintenanceDowntime," +
                "(SM.ManHour-${deduct}-isnull(MT.NormalDownTime,0)-isnull(MT.MaintenanceDowntime,0))/(SM.ManHour-${deduct}) Rate1," +
                "(P.PowerTime*(datediff(day,'${startDate}','${endDate}')+1)-${deduct}-isnull(MT.NormalDownTime,0)-isnull(MT.MaintenanceDowntime,0))/(P.PowerTime*(datediff(day,'${startDate}','${endDate}')+1)-${deduct}) Rate2 "+
                "from P_Machine P,"+
                    "(select MachineID,sum(manhours) manHour from ("+
                        "select MachineID,case when dates=1 then datediff(wk,dateadd(day,-1,'${startDate}'),'${endDate}')* a.manhour "+
			                                "else (datediff(day,dateadd(day,-1,'${startDate}'),'${endDate}')-datediff(wk,dateadd(day,-1,'${startDate}'),'${endDate}'))* a.manhour end manhours "+
                        "from "+
                                "(select datediff(wk,dateadd(day,-1,A.BillDate),A.BillDate)dates,MachineID,sum(ManHour) ManHour "+
                                        "from D_StandardManHour A,D_StandardManHourBill B "+
                                        "where A.ID=B.MainID "+
                                  "group by MachineID,A.BillDate) A "+
                        ") B "+
                      "group by MachineID) SM "+
                    " left join "+
                    "(select B.MachineID,"+
                           "sum(B.NormalDownTime) NormalDownTime,"+
                           "sum(B.MaintenanceDowntime) MaintenanceDowntime "+
                      "from D_MaintenanceTime A, D_MaintenanceTimeBill B "+
                            "where A.ID = B.MainID and A.BillDate>='${startDate}' and A.BillDate<='${endDate}'"+
                     "group by B.MachineID) MT "+
                     " on SM.MachineID = MT.MachineID "+
                "where SM.MachineID = P.ID ${MachineID}";
            _IsAppendSQL = false;

            base.AddCondition("startDate", "��ʼʱ��", "Date", "{0:yyyy-MM-dd}");
            base.AddCondition("endDate", "����ʱ��", "Date", "{0:yyyy-MM-dd}");
            base.AddCondition("MachineID", "�豸", "Dict:P_Machine", "and SM.MachineID='{0}'");
            base.AddCondition("deduct", "�۳���", "int", "{0}");

            base.AddColumn("Name", "�豸");
            base.AddColumn("MaintainWorker", "������");
            base.AddColumn("StandardTime", "��׼ʱ��", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("PowerTime", "����ʱ��", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("NormalDownTime", "����ͣ��", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("MaintenanceDowntime", "����ͣ��", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Rate1", "��ת��", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Average);
            base.AddColumn("Rate2", "�ڶ���", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Average);

            return base.Initialize();
        }
    }
}
