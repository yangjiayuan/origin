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
            _Title = "������ϸ";
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

            base.AddCondition("Factory", "����", "Dict:P_Factory", "G.FactoryID='{0}'");
            base.AddCondition("Department", "����", "Dict:P_Department", "F.DepartmentID='{0}'");
            base.AddCondition("ProcessID", "����", "Dict:P_Process", "A.MachineID in (Select ID from P_Machine where ProcessID ='{0}')");
            base.AddCondition("MachineID", "�豸", "Dict:P_Machine", "A.MachineID='{0}'");
            base.AddCondition("startDate", "��ʼʱ��", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "����ʱ��", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "����", "Enum:0=ȫ��,1=A��,2=B��,3=C��", "(0={0} or A.Class={0})");

            base.AddColumn("ProcessName", "����"); 
            base.AddColumn("MachineName", "�豸");
            base.AddColumn("Class", "����", "Enum:1=A��,2=B��,3=C��", Infragistics.Win.HAlign.Left, "", true, RptSummaryType.None);
            base.AddColumn("ClassType", "���", "Enum:0=�հ�,1=ҹ��", Infragistics.Win.HAlign.Left, "", true, RptSummaryType.None); 
            base.AddColumn("Operators", "��Ա");
            base.AddColumn("MaterialName", "��Ʒ");
            base.AddColumn("MachiningStandardName", "�ӹ���׼");
            base.AddColumn("StandardLevelName", "��׼����");
            
            base.AddColumn("Length", "����", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.None);
            base.AddColumn("Quantity1stClass", "һ��Ʒ", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Quantity2ndClass", "����Ʒ", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Quantity3rdClass", "�乫Ʒ", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMaterialWaster1", "�Ϸ�", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMaterialWaster2", "ԭ���Ϸ�", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMachineWaster1", "����", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMachineWaster1", "�豸����", "number:10,0", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            return base.Initialize();
        }
    }
}
