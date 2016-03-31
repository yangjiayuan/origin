using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using System.Data.SqlClient;
using Base;
using UI;
using Infragistics.Win.Printing;


namespace OH
{
    public class clsWarehouseDetailForm : ToolDetailForm
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out   int ID);

        public enum enuWarehouseIn : int { In = 0, BuyIn = 1, MoveIn = 2, RepairIn = 3};
        public enum enuWarehouseOut : int { Out = 0, ProduceOut = 1, MoveOut = 2, ScrapOut = 3, CheckOut = 4, ReturnOut = 5, RepairOut = 6 };
        public enum enuWarehouseOperate : int { In = 0, BuyIn = 1, MoveIn = 2, RepairIn = 3, Out = 4, ProduceOut = 5, MoveOut = 6, ScrapOut = 7, CheckOut = 8, ReturnOut = 9, RepairOut = 10 };
        private enuWarehouseOperate _WarehouseOperate;
        private Guid _WarehouseID;
        private string _WarehouseCode;
        private string _WarehouseName;
        private int _WarehouseType;
        private bool _IsFittingPart;

        private ToolStripButton toolPrint;
        private ToolStripButton toolPreview;

        public clsWarehouseDetailForm()
        {
        }
        public clsWarehouseDetailForm(enuWarehouseOperate warehouseOperate, Guid warehouseID, string WarehouseCode, string WarehouseName, int WarehouseType)
        {
            _WarehouseOperate = warehouseOperate;
            _WarehouseID = warehouseID;
            _WarehouseCode = WarehouseCode;
            _WarehouseName = WarehouseName;
            _WarehouseType = WarehouseType;
        }
        public override bool AutoCode
        {
            get
            {
                return true;
            }
        }
        public override string GetPrefix()
        {
            if (_WarehouseOperate < enuWarehouseOperate.Out)
                return "CK" + _WarehouseCode;
            else
                return "RK" + _WarehouseCode;
        }
        public override void SetCreateDetail(CCreateDetailForm createDetailForm)
        {
            base.SetCreateDetail(createDetailForm);
            createDetailForm.BeforeSelectForm += new BeforeSelectFormEventHandler(createDetailForm_BeforeSelectForm);
            createDetailForm.AfterSelectForm += new AfterSelectFormEventHandler(createDetailForm_AfterSelectForm);
        }

        void createDetailForm_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {
            switch (e.Field.FieldName)
            {
                case "TargetUserName":
                    string OperatorName = (string)_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["TargetUserName"];
                    Guid OperatorID = (Guid)_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["TargetUserID"];
                    foreach (DataRow dr in _DetailForm.MainDataSet.Tables["D_WarehouseOutBill"].Rows)
                    {
                        //if (dr["OperatorID"] ==null || dr["OperatorID"] == DBNull.Value)
                        {
                            dr["OperatorID"] = OperatorID;
                            dr["OperatorName"] = OperatorName;
                        }
                    }
                    break;
                case "PositionName":
                    string PositionName = (string)_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["PositionName"];
                    Guid PositionID = (Guid)_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["PositionID"];
                    System.Data.DataTable dt;
                    if (_DetailForm.MainDataSet.Tables.Contains("D_WarehouseOutBill"))
                        dt = _DetailForm.MainDataSet.Tables["D_WarehouseOutBill"];
                    else
                        dt = _DetailForm.MainDataSet.Tables["D_WarehouseInBill"];
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["PositionID"] == null || dr["PositionID"] == DBNull.Value)
                        {
                            dr["PositionID"] = PositionID;
                            dr["PositionName"] = PositionName;
                            dr["PricePositionID"] = e.Row["PricePositionID"];
                        }
                    }
                    break;
                case "MachineName":
                    string MachineName = (string)_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["MachineName"];
                    Guid MachineID = (Guid)_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["MachineID"];
                    foreach (DataRow dr in _DetailForm.MainDataSet.Tables["D_WarehouseOutBill"].Rows)
                    {
                        //if (dr["MachineID"] == null || dr["MachineID"] == DBNull.Value)
                        {
                            dr["MachineID"] = MachineID;
                            dr["MachineName"] = MachineName;
                        }
                    }
                    break;
            }
        }

        void createDetailForm_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            switch (e.Field.FieldName)
            {
                case "TargetPositionName":
                    if (this._DetailForm.MainDataSet.Tables["D_WarehouseOut"].Rows[0]["TargetWarehouseID"] != DBNull.Value)
                    {
                        e.Where = "P_Position.WarehouseID='" + this._DetailForm.MainDataSet.Tables["D_WarehouseOut"].Rows[0]["TargetWarehouseID"] + "'";
                    }
                    break;
                case "TargetWarehouseName":
                    if (_IsFittingPart)
                        e.Where = "(P_Warehouse.WarehouseType=2 or P_Warehouse.WarehouseType=3)";
                    else if (_WarehouseOperate== enuWarehouseOperate.RepairOut)
                        e.Where = "P_Warehouse.WarehouseType=" + 1;
                    else
                        e.Where = "P_Warehouse.WarehouseType=" + _WarehouseType;
                    break;
                case "TargetUserName":
                    if (this._DetailForm.MainDataSet.Tables["D_WarehouseOut"].Rows[0]["TargetDepartID"] != DBNull.Value)
                    {
                        e.Where = "P_Operator.DepartmentID='" + this._DetailForm.MainDataSet.Tables["D_WarehouseOut"].Rows[0]["TargetDepartID"] + "'";
                    }
                    break;
                case "MachineName":
                    if (this._DetailForm.MainDataSet.Tables["D_WarehouseOut"].Rows[0]["TargetDepartID"] != DBNull.Value)
                    {
                        e.Where = "P_Machine.DepartmentID='" + this._DetailForm.MainDataSet.Tables["D_WarehouseOut"].Rows[0]["TargetDepartID"] + "'";
                    }
                    break;
                case "PositionName":
                    e.Where = "P_Position.WarehouseID='" + _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["WarehouseID"] + "'";
                    break;
            }
        }
        public override void SetCreateGrid(CCreateGrid createGrid)
        {
            base.SetCreateGrid(createGrid);
            createGrid.AfterSelectForm += new AfterSelectFormEventHandler(_CreateGrid_AfterSelectForm);
            createGrid.BeforeSelectForm += new BeforeSelectFormEventHandler(_CreateGrid_BeforeSelectForm);
        }
        void _CreateGrid_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            string[] s = e.Field.ValueType.Split(':');
            switch (s[1])
            {
                case "P_Position":
                    e.Where = "P_Position.WarehouseID='" + _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["WarehouseID"] + "'";
                    break;
                case "P_Material":
                    if (_WarehouseType == 4)
                    {
                        Guid warehouseID = (Guid)this._DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["WarehouseID"];
                        CCreateGrid createdGrid = sender as CCreateGrid;
                        if (createdGrid.Grid.ActiveRow.Cells["PositionID"].Value == DBNull.Value)
                            e.Where = string.Format("P_Material.ID in (select B.MaterialID from C_Price A,C_PriceBill B where A.ID=B.MainID and A.WarehouseID ='{0}')", warehouseID);
                        else
                        {
                            Guid positionID = (Guid)createdGrid.Grid.ActiveRow.Cells["PositionID"].Value;
                            if (createdGrid.Grid.ActiveRow.Cells["PricePositionID"].Value == DBNull.Value)
                                e.Where = string.Format("P_Material.ID in (select B.MaterialID from C_Price A,C_PriceBill B where A.ID=B.MainID and A.WarehouseID ='{0}' and A.PositionID='{1}')", warehouseID, positionID);
                            else
                                e.Where = string.Format("P_Material.ID in (select B.MaterialID from C_Price A,C_PriceBill B where A.ID=B.MainID)");
                        }
                    }
                    else
                        e.Where = "P_Material.MaterialType=" + _WarehouseType;
                    break;
                case "P_MachiningStandard":
                    CCreateGrid createdGrid2= sender as CCreateGrid;
                    if ( createdGrid2.Grid.ActiveRow.Cells["MaterialID"].Value == DBNull.Value)
                        return;
                    if (_WarehouseType==2)//成品
                        e.Where = "P_MachiningStandard.MaterialID in (Select WIPID from P_Material where ID='" + createdGrid2.Grid.ActiveRow.Cells["MaterialID"].Value + "')";
                    else
                        e.Where = "P_MachiningStandard.MaterialID ='" + createdGrid2.Grid.ActiveRow.Cells["MaterialID"].Value + "'";
                    break;
                case "P_Machine":
                    if (_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["TargetDepartID"] != DBNull.Value)
                        e.Where = "P_Machine.DepartmentID='" + _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["TargetDepartID"] + "'";
                    break;
            }
        }

        void _CreateGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {
            string  fieldName = e.Field.FieldName;

            //需要判断是否是辅料仓库，并取得单价
            if (fieldName == "MaterialCode" || fieldName == "MaterialName")
            {
                //复制一些字段,仅指上一行的
                if (e.GridRow.Index > 0)
                {
                    UltraGrid grid = ((CCreateGrid)sender).Grid;
                    UltraGridRow lastRow = grid.Rows[e.GridRow.Index - 1];
                    //e.GridRow.Cells["PositionID"].Value = lastRow.Cells["PositionID"].Value;
                    //e.GridRow.Cells["PositionName"].Value = lastRow.Cells["PositionName"].Value;
                    if (lastRow.Band.Columns.Exists("MachineID"))
                    {
                        e.GridRow.Cells["MachineID"].Value = lastRow.Cells["MachineID"].Value;
                        e.GridRow.Cells["MachineName"].Value = lastRow.Cells["MachineName"].Value;
                    }
                    if (lastRow.Band.Columns.Exists("OperatorID"))
                    {
                        e.GridRow.Cells["OperatorID"].Value = lastRow.Cells["OperatorID"].Value;
                        e.GridRow.Cells["OperatorName"].Value = lastRow.Cells["OperatorName"].Value;
                    }
                }

                //设置长度是否可以修改
                if ((int)e.Row["NeedLength"] == 1)
                    e.GridRow.Cells["Length"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                else
                    e.GridRow.Cells["Length"].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                if (e.Row["SecMeasureID"] == DBNull.Value)
                {
                    e.GridRow.Cells["Quantity2"].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                }
                //如果辅料，引入价格
                if ((int)_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["WarehouseType"] == 4)
                {
                    if (e.GridRow.Cells["PositionID"].Value != DBNull.Value)
                    {
                        DataSet ds;
                        if (e.GridRow.Cells["PricePositionID"].Value == DBNull.Value)
                            ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select Price from C_Price A,C_PriceBill B where A.ID=B.MainID and B.MaterialID='{0}' and A.PositionID='{1}'", e.GridRow.Cells["MaterialID"].Value, e.GridRow.Cells["PositionID"].Value));
                        else
                            ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select Price from C_Price A,C_PriceBill B where A.ID=B.MainID and B.MaterialID='{0}'", e.GridRow.Cells["MaterialID"].Value));
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            e.GridRow.Cells["Price"].Value = ds.Tables[0].Rows[0]["Price"];
                        }
                    }
                }
            }
            else if (fieldName == "DesignCodeName")
            {
                if (e.Row["MaterialID"] != DBNull.Value)
                {
                    e.GridRow.Cells["IngredientID"].Value = e.Row["IngredientID"];
                    e.GridRow.Cells["IngredientName"].Value = e.Row["IngredientName"];
                    e.GridRow.Cells["Length"].Value = e.Row["Length"];
                    e.GridRow.Cells["MaterialID"].Value = e.Row["MaterialID"];
                    e.GridRow.Cells["MaterialCode"].Value = e.Row["MaterialCode"];
                    e.GridRow.Cells["MaterialName"].Value = e.Row["MaterialName"];
                    e.GridRow.Cells["FirMeasure"].Value = e.Row["FirMeasure"];
                    e.GridRow.Cells["SecMeasure"].Value = e.Row["SecMeasure"];
                    e.GridRow.Cells["ConvertQuotiety"].Value = e.Row["ConvertQuotiety"];
                    e.GridRow.Cells["QuantityCheckMethod"].Value = e.Row["QuantityCheckMethod"];
                    e.GridRow.Cells["AmountCheckMethod"].Value = e.Row["AmountCheckMethod"];
                    e.GridRow.Cells["NeedLength"].Value = e.Row["NeedLength"];
                    if ((int)e.Row["NeedLength"] == 1)
                        e.GridRow.Cells["Length"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                    else
                        e.GridRow.Cells["Length"].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                }
            }
        }


        public override void NewGrid(COMFields fields, UltraGrid grid)
        {
            grid.AfterCellUpdate += new CellEventHandler(grid_AfterCellUpdate);
            grid.AfterRowInsert += new RowEventHandler(grid_AfterRowInsert);
        }

        void grid_AfterRowInsert(object sender, RowEventArgs e)
        {
            if (_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Columns.Contains("TargetUserID"))
            {
                e.Row.Cells["OperatorID"].Value = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["TargetUserID"];
                e.Row.Cells["OperatorName"].Value = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["TargetUserName"];
            }
            //if (_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Columns.Contains("PositionID"))
            //{
            //    e.Row.Cells["PositionID"].Value = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["PositionID"];
            //    e.Row.Cells["PositionName"].Value = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["PositionName"];
            //}
            if (_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Columns.Contains("MachineID"))
            {
                e.Row.Cells["MachineID"].Value = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["MachineID"];
                e.Row.Cells["MachineName"].Value = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["MachineName"];
            }
            if (e.Row.Index == 0)
            {
                //使用头上的库位
                e.Row.Cells["PositionID"].Value = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["PositionID"];
                e.Row.Cells["PositionName"].Value = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["PositionName"];
                e.Row.Cells["PricePositionID"].Value = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["PricePositionID"];
            }
            else
            {
                UltraGrid grid=(UltraGrid)sender;
                UltraGridRow row = grid.Rows[e.Row.Index - 1];
                e.Row.Cells["PositionID"].Value =  row.Cells["PositionID"].Value;
                e.Row.Cells["PositionName"].Value = row.Cells["PositionName"].Value;
                e.Row.Cells["PricePositionID"].Value = row.Cells["PricePositionID"].Value;
            }
        }
        private bool Updating = false;
        void grid_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (Updating) return;
            switch (e.Cell.Column.Key)
            {
                case "Quantity1":
                    if ((int)e.Cell.Row.Cells["QuantityCheckMethod"].Value == 1)
                    {
                        decimal length = 1;
                        if (e.Cell.Row.Cells["Length"].Activation == Activation.AllowEdit && e.Cell.Row.Cells["Length"].Value !=DBNull.Value)
                            length = (decimal)e.Cell.Row.Cells["Length"].Value;
                        decimal conv = (decimal)e.Cell.Row.Cells["ConvertQuotiety"].Value;
                        if (conv == 0)
                            conv = 1;
                        if (length == 0)
                            length = 1;
                        e.Cell.Row.Cells["Quantity2"].Value = Math.Round((decimal)e.Cell.Value * length / conv, 4);
                    }
                    if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 1)
                    {
                        if (e.Cell.Row.Cells["Price"].Value != DBNull.Value)
                        {
                            Updating = true;
                            e.Cell.Row.Cells["Money"].Value = (decimal)e.Cell.Row.Cells["Price"].Value * (decimal)e.Cell.Value;
                            GetTotal((UltraGrid)sender);
                            Updating = false;
                        }
                    }
                    break;
                case "Quantity2":
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
                        e.Cell.Row.Cells["Quantity1"].Value = Math.Round((decimal)e.Cell.Value * length / conv, 4);
                    }
                    if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 2)
                    {
                        if (e.Cell.Row.Cells["Price"].Value != DBNull.Value)
                        {
                            Updating = true;
                            e.Cell.Row.Cells["Money"].Value = (decimal)e.Cell.Row.Cells["Price"].Value * (decimal)e.Cell.Value;
                            GetTotal((UltraGrid)sender);
                            Updating = false;
                        }
                    }
                    break;
                case "Price":
                    decimal qty = 0;
                    if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 1)
                    {
                        if (e.Cell.Row.Cells["Quantity1"].Value != DBNull.Value)
                            qty = (decimal)e.Cell.Row.Cells["Quantity1"].Value;
                        else
                            break;
                    }
                    else if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 2)
                    {
                        if (e.Cell.Row.Cells["Quantity2"].Value != DBNull.Value)
                            qty = (decimal)e.Cell.Row.Cells["Quantity2"].Value;
                        else
                            break;
                    }
                    Updating = true;
                    e.Cell.Row.Cells["Money"].Value = (decimal)e.Cell.Value * qty;
                    GetTotal((UltraGrid)sender);
                    Updating = false;
                    break;
                case "Money":
                    //计算总金额
                    UltraGrid grid = (UltraGrid)sender;
                    decimal total = 0;
                    foreach (UltraGridRow row in grid.Rows)
                        if (row.Cells["Money"].Value!=DBNull.Value)
                            total += (decimal)row.Cells["Money"].Value;
                    
                    if (_ControlMap.ContainsKey("Amount"))
                        _ControlMap["Amount"].Text = total.ToString();
                    if (e.Cell.Row.Cells.Exists("Price"))
                    {
                        //计算单价
                        decimal qty2 = 0;
                        if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 1)
                        {
                            if (e.Cell.Row.Cells["Quantity1"].Value != DBNull.Value)
                            {
                                qty2 = (decimal)e.Cell.Row.Cells["Quantity1"].Value;
                            }
                        }
                        else if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 2)
                        {
                            if (e.Cell.Row.Cells["Quantity2"].Value==DBNull.Value)
                                qty2 = (decimal)e.Cell.Row.Cells["Quantity2"].Value;
                        }
                        else
                            break;
                        if (qty2 != 0)
                        {
                            Updating = true;
                            e.Cell.Row.Cells["Price"].Value = (decimal)e.Cell.Value / qty2;
                            Updating = false;
                        }
                    }
                    break;
            }
        }
        private void GetTotal(UltraGrid grid)
        {
            decimal total = 0;
            foreach (UltraGridRow row in grid.Rows)
                if (row.Cells["Money"].Value != DBNull.Value)
                    total += (decimal)row.Cells["Money"].Value;

            if (_ControlMap.ContainsKey("Amount"))
                _ControlMap["Amount"].Text = total.ToString();

        }
        public override bool NewData(DataSet ds, COMFields mainTableDefine, List<COMFields> detailTableDefine)
        {
            Guid defaultPositionID = CSystem.Sys.Svr.NullID;
            string defaultPositionName = null;
            object pricePositionID = DBNull.Value;
            if (_WarehouseOperate != enuWarehouseOperate.MoveIn)
            {
                //查询出默认库位
                DataSet dsPosition = CSystem.Sys.Svr.cntMain.Select(string.Format("Select A.DefaultPositionID PositionID,B.Name PositionName,B.PricePositionID from P_Warehouse A,P_Position B where A.DefaultPositionID=B.ID and A.ID='{0}'", _WarehouseID));
                if (ds.Tables[0].Rows.Count == 1)
                {
                    try
                    {
                        defaultPositionID = (Guid)dsPosition.Tables[0].Rows[0]["PositionID"];
                        defaultPositionName = dsPosition.Tables[0].Rows[0]["PositionName"] as string;
                        pricePositionID = dsPosition.Tables[0].Rows[0]["PricePositionID"];
                    }catch{
                    }
                }
            }
            //普通入库；移库入库；采购入库；
            //不处理；先选择移库单；先选择采购单，再确定其中的数量和金额；
            //普通出库；移库出库；报废出库；盘点出库；退货出库；
            //移库：只是显示出目标的仓库和库位，且必须选择；其中，有插入行，只是类别不同。
            switch (_WarehouseOperate)
            {
                case enuWarehouseOperate.BuyIn:
                    frmBuyOrderDetail frm = new frmBuyOrderDetail();
                    DataSet dsBuyIn = frm.GetData();
                    if (dsBuyIn == null)
                        return false;
                    else
                    {
                        ds.Merge(dsBuyIn);
                        ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["BuyOrderID"]=frm.BuyOrderID;
                        ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["BuyOrderCode"]=frm.BuyOrderCode;
                        //计算出合计数
                        decimal total=0;
                        foreach (DataRow dr in ds.Tables[detailTableDefine[0].OrinalTableName].Rows)
                            if (dr["Money"]!=DBNull.Value)
                                total += (decimal)dr["Money"];
                        ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["Amount"] = total;
                        if (defaultPositionID!= CSystem.Sys.Svr.NullID)
                        {
                            ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["PositionID"] = defaultPositionID;
                            ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["PositionName"] = defaultPositionName;
                            ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["PricePositionID"] = pricePositionID;
                        }
                    }
                    return true;
                case enuWarehouseOperate.MoveIn:
                    DataSet dsMoveIn = new frmWarehouseMoveSelect().GetMoveData(_WarehouseID,ds);
                    if (dsMoveIn == null)
                        return false;
                    //else
                    //    ds.Merge(dsMoveIn);
                    return true;
                case enuWarehouseOperate.RepairIn:
                    DataSet dsRepairIn = new frmWarehouseMoveSelect(true).GetMoveData(_WarehouseID, ds);
                    if (dsRepairIn == null)
                        return false;
                    return true;
                case enuWarehouseOperate.Out:
                case enuWarehouseOperate.MoveOut:
                case enuWarehouseOperate.RepairOut:
                default:
                    if (defaultPositionID != CSystem.Sys.Svr.NullID)
                    {
                        ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["PositionID"] = defaultPositionID;
                        ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["PositionName"] = defaultPositionName;
                        ds.Tables[mainTableDefine.OrinalTableName].Rows[0]["PricePositionID"] = pricePositionID;
                    }
                    return true;
            }
        }

        public override DataSet InsertRowsInGrid(List<COMFields> detailTableDefine)
        {
            if (_WarehouseOperate >= enuWarehouseOperate.Out )
            {
                if ((int)_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["IsZeroStock"] == 1)
                    return null;
                else
                {
                    DataSet ds = new frmWarehouseOutSelect(_WarehouseID, detailTableDefine[0]).GetData();
                    if (ds!=null && _WarehouseType == 4)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            dr["OperatorID"] = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["TargetUserID"];
                            dr["OperatorName"] = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["TargetUserName"];
                            dr["MachineID"] = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["MachineID"];
                            dr["MachineName"] = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["MachineName"];
                            dr["PositionID"] = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["PositionID"];
                            dr["PositionName"] = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["PositionName"];
                            dr["PricePositionID"] = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["PricePositionID"];
                        }
                    }
                    return ds;
                }
            }
            else
                return null;
        }

        public override bool AllowDeleteRowInToolBar
        {
            get
            {
                switch (_WarehouseOperate)
                {
                    case enuWarehouseOperate.In:
                    case enuWarehouseOperate.RepairIn:
                        return true;
                    default:
                        if (_WarehouseOperate >= clsWarehouseDetailForm.enuWarehouseOperate.Out)
                            return true;
                        else
                            return false;
                }
            }
        }
        public override bool AllowCopyLine(string TableName)
        {
            if (_WarehouseOperate>= enuWarehouseOperate.Out)
                return false;
            else
                return true;
        }
        public override bool AllowInsertRowInGrid(string TableName)
        {
            switch (_WarehouseOperate)
            {
                case enuWarehouseOperate.MoveOut:
                case enuWarehouseOperate.Out:
                    if ((int)_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["IsZeroStock"] == 1)
                        return true;
                    else
                        return false;
                case enuWarehouseOperate.MoveIn:
                case enuWarehouseOperate.BuyIn:
                case enuWarehouseOperate.RepairIn:
                case enuWarehouseOperate.RepairOut:
                    return false;
                default:
                    return true;
            }
        }

        public override bool AllowUpdateInGrid(string TableName)
        {
            switch (_WarehouseOperate)
            {
                case enuWarehouseOperate.MoveIn:
                case enuWarehouseOperate.RepairIn:
                    return false;
                default:
                    return true;
            }
        }

        public override bool AllowInsertRowInToolBar
        {
            get
            {
                if (_WarehouseOperate >= enuWarehouseOperate.Out)
                    return true;
                else
                    return false;
            }
        }
        public override bool AllowCheck
        {
            get
            {
                if (_WarehouseOperate == enuWarehouseOperate.RepairIn)
                    return false;
                else
                    return true;
            }
        }
        public override bool AllowEdit
        {
            get
            {
                if (_WarehouseOperate == enuWarehouseOperate.RepairIn)
                    return false;
                else
                    return true;
            }
        }
        public override void AfterSetNewRowIDWhenCopy(DataSet ds)
        {
            base.AfterSetNewRowIDWhenCopy(ds);
            if (ds.Tables[this.MainTableName].Columns.Contains("MoveStatus"))
                ds.Tables[this.MainTableName].Rows[0]["MoveStatus"] = 0;
            if (ds.Tables[this.MainTableName].Columns.Contains("WarehouseInID"))
                ds.Tables[this.MainTableName].Rows[0]["WarehouseInID"] = DBNull.Value;
            if (ds.Tables[this.MainTableName].Columns.Contains("AllocationID"))
                ds.Tables[this.MainTableName].Rows[0]["AllocationID"] = DBNull.Value;
            if (ds.Tables[this.MainTableName].Columns.Contains("InvoiceStatus"))
                 ds.Tables[this.MainTableName].Rows[0]["InvoiceStatus"] = 0;
        }


        public override void InsertToolStrip(ToolStrip toolStrip, ToolDetailForm.enuInsertToolStripType insertType)
        {
            base.InsertToolStrip(toolStrip, insertType);
            int i = toolStrip.Items.Count;

            if (insertType == enuInsertToolStripType.Detail)
            {
                ToolStripSeparator tss = new ToolStripSeparator();
                toolStrip.Items.Insert(i - 2, tss);

                //导入
                ToolStripButton toolImport = new ToolStripButton();
                toolImport.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolImport.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolImport.Name = "toolSplit";

                toolImport.Text = "导入";
                toolImport.Image = clsResources.Import;
                toolImport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolImport.Click += new EventHandler(toolImport_Click);

                toolStrip.Items.Insert(i - 1, toolImport);
            }
            if (_WarehouseOperate >= enuWarehouseOperate.Out)
            {
                i = i - 2;
                toolPreview = new ToolStripButton();
                toolPreview.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolPreview.Name = "toolPreview";
                toolPreview.Size = new System.Drawing.Size(39, 34);
                toolPreview.Text = "打印";
                toolPreview.Image = UI.clsResources.Preview;
                toolPreview.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolPreview.Click += new EventHandler(toolPrint_Click);
                toolStrip.Items.Insert(i, toolPreview);

                toolPrint = new ToolStripButton();
                toolPrint.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolPrint.Name = "toolPrint";
                toolPrint.Size = new System.Drawing.Size(39, 34);
                toolPrint.Text = "预览";
                toolPrint.Image = UI.clsResources.Print;
                toolPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolPrint.Click += new EventHandler(toolPreview_Click);
                toolStrip.Items.Insert(i, toolPrint);
            }
        }
        private void toolPreview_Click(object sender, EventArgs e)
        {
            Print(true);
        }
        private void toolPrint_Click(object sender, EventArgs e)
        {
            Print(false);
        }
        private void Print(bool IsPreview)
        {
            if (this._BrowserForm.grid.ActiveRow == null)
                return;

            frmPrintSetting ps = new frmPrintSetting(AppDomain.CurrentDomain.BaseDirectory + "PrintTemplate\\WarehouseOut");
            if (ps.ShowDialog() == DialogResult.OK)
            {
                string tempFile = ps.TempateFile;
                clsPrintTemplate Template = new clsPrintTemplate(tempFile);

                if (_BrowserForm.grid.Selected.Rows.Count == 0 && _BrowserForm.grid.ActiveRow != null)
                    _BrowserForm.grid.ActiveRow.Selected = true;

                int index = 1;
                int count = _BrowserForm.grid.Selected.Rows.Count;
                foreach (UltraGridRow row in _BrowserForm.grid.Selected.Rows)
                {
                    Guid id = (Guid)row.Cells["ID"].Value;

                    DataSet ds = GetData(id);
                    clsPrintDocument doc = new clsPrintDocument(Template, ds, ps.PrintSetting);
                    if (IsPreview)
                    {
                        if (count > 1 && Msg.Question(string.Format("将要预览第{0}领料单，共{1}张领料单，是否继续？", index, count)) != DialogResult.Yes)
                            return;
                        index++;

                        UltraPrintPreviewDialog pre = new UltraPrintPreviewDialog();
                        pre.Document = doc;
                        pre.ShowDialog();
                    }
                    else
                        doc.Print();
                }
            }
        }
        private DataSet GetData(Guid id)
        {
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(this.MainCOMFields.QuerySQLWithClause(this.MainTableName + ".ID='" + id + "'"), this.MainTableName);
            CSystem.Sys.Svr.cntMain.Select(this.DetailCOMFields[0].QuerySQLWithClause(this.DetailCOMFields[0].GetTableName(false) + ".MainID='" + id + "'"), this.DetailCOMFields[0].GetTableName(false), ds);
            return ds;
        }
        void toolImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Filter = "Excel文件(*.xls)|*.xls";
            ofd.DefaultExt = "xls";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                Excel.Workbook wb= excel.Workbooks.Open(ofd.FileName,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing);
                Excel.Worksheet workSheet = (Worksheet)wb.Worksheets[1];

                int colCount = workSheet.UsedRange.Columns.Count; //获得列数
                int rowCount = workSheet.UsedRange.Rows.Count; //获得行数
                string lasColumn = char.ConvertFromUtf32(char.ConvertToUtf32("A", 0) + colCount - 1);
                Excel.Range rData = workSheet.get_Range("A1", lasColumn + rowCount);
                
                object[,] obj = (object[,])rData.get_Value(Type.Missing);
                //将列名放入集合
                SortedList<string, int> fields = new SortedList<string, int>();
                for (int i = 1; i <= colCount; i++)
                {
                    string f = obj[1,i] as string;
                    if (f!=null && !fields.ContainsKey(f))
                        fields.Add(f, i);
                }
                //读数据
                COMFields tableDefine = _DetailForm.DetailTableDefine[0];
                System.Data.DataTable dt = _DetailForm.MainDataSet.Tables[tableDefine.OrinalTableName];
                for (int i = 2; i <= rowCount; i++)
                {
                    DataRow dr = dt.NewRow();
                    string location = obj[i, fields["库位"]] as string;
                    if (location == null)
                        break;
                    COMFields fieldTableDefine = null;
                    //读库位ID
                    if (!SetValue(GetFields("P_Position", "Name", location, out fieldTableDefine, " and P_Position.WarehouseID='" + _WarehouseID+"'"), dr, tableDefine, "PositionName", fieldTableDefine))
                        break;
                    //读物料ID
                    if (!SetValue(GetFields("P_Material", "Code", (string)obj[i, fields["物料代码"]], out fieldTableDefine), dr, tableDefine, "MaterialCode", fieldTableDefine))
                        break;
                    //读材质ID
                    if (fields.ContainsKey("材质"))
                    {
                        if (!SetValue(GetFields("P_Ingredient", "Name", (string)obj[i, fields["材质"]], out fieldTableDefine), dr, tableDefine, "IngredientName", fieldTableDefine))
                            break;
                    }
                    //读加工图号
                    if (fields.ContainsKey("加工图号"))
                    {
                        if (!SetValue(GetFields("P_MachiningStandard", "Name", (string)obj[i, fields["加工图号"]], out fieldTableDefine), dr, tableDefine, "MachiningStandardName", fieldTableDefine))
                            break;
                    }
                    //读长度,
                    if (obj[i,fields["长度"]]!=null && obj[i,fields["长度"]].GetType()==typeof(double))
                        dr["Length"]=(double)obj[i,fields["长度"]];
                    else
                        dr["Length"]=0;
                    
                    //主数量
                    if (obj[i,fields["主数量"]].GetType()==typeof(double))
                        dr["Quantity1"]=(double)obj[i,fields["主数量"]];
                    else
                        dr["Quantity1"] = 0;
                    
                    //辅数量
                    if (obj[i,fields["辅数量"]].GetType()==typeof(double))
                        dr["Quantity2"] = (double)obj[i, fields["辅数量"]];
                    else
                        dr["Quantity2"] = 0;

                    dt.Rows.Add(dr);
                }

                //关闭当前的excel进程
                excel.Application.Workbooks.Close();
                excel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                IntPtr t = new IntPtr(excel.Hwnd); 
                excel = null;
                int kid = 0;
                GetWindowThreadProcessId(t, out kid);
                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(kid);
                p.Kill();
            }
        }
        private bool SetValue(DataRow fieldRow, DataRow newRow,COMFields tableDefine,string fieldName,COMFields fieldTableDefine)
        {
            COMField f = tableDefine[fieldName];
            if (fieldRow == null)
                return false;
            foreach (COMField field in tableDefine.Fields)
            {
                if (field.TableRelation == f.TableRelation)
                {
                    newRow[field.FieldName] = fieldRow[field.RFieldName];
                }
                if (field.RelationPath != null && field.RelationPath.Length > 0 && field.RelationPath.StartsWith(f.TableRelation))
                {
                    if (field.RelationPath == f.TableRelation)
                    {
                        foreach (COMField f2 in fieldTableDefine.Fields)
                        {
                            if (f2.TableRelation == field.TableRelation && f2.RFieldName == field.RFieldName)
                                newRow[field.FieldName] = fieldRow[f2.FieldName];
                        }
                    }
                    else
                    {
                        //需要通过查询取得数据
                        string leftTable;
                        string leftField;
                        string rightTable;
                        string rightField;
                        string from = CSystem.Sys.Svr.Relations.GetRelationFrom(field.RelationPath + "." + field.TableRelation, 2, out leftTable, out rightTable, out leftField, out rightField);
                        DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select {5}.{1} from {2} where {0}.{3}='{4}'", leftTable, field.RFieldName, from, rightField, fieldRow[leftField], rightTable));
                        newRow[field.FieldName] = ds.Tables[0].Rows[0][field.RFieldName];
                    }
                }
            }
            return true;
        }
        private SortedList<string, SortedList<string, DataRow>> DataRowData;
        private DataRow GetFields(string tableName, string field, string value, out COMFields tableDefine)
        {
            return GetFields(tableName, field, value, out tableDefine, "");
        }
        private DataRow GetFields(string tableName, string field,string value,out COMFields tableDefine,string filter)
        {
            if (value == null)
            {
                tableDefine = null;
                return null;
            }
            tableDefine = CSystem.Sys.Svr.LDI.GetFields(tableName);
            if (DataRowData == null)
                DataRowData = new SortedList<string, SortedList<string, DataRow>>();
            SortedList<string, DataRow> data=null;
            if (DataRowData.ContainsKey(tableName))
                data = DataRowData[tableName];
            else
                data = new SortedList<string, DataRow>();
            if (data.ContainsKey(value))
                return data[value];

            DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("{0} where {1}.{2}='{3}' and {1}.Disable=0 {4}", tableDefine.QuerySQL, tableName, field, value, filter));
            if (ds.Tables[0].Rows.Count != 0)
            {
                data.Add(value, ds.Tables[0].Rows[0]);
                return ds.Tables[0].Rows[0];
            }
            else
                return null;
        }
        public override bool Check(Guid ID, SqlConnection conn, SqlTransaction sqlTran,DataRow mainRow)
        {
            string table;
            if (_WarehouseOperate < enuWarehouseOperate.Out)
                table = "D_WarehouseIn";
            else
                table = "D_WarehouseOut";
            DataSet dsWarehouse = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from {0} where ID='{1}'", table, ID), table, conn, sqlTran);
            if (dsWarehouse.Tables[0].Rows.Count != 1)
            {
                Msg.Information("单据不存在！");
                return false;
            }
            else
            {
                if ((int)dsWarehouse.Tables[0].Rows[0]["Status"] == 1)
                {
                    Msg.Error("该单据已核算不能反复核！");
                    return false;
                }
            }
            switch (_WarehouseOperate)
            {
                case enuWarehouseOperate.BuyIn:
                    DataSet dsWarehouseInBill = CSystem.Sys.Svr.cntMain.Select("Select * from D_WarehouseInBill where MainID='" + ID + "'", "D_WarehouseInBil", conn, sqlTran);
                    DataSet dsBuyOrderBill = CSystem.Sys.Svr.cntMain.Select("Select * from D_BuyOrderBill where MainID='" + mainRow["BuyOrderID"] + "'", "D_BuyOrderBill",conn,sqlTran);
                    //DataSet dsWarehouseInBill = DBConnection.Connection.Select("Select * from D_WarehouseInBill where MainID='" + WarehouseInID + "'", "D_WarehouseInBill");
                    int consignStatus = 0;
                    int finished = 0;
                    
                    foreach (DataRow dr in dsBuyOrderBill.Tables[0].Rows)
                    {
                        string sql = string.Format("SourceID='{0}'", dr["ID"]);
                        DataRow[] drs = dsWarehouseInBill.Tables[0].Select(sql);

                        if (drs.Length == 0)
                        {
                            if ((decimal)dr["FinishQuantity1"] < (decimal)dr["Quantity1"])
                            {
                                consignStatus = 1;
                                finished = 0;
                            }
                        }
                        else
                        {
                            bool yes=true;
                            if ((decimal)drs[0]["Quantity1"] + (decimal)dr["FinishQuantity1"] > (decimal)dr["Quantity1"]
                                || (decimal)drs[0]["Quantity2"] + (decimal)dr["FinishQuantity2"] > (decimal)dr["Quantity2"])
                            {
                                if (Msg.Question("入库的数量大于采购数量，是否继续？") == DialogResult.Yes)
                                    yes=true;
                                else
                                    yes=false;
                            }
                            if (yes)
                            {
                                dr["FinishQuantity1"] = (decimal)drs[0]["Quantity1"] + (decimal)dr["FinishQuantity1"];
                                dr["FinishQuantity2"] = (decimal)drs[0]["Quantity2"] + (decimal)dr["FinishQuantity2"];
                                if (((decimal)dr["FinishQuantity1"] == (decimal)dr["Quantity1"]
                                    || (decimal)dr["FinishQuantity2"] == (decimal)dr["Quantity2"]) && consignStatus != 1)
                                {
                                    consignStatus = 2;
                                    finished = 1;
                                }
                                else
                                {
                                    consignStatus = 1;
                                    finished = 0;
                                }
                            }
                        }
                    }

                    CSystem.Sys.Svr.cntMain.Update(dsBuyOrderBill.Tables[0], conn, sqlTran);
                    CSystem.Sys.Svr.cntMain.Excute(string.Format("Update D_BuyOrder set ConsignStatus ={0},Finished={1} where ID='{2}'", consignStatus, finished, mainRow["BuyOrderID"]), conn, sqlTran);
                    break;
                case enuWarehouseOperate.RepairIn:
                case enuWarehouseOperate.MoveIn:
                    DataSet dsWarehouseIn = CSystem.Sys.Svr.cntMain.Select("Select * from D_WarehouseIn where ID='" + ID + "'", "D_WarehouseIn", conn, sqlTran);
                    DataSet dsWarehouseOut = CSystem.Sys.Svr.cntMain.Select("Select * from D_WarehouseOut where MoveStatus=0 and ID='" + dsWarehouseIn.Tables[0].Rows[0]["WarehouseOutID"] + "'", "D_WarehouseOut", conn, sqlTran);
                    if (dsWarehouseOut.Tables[0].Rows.Count == 1)
                    {
                        dsWarehouseOut.Tables[0].Rows[0]["MoveStatus"] = 1;
                        CSystem.Sys.Svr.cntMain.Update(dsWarehouseOut.Tables[0], conn, sqlTran);
                    }
                    else
                    {
                        Msg.Information("移库的出库单不存在或已被核销！");
                        return false;
                    }
                    break;
                case enuWarehouseOperate.RepairOut:
                    DataSet dsWarehouseOutData = CSystem.Sys.Svr.cntMain.Select("Select * from D_WarehouseOut where ID='" + ID + "'", "D_WarehouseOut", conn, sqlTran);
                    DataRow drMainOut = dsWarehouseOutData.Tables[0].Rows[0];
                    COMFields fieldMain = CSystem.Sys.Svr.LDI.GetFields("D_WarehouseIn");
                    DataSet dsIn = CSystem.Sys.Svr.cntMain.Select(fieldMain.QuerySQL + " where 1=0", fieldMain.OrinalTableName, conn, sqlTran);
                    DataRow drMain = dsIn.Tables[0].NewRow();

                    Guid mainID = Guid.NewGuid();
                    drMain["ID"] = mainID;
                    drMain["WarehouseID"] = drMainOut["TargetWarehouseID"];
                    drMain["WarehouseOutID"] = ID;
                    drMain["Code"] = drMainOut["Code"];
                    drMain["BillDate"] = drMainOut["BillDate"];
                    drMain["Type"] = enuWarehouseIn.RepairIn;
                    drMain["CreatedBy"] = drMainOut["CreatedBy"];
                    drMain["CreateDate"] = drMainOut["CreateDate"];
                    drMain["CheckStatus"] = 1;
                    drMain["CheckedBy"] = CSystem.Sys.Svr.User;
                    drMain["CheckDate"] = CSystem.Sys.Svr.SystemTime;
                    dsIn.Tables[0].Rows.Add(drMain);

                    COMFields fields2 = CSystem.Sys.Svr.LDI.GetFields("D_WarehouseOutBill");

                    string sql2 = "select '' SendBillCode, A.*,C.Code TargetCode,C.Name TargetName,C.ID TargetID from (" +
                            fields2.QuerySQL + " where D_WarehouseOutBill.MainID='" + drMainOut["ID"] + "') A " +
                            "inner join p_material B  on a.MaterialID=B.ID inner join P_Material C  on B.WIPID=C.ID";

                    CSystem.Sys.Svr.cntMain.Select(sql2, "D_WarehouseInBill", dsIn, conn, sqlTran);
                    foreach (DataRow dr in dsIn.Tables["D_WarehouseInBill"].Rows)
                    {
                        dr["ID"] = Guid.NewGuid();
                        dr["MainID"] = mainID;
                        dr["PositionID"] = drMainOut["TargetPositionID"];
                        dr["MaterialID"] = dr["TargetID"];
                        dr["MachiningStandardID"] = DBNull.Value;

                        dr.AcceptChanges();
                        dr.SetAdded();
                    }
                    dsIn.Tables["D_WarehouseInBill"].Columns.Remove("TargetID");
                    dsIn.Tables["D_WarehouseInBill"].Columns.Remove("TargetCode");
                    dsIn.Tables["D_WarehouseInBill"].Columns.Remove("TargetName");
                    CSystem.Sys.Svr.cntMain.Update(dsIn.Tables["D_WarehouseIn"], conn, sqlTran);
                    CSystem.Sys.Svr.cntMain.Update(dsIn.Tables["D_WarehouseInBill"], conn, sqlTran);
                    break;
                case enuWarehouseOperate.ProduceOut:
                    //查询出库位是否有对应的半成品仓库，如有则生成半成品入库
                    DataSet dsOut = CSystem.Sys.Svr.cntMain.Select("Select * from D_WarehouseOutBill where MainID='" + ID + "'", "D_WarehouseOutBill", conn, sqlTran);
                    if (dsOut.Tables["D_WarehouseOutBill"].Rows.Count > 0)
                    {
                        DataSet ds = CSystem.Sys.Svr.cntMain.Select(CSystem.Sys.Svr.LDI.GetFields("P_Position").QuerySQLWithClause("P_Position.ID='" + dsOut.Tables["D_WarehouseOutBill"].Rows[0]["PositionID"] + "'"), "P_Position", conn, sqlTran);
                        if (ds.Tables[0].Rows[0]["WIPPositionID"] == DBNull.Value)
                            return true;
                        Guid WIPPositionID = (Guid)ds.Tables[0].Rows[0]["WIPPositionID"];
                        Guid WIPWarehouseID = (Guid)ds.Tables[0].Rows[0]["WIPWarehouseID"];
                        CSystem.Sys.Svr.cntMain.Select("Select * from D_WarehouseOut where ID='" + ID + "'", "D_WarehouseOut", dsOut, conn, sqlTran);
                        DataSet dsInNew = CSystem.Sys.Svr.cntMain.Select("Select * from D_WarehouseIn where 1=0", "D_WarehouseIn", conn, sqlTran);
                        CSystem.Sys.Svr.cntMain.Select("Select * from D_WarehouseInBill where 1=0", "D_WarehouseInBill", dsInNew, conn, sqlTran);
                        //设置表头
                        DataRow drNew = dsInNew.Tables["D_WarehouseIn"].NewRow();
                        DataRow drOld = dsOut.Tables["D_WarehouseOut"].Rows[0];
                        foreach (DataColumn dc in dsOut.Tables["D_WarehouseOut"].Columns)
                        {
                            switch (dc.ColumnName)
                            {
                                case "WarehouseID":
                                    drNew[dc.ColumnName] = WIPWarehouseID;
                                    break;
                                case "PositionID":
                                    drNew[dc.ColumnName] = WIPPositionID;
                                    break;
                                default:
                                    if (dsInNew.Tables["D_WarehouseIn"].Columns.Contains(dc.ColumnName))
                                    {
                                        drNew[dc.ColumnName] = drOld[dc.ColumnName];
                                    }
                                    break;
                            }
                        }
                        drNew["WarehouseOutID"] = drOld["ID"];
                        drNew["Type"] = enuWarehouseIn.In;
                        drNew["CheckStatus"] = 1;
                        drNew["CheckedBy"] = CSystem.Sys.Svr.User;
                        drNew["CheckDate"] = CSystem.Sys.Svr.SystemTime;
                        dsInNew.Tables["D_WarehouseIn"].Rows.Add(drNew);
                        //设置表体
                        foreach (DataRow drBillOld in dsOut.Tables["D_WarehouseOutBill"].Rows)
                        {
                            DataRow drBillNew = dsInNew.Tables["D_WarehouseInBill"].NewRow();
                            foreach (DataColumn dc in dsOut.Tables["D_WarehouseOutBill"].Columns)
                            {
                                switch (dc.ColumnName)
                                {
                                    //case "WarehouseID":
                                    //    drNew[dc.ColumnName] = WIPWarehouseID;
                                    //    break;
                                    case "PositionID":
                                        drBillNew[dc.ColumnName] = WIPPositionID;
                                        break;
                                    case "MaterialID":
                                        ds = CSystem.Sys.Svr.cntMain.Select("Select WIPID from P_Material where ID='" + drBillOld["MaterialID"] + "'", "P_Material", conn, sqlTran);
                                        if (ds.Tables["P_Material"].Rows[0]["WIPID"] == DBNull.Value)
                                        {
                                            Msg.Error("存在没有设置对应的半成品的物料！");
                                            return false;
                                        }
                                        else
                                        {
                                            drBillNew[dc.ColumnName] = ds.Tables["P_Material"].Rows[0]["WIPID"];
                                        }
                                        break;
                                    case "IngredientID":
                                        break;
                                    default:
                                        if (dsInNew.Tables["D_WarehouseInBill"].Columns.Contains(dc.ColumnName))
                                        {
                                            drBillNew[dc.ColumnName] = drBillOld[dc.ColumnName];
                                        }
                                        break;
                                }
                            }
                            dsInNew.Tables["D_WarehouseInBill"].Rows.Add(drBillNew);
                        }
                        CSystem.Sys.Svr.cntMain.Update(dsInNew.Tables["D_WarehouseIn"], conn, sqlTran);
                        CSystem.Sys.Svr.cntMain.Update(dsInNew.Tables["D_WarehouseInBill"], conn, sqlTran);
                    }
                    break;
            }
            return base.Check(ID, conn, sqlTran,mainRow);
        }
        public override bool UnCheck(Guid ID, SqlConnection conn, SqlTransaction sqlTran, DataRow mainRow)
        {
            string table;
            if (_WarehouseOperate < enuWarehouseOperate.Out)
                table = "D_WarehouseIn";
            else
                table = "D_WarehouseOut";
            DataSet dsWarehouse = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from {0} where ID='{1}'", table, ID), table, conn, sqlTran);
            if (dsWarehouse.Tables[0].Rows.Count != 1)
            {
                Msg.Information("单据不存在！");
                return false;
            }
            else
            {
                if ((int)dsWarehouse.Tables[0].Rows[0]["Status"] == 1)
                {
                    Msg.Error("该单据已核算不能反复核！");
                    return false;
                }
                if (dsWarehouse.Tables[0].Rows[0]["WorkOrderID"] != DBNull.Value)
                {
                    Msg.Error("该单据为工票生成,不能反复核！");
                    return false;
                }
            }
            switch (_WarehouseOperate)
            {
                case enuWarehouseOperate.BuyIn:
                    DataSet dsWarehouseInBill = CSystem.Sys.Svr.cntMain.Select("Select * from D_WarehouseInBill where MainID='" + ID + "'", "D_WarehouseInBil", conn, sqlTran);
                    DataSet dsBuyOrderBill = CSystem.Sys.Svr.cntMain.Select("Select * from D_BuyOrderBill where MainID='" + mainRow["BuyOrderID"] + "'", "D_BuyOrderBill", conn, sqlTran);
                    //DataSet dsWarehouseInBill = DBConnection.Connection.Select("Select * from D_WarehouseInBill where MainID='" + WarehouseInID + "'", "D_WarehouseInBill");
                    int consignStatus = 0;
                    int finished = 0;
                    //获取状态
                    DataSet dsBuyOrder = CSystem.Sys.Svr.cntMain.Select("Select * from D_BuyOrder where ID='" + mainRow["BuyOrderID"] + "'", "D_BuyOrder", conn, sqlTran);
                    finished = (int)dsBuyOrder.Tables[0].Rows[0]["Finished"];
                    consignStatus = (int)dsBuyOrder.Tables[0].Rows[0]["ConsignStatus"];
                    bool BuyBillNotInWarehouseIn = false;
                    foreach (DataRow dr in dsBuyOrderBill.Tables[0].Rows)
                    {
                        string sql = string.Format("SourceID='{0}'", dr["ID"]);
                        DataRow[] drs = dsWarehouseInBill.Tables[0].Select(sql);

                        if (drs.Length == 0)
                        {
                            BuyBillNotInWarehouseIn = true;
                            if (consignStatus == 0 && ((decimal)dr["FinishQuantity1"] > 0||(decimal)dr["FinishQuantity2"] > 0))
                                consignStatus = 1;
                        }
                        else
                        {
                            bool yes = true;
                            if ((decimal)drs[0]["Quantity1"] > (decimal)dr["FinishQuantity1"]
                                || (decimal)drs[0]["Quantity2"] > (decimal)dr["FinishQuantity2"])
                            {
                                if (Msg.Question("入库单的数量大于采购数量，是否继续？") == DialogResult.Yes)
                                    yes = true;
                                else
                                    yes = false;
                            }
                            if (yes)
                            {
                                dr["FinishQuantity1"] = (decimal)dr["FinishQuantity1"] - (decimal)drs[0]["Quantity1"];
                                dr["FinishQuantity2"] = (decimal)dr["FinishQuantity2"] - (decimal)drs[0]["Quantity2"];
                                if (((decimal)dr["FinishQuantity1"] == 0
                                    || (decimal)dr["FinishQuantity2"] == 0))
                                {
                                    finished = 0;
                                    if (BuyBillNotInWarehouseIn)
                                    {
                                        consignStatus = 1;
                                    }
                                    else
                                    {
                                        consignStatus = 0;
                                    }
                                }
                                else
                                {
                                    finished = 0;
                                    consignStatus = 1;
                                }
                            }
                        }
                    }

                    CSystem.Sys.Svr.cntMain.Update(dsBuyOrderBill.Tables[0], conn, sqlTran);
                    CSystem.Sys.Svr.cntMain.Excute(string.Format("Update D_BuyOrder set ConsignStatus ={0},Finished={1} where ID='{2}'", consignStatus, finished, mainRow["BuyOrderID"]), conn, sqlTran);
                    break;
                case enuWarehouseOperate.RepairIn:
                case enuWarehouseOperate.MoveIn:
                    DataSet dsWarehouseOut = CSystem.Sys.Svr.cntMain.Select("Select * from D_WarehouseOut where MoveStatus=1 and ID='" + dsWarehouse.Tables[0].Rows[0]["WarehouseOutID"] + "'", "D_WarehouseOut", conn, sqlTran);
                    if (dsWarehouseOut.Tables[0].Rows.Count == 1)
                    {
                        dsWarehouseOut.Tables[0].Rows[0]["MoveStatus"] = 0;
                        CSystem.Sys.Svr.cntMain.Update(dsWarehouseOut.Tables[0], conn, sqlTran);
                    }
                    else
                    {
                        if (Msg.Question("移库的出库单不存在或没有被核销，是否继续？") == DialogResult.Yes)
                            return true;
                        else
                            return false;
                    }
                    break;
                case enuWarehouseOperate.RepairOut:
                case enuWarehouseOperate.ProduceOut: 
                    CSystem.Sys.Svr.cntMain.Excute(string.Format("Delete from D_WarehouseInBill where MainID in (Select ID from D_WarehouseIn where WarehouseOutID='{0}');Delete from D_WarehouseIn where WarehouseOutID='{0}'", ID), conn, sqlTran);
                    break;
            }
            return base.UnCheck(ID, conn, sqlTran,mainRow);
        }
        public override string BeforeSaving(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            string s=null;
            foreach (DataRow dr in ds.Tables[this._DetailForm.DetailTableDefine[0].OrinalTableName].Rows)
            {
                if (dr.RowState!= DataRowState.Deleted && (int)dr["NeedLength"] == 1 && (dr["Length"] == DBNull.Value || (decimal)dr["Length"] == 0))
                    s = s + (string)dr["MaterialName"] + " 的长度没有输入！";
            }
            //检测工票的单据日期是否,低于当前会计期
            DataSet data = CSystem.Sys.Svr.cntMain.Select(@"Select Value from S_BaseInfo where ID='系统\当前会计期'");
            if (ds.Tables[0].Rows.Count == 1)
            {
                DateTime currPeriod = DateTime.Parse((string)data.Tables[0].Rows[0][0]);
                if ((DateTime)ds.Tables[_DetailForm.MainTable].Rows[0]["BillDate"] < currPeriod)
                {
                    s = s + "不能输入以前会计期的单据!";
                }
            }
            return s;
        }
        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            try
            {
                if (right.Paramters==null)
                    return null;
                string[] s = right.Paramters.Split(new char[] { ',' });
                bool isOther=false;
                enuWarehouseOperate opt;
                if (s.Length > 0)
                    opt = (enuWarehouseOperate)int.Parse(s[0]);
                else
                    return null;
                if (s.Length>1)
                    isOther=int.Parse(s[1])==1;
                string filter;
                if (isOther)
                    filter = " and P_Warehouse.WarehouseType=4";
                else
                    filter = " and P_Warehouse.WarehouseType<>4";

                if (opt < enuWarehouseOperate.Out)
                    this.MainTableName = "D_WarehouseIn";
                else
                    this.MainTableName = "D_WarehouseOut";

                frmBrowser frmWarehouse = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("P_Warehouse"), null, "P_Warehouse.Disable=0"+filter, enuShowStatus.None);
                DataRow dr = frmWarehouse.ShowSelect(null, null, null);
                if (dr != null)
                {
                    _IsFittingPart = (int)dr["IsFittingPart"] == 1;
                    int InOutType;
                    if (opt < enuWarehouseOperate.Out)
                        InOutType = (int)opt;
                    else
                        InOutType =(int)opt - (int)enuWarehouseOperate.Out;
                    SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                    SortedList<string, object> Value = new SortedList<string, object>();
                    Value.Add("WarehouseID", dr["ID"]);
                    Value.Add("WarehouseName", dr["Name"]);
                    Value.Add("WarehouseType", dr["WarehouseType"]);
                    Value.Add("IsZeroStock", dr["IsZeroStock"]);
                    Value.Add("Type", InOutType);
                    Value.Add("BillDate", CSystem.Sys.Svr.SystemTime.Date);
                    defaultValue.Add(this.MainTableName, Value);
                    
                    COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                    List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);
                    switch (opt)
                    {
                    case enuWarehouseOperate.In:
                        mainTableDefine.Property.Title = string.Format("普通入库单[{0}]", (string)dr["Name"]);
                        mainTableDefine["BuyOrderCode"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["WarehouseOutCode"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["WarehouseOutWorkCode"].Visible = COMField.Enum_Visible.NotVisible;
                        break;
                    case enuWarehouseOperate.BuyIn:
                        mainTableDefine.Property.Title = string.Format("采购入库单[{0}]", (string)dr["Name"]);
                        mainTableDefine["WarehouseOutCode"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["WarehouseOutWorkCode"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["WorkCode"].FieldTitle = "收料单号";
                        detailTableDefines[0]["SendBillCode"].Mandatory = true;
                        detailTableDefines[0]["SendBillCode"].Enable = true;
                        detailTableDefines[0]["SendBillCode"].Visible = COMField.Enum_Visible.VisibleAll;
                        detailTableDefines[0]["MaterialCode"].Enable = false;
                        detailTableDefines[0]["MaterialName"].Enable = false;
                        detailTableDefines[0]["IngredientName"].Enable = false;
                        detailTableDefines[0]["Length"].Enable = false;
                        detailTableDefines[0]["LineNumber"].Visible = COMField.Enum_Visible.VisibleAll;
                        detailTableDefines[0]["LineNumber"].Enable = false;
                        break;
                    case enuWarehouseOperate.MoveIn:
                        mainTableDefine.Property.Title = string.Format("调拨入库单[{0}]", (string)dr["Name"]);
                        mainTableDefine["BuyOrderCode"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["WorkCode"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["WorkCode"].Mandatory = false;
                        break;
                    case enuWarehouseOperate.RepairIn:
                        mainTableDefine.Property.Title = string.Format("返修入库单[{0}]", (string)dr["Name"]);
                        mainTableDefine["BuyOrderCode"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["WorkCode"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["WorkCode"].Mandatory = false;
                        //OptType = clsWarehouseDetailForm.enuWarehouseOperate.RepairIn;
                        break;
                    case enuWarehouseOperate.MoveOut:
                        mainTableDefine.Property.Title = string.Format("调拨出库单[{0}]", (string)dr["Name"]);
                        mainTableDefine["WorkCode"].FieldTitle = "调拨单号";
                        mainTableDefine["TargetDepartID"].Visible= COMField.Enum_Visible.NotVisible;
                        mainTableDefine["TargetDepartID"].Mandatory= false;
                        mainTableDefine["TargetDepartName"].Visible= COMField.Enum_Visible.NotVisible;
                        mainTableDefine["TargetDepartName"].Mandatory= false;
                        mainTableDefine["TargetUserID"].Visible= COMField.Enum_Visible.NotVisible;
                        mainTableDefine["TargetUserID"].Mandatory= false;
                        mainTableDefine["TargetUserName"].Visible = COMField.Enum_Visible.NotVisible;
                        mainTableDefine["TargetUserName"].Mandatory = false;

                        mainTableDefine["TargetWarehouseName"].Visible = COMField.Enum_Visible.VisibleAll;
                        mainTableDefine["TargetPositionName"].Visible = COMField.Enum_Visible.VisibleAll;
                        mainTableDefine["TargetWarehouseName"].Mandatory = true;
                        mainTableDefine["TargetPositionName"].Mandatory = true;
                        mainTableDefine["TargetWarehouseName"].Enable = true;
                        mainTableDefine["TargetPositionName"].Enable = true;
                        break;
                    case enuWarehouseOperate.CheckOut:
                        mainTableDefine.Property.Title = string.Format("盘点出库单[{0}]", (string)dr["Name"]);
                        goto default;
                    case enuWarehouseOperate.ScrapOut:
                        mainTableDefine.Property.Title = string.Format("废品出库单[{0}]", (string)dr["Name"]);
                        goto default;
                    case enuWarehouseOperate.RepairOut:
                        mainTableDefine.Property.Title = string.Format("返修出库单[{0}]", (string)dr["Name"]);
                        mainTableDefine["WorkCode"].FieldTitle = "返修单号";
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
                    case enuWarehouseOperate.Out:
                        mainTableDefine.Property.Title = string.Format("普通出库单[{0}]", (string)dr["Name"]);
                        goto default;
                    case enuWarehouseOperate.ProduceOut:
                        mainTableDefine.Property.Title = string.Format("生产领料单[{0}]", (string)dr["Name"]);
                        goto default;
                    case enuWarehouseOperate.ReturnOut:
                        mainTableDefine.Property.Title = string.Format("退货单[{0}]", (string)dr["Name"]);
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

                    //增加对辅料仓库及零余额仓库的处理
                    if ((int)dr["WarehouseType"] == 4)
                    {
                        if (opt <enuWarehouseOperate.Out && opt != enuWarehouseOperate.MoveIn)
                        {
                            mainTableDefine["SupplierName"].Visible = COMField.Enum_Visible.VisibleAll;
                            mainTableDefine["SupplierName"].Enable = true;
                            mainTableDefine["SupplierName"].Mandatory = true;
                        }
                        detailTableDefines[0]["Money"].Visible = COMField.Enum_Visible.VisibleAll;
                        detailTableDefines[0]["Money"].Mandatory = true;
                        detailTableDefines[0]["Price"].Visible = COMField.Enum_Visible.VisibleAll;
                        detailTableDefines[0]["Price"].Mandatory = true;
                        detailTableDefines[0]["Price"].Enable = true;
                        if (opt< enuWarehouseOperate.Out)
                            mainTableDefine["Amount"].Visible = COMField.Enum_Visible.VisibleAll;
                        else
                        {
                            mainTableDefine["MachineID"].Mandatory = true;
                            mainTableDefine["MachineName"].Mandatory = true;
                            mainTableDefine["MachineName"].Enable = true;
                            mainTableDefine["MachineName"].Visible = COMField.Enum_Visible.VisibleAll;

                            mainTableDefine["TargetUserID"].Visible = COMField.Enum_Visible.NotVisible;
                            mainTableDefine["TargetUserID"].Mandatory = true;
                            mainTableDefine["TargetUserName"].Visible = COMField.Enum_Visible.VisibleAll;
                            mainTableDefine["TargetUserName"].Mandatory = true;

                            //出库所有字段都不改
                            foreach (COMField field in detailTableDefines[0].Fields)
                                field.Enable = false;
                            detailTableDefines[0]["MaterialCode"].Enable = true;
                            detailTableDefines[0]["MaterialName"].Enable = true;
                            detailTableDefines[0]["Quantity1"].Enable = true;
                            detailTableDefines[0]["Quantity2"].Enable = true;

                            detailTableDefines[0]["MaterialCheckObjectName"].Visible = COMField.Enum_Visible.VisibleAll;
                            detailTableDefines[0]["MaterialClassName"].Visible = COMField.Enum_Visible.VisibleAll;
                        }
                    }
                    else if ((int)dr["WarehouseType"] == 1 && (opt== enuWarehouseOperate.Out || opt==enuWarehouseOperate.In))
                    {
                        mainTableDefine["WorkOrderCode"].Visible = COMField.Enum_Visible.VisibleAll;
                    }

                    if ((int)dr["CheckIngredient"] == 0)
                    {
                        detailTableDefines[0]["IngredientName"].Visible = COMField.Enum_Visible.NotVisible;
                        detailTableDefines[0]["IngredientName"].Mandatory = false;
                        detailTableDefines[0]["IngredientID"].Mandatory = false;
                    }
                    if ((int)dr["CheckMachiningStandard"] == 0)
                    {
                        detailTableDefines[0]["MachiningStandardName"].Visible = COMField.Enum_Visible.NotVisible;
                        detailTableDefines[0]["MachiningStandardCode"].Visible = COMField.Enum_Visible.NotVisible;
                        detailTableDefines[0]["Text1"].Visible = COMField.Enum_Visible.NotVisible;
                        detailTableDefines[0]["MachiningStandardCode"].Mandatory = false;
                        detailTableDefines[0]["MachiningStandardID"].Mandatory = false;
                    }
                    if ((int)dr["CheckDesignCode"] == 0)
                    {
                        detailTableDefines[0]["DesignCodeName"].Visible = COMField.Enum_Visible.NotVisible;
                        detailTableDefines[0]["DesignCodeName"].Mandatory = false;
                        detailTableDefines[0]["DesignCodeID"].Mandatory = false;
                    }
                    if ((int)dr["CheckProductStandard"] == 0)
                    {
                        detailTableDefines[0]["ProductStandardName"].Visible = COMField.Enum_Visible.NotVisible;
                        detailTableDefines[0]["ProductStandardName"].Mandatory = false;
                        detailTableDefines[0]["ProductStandardID"].Mandatory = false;
                    }

                    this._MainCOMFields = mainTableDefine;
                    this._DetailCOMFields = detailTableDefines;
                    this._WarehouseOperate=opt;
                    frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
                    frm.Where= string.Format("{2}.Type={0} and {2}.WarehouseID='{1}'", (int)InOutType, dr["ID"],this.MainTableName);
                    this._WarehouseID= (Guid)dr["ID"];
                    this._WarehouseCode=(string)dr["Code"];
                    this._WarehouseName=(string)dr["Name"];
                    this._WarehouseType =(int)dr["WarehouseType"];
                    frm.Path = mainTableDefine.OrinalTableName + "\\" + dr["ID"] + "\\" + InOutType;
                    frm.Tag = string.Format("isOther={0} path={1}", isOther, frm.Path);
                    frm.DefaultValue=defaultValue;
                   
                    return frm;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
