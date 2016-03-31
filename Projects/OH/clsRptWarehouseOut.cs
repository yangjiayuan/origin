using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinEditors;
using System.Windows.Forms;
using Base;
using UI;
using System.Data;

namespace OH
{
    public class clsRptWarehouseOut:BaseReport
    {
        public override bool Initialize()
        {
            //_SQL = "select A.WarehouseID,D.Code WarehouseCode,D.Name WarehouseName,D.WarehouseType,A.Type,A.ID,A.Code,B.PositionID,C.Name PositionName,A.TargetDepartID,E.Name TargetDepartName, " +
            //            "sum(Quantity1) Qty,sum(Money) Mny " +
            //        "from D_WarehouseOut A inner join D_WarehouseOutBill B on A.ID=B.MainID " +
            //            "inner join P_Position C on B.PositionID=C.ID  " +
            //            "inner join P_Warehouse D on A.WarehouseID=D.ID " +
            //            "left join P_Department E on A.TargetDepartID=E.ID " +
            //            "${Where}"+
            //        "group by A.ID,A.Code,A.WarehouseID,D.Code,D.Name,D.WarehouseType,A.Type,B.PositionID,C.Name,A.TargetDepartID,E.Name ";

            _SQL = "select cls.Name MaterialClassName,A.ID,A.WarehouseID,D.Name WarehouseName,D.WarehouseType,D.Code WarehouseCode,A.Type,B.PositionID,C.Name PositionName,A.TargetDepartID,E.Name TargetDepartName,G.Name TargetUserName,H.Name MachineName,A.WorkCode, " +
                                    "A.Code,A.BillDate,B.MaterialID, F.Name MaterialName,F.Spec,I.Name FirMeasureName,I2.Name SecMeasureName,B.Quantity1,B.Quantity2,B.Money,B.Price,MCO.Name MaterialCheckObjectName " +
                                "from D_WarehouseOut A inner join D_WarehouseOutBill B on A.ID=B.MainID " +
                                    "inner join P_Position C on B.PositionID=C.ID  " +
                                    "inner join P_Warehouse D on A.WarehouseID=D.ID and WarehouseType=4 " +
                                    "inner join P_Material F on B.MaterialID=F.ID " +
                                    "inner join P_Measure I on F.FirMeasureID=I.ID " +
                                    "left join P_Measure I2 on F.SecMeasureID=I2.ID " +
                                    "left join P_MaterialClass cls on F.MaterialClassID=cls.ID "+
                                    "left join P_Department E on A.TargetDepartID=E.ID " +
                                    "left join P_Operator G on B.OperatorID=G.ID " +
                                    "left join P_Machine H on B.MachineID=H.ID " +
                                    "left join P_MaterialCheckObject MCO on MCO.ID=F.MaterialCheckObjectID ";
            _IsWhere = true;
            _IsAppendSQL = true;

            _Title = "���ϲ�ѯ";

            base.AddCondition("startDate", "��ʼʱ��", "Date", "A.BillDate>='{0:yyyy-MM-dd}'");
            base.AddCondition("endDate", "����ʱ��", "Date", "A.BillDate<='{0:yyyy-MM-dd}'");
            base.AddCondition("WarehouseID", "�ֿ�", "Dict:P_Warehouse", "A.WarehouseID='{0}'");
            base.AddCondition("PositionID", "��λ", "Dict:P_Position", "B.PositionID='{0}'");
            base.AddCondition("WorkCode", "���ϵ���", "String", "A.WorkCode like '%{0}%'");
            base.AddCondition("MachineID", "�����豸", "Dict:P_Machine", "B.MachineID='{0}'");
            base.AddCondition("OperatorID", "������", "Dict:P_Operator", "B.OperatorID='{0}'");
            base.AddCondition("chkGroup", "��ϸ�������ϻ���", "Boolean", "", "");
            base.AddCondition("chkWarehouse", "�����ֲֿ�", "Boolean", "", "");

            base.AddColumn("WarehouseName", "�ֿ�");
            base.AddColumn("PositionName", "��λ");
            base.AddColumn("Type", "����", "Enum:0=��ͨ,1=�ƿ�", Infragistics.Win.HAlign.Default, "", true);
            base.AddColumn("TargetDepartName", "���ϲ���");
            base.AddColumn("TargetUserName", "������");
            base.AddColumn("Code", "��ˮ��");
            base.AddColumn("WorkCode", "���ⵥ��");
            base.AddColumn("BillDate", "��������");
            base.AddColumn("MachineName", "�豸");
            base.AddColumn("MaterialClassName", "���ϴ���");
            base.AddColumn("MaterialName", "��������");
            base.AddColumn("Spec", "���");
            base.AddColumn("MaterialCheckObjectName", "���Ͽ��˶���");
            base.AddColumn("FirMeasureName", "����λ");
            base.AddColumn("SecMeasureName", "����λ");
            base.AddColumn("Quantity1", "������", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.Sum);
            base.AddColumn("Quantity2", "������", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.Sum);
            base.AddColumn("Price", "����", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.None);
            base.AddColumn("Money", "���", "Number:18,2", Infragistics.Win.HAlign.Left, "#,##0.00", true, RptSummaryType.Sum);

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
        public override void BeforeQuery(object sender, frmReport.BeforeQueryEventArgs e)
        {
            CheckBox chk = base.GetCondition("chkGroup").Control as CheckBox;
            if (chk.Checked)
            {
                CheckBox chkWarehouse=base.GetCondition("chkWarehouse").Control as CheckBox;
                string warehouse1 = "";
                if (!chkWarehouse.Checked)
                {
                    warehouse1 = "WarehouseName,PositionName,";
                }
                e.SQL = "select MaterialClassName,"+warehouse1+"MaterialName,FirMeasureName,SecMeasureName,Spec,MaterialCheckObjectName,sum(Quantity1) Quantity1,sum(Quantity2) Quantity2,sum(Money) Money from (" +
                    e.SQL +
                    "${Where}) A Group by MaterialClassName," + warehouse1 + "MaterialName,FirMeasureName,SecMeasureName,Spec,MaterialCheckObjectName";
                e.IsAppendSQL = false;
            }
        }
        
        public override bool Strike(System.Windows.Forms.Form frm, Infragistics.Win.UltraWinGrid.UltraGridRow row)
        {
            if (row.Band.Columns.Exists("ID"))
            {
                //COMFields _MainTableDefine = CSystem.Sys.Svr.LDI.GetFields("D_WarehouseOut").Clone();
                //_MainTableDefine["MachineName"].Visible = COMField.Enum_Visible.VisibleAll;
                //_MainTableDefine["MachineName"].Mandatory = true;
                //_MainTableDefine["MachineName"].Enable = true;
                //List<COMFields> _DetailTableDefine = CSystem.Sys.Svr.Properties.DetailTableDefineListClone("D_WarehouseOut");
                //_DetailTableDefine[0]["Money"].Visible = COMField.Enum_Visible.VisibleAll;
                //_DetailTableDefine[0]["Money"].Mandatory = true;
                //_DetailTableDefine[0]["Price"].Visible = COMField.Enum_Visible.VisibleAll;
                //_DetailTableDefine[0]["Price"].Mandatory = true;
                ////_DetailTableDefine[0]["MachineName"].Visible = COMField.Enum_Visible.VisibleAll;
                ////_DetailTableDefine[0]["MachineName"].Mandatory = true;
                ////_DetailTableDefine[0]["MachineName"].Enable = true;
                //_DetailTableDefine[0]["OperatorName"].Visible = COMField.Enum_Visible.VisibleAll;
                //_DetailTableDefine[0]["OperatorName"].Mandatory = true;
                //_DetailTableDefine[0]["OperatorName"].Enable = true;


                frmDetail form = new frmDetail();
                if (form != null)
                {
                    clsWarehouseDetailForm.enuWarehouseOut Type = (clsWarehouseDetailForm.enuWarehouseOut)row.Cells["Type"].Value;
                    clsWarehouseDetailForm.enuWarehouseOperate opt;
                    if (Type == clsWarehouseDetailForm.enuWarehouseOut.RepairOut)
                        opt = clsWarehouseDetailForm.enuWarehouseOperate.RepairOut;
                    else if (Type == clsWarehouseDetailForm.enuWarehouseOut.MoveOut)
                        opt = clsWarehouseDetailForm.enuWarehouseOperate.MoveOut;
                    else
                        opt = clsWarehouseDetailForm.enuWarehouseOperate.Out;
                    form.toolDetailForm = new clsWarehouseDetailForm(opt, (Guid)row.Cells["WarehouseID"].Value, (string)row.Cells["WarehouseCode"].Value, (string)row.Cells["WarehouseName"].Value, (int)row.Cells["WarehouseType"].Value);
                    COMFields _MainTableDefine;
                    List<COMFields> _DetailTableDefine;
                    ProcessDefine(out _MainTableDefine, out  _DetailTableDefine, Type, (string)row.Cells["WarehouseName"].Value,(int)row.Cells["WarehouseType"].Value);
                    DataSet ds = CSystem.Sys.Svr.cntMain.Select(_MainTableDefine.QuerySQL + " where D_WarehouseOut.ID='" + row.Cells["ID"].Value + "'", _MainTableDefine.OrinalTableName);
                    bool bShowData = form.ShowData(_MainTableDefine, _DetailTableDefine, new DataSetEventArgs(ds), false, frm.MdiParent);
                }
            }
            return true;
        }
        private void ProcessDefine(out COMFields mainTableDefine, out List<COMFields> detailTableDefines, clsWarehouseDetailForm.enuWarehouseOut Type,string WarehouseName,int WarehouseType)
        {
            mainTableDefine = CSystem.Sys.Svr.LDI.GetFields("D_WarehouseOut").Clone();
            switch (Type)
            {
                case clsWarehouseDetailForm.enuWarehouseOut.MoveOut:
                    mainTableDefine.Property.Title = string.Format("�������ⵥ[{0}]", WarehouseName);
                    mainTableDefine["WorkCode"].FieldTitle = "��������";
                    mainTableDefine["TargetDepartID"].Visible = COMField.Enum_Visible.NotVisible;
                    mainTableDefine["TargetDepartID"].Mandatory = false;
                    mainTableDefine["TargetDepartName"].Visible = COMField.Enum_Visible.NotVisible;
                    mainTableDefine["TargetDepartName"].Mandatory = false;
                    mainTableDefine["TargetUserID"].Visible = COMField.Enum_Visible.NotVisible;
                    mainTableDefine["TargetUserID"].Mandatory = false;
                    mainTableDefine["TargetUserName"].Visible = COMField.Enum_Visible.NotVisible;
                    mainTableDefine["TargetUserName"].Mandatory = false;

                    mainTableDefine["TargetWarehouseName"].Visible = COMField.Enum_Visible.VisibleAll;
                    mainTableDefine["TargetPositionName"].Visible = COMField.Enum_Visible.VisibleAll;
                    mainTableDefine["TargetWarehouseName"].Mandatory = true;
                    mainTableDefine["TargetPositionName"].Mandatory = true;
                    mainTableDefine["TargetWarehouseName"].Enable = true;
                    mainTableDefine["TargetPositionName"].Enable = true;
                    break;
                case clsWarehouseDetailForm.enuWarehouseOut.CheckOut:
                    mainTableDefine.Property.Title = string.Format("�̵���ⵥ[{0}]", WarehouseName);
                    goto default;
                case clsWarehouseDetailForm.enuWarehouseOut.ScrapOut:
                    mainTableDefine.Property.Title = string.Format("��Ʒ���ⵥ[{0}]", WarehouseName);
                    goto default;
                case clsWarehouseDetailForm.enuWarehouseOut.RepairOut:
                    mainTableDefine.Property.Title = string.Format("���޳��ⵥ[{0}]", WarehouseName);
                    mainTableDefine["WorkCode"].FieldTitle = "���޵���";
                    mainTableDefine["TargetDepartID"].Visible = COMField.Enum_Visible.NotVisible;
                    mainTableDefine["TargetDepartID"].Mandatory = false;
                    mainTableDefine["TargetDepartName"].Visible = COMField.Enum_Visible.NotVisible;
                    mainTableDefine["TargetDepartName"].Mandatory = false;
                    mainTableDefine["TargetUserID"].Visible = COMField.Enum_Visible.NotVisible;
                    mainTableDefine["TargetUserID"].Mandatory = false;
                    mainTableDefine["TargetUserName"].Visible = COMField.Enum_Visible.NotVisible;
                    mainTableDefine["TargetUserName"].Mandatory = false;

                    mainTableDefine["TargetWarehouseName"].Visible = COMField.Enum_Visible.VisibleAll;
                    mainTableDefine["TargetPositionName"].Visible = COMField.Enum_Visible.VisibleAll;
                    mainTableDefine["TargetWarehouseName"].Mandatory = true;
                    mainTableDefine["TargetPositionName"].Mandatory = true;
                    mainTableDefine["TargetWarehouseName"].Enable = true;
                    mainTableDefine["TargetPositionName"].Enable = true;
                    break;
                case clsWarehouseDetailForm.enuWarehouseOut.Out:
                    mainTableDefine.Property.Title = string.Format("��ͨ���ⵥ[{0}]", WarehouseName);
                    goto default;
                case clsWarehouseDetailForm.enuWarehouseOut.ProduceOut:
                    mainTableDefine.Property.Title = string.Format("�������ϵ�[{0}]", WarehouseName);
                    goto default;
                case clsWarehouseDetailForm.enuWarehouseOut.ReturnOut:
                    mainTableDefine.Property.Title = string.Format("�˻���[{0}]", WarehouseName);
                    goto default;
                default:
                    mainTableDefine["TargetWarehouseName"].Visible = COMField.Enum_Visible.NotVisible;
                    mainTableDefine["TargetPositionName"].Visible = COMField.Enum_Visible.NotVisible;
                    mainTableDefine["TargetWarehouseName"].Mandatory = false;
                    mainTableDefine["TargetPositionName"].Mandatory = false;
                    mainTableDefine["TargetWarehouseName"].Enable = false;
                    mainTableDefine["TargetPositionName"].Enable = false;
                    break;
            }

            detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone("D_WarehouseOut");

            //���������ֶζ�����
            foreach (COMField field in detailTableDefines[0].Fields)
                field.Enable = false;

            //���ӶԸ��ϲֿ⼰�����ֿ�Ĵ���

            if (WarehouseType == 4)
            {
                mainTableDefine["MachineID"].Mandatory = true;
                mainTableDefine["MachineName"].Mandatory = true;
                mainTableDefine["MachineName"].Enable = true;
                mainTableDefine["MachineName"].Visible = COMField.Enum_Visible.VisibleAll;

                detailTableDefines[0]["Money"].Visible = COMField.Enum_Visible.VisibleAll;
                detailTableDefines[0]["Money"].Mandatory = true;
                detailTableDefines[0]["Price"].Visible = COMField.Enum_Visible.VisibleAll;
                detailTableDefines[0]["Price"].Mandatory = true;
                detailTableDefines[0]["Price"].Enable = true;

                //detailTableDefines[0]["PositionID"].Mandatory = true;
                //detailTableDefines[0]["PositionName"].Enable = true;
                detailTableDefines[0]["MaterialCode"].Enable = true;
                detailTableDefines[0]["MaterialName"].Enable = true;
                detailTableDefines[0]["Quantity1"].Enable = true;
                detailTableDefines[0]["Quantity2"].Enable = true;

                detailTableDefines[0]["MaterialCheckObjectName"].Visible = COMField.Enum_Visible.VisibleAll;
                detailTableDefines[0]["MaterialClassName"].Visible = COMField.Enum_Visible.VisibleAll;

                //detailTableDefines[0]["OperatorName"].Visible = COMField.Enum_Visible.VisibleAll;
                //detailTableDefines[0]["OperatorName"].Mandatory = true;
                //detailTableDefines[0]["OperatorName"].Enable = true;
                //detailTableDefines[0]["OperatorID"].Mandatory = true;
                if (Type != clsWarehouseDetailForm.enuWarehouseOut.MoveOut)
                {

                }
                else
                {
                    mainTableDefine["TargetUserID"].Visible = COMField.Enum_Visible.NotVisible;
                    mainTableDefine["TargetUserID"].Mandatory = true;
                    mainTableDefine["TargetUserName"].Visible = COMField.Enum_Visible.VisibleAll;
                    mainTableDefine["TargetUserName"].Mandatory = true;
                }

            }

                detailTableDefines[0]["IngredientName"].Visible = COMField.Enum_Visible.NotVisible;
                detailTableDefines[0]["IngredientName"].Mandatory = false;
                detailTableDefines[0]["IngredientID"].Mandatory = false;
                detailTableDefines[0]["MachiningStandardName"].Visible = COMField.Enum_Visible.NotVisible;
                detailTableDefines[0]["MachiningStandardName"].Mandatory = false;
                detailTableDefines[0]["MachiningStandardID"].Mandatory = false;
                detailTableDefines[0]["DesignCodeName"].Visible = COMField.Enum_Visible.NotVisible;
                detailTableDefines[0]["DesignCodeName"].Mandatory = false;
                detailTableDefines[0]["DesignCodeID"].Mandatory = false;
                detailTableDefines[0]["ProductStandardName"].Visible = COMField.Enum_Visible.NotVisible;
                detailTableDefines[0]["ProductStandardName"].Mandatory = false;
                detailTableDefines[0]["ProductStandardID"].Mandatory = false;
        }

    }
}
