using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinEditors;
using System.Windows.Forms;
using System.Data;
using UI;

namespace OH
{
    public class clsRptWarehouseBalance:BaseReport
    {
        private bool _ShowMoney;
        private bool _ShowOther;
        public clsRptWarehouseBalance(bool showMoney,bool showOther)
        {
            _ShowMoney = showMoney;
            _ShowOther = showOther;
        }
        public clsRptWarehouseBalance(bool showMoney):this(showMoney,false)
        {
        }
        public clsRptWarehouseBalance():this(false)
        {
        }
        public override bool Initialize()
        {
            _Title = "�ֿ��շ������ϸ��";
            _SQL = "Select * " +
                      "from D_WarehouseBalance A " +
                      "inner join (select ID BID,Name WarehouseName from P_Warehouse) B on A.WarehouseID=B.BID " +
                      "inner join (select ID CID,Name PositionName from P_Position) C on A.PositionID=C.CID " +
                      "inner join (select ID DID,Name MaterialName,Code MaterialCode,Spec,MaterialClassID from P_Material) D on A.MaterialID=D.DID " +
                      "left join (select ID GID,Name MaterialClassName from P_MaterialClass) G on D.MaterialClassID=G.GID " +
                      "left join (select ID EID,Name IngredientName from P_Ingredient) E on A.IngredientID=E.EID " +
                      "left join (select ID FID,Name MachiningStandardName,Code MachiningStandardCode,Text1 from P_MachiningStandard) F on A.MachiningStandardID=F.FID " +
                      " ${Where} ";
            _IsAppendSQL = false;
            _IsWhere = true;
            
            base.AddCondition("WarehouseID", "�ֿ�", "Dict:P_Warehouse", "A.WarehouseID='{0}'");
            base.AddCondition("PositionID", "��λ", "Dict:P_Position", "A.PositionID='{0}'");
            base.AddCondition("MaterialID", "����", "Dict:P_Material", "A.MaterialID='{0}'");
            base.AddCondition("MachiningStandardID", "�ӹ�ͼ��", "Dict:P_MachiningStandard", "A.MachiningStandardID='{0}'");
            base.AddCondition("period", "�·�", "period", "A.CheckDate>='{0:yyyy-MM-dd}' and A.CheckDate<'{1:yyyy-MM-dd}'");
            base.AddCondition("IncludeUncheckData", "����δ����", "Boolean", ""," and A.CheckStatus=1 ");

            base.AddCondition("chkPosition", "����ʾ��λ", "Boolean", "", "");

            base.AddColumn("WarehouseName", "�ֿ�", true);
            base.AddColumn("PositionName", "��λ", true);
            base.AddColumn("MaterialClassName", "���ϴ���", true);
            base.AddColumn("MaterialCode", "���ϴ���", true);
            base.AddColumn("MaterialName", "��������", true);
            base.AddColumn("Spec", "���", true);
            base.AddColumn("Length", "����", "number:10,2", Infragistics.Win.HAlign.Right, "#,##0.00;-#,##0.00; ", true, RptSummaryType.None);
            base.AddColumn("IngredientName", "����");
            base.AddColumn("MachiningStandardCode", "�ӹ�ͼ�Ŵ���");
            base.AddColumn("MachiningStandardName", "�ӹ�ͼ������");
            base.AddColumn("Text1", "��Ʒ��׼");
            //������
            base.AddColumn("BeginQuantity1", "�ڳ�������","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            base.AddColumn("InQuantity1", "���������", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutQuantity1", "����������", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("EndQuantity1", "��ĩ������", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            //������
            base.AddColumn("BeginQuantity2", "�ڳ�������","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            base.AddColumn("InQuantity2", "��⸨����", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutQuantity2", "���⸨����", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("EndQuantity2", "��ĩ������", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            //���
            if (_ShowMoney) base.AddColumn("BeginMoney", "�ڳ����", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("InMoney", "�����", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("OutMoney", "������", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("EndMoney", "��ĩ���", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            //����
            base.AddColumn("InQuantity1_0", "��ͨ���������","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            base.AddColumn("InQuantity2_0", "��ͨ��⸨����","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("InMoney_0", "��ͨ�����", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("InQuantity1_1", "�ɹ����������","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            base.AddColumn("InQuantity2_1", "�ɹ���⸨����","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("InMoney_1", "�ɹ������", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("InQuantity1_2", "�ƿ����������","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            base.AddColumn("InQuantity2_2", "�ƿ���⸨����","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("InMoney_2", "�ƿ������", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("InQuantity1_3", "�������������","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            base.AddColumn("InQuantity2_3", "������⸨����","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("InMoney_3", "���������", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            base.AddColumn("OutQuantity1_0", "��ͨ����������","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            base.AddColumn("OutQuantity2_0", "��ͨ���⸨����", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("OutMoney_0", "��ͨ������", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutQuantity1_1", "��������������", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutQuantity2_1", "�������ϸ�����", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("OutMoney_1", "�������Ͻ��", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutQuantity1_2", "�ƿ����������", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutQuantity2_2", "�ƿ���⸨����","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("OutMoney_0", "�ƿ������", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutQuantity1_3", "��Ʒ����������","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            base.AddColumn("OutQuantity2_3", "��Ʒ���⸨����","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("OutMoney_3", "��Ʒ������", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutQuantity1_4", "�̵����������","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            base.AddColumn("OutQuantity2_4", "�̵���⸨����","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("OutMoney_4", "�̵������", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutQuantity1_5", "�˻�����������","number:10,4", Infragistics.Win.HAlign.Right,"",true,RptSummaryType.Sum);
            base.AddColumn("OutQuantity2_5", "�˻����⸨����", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("OutMoney_5", "�˻�������", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutQuantity1_6", "���޳���������", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutQuantity2_6", "���޳��⸨����", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            if (_ShowMoney) base.AddColumn("OutMoney_6", "���޳�����", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            return base.Initialize();
        }

        public override void BeforeQuery(object sender, frmReport.BeforeQueryEventArgs e)
        {
            ////�Ȳ�ѯ����ǰ�����
            //DataSet ds = CSystem.Sys.Svr.cntMain.Select(@"Select Value from S_BaseInfo where ID='ϵͳ\��ǰ�����'");
            //if (ds.Tables[0].Rows.Count != 1)
            //    e.Cancel = true;
            //DateTime currPeriod = DateTime.Parse((string)ds.Tables[0].Rows[0][0]);
            ////frmReport frm = (frmReport)sender;
            //ReportCondition rc = base.GetCondition("period");
            //UltraDateTimeEditor ute = (UltraDateTimeEditor)rc.Control;
            //DateTime selectPeriod = ute.DateTime;
            //selectPeriod = new DateTime(selectPeriod.Year, selectPeriod.Month, 1);
            //if (currPeriod <= selectPeriod)
            //{
            //    e.SQL = "Select * " +
            //            "from (select '${period.begin}' CheckDate,WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,Length,sum(BeginQuantity1) BeginQuantity1,sum(BeginQuantity2) BeginQuantity2,sum(BeginMoney) BeginMoney,sum(InQuantity1) InQuantity1,sum(InQuantity2) InQuantity2,sum(InMoney) InMoney,sum(InQuantity1_0) InQuantity1_0,sum(InQuantity2_0) InQuantity2_0,sum(InMoney_0) InMoney_0,sum(InQuantity1_1) InQuantity1_1,sum(InQuantity2_1) InQuantity2_1,sum(InMoney_1) InMoney_1,sum(InQuantity1_2) InQuantity1_2,sum(InQuantity2_2) InQuantity2_2,sum(InMoney_2) InMoney_2,sum(InQuantity1_3) InQuantity1_3,sum(InQuantity2_3) InQuantity2_3,sum(InMoney_3) InMoney_3,sum(OutQuantity1) OutQuantity1,sum(OutQuantity2) OutQuantity2,sum(OutMoney) OutMoney,sum(OutQuantity1_0) OutQuantity1_0,sum(OutQuantity2_0) OutQuantity2_0,sum(OutMoney_0) OutMoney_0,sum(OutQuantity1_1) OutQuantity1_1,sum(OutQuantity2_1) OutQuantity2_1,sum(OutMoney_1) OutMoney_1,sum(OutQuantity1_2) OutQuantity1_2,sum(OutQuantity2_2) OutQuantity2_2,sum(OutMoney_2) OutMoney_2,sum(OutQuantity1_3) OutQuantity1_3,sum(OutQuantity2_3) OutQuantity2_3,sum(OutMoney_3) OutMoney_3,sum(OutQuantity1_4) OutQuantity1_4,sum(OutQuantity2_4) OutQuantity2_4,sum(OutMoney_4) OutMoney_4,sum(OutQuantity1_5) OutQuantity1_5,sum(OutQuantity2_5) OutQuantity2_5,sum(OutMoney_5) OutMoney_5,sum(OutQuantity1_6) OutQuantity1_6,sum(OutQuantity2_6) OutQuantity2_6,sum(OutMoney_6) OutMoney_6,sum(BeginQuantity1)+sum(InQuantity1)-sum(OutQuantity1) EndQuantity1,sum(BeginQuantity2)+sum(InQuantity2)-sum(OutQuantity2) EndQuantity2,sum(BeginMoney)+sum(InMoney)-sum(OutMoney) EndMoney " +
            //            "from ( " +
            //            "select WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,Length,BeginQuantity1,BeginQuantity2,BeginMoney,0 InQuantity1,0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseBalance where CheckDate= '" + currPeriod + "' " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,sum(Quantity1) BeginQuantity1,sum(Quantity2) BeginQuantity2,sum(Money) BeginMoney,0 InQuantity1, 0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseIn A,D_WarehouseInBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='" + currPeriod + "' and A.BillDate<'${period.begin}' ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,-sum(Quantity1) BeginQuantity1,-sum(Quantity2) BeginQuantity2,-sum(Money) BeginMoney,0 InQuantity1, 0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseOut A,D_WarehouseOutBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='" + currPeriod + "' and A.BillDate<'${period.begin}' ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,0 BeginQuantity1,0 BeginQuantity2,0 BeginMoney,sum(Quantity1) InQuantity1, sum(Quantity2) InQuantity2,sum(Money) InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseIn A,D_WarehouseInBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}' ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,0 BeginQuantity1,0 BeginQuantity2,0 BeginMoney,0 InQuantity1,0 InQuantity2,0 InMoney,sum(Quantity1) InQuantity1_0,sum(Quantity2) InQuantity2_0,sum(Money) InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseIn A,D_WarehouseInBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}' and A.Type=0  ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,0 BeginQuantity1,0 BeginQuantity2,0 BeginMoney,0 InQuantity1,0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,sum(Quantity1) InQuantity1_1,sum(Quantity2) InQuantity2_1,sum(Money) InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseIn A,D_WarehouseInBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}' and A.Type=1  ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,0 BeginQuantity1,0 BeginQuantity2,0 BeginMoney,0 InQuantity1,0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,sum(Quantity1) InQuantity1_2,sum(Quantity2) InQuantity2_2,sum(Money) InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseIn A,D_WarehouseInBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}' and A.Type=2 ${IncludeUncheckData}  " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,0 BeginQuantity1,0 BeginQuantity2,0 BeginMoney,0 InQuantity1,0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,sum(Quantity1) InQuantity1_3,sum(Quantity2) InQuantity2_3,sum(Money) InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseIn A,D_WarehouseInBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}' and A.Type=3  ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,0 BeginQuantity1,0 BeginQuantity2,0 BeginMoney,0 InQuantity1,0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,sum(Quantity1) OutQuantity1, sum(Quantity2) OutQuantity2,sum(Money) OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseOut A,D_WarehouseOutBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}'  ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,0 BeginQuantity1,0 BeginQuantity2,0 BeginMoney,0 InQuantity1,0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,sum(Quantity1) OutQuantity1_0,sum(Quantity2) OutQuantity2_0,sum(Money) OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseOut A,D_WarehouseOutBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}' and A.Type=0  ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,0 BeginQuantity1,0 BeginQuantity2,0 BeginMoney,0 InQuantity1,0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,sum(Quantity1) OutQuantity1_1,sum(Quantity2) OutQuantity2_1,sum(Money) OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseOut A,D_WarehouseOutBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}' and A.Type=1  ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,0 BeginQuantity1,0 BeginQuantity2,0 BeginMoney,0 InQuantity1,0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,sum(Quantity1) OutQuantity1_2,sum(Quantity2) OutQuantity2_2,sum(Money) OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseOut A,D_WarehouseOutBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}' and A.Type=2  ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,0 BeginQuantity1,0 BeginQuantity2,0 BeginMoney,0 InQuantity1,0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,sum(Quantity1) OutQuantity1_3,sum(Quantity2) OutQuantity2_3,sum(Money) OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseOut A,D_WarehouseOutBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}' and A.Type=3  ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,0 BeginQuantity1,0 BeginQuantity2,0 BeginMoney,0 InQuantity1,0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,sum(Quantity1) OutQuantity1_4,sum(Quantity2) OutQuantity2_4,sum(Money) OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseOut A,D_WarehouseOutBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}' and A.Type=4  ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,0 BeginQuantity1,0 BeginQuantity2,0 BeginMoney,0 InQuantity1,0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,sum(Quantity1) OutQuantity1_5,sum(Quantity2) OutQuantity2_5,sum(Money) OutMoney_5,0 OutQuantity1_6,0 OutQuantity2_6,0 OutMoney_6 from D_WarehouseOut A,D_WarehouseOutBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}' and A.Type=5  ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            "union all " +
            //            "select WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length,0 BeginQuantity1,0 BeginQuantity2,0 BeginMoney,0 InQuantity1,0 InQuantity2,0 InMoney,0 InQuantity1_0,0 InQuantity2_0,0 InMoney_0,0 InQuantity1_1,0 InQuantity2_1,0 InMoney_1,0 InQuantity1_2,0 InQuantity2_2,0 InMoney_2,0 InQuantity1_3,0 InQuantity2_3,0 InMoney_3,0 OutQuantity1,0 OutQuantity2,0 OutMoney,0 OutQuantity1_0,0 OutQuantity2_0,0 OutMoney_0,0 OutQuantity1_1,0 OutQuantity2_1,0 OutMoney_1,0 OutQuantity1_2,0 OutQuantity2_2,0 OutMoney_2,0 OutQuantity1_3,0 OutQuantity2_3,0 OutMoney_3,0 OutQuantity1_4,0 OutQuantity2_4,0 OutMoney_4,0 OutQuantity1_5,0 OutQuantity2_5,0 OutMoney_5,sum(Quantity1) OutQuantity1_6,sum(Quantity2) OutQuantity2_6,sum(Money) OutMoney_6 from D_WarehouseOut A,D_WarehouseOutBill B " +
            //            "where A.ID=B.MainID and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}' and A.Type=6  ${IncludeUncheckData} " +
            //            "group by WarehouseID,B.PositionID,MaterialID,IngredientID,MachiningStandardID,Length " +
            //            ") A " +
            //            "group   by WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,Length) A " +
            //          "inner join (select ID BID,Name WarehouseName from P_Warehouse) B on A.WarehouseID=B.BID " +
            //          "inner join (select ID CID,Name PositionName from P_Position) C on A.PositionID=C.CID " +
            //          "inner join (select ID DID,Name MaterialName,Code MaterialCode,Spec,MaterialClassID from P_Material) D on A.MaterialID=D.DID " +
            //          "left join (select ID GID,Name MaterialClassName from P_MaterialClass) G on D.MaterialClassID=G.GID " +
            //          "left join (select ID EID,Name IngredientName from P_Ingredient) E on A.IngredientID=E.EID " +
            //          "left join (select ID FID,Name MachiningStandardName,Code MachiningStandardCode,Text1 from P_MachiningStandard) F on A.MachiningStandardID=F.FID " +
            //          " ${Where} ";
            //}
            //if (((CheckBox)(base.GetCondition("chkPosition").Control)).Checked)
            //{
            //    e.SQL = "Select sum(A.BeginQuantity1) BeginQuantity1," +
            //            "sum(A.BeginQuantity2) BeginQuantity2," +
            //            "sum(A.BeginMoney) BeginMoney," +
            //            "sum(A.InQuantity1) InQuantity1," +
            //            "sum(A.InQuantity2) InQuantity2," +
            //            "sum(A.InMoney) InMoney," +
            //            "sum(A.InQuantity1_0) InQuantity1_0," +
            //            "sum(A.InQuantity2_0) InQuantity2_0," +
            //            "sum(A.InMoney_0) InMoney_0," +
            //            "sum(A.InQuantity1_1) InQuantity1_1," +
            //            "sum(A.InQuantity2_1) InQuantity2_1," +
            //            "sum(A.InMoney_1) InMoney_1," +
            //            "sum(A.InQuantity1_2) InQuantity1_2," +
            //            "sum(A.InQuantity2_2) InQuantity2_2," +
            //            "sum(A.InMoney_2) InMoney_2," +
            //            "sum(A.InQuantity1_3) InQuantity1_3," +
            //            "sum(A.InQuantity2_3) InQuantity2_3," +
            //            "sum(A.InMoney_3) InMoney_3," +
            //            "sum(A.OutQuantity1) OutQuantity1," +
            //            "sum(A.OutQuantity2) OutQuantity2," +
            //            "sum(A.OutMoney) OutMoney," +
            //            "sum(A.OutQuantity1_0) OutQuantity1_0," +
            //            "sum(A.OutQuantity2_0) OutQuantity2_0," +
            //            "sum(A.OutMoney_0) OutMoney_0," +
            //            "sum(A.OutQuantity1_1) OutQuantity1_1," +
            //            "sum(A.OutQuantity2_1) OutQuantity2_1," +
            //            "sum(A.OutMoney_0) OutMoney_0," +
            //            "sum(A.OutQuantity1_2) OutQuantity1_2," +
            //            "sum(A.OutQuantity2_2) OutQuantity2_2," +
            //            "sum(A.OutMoney_2) OutMoney_2," +
            //            "sum(A.OutQuantity1_3) OutQuantity1_3," +
            //            "sum(A.OutQuantity2_3) OutQuantity2_3," +
            //            "sum(A.OutMoney_3) OutMoney_3," +
            //            "sum(A.OutQuantity1_4) OutQuantity1_4," +
            //            "sum(A.OutQuantity2_4) OutQuantity2_4," +
            //            "sum(A.OutMoney_4) OutMoney_4," +
            //            "sum(A.OutQuantity1_5) OutQuantity1_5," +
            //            "sum(A.OutQuantity2_5) OutQuantity2_5," +
            //            "sum(A.OutMoney_5) OutMoney_5," +
            //            "sum(A.OutQuantity1_6) OutQuantity1_6," +
            //            "sum(A.OutQuantity2_6) OutQuantity2_6," +
            //            "sum(A.OutMoney_6) OutMoney_6," +
            //            "sum(A.EndQuantity1) EndQuantity1," +
            //            "sum(A.EndQuantity2) EndQuantity2," +
            //            "sum(A.EndMoney) EndMoney," +
            //            "A.MaterialClassName,A.Length,A.WarehouseName,A.MaterialCode,A.MaterialName,A.Spec,A.IngredientName,A.MachiningStandardName,A.MachiningStandardCode,A.Text1 " +
            //            ",A.WarehouseID,A.MaterialID,A.IngredientID,A.MachiningStandardID " +
            //            "from (" + e.SQL + ") A " +
            //            "Group By A.MaterialClassName,A.Length,A.WarehouseName,A.MaterialCode,A.MaterialName,A.Spec,A.IngredientName,A.MachiningStandardName,A.MachiningStandardCode,A.Text1 " +
            //            ",A.WarehouseID,A.MaterialID,A.IngredientID,A.MachiningStandardID ";
            //}
        }

        public override string BeforeSelectForm(ReportCondition condition)
        {
            if (condition.Name == "PositionID")
            {
                ReportCondition rc = base.GetCondition("WarehouseID");
                if (rc != null)
                {
                    UltraTextEditor txt = rc.Control as UltraTextEditor;
                    if (txt != null && txt.Tag != null)
                    {
                        return string.Format("P_Position.WarehouseID = '{0}'", (Guid)txt.Tag);
                    }
                }
            }
            else if (condition.Name == "WarehouseID")
            {
                if (_ShowOther)
                    return "P_Warehouse.Warehousetype=4";
                else
                    return "P_Warehouse.Warehousetype<>4";
            }
            else if (condition.Name == "MaterialID")
            {
                ReportCondition rc = base.GetCondition("WarehouseID");
                if (rc != null)
                {
                    UltraTextEditor txt = rc.Control as UltraTextEditor;
                    if (txt != null && txt.Tag != null)
                    {
                        return string.Format(" P_Material.MaterialType = (Select WarehouseType from P_Warehouse where ID='{0}')", (Guid)txt.Tag);
                    }
                }
            }
            else if (condition.Name == "MachiningStandardID")
            {
                ReportCondition rc = base.GetCondition("MaterialID");
                if (rc != null)
                {
                    UltraTextEditor txt = rc.Control as UltraTextEditor;
                    if (txt != null && txt.Tag!=null)
                    {
                        return string.Format(" P_MachiningStandard.MaterialID in (select WIPID from P_Material where ID= '{0}')", (Guid)txt.Tag);
                    }
                }
            }
            return "";
        }
        public override bool Strike(Form frm, Infragistics.Win.UltraWinGrid.UltraGridRow row)
        {
            //BaseReport rpt = new clsRptWarehouseInOutDetail();
            //frmReport frmNew = new frmReport(rpt, frm.MdiParent);
            
            //DateTime date=(DateTime)((UltraDateTimeEditor)this.GetCondition("period").Control).Value;
            //UltraDateTimeEditor startDate = (UltraDateTimeEditor)rpt.GetCondition("startDate").Control;
            //date= new DateTime(date.Year,date.Month,1);
            //startDate.Value = date;
            //UltraDateTimeEditor endDate = (UltraDateTimeEditor)rpt.GetCondition("endDate").Control;
            //endDate.Value = date.AddMonths(1).AddDays(-1);
            //UltraTextEditor warehouse = (UltraTextEditor)rpt.GetCondition("WarehouseID").Control;
            //warehouse.Tag = row.Cells["WarehouseID"].Value;
            //warehouse.Text = row.Cells["WarehouseName"].Value as string;
            //if (row.Band.Columns.Exists("PositionID"))
            //{
            //    UltraTextEditor Position = (UltraTextEditor)rpt.GetCondition("PositionID").Control;
            //    Position.Tag = row.Cells["PositionID"].Value;
            //    Position.Text = row.Cells["PositionName"].Value as string;
            //}
            //UltraTextEditor Material = (UltraTextEditor)rpt.GetCondition("MaterialID").Control;
            //Material.Tag = row.Cells["MaterialID"].Value;
            //Material.Text = row.Cells["MaterialName"].Value as string;
            //UltraTextEditor Ingredient = (UltraTextEditor)rpt.GetCondition("IngredientID").Control;
            //Ingredient.Tag = row.Cells["IngredientID"].Value;
            //Ingredient.Text = row.Cells["IngredientName"].Value as string;
            //UltraTextEditor MachiningStandard = (UltraTextEditor)rpt.GetCondition("MachiningStandardID").Control;
            //MachiningStandard.Tag = row.Cells["MachiningStandardID"].Value;
            //MachiningStandard.Text = row.Cells["MachiningStandardName"].Value as string;
            //UltraCurrencyEditor length = (UltraCurrencyEditor)rpt.GetCondition("length").Control;
            //length.Value = (decimal)row.Cells["Length"].Value;
            
            //frmNew.Show();
            //frmNew.Query();
            return true;
        }
        public override Form GetForm(Base.CRightItem right, Form mdiForm)
        {
            frmReport frm = (frmReport)base.GetForm(right, mdiForm);

            if (right.Paramters == null)
                return frm;
            string[] s = right.Paramters.Split(new char[] { ',' });

            if (s.Length > 0)
                this._ShowMoney = int.Parse(s[0]) == 1;
            else
                this._ShowMoney = false;
            if (s.Length > 1)
                this._ShowOther = int.Parse(s[1]) == 1;
            else
                this._ShowOther = false;
            return frm;
        }
    }
}
