using System;
using System.Collections.Generic;
using System.Text;
using UI;

namespace OH
{
    public class clsRptWageQuotiety:BaseReport
    {
        public override bool Initialize()
        {
            _SQL = "select P.Name ProcessName,M.Name MachineName,M.Wagequotiety MWagequotiety,C.Name CraftName,Mt.Name MaterialName,SL.Name StandardLevelName,A.Name AdditionName," +
                        "D.WageQuotiety,D.Quantity1st,D.Quantity2nd,D.Quantity3rd,D.QuantityMaterialWaster,D.QuantityMachineWaster "+
                    "from P_CraftDetail D left join P_Addition A on D.AdditionID=A.ID,P_Craft C,P_Machine M,P_Process P,P_Material Mt,P_StandardLevel SL "+
                    "where D.MainID=C.ID and C.MachineID is null and C.ProcessID=P.ID and M.ProcessID=P.ID and D.MaterialID=Mt.ID and D.StandardLevelID=SL.ID ${Where} "+
                    "union "+
                    "select P.Name ProcessName,M.Name MachineName,M.Wagequotiety MWagequotiety,C.Name CraftName,Mt.Name MaterialName,SL.Name StandardLevelName,A.Name AdditionName," +
                        "D.WageQuotiety,D.Quantity1st,D.Quantity2nd,D.Quantity3rd,D.QuantityMaterialWaster,D.QuantityMachineWaster "+
                    "from P_CraftDetail D left join P_Addition A on D.AdditionID=A.ID,P_Craft C,P_Machine M,P_Process P,P_Material Mt,P_StandardLevel SL " +
                    "where D.MainID=C.ID and C.MachineID=M.ID and C.ProcessID=P.ID and M.ProcessID=P.ID and D.MaterialID=Mt.ID and D.StandardLevelID=SL.ID ${Where}";
            _Title = "����ϵ����ѯ";
            _IsAppendSQL = false;
            _IsWhere = false;

            base.AddCondition("ProcessID", "����", "Dict:P_Process", "P.ID='{0}'");

            base.AddColumn("ProcessName", "����");
            base.AddColumn("MachineName", "�豸");
            base.AddColumn("MWagequotiety", "�豸ϵ��", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("CraftName", "����");
            base.AddColumn("MaterialName", "��Ʒ");
            base.AddColumn("StandardLevelName", "��׼����");
            base.AddColumn("AdditionName", "��������");
            base.AddColumn("WageQuotiety", "����ϵ��", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("Quantity1st", "һ��Ʒ", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("Quantity2nd", "����Ʒ", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("Quantity3rd", "�乫Ʒ", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("QuantityMaterialWaster", "�Ϸ�", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("QuantityMachineWaster", "����", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);

            return base.Initialize();
        }
    }
}
