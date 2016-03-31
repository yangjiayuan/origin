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
            _Title = "物料考核";
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
            base.AddCondition("period", "期间", "Period", "");
            base.AddCondition("FactoryID", "工厂", "Dict:P_Factory", "E.FactoryID in (select ID from P_Factory where ID='{0}')");
            base.AddCondition("DepartmentID", "部门", "Dict:P_Department", "D.DepartmentID in (select ID from P_Department where ID='{0}')");
            base.AddCondition("ProcessID", "工序", "Dict:P_Process", "D.ProcessID='{0}'");
            base.AddCondition("MachineID", "设备", "Dict:P_Machine", "C.MachineID='{0}'");
            base.AddCondition("CheckStandard", "考核标准", "Data:C_CheckStandard", "");

            //工序,设备,负责人,考核对象,标准数量,产量单位,产量标准,实际领用,实际产量,节约数量,节约单价(考核对象里的单价),节约金额,奖,罚
            //增加　物料考核类别
            base.AddColumn("MaterialTypeName", "物料考核类别");
            base.AddColumn("ProcessName", "工序");
            base.AddColumn("MachineName", "设备");
            base.AddColumn("Leader", "负责人");
            base.AddColumn("MaterialCheckObjectName", "考核对象");
            base.AddColumn("Quantity", "标准数量", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("CheckObjectMeasureName", "考核单位");
            base.AddColumn("Output", "产量标准", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutMeasureName", "产量单位");
            base.AddColumn("FactQty", "实际领用", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("FactOutput", "实际产量", "int", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("SaveQty", "节约数量", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Price", "节约单价", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("SaveMny", "节约金额", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("FactReward", "奖(罚为负数)", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            //base.AddColumn("FactPunish", "罚", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Reward", "奖(标准)", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Punish", "罚(标准)", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            //增加领料明细
            string detailSQL = "select A.WorkCode,A.BillDate,E.Name MachineName,F.Name OperatorName,C.Name CheckObjectName,M.Code MaterialCode,M.Name MaterialName,B.Quantity1,B.Price,B.Money,D.Name MeausreName " +
                                "from D_WarehouseOut A,D_WarehouseOutBill B,P_Material M,P_MaterialCheckObject C,P_Measure D ,P_Machine E,P_Operator F " +
                                "where A.ID=B.MainID and B.MaterialID=M.ID and M.MaterialCheckObjectID=C.ID and M.FirMeasureID=D.ID and B.MachineID=E.ID and F.ID=B.OperatorID " +
                                "and B.MachineID='{0}' and M.MaterialCheckObjectID='{1}' and A.BillDate>='{2}' and A.BillDate<'{3}'";
            ReportDetialButton rdb = new ReportDetialButton("领用明细", "Detail", detailSQL);
            rdb.Columns.Add(new ReportColumn("WorkCode", "领料单号"));
            rdb.Columns.Add(new ReportColumn("BillDate", "日期"));
            rdb.Columns.Add(new ReportColumn("MachineName","设备"));
            rdb.Columns.Add(new ReportColumn("OperatorName","领料人"));
            rdb.Columns.Add(new ReportColumn("CheckObjectName","考核结象"));
            rdb.Columns.Add(new ReportColumn("MaterialCode", "物料代码"));
            rdb.Columns.Add(new ReportColumn("MaterialName", "物料名称"));
            rdb.Columns.Add(new ReportColumn("Quantity1", "数量", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.Sum));
            rdb.Columns.Add(new ReportColumn("Price", "单价", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.None));
            rdb.Columns.Add(new ReportColumn("Money", "金额", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.Sum));
            rdb.Columns.Add(new ReportColumn("MeasureName","单位"));
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
            //先查询出考核版本
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(@"Select * from S_BaseInfo where ID in ('物料考核\考核版本','系统\当前会计期')");
            if (ds.Tables[0].Rows.Count != 2)
                e.Cancel = true;
            Guid checkStandardID = Guid.NewGuid();
            DateTime currentPeriod= DateTime.Now;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                switch ((string)dr["ID"])
                {
                    case @"物料考核\考核版本":
                        checkStandardID = new Guid((string)dr["Value"]);
                        break;
                    case @"系统\当前会计期":
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
            //查询历史上有没有这个考核标准
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
