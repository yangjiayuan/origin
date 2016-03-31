using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinEditors;
using System.Data;
using Base;
using UI;

namespace OH
{
    public class clsRptWarehouseOutToMachine:BaseReport
    {
        public override bool Initialize()
        {
            _Title = "领用查询";
            _SQL = "select E.Code MaterialCode,E.Name MaterialName,F.Name ProcessName,D.Name MachineName,C.MaterialCheckObjectID,B.MachineID,sum(B.Quantity1) Qty,sum(B.Money) Mny " +
                    "from D_WarehouseOut A,D_WarehouseOutBill B,P_Material C,P_Machine D,P_MaterialCheckObject E,P_Process F "+
                    "where A.ID=B.MainID and B.MaterialID=C.ID and B.MachineID=D.ID and C.MaterialCheckObjectID=E.ID and D.ProcessID=F.ID " +
                    "${Where} "+
                    "Group By  E.Code,E.Name,F.Name,D.Name,C.MaterialCheckObjectID,B.MachineID";
            _IsAppendSQL = false;
            _IsWhere = false;
            base.AddCondition("startDate", "开始时间", "Date", "A.BillDate>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "结束时间", "Date", "A.BillDate<='{0:yyyy-MM-dd}'");
            base.AddCondition("Machine", "设备", "Dict:P_Machine", "B.MachineID='{0}'");
            base.AddCondition("WarehouseID", "仓库", "Dict:P_Warehouse", "A.WarehouseID='{0}'");
            base.AddCondition("PositionID", "库位", "Dict:P_Position", "B.PositionID='{0}'");

            base.AddColumn("ProcessName", "工序"); 
            base.AddColumn("MachineName", "设备");
            base.AddColumn("MaterialCode", "考核对象代码");
            base.AddColumn("MaterialName", "考核对象名称");
            base.AddColumn("Qty", "数量", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Mny", "金额", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            //增加领料明细
            string detailSQL = "select A.ID,A.BillDate,E.Name MachineName,F.Name OperatorName,M.Code MaterialCode,M.Name MaterialName,B.Quantity1,B.Money,D.Name MeausreName " +
                                "from D_WarehouseOut A,D_WarehouseOutBill B,P_Material M,P_Measure D ,P_Machine E,P_Operator F " +
                                "where A.ID=B.MainID and B.MaterialID=M.ID and M.FirMeasureID=D.ID and B.MachineID=E.ID and F.ID=B.OperatorID " +
                                "and B.MachineID='{0}' and M.MaterialCheckObjectID='{1}' and A.BillDate>='{2}' and A.BillDate<='{3}'";
            ReportDetialButton rdb = new ReportDetialButton("领用明细", "Detail", detailSQL);
            rdb.Columns.Add(new ReportColumn("BillDate", "领料日期"));
            rdb.Columns.Add(new ReportColumn("MachineName", "设备"));
            rdb.Columns.Add(new ReportColumn("OperatorName", "领料人"));
            rdb.Columns.Add(new ReportColumn("MaterialCode", "物料代码"));
            rdb.Columns.Add(new ReportColumn("MaterialName", "物料名称"));
            rdb.Columns.Add(new ReportColumn("MeasureName", "单位"));
            rdb.Columns.Add(new ReportColumn("Quantity1", "数量", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.Sum));
            rdb.Columns.Add(new ReportColumn("Money", "金额", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.Sum));
            

            base.DetailButtons.Add(rdb);

            return base.Initialize();
        }
        public override string BeforeSelectForm(ReportCondition condition)
        {
            if (condition.Name == "WarehouseID")
            {
                return "WarehouseType=4";
            }
            else if (condition.Name == "PositionID")
            {
                ReportCondition rc = base.GetCondition("WarehouseID");
                if (rc != null)
                {
                    UltraTextEditor txt = rc.Control as UltraTextEditor;
                    if (txt != null && txt.Tag!=null)
                    {
                        return string.Format("P_Position.WarehouseID = '{0}'", (Guid)txt.Tag);
                    }
                }
            }
            return "";
        }
        public override string DetailButtonClick(ReportDetialButton rdb, Infragistics.Win.UltraWinGrid.UltraGridRow Row)
        {
            if (Row == null) return null;
            DateTime begin = ((UltraDateTimeEditor)base.GetCondition("startDate").Control).DateTime;
            DateTime end = ((UltraDateTimeEditor)base.GetCondition("endDate").Control).DateTime;
            return string.Format(rdb.SQL, Row.Cells["MachineID"].Value, Row.Cells["MaterialCheckObjectID"].Value, begin, end);
        }
        public override bool Strike(System.Windows.Forms.Form frm, Infragistics.Win.UltraWinGrid.UltraGridRow row)
        {
            if (row.Band.Columns.Exists("ID"))
            {
                COMFields _MainTableDefine = CSystem.Sys.Svr.LDI.GetFields("D_WarehouseOut");
                List<COMFields> _DetailTableDefine = CSystem.Sys.Svr.Properties.DetailTableDefineList("D_WarehouseOut");
                _DetailTableDefine[0]["Money"].Visible = COMField.Enum_Visible.VisibleAll;
                _DetailTableDefine[0]["Money"].Mandatory = true;
                _DetailTableDefine[0]["Price"].Visible = COMField.Enum_Visible.VisibleAll;
                _DetailTableDefine[0]["Price"].Mandatory = true;
                _DetailTableDefine[0]["MachineName"].Visible = COMField.Enum_Visible.VisibleAll;
                _DetailTableDefine[0]["MachineName"].Mandatory = true;
                _DetailTableDefine[0]["MachineName"].Enable = true;
                _DetailTableDefine[0]["OperatorName"].Visible = COMField.Enum_Visible.VisibleAll;
                _DetailTableDefine[0]["OperatorName"].Mandatory = true;
                _DetailTableDefine[0]["OperatorName"].Enable = true;

                DataSet ds = CSystem.Sys.Svr.cntMain.Select(_MainTableDefine.QuerySQL + " where D_WarehouseOut.ID='" + row.Cells["ID"].Value + "'", _MainTableDefine.OrinalTableName);

                frmDetail form = new frmDetail();
                if (form != null)
                {
                    form.toolDetailForm = new DetailFormWithoutEdit();
                    bool bShowData = form.ShowData(_MainTableDefine, _DetailTableDefine, new DataSetEventArgs(ds), false,frm.MdiParent);
                }
            }
            return true;
        }
    }
    public class DetailFormWithoutEdit : ToolDetailForm
    {
        public override bool AllowCheck
        {
            get
            {
                return false;
            }
        }
        public override bool AllowEdit
        {
            get
            {
                return false;
            }
        }
    }
 }
