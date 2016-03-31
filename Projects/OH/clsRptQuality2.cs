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
            _Title = "�ʼ챨��(����̨)";
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
            base.AddCondition("startDate", "��ʼʱ��", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "����ʱ��", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "����", "Enum:0=ȫ��,1=A��,2=B��,3=C��", "(0={0} or A.Class={0})");
            base.AddCondition("Process", "����", "Dict:P_Process", "E.ProcessID='{0}'");
            base.AddCondition("Machine", "�豸", "Dict:P_Machine", "A.MachineID='{0}'");

            base.AddColumn("Date", "ʱ��");
            base.AddColumn("ProcessName", "����");
            base.AddColumn("MachineName", "�豸");
            base.AddColumn("LevelName", "�ȼ�");
            base.AddColumn("Rate1", "һ�κϸ���", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Average);
            base.AddColumn("Rate2", "����Ʒ��", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Average);
            base.AddColumn("Rate3", "�乫��", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Average);
            base.AddColumn("Rate4", "������", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Average);

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
