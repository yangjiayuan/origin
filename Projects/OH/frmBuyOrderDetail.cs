using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;
using Base;
using UI;

namespace OH
{
    public partial class frmBuyOrderDetail : Form
    {
        private Guid _BuyOrderID;
        private string _BuyOrderCode;

        public frmBuyOrderDetail()
        {
            InitializeComponent();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool needSend = !(ActiveControl is Button);
            if (needSend)
            {
                if (ActiveControl is Infragistics.Win.EmbeddableTextBoxWithUIPermissions)
                {
                    UltraGrid grid = ActiveControl.Parent as UltraGrid;
                    if (grid != null)
                    {
                        if (grid.ActiveCell != null)
                        {
                            Infragistics.Win.EditorWithText edit = grid.ActiveCell.Editor as Infragistics.Win.EditorWithText;
                            if (edit == null) edit = grid.ActiveCell.Column.Editor as Infragistics.Win.EditorWithText;
                            if (edit != null)
                            {
                                needSend = edit.ButtonsRight.Count == 0;
                            }
                        }
                    }
                    else
                    {
                        UltraTextEditor edit = ActiveControl.Parent as UltraTextEditor;
                        if (edit != null)
                        {
                            needSend = edit.ButtonsRight.Count == 0;
                        }
                    }
                }
            }
            if (needSend && (keyData == Keys.Enter || keyData == (Keys.Enter | Keys.Shift)))
            {
                if (keyData == Keys.Enter)
                    SendKeys.Send("{tab}");
                else
                    SendKeys.Send("+{tab}");
                return true;
            }
            else
                return base.ProcessCmdKey(ref msg, keyData);
        }

        private void txtBuyOrderCode_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            getBuyOrder(false);
        }
        public Guid BuyOrderID
        {
            get { return _BuyOrderID; }
        }
        public string BuyOrderCode
        {
            get { return _BuyOrderCode; }
        }
        private void getBuyOrder(bool isKeyDown)
        {
            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_BuyOrder"), null, "(D_BuyOrder.Finished=0 and D_BuyOrder.ConsignStatus<>2) and D_BuyOrder.CheckStatus=1", enuShowStatus.None);
            string txt = "";
            if (isKeyDown)
                txt = txtBuyOrderCode.Text;
            //DataRow dr = frm.Tools.ShowSelect("A.Code", txt, "A.WarehouseID='" + _WarehouseID + "'");
            DataRow dr = frm.ShowSelect("D_BuyOrder.Code", txt, null);
            if (dr != null)
            {
                txtBuyOrderCode.Tag = dr["ID"];
                txtBuyOrderCode.Text = (string)(string)dr["Code"];
                _BuyOrderID = (Guid)dr["ID"];
                _BuyOrderCode = (string)dr["Code"];
                DataSet dsBuyOrderBill = CSystem.Sys.Svr.cntMain.Select(string.Format("Select A.*,F.Name IngredientName,C.TaxRate,A.Price,B.NeedLength,B.AmountCheckMethod,B.QuantityCheckMethod,B.ConvertQuotiety,(case B.AmountCheckMethod when 1 then (A.Money/A.Quantity1) else (A.Money/A.Quantity2) end) PriceWithoutTax,B.Code MaterialCode,B.Name MaterialName,B.Spec,D.Name FirMeasure,E.Name SecMeasure,0.0 FactQuantity1,0.0 FactQuantity2,0.0 FactMoney from D_BuyOrderBill A inner join D_BuyOrder C on A.MainID=C.ID inner join P_Material B on A.MaterialID=B.ID inner join P_Ingredient F on A.IngredientID=F.ID inner join P_Measure D on B.FirMeasureID=D.ID left join P_Measure E on B.SecMeasureID=E.ID  where A.MainID='{0}'", dr["ID"]), "D_BuyOrderBill");
                grid.SetDataBinding(dsBuyOrderBill, "D_BuyOrderBill", true);
                //设置一下主辅数量编辑模式
                foreach (UltraGridRow row in grid.Rows)
                {
                    switch ((int)row.Cells["QuantityCheckMethod"].Value)
                    {
                        case 1:
                            row.Cells["FactQuantity2"].Activation = Activation.ActivateOnly;
                            break;
                        case 2:
                            row.Cells["FactQuantity1"].Activation = Activation.ActivateOnly;
                            break;
                    }
                }
           }
        }

        private void txtBuyOrderCode_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.Alt && e.KeyCode == Keys.Down))
            {
                getBuyOrder(true);
            }
        }
        private DataSet dsWarehouseInBill = null;
        public DataSet GetData()
        {
            if (this.ShowDialog() == DialogResult.OK)
            {
                if (dsWarehouseInBill.Tables[0].Rows.Count > 0)
                    return dsWarehouseInBill;
                else
                    return null;
            }
            return null;
        }
        private void butOK_Click(object sender, EventArgs e)
        {
            grid.PerformAction(UltraGridAction.ExitEditMode);
            if (txtBuyOrderCode.Tag == null || txtBuyOrderCode.Tag == DBNull.Value)
            {
                Msg.Information("没有选择采购订单！");
                return;
            }
            dsWarehouseInBill = CSystem.Sys.Svr.cntMain.Select(CSystem.Sys.Svr.LDI.GetFields("D_WarehouseInBill").QuerySQL + " Where 1=0", "D_WarehouseInBill");
            foreach (UltraGridRow row in grid.Rows)
            {
                DialogResult dResult= DialogResult.Yes;
                if (((int)row.Cells["AmountCheckMethod"].Value == 1 && (decimal)row.Cells["Quantity1"].Value - (decimal)row.Cells["FinishQuantity1"].Value < (decimal)row.Cells["FactQuantity1"].Value) ||
                    ((int)row.Cells["AmountCheckMethod"].Value == 2 && (decimal)row.Cells["Quantity2"].Value - (decimal)row.Cells["FinishQuantity2"].Value < (decimal)row.Cells["FactQuantity2"].Value))
                {
                    dResult = Msg.Question(string.Format("第{0}行输入的数量大于采购数量！\r\n是否继续？", row.Index + 1));
                    if (dResult != DialogResult.Yes)
                        return;
                }
                if ((row.Cells["FactQuantity1"].Value != DBNull.Value && (decimal)row.Cells["FactQuantity1"].Value != 0) || (row.Cells["FactQuantity2"].Value != DBNull.Value && (decimal)row.Cells["FactQuantity2"].Value != 0))
                {
                    if ((decimal)row.Cells["FactMoney"].Value == 0)
                    {
                        dResult = Msg.Question(string.Format("第{0}行没有输入金额！\r\n是否继续？", row.Index + 1));
                        if (dResult != DialogResult.Yes)
                            return;
                    }
                    DataRow dr = dsWarehouseInBill.Tables[0].NewRow();
                    dr["SourceID"] = row.Cells["ID"].Value;
                    dr["LineNumber"] = row.Cells["LineNumber"].Value;
                    dr["ID"] = System.Guid.NewGuid();
                    dr["MaterialID"] = row.Cells["MaterialID"].Value;
                    dr["MaterialCode"] = row.Cells["MaterialCode"].Value;
                    dr["MaterialName"] = row.Cells["MaterialName"].Value;
                    dr["Spec"] = row.Cells["Spec"].Value;
                    dr["IngredientID"] = row.Cells["IngredientID"].Value;
                    dr["IngredientName"] = row.Cells["IngredientName"].Value;
                    dr["FirMeasure"] = row.Cells["FirMeasure"].Value;
                    dr["SecMeasure"] = row.Cells["SecMeasure"].Value;
                    dr["AmountCheckMethod"] = row.Cells["AmountCheckMethod"].Value;
                    dr["NeedLength"] = row.Cells["NeedLength"].Value;
                    dr["QuantityCheckMethod"] = row.Cells["QuantityCheckMethod"].Value;
                    dr["ConvertQuotiety"] = row.Cells["ConvertQuotiety"].Value;
                    dr["Length"] = row.Cells["Length"].Value;
                    dr["Quantity1"] = row.Cells["FactQuantity1"].Value;
                    dr["Quantity2"] = row.Cells["FactQuantity2"].Value;
                    dr["Price"] = row.Cells["PriceWithoutTax"].Value;
                    dr["Money"] = row.Cells["FactMoney"].Value;
                    dsWarehouseInBill.Tables[0].Rows.Add(dr);
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void grid_AfterCellUpdate(object sender, CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "FactQuantity1":
                    if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 1)
                        e.Cell.Row.Cells["FactMoney"].Value = (decimal)e.Cell.Value * (decimal)e.Cell.Row.Cells["PriceWithoutTax"].Value;
                    if ((int)e.Cell.Row.Cells["QuantityCheckMethod"].Value == 1)
                    {
                        decimal length = 1;
                        if (e.Cell.Row.Cells["Length"].Activation == Activation.AllowEdit && e.Cell.Row.Cells["Length"].Value != DBNull.Value && (decimal)e.Cell.Row.Cells["Length"].Value!=0)
                            length = (decimal)e.Cell.Row.Cells["Length"].Value;
                        decimal conv = (decimal)e.Cell.Row.Cells["ConvertQuotiety"].Value;
                        if (conv == 0)
                            conv = 1;
                        e.Cell.Row.Cells["FactQuantity2"].Value = Math.Round((decimal)e.Cell.Value * length / conv, 4);
                    }
                    break;
                case "FactQuantity2":
                    if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 2)
                        e.Cell.Row.Cells["FactMoney"].Value = (decimal)e.Cell.Value * (decimal)e.Cell.Row.Cells["PriceWithoutTax"].Value;
                    if ((int)e.Cell.Row.Cells["QuantityCheckMethod"].Value == 2)
                    {
                        decimal length = 1;
                        if (e.Cell.Row.Cells["Length"].Activation == Activation.AllowEdit && e.Cell.Row.Cells["Length"].Value != DBNull.Value && (decimal)e.Cell.Row.Cells["Length"].Value != 0)
                            length = (decimal)e.Cell.Row.Cells["Length"].Value;
                        decimal conv = (decimal)e.Cell.Row.Cells["ConvertQuotiety"].Value;
                        if (conv == 0)
                            conv = 1;
                        e.Cell.Row.Cells["FactQuantity1"].Value = Math.Round((decimal)e.Cell.Value * length / conv, 4);
                    }
                    break;
            }
        }
    }
}