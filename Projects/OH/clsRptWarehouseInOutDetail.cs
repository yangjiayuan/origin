   using System;
using System.Collections.Generic;
using System.Text;
using Base;
using UI;
using System.Data;

namespace OH
{
    public class clsRptWarehouseInOutDetail:BaseReport
    {
        public override bool Initialize()
        {
            _SQL="select A.ID,A.Type,A.WorkType,A.Code,A.WorkCode,BO.Code BuyOrderCode,WO.NO WorkOrderCode,A.BillDate,"+
                    "P.Name PositionName,M.Code MaterialCode,M.Name MaterialName,M.Spec,I.Name IngredientName,MS.Code MachiningStandardCode,MS.Name MachiningStandardName,MS.Text1 ProductionStandardName, " +
                    "B.Length,B.Quantity1 InQuantity1,B.Quantity2 InQuantity2,0 OutQuantity1, 0 OutQuantity2 "+
                "from (select O.Type,1 WorkType,O.Code,O.WorkCode,O.BillDate,O.ID,O.WarehouseID,O.WorkOrderID,O.BuyOrderID from D_WarehouseIn O) A inner join D_WarehouseInBill  B on A.ID=B.MainID " +
                    "inner join P_Warehouse W on A.WarehouseID=W.ID  "+
                    "inner join P_Position P on B.PositionID=P.ID  "+
                    "inner join P_Material M on B.MaterialID=M.ID  "+
                    "left join D_BuyOrder BO on A.BuyOrderID=B.ID "+
                    "left join D_WorkOrder WO on A.WorkOrderID=WO.ID "+
                    "left join P_Ingredient I on B.IngredientID=I.ID "+
                    "left join P_MachiningStandard MS on B.MachiningStandardID=MS.ID  ${Where}" +
                "union all "+
                "select A.ID,A.Type Type,A.WorkType,A.Code,A.WorkCode,null BuyOrderCode,WO.Code WorkOrderCode,A.BillDate, "+
                    "P.Name PositionName,M.Code MaterialCode,M.Name MaterialName,M.Spec,I.Name IngredientName,MS.Code MachiningStandardCode,MS.Name MachiningStandardName,MS.Text1 ProductionStandardName, " +
                    "B.Length,0 InQuantity1,0 InQuantity2,B.Quantity1 OutQuantity1,B.Quantity2 OutQuantity2 " +
                "from (select O.Type+10 Type,2 WorkType,O.Code,O.WorkCode,O.BillDate,O.ID,O.WarehouseID,O.WorkOrderID from D_WarehouseOut O) A inner join D_WarehouseOutBill B on A.ID=B.MainID "+
                    "inner join P_Warehouse W on A.WarehouseID=W.ID  "+
                    "inner join P_Position P on B.PositionID=P.ID  "+
                    "inner join P_Material M on B.MaterialID=M.ID  "+
                    "left join D_WorkOrder WO on A.WorkOrderID=WO.ID "+
                    "left join P_Ingredient I on B.IngredientID=I.ID "+
                    "left join P_MachiningStandard MS on B.MachiningStandardID=MS.ID ${Where}";
            _Title = "�������ϸ��";
            _IsWhere = true;
            _IsAppendSQL = false;

            base.AddCondition("WarehouseID", "�ֿ�", "Dict:P_Warehouse", "A.WarehouseID='{0}'");
            base.AddCondition("PositionID", "��λ", "Dict:P_Position", "B.PositionID='{0}'");
            base.AddCondition("MaterialID", "����", "Dict:P_Material", "B.MaterialID='{0}'");
            base.AddCondition("IngredientID", "����", "Dict:P_Ingredient", "B.IngredientID='{0}'");
            base.AddCondition("MachiningStandardID", "�ӹ�ͼ��", "Dict:P_MachiningStandard", "B.MachiningStandardID='{0}'");
            base.AddCondition("length", "����", "number:10,2", "B.Length={0}");
            base.AddCondition("startDate", "��ʼʱ��", "Date", "A.BillDate>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "����ʱ��", "Date", "A.BillDate<='{0:yyyy-MM-dd}'");
            base.AddCondition("Type", "����", "Enum:-1=ȫ��,0=��ͨ,1=�ɹ�,2=�ƿ�,3=����,10=��ͨ,11=����,12=�ƿ�,13=��Ʒ,14=�̵�,15=�˻�,16=����", "({0}=-1 or A.Type={0})");
            base.AddCondition("WorkType", "�����", "Enum:0=ȫ��,1=���,2=����", "({0}=0 or A.WorkType={0})");

            base.AddColumn("Type", "����", "Enum:0=��ͨ,1=�ɹ�,2=�ƿ�,3=����,10=��ͨ,11=����,12=�ƿ�,13=��Ʒ,14=�̵�,15=�˻�,16=����", Infragistics.Win.HAlign.Default, "", true, RptSummaryType.None);
            base.AddColumn("WorkType", "�����", "Enum:1=���,2=����", Infragistics.Win.HAlign.Default, "", true);
            base.AddColumn("PositionName", "��λ");
            base.AddColumn("BillDate", "����", "date", Infragistics.Win.HAlign.Left, "yyyy-MM-dd", true, RptSummaryType.None);
            base.AddColumn("MaterialCode", "���ϴ���");
            base.AddColumn("MaterialName", "��������");
            base.AddColumn("Spec", "���");
            base.AddColumn("Length", "����", "number:10,2", Infragistics.Win.HAlign.Right, "#,##0.00;-#,##0.00; ", true, RptSummaryType.None);
            base.AddColumn("IngredientName", "����");
            base.AddColumn("MachiningStandardCode", "�ӹ�ͼ�Ŵ���");
            base.AddColumn("MachiningStandardName", "�ӹ�ͼ������");
            base.AddColumn("ProductionStandardName", "��Ʒ��׼");
            base.AddColumn("InQuantity1", "���������", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("InQuantity2", "��⸨����", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutQuantity1", "����������", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);
            base.AddColumn("OutQuantity2", "���⸨����", "number:10,4", Infragistics.Win.HAlign.Right, "", true, RptSummaryType.Sum);

            return base.Initialize();
        }
        public override bool Strike(System.Windows.Forms.Form frm, Infragistics.Win.UltraWinGrid.UltraGridRow row)
        {
            if (row.Band.Columns.Exists("ID"))
            {
                COMFields _MainTableDefine =null;
                List<COMFields> _DetailTableDefine = null;
                string Table = null;
                if ((int)row.Cells["WorkType"].Value == 1)
                {
                    Table = "D_WarehouseIn";
                }
                else
                {
                    Table = "D_WarehouseOut";
                }
                _MainTableDefine = CSystem.Sys.Svr.LDI.GetFields(Table);
                _DetailTableDefine = CSystem.Sys.Svr.Properties.DetailTableDefineList(Table);

                DataSet ds = CSystem.Sys.Svr.cntMain.Select(_MainTableDefine.QuerySQL + " where " + Table + ".ID='" + row.Cells["ID"].Value + "'", _MainTableDefine.OrinalTableName);

                frmDetail form = new frmDetail();
                if (form != null)
                {
                    form.toolDetailForm = new DetailFormWithoutEdit();
                    bool bShowData = form.ShowData(_MainTableDefine, _DetailTableDefine, new DataSetEventArgs(ds), false, frm.MdiParent);
                }
            }
            return true;

        }
    }
}
