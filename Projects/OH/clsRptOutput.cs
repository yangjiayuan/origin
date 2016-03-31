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
            _Title = "������ѯ";
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
            base.AddCondition("startDate", "��ʼʱ��", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "����ʱ��", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "����", "Enum:0=ȫ��,1=A��,2=B��,3=C��", "(0={0} or A.Class={0})");            
            base.AddCondition("ClassType", "���", "Enum:2=ȫ��,0=�װ�,1=ҹ��", "(2={0} or A.ClassType={0})");
            base.AddCondition("Factory", "����", "Dict:P_Factory", "G.FactoryID='{0}'");
            base.AddCondition("Department", "����", "Dict:P_Department", "D.DepartmentID='{0}'");
            base.AddCondition("Process", "����", "Dict:P_Process", "D.ProcessID='{0}'");
            base.AddCondition("Machine", "�豸", "Dict:P_Machine", "A.MachineID='{0}'");
            base.AddCondition("Material", "����", "Dict:P_Material", "B.MaterialID='{0}'");


            base.AddCondition("ShowClass", "�Ƿ���ʾ����", "Boolean", ",A.Class", "");
            base.AddCondition("ShowClassType", "�Ƿ���ʾ���", "Boolean", ",A.ClassType", "");
            base.AddCondition("ShowMaterial", "�Ƿ���ʾ����", "Boolean", ",E.Code,E.Name,F.MSCode,F.MSName,I.StandardLevelName,B.Length", "");
            base.AddCondition("ShowRework", "�Ƿ��������", "Boolean", "", " and A.IsRework=0 ");

            base.AddColumn("NO", "��Ʊ��");
            base.AddColumn("Date", "ʱ��");
            base.AddColumn("Class", "����", "Enum:0=ȫ��,1=A��,2=B��,3=C��", Infragistics.Win.HAlign.Default,"",true,RptSummaryType.None);
            base.AddColumn("ClassType", "���", "Enum:2=ȫ��,0=�װ�,1=ҹ��", Infragistics.Win.HAlign.Default, "", true, RptSummaryType.None);
            base.AddColumn("Operators", "������Ա");
            //"H.DepartName,G.ProcessName,I.StandardLevelName,A.IsRework,H.OperatorName" +
            base.AddColumn("DepartName", "����");
            base.AddColumn("ProcessName", "����");
            base.AddColumn("CraftName", "����");
            base.AddColumn("PAName", "��������");
            base.AddColumn("MachineName", "�豸");
            base.AddColumn("IsRework", "�Ƿ���", "Enum:0=��,1=��", Infragistics.Win.HAlign.Default, "", true, RptSummaryType.None);
            base.AddColumn("OperatorName", "����Ա"); 
            base.AddColumn("Code", "���ϴ���");
            base.AddColumn("Name", "��������");
            base.AddColumn("MSName", "�ӹ�ͼ��");
            base.AddColumn("StandardLevelName", "�ӹ��ȼ�"); 
            base.AddColumn("Length", "����");
            base.AddColumn("Quantity1stClass", "һ��Ʒ", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Quantity2ndClass", "����Ʒ", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Quantity3rdClass", "�乫Ʒ", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMaterialWaster1", "�Ϸ�", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMaterialWaster2", "ԭ���Ϸ�", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMachineWaster1", "����", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("QuantityMachineWaster2", "�豸����", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            if (_ShowMoney)
            {
                base.AddColumn("Amount", "�ܽ��", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("TotalAmount", "��׼�ܽ��", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("Amount2ndClass", "����Ʒ", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("Amount3rdClass", "�乫Ʒ", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("AmountMaterialWaster1", "�Ϸ�", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("AmountMaterialWaster2", "ԭ���Ϸ�", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("AmountMachineWaster1", "����", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
                base.AddColumn("AmountMachineWaster2", "�豸����", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
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
