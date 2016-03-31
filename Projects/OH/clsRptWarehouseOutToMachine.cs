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
            _Title = "���ò�ѯ";
            _SQL = "select E.Code MaterialCode,E.Name MaterialName,F.Name ProcessName,D.Name MachineName,C.MaterialCheckObjectID,B.MachineID,sum(B.Quantity1) Qty,sum(B.Money) Mny " +
                    "from D_WarehouseOut A,D_WarehouseOutBill B,P_Material C,P_Machine D,P_MaterialCheckObject E,P_Process F "+
                    "where A.ID=B.MainID and B.MaterialID=C.ID and B.MachineID=D.ID and C.MaterialCheckObjectID=E.ID and D.ProcessID=F.ID " +
                    "${Where} "+
                    "Group By  E.Code,E.Name,F.Name,D.Name,C.MaterialCheckObjectID,B.MachineID";
            _IsAppendSQL = false;
            _IsWhere = false;
            base.AddCondition("startDate", "��ʼʱ��", "Date", "A.BillDate>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "����ʱ��", "Date", "A.BillDate<='{0:yyyy-MM-dd}'");
            base.AddCondition("Machine", "�豸", "Dict:P_Machine", "B.MachineID='{0}'");
            base.AddCondition("WarehouseID", "�ֿ�", "Dict:P_Warehouse", "A.WarehouseID='{0}'");
            base.AddCondition("PositionID", "��λ", "Dict:P_Position", "B.PositionID='{0}'");

            base.AddColumn("ProcessName", "����"); 
            base.AddColumn("MachineName", "�豸");
            base.AddColumn("MaterialCode", "���˶������");
            base.AddColumn("MaterialName", "���˶�������");
            base.AddColumn("Qty", "����", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("Mny", "���", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            //����������ϸ
            string detailSQL = "select A.ID,A.BillDate,E.Name MachineName,F.Name OperatorName,M.Code MaterialCode,M.Name MaterialName,B.Quantity1,B.Money,D.Name MeausreName " +
                                "from D_WarehouseOut A,D_WarehouseOutBill B,P_Material M,P_Measure D ,P_Machine E,P_Operator F " +
                                "where A.ID=B.MainID and B.MaterialID=M.ID and M.FirMeasureID=D.ID and B.MachineID=E.ID and F.ID=B.OperatorID " +
                                "and B.MachineID='{0}' and M.MaterialCheckObjectID='{1}' and A.BillDate>='{2}' and A.BillDate<='{3}'";
            ReportDetialButton rdb = new ReportDetialButton("������ϸ", "Detail", detailSQL);
            rdb.Columns.Add(new ReportColumn("BillDate", "��������"));
            rdb.Columns.Add(new ReportColumn("MachineName", "�豸"));
            rdb.Columns.Add(new ReportColumn("OperatorName", "������"));
            rdb.Columns.Add(new ReportColumn("MaterialCode", "���ϴ���"));
            rdb.Columns.Add(new ReportColumn("MaterialName", "��������"));
            rdb.Columns.Add(new ReportColumn("MeasureName", "��λ"));
            rdb.Columns.Add(new ReportColumn("Quantity1", "����", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.Sum));
            rdb.Columns.Add(new ReportColumn("Money", "���", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.Sum));
            

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
