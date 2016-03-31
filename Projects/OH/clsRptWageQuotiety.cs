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
            _Title = "工资系数查询";
            _IsAppendSQL = false;
            _IsWhere = false;

            base.AddCondition("ProcessID", "工序", "Dict:P_Process", "P.ID='{0}'");

            base.AddColumn("ProcessName", "工序");
            base.AddColumn("MachineName", "设备");
            base.AddColumn("MWagequotiety", "设备系数", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("CraftName", "工艺");
            base.AddColumn("MaterialName", "产品");
            base.AddColumn("StandardLevelName", "标准级别");
            base.AddColumn("AdditionName", "附加条件");
            base.AddColumn("WageQuotiety", "工资系数", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("Quantity1st", "一等品", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("Quantity2nd", "二等品", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("Quantity3rd", "充公品", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("QuantityMaterialWaster", "料废", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("QuantityMachineWaster", "报废", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);

            return base.Initialize();
        }
    }
}
