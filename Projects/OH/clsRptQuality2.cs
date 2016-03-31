using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using UI;

namespace OH
{
    public class clsRptQuality2:BaseReport
    {
        public override bool Initialize()
        {
            _Title = "质检报表(按机台)";
            _SQL = "select F.Name ProcessName,E.Name MachineName,G.Name LevelName,A.Class,A.ClassType,A.Date," +
                    "cast(sum(Quantity1stClass) as decimal)/sum(Quantity1stClass+Quantity2ndClass+Quantity3rdClass+QuantityMaterialWaster1+QuantityMaterialWaster2+QuantityMachineWaster1+QuantityMachineWaster2) Rate1," +
                    "cast(sum(Quantity2ndClass) as decimal)/sum(Quantity1stClass+Quantity2ndClass+Quantity3rdClass+QuantityMaterialWaster1+QuantityMaterialWaster2+QuantityMachineWaster1+QuantityMachineWaster2) Rate2," +
                    "cast(sum(Quantity3rdClass) as decimal)/sum(Quantity1stClass+Quantity2ndClass+Quantity3rdClass+QuantityMaterialWaster1+QuantityMaterialWaster2+QuantityMachineWaster1+QuantityMachineWaster2) Rate3," +
                    "cast(sum(QuantityMaterialWaster1+QuantityMaterialWaster2+QuantityMachineWaster1+QuantityMachineWaster2) as decimal)/sum(Quantity1stClass+Quantity2ndClass+Quantity3rdClass+QuantityMaterialWaster1+QuantityMaterialWaster2+QuantityMachineWaster1+QuantityMachineWaster2) Rate4 " +
                    "from D_WorkOrder A,D_WorkOrderProduct B,D_WorkOrderDetail C ,P_MachiningStandard D,P_Machine E,P_Process F,P_StandardLevel G " +
                    "where A.ID=B.MainID and A.ID=C.MainID and B.Tag=C.Tag and B.MachineStandardID=D.ID and A.MachineID=E.ID and E.ProcessID=F.ID and D.QAStandardLevelID=G.ID " +
                    "${Where} "+
                    "group by F.Name,E.Name,G.Name,A.Class,A.ClassType,A.Date,A.MachineID,D.StandardLevelID "+
                    "Order by A.Date,F.Name,E.Name";
            _IsAppendSQL = false;
            _IsWhere = false;
            base.AddCondition("startDate", "开始时间", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "结束时间", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "班组", "Enum:0=全部,1=A班,2=B班,3=C班", "(0={0} or A.Class={0})");
            base.AddCondition("Process", "工序", "Dict:P_Process", "E.ProcessID='{0}'");
            base.AddCondition("Machine", "设备", "Dict:P_Machine", "A.MachineID='{0}'");

            base.AddColumn("Date", "时间");
            base.AddColumn("ProcessName", "工序");
            base.AddColumn("MachineName", "设备");
            base.AddColumn("LevelName", "等级");
            base.AddColumn("Rate1", "一次合格率", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Average);
            base.AddColumn("Rate2", "二等品率", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Average);
            base.AddColumn("Rate3", "充公率", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Average);
            base.AddColumn("Rate4", "报废率", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Average);

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
                    if (txt != null && txt.Tag != null)
                    {
                        return string.Format("P_Machine.ProcessID = '{0}'", (Guid)txt.Tag);
                    }
                }
            }
            return "";
        }
    }
}
