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
            _Title = "�������ϸ����";
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

            //�豸,ʱ���,����
            base.AddCondition("MachineID", "�豸", "Dict:P_Machine", "A.MachineID='{0}'");
            base.AddCondition("startDate", "��ʼʱ��", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "����ʱ��", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "����", "Enum:0=ȫ��,1=A��,2=B��,3=C��", "(0={0} or A.Class={0})");

            //�豸,����,ά����,��Щ��Ʊ,ʱ��,Ʒ��,����,��׼����,��׼��ʱ
            base.AddColumn("MachineName", "�豸");
            base.AddColumn("MaintainWorker", "ά����");
            base.AddColumn("Class", "����", "Enum:1=A��,2=B��,3=C��", Infragistics.Win.HAlign.Left, "", true, RptSummaryType.None);
            base.AddColumn("NO", "��Ʊ��");
            base.AddColumn("BillDate", "ʱ��");
            base.AddColumn("MaterialCode", "Ʒ�ִ���");
            base.AddColumn("MaterialName", "Ʒ��");
            base.AddColumn("StandardLevelName", "�ȼ�");
            base.AddColumn("Quantity", "����", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutputPerHour", "ÿСʱ����", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("ManHour", "��׼��ʱ", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            return base.Initialize();
        }

    }
}
