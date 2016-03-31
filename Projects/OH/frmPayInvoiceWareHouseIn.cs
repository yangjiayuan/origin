using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Base;
using UI;

namespace OH
{
    public partial class frmPayInvoiceWareHouseIn : Form
    {
        private Guid VendorID;
        public frmPayInvoiceWareHouseIn()
        {
            InitializeComponent();
        }

        private void txtBuyOrder_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            getBuyOrder(false);
        }
        private void getBuyOrder(bool isKeyDown)
        {
            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_BuyOrder"), null, string.Format("D_BuyOrder.CheckStatus=1 and D_BuyOrder.InvoiceStatus<2 and D_BuyOrder.SupplierID='{0}'", VendorID), enuShowStatus.None);
            string txt = "";
            if (isKeyDown)
                txt = txtBuyOrder.Text;
           
            DataRow dr = frm.ShowSelect("Code", txt, null);
            if (dr != null)
            {
                txtBuyOrder.Text = (string)dr["Code"];
                Guid ID = (Guid)dr["ID"];
                DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select A.*,B.Name WarehouseName from D_WarehouseIn A,P_Warehouse B where A.WarehouseID=B.ID and A.BuyOrderID='{0}'", ID));
                foreach (DataRow drWarehouseIn in ds.Tables[0].Rows)
                {
                    DataRow[] drs = _Data.Tables["D_PayWarehouseIn"].Select(string.Format("WarehouseInID='{0}'", drWarehouseIn["ID"]));
                    if (drs.Length > 0)
                        continue;
                    DataRow newDR = _Data.Tables["D_PayWarehouseIn"].NewRow();
                    newDR["BuyOrderCode"] = dr["Code"];
                    //newDR["BuyOrderID"] = dr["ID"];
                    //newDR["WarehouseID"] = drWarehouseIn["WarehouseID"];
                    newDR["WarehouseName"] = drWarehouseIn["WarehouseName"];
                    newDR["WarehouseInID"] = drWarehouseIn["ID"];
                    newDR["WarehouseInCode"] = drWarehouseIn["Code"];
                    newDR["BillDate"] = drWarehouseIn["BillDate"];
                    _Data.Tables["D_PayWarehouseIn"].Rows.Add(newDR);
                }
            }
        }
        private void txtWarehouse_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            getWarehouse(false);
        }

        private void getWarehouse(bool isKeyDown)
        {
            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("P_Warehouse"), null, "P_Warehouse.WarehouseType=0 or P_Warehouse.WarehouseType=1", enuShowStatus.None);
            string txt = "";
            if (isKeyDown)
                txt = txtWarehouse.Text;
            //DataRow dr = frm.Tools.ShowSelect("A.Code", txt, "A.WarehouseID='" + _WarehouseID + "'");
            DataRow dr = frm.ShowSelect("Code", txt, null);
            if (dr != null)
            {
                txtWarehouse.Text = (string)dr["Name"];
                txtWarehouse.Tag = (Guid)dr["ID"];
            }
        }

        private void txtWarehouseIn_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            getWarehouseIn(false);
        }

        private void getWarehouseIn(bool isKeyDown)
        {
            if (txtWarehouse.Tag==null)
            {
                Msg.Information("请先选择仓库!");
                return;
            }
            frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("D_WarehouseIn"), null, string.Format("D_WarehouseIn.WarehouseID='{0}' and D_WarehouseIn.Type=1 and D_WarehouseIn.BuyOrderID in ( Select ID from D_BuyOrder where CheckStatus=1 and InvoiceStatus<2 and SupplierID='{1}')", (Guid)txtWarehouse.Tag, VendorID), enuShowStatus.None);
            string txt = "";
            if (isKeyDown)
                txt = txtWarehouseIn.Text;
            //DataRow dr = frm.Tools.ShowSelect("A.Code", txt, "A.WarehouseID='" + _WarehouseID + "'");
            DataRow dr = frm.ShowSelect("Code", txt, null);
            if (dr != null)
            {
                DataRow[] drs = _Data.Tables["D_PayWarehouseIn"].Select(string.Format("WarehouseInID='{0}'", dr["ID"]));
                if (drs.Length > 0)
                    return;
                txtWarehouseIn.Text = (string)dr["Code"];
                DataRow newDR = _Data.Tables["D_PayWarehouseIn"].NewRow();
                newDR["BuyOrderCode"] = dr["BuyOrderCode"];
                //newDR["BuyOrderID"] = dr["ID"];
                //newDR["WarehouseID"] = dr["WarehouseID"];
                newDR["WarehouseName"] = dr["WarehouseName"];
                newDR["WarehouseInCode"] = dr["Code"];
                newDR["WarehouseInID"] = dr["ID"];
                newDR["BillDate"] = dr["BillDate"];
                _Data.Tables["D_PayWarehouseIn"].Rows.Add(newDR);
            }
        }
        private DataSet _Data = null;
        public DataSet GetData(DataSet data)
        {
            if (data.Tables["D_PayInvoice"].Rows[0]["VendorID"] == DBNull.Value)
            {
                Msg.Information("请先选择供应商!");
                return null;
            }
            VendorID = (Guid)data.Tables["D_PayInvoice"].Rows[0]["VendorID"];
            _Data = data.Clone();
            //if (this.ShowDialog() == DialogResult.OK)
            //{
            //    StringBuilder sb=new StringBuilder();
            //    sb.Append(" A.MainID in (");
            //    foreach (DataRow dr in _Data.Tables["D_PayWarehouseIn"].Rows)
            //    {
            //        sb.Append("'");
            //        sb.Append(dr["WarehouseInID"]);
            //        sb.Append("',");
            //    }
            //    if (sb.Length > " A.MainID in (".Length)
            //    {
            //        sb.Remove(sb.Length - 1, 1);
            //        sb.Append(")");
            //        DataSet ds = CSystem.Sys.Svr.cntMain.Select("Select A.ID,A.MainID,B.ID WarehouseInID,B.BuyOrderID,C.Code BuyOrderCode,A.SendBillCode,A.LineNumber,B.WarehouseID,D.Name WarehouseName,A.PositionID,E.Name PositionName," +
            //                                                    "A.MaterialID,F.Name MaterialName,F.Code MaterialCode,A.IngredientID,G.Name IngredientName,A.Length," +
            //                                                    "M1.Name FirMeasure,M2.Name SecMeasure,A.Quantity1,A.Quantity2,A.Price,A.Money " +
            //                                                    "from D_WarehouseInBill A " +
            //                                                    "inner join D_WarehouseIn B on A.MainID=B.ID " +
            //                                                    "inner join D_BuyOrder C on B.BuyOrderID=C.ID " +
            //                                                    "inner join P_Warehouse D on B.WarehouseID=D.ID " +
            //                                                    "inner join P_Position E on A.PositionID=E.ID " +
            //                                                    "inner join P_Material F on A.MaterialID=F.ID " +
            //                                                    "inner join P_Measure M1 on F.FirMeasureID=M1.ID " +
            //                                                    "left join P_Measure M2 on F.SecMeasureID=M2.ID " +
            //                                                    "left join P_Ingredient G on A.IngredientID=G.ID " +
            //                                                    "where  " + sb.ToString(),"V_PayWarehouseIn");
            //        _Data.Merge(ds);
            //        return _Data;
            //    }
            //    else
            //        return null;
            //}
            //else
                return null;
        }

        private void frmPayInvoiceWareHouseIn_Load(object sender, EventArgs e)
        {
            grid.SetDataBinding(_Data, "D_PayWarehouseIn", true);
        }

        private void grid_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
        {
            e.DisplayPromptMsg = false;
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            if (grid.Selected.Rows.Count == 0 && grid.ActiveRow != null)
                grid.ActiveRow.Selected = true;
            grid.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.DeleteRows);
        }
    }
}