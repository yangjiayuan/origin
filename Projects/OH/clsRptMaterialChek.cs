using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinEditors;
using System.Data;
using UI;

namespace OH
{
    public class clsRptMaterialChek:BaseReport
    {
        public override bool Initialize()
        {
            _Title = "���Ͽ���";
            _SQL = "select CT.Name MaterialTypeName,C.MaterialCheckObjectID,C.MachineID,E.Name ProcessName,D.Name MachineName,D.Leader,F.Name MaterialCheckObjectName,C.Quantity,M1.Name CheckObjectMeasureName,C.Output, M2.Name OutMeasureName," +
                    "C.FactQty,C.FactOutput,C.SaveQty,C.SaveQty*F.Price SaveMny,(case when C.SaveQty>0 then C.SaveQty*C.Reward else -1*C.SaveQty*C.Punish end) FactReward,C.Reward,C.Punish " +
                    "from " +
                    "(select isnull(A.Qty,0) FactQty,isnull(sum(B.Qty),0) FactOutput,(A.Quantity*isnull(sum(B.Qty),0))/A.Output-isnull(A.Qty,0) SaveQty,A.MachineID,A.CheckObjectMeasureID,A.OutMeasureID,A.MaterialCheckObjectID,A.MaterialCheckTypeID,A.Output,A.Quantity,A.Reward,A.Punish from " +
                        "(select C.MachineID,C.CheckObjectMeasureID,C.OutMeasureID,C.MaterialCheckObjectID,C.MaterialCheckTypeID,C.Output,C.Quantity,C.Reward,C.Punish,D.IsCheckAmount,D.CheckCountType,(case D.IsCheckAmount when 0 then (case D.CheckType when 0 then Sum(WO.Quantity1) else  Sum(WO.Quantity2) end ) else Sum(WO.Money) end) Qty from " +
                            "(Select * from C_CheckStandardBill where MainID='${CheckStandard}') C left join "+
                            "(Select distinct A.ID,A.CheckType,A.IsCheckAmount,1 CheckCountType from P_MaterialCheckObject A " +
                                        "where exists(select id from P_MaterialCheckObjectCraft B where A.ID=B.MainID) " +
                                    "union Select distinct A.ID,A.CheckType,A.IsCheckAmount,2 CheckCountType from P_MaterialCheckObject A  " +
                                        "where exists(select id from P_MaterialCheckObjectAddition B where A.ID=B.MainID) " +
                                            "and not exists(select id from P_MaterialCheckObjectCraft B where A.ID=B.MainID) " +
                                    "union Select distinct A.ID,A.CheckType,A.IsCheckAmount,0 CheckCountType from P_MaterialCheckObject A  " +
                                        "where not exists(select id from P_MaterialCheckObjectCraft B where A.ID=B.MainID) " +
                                            "and not exists(select id from P_MaterialCheckObjectAddition B where A.ID=B.MainID)) D on C.MaterialCheckObjectID=D.ID left join " +
                            "(Select B.*,C.MaterialCheckObjectID from D_WarehouseOut A,D_WarehouseOutBill B,P_Material C where A.ID=B.MainID and B.MaterialID=C.ID and C.MaterialType=4  " +
                                    "and A.BillDate>='${period.begin}' and A.BillDate<'${period.end}') WO  on WO.MachineID=C.MachineID and WO.MaterialCheckObjectID=D.ID " +
                        "group by C.MachineID,C.CheckObjectMeasureID,C.OutMeasureID,C.MaterialCheckObjectID,C.MaterialCheckTypeID,C.Output,C.Quantity,C.Reward,C.Punish,D.IsCheckAmount,D.CheckType,D.CheckCountType) A " +
                        "left join " +
                        "(select A.MachineID,B.CraftID,B.AdditionID,Sum(B.Quantity) Qty from D_WorkOrder A,D_WorkOrderProduct B where A.ID=B.MainID " +
                            "and A.Date>='${period.begin}' and A.Date<'${period.end}' "+
                            "group by A.MachineID,B.CraftID,B.AdditionID) B on B.MachineID=A.MachineID " +
                        " left join P_MaterialCheckObjectCraft E on A.MaterialCheckObjectID=E.MainID and B.CraftID=E.CraftID left join P_MaterialCheckObjectAddition F on A.MaterialCheckObjectID=F.MainID and F.AdditionID=B.AdditionID " +
                        "where (A.CheckCountType=0 or A.CheckCountType is null or (A.CheckCountType=1 and not E.ID is null) or (A.CheckCountType=2 and not F.ID is null)) " +
                        "Group by A.MachineID,A.CheckObjectMeasureID,A.OutMeasureID,A.MaterialCheckObjectID,A.MaterialCheckTypeID,A.Output,A.Quantity,A.Reward,A.Punish,isnull(A.Qty,0)" +
                    ") C inner join P_Machine D on C.MachineID=D.ID " +
                    " inner join P_Process E on D.ProcessID=E.ID "+
                    " inner join P_MaterialCheckObject F on F.ID=C.MaterialCheckObjectID "+
                    " inner join P_Measure M1 on C.CheckObjectMeasureID=M1.ID "+
                    " inner join P_Measure M2 on C.OutMeasureID=M2.ID " +
                    " left join P_MaterialCheckType CT on C.MaterialCheckTypeID=CT.ID "+
                    " ${Where}";
            _IsAppendSQL = false;
            _IsWhere = true;
            base.AddCondition("period", "�ڼ�", "Period", "");
            base.AddCondition("FactoryID", "����", "Dict:P_Factory", "E.FactoryID in (select ID from P_Factory where ID='{0}')");
            base.AddCondition("DepartmentID", "����", "Dict:P_Department", "D.DepartmentID in (select ID from P_Department where ID='{0}')");
            base.AddCondition("ProcessID", "����", "Dict:P_Process", "D.ProcessID='{0}'");
            base.AddCondition("MachineID", "�豸", "Dict:P_Machine", "C.MachineID='{0}'");
            base.AddCondition("CheckStandard", "���˱�׼", "Data:C_CheckStandard", "");

            //����,�豸,������,���˶���,��׼����,������λ,������׼,ʵ������,ʵ�ʲ���,��Լ����,��Լ����(���˶�����ĵ���),��Լ���,��,��
            //���ӡ����Ͽ������
            base.AddColumn("MaterialTypeName", "���Ͽ������");
            base.AddColumn("ProcessName", "����");
            base.AddColumn("MachineName", "�豸");
            base.AddColumn("Leader", "������");
            base.AddColumn("MaterialCheckObjectName", "���˶���");
            base.AddColumn("Quantity", "��׼����", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("CheckObjectMeasureName", "���˵�λ");
            base.AddColumn("Output", "������׼", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutMeasureName", "������λ");
            base.AddColumn("FactQty", "ʵ������", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("FactOutput", "ʵ�ʲ���", "int", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("SaveQty", "��Լ����", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Price", "��Լ����", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("SaveMny", "��Լ���", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("FactReward", "��(��Ϊ����)", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            //base.AddColumn("FactPunish", "��", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Reward", "��(��׼)", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Punish", "��(��׼)", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            //����������ϸ
            string detailSQL = "select A.WorkCode,A.BillDate,E.Name MachineName,F.Name OperatorName,C.Name CheckObjectName,M.Code MaterialCode,M.Name MaterialName,B.Quantity1,B.Price,B.Money,D.Name MeausreName " +
                                "from D_WarehouseOut A,D_WarehouseOutBill B,P_Material M,P_MaterialCheckObject C,P_Measure D ,P_Machine E,P_Operator F " +
                                "where A.ID=B.MainID and B.MaterialID=M.ID and M.MaterialCheckObjectID=C.ID and M.FirMeasureID=D.ID and B.MachineID=E.ID and F.ID=B.OperatorID " +
                                "and B.MachineID='{0}' and M.MaterialCheckObjectID='{1}' and A.BillDate>='{2}' and A.BillDate<'{3}'";
            ReportDetialButton rdb = new ReportDetialButton("������ϸ", "Detail", detailSQL);
            rdb.Columns.Add(new ReportColumn("WorkCode", "���ϵ���"));
            rdb.Columns.Add(new ReportColumn("BillDate", "����"));
            rdb.Columns.Add(new ReportColumn("MachineName","�豸"));
            rdb.Columns.Add(new ReportColumn("OperatorName","������"));
            rdb.Columns.Add(new ReportColumn("CheckObjectName","���˽���"));
            rdb.Columns.Add(new ReportColumn("MaterialCode", "���ϴ���"));
            rdb.Columns.Add(new ReportColumn("MaterialName", "��������"));
            rdb.Columns.Add(new ReportColumn("Quantity1", "����", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.Sum));
            rdb.Columns.Add(new ReportColumn("Price", "����", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.None));
            rdb.Columns.Add(new ReportColumn("Money", "���", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.Sum));
            rdb.Columns.Add(new ReportColumn("MeasureName","��λ"));
            base.DetailButtons.Add(rdb);

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
        public override void BeforeQuery(object sender, frmReport.BeforeQueryEventArgs e)
        {
            //�Ȳ�ѯ�����˰汾
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(@"Select * from S_BaseInfo where ID in ('���Ͽ���\���˰汾','ϵͳ\��ǰ�����')");
            if (ds.Tables[0].Rows.Count != 2)
                e.Cancel = true;
            Guid checkStandardID = Guid.NewGuid();
            DateTime currentPeriod= DateTime.Now;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                switch ((string)dr["ID"])
                {
                    case @"���Ͽ���\���˰汾":
                        checkStandardID = new Guid((string)dr["Value"]);
                        break;
                    case @"ϵͳ\��ǰ�����":
                        currentPeriod = DateTime.Parse((string)dr["Value"]);
                        break;
                }
            }
            
            ReportCondition rc = base.GetCondition("CheckStandard");
            UltraTextEditor ute = (UltraTextEditor)rc.Control;
            if (ute.Tag != null)
            {
                checkStandardID = (Guid)ute.Tag;
            }
            rc=base.GetCondition("period");
            UltraDateTimeEditor dat = (UltraDateTimeEditor)rc.Control;
            DateTime selectPeriod=new DateTime(dat.DateTime.Year,dat.DateTime.Month,1);

            if (selectPeriod >= currentPeriod)
            {
                if (ute.Tag == null)
                    ute.Tag = checkStandardID;
                return;
            }
            //��ѯ��ʷ����û��������˱�׼
            ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select Distinct SourceMainID from C_CheckStandardBillOld where Period='{0}'", selectPeriod));
            if (ds.Tables[0].Rows.Count==0)
            {
                if (ute.Tag==null)
                    ute.Tag = checkStandardID;
                //e.Cancel=true;
            }
            else
            {
                e.SQL = e.SQL.Replace("C_CheckStandardBill where MainID=",string.Format( "C_CheckStandardBillOld where Period='{0}' and SourceMainID=",selectPeriod));
                if (ute.Tag==null)
                    ute.Tag = ds.Tables[0].Rows[0]["SourceMainID"];
            }
        }
        public override string DetailButtonClick(ReportDetialButton rdb, Infragistics.Win.UltraWinGrid.UltraGridRow Row)
        {
            if (Row == null) return null;
            DateTime begin = ((UltraDateTimeEditor)base.GetCondition("period").Control).DateTime;
            begin = new DateTime(begin.Year, begin.Month, 1);
            DateTime end=begin.AddMonths(1);
            return string.Format(rdb.SQL,Row.Cells["MachineID"].Value, Row.Cells["MaterialCheckObjectID"].Value, begin, end);
        }
    }
}
