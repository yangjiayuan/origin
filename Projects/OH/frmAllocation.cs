using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Base;
using Infragistics.Win.UltraWinGrid;
using UI;

namespace OH
{
    public partial class frmAllocation : Form
    {
        private Guid _PackagePlanID;
        private string _PackagePlanCode;
        private Guid _WarehouseID;
        private string _WarehouseName;
        private string _CustomerName;
        private DateTime _ConsignDate;

        public Guid PackagePlanID
        {
            get { return _PackagePlanID; }
        }
        public string PackagePlanCode
        {
            get { return _PackagePlanCode; }
        }
        public DateTime ConsignDate
        {
            get { return _ConsignDate; }
        }
        public string CustomerName
        {
            get { return _CustomerName; }
        }
        public Guid WarehouseID
        {
            get { return _WarehouseID; }
        }
        public string WarehouseName
        {
            get { return _WarehouseName; }
        }
        public frmAllocation()
        {
            InitializeComponent();
        }

        private void txtPackagePlan_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            getPackagePlan(false);
        }

        private void txtWarehouse_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            getWarehouse(false);
        }
        private void getPackagePlan(bool isKeyDown)
        {
            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_PackagePlan"), null, "(D_PackagePlan.Finished=0 and D_PackagePlan.PackageStatus<>2) and D_PackagePlan.CheckStatus=1", enuShowStatus.None);
            string txt = "";
            if (isKeyDown)
                txt = txtPackagePlan.Text;
            //DataRow dr = frm.Tools.ShowSelect("A.Code", txt, "A.WarehouseID='" + _WarehouseID + "'");
            DataRow dr = frm.ShowSelect("Code", txt, null);
            if (dr != null)
            {
                txtPackagePlan.Tag = dr["ID"];
                txtPackagePlan.Text = (string)(string)dr["Code"];
                _PackagePlanID = (Guid)dr["ID"];
                _PackagePlanCode = (string)dr["Code"];
                _ConsignDate = (DateTime)dr["ConsignDate"];
                _CustomerName = (string)dr["CustomerName"];
            }
        }
        private void getWarehouse(bool isKeyDown)
        {
            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("P_Warehouse"), null, "P_Warehouse.Disable=0 and P_Warehouse.WarehouseType=2", enuShowStatus.None);
            string txt = "";
            if (isKeyDown)
                txt = txtWarehouse.Text;
            //DataRow dr = frm.Tools.ShowSelect("A.Code", txt, "A.WarehouseID='" + _WarehouseID + "'");
            DataRow dr = frm.ShowSelect("Name", txt, null);
            if (dr != null)
            {
                txtWarehouse.Tag = dr["ID"];
                txtWarehouse.Text = (string)(string)dr["Name"];
                _WarehouseID = (Guid)dr["ID"];
                _WarehouseName = (string)dr["Name"];
            }
        }
        private void txtWarehouse_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.Alt && e.KeyCode == Keys.Down))
            {
                getWarehouse(true);
            }
        }

        private void txtPackagePlan_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.Alt && e.KeyCode == Keys.Down))
            {
                getPackagePlan(true);
            }
        }

        private void butSearch_Click(object sender, EventArgs e)
        {
            if (txtPackagePlan.Tag == null || txtPackagePlan.Tag == DBNull.Value)
            {
                Msg.Information("没有选择包装计划！");
                return;
            }
            if (txtWarehouse.Tag == null || txtWarehouse.Tag == DBNull.Value)
            {
                Msg.Information("没有选择仓库！");
                return;
            }
            //先查询出当前会计期
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(@"Select Value from S_BaseInfo where ID='系统\当前会计期'");
            if (ds.Tables[0].Rows.Count != 1)
            {
                return;
            }
            DateTime currPeriod = DateTime.Parse((string)ds.Tables[0].Rows[0][0]);
            string sql = string.Format("Select A.*,Balance.PositionID,P.Name PositionName,F.Name IngredientName,Ms.Name MachineStandardName,B.NeedLength,B.AmountCheckMethod,B.QuantityCheckMethod,B.ConvertQuotiety,B.Code MaterialCode,B.Name MaterialName,D.Name FirMeasure,E.Name SecMeasure,BalanceQuantity1,BalanceQuantity2,A.FinishQuantity1+isnull(Allocation.AllocationQuantity1,0) FQuantity1,A.FinishQuantity2+isnull(Allocation.AllocationQuantity2,0) FQuantity2, " +
                        "case when A.Quantity1-A.FinishQuantity1-isnull(Allocation.AllocationQuantity1,0)>BalanceQuantity1 then BalanceQuantity1 else  A.Quantity1-A.FinishQuantity1-isnull(Allocation.AllocationQuantity1,0) end FactQuantity1,case when A.Quantity2-A.FinishQuantity2-isnull(Allocation.AllocationQuantity2,0)>BalanceQuantity2 then BalanceQuantity2 else  A.Quantity2-A.FinishQuantity2-isnull(Allocation.AllocationQuantity2,0) end FactQuantity2 " +
                        "from D_PackagePlanBill A inner join D_PackagePlan C on A.MainID=C.ID  " +
                        "inner join (Select WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,ProductStandardID,Length,sum(BalanceQuantity1) BalanceQuantity1, sum(BalanceQuantity2) BalanceQuantity2  " +
                            "from (select WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,ProductStandardID,Length,BeginQuantity1 BalanceQuantity1,BeginQuantity2 BalanceQuantity2 from D_WarehouseBalance where CheckDate= '{1}' and WarehouseID='{2}' " +
                            "union all " +
                            "select WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,ProductStandardID,Length,sum(Quantity1) BalanceQuantity1, sum(Quantity2) BalanceQuantity2 from D_WarehouseIn A,D_WarehouseInBill B " +
                            "where A.ID=B.MainID and A.BillDate>='{1}' and WarehouseID='{2}' " +
                            "group by WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,ProductStandardID,Length " +
                            "union all " +
                            "select WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,ProductStandardID,Length,sum(Quantity1) BalanceQuantity1, sum(Quantity2) BalanceQuantity2 from D_WarehouseOut A,D_WarehouseOutBill B " +
                            "where A.ID=B.MainID and A.BillDate>='{1}' and WarehouseID='{2}' " +
                            "group by WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,ProductStandardID,Length "+
                             "union all " +
                            "select WarehouseID,PositionID,MaterialID,IngredientID,MachineStandardID MachiningStandardID,null ProductStandardID,Length,isnull(-sum(Quantity1),0) BalanceQuantity1, isnull(-sum(Quantity2),0) BalanceQuantity2 from D_Allocation A,D_AllocationBill B " +
                            "where A.ID=B.MainID and A.CheckStatus=0 and WarehouseID='{2}' " +
                            "group by WarehouseID,PositionID,MaterialID,IngredientID,MachineStandardID,Length) A " +
                        "group by WarehouseID,PositionID,MaterialID,IngredientID,MachiningStandardID,ProductStandardID,Length) Balance " +
                        "on A.MaterialID=Balance.MaterialID and A.IngredientID=Balance.IngredientID and A.MachineStandardID=Balance.MachiningStandardID and A.Length=Balance.Length " +
                        "inner join P_Position P on Balance.PositionID=P.ID " +
                        "inner join P_Material B on A.MaterialID=B.ID inner join P_Ingredient F on A.IngredientID=F.ID " +
                        "inner join P_MachiningStandard Ms on A.MachineStandardID=Ms.ID " +
                        "inner join P_Measure D on B.FirMeasureID=D.ID left join P_Measure E on B.SecMeasureID=E.ID "+
                        "left join (select WarehouseID,PositionID,MaterialID,IngredientID,MachineStandardID MachiningStandardID,Length,sum(Quantity1) AllocationQuantity1, sum(Quantity2) AllocationQuantity2 from D_Allocation A,D_AllocationBill B " +
                            "where A.ID=B.MainID and A.CheckStatus=0 and WarehouseID='{2}' " +
                            "group by WarehouseID,PositionID,MaterialID,IngredientID,MachineStandardID,Length) Allocation "+
                         "on A.MaterialID=Allocation.MaterialID and A.IngredientID=Allocation.IngredientID and A.MachineStandardID=Allocation.MachiningStandardID and A.Length=Allocation.Length "+
                          "where A.MainID='{0}' ", _PackagePlanID, currPeriod, _WarehouseID);
            DataSet dsAllocation = CSystem.Sys.Svr.cntMain.Select(sql, "D_PackagePlan");
            grid.SetDataBinding(dsAllocation, "D_PackagePlan", true);
        }
        private DataSet dsAllocation;
        internal DataSet GetData()
        {
            if (this.ShowDialog() == DialogResult.OK)
            {
                if (dsAllocation.Tables[0].Rows.Count > 0)
                    return dsAllocation;
                else
                    return null;
            }
            return null;
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            grid.PerformAction(UltraGridAction.ExitEditMode);
            if (txtPackagePlan.Tag == null || txtPackagePlan.Tag == DBNull.Value)
            {
                Msg.Information("没有选择包装计划！");
                return;
            }
            if (txtWarehouse.Tag == null || txtWarehouse.Tag == DBNull.Value)
            {
                Msg.Information("没有选择仓库！");
                return;
            }
            dsAllocation = CSystem.Sys.Svr.cntMain.Select(CSystem.Sys.Svr.LDI.GetFields("D_AllocationBill").QuerySQL + " Where 1=0", "D_AllocationBill");
            foreach (UltraGridRow row in grid.Rows)
            {

                if (((int)row.Cells["AmountCheckMethod"].Value == 1 && (decimal)row.Cells["Quantity1"].Value - (decimal)row.Cells["FinishQuantity1"].Value < (decimal)row.Cells["FactQuantity1"].Value) ||
                    ((int)row.Cells["AmountCheckMethod"].Value == 2 && (decimal)row.Cells["Quantity2"].Value - (decimal)row.Cells["FinishQuantity2"].Value < (decimal)row.Cells["FactQuantity2"].Value))
                {
                    Msg.Information(string.Format("第{0}行输入的数量大于采购数量！", row.Index + 1));
                    return;
                }
                if ((row.Cells["FactQuantity1"].Value != DBNull.Value && (decimal)row.Cells["FactQuantity1"].Value != 0) || (row.Cells["FactQuantity2"].Value != DBNull.Value && (decimal)row.Cells["FactQuantity2"].Value != 0))
                {
                    DataRow dr = dsAllocation.Tables[0].NewRow();
                    //dr["SourceID"] = row.Cells["ID"].Value;
                    dr["ID"] = System.Guid.NewGuid();
                    dr["PositionID"] = row.Cells["PositionID"].Value;
                    dr["PositionName"] = row.Cells["PositionName"].Value;
                    dr["MaterialID"] = row.Cells["MaterialID"].Value;
                    dr["MaterialCode"] = row.Cells["MaterialCode"].Value;
                    dr["MaterialName"] = row.Cells["MaterialName"].Value;
                    dr["IngredientID"] = row.Cells["IngredientID"].Value;
                    dr["IngredientName"] = row.Cells["IngredientName"].Value;
                    dr["MachineStandardID"] = row.Cells["MachineStandardID"].Value;
                    dr["MachineStandardName"] = row.Cells["IngredientName"].Value;
                    dr["FirMeasure"] = row.Cells["FirMeasure"].Value;
                    dr["SecMeasure"] = row.Cells["SecMeasure"].Value;
                    dr["AmountCheckMethod"] = row.Cells["AmountCheckMethod"].Value;
                    dr["NeedLength"] = row.Cells["NeedLength"].Value;
                    dr["QuantityCheckMethod"] = row.Cells["QuantityCheckMethod"].Value;
                    dr["ConvertQuotiety"] = row.Cells["ConvertQuotiety"].Value;
                    dr["Length"] = row.Cells["Length"].Value;
                    dr["Quantity1"] = row.Cells["FactQuantity1"].Value;
                    dr["Quantity2"] = row.Cells["FactQuantity2"].Value;
                    dr["SourceID"] = row.Cells["ID"].Value;
                    //dr["Price"] = row.Cells["PriceWithoutTax"].Value;
                    //dr["Money"] = row.Cells["FactMoney"].Value;
                    dsAllocation.Tables[0].Rows.Add(dr);
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}