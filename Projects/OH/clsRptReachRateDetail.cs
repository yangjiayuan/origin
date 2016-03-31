using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using UI;

namespace OH
{
    public class clsRptReachRateDetail:BaseReport
    {
        //frmReport frm = null;
        public override bool Initialize()
        {
            _Title = "达成率明细数据";
            _SQL = "select M.Name MachineName,M.MaintainWorker,D.ClassType,D.Class,D.NO,D.BillDate,mat.Name MaterialName,mat.code MaterialCode,SL.Name StandardLevelName,D.qty Quantity,C.OutputPerHour,SM.ManHour " +
                        "from C_StandardOutput C,P_StandardLevel SL,P_Machine M,P_Material mat, " +
                            "(Select datediff(wk,dateadd(day,-1,A.BillDate),A.BillDate)dates,B.* from D_StandardManHour A,D_StandardManHourBill B where A.ID=B.MainID) SM," +
                            "(select A.MachineID,A.ClassType,A.Class,A.NO,A.Date BillDate,B.MaterialID,D.StandardLevelID," +
                                "sum(B.Quantity) qty " +
                            "from D_workorder A," +
                               "D_WorkOrderProduct B," +
                               "P_MachiningStandard D " +
                         "where A.ID = B.MainID and B.MachineStandardID = D.ID " +
                         "${Where} " +
                         "group by B.MaterialID,A.ClassType,A.Class,A.NO,A.Date,A.MachineID,D.StandardLevelID) D " +
                     "where C.MachineID = D.MachineID and C.MaterialID = D.MaterialID and C.StandardLevelID = D.StandardLevelID " +
                        "and SM.MachineID=D.MachineID and SM.ClassType=D.ClassType and SM.dates=datediff(wk,dateadd(day,-1,D.BillDate),D.BillDate)" +
                        "and SL.ID=C.StandardLevelID " +
                        "and M.ID=C.MachineID " +
                        "and mat.ID=D.MaterialID " +
                      "order by D.BillDate,M.Name,D.Class,D.ClassType,SL.Name,mat.Code";
            _IsAppendSQL = false;
            _IsWhere = false;

            //设备,时间段,班组
            base.AddCondition("MachineID", "设备", "Dict:P_Machine", "A.MachineID='{0}'");
            base.AddCondition("startDate", "开始时间", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "结束时间", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "班组", "Enum:0=全部,1=A班,2=B班,3=C班", "(0={0} or A.Class={0})");

            //设备,班组,维修组,哪些工票,时间,品种,数量,标准产量,标准工时
            base.AddColumn("MachineName", "设备");
            base.AddColumn("MaintainWorker", "维修组");
            base.AddColumn("Class", "班组", "Enum:1=A班,2=B班,3=C班", Infragistics.Win.HAlign.Left, "", true, RptSummaryType.None);
            base.AddColumn("NO", "工票号");
            base.AddColumn("BillDate", "时间");
            base.AddColumn("MaterialCode", "品种代码");
            base.AddColumn("MaterialName", "品种");
            base.AddColumn("StandardLevelName", "等级");
            base.AddColumn("Quantity", "产量", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutputPerHour", "每小时产量", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("ManHour", "标准工时", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            return base.Initialize();
        }

    }
}
