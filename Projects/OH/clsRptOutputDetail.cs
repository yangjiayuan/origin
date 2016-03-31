using System;
using System.Collections.Generic;
using System.Text;
using UI;

namespace OH
{
    public class clsRptOutputDetail:BaseReport
    {
        public override bool Initialize()
        {
            _Title = "产量明细";
            _SQL = "select dbo.getOperators(A.ID) Operators,F.Name MachineName,G.Name ProcessName,A.Class,A.ClassType,A.Date,B.Tag,B.MaterialID,B.MachineStandardID,B.Length,E.Name MaterialName,D.Name MachiningStandardName,H.Name StandardLevelName," +
                        "sum(C.Quantity1stClass) Quantity1stClass,"+
                        "sum(C.Quantity2ndClass) Quantity2ndClass,"+
                        "sum(C.Quantity3rdClass) Quantity3rdClass,"+
                        "sum(C.QuantityMaterialWaster1) QuantityMaterialWaster1,"+
                        "sum(C.QuantityMaterialWaster2) QuantityMaterialWaster2,"+
                        "sum(C.QuantityMachineWaster1) QuantityMachineWaster1,"+
                        "sum(C.QuantityMachineWaster2) QuantityMachineWaster2 "+
                    "from D_workorder         A,"+
                       "D_WorkOrderProduct  B,"+
                       "D_WorkOrderDetail   C,"+
                       "P_MachiningStandard D,"+
				       "P_Material E, "+
                       "P_Machine F,"+
                       "P_Process G, "+
                       "P_StandardLevel H "+
                    "where A.ID = B.MainID and A.ID = C.MainID and B.Tag = c.Tag and B.MachineStandardID = D.ID and E.ID=B.MaterialID and A.MachineID=F.ID and F.ProcessID=G.ID and D.StandardLevelID=H.ID " +
                    "${Where}"+
                    "group by  F.Name,G.Name,A.Date,A.Class,A.ClassType,B.Tag,B.MaterialID,B.MachineStandardID,B.Length,D.Name,E.Name,H.Name,dbo.getOperators(A.ID) ";

            _IsAppendSQL = false;
            _IsWhere = false;

            base.AddCondition("Factory", "工厂", "Dict:P_Factory", "G.FactoryID='{0}'");
            base.AddCondition("Department", "部门", "Dict:P_Department", "F.DepartmentID='{0}'");
            base.AddCondition("ProcessID", "工序", "Dict:P_Process", "A.MachineID in (Select ID from P_Machine where ProcessID ='{0}')");
            base.AddCondition("MachineID", "设备", "Dict:P_Machine", "A.MachineID='{0}'");
            base.AddCondition("startDate", "开始时间", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "结束时间", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "班组", "Enum:0=全部,1=A班,2=B班,3=C班", "(0={0} or A.Class={0})");

            base.AddColumn("ProcessName", "工序"); 
            base.AddColumn("MachineName", "设备");
            base.AddColumn("Class", "班组", "Enum:1=A班,2=B班,3=C班", Infragistics.Win.HAlign.Left, "", true, RptSummaryType.None);
            base.AddColumn("ClassType", "班别", "Enum:0=日班,1=夜班", Infragistics.Win.HAlign.Left, "", true, RptSummaryType.None); 
            base.AddColumn("Operators", "人员");
            base.AddColumn("MaterialName", "产品");
            base.AddColumn("MachiningStandardName", "加工标准");
            base.AddColumn("StandardLevelName", "标准级别");
            
            base.AddColumn("Length", "长度", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("Quantity1stClass", "一等品", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Quantity2ndClass", "二等品", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Quantity3rdClass", "充公品", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMaterialWaster1", "料废", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMaterialWaster2", "原料料废", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMachineWaster1", "报废", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMachineWaster1", "设备报废", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            return base.Initialize();
        }
    }
}
