using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinEditors;
using UI;
using System.Windows.Forms;

namespace OH
{
    public class clsRptOperatorReward:BaseReport
    {
        public override bool Initialize()
        {
            _Title = "��Ա�����ѯ";
            _SQL = "select A.MaterialCheckObjectID,A.MachineID,A.OperatorID,A.Reward-A.Punish Reward,B.Name MachineName,C.Name OperatorName,D.Name DepartmentName,E.Name MaterialCheckObjectName from D_OperatorRewardPunish A,P_Machine B,P_Operator C,P_Department D,P_MaterialCheckObject E " +
                "where A.MachineID=B.ID and A.OperatorID=C.ID and A.MaterialCheckObjectID=E.ID and C.DepartmentID=D.ID ";

            _IsWhere = false;
            _IsAppendSQL = true;

            base.AddCondition("period", "�ڼ�", "Period", "A.Period>='{0:yyyy-MM-dd}' and A.Period <'{1:yyyy-MM-dd}'");
            base.AddCondition("DepartmentID", "����", "Dict:P_Department", "C.DepartmentID='{0}'");
            base.AddCondition("OperatorID", "��Ա", "Dict:P_Operator", "A.OperatorID='{0}'");
            base.AddCondition("MachineID", "�豸", "Dict:P_Machine", "A.MachineID='{0}'");
            base.AddCondition("MaterialCheckObjectID", "���˶���", "Dict:P_MaterialCheckObject", "A.MaterialCheckObjectID='{0}'");
            base.AddCondition("chkOperator", "����Ա����", "Boolean", "", "");

            base.AddColumn("MachineName", "�豸");
            base.AddColumn("DepartmentName", "����");
            base.AddColumn("OperatorName", "��Ա");
            base.AddColumn("MaterialCheckObjectName", "���˶���");
            base.AddColumn("Reward", "��(��Ϊ����)", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            //base.AddColumn("Punish", "��", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            return base.Initialize();
        }
        public override string BeforeSelectForm(ReportCondition condition)
        {
            if (condition.Name == "OperatorID")
            {
                ReportCondition rc = base.GetCondition("DepartmentID");
                if (rc != null)
                {
                    UltraTextEditor txt = rc.Control as UltraTextEditor;
                    if (txt != null && txt.Tag != null)
                    {
                        return string.Format("P_Operator.DepartmentID = '{0}'", (Guid)txt.Tag);
                    }
                }
            }
            return "";
        }
        public override void BeforeQuery(object sender, frmReport.BeforeQueryEventArgs e)
        {
            if (((CheckBox)(base.GetCondition("chkOperator").Control)).Checked)
            {
                e.IsAppendSQL = false;
                e.SQL = "Select sum(Reward) Reward,OperatorName,DepartmentName from (" +
                    e.SQL + " ${Where} "+
                    ") A group by  OperatorName,DepartmentName ";
            }
            base.BeforeQuery(sender, e);
        }
    }
}
