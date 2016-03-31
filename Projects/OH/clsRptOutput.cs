using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using UI;

namespace OH
{
    public class clsRptOutput:BaseReport
    {
        private bool _ShowMoney;

        public clsRptOutput()
        {
            _ShowMoney = true;
        }
        public clsRptOutput(bool showMoney)
        {
            _ShowMoney = showMoney;
        }
        public override bool  Initialize()
        {
            _Title = "产量查询";
            _SQL = "select PA.PAName,A.Code WorkOrderCode,A.NO,A.Date,D.MachineName,A.MachineID ${ShowMaterial} ${ShowClass} ${ShowClassType},dbo.getOperators(A.ID) Operators," +
                            "H.DepartName,G.ProcessName,A.IsRework,J.OperatorName,Cr.CraftName," +
                            "sum(C.Quantity1stClass) Quantity1stClass," +
                            "sum(C.Quantity2ndClass) Quantity2ndClass," +
                            "sum(C.Quantity3rdClass) Quantity3rdClass," +
                            "sum(C.QuantityMaterialWaster1) QuantityMaterialWaster1," +
                            "sum(C.QuantityMaterialWaster2) QuantityMaterialWaster2," +
                            "sum(C.QuantityMachineWaster1) QuantityMachineWaster1, " +
                            "sum(C.QuantityMachineWaster2) QuantityMachineWaster2, " +
                            "sum(C.TotalAmount) TotalAmount," +
                            "sum(Amount1stClass) Amount1stClass,"+
                            "sum(Amount2ndClass) Amount2ndClass,"+
                            "sum(Amount3rdClass) Amount3rdClass,"+
                            "sum(AmountMaterialWaster1) AmountMaterialWaster1,"+
                            "sum(AmountMachineWaster1) AmountMachineWaster1,"+
                            "sum(AmountMaterialWaster2) AmountMaterialWaster2,"+
                            "sum(AmountMachineWaster2) AmountMachineWaster2,"+
                            "sum(C.Amount) Amount " +
                        "from D_workorder A,D_WorkOrderProduct B left join (Select ID PAID,Name PAName from P_Addition) PA on B.AdditionID=PA.PAID," +
                        "D_WorkOrderDetail C,(select Name MachineName,ID,ProcessID,DepartmentID from P_Machine) D,P_Material E,"+
                        "(select Code MSCode,Name MSName,ID,StandardLevelID from P_MachiningStandard) F,"+
                        "(select Name ProcessName,ID ProcessID,FactoryID from P_Process) G, " +
                        "(select Name CraftName,ID CraftID from P_Craft) Cr, " +
                        "(Select Name DepartName,ID DepartID from P_Department) H, "+
                        "(Select Name StandardLevelName,ID StandardLevelID from P_StandardLevel) I, "+
                        "(Select Name OperatorName,ID OperatorID from P_Operator) J "+
                        "where A.MachineID=D.ID and A.ID=B.MainID and A.ID=C.MainID and B.Tag=c.Tag and E.ID=B.MaterialID and F.ID=B.MachineStandardID ${ShowRework} " +
                        "and D.ProcessID=G.ProcessID and D.DepartmentID=H.DepartID and I.StandardLevelID=F.StandardLevelID and J.OperatorID=A.OperatorID " +
                        "and B.CraftID=Cr.CraftID "+
                        "${Where} " +
                        "group by PA.PAName,A.Code,A.NO,A.Date,A.MachineID,D.MachineName,H.DepartName,G.ProcessName,Cr.CraftName,A.IsRework,J.OperatorName ${ShowMaterial} ${ShowClass} ${ShowClassType},dbo.getOperators(A.ID)";
            _IsAppendSQL = false;
            _IsWhere = false;
            base.AddCondition("startDate", "开始时间", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "结束时间", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "班组", "Enum:0=全部,1=A班,2=B班,3=C班", "(0={0} or A.Class={0})");            
            base.AddCondition("ClassType", "班次", "Enum:2=全部,0=白班,1=夜班", "(2={0} or A.ClassType={0})");
            base.AddCondition("Factory", "工厂", "Dict:P_Factory", "G.FactoryID='{0}'");
            base.AddCondition("Department", "部门", "Dict:P_Department", "D.DepartmentID='{0}'");
            base.AddCondition("Process", "工序", "Dict:P_Process", "D.ProcessID='{0}'");
            base.AddCondition("Machine", "设备", "Dict:P_Machine", "A.MachineID='{0}'");
            base.AddCondition("Material", "物料", "Dict:P_Material", "B.MaterialID='{0}'");


            base.AddCondition("ShowClass", "是否显示班组", "Boolean", ",A.Class", "");
            base.AddCondition("ShowClassType", "是否显示班次", "Boolean", ",A.ClassType", "");
            base.AddCondition("ShowMaterial", "是否显示物料", "Boolean", ",E.Code,E.Name,F.MSCode,F.MSName,I.StandardLevelName,B.Length", "");
            base.AddCondition("ShowRework", "是否包括返修", "Boolean", "", " and A.IsRework=0 ");

            base.AddColumn("NO", "工票号");
            base.AddColumn("Date", "时间");
            base.AddColumn("Class", "班组", "Enum:0=全部,1=A班,2=B班,3=C班", Infragistics.Win.HAlign.Default,"",true,RptSummaryType.None);
            base.AddColumn("ClassType", "班次", "Enum:2=全部,0=白班,1=夜班", Infragistics.Win.HAlign.Default, "", true, RptSummaryType.None);
            base.AddColumn("Operators", "班组人员");
            //"H.DepartName,G.ProcessName,I.StandardLevelName,A.IsRework,H.OperatorName" +
            base.AddColumn("DepartName", "部门");
            base.AddColumn("ProcessName", "工序");
            base.AddColumn("CraftName", "工艺");
            base.AddColumn("PAName", "附加条件");
            base.AddColumn("MachineName", "设备");
            base.AddColumn("IsRework", "是否返修", "Enum:0=否,1=是", Infragistics.Win.HAlign.Default, "", true, RptSummaryType.None);
            base.AddColumn("OperatorName", "检验员"); 
            base.AddColumn("Code", "物料代码");
            base.AddColumn("Name", "物料名称");
            base.AddColumn("MSName", "加工图号");
            base.AddColumn("StandardLevelName", "加工等级"); 
            base.AddColumn("Length", "长度");
            base.AddColumn("Quantity1stClass", "一等品", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Quantity2ndClass", "二等品", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Quantity3rdClass", "充公品", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMaterialWaster1", "料废", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMaterialWaster2", "原料料废", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMachineWaster1", "报废", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMachineWaster2", "设备报废", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            if (_ShowMoney)
            {
                base.AddColumn("Amount", "总金额", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("TotalAmount", "标准总金额", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("Amount2ndClass", "二等品", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("Amount3rdClass", "充公品", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("AmountMaterialWaster1", "料废", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("AmountMaterialWaster2", "原料料废", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("AmountMachineWaster1", "报废", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("AmountMachineWaster2", "设备报废", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            }
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
        public override Form GetForm(Base.CRightItem right, Form mdiForm)
        {
            Form frm = base.GetForm(right, mdiForm);
            if (right.Paramters != null && right.Paramters.ToLower() == "showmoney")
                this._ShowMoney = true;
            else
                this._ShowMoney = false;
            return frm;
        }
    }
}
