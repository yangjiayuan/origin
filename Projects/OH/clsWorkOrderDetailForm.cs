using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using System.Data;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinEditors;
using Base;
using UI;
using System.Windows.Forms;
using Infragistics.Win;

namespace OH
{
    public class clsWorkOrderDetailForm : ToolDetailForm
    {

        public enum Enum_WarehouseMovetype : int { WarehouseIn = 1, WarehouseOut = 2};
        private Guid MachineID;
        private int ClassIndex=0; //班组
        private CCreateGrid WorkOrderOperatorGrid;
        
        public override bool AutoCode
        {
            get
            {
                return true;
            }
        }
        public override string GetPrefix()
        {
            return "WO";
        }
        //public override bool InsertLines(DataTable dataTable)
        //{
        //    if (dataTable.TableName == "D_WorkOrderDetail")
        //    {
        //        Guid ProcessID = (Guid)base._DetailForm.MainDataSet.Tables["D_WorkOrder"].Rows[0]["ProcessID"];
        //        foreach (DataRow DR in dataTable.Rows)
        //            DR["ProcessID"] = ProcessID;
        //    }
        //    return base.InsertLines(dataTable);
        //}

        public override void NewControl(COMField f, System.Windows.Forms.Control ctl)
        {
            if (f.FieldName == "Class")
            {
                ComboBox Class = ctl as ComboBox;
                Class.SelectedIndexChanged +=new EventHandler(Class_SelectedIndexChanged);
                IntItem SelectedItem = (IntItem)Class.SelectedItem;
                ClassIndex = SelectedItem.Int;
            }
            if (f.FieldName == "OperatorName")
            {
            }
        }

        private void Class_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox Class = sender as ComboBox;
            IntItem SelectedItem = (IntItem)Class.SelectedItem;
            ClassIndex = SelectedItem.Int;
            //MessageBox.Show(SelectedItem.Int + SelectedItem.Name);
        }
        public override void SetCreateDetail(CCreateDetailForm createDetailForm)
        {
            base.SetCreateDetail(createDetailForm);
            _CreateDetailForm.AfterSelectForm += new AfterSelectFormEventHandler(UTE_ValueChanged);
            _CreateDetailForm.BeforeSelectForm += new BeforeSelectFormEventHandler(_CreateDetailForm_BeforeSelectForm);
        }

        void _CreateDetailForm_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            if (e.Field.FieldName == "OperatorName")
            {
                e.Where = "P_Operator.IsChecker=1";
            }
            else if (e.Field.FieldName == "MachineName")
            {
                if (_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["DepartmentID"] != DBNull.Value)
                    e.Where = "P_Machine.DepartmentID='" + _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["DepartmentID"] + "'";
            }
        }
        void UTE_ValueChanged(object sender, AfterSelectFormEventArgs e)
        {
            UltraTextEditor UTE = sender as UltraTextEditor;
 
            if (UTE.Value  == null)
                return;

            switch (e.Field.FieldName)
            {
                case "MachineName":
                    MachineID = (Guid)e.Row["ID"];
                    break;
                case "":
                    break;
            }     
             
        }
        public override void SetCreateGrid(CCreateGrid createGrid)
        {
            base.SetCreateGrid(createGrid);
            if (createGrid.Fields.OrinalTableName.ToUpper()  == String.Format("D_WorkOrderOperator").ToUpper())
                WorkOrderOperatorGrid = createGrid;
            if (createGrid.Fields.OrinalTableName.ToUpper()  == String.Format("D_WorkOrderDetail").ToUpper())
                createGrid.Grid.AfterRowInsert += new RowEventHandler(GridDetail_AfterRowInsert);
            if (createGrid.Fields.OrinalTableName.ToUpper() == String.Format("D_WorkOrderProduct").ToUpper())
                createGrid.Grid.AfterRowInsert += new RowEventHandler(GridProduct_AfterRowInsert);
            createGrid.AfterSelectForm += new AfterSelectFormEventHandler(_CreateGrid_AfterSelectForm);
            createGrid.BeforeSelectForm += new BeforeSelectFormEventHandler(_CreateGrid_BeforeSelectForm);
            //设置标识的宽度
            if (createGrid.Grid.DisplayLayout.Bands[0].Columns.Exists("Tag"))
                createGrid.Grid.DisplayLayout.Bands[0].Columns["Tag"].Width = 30;
        }
        void GridProduct_AfterRowInsert(object sender, RowEventArgs e)
        {
            //int i = char.ConvertToUtf32("A", 0);
            e.Row.Cells["Tag"].Value =(e.Row.Index+1).ToString("000"); 
            //char.ConvertFromUtf32(i + e.Row.Index);
            //默认长度为５
            e.Row.Cells["Length"].Value = 5;
        }
        void GridDetail_AfterRowInsert(object sender, RowEventArgs e)
        {
            
            foreach (UltraGridRow rowPrd in _GridMap["D_WorkOrderProduct"].Rows)
            {
                try
                {
                    int totalQty = (int)rowPrd.Cells["Quantity"].Value;
                    int detailQty = 0;
                    foreach (UltraGridRow rowDetail in _GridMap["D_WorkOrderDetail"].Rows)
                    {
                        try
                        {
                            if ( rowDetail.Cells["Tag"].Value!=DBNull.Value && (string)rowDetail.Cells["Tag"].Value == (string)rowPrd.Cells["Tag"].Value)
                            {
                                int q1 = Convert.IsDBNull(rowDetail.Cells["Quantity1stClass"].Value) ? 0 : (int)rowDetail.Cells["Quantity1stClass"].Value;
                                int q2 = Convert.IsDBNull(rowDetail.Cells["Quantity2ndClass"].Value) ? 0 : (int)rowDetail.Cells["Quantity2ndClass"].Value;
                                int q3 = Convert.IsDBNull(rowDetail.Cells["Quantity3rdClass"].Value) ? 0 : (int)rowDetail.Cells["Quantity3rdClass"].Value;
                                int q4 = Convert.IsDBNull(rowDetail.Cells["QuantityMaterialWaster1"].Value) ? 0 : (int)rowDetail.Cells["QuantityMaterialWaster1"].Value;
                                int q5 = Convert.IsDBNull(rowDetail.Cells["QuantityMaterialWaster2"].Value) ? 0 : (int)rowDetail.Cells["QuantityMaterialWaster2"].Value;
                                int q6 = Convert.IsDBNull(rowDetail.Cells["QuantityMachineWaster1"].Value) ? 0 : (int)rowDetail.Cells["QuantityMachineWaster1"].Value;
                                int q7 = Convert.IsDBNull(rowDetail.Cells["QuantityMachineWaster2"].Value) ? 0 : (int)rowDetail.Cells["QuantityMachineWaster2"].Value;
                                detailQty = detailQty + q1 + q2 + q3 + q4 + q5 + q6 + q7;
                            }
                        }
                        catch { }
                    }
                    if (totalQty > detailQty)
                    {
                        e.Row.Cells["Tag"].Value = rowPrd.Cells["Tag"].Value;
                        e.Row.Cells["Length"].Value = rowPrd.Cells["Length"].Value;
                        e.Row.Cells["MachineStandardID"].Value = rowPrd.Cells["MachineStandardID"].Value;
                        e.Row.Cells["MachineStandardCode"].Value = rowPrd.Cells["MachineStandardCode"].Value;
                        e.Row.Cells["Quantity1stClass"].Value = totalQty - detailQty;
                        return;
                    }
                }
                catch { }
            }
        }

        void _CreateGrid_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            CCreateGrid createdGrid = (CCreateGrid)sender;
            string[] s = e.Field.ValueType.Split(':');
            switch (s[1])
            {
                case "P_Material":
                    e.Where = "P_Material.MaterialType = 1";
                    break;
                case "P_MachiningStandard":
                    if (createdGrid.Fields.OrinalTableName == "D_WorkOrderDetail")
                    {
                        string tag =createdGrid.Grid.ActiveRow.Cells["Tag"].Value as string;
                        for (int i = 0; i < _ListCreateGrid.Count; i++)
                        {
                            if (_ListCreateGrid[i].Fields.OrinalTableName == "D_WorkOrderProduct")
                            {
                                foreach (UltraGridRow row in _ListCreateGrid[i].Grid.Rows)
                                {
                                    if ((string)row.Cells["Tag"].Value == tag)
                                    {
                                        e.Where = "P_MachiningStandard.MaterialID='" + row.Cells["MaterialID"].Value + "'";
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else if (createdGrid.Fields.OrinalTableName == "D_WorkOrderProduct")
                    {
                        e.Where = "P_MachiningStandard.MaterialID='" + createdGrid.Grid.ActiveRow.Cells["MaterialID"].Value + "'";
                    }
                    break;
                case "P_Craft":
                    if (this._DetailForm.MainDataSet.Tables[this._DetailForm.MainTable].Rows[0]["ProcessID"] != DBNull.Value)
                       e.Where = "P_Craft.ProcessID='" + (Guid)this._DetailForm.MainDataSet.Tables[this._DetailForm.MainTable].Rows[0]["ProcessID"] + "'";
                   if (this._DetailForm.MainDataSet.Tables[this._DetailForm.MainTable].Rows[0]["MachineID"] != DBNull.Value)
                       e.Where = e.Where + " and (P_Craft.MachineID is null or P_Craft.MachineID='" + (Guid)this._DetailForm.MainDataSet.Tables[this._DetailForm.MainTable].Rows[0]["MachineID"] + "')";
                    break;
                case "P_Addition":
                    UltraGridRow gridRow=createdGrid.Grid.ActiveRow;
                    if (gridRow.Cells["CraftID"].Value == DBNull.Value || gridRow.Cells["MaterialID"].Value == DBNull.Value || gridRow.Cells["StandardLevelID"].Value == DBNull.Value)
                        e.Cancel = true;
                    e.Where = string.Format("P_Addition.ID in (Select AdditionID from P_CraftDetail where MainID='{0}' and MaterialID='{1}' and StandardLevelID='{2}')",gridRow.Cells["CraftID"].Value,gridRow.Cells["MaterialID"].Value,gridRow.Cells["StandardLevelID"].Value);
                    break;
                case "P_Operator":
                    //if (this._DetailForm.MainDataSet.Tables[this._DetailForm.MainTable].Rows[0]["MachineID"] != DBNull.Value)
                    //    e.Where = string.Format("P_Operator.ID in (select distinct B.OperatorID from D_WorkOrder A,D_WorkOrderOperator B where A.ID=B.MainID and A.MachineID='{0}')",(Guid)this._DetailForm.MainDataSet.Tables[this._DetailForm.MainTable].Rows[0]["MachineID"]);
                    e.Where = "P_Operator.IsChecker=0 and P_Operator.Disable=0";
                    break;
            }
        }
        private void setAdditionEditStatus(UltraGridRow row)
        {
            Guid craftID;
            Guid materialID;
            Guid standardLevelID;
            if (row.Cells["CraftID"].Value == DBNull.Value)
                return;
            else
                craftID = (Guid)row.Cells["CraftID"].Value;
            if (row.Cells["MaterialID"].Value == DBNull.Value)
                return;
            else
                materialID = (Guid)row.Cells["MaterialID"].Value;
            if (row.Cells["StandardLevelID"].Value == DBNull.Value)
                return;
            else
                standardLevelID = (Guid)row.Cells["StandardLevelID"].Value;
            string sql = string.Format("Select count(*) from P_CraftDetail where MainID='{0}' and MaterialID='{1}' and StandardLevelID='{2}' and not AdditionID is null", craftID, materialID, standardLevelID);
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(sql);
            if ((int)ds.Tables[0].Rows[0][0] > 0)
                row.Cells["AdditionName"].Activation = Activation.AllowEdit;
            else
                row.Cells["AdditionName"].Activation = Activation.NoEdit;
        }

        void _CreateGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {
            CCreateGrid createGrid = (CCreateGrid)sender;
            if (e.Field.FieldName == "MaterialCode" || e.Field.FieldName == "MaterialName")
            {
                if ((int)e.Row["NeedLength"] == 1)
                    e.GridRow.Cells["Length"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                else
                {
                    e.GridRow.Cells["Length"].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
                    e.GridRow.Cells["Length"].Value = 0;
                }
            }
            else if (createGrid.Fields.OrinalTableName == "D_WorkOrderProduct" && e.Field.ValueType.StartsWith("Dict") && e.Field.ValueType != "Dict:P_Addition")
            {
                setAdditionEditStatus(e.GridRow);
            }
            else if (e.Field.FieldName == "OperatorNO" || e.Field.FieldName == "OperatorName")
            {
                e.GridRow.Cells["OperatorPropertyID"].Value = e.Row["OperatorPropertyID"];
                e.GridRow.Cells["OperatorPropertyName"].Value = e.Row["OperatorPropertyName"];
                e.GridRow.Cells["Ratio"].Value = e.Row["Ratio"];
                e.GridRow.Cells["StationID"].Value = e.Row["StationID"];
                e.GridRow.Cells["StationName"].Value = e.Row["StationName"];
                e.GridRow.Cells["Rate"].Value = e.Row["WageQuotiety"];
                e.GridRow.Cells["Adjust"].Value = 0;
                e.GridRow.Cells["Amount"].Value = 0;
            }
            else if (e.Field.FieldName == "OperatorPropertyName")
            {
                e.GridRow.Cells["Ratio"].Value = e.Row["Ratio"];
            }
            else if (e.Field.FieldName == "StationName")
            {
                e.GridRow.Cells["Rate"].Value = e.Row["WageQuotiety"];
            }
        }

        public override void NewGrid(COMFields fields, UltraGrid grid)
        {
            grid.AfterCellUpdate += new CellEventHandler(grid_AfterCellUpdate);
            if (fields.OrinalTableName=="D_WorkOrderDetail")
                grid.AfterRowInsert += new RowEventHandler(grid_AfterRowInsert);
        }

        void grid_AfterRowInsert(object sender, RowEventArgs e)
        {
            //e.Row.Cells["ProcessID"].Value = this._DetailForm.MainDataSet.Tables["D_WorkOrder"].Rows[0]["ProcessID"];
            //e.Row.Cells["ProcessName"].Value = this._DetailForm.MainDataSet.Tables["D_WorkOrder"].Rows[0]["ProcessName"];
        }

        void grid_AfterCellUpdate(object sender, CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "Rate":
                case "Ratio":
                    this.CalculateWage();
                    break;
                case "Adjust":
                    ////计算总金额
                    //UltraGrid grid = (UltraGrid)sender;
                    //decimal total = 0;
                    //foreach (UltraGridRow row in grid.Rows)
                    //    total += (decimal)row.Cells["Money"].Value;
                    //_ControlMap["Money"].Text = total.ToString();
                    ////计算含税金额
                    //e.Cell.Row.Cells["Amount"].Value = Math.Round((1 + ((UltraCurrencyEditor)this._ControlMap["TaxRate"]).Value / 100) * (decimal)e.Cell.Value, 2);
                    ////计算单价
                    //decimal qty2 = 0;
                    //if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 1)
                    //    qty2 = (decimal)e.Cell.Row.Cells["Quantity1"].Value;
                    //else if ((int)e.Cell.Row.Cells["AmountCheckMethod"].Value == 2)
                    //    qty2 = (decimal)e.Cell.Row.Cells["Quantity2"].Value;
                    //else
                    //    break;
                    //if (qty2 != 0)
                    //    e.Cell.Row.Cells["Price"].Value = Math.Round((decimal)e.Cell.Value / qty2, 4);
                    break;
                case "Amount":
                    //UltraGrid grid2 = (UltraGrid)sender;
                    //decimal total2 = 0;
                    //foreach (UltraGridRow row in grid2.Rows)
                    //    total2 += (decimal)row.Cells["Amount"].Value;
                    //_ControlMap["Amount"].Text = total2.ToString();
                    break;
                default:
                    if (e.Cell.Column.DataType == typeof(int) && e.Cell.Value == DBNull.Value)
                        e.Cell.Value = (int)0;
                    else if (e.Cell.Column.DataType == typeof(int) && e.Cell.Column.DataType == typeof(decimal))
                        e.Cell.Value = (decimal)0;
                    break;
            }

        }

        public override DataSet InsertRowsInGrid(List<COMFields> detailTableDefine)
        {
            
            //for (int i = 0; i < detailTableDefine.Count; i++)
            //{
            //    if (detailTableDefine[i].OrinalTableName=="D_WorkOrderDetail")
            //        foreach (COMField f in detailTableDefine[i])
            //            if (f.FieldName =="ProcessID")
            //                f.DefaultValue = ProcessID;

            //}
            return base.InsertRowsInGrid(detailTableDefine);
        }
        public override string BeforeSaving(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            string result = null;
            string no = ds.Tables[_DetailForm.MainTable].Rows[0]["NO"] as string;
            Guid id = (Guid)ds.Tables[_DetailForm.MainTable].Rows[0]["ID"];
            DataSet data = CSystem.Sys.Svr.cntMain.Select(string.Format("Select Count(*) from D_WorkOrder where NO='{0}' and ID<>'{1}'", no,id), conn, tran);
            if ((int)data.Tables[0].Rows[0][0] > 0)
            {
                result = "工票号重复，请重新输入！";
            }
            //检测工票的单据日期是否,低于当前会计期
            data = CSystem.Sys.Svr.cntMain.Select(@"Select Value from S_BaseInfo where ID='系统\当前会计期'");
            if (ds.Tables[0].Rows.Count == 1)
            {
                DateTime currPeriod = DateTime.Parse((string)data.Tables[0].Rows[0][0]);
                if ((DateTime)ds.Tables[_DetailForm.MainTable].Rows[0]["Date"] < currPeriod)
                {
                    result = result + "不能输入以前会计期的工票!";
                }
            }
            
            //检测工单的标识是否有重复
            SortedList<string, int> Tags = new SortedList<string, int>();
            foreach (DataRow dr in ds.Tables["D_WorkOrderProduct"].Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                    continue;
                string tag = dr["Tag"] as string;
                if (tag == null)
                {
                    result = result + "标识不能为空!";
                    continue;
                }
                if (dr["Quantity"] == DBNull.Value)
                {
                    result = result + tag + "行数量不能为空";
                    continue;
                }
                int qty = (int)dr["Quantity"];
                if (Tags.ContainsKey(tag))
                    result = string.Format("{1} 标识{0}重复", tag, result);
                else
                    Tags.Add(tag, qty);
            }
            //检测工单中与工单明细的标识
            foreach (DataRow dr in ds.Tables["D_WorkOrderDetail"].Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                    continue;
                string tag = dr["Tag"] as string;
                if (tag==null)
                {
                    result =result+"标识不能为空!";
                }
                int q1;
                int q2;
                int q3;
                int q4;
                int q5;
                int q6;
                int q7;

                q1 = Convert.IsDBNull(dr["Quantity1stClass"])?0:(int)dr["Quantity1stClass"];
                q2 = Convert.IsDBNull(dr["Quantity2ndClass"])?0:(int)dr["Quantity2ndClass"];
                q3 = Convert.IsDBNull(dr["Quantity3rdClass"])?0:(int)dr["Quantity3rdClass"];
                q4 = Convert.IsDBNull(dr["QuantityMaterialWaster1"])?0:(int)dr["QuantityMaterialWaster1"];
                q5 = Convert.IsDBNull(dr["QuantityMaterialWaster2"])?0:(int)dr["QuantityMaterialWaster2"];
                q6 = Convert.IsDBNull(dr["QuantityMachineWaster1"])?0:(int)dr["QuantityMachineWaster1"];
                q7 = Convert.IsDBNull(dr["QuantityMachineWaster2"])?0:(int)dr["QuantityMachineWaster2"];
                if (Tags.ContainsKey(tag))
                {
                    Tags[tag] = Tags[tag] - q1 - q2 - q3 - q4 - q5 - q6 - q7;
                }
                else
                {
                    result = result + "存在不合格的标识!";
                }
            }

            foreach (string tag in Tags.Keys)
            {
                if (Tags[tag] != 0)
                    result = result + "标识为" + tag + "的工单明细与工单数量不相等!";
            }
            
            //检测有没有操作工
            if (ds.Tables["D_WorkOrderDetail"].Rows.Count > 0 && ds.Tables["D_WorkOrderOperator"].Rows.Count == 0)
            {
                result = result + "没有引入操作工！";
            }

            //检测工资
            decimal mny = 0;
            foreach (DataRow dr in ds.Tables["D_WorkOrderProduct"].Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                    continue;
                if (dr["Amount"] != DBNull.Value)
                    mny += (decimal)dr["Amount"];
            }
            if (mny == 0)
                result = result + "没有计算工资！";
            //检测工票上,总的金额与工资是否相等.
            decimal mny2 = 0;
            foreach (DataRow dr in ds.Tables["D_WorkOrderOperator"].Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                    continue;
                if (dr["Amount"] != DBNull.Value)
                    mny2 += (decimal)dr["Amount"];
            }
            if (mny != mny2)
                result = result + "工资总额与分配的工资总额不相等！请重新计算";

            return result;
        }
        public override bool AllowDeleteRowInToolBar
        {
            get
            {
                return true;
            }
        }
        public override bool AllowInsertRowInGrid(string TableName)
        {
            return true;
        }
        public override bool AllowUpdateInGrid(string TableName)
        {
            return true;
        }
        public override bool NewData(DataSet ds, COMFields mainTableDefine, List<COMFields> detailTableDefine)
        {
            return true;
        }
        public override void InsertToolStrip(System.Windows.Forms.ToolStrip toolStrip, ToolDetailForm.enuInsertToolStripType insertType)
        {
            base.InsertToolStrip(toolStrip,insertType);
            int i = toolStrip.Items.Count;

            if (insertType == enuInsertToolStripType.Detail)
            {
                ToolStripSeparator tss = new ToolStripSeparator();
                toolStrip.Items.Insert(i - 2, tss);

                //引入操作工
                ToolStripButton toolInsertRow = new ToolStripButton();
                toolInsertRow.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolInsertRow.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolInsertRow.Name = "toolSplit";

                toolInsertRow.Text = "引入操作工";
                toolInsertRow.Image = clsResources.Persons;
                toolInsertRow.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolInsertRow.Click += new EventHandler(toolInsertRow_Click);

                toolStrip.Items.Insert(i - 1, toolInsertRow);

                //增加重新计算的按钮
                ToolStripButton toolReCalulate = new ToolStripButton();
                toolReCalulate.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolReCalulate.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolReCalulate.Name = "toolSplit";

                toolReCalulate.Text = "重新计算";
                toolReCalulate.Image = clsResources.Calculator;
                toolReCalulate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolReCalulate.Click += new EventHandler(ReCalculateWage);

                toolStrip.Items.Insert(i - 1, toolReCalulate);

                //操作工
                ToolStripButton toolNewOperator = new ToolStripButton();
                toolNewOperator.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolNewOperator.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolNewOperator.Name = "toolSplit";

                toolNewOperator.Text = "操作工";
                toolNewOperator.Image = clsResources.Person;
                toolNewOperator.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolNewOperator.Click += new EventHandler(toolNewOperator_Click);

                toolStrip.Items.Insert(i - 1, toolNewOperator);
            }
        }

        void toolNewOperator_Click(object sender, EventArgs e)
        {
            DataRow MainRow = base._DetailForm.MainDataSet.Tables["D_WorkOrder"].Rows[0];
            if (MainRow["MachineID"] == DBNull.Value)
                return;
            MachineID = (Guid)MainRow["MachineID"];
            if (MachineID != CSystem.Sys.Svr.NullID)
            {
                string Where = string.Format(" P_Operator.IsChecker=0 and P_Operator.Disable=0 and P_Operator.ID in (select distinct B.OperatorID from D_WorkOrder A,D_WorkOrderOperator B where A.ID=B.MainID and A.MachineID='{0}')", MachineID);
                frmBrowser frm = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("P_Operator"), null,Where, enuShowStatus.None);
                DataRow[] drs = frm.ShowSelectRows("", "", "");

                
                DataTable dtOperator = base._DetailForm.MainDataSet.Tables["D_WorkOrderOperator"];

                for (int i = 0; drs!=null && i < drs.Length; i++)
                {
                    DataRow dr = drs[i];
                    DataRow NewRow = base._DetailForm.MainDataSet.Tables["D_WorkOrderOperator"].NewRow();
                    NewRow["ID"] = System.Guid.NewGuid();
                    NewRow["OperatorID"] = (Guid)dr["ID"]; ;
                    NewRow["OperatorNO"] = (string)dr["Code"];
                    NewRow["OperatorName"] = (string)dr["Name"];
                    NewRow["OperatorPropertyID"] = (Guid)dr["OperatorPropertyID"];
                    NewRow["OperatorPropertyName"] = (string)dr["OperatorPropertyName"];
                    NewRow["Ratio"] = (decimal)dr["Ratio"];
                    NewRow["StationID"] = (Guid)dr["StationID"];
                    NewRow["StationName"] = (string)dr["StationName"];
                    NewRow["Rate"] = (decimal)dr["WageQuotiety"];
                    NewRow["Adjust"] = 0;
                    NewRow["Amount"] = 0;

                    base._DetailForm.MainDataSet.Tables["D_WorkOrderOperator"].Rows.Add(NewRow);
                }
            }
            //此时再调用工资计算的方法对工资进行计算
            CalculateWage();
        }
        public override bool AllowEntryEditMode(string tableName)
        {
            if (this._DetailForm.IsChecked)
            {
                if (tableName == "D_WorkOrderOperator")
                    return true;
                else
                    return false;
            }
            else
                return true;
        }
        void toolInsertRow_Click(object sender, EventArgs e)
        {
            //if (!this._DetailForm.IsChecked)
            //{
            //    Msg.Information("工票没有复核，不能引入操作工！");
            //    return;
            //}
            DataRow MainRow = base._DetailForm.MainDataSet.Tables["D_WorkOrder"].Rows[0];
            MachineID = (Guid) MainRow["MachineID"];
            if (MachineID != CSystem.Sys.Svr.NullID)
            {
                string WhereClause = string.Format("(P_Operator.MachineID = '{0}') AND (P_Operator.Class = {1}) and P_Operator.IsChecker=0 and P_Operator.Disable=0", (Guid)MachineID, ClassIndex);
                string SQL= CSystem.Sys.Svr.LDI.GetFields("P_Operator").QuerySQLWithClause(WhereClause);
                DataSet ds= CSystem.Sys.Svr.cntMain.Select(SQL, "P_Operator");
                DataTable dt = ds.Tables["P_Operator"];
                DataTable dtOperator = base._DetailForm.MainDataSet.Tables["D_WorkOrderOperator"];
                if (dtOperator.Rows.Count > 0)
                {
                    if (Msg.Question("您已引入操作工，如果重新引入将清除原来的操作工数据，是否继续？") == DialogResult.Yes)
                    {
                        for (int i = dtOperator.Rows.Count - 1; i >= 0; i--)
                            dtOperator.Rows[i].Delete();
                    }
                    else
                        return;
                }

                //进入修改状态
                this._DetailForm.updateTool(true);

                for (int i = 0; i < dt.Rows.Count;i++ )
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    DataRow NewRow = base._DetailForm.MainDataSet.Tables["D_WorkOrderOperator"].NewRow();
                    NewRow["ID"] = System.Guid.NewGuid();
                    NewRow["OperatorID"] = (Guid)dr["ID"]; ;
                    NewRow["OperatorNO"] = (string)dr["Code"];
                    NewRow["OperatorName"] = (string)dr["Name"];
                    NewRow["OperatorPropertyID"] = dr["OperatorPropertyID"];
                    NewRow["OperatorPropertyName"] = dr["OperatorPropertyName"];
                    NewRow["Ratio"] = (decimal)dr["Ratio"];
                    NewRow["StationID"] = dr["StationID"];
                    NewRow["StationName"] = dr["StationName"];
                    if (dr["WageQuotiety"] == DBNull.Value)
                        NewRow["Rate"] = 0;
                    else
                        NewRow["Rate"] = dr["WageQuotiety"];
                    NewRow["Adjust"] = 0;
                    NewRow["Amount"]= 0;

                    base._DetailForm.MainDataSet.Tables["D_WorkOrderOperator"].Rows.Add(NewRow);
                }
            }
            //此时再调用工资计算的方法对工资进行计算
            CalculateWage();
                        
        }

        void ReCalculateWage(object sender, EventArgs e)
        {
            //进入修改状态
            this._DetailForm.updateTool(true);
            CalculateWage();
        }

        void CalculateWage() 
        {
            try
            {
                //同步数据
                foreach (Infragistics.Win.UltraWinGrid.UltraGrid grid in base._GridMap.Values)
                {
                    grid.PerformAction(UltraGridAction.ExitEditMode);
                    grid.UpdateData();
                }
                base._DetailForm.BindingContext[base._DetailForm.MainDataSet,base._DetailForm.MainTable].EndCurrentEdit();
                foreach (COMFields d in base._DetailForm.DetailTableDefine)
                    base._DetailForm.BindingContext[base._DetailForm.MainDataSet, d.OrinalTableName].EndCurrentEdit();

                decimal WorkOrderTotalWage = TotalWage();
                Guid OperatorID;

                DataTable dtWorkOrderOperator = base._DetailForm.MainDataSet.Tables["D_WorkOrderOperator"];
                decimal total = 0;//处理尾差用
                DataRow firstRow = null;
                for (int i = 0; i < dtWorkOrderOperator.Rows.Count; i++)
                {
                    DataRow dr = dtWorkOrderOperator.Rows[i];
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        if (firstRow == null)
                            firstRow = dr;
                        OperatorID = (Guid)dr["OperatorID"];
                        decimal v= Math.Round( WageAllocation(WorkOrderTotalWage, dtWorkOrderOperator, OperatorID),2);
                        dr["Amount"] = v;
                        total += v;
                    }
                }
                if (firstRow != null && total != WorkOrderTotalWage)
                    firstRow["Amount"] = (decimal)firstRow["Amount"] + (WorkOrderTotalWage - total);
                //设置工资计算标记
                DataRow drMain = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0];
                drMain["CalculateStatus"]=1;
                drMain["CalculatedBy"]=CSystem.Sys.Svr.User;;
                drMain["CalculatedByName"]=CSystem.Sys.Svr.UserName;
                drMain["CalculateDate"]=CSystem.Sys.Svr.SystemTime;
            }
            catch (Exception  CalException)
            {
                
                MessageBox.Show(CalException.Message,"工资计算错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
         }

       

        //计算工单的工资在工人间分配
        //步骤:第一步: 计算出所有工人的分配系数的总和,然后根据当前工人的分配系数计算出分配金额
        //     第二部: 根据当前待2次分配的学徒工扣减金额数来进行计算。如果本工单的操作员全部是正式工或者全部是学徒工,忽略此步。不然继续
        //             如果当前操作工为学徒工,按他自己的扣减比例扣除。
        //             如果当前操作工是正式工则在第一步分配的工资基础上加上 待分配的学徒工扣减数/本工单所有正式工的数量。

        decimal WageAllocation(decimal WorkOrderTotalWage, DataTable OperatorTable,Guid OperatorID)
        {
            decimal Result;
            decimal TotalQuotiety = 0;       //分配系数总和
            decimal TotalInternDiscount = 0; //所有学徒工的扣减金额
            int RegularCount = 0; //正式工人数;
            int InternCount = 0; //学徒工人数;
            decimal OperatorQuotiety; //分配系数
            decimal InternRatio;      //学徒工扣减比例
            decimal InternDiscount=0;   //扣减金额
            bool IsIntern=false;
            decimal myQuotiety=0;
            decimal myDiscount=0;

            //先计算总的分配系数的综合
            for (int i = 0; i < OperatorTable.Rows.Count; i++)
            { 
                DataRow DR = OperatorTable.Rows[i];
                if (DR.RowState != DataRowState.Deleted)   
                    TotalQuotiety += (decimal)DR["Rate"];             
            }


            for (int i = 0; i < OperatorTable.Rows.Count; i++)
            {   
                DataRow DR = OperatorTable.Rows[i];
                if (DR.RowState != DataRowState.Deleted)
                {
                    OperatorQuotiety = (decimal)DR["Rate"];
                    InternRatio = (decimal)DR["Ratio"];

                    if (InternRatio == 0)
                        RegularCount++;
                    else
                    {
                        InternCount++;
                        InternDiscount = (WorkOrderTotalWage * OperatorQuotiety / TotalQuotiety * InternRatio);
                        TotalInternDiscount += InternDiscount;
                    }


                    if ((Guid)DR["OperatorID"] == OperatorID)
                    {
                        myQuotiety = OperatorQuotiety; ;
                        if (InternRatio != 0)
                        {
                            IsIntern = true;
                            myDiscount = InternDiscount;
                        }


                    }
                }
            }

            Result = WorkOrderTotalWage * (myQuotiety / TotalQuotiety);

            if ((InternCount == 0) || (RegularCount == 0))
                return Result;

            if (IsIntern == true)
                Result -= myDiscount;
            else
                Result += (TotalInternDiscount / RegularCount);

            return Result ;

        }
        decimal TotalWage()
        {
            //工票的明细行,要先算出一个金额来.然后,再在工人间分配的.将各行的金额的汇总,再在人员间分配的。
            //工单的行中的金额,在劳资做工资时,再去计算出来.因为他们有可能在输完工票之后,劳资部门,才去调整标准.
            //规则是公司工资(基本信息里的工资)BasePay×品种 Material +工艺 Craft+ 标准级别 StandardLevel
            //第一个系数在P_Machine中WageQuotiety,称之为设备系数；
            //第二系数,比较复杂,是P_CraftDetail表
            //他是工艺的从表,涉及到Material,StandardLevel,
            //其中物料好理解,还有叫标准级别,这个加工标准的一个属性,由于加工标准很多,这里只是给加工标准归归类,也就是只要他们加工标准的级别相同,他们的系数是相同的.
            //该表有个字段为WageQuotiety,该系数,实际是区分了品种,工艺和加工标准的.我暂时就称之为品种系数吧.
            //工资基数*设备系数*品种系数,就得出标准的工资
            //P_CraftDetail表,还有几个字段,分别是一等品,二等品,充公品,料废品,报废品.
            //其中一等品和二等品是系数,充公品,料废品,报废品是具体的金额
            //也就是说,一,二等品是乘以标准的工资,而充公品,料废品,报废品是具体直接就给出具体的金额只要乘以数量
            //标准工资=基数*品种系数*设备系数 
            //总工资=总数量*标准工资
            //一等品,二等品是奖罚比例；充公品,料废,报废是奖罚数
            //如二等品的金额列,二等品的系数在设置是,如设为-0.7,等于标准工资*比例*数量
            //充公品,扣除数*数量；料废,报废类似.
            //最后,将各项数据相加,得出实际的工资.

            decimal BasePay;
            decimal MachineQuotiety;
            decimal StandardWage;
            decimal TotalActualWage=0;
            string HeadTag;
            string DetailTag;

            string SQL = String.Format("select ID,Name,Value from s_baseinfo where ID='HR\\BasePay'");
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(SQL, "S_BaseInfo");
            DataTable dt = ds.Tables["S_BaseInfo"];
            BasePay = Convert.ToDecimal(dt.Rows[0]["Value"].ToString());
            
            MachineQuotiety = (decimal)base._DetailForm.MainDataSet.Tables["D_WorkOrder"].Rows[0]["MachineQuotiety"];

            Guid CraftID; Guid MaterialID; Guid StdLevelID; Guid AdditionID;
            CraftID = CSystem.Sys.Svr.NullID;
            MaterialID=CSystem.Sys.Svr.NullID;
            StdLevelID = CSystem.Sys.Svr.NullID;
            
            decimal WageByProduct;
            decimal ActualWageByProduct;
            foreach (DataRow HeadRow in base._DetailForm.MainDataSet.Tables["D_WorkOrderProduct"].Rows)
            {
                if (HeadRow.RowState == DataRowState.Deleted)
                    continue;
                HeadTag = (string)HeadRow["Tag"];
                CraftID = (Guid)HeadRow["CraftID"];
                MaterialID = (Guid)HeadRow["MaterialID"];
                StdLevelID = (Guid)HeadRow["StandardLevelID"];
                if (HeadRow["AdditionID"] == DBNull.Value)
                    AdditionID = Guid.Empty;
                else
                    AdditionID = (Guid)HeadRow["AdditionID"];
                WageByProduct = 0;
                ActualWageByProduct = 0;


                foreach (DataRow DetailRow in base._DetailForm.MainDataSet.Tables["D_WorkOrderDetail"].Rows)
                {
                    if (DetailRow.RowState == DataRowState.Deleted)
                        continue;
                    int Qty1stClass = 0; //一等品数量
                    int Qty2ndClass = 0; //二等品数量
                    int Qty3rdClass = 0; //充公品数量
                    int QtyMaterialWaster1 = 0; //料费数量
                    int QtyMaterialWaster2 = 0; //料费数量
                    int QtyWaster1 = 0;  //报废数量
                    int QtyWaster2 = 0;  //报废数量
                    int SubTotalQty; // 总数量

                    decimal Class1stOffset; //一等品奖罚金额
                    decimal Class2ndOffset; //二等品奖罚金额
                    decimal Class3rdOffset; //充公品奖罚金额
                    decimal ClassMaterialWasterOffset1; //料废品奖罚金额
                    decimal ClassWasterOffset1; //报废品奖罚金额
                    decimal ClassMaterialWasterOffset2; //料废品奖罚金额
                    decimal ClassWasterOffset2; //报废品奖罚金额


                    decimal SubTotalWage = 0;
                    decimal SubActualWage = 0;
                    DetailTag = (string)DetailRow["Tag"];

                    if (HeadTag == DetailTag)
                    {
                        StandardWage = StandardWageCalc(BasePay, MachineQuotiety, CraftID, MaterialID, StdLevelID, AdditionID, HeadRow);
                        if (DetailRow["Quantity1stClass"] != DBNull.Value)
                            Qty1stClass = (int)DetailRow["Quantity1stClass"];

                        if (DetailRow["Quantity2ndClass"] != DBNull.Value)
                            Qty2ndClass = (int)DetailRow["Quantity2ndClass"];

                        if (DetailRow["Quantity3rdClass"] != DBNull.Value)
                            Qty3rdClass = (int)DetailRow["Quantity3rdClass"];

                        if (DetailRow["QuantityMaterialWaster1"] != DBNull.Value)
                            QtyMaterialWaster1 = (int)DetailRow["QuantityMaterialWaster1"];

                        if (DetailRow["QuantityMachineWaster1"] != DBNull.Value)
                            QtyWaster1 = (int)DetailRow["QuantityMachineWaster1"];

                        if (DetailRow["QuantityMaterialWaster2"] != DBNull.Value)
                            QtyMaterialWaster2 = (int)DetailRow["QuantityMaterialWaster2"];

                        if (DetailRow["QuantityMachineWaster2"] != DBNull.Value)
                            QtyWaster2 = (int)DetailRow["QuantityMachineWaster2"];

                        SubTotalQty = Qty1stClass + Qty2ndClass + Qty3rdClass + QtyMaterialWaster1 + QtyWaster1 + QtyMaterialWaster2 + QtyWaster2;

                        //总工资=总数量*标准工资
                        SubTotalWage = Math.Round(StandardWage * SubTotalQty, 2);
                        WageByProduct += SubTotalWage;

                        Class1stOffset = 0;//OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, "Quantity1st", Qty1stClass);
                        Class2ndOffset = OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, AdditionID, "Quantity2nd", Qty2ndClass);
                        Class3rdOffset = OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, AdditionID, "Quantity3rd", Qty3rdClass);
                        ClassMaterialWasterOffset1 = OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, AdditionID, "QuantityMaterialWaster", QtyMaterialWaster1);
                        ClassWasterOffset1 = OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, AdditionID, "QuantityMachineWaster", QtyWaster1);
                        ClassMaterialWasterOffset2 = OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, AdditionID, "QuantityMaterialWaster", QtyMaterialWaster2);
                        ClassWasterOffset2 = OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, AdditionID, "QuantityMachineWaster", QtyWaster2);

                        //将各项数据相加,得出实际的工资
                        SubActualWage = SubTotalWage + Class1stOffset + Class2ndOffset + Class3rdOffset + ClassMaterialWasterOffset1 + ClassWasterOffset1 + ClassMaterialWasterOffset2 + ClassWasterOffset2;
                        DetailRow["TotalAmount"] = SubTotalWage;
                        DetailRow["Amount1stClass"] = Class1stOffset;
                        DetailRow["Amount2ndClass"] = Class2ndOffset;
                        DetailRow["Amount3rdClass"] = Class3rdOffset;
                        DetailRow["AmountMaterialWaster1"] = ClassMaterialWasterOffset1;
                        DetailRow["AmountMachineWaster1"] = ClassWasterOffset1;
                        DetailRow["AmountMaterialWaster2"] = ClassMaterialWasterOffset2;
                        DetailRow["AmountMachineWaster2"] = ClassWasterOffset2;
                        DetailRow["Amount"] = SubActualWage;

                        ActualWageByProduct += SubActualWage;
                        TotalActualWage += SubActualWage;
                    } // End if
                    HeadRow["TotalAmount"] = WageByProduct;
                    HeadRow["Amount"] = ActualWageByProduct;
                }   //End WorkOrderDetail
            }//End WorkOrder Product

            return TotalActualWage;
        }



        //先计算品种系数，根据工艺,品种和标准等级到 P_CraftDetail中取得。然后再结合BasePay及设备系数得出标准工资
        decimal StandardWageCalc(decimal BasePay,decimal MachineQuotiety, Guid CraftID, Guid MaterialID, Guid StdLevelID,Guid AdditionID,DataRow HeadRow)
        {
            string SQL ;
            if (AdditionID==null || AdditionID == CSystem.Sys.Svr.NullID)
                SQL = String.Format("select WageQuotiety from P_CraftDetail Where MainID='{0}' and MaterialID='{1}' and StandardLevelID='{2}' and AdditionID is null ", CraftID, MaterialID, StdLevelID);
            else
                SQL = String.Format("select WageQuotiety from P_CraftDetail Where MainID='{0}' and MaterialID='{1}' and StandardLevelID='{2}' and AdditionID='{3}'", CraftID, MaterialID, StdLevelID, AdditionID);
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(SQL, "P_CraftDetail");
            DataTable dt = ds.Tables["P_CraftDetail"];

            if (dt.Rows.Count ==0)   
            {   string CraftName="Null";
                string MaterialName="Null";
                string StdLevelName="Null";
                string AdditionName = "Null";
                if (HeadRow["CraftName"] != DBNull.Value )
                    CraftName=(string)HeadRow["CraftName"];
                if (HeadRow["MaterialName"] != DBNull.Value )
                    MaterialName=(string)HeadRow["MaterialName"];
                if (HeadRow["StandardLevelName"] != DBNull.Value )
                    StdLevelName =(string)HeadRow["StandardLevelName"];
                if (HeadRow["AdditionName"] != DBNull.Value)
                    AdditionName = (string)HeadRow["AdditionName"];

                throw new Exception(string.Format("品种【{0}】未设置对应与工艺【{1}】，标准等级【{2}】和附加条件【{3}】品种系数", MaterialName, CraftName, StdLevelName, AdditionName));
            }

            decimal MaterialQuotiety = (decimal) dt.Rows[0]["WageQuotiety"];
            decimal Result = decimal.Round((BasePay * MachineQuotiety * MaterialQuotiety),2);
            return Result;
            
        }

        //计算各个等级的奖惩数
        decimal OffsetCalc(decimal StandardWage, Guid CraftID, Guid MaterialID, Guid StdLevelID,Guid AdditionID, string ClassName,int Quantity)
       {
            decimal Result=0;

            string SQL;
            if (AdditionID==null || AdditionID==CSystem.Sys.Svr.NullID)
                SQL = String.Format("select {0} from P_CraftDetail Where MainID='{1}' and MaterialID='{2}' and StandardLevelID='{3}' and AdditionID is null", ClassName, CraftID, MaterialID, StdLevelID);
            else
                SQL = String.Format("select {0} from P_CraftDetail Where MainID='{1}' and MaterialID='{2}' and StandardLevelID='{3}' and AdditionID='{4}'", ClassName, CraftID, MaterialID, StdLevelID, AdditionID);
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(SQL, "P_CraftDetail");
            DataTable dt = ds.Tables["P_CraftDetail"];

            if (dt.Rows.Count == 0)
                return Result ;

            decimal  OffSet= (decimal)dt.Rows[0][ClassName];

            switch (ClassName)
            {
                //case "Quantity1st":
                //    Result = StandardWage * Quantity * OffSet;
                //    break;
                case "Quantity2nd":
                case"QuantityMaterialWaster":
                    Result = StandardWage * Quantity * OffSet;
                    break;
                case"Quantity3rd":
                case"QuantityMachineWaster":
                    Result = Quantity * OffSet;
                    break;
            }
            
            return Math.Round(Result,2);
        }
        public override bool Check(Guid ID, SqlConnection conn, SqlTransaction sqlTran, DataRow mainRow)
        {
            ////首先判断是否要生成本工序的入库。
            /// 入库的时候生成的是所有等级的(包括报废)的数量。
            /// 判断是否需要生产本工序的出库,如果有下道工序,并且下道工序和本工序对应的库位不同。那么就生成本工序的出库
            /// 出库的时候生产一等品的数量。
            /// 判断是否需要生产下道工序的入库
            /// 下道工序的入库和本道工序的出库的内容相同,除工序对于的库位不同。

            Guid WorkOrderID = ID; //工单
            Guid ProcessID; //本道工序
            Guid NextProcessID = CSystem.Sys.Svr.NullID; //下一道工序
            string WorkOrderNo;
            bool IsRework = false; //是否是返修工单

            bool IsProcessIn; //是否要生成本工序的入库。
            bool IsProcessOut; //是否要生成本工序的出库
            bool IsNextProcessIn; //是否要生成下一道工序的入库
            Guid PositionID = CSystem.Sys.Svr.NullID; //工序对应的库位
            Guid WarehouseID = CSystem.Sys.Svr.NullID; //工序对于的仓库



            string WhereClause = string.Format("(D_WorkOrder.ID = '{0}')", WorkOrderID);
            string SQL = CSystem.Sys.Svr.LDI.GetFields("D_WorkOrder").QuerySQLWithClause(WhereClause);
            DataSet DSWorkOrder = CSystem.Sys.Svr.cntMain.Select(SQL, conn, sqlTran);

            DataRow drWorkorder = DSWorkOrder.Tables[0].Rows[0];

            ProcessID = (Guid)drWorkorder["ProcessID"];
            WorkOrderNo = drWorkorder["Code"].ToString();
            IsRework = Convert.ToBoolean(drWorkorder["IsRework"]);

            DateTime WorkOrderDate = Convert.ToDateTime(drWorkorder["Date"]);

            if (drWorkorder["NextProcessID"] != DBNull.Value)
                NextProcessID = (Guid)drWorkorder["NextProcessID"];

            WhereClause = string.Format("(P_Process.ID = '{0}')", ProcessID);
            SQL = CSystem.Sys.Svr.LDI.GetFields("P_Process").QuerySQLWithClause(WhereClause);
            DataSet DSProcess = CSystem.Sys.Svr.cntMain.Select(SQL, conn, sqlTran);
            DataRow drProcess = DSProcess.Tables[0].Rows[0];


            IsProcessIn = Convert.ToBoolean(drProcess["IsProcessIn"]);
            IsProcessOut = Convert.ToBoolean(drProcess["IsProcessOut"]);
            IsNextProcessIn = Convert.ToBoolean(drProcess["IsNextProcessIn"]);
            if (drProcess["PositionID"] != DBNull.Value)
                PositionID = (Guid)drProcess["PositionID"];

            if (drProcess["WarehouseID"] != DBNull.Value)
                WarehouseID = (Guid)drProcess["WarehouseID"];

            //最新处理,返修不产生出入库
            //又改了，只有本工序的入库不生成，其他都生成。at 2009.11.02
            //又改回去了.at 2009.12.02
            if (!IsRework)
            {
                if (IsProcessIn == true)
                    //生成本工序的入库,入库的是所有等级的(包括报废)的数量
                    ProcessIn(WarehouseID, ProcessID, PositionID, WorkOrderID, WorkOrderNo, WorkOrderDate, IsRework, conn, sqlTran);

                if (IsProcessOut == true)
                    //生成本工序的出库,出库的是一等品的数量。以及二等品转成一等品的数量
                    ProcessOut(WarehouseID, ProcessID, PositionID, NextProcessID, WorkOrderID, WorkOrderNo, WorkOrderDate, IsRework, conn, sqlTran);

                if (IsNextProcessIn == true)
                    //生成下道工序的入库,和本道工序的出库的内容相同,除工序对于的库位不同。
                    NextProcessIn(WarehouseID, ProcessID, PositionID, NextProcessID, WorkOrderID, WorkOrderNo, WorkOrderDate, IsRework, conn, sqlTran);

                //如果有工序中设置了成品对应的库位
                if (drProcess["FPositionID"] != DBNull.Value)
                    ProcessInFinish((Guid)drProcess["FWarehouseID"], ProcessID, (Guid)drProcess["FPositionID"], WorkOrderID, WorkOrderNo, WorkOrderDate, IsRework, conn, sqlTran);

            }

            return base.Check(ID, conn, sqlTran, mainRow);
        }
        public override bool UnCheck(Guid ID, SqlConnection conn, SqlTransaction sqlTran, DataRow mainRow)
        {
            DateTime currDate;
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"系统\当前会计期"), conn, sqlTran);
            if (ds.Tables[0].Rows.Count != 1)
            {
                Msg.Error("基本信息表数据丢失!");
                return false;
            }
            currDate = DateTime.Parse((string)ds.Tables[0].Rows[0]["Value"]);
            if (currDate > (DateTime)mainRow["Date"])
            {
                Msg.Error("您不能反复核历史工票！");
                return false;
            }

            //删除所有相关的出库单和入库单
            CSystem.Sys.Svr.cntMain.Excute(string.Format("Delete from D_WarehouseInBill where MainID in (Select ID from D_WarehouseIn where WorkOrderID='{0}');" +
                                                        "Delete from D_WarehouseIn where WorkOrderID='{0}';" +
                                                        "Delete from D_WarehouseOutBill where MainID in (Select ID from D_WarehouseOut where WorkOrderID='{0}');" +
                                                        "Delete from D_WarehouseOut where WorkOrderID='{0}';",
                                                        ID), conn, sqlTran);
            return true;
        }

        //生产工序对应库位的的出入库
        //20090614 出入库的时候需要带出材质信息
        bool ProcessInOut(Guid WarehouseID,Enum_WarehouseMovetype MovementType, Guid ProcessID, Guid WorkOrderID, String WorkOrderNo, DateTime WorkOrderDate, Guid PositionID,bool IsNextProcess,bool IsRework, SqlConnection conn, SqlTransaction sqlTran)
        {

            string StockHeadTable;
            string StockDetailTable;
            string WhereClause;
            string SQL;
            Guid MaterialID;
            //Guid IngredientID = CSystem.Sys.Svr.NullID;
            string Tag;


            if (MovementType == Enum_WarehouseMovetype.WarehouseIn)
            {
                StockHeadTable ="D_WarehouseIn";
                StockDetailTable = "D_WarehouseInBill";
            }
            else
            {
                StockHeadTable = "D_WarehouseOut";
                StockDetailTable = "D_WarehouseOutBill";
            }
            
            int Status=0;  //Status: 0-新建
            int Type=0;    //一般  对于返修的工单。 出库的Type为3，入库为6
            Guid WarehouseInOutID;     //出库单或入库单的ID
            int Qty1stClass; //一等品数量
            int Qty2ndClass; //二等品数量
            int SubTotalQty; // 总数量

            if (IsRework == true)
                if (MovementType != Enum_WarehouseMovetype.WarehouseOut)
                    Type = 3;
                else
                    Type = 6;
           

            WhereClause = string.Format("1=0");
            SQL = CSystem.Sys.Svr.LDI.GetFields(StockHeadTable).QuerySQLWithClause(WhereClause);
            DataSet DSWarehouse = CSystem.Sys.Svr.cntMain.Select(SQL,StockHeadTable,conn,sqlTran);
            DataRow dr = DSWarehouse.Tables[StockHeadTable].NewRow();
            WarehouseInOutID= System.Guid.NewGuid();
            dr["ID"] =WarehouseInOutID;
            dr["WarehouseID"] = WarehouseID;
            dr["Code"] = WorkOrderNo;
            dr["WorkOrderID"] = WorkOrderID;
            dr["Status"] = Status;
            dr["CheckStatus"] = 1;//默认为已复核
            dr["CheckDate"] = CSystem.Sys.Svr.SystemTime;
            dr["CheckedBy"] = CSystem.Sys.Svr.User;
            dr["Type"] = Type;
            //dr["Amount"] = 0;
            dr["CreatedBy"] = CSystem.Sys.Svr.User;
            dr["BillDate"] = WorkOrderDate;
            dr["CreateDate"] = CSystem.Sys.Svr.SystemTime;
            dr["PositionID"] = PositionID;
            DSWarehouse.Tables[0].Rows.Add(dr);

            CSystem.Sys.Svr.cntMain.Update(DSWarehouse.Tables[0],conn,sqlTran);


            WhereClause = string.Format("1=0");
            SQL = CSystem.Sys.Svr.LDI.GetFields(StockDetailTable).QuerySQLWithClause(WhereClause);
            DataSet DSWarehouseDetail = CSystem.Sys.Svr.cntMain.Select(SQL, StockDetailTable, conn, sqlTran);


            WhereClause = string.Format("(D_WorkOrderProduct.MainID = '{0}')", WorkOrderID);
            SQL = CSystem.Sys.Svr.LDI.GetFields("D_WorkOrderProduct").QuerySQLWithClause(WhereClause);
            DataSet DSWorkOrderProduct = CSystem.Sys.Svr.cntMain.Select(SQL, conn, sqlTran);

            for (int i = 0; i < DSWorkOrderProduct.Tables[0].Rows.Count; i++)
            {
                DataRow drWorkOrderProduct = DSWorkOrderProduct.Tables[0].Rows[i];
                //判断工艺是否为不生成出入库
                if ((int)drWorkOrderProduct["CraftIsNotMakeStore"] == 1)
                    continue;
                Tag =(string)drWorkOrderProduct["Tag"];
                MaterialID  = (Guid) drWorkOrderProduct["MaterialID"];

                //if (drWorkOrderProduct["SourceIngredientID"] != DBNull.Value)
                //    IngredientID = (Guid)drWorkOrderProduct["SourceIngredientID"];
                //else
                //    IngredientID = (Guid)drWorkOrderProduct["IngredientID"];

                
                    
                WhereClause = string.Format("(D_WorkOrderDetail.MainID = '{0}' and D_WorkOrderDetail.Tag = '{1}')", WorkOrderID,Tag);
                SQL = CSystem.Sys.Svr.LDI.GetFields("D_WorkOrderDetail").QuerySQLWithClause(WhereClause);
                DataSet DSWorkOrderDtl = CSystem.Sys.Svr.cntMain.Select(SQL, conn, sqlTran);

                for (int J = 0; J < DSWorkOrderDtl.Tables[0].Rows.Count; J++)
                {
                    DataRow drWorkOrderDtl = DSWorkOrderDtl.Tables[0].Rows[J];

                    DataRow drDetail = DSWarehouseDetail.Tables[0].NewRow();

                    //如果是下道工序入库,材质取WorkOrderDetail中加工图号中的材质的值
                    //if (IsNextProcess == true)
                    //    if (drDetail["IngredientID"] !=DBNull.Value )
                    //        IngredientID =(Guid)drDetail["IngredientID"];

                    drDetail["ID"] = System.Guid.NewGuid();
                    drDetail["MainID"] = WarehouseInOutID;
                    drDetail["LineNumber"] = J;
                    drDetail["PositionID"] = PositionID;
                    drDetail["MaterialID"] = MaterialID;
                    //drDetail["IngredientID"] = IngredientID;
                    decimal length = (decimal)drWorkOrderProduct["Length"];
                    drDetail["Length"] = length;
                    Qty1stClass = (int)drWorkOrderDtl["Quantity1stClass"];
                    Qty2ndClass = (int)drWorkOrderDtl["Quantity2ndClass"]; 
                    SubTotalQty = Qty1stClass + Qty2ndClass;            
                    drDetail["Quantity1"] = SubTotalQty;
                    if ((int)drWorkOrderProduct["QuantityCheckMethod"] == 1)
                    {
                        decimal conv = (decimal)drWorkOrderProduct["ConvertQuotiety"];
                        if (conv == 0)
                            conv = 1;
                        drDetail["Quantity2"] = Math.Round(SubTotalQty * length / conv, 4);
                    }
                    else
                    {
                        drDetail["Quantity2"] = Math.Round(SubTotalQty * length, 4);
                    }

                    //drDetail["Price"] = 0;
                    drDetail["Money"] = 0;
                    DSWarehouseDetail.Tables[StockDetailTable].Rows.Add(drDetail);
                }


            }

            CSystem.Sys.Svr.cntMain.Update(DSWarehouseDetail.Tables[StockDetailTable], conn, sqlTran);

            return true;
 
        }

        //生成成品库的入库
        bool ProcessInFinish(Guid WarehouseID, Guid ProcessID, Guid PositionID, Guid WorkOrderID, String WorkOrderNo, DateTime WorkOrderDate, bool IsRework, SqlConnection conn, SqlTransaction sqlTran)
        {

            string StockHeadTable;
            string StockDetailTable;


            StockHeadTable = "D_WarehouseIn";
            StockDetailTable = "D_WarehouseInBill";

            string WhereClause = string.Format("(D_WorkOrderProduct.MainID = '{0}')", WorkOrderID);
            string SQL = CSystem.Sys.Svr.LDI.GetFields("D_WorkOrderProduct").QuerySQLWithClause(WhereClause);
            DataSet DSWorkOrderProduct = CSystem.Sys.Svr.cntMain.Select(SQL, conn, sqlTran);

            int Status = 0;  //Status: 0-新建
            int Type = 0;    //一般  对于返修的工单。 出库的Type为3，入库为6

            if (IsRework == true)
                Type = 6;

            Guid WarehouseInOutID;     //出库单或入库单的ID
            int Quantity = 0;

            WhereClause = string.Format("1=0");
            SQL = CSystem.Sys.Svr.LDI.GetFields(StockHeadTable).QuerySQLWithClause(WhereClause);
            DataSet DSWarehouse = CSystem.Sys.Svr.cntMain.Select(SQL, StockHeadTable, conn, sqlTran);
            DataRow dr = DSWarehouse.Tables[StockHeadTable].NewRow();
            WarehouseInOutID = System.Guid.NewGuid();
            dr["ID"] = WarehouseInOutID;
            dr["WarehouseID"] = WarehouseID;
            dr["Code"] = WorkOrderNo;
            dr["WorkOrderID"] = WorkOrderID;
            dr["Status"] = Status;
            dr["CheckStatus"] = 1;//默认为已复核
            dr["CheckDate"] = CSystem.Sys.Svr.SystemTime;
            dr["CheckedBy"] = CSystem.Sys.Svr.User;
            dr["Type"] = Type;
            dr["Amount"] = 0;
            dr["CreatedBy"] = CSystem.Sys.Svr.User;
            dr["BillDate"] = WorkOrderDate;
            dr["CreateDate"] = CSystem.Sys.Svr.SystemTime;
            dr["PositionID"] = PositionID;
            DSWarehouse.Tables[0].Rows.Add(dr);

            CSystem.Sys.Svr.cntMain.Update(DSWarehouse.Tables[0], conn, sqlTran);

            SQL = CSystem.Sys.Svr.LDI.GetFields(StockDetailTable).QuerySQLWithClause(WhereClause);
            DataSet DSWarehouseDetail = CSystem.Sys.Svr.cntMain.Select(SQL, StockDetailTable, conn, sqlTran);
            WhereClause = string.Format("(D_WorkOrderDetail.MainID = '{0}')", WorkOrderID);
            SQL = CSystem.Sys.Svr.LDI.GetFields("D_WorkOrderDetail").QuerySQLWithClause(WhereClause);
            DataSet DSWorkOrderDtl = CSystem.Sys.Svr.cntMain.Select(SQL, conn, sqlTran);

            for (int i = 0; i < DSWorkOrderProduct.Tables[0].Rows.Count; i++)
            {
                DataRow drWorkOrderProduct = DSWorkOrderProduct.Tables[0].Rows[i];
                //判断工艺是否为不生成出入库
                if ((int)drWorkOrderProduct["CraftIsNotMakeStore"] == 1)
                    continue;
                string Tag =(string)drWorkOrderProduct["Tag"];
                //MaterialID  = (Guid) drWorkOrderProduct["MaterialID"];

                //if (drWorkOrderProduct["SourceIngredientID"] != DBNull.Value)
                //    IngredientID = (Guid)drWorkOrderProduct["SourceIngredientID"];
                //else
                //    IngredientID = (Guid)drWorkOrderProduct["IngredientID"];

                
                DataRow[] drDetails = DSWorkOrderDtl.Tables[0].Select("Tag='"+Tag+"'");

                for (int J = 0; J < drDetails.Length; J++)
                {
                    DataRow drWorkOrderDtl = drDetails[J];

                    DataRow drDetail = DSWarehouseDetail.Tables[0].NewRow();

                    drDetail["ID"] = System.Guid.NewGuid();
                    drDetail["MainID"] = WarehouseInOutID;
                    drDetail["LineNumber"] = J;
                    drDetail["PositionID"] = PositionID;
                    //找出对应的成品物料
                    DataSet ds = CSystem.Sys.Svr.cntMain.Select("Select ID from P_Material where MaterialType=2 and WIPID='" + drWorkOrderProduct["MaterialID"] + "'");
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        Msg.Error(string.Format("{0}没有被相应的成品设置关系！", drWorkOrderProduct["MaterialName"]));
                        return false;
                    }
                    else
                        drDetail["MaterialID"] = ds.Tables[0].Rows[0]["ID"];

                    //加工图号
                    drDetail["MachiningStandardID"] = drWorkOrderDtl["MachineStandardID"];

                    //drDetail["IngredientID"] = IngredientID;
                    decimal length = (decimal)drWorkOrderProduct["Length"];
                    drDetail["Length"] = length;
                    int Qty1stClass = (int)drWorkOrderDtl["Quantity1stClass"];
                    int Qty2ndClass = (int)drWorkOrderDtl["Quantity2ndClass"];
                    int SubTotalQty = Qty1stClass + Qty2ndClass;
                    drDetail["Quantity1"] = SubTotalQty;
                    if ((int)drWorkOrderProduct["QuantityCheckMethod"] == 1)
                    {
                        decimal conv = (decimal)drWorkOrderProduct["ConvertQuotiety"];
                        if (conv == 0)
                            conv = 1;
                        drDetail["Quantity2"] = Math.Round(SubTotalQty * length / conv, 4);
                    }
                    else
                    {
                        drDetail["Quantity2"] = Math.Round(SubTotalQty * length, 4);
                    }

                    //drDetail["Price"] = 0;
                    drDetail["Money"] = 0;
                    DSWarehouseDetail.Tables[StockDetailTable].Rows.Add(drDetail);
                }
            }

            CSystem.Sys.Svr.cntMain.Update(DSWarehouseDetail.Tables[StockDetailTable], conn, sqlTran);

            return true;

        }
        //本工序的入库,这里入库的数据来自于D_WorkOrderProduct.
        //生成本工序的入库时,加工图号(即原来的加工标准),以"来源图号"为准,如没有来源图号,以本身的加工图号为准
        bool ProcessIn(Guid WarehouseID, Guid ProcessID, Guid PositionID, Guid WorkOrderID, String WorkOrderNo, DateTime WorkOrderDate, bool IsRework, SqlConnection conn, SqlTransaction sqlTran)
        {

            string StockHeadTable;
            string StockDetailTable;

  
            StockHeadTable = "D_WarehouseIn";
            StockDetailTable = "D_WarehouseInBill";
 
            string WhereClause = string.Format("(D_WorkOrderProduct.MainID = '{0}')", WorkOrderID);
            string SQL = CSystem.Sys.Svr.LDI.GetFields("D_WorkOrderProduct").QuerySQLWithClause(WhereClause);
            DataSet DSWorkOrderProduct = CSystem.Sys.Svr.cntMain.Select(SQL, conn, sqlTran);

            int Status = 0;  //Status: 0-新建
            int Type = 0;    //一般  对于返修的工单。 出库的Type为3，入库为6
            
            if (IsRework == true)
                Type = 6;

            Guid WarehouseInOutID;     //出库单或入库单的ID
            int Quantity = 0;
            //Guid IngredientID = CSystem.Sys.Svr.NullID;

            

            WhereClause = string.Format("1=0");
            SQL = CSystem.Sys.Svr.LDI.GetFields(StockHeadTable).QuerySQLWithClause(WhereClause);
            DataSet DSWarehouse = CSystem.Sys.Svr.cntMain.Select(SQL, StockHeadTable, conn, sqlTran);
            DataRow dr = DSWarehouse.Tables[StockHeadTable].NewRow();
            WarehouseInOutID = System.Guid.NewGuid();
            dr["ID"] = WarehouseInOutID;
            dr["WarehouseID"] = WarehouseID;
            dr["Code"] = WorkOrderNo;
            dr["WorkOrderID"] = WorkOrderID;
            dr["Status"] = Status;
            dr["CheckStatus"] = 1;//默认为已复核
            dr["CheckDate"] = CSystem.Sys.Svr.SystemTime;
            dr["CheckedBy"] = CSystem.Sys.Svr.User;
            dr["Type"] = Type;
            dr["Amount"] = 0;
            dr["CreatedBy"] = CSystem.Sys.Svr.User;
            dr["BillDate"] = WorkOrderDate;
            dr["CreateDate"] = CSystem.Sys.Svr.SystemTime;
            dr["PositionID"] = PositionID;
            DSWarehouse.Tables[0].Rows.Add(dr);

            CSystem.Sys.Svr.cntMain.Update(DSWarehouse.Tables[0], conn, sqlTran);

            SQL = CSystem.Sys.Svr.LDI.GetFields(StockDetailTable).QuerySQLWithClause(WhereClause);
            DataSet DSWarehouseDetail = CSystem.Sys.Svr.cntMain.Select(SQL, StockDetailTable, conn, sqlTran);

            for (int i = 0; i < DSWorkOrderProduct.Tables[0].Rows.Count; i++)
            {
                DataRow drWorkOrderProduct = DSWorkOrderProduct.Tables[0].Rows[i];

                //判断工艺是否为不生成出入库
                if ((int)drWorkOrderProduct["CraftIsNotMakeStore"] == 1)
                    continue;
                DataRow drDetail = DSWarehouseDetail.Tables[0].NewRow();
                drDetail["ID"] = System.Guid.NewGuid();
                drDetail["MainID"] = WarehouseInOutID;
                drDetail["LineNumber"] = i;
                drDetail["PositionID"] = PositionID;
                drDetail["MaterialID"] = drWorkOrderProduct["MaterialID"];
                
                //if (drWorkOrderProduct["SourceIngredientID"] != DBNull.Value )
                //    IngredientID = (Guid)drWorkOrderProduct["SourceIngredientID"];
                //else
                //    if (drWorkOrderProduct["IngredientID"] !=DBNull.Value)
                //     IngredientID = (Guid)drWorkOrderProduct["IngredientID"];

                //drDetail["IngredientID"] = IngredientID;
                decimal length = (decimal)drWorkOrderProduct["Length"];
                drDetail["Length"] = length;
                Quantity  = (int)drWorkOrderProduct["Quantity"];
                drDetail["Quantity1"] = Quantity;
                if ((int)drWorkOrderProduct["QuantityCheckMethod"] == 1)
                {
                    decimal conv = (decimal)drWorkOrderProduct["ConvertQuotiety"];
                    if (conv == 0)
                        conv = 1;
                    drDetail["Quantity2"] = Math.Round(Quantity * length / conv, 4);
                }else
                {
                    drDetail["Quantity2"] = Math.Round(Quantity * length, 4);
                }
                drDetail["Price"] = 0;
                drDetail["Money"] = 0;
                DSWarehouseDetail.Tables[StockDetailTable].Rows.Add(drDetail);
            }

            CSystem.Sys.Svr.cntMain.Update(DSWarehouseDetail.Tables[StockDetailTable], conn, sqlTran);

            return true;

        }


        //这里传入下一道工序作参数,主要是用于判断下道工序和本工序对应的库位是否相同。如果不同就生成本工序的出库,反之如果库位相同就不生成本工序的出库
        bool ProcessOut(Guid WarehouseID, Guid ProcessID, Guid PositionID, Guid NextProcessID, Guid WorkOrderID, String WorkOrderNo, DateTime WorkOrderDate, bool IsRework, SqlConnection conn, SqlTransaction sqlTran)
        {
            Guid NextPositionID=CSystem.Sys.Svr.NullID;
            if (NextProcessID!=CSystem.Sys.Svr.NullID)
                NextPositionID = GetPositionID(NextProcessID);

            if (PositionID != NextPositionID)
                ProcessInOut(WarehouseID,Enum_WarehouseMovetype.WarehouseOut, ProcessID, WorkOrderID, WorkOrderNo,WorkOrderDate,PositionID,false,IsRework, conn, sqlTran);

            return true;
 
        }

        //这里传入下一道工序作参数,主要是用于判断下道工序和本工序对应的库位是否相同。如果不同就生成下一道工序的入库,反之如果库位相同就不生成下一道工序的入库
        bool NextProcessIn(Guid WarehouseID, Guid ProcessID, Guid PositionID, Guid NextProcessID, Guid WorkOrderID, String WorkOrderNo, DateTime WorkOrderDate, bool IsRework, SqlConnection conn, SqlTransaction sqlTran)
        {
            Guid NextPositionID = CSystem.Sys.Svr.NullID;

            NextPositionID = GetPositionID(NextProcessID);
            if (PositionID != NextPositionID)
                ProcessInOut(WarehouseID, Enum_WarehouseMovetype.WarehouseIn, ProcessID, WorkOrderID, WorkOrderNo, WorkOrderDate, NextPositionID, true, IsRework, conn, sqlTran);


            return true;
 
        }

        //根据工序取得其所对应的库位
        Guid GetPositionID(Guid ProcessID)
        {

            string WhereClause = string.Format("(P_Process.ID = '{0}')", ProcessID);
            string SQL = CSystem.Sys.Svr.LDI.GetFields("P_Process").QuerySQLWithClause(WhereClause);
            DataSet DSProcess = CSystem.Sys.Svr.cntMain.Select(SQL);
            DataRow drProcess = null;
            Guid PostionsID = CSystem.Sys.Svr.NullID;
            if (DSProcess.Tables[0].Rows.Count == 1)
                drProcess = DSProcess.Tables[0].Rows[0];
            else
                return PostionsID;

            if (drProcess["PositionID"] != DBNull.Value)
                PostionsID = (Guid)drProcess["PositionID"];

            return PostionsID;

        }

        public override bool ShowAddRowButton(string tableName)
        {
            switch (tableName)
            {
                case "D_WorkOrderDetail":
                    return true;
                case "D_WorkOrderProduct":
                    return true;
                default:
                    return false;
            }
        }
        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
            //设置要复制的值
            SortedList<string, string> copy = new SortedList<string, string>();
            copy.Add("DepartmentID", "DepartmentID");
            copy.Add("DepartmentName", "DepartmentName");
            copy.Add("Class", "Class");
            copy.Add("ClassType", "ClassType");
            //copy.Add("MachineID", "MachineID");
            //copy.Add("MachineName", "MachineName");
            //copy.Add("MachineQuotiety", "MachineQuotiety");
            copy.Add("ProcessID", "ProcessID");
            copy.Add("ProcessName", "ProcessName");
            copy.Add("NextProcessID", "NextProcessID");
            copy.Add("NextProcess", "NextProcess");
            //copy.Add("OperatorID", "OperatorID");
            //copy.Add("OperatorName", "OperatorName");
            //copy.Add("IsRework", "IsRework");
            copy.Add("Date", "Date");
            frm.CopyLastValue = copy;
            return frm;
        }
    }
}
