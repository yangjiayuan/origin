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
    public partial class frmWarehouseMoveSelect : Form
    {
        private string sql = null;
        private COMFields fieldsWarehouseOut = null;
        private bool IsRepair = false;
        public frmWarehouseMoveSelect()
        {
            InitializeComponent();
        }
        public frmWarehouseMoveSelect(bool isRepair):this()
        {
            IsRepair = isRepair;
        }
        public DataSet GetMoveData(Guid warehouseID, DataSet dsIn)
        {
            fieldsWarehouseOut = CSystem.Sys.Svr.LDI.GetFields("D_WarehouseOut");
            sql = fieldsWarehouseOut.QuerySQL + " where D_WarehouseOut.MoveStatus=0 and D_WarehouseOut.TargetWarehouseID='" + warehouseID + "'";
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(sql);
            CCreateGrid createGrid = new CCreateGrid(fieldsWarehouseOut, this.grid, ds, Base.COMField.Enum_Visible.VisibleInBrower,null);
            if (this.ShowDialog() == DialogResult.OK && grid.ActiveRow != null)
            {
                COMFields fieldMain = CSystem.Sys.Svr.LDI.GetFields("D_WarehouseIn");
                //DataSet dsIn = CSystem.Sys.Svr.cntMain.Select(fieldMain.QuerySQL + " where 1=0", fieldMain.OrinalTableName);
                DataRow drMain = dsIn.Tables[0].Rows[0];
                Guid mainID = (Guid)drMain["ID"];
                drMain["WarehouseID"] = warehouseID;
                drMain["WarehouseOutID"] = grid.ActiveRow.Cells["ID"].Value;
                drMain["WarehouseOutCode"] = grid.ActiveRow.Cells["Code"].Value;
                drMain["WarehouseOutWorkCode"] = grid.ActiveRow.Cells["WorkCode"].Value;
                drMain["WarehouseOutWarehouse"] = grid.ActiveRow.Cells["WarehouseName"].Value;
                drMain["WarehouseOutPosition"] = grid.ActiveRow.Cells["PositionName"].Value;
                drMain["PositionID"] = grid.ActiveRow.Cells["TargetPositionID"].Value;
                drMain["PositionName"] = grid.ActiveRow.Cells["TargetPositionName"].Value;
                drMain["BillDate"] = grid.ActiveRow.Cells["BillDate"].Value;
                //dsIn.Tables[0].Rows.Add(drMain);

                COMFields fields2 = CSystem.Sys.Svr.LDI.GetFields("D_WarehouseOutBill");

                if (IsRepair)
                    sql = "select A.*,C.Code TargetCode,C.Name TargetName,C.ID TargetID from (" +
                        fields2.QuerySQL + " where D_WarehouseOutBill.MainID='" + grid.ActiveRow.Cells["ID"].Value + "') A " +
                        "inner join p_material B  on a.MaterialID=B.ID inner join P_Material C  on B.WIPID=C.ID";
                else
                    sql = fields2.QuerySQL + " where D_WarehouseOutBill.MainID='" + grid.ActiveRow.Cells["ID"].Value + "'";
                CSystem.Sys.Svr.cntMain.Select(sql, "D_WarehouseInBill", dsIn);
                foreach (DataRow dr in dsIn.Tables["D_WarehouseInBill"].Rows)
                {
                    dr["ID"] = Guid.NewGuid();
                    dr["MainID"] = mainID;
                    dr["PositionID"] = grid.ActiveRow.Cells["TargetPositionID"].Value;
                    dr["PositionName"] = grid.ActiveRow.Cells["TargetPositionName"].Value;
                    if (IsRepair)
                    {
                        dr["MaterialID"] = dr["TargetID"];
                        dr["MaterialCode"] = dr["TargetCode"];
                        dr["MaterialName"] = dr["TargetName"];
                    }
                    if ((int)grid.ActiveRow.Cells["ChangeQuantity"].Value == 1 && dr["Length"] != DBNull.Value)
                    {
                        decimal length = (decimal)dr["Length"];
                        if (length != 0)
                        {
                            dr["Length"] = 5;
                            dr["Quantity1"] = (decimal)dr["Quantity1"] * length / 5;
                            //dr["Quantity2"] = (decimal)dr["Quantity2"] * length / 5;
                        }
                    }
                    dr.AcceptChanges();
                    dr.SetAdded();
                }
                if (IsRepair)
                {
                    dsIn.Tables["D_WarehouseInBill"].Columns.Remove("TargetID");
                    dsIn.Tables["D_WarehouseInBill"].Columns.Remove("TargetCode");
                    dsIn.Tables["D_WarehouseInBill"].Columns.Remove("TargetName");
                }
                return dsIn;
            }
            return null;
        }

        private void butSearch_Click(object sender, EventArgs e)
        {
            string s = null;
            if (txtCode.Text.Length > 0)
                s = sql + " and D_WarehouseOut.WorkCode like '%" + txtCode.Text + "%'";
            else
                s = sql;
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(s);
            CCreateGrid createGrid = new CCreateGrid(fieldsWarehouseOut, this.grid, ds, Base.COMField.Enum_Visible.VisibleInBrower, null);
        }
    }
}