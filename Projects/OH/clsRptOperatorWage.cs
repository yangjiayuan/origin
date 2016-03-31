using System;
using System.Collections.Generic;
using System.Text;
using Base;
using UI;
using System.Data;
using Infragistics.Win.UltraWinEditors;

namespace OH
{
    public class clsRptOperatorWage:BaseReport
    {
        public override bool Initialize()
        {
            _SQL = "select A.ID,A.Date,U2.Code WorkerCode,U2.Name WorkerName,C.Name DepartmentName,A.NO WorkOrderCode,A.Class,A.ClassType,D.Name MachineName," +
                        "U1.Name CheckerName,A.Memo WorkOrderNotes,B.Rate,B.Ratio,B.Amount " +
                    "from D_WorkOrder A,D_workOrderOperator B,P_Department C,P_Machine D,P_Operator U1,P_Operator U2,P_Station G,P_OperatorProperty H " +
                    "where A.ID=B.MainID and A.OperatorID=U1.ID and B.OperatorID=U2.ID and U2.StationID=G.ID and U2.OperatorPropertyID=H.ID "+
                        " and C.ID=A.DepartmentID and A.MachineID=D.ID";
            _IsAppendSQL = true;
            _IsWhere = false;
            _Title = "���˹��ʲ�ѯ";

            base.AddCondition("startDate", "��ʼʱ��", "Date", "A.Date>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "����ʱ��", "Date", "A.Date<='{0:yyyy-MM-dd}'");
            base.AddCondition("Class", "����", "Enum:0=ȫ��,1=A��,2=B��,3=C��", "(0={0} or A.Class={0})");
            base.AddCondition("ClassType", "���", "Enum:2=ȫ��,0=�װ�,1=ҹ��", "(2={0} or A.ClassType={0})");
            base.AddCondition("Factory", "����", "Dict:P_Factory", "A.DepartmentID in (Select ID from P_Department where FactoryID='{0}')");
            base.AddCondition("Department", "����", "Dict:P_Department", "A.DepartmentID='{0}'");
            base.AddCondition("Process", "����", "Dict:P_Process", "A.MachineID in ( Select ID from P_Machine where ProcessID='{0}')");
            base.AddCondition("MachineID", "�豸", "Dict:P_Machine", "A.MachineID='{0}'");

            base.AddCondition("OperatorID", "��Ա", "Dict:P_Operator", "B.OperatorID='{0}'");
            //base.AddCondition("period", "�·�", "period", "A.Date>='{0:yyyy-MM-dd}' and A.Date<'{1:yyyy-MM-dd}'");

            //���ڡ����š����������š���Ʊ�š�
            //����豸������Ա����ע�������������������� ���˱�ע
            base.AddColumn("Date", "����", "date", Infragistics.Win.HAlign.Default, "yyyy-MM-dd", true);
            base.AddColumn("WorkerCode", "����");
            base.AddColumn("WorkerName", "����");
            base.AddColumn("DepartmentName", "����");
            base.AddColumn("WorkOrderCode", "��Ʊ��");
            base.AddColumn("Class", "����", "Enum:0=ȫ��,1=A��,2=B��,3=C��", Infragistics.Win.HAlign.Default, "", true, RptSummaryType.None);
            base.AddColumn("ClassType", "���", "Enum:2=ȫ��,0=�װ�,1=ҹ��", Infragistics.Win.HAlign.Default, "", true, RptSummaryType.None);
            base.AddColumn("MachineName", "�豸");
            base.AddColumn("CheckerName", "����Ա");
            base.AddColumn("WorkOrderNotes", "��ע");
            base.AddColumn("Rate", "�������");
            base.AddColumn("Ratio", "�ۼ�����");
            base.AddColumn("Amount", "���", "number:10,2", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            return base.Initialize();
        }
        public override string BeforeSelectForm(ReportCondition condition)
        {
            string s = "";
            ReportCondition rc;
            switch (condition.Name)
            {
                case "OperatorID":
                    return "P_Operator.IsChecker=0";
                case "MachineID":
                    rc = base.GetCondition("Department");
                    if (rc != null)
                    {
                        UltraTextEditor txt = rc.Control as UltraTextEditor;
                        if (txt != null && txt.Tag != null)
                        {
                            s = string.Format("P_Machine.DepartmentID = '{0}'", (Guid)txt.Tag);
                        }
                    }
                    rc = base.GetCondition("Process");
                    if (rc != null)
                    {
                        UltraTextEditor txt = rc.Control as UltraTextEditor;
                        if (txt != null && txt.Tag != null)
                        {
                            s = string.Format("P_Machine.ProcessID = '{0}'", (Guid)txt.Tag);
                        }
                    }
                    break;
                case "Department":
                    rc = base.GetCondition("Factory");
                    if (rc != null)
                    {
                        UltraTextEditor txt = rc.Control as UltraTextEditor;
                        if (txt != null && txt.Tag != null)
                        {
                            return string.Format("P_Department.FactoryID = '{0}'", (Guid)txt.Tag);
                        }
                    }
                    break;
                case "Process":
                    rc = base.GetCondition("Factory");
                    if (rc != null)
                    {
                        UltraTextEditor txt = rc.Control as UltraTextEditor;
                        if (txt != null && txt.Tag != null)
                        {
                            return string.Format("P_Process.FactoryID = '{0}'", (Guid)txt.Tag);
                        }
                    }
                    break;
            }
            return "";
        }

        public override bool Strike(System.Windows.Forms.Form frm, Infragistics.Win.UltraWinGrid.UltraGridRow row)
        {
            if (row.Band.Columns.Exists("ID"))
            {
                COMFields _MainTableDefine = CSystem.Sys.Svr.LDI.GetFields("D_WorkOrder");
                List<COMFields> _DetailTableDefine = CSystem.Sys.Svr.Properties.DetailTableDefineList("D_WorkOrder");

                DataSet ds = CSystem.Sys.Svr.cntMain.Select(_MainTableDefine.QuerySQL + " where D_WorkOrder.ID='" + row.Cells["ID"].Value + "'", _MainTableDefine.OrinalTableName);

                frmDetail form = new frmDetail();
                if (form != null)
                {
                    form.toolDetailForm = new clsWorkOrderDetailForm();
                    bool bShowData = form.ShowData(_MainTableDefine, _DetailTableDefine, new DataSetEventArgs(ds), false, frm.MdiParent);
                }
            }
            return true;
        }
    }
}
