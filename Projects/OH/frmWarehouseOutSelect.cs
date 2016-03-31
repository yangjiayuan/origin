using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Base;
using UI;

namespace OH
{
    public partial class frmWarehouseOutSelect : Form
    {
        private Guid _WarehouseID;
        private string mainSQL;
        private DataSet _WarehouseOut;
        private COMFields _DetailFields;
        public DataSet WarehouseOut
        {
            get { return _WarehouseOut; }
        }
        public frmWarehouseOutSelect()
        {
            InitializeComponent();
        }
        public frmWarehouseOutSelect(Guid warehouseID,COMFields detailFields)
        {
            InitializeComponent();
            _WarehouseID = warehouseID;
            _DetailFields = detailFields;
            //取得最大的仓库记账日
            DateTime dt = DateTime.Now.AddYears(-1);
            string sql = "select max(CheckDate) from D_WarehouseBalance";
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(sql);
            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                dt = (DateTime)ds.Tables[0].Rows[0][0];
            //显示当前的库中物料的库存、包括未确认的库存（库存＋未确认的入库＋未确认的出库）
            mainSQL = string.Format("select Price.Price,0.00 Money,A.Code MaterialCode,A.Name MaterialName,A.Spec,A.ConvertQuotiety,A.QuantityCheckMethod,A.AmountCheckMethod,A.NeedLength,M1.Name FirMeasure,M2.Name SecMeasure,P.Name PositionName,B.PositionID,B.IngredientID,I.Name IngredientName,B.MachiningStandardID,MS.Code MachiningStandardCode, MS.Name MachiningStandardName,MS.Text1,B.Length,B.MaterialID,B.BalanceQty1,B.UnCheckQty1,0.0 OutQty1,B.BalanceQty2,B.UnCheckQty2,0.0 OutQty2,B.LastQty1,B.LastQty2 from " +
                "(Select Length,MaterialID, PositionID, IngredientID, MachiningStandardID, DesignCodeID, ProductStandardID, sum(BalanceQty1) BalanceQty1, sum(UnCheckQty1) UnCheckQty1, sum(BalanceQty2) BalanceQty2, sum(UnCheckQty2) UnCheckQty2,sum(LastQty1) LastQty1,sum(LastQty2) LastQty2 from " +
                "(Select A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID, sum(A.BeginQuantity1) BalanceQty1, 0 UnCheckQty1, sum(A.BeginQuantity2) BalanceQty2, 0 UnCheckQty2,sum(A.BeginQuantity1) LastQty1,sum(A.BeginQuantity2) LastQty2 from D_WarehouseBalance A where A.CheckDate = '{0}' and A.WarehouseID = '{1}' Group by A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID " +
                "union all Select A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID, Sum(A.Quantity1) BalanceQty1, 0 UnCheckQty1, sum(A.Quantity2) BalanceQty2, 0 UnCheckQty2,0 LastQty1,0 LastQty2 from D_WarehouseInBill A, D_WarehouseIn B where A.MainID = B.ID and B.BillDate >= '{0}' and B.CheckStatus = 1 and B.WarehouseID = '{1}' group by A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID " +
                "union all Select A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID, -Sum(A.Quantity1) BalanceQty1, 0 UnCheckQty1, -sum(A.Quantity2) BalanceQty2, 0 UnCheckQty2,0 LastQty1,0 LastQty2 from D_WarehouseOutBill A, D_WarehouseOut B where A.MainID = B.ID and B.BillDate >= '{0}' and B.CheckStatus = 1 and B.WarehouseID = '{1}' group by A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID " +
                "union all Select A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID, 0 BalanceQty1, Sum(A.Quantity1) UnCheckQty1, 0 BalanceQty2, sum(A.Quantity2) UnCheckQty2,0 LastQty1,0 LastQty2 from D_WarehouseInBill A, D_WarehouseIn B where A.MainID = B.ID and B.BillDate >= '{0}' and B.CheckStatus = 0 and B.WarehouseID = '{1}' group by A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID " +
                "union all Select A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID, 0 BalanceQty1, -Sum(A.Quantity1) UnCheckQty1, 0 BalanceQty2, -sum(A.Quantity2) UnCheckQty2,0 LastQty1,0 LastQty2 from D_WarehouseOutBill A, D_WarehouseOut B where A.MainID = B.ID and B.BillDate >= '{0}' and B.CheckStatus = 0 and B.WarehouseID = '{1}' group by A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID " +
                "union all Select A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID,0 BalanceQty1, 0 UnCheckQty1, 0 BalanceQty2, 0 UnCheckQty2,sum(A.Quantity1) LastQty1,sum(A.Quantity2) LastQty2 from D_WarehouseInBill A, D_WarehouseIn B where A.MainID = B.ID and B.BillDate >= '{0}' and B.BillDate<'{2}' and B.CheckStatus = 1 and B.WarehouseID = '{1}' group by A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID " +
                "union all Select A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID, 0 BalanceQty1, 0 UnCheckQty1, 0 BalanceQty2, 0 UnCheckQty2,-sum(A.Quantity1) LastQty1,-sum(A.Quantity2) LastQty2 from D_WarehouseOutBill A, D_WarehouseOut B where A.MainID = B.ID and B.BillDate >= '{0}' and B.BillDate<'{2}' and B.CheckStatus = 1 and B.WarehouseID = '{1}' group by A.Length,A.MaterialID, A.PositionID, A.IngredientID, A.MachiningStandardID, A.DesignCodeID, A.ProductStandardID ) D " +
                "group by Length,MaterialID, PositionID, IngredientID, MachiningStandardID, DesignCodeID, ProductStandardID) B inner join P_Material A on A.ID = B.MaterialID and A.Disable=0 inner join P_Position P on B.PositionID=P.ID inner join P_Measure M1 on A.FirMeasureID=M1.ID left join P_Measure M2 on A.SecMeasureID=M2.ID left join P_Ingredient I on B.IngredientID=I.ID left join P_MachiningStandard MS on B.MachiningStandardID=MS.ID "+
                "left join (Select A.PositionID,B.MaterialID,B.Price from C_Price A,C_PriceBill B where A.ID=B.MainID) Price on A.ID=Price.MaterialID and (P.ID=Price.PositionID or P.PricePositionID=Price.PositionID)", dt, _WarehouseID,dt.AddMonths(1));
            //mainSQL = string.Format("select A.Code MaterialCode,A.Name MaterialName,A.DesignCode MaterialDesignCode,A.DesignVersion MaterialDesignVersion,A.IdentityCode MaterialIdentityCode,A.FirMeasure,A.SecMeasure,B.MaterialSpecID,C.Name MaterialSpecName,B.MaterialID,B.Parameters,A.ParaNotes,B.BalanceQty1,B.UnCheckQty1,0.0 OutQty1,B.BalanceQty2,B.UnCheckQty2,0.0 OutQty2 " +
            //" from (Select MaterialID,MaterialSpecID,Parameters,sum(BalanceQty1) BalanceQty1,sum(UnCheckQty1) UnCheckQty1,sum(BalanceQty2) BalanceQty2,sum(UnCheckQty2) UnCheckQty2 from (Select A.MaterialID,A.MaterialSpecID,A.Parameters,sum(A.Quantity1) BalanceQty1,0 UnCheckQty1,sum(A.Quantity2) BalanceQty2,0 UnCheckQty2 from D_WarehouseBalance A where A.CheckDate='{0}'  and A.WarehouseID='{1}' Group by A.MaterialID,A.MaterialSpecID,A.Parameters  " +
            //" union all Select A.MaterialID,A.MaterialSpecID,A.Parameters,Sum(A.Quantity1) BalanceQty1,0 UnCheckQty1,sum(A.Quantity2) BalanceQty2,0 UnCheckQty2 from D_WarehouseInBill A,D_WarehouseIn B where A.MainID=B.ID and B.CreateDate>='{0}' and B.CheckStatus=1 and B.WarehouseID='{1}' group by A.MaterialID,A.MaterialSpecID,A.Parameters " +
            //" union all Select A.MaterialID,A.MaterialSpecID,A.Parameters,-Sum(A.Quantity1) BalanceQty1,0 UnCheckQty1,-sum(A.Quantity2) BalanceQty2,0 UnCheckQty2 from D_WarehouseOutBill A,D_WarehouseOut B where A.MainID=B.ID and B.CreateDate>='{0}' and B.CheckStatus=1 and B.WarehouseID='{1}' group by A.MaterialID,A.MaterialSpecID,A.Parameters " +
            //" union all Select A.MaterialID,A.MaterialSpecID,A.Parameters,0 BalanceQty1,Sum(A.Quantity1) UnCheckQty1,0 BalanceQty2,sum(A.Quantity2) UnCheckQty2 from D_WarehouseInBill A,D_WarehouseIn B where A.MainID=B.ID and B.CreateDate>='{0}' and B.CheckStatus=0 and B.WarehouseID='{1}' group by A.MaterialID,A.MaterialSpecID,A.Parameters " +
            //" union all Select A.MaterialID,A.MaterialSpecID,A.Parameters,0 BalanceQty1,-Sum(A.Quantity1) UnCheckQty1,0 BalanceQty2,-sum(A.Quantity2) UnCheckQty2 from D_WarehouseOutBill A,D_WarehouseOut B where A.MainID=B.ID and B.CreateDate>='{0}' and B.CheckStatus=0 and B.WarehouseID='{1}' group by A.MaterialID,A.MaterialSpecID,A.Parameters) D group by  MaterialID,MaterialSpecID,Parameters) B " +
            //" inner join P_Material A on A.ID=B.MaterialID left join P_MaterialSpec C on B.MaterialSpecID=C.ID", dt, _WarehouseID);
            //string field="newid() ID,A.Code MaterialCode,A.Name MaterialName,A.DesignCode MaterialDesignCode,A.IdentityCode MaterialIdentityCode,B.MaterialSpecID,C.Name MaterialSpecName,B.MaterialID,B.Parameters,B.BalanceQty1,B.UnCheckQty1,0.0 OutQty1,,B.BalanceQty2,B.UnCheckQty2,0.0 OutQty2";
            //string from =string.Format("(Select A.MaterialID,A.MaterialSpecID,A.Parameters,sum(A.Quantity1) BalanceQty1,0 UnCheckQty1,sum(A.Quantity2) BalanceQty2,0 UnCheckQty2 from D_WarehouseBalance A where A.CheckDate='{0}' Group by A.MaterialID,A.MaterialSpecID,A.Parameters  " +
            //" union Select A.MaterialID,A.MaterialSpecID,A.Parameters,Sum(A.Quantity1) BalanceQty1,0 UnCheckQty1,sum(A.Quantity2) BalanceQty2,0 UnCheckQty2 from D_WarehouseInBill A,D_WarehouseIn B where A.MainID=B.ID and B.CreateDate>='{0}' and B.CheckStatus=1 and A.Warehouse='{1}' group by A.MaterialID,A.MaterialSpecID,A.Parameters " +
            //" union Select A.MaterialID,A.MaterialSpecID,A.Parameters,-Sum(A.Quantity1) BalanceQty1,0 UnCheckQty1,sum(A.Quantity2) BalanceQty2,0 UnCheckQty2 from D_WarehouseOutBill A,D_WarehouseOut B where A.MainID=B.ID and B.CreateDate>='{0}' and B.CheckStatus=1 and A.Warehouse='{1}' group by A.MaterialID,A.MaterialSpecID,A.Parameters " +
            //" union Select A.MaterialID,A.MaterialSpecID,A.Parameters,0 BalanceQty1,Sum(A.Quantity1) UnCheckQty1,sum(A.Quantity2) BalanceQty2,0 UnCheckQty2 from D_WarehouseInBill A,D_WarehouseIn B where A.MainID=B.ID and B.CreateDate>='{0}' and B.CheckStatus=0 and A.Warehouse='{1}' group by A.MaterialID,A.MaterialSpecID,A.Parameters " +
            //" union Select A.MaterialID,A.MaterialSpecID,A.Parameters,0 BalanceQty1,-Sum(A.Quantity1) UnCheckQty1,sum(A.Quantity2) BalanceQty2,0 UnCheckQty2 from D_WarehouseOutBill A,D_WarehouseOut B where A.MainID=B.ID and B.CreateDate>='{0}' and B.CheckStatus=0 and A.Warehouse='{1}' group by A.MaterialID,A.MaterialSpecID,A.Parameters) B " +
            //" inner join P_Material A on A.ID=B.MaterialID left join P_MaterialSpec C on B.MaterialSpecID=C.ID", dt, _WarehouseID);
            //clsSelect select = new clsSelect("Balance", field, from, "", "ID", "A.Code");

        }

        private void butFilter_Click(object sender, EventArgs e)
        {
            string filter = "";
            object o = txtLocation.Tag;
            if (o != null && o != DBNull.Value && (Guid)o != CSystem.Sys.Svr.NullID)
                filter = " and B.PositionID='" + o + "'";
            if (txtCode.Text.Length > 0)
                filter = filter + " and A.Code Like '%" + txtCode.Text + "%' ";
            if (txtName.Text.Length > 0)
                filter = filter + " and A.Name like '%" + txtName.Text + "%' ";
            if (filter.Length > 0)
                filter = " where " + filter.Substring(4);
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(mainSQL + filter, "Balance");
            grid.SetDataBinding(ds, "Balance", true);
            foreach (UltraGridRow row in grid.Rows)
            {
                switch ((int)row.Cells["QuantityCheckMethod"].Value)
                {
                    case 1:
                        row.Cells["OutQty2"].Activation = Activation.Disabled;
                        break;
                    case 2:
                        row.Cells["OutQty1"].Activation = Activation.Disabled;
                        break;
                }
            }

            //计量单位
            //Infragistics.Win.ValueList Measurelist = Tools.GetValueList(grid, "P_Measure", "Measures");
            //this.grid.DisplayLayout.Bands[0].Columns["FirMeasure"].ValueList = Measurelist;
            //this.grid.DisplayLayout.Bands[0].Columns["SecMeasure"].ValueList = Measurelist;
            //foreach (UltraGridRow row in grid.Rows)
            //{
            //    if (row.Cells["SecMeasure"].Value == DBNull.Value || row.Cells["SecMeasure"].Value == null || (Guid)row.Cells["SecMeasure"].Value == clsSession.NullID)
            //        row.Cells["OutQty2"].Activation = Activation.Disabled;
            //}
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            if (grid.Rows.Count == 0)
            {
                Msg.Information("没有选择出库存！");
                return;
            }
            _WarehouseOut = CSystem.Sys.Svr.cntMain.Select(_DetailFields.QuerySQL + " where 1=0", "D_WarehouseOutBill");
            foreach (UltraGridRow row in grid.Rows)
            {
                if (((decimal)row.Cells["OutQty1"].Value == 0 && (decimal)row.Cells["OutQty2"].Value != 0) || (decimal)row.Cells["OutQty1"].Value != 0)
                {
                    DataRow dr = _WarehouseOut.Tables[0].NewRow();
                    dr["ID"] = System.Guid.NewGuid();
                    dr["PositionID"] = row.Cells["PositionID"].Value;
                    dr["PositionName"] = row.Cells["PositionName"].Value;
                    dr["MaterialID"] = row.Cells["MaterialID"].Value;
                    dr["MaterialCode"] = row.Cells["MaterialCode"].Value;
                    dr["MaterialName"] = row.Cells["MaterialName"].Value;
                    dr["Spec"] = row.Cells["Spec"].Value;
                    dr["IngredientID"] = row.Cells["IngredientID"].Value;
                    dr["IngredientName"] = row.Cells["IngredientName"].Value;
                    dr["MachiningStandardID"] = row.Cells["MachiningStandardID"].Value;
                    dr["MachiningStandardName"] = row.Cells["MachiningStandardName"].Value;
                    dr["MachiningStandardCode"] = row.Cells["MachiningStandardCode"].Value;
                    dr["Text1"] = row.Cells["Text1"].Value;
                    //dr["DesignCodeID"] = row.Cells["DesignCodeID"].Value;
                    //dr["DesignCodeName"] = row.Cells["DesignCodeName"].Value;
                    //dr["ProductStandardID"] = row.Cells["ProductStandardID"].Value;
                    //dr["ProductStandardName"] = row.Cells["ProductStandardName"].Value;
                    dr["Length"] = row.Cells["Length"].Value;
                    dr["ConvertQuotiety"] = row.Cells["ConvertQuotiety"].Value;
                    dr["QuantityCheckMethod"] = row.Cells["QuantityCheckMethod"].Value;
                    dr["AmountCheckMethod"] = row.Cells["AmountCheckMethod"].Value;
                    dr["NeedLength"] = row.Cells["NeedLength"].Value;
                    dr["FirMeasure"] = row.Cells["FirMeasure"].Value;
                    dr["Quantity1"] = row.Cells["OutQty1"].Value;
                    dr["SecMeasure"] = row.Cells["SecMeasure"].Value;
                    dr["Quantity2"] = row.Cells["OutQty2"].Value;
                    dr["Money"] = row.Cells["Money"].Value;
                    dr["Price"] = row.Cells["Price"].Value;

                    _WarehouseOut.Tables[0].Rows.Add(dr);
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        public DataSet GetData()
        {
            if (this.ShowDialog() == DialogResult.OK)
            {
                if (_WarehouseOut.Tables[0].Rows.Count > 0)
                    return _WarehouseOut;
                else
                    return null;
            }
            return null;
        }

        private void txtLocation_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            getLocation(false);
        }

        private void txtLocation_TextChanged(object sender, EventArgs e)
        {
            if (txtLocation.Text.Length == 0)
                txtLocation.Tag = null;
        }
        private void getLocation(bool isKeyDown)
        {
            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("P_Position"), null, "P_Position.WarehouseID='" + _WarehouseID + "'", enuShowStatus.None);
            string txt = "";
            if (isKeyDown)
                txt = txtLocation.Text;
            //DataRow dr = frm.Tools.ShowSelect("A.Code", txt, "A.WarehouseID='" + _WarehouseID + "'");
            DataRow dr = frm.ShowSelect("Code", txt, null);
            if (dr != null)
            {
                txtLocation.Tag = dr["ID"];
                txtLocation.Text = (string)(string)dr["Name"];
            }
        }

        private void txtLocation_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.Alt && e.KeyCode == Keys.Down))
            {
                getLocation(true);
            }
        }

        private void grid_AfterCellUpdate(object sender, CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "OutQty1":
                    if ((int)e.Cell.Row.Cells["QuantityCheckMethod"].Value == 1)
                    {
                        decimal length = 1;
                        if (e.Cell.Row.Cells["Length"].Activation == Activation.AllowEdit && e.Cell.Row.Cells["Length"].Value != DBNull.Value)
                            length = (decimal)e.Cell.Row.Cells["Length"].Value;
                        decimal conv = (decimal)e.Cell.Row.Cells["ConvertQuotiety"].Value;
                        if (conv == 0)
                            conv = 1;
                        if (length == 0)
                            length = 1;
                        e.Cell.Row.Cells["OutQty2"].Value = Math.Round((decimal)e.Cell.Value * length / conv, 4);
                    }
                    //只有辅料才有作用
                    if (e.Cell.Row.Cells["Price"].Value != DBNull.Value && (decimal)e.Cell.Row.Cells["Price"].Value != 0)
                    {
                        e.Cell.Row.Cells["Money"].Value = Math.Round((decimal)e.Cell.Value * (decimal)e.Cell.Row.Cells["Price"].Value, 2);
                    }
                    break;
                case "OutQty2":
                    if ((int)e.Cell.Row.Cells["QuantityCheckMethod"].Value == 2)
                    {
                        decimal length = 1;
                        if (e.Cell.Row.Cells["Length"].Activation == Activation.AllowEdit)
                            length = (decimal)e.Cell.Row.Cells["Length"].Value;
                        decimal conv = (decimal)e.Cell.Row.Cells["ConvertQuotiety"].Value;
                        if (conv == 0)
                            conv = 1;
                        if (length == 0)
                            length = 1;
                        e.Cell.Row.Cells["OutQty1"].Value = Math.Round((decimal)e.Cell.Value * length / conv, 4);
                    }
                    break;
            }
        }
    }
}