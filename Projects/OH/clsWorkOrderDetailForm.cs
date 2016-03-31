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
        private int ClassIndex=0; //����
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
            //���ñ�ʶ�Ŀ��
            if (createGrid.Grid.DisplayLayout.Bands[0].Columns.Exists("Tag"))
                createGrid.Grid.DisplayLayout.Bands[0].Columns["Tag"].Width = 30;
        }
        void GridProduct_AfterRowInsert(object sender, RowEventArgs e)
        {
            //int i = char.ConvertToUtf32("A", 0);
            e.Row.Cells["Tag"].Value =(e.Row.Index+1).ToString("000"); 
            //char.ConvertFromUtf32(i + e.Row.Index);
            //Ĭ�ϳ���Ϊ��
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
                    ////�����ܽ��
                    //UltraGrid grid = (UltraGrid)sender;
                    //decimal total = 0;
                    //foreach (UltraGridRow row in grid.Rows)
                    //    total += (decimal)row.Cells["Money"].Value;
                    //_ControlMap["Money"].Text = total.ToString();
                    ////���㺬˰���
                    //e.Cell.Row.Cells["Amount"].Value = Math.Round((1 + ((UltraCurrencyEditor)this._ControlMap["TaxRate"]).Value / 100) * (decimal)e.Cell.Value, 2);
                    ////���㵥��
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
                result = "��Ʊ���ظ������������룡";
            }
            //��⹤Ʊ�ĵ��������Ƿ�,���ڵ�ǰ�����
            data = CSystem.Sys.Svr.cntMain.Select(@"Select Value from S_BaseInfo where ID='ϵͳ\��ǰ�����'");
            if (ds.Tables[0].Rows.Count == 1)
            {
                DateTime currPeriod = DateTime.Parse((string)data.Tables[0].Rows[0][0]);
                if ((DateTime)ds.Tables[_DetailForm.MainTable].Rows[0]["Date"] < currPeriod)
                {
                    result = result + "����������ǰ����ڵĹ�Ʊ!";
                }
            }
            
            //��⹤���ı�ʶ�Ƿ����ظ�
            SortedList<string, int> Tags = new SortedList<string, int>();
            foreach (DataRow dr in ds.Tables["D_WorkOrderProduct"].Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                    continue;
                string tag = dr["Tag"] as string;
                if (tag == null)
                {
                    result = result + "��ʶ����Ϊ��!";
                    continue;
                }
                if (dr["Quantity"] == DBNull.Value)
                {
                    result = result + tag + "����������Ϊ��";
                    continue;
                }
                int qty = (int)dr["Quantity"];
                if (Tags.ContainsKey(tag))
                    result = string.Format("{1} ��ʶ{0}�ظ�", tag, result);
                else
                    Tags.Add(tag, qty);
            }
            //��⹤�����빤����ϸ�ı�ʶ
            foreach (DataRow dr in ds.Tables["D_WorkOrderDetail"].Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                    continue;
                string tag = dr["Tag"] as string;
                if (tag==null)
                {
                    result =result+"��ʶ����Ϊ��!";
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
                    result = result + "���ڲ��ϸ�ı�ʶ!";
                }
            }

            foreach (string tag in Tags.Keys)
            {
                if (Tags[tag] != 0)
                    result = result + "��ʶΪ" + tag + "�Ĺ�����ϸ�빤�����������!";
            }
            
            //�����û�в�����
            if (ds.Tables["D_WorkOrderDetail"].Rows.Count > 0 && ds.Tables["D_WorkOrderOperator"].Rows.Count == 0)
            {
                result = result + "û�������������";
            }

            //��⹤��
            decimal mny = 0;
            foreach (DataRow dr in ds.Tables["D_WorkOrderProduct"].Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                    continue;
                if (dr["Amount"] != DBNull.Value)
                    mny += (decimal)dr["Amount"];
            }
            if (mny == 0)
                result = result + "û�м��㹤�ʣ�";
            //��⹤Ʊ��,�ܵĽ���빤���Ƿ����.
            decimal mny2 = 0;
            foreach (DataRow dr in ds.Tables["D_WorkOrderOperator"].Rows)
            {
                if (dr.RowState == DataRowState.Deleted)
                    continue;
                if (dr["Amount"] != DBNull.Value)
                    mny2 += (decimal)dr["Amount"];
            }
            if (mny != mny2)
                result = result + "�����ܶ������Ĺ����ܶ��ȣ������¼���";

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

                //���������
                ToolStripButton toolInsertRow = new ToolStripButton();
                toolInsertRow.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolInsertRow.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolInsertRow.Name = "toolSplit";

                toolInsertRow.Text = "���������";
                toolInsertRow.Image = clsResources.Persons;
                toolInsertRow.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolInsertRow.Click += new EventHandler(toolInsertRow_Click);

                toolStrip.Items.Insert(i - 1, toolInsertRow);

                //�������¼���İ�ť
                ToolStripButton toolReCalulate = new ToolStripButton();
                toolReCalulate.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolReCalulate.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolReCalulate.Name = "toolSplit";

                toolReCalulate.Text = "���¼���";
                toolReCalulate.Image = clsResources.Calculator;
                toolReCalulate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolReCalulate.Click += new EventHandler(ReCalculateWage);

                toolStrip.Items.Insert(i - 1, toolReCalulate);

                //������
                ToolStripButton toolNewOperator = new ToolStripButton();
                toolNewOperator.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolNewOperator.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolNewOperator.Name = "toolSplit";

                toolNewOperator.Text = "������";
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
            //��ʱ�ٵ��ù��ʼ���ķ����Թ��ʽ��м���
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
            //    Msg.Information("��Ʊû�и��ˣ����������������");
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
                    if (Msg.Question("�������������������������뽫���ԭ���Ĳ��������ݣ��Ƿ������") == DialogResult.Yes)
                    {
                        for (int i = dtOperator.Rows.Count - 1; i >= 0; i--)
                            dtOperator.Rows[i].Delete();
                    }
                    else
                        return;
                }

                //�����޸�״̬
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
            //��ʱ�ٵ��ù��ʼ���ķ����Թ��ʽ��м���
            CalculateWage();
                        
        }

        void ReCalculateWage(object sender, EventArgs e)
        {
            //�����޸�״̬
            this._DetailForm.updateTool(true);
            CalculateWage();
        }

        void CalculateWage() 
        {
            try
            {
                //ͬ������
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
                decimal total = 0;//����β����
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
                //���ù��ʼ�����
                DataRow drMain = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0];
                drMain["CalculateStatus"]=1;
                drMain["CalculatedBy"]=CSystem.Sys.Svr.User;;
                drMain["CalculatedByName"]=CSystem.Sys.Svr.UserName;
                drMain["CalculateDate"]=CSystem.Sys.Svr.SystemTime;
            }
            catch (Exception  CalException)
            {
                
                MessageBox.Show(CalException.Message,"���ʼ������",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
         }

       

        //���㹤���Ĺ����ڹ��˼����
        //����:��һ��: ��������й��˵ķ���ϵ�����ܺ�,Ȼ����ݵ�ǰ���˵ķ���ϵ�������������
        //     �ڶ���: ���ݵ�ǰ��2�η����ѧͽ���ۼ�����������м��㡣����������Ĳ���Աȫ������ʽ������ȫ����ѧͽ��,���Դ˲�����Ȼ����
        //             �����ǰ������Ϊѧͽ��,�����Լ��Ŀۼ������۳���
        //             �����ǰ����������ʽ�����ڵ�һ������Ĺ��ʻ����ϼ��� �������ѧͽ���ۼ���/������������ʽ����������

        decimal WageAllocation(decimal WorkOrderTotalWage, DataTable OperatorTable,Guid OperatorID)
        {
            decimal Result;
            decimal TotalQuotiety = 0;       //����ϵ���ܺ�
            decimal TotalInternDiscount = 0; //����ѧͽ���Ŀۼ����
            int RegularCount = 0; //��ʽ������;
            int InternCount = 0; //ѧͽ������;
            decimal OperatorQuotiety; //����ϵ��
            decimal InternRatio;      //ѧͽ���ۼ�����
            decimal InternDiscount=0;   //�ۼ����
            bool IsIntern=false;
            decimal myQuotiety=0;
            decimal myDiscount=0;

            //�ȼ����ܵķ���ϵ�����ۺ�
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
            //��Ʊ����ϸ��,Ҫ�����һ�������.Ȼ��,���ڹ��˼�����.�����еĽ��Ļ���,������Ա�����ġ�
            //���������еĽ��,������������ʱ,��ȥ�������.��Ϊ�����п��������깤Ʊ֮��,���ʲ���,��ȥ������׼.
            //�����ǹ�˾����(������Ϣ��Ĺ���)BasePay��Ʒ�� Material +���� Craft+ ��׼���� StandardLevel
            //��һ��ϵ����P_Machine��WageQuotiety,��֮Ϊ�豸ϵ����
            //�ڶ�ϵ��,�Ƚϸ���,��P_CraftDetail��
            //���ǹ��յĴӱ�,�漰��Material,StandardLevel,
            //�������Ϻ����,���нб�׼����,����ӹ���׼��һ������,���ڼӹ���׼�ܶ�,����ֻ�Ǹ��ӹ���׼�����,Ҳ����ֻҪ���Ǽӹ���׼�ļ�����ͬ,���ǵ�ϵ������ͬ��.
            //�ñ��и��ֶ�ΪWageQuotiety,��ϵ��,ʵ����������Ʒ��,���պͼӹ���׼��.����ʱ�ͳ�֮ΪƷ��ϵ����.
            //���ʻ���*�豸ϵ��*Ʒ��ϵ��,�͵ó���׼�Ĺ���
            //P_CraftDetail��,���м����ֶ�,�ֱ���һ��Ʒ,����Ʒ,�乫Ʒ,�Ϸ�Ʒ,����Ʒ.
            //����һ��Ʒ�Ͷ���Ʒ��ϵ��,�乫Ʒ,�Ϸ�Ʒ,����Ʒ�Ǿ���Ľ��
            //Ҳ����˵,һ,����Ʒ�ǳ��Ա�׼�Ĺ���,���乫Ʒ,�Ϸ�Ʒ,����Ʒ�Ǿ���ֱ�Ӿ͸�������Ľ��ֻҪ��������
            //��׼����=����*Ʒ��ϵ��*�豸ϵ�� 
            //�ܹ���=������*��׼����
            //һ��Ʒ,����Ʒ�ǽ����������乫Ʒ,�Ϸ�,�����ǽ�����
            //�����Ʒ�Ľ����,����Ʒ��ϵ����������,����Ϊ-0.7,���ڱ�׼����*����*����
            //�乫Ʒ,�۳���*�������Ϸ�,��������.
            //���,�������������,�ó�ʵ�ʵĹ���.

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
                    int Qty1stClass = 0; //һ��Ʒ����
                    int Qty2ndClass = 0; //����Ʒ����
                    int Qty3rdClass = 0; //�乫Ʒ����
                    int QtyMaterialWaster1 = 0; //�Ϸ�����
                    int QtyMaterialWaster2 = 0; //�Ϸ�����
                    int QtyWaster1 = 0;  //��������
                    int QtyWaster2 = 0;  //��������
                    int SubTotalQty; // ������

                    decimal Class1stOffset; //һ��Ʒ�������
                    decimal Class2ndOffset; //����Ʒ�������
                    decimal Class3rdOffset; //�乫Ʒ�������
                    decimal ClassMaterialWasterOffset1; //�Ϸ�Ʒ�������
                    decimal ClassWasterOffset1; //����Ʒ�������
                    decimal ClassMaterialWasterOffset2; //�Ϸ�Ʒ�������
                    decimal ClassWasterOffset2; //����Ʒ�������


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

                        //�ܹ���=������*��׼����
                        SubTotalWage = Math.Round(StandardWage * SubTotalQty, 2);
                        WageByProduct += SubTotalWage;

                        Class1stOffset = 0;//OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, "Quantity1st", Qty1stClass);
                        Class2ndOffset = OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, AdditionID, "Quantity2nd", Qty2ndClass);
                        Class3rdOffset = OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, AdditionID, "Quantity3rd", Qty3rdClass);
                        ClassMaterialWasterOffset1 = OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, AdditionID, "QuantityMaterialWaster", QtyMaterialWaster1);
                        ClassWasterOffset1 = OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, AdditionID, "QuantityMachineWaster", QtyWaster1);
                        ClassMaterialWasterOffset2 = OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, AdditionID, "QuantityMaterialWaster", QtyMaterialWaster2);
                        ClassWasterOffset2 = OffsetCalc(StandardWage, CraftID, MaterialID, StdLevelID, AdditionID, "QuantityMachineWaster", QtyWaster2);

                        //�������������,�ó�ʵ�ʵĹ���
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



        //�ȼ���Ʒ��ϵ�������ݹ���,Ʒ�ֺͱ�׼�ȼ��� P_CraftDetail��ȡ�á�Ȼ���ٽ��BasePay���豸ϵ���ó���׼����
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

                throw new Exception(string.Format("Ʒ�֡�{0}��δ���ö�Ӧ�빤�ա�{1}������׼�ȼ���{2}���͸���������{3}��Ʒ��ϵ��", MaterialName, CraftName, StdLevelName, AdditionName));
            }

            decimal MaterialQuotiety = (decimal) dt.Rows[0]["WageQuotiety"];
            decimal Result = decimal.Round((BasePay * MachineQuotiety * MaterialQuotiety),2);
            return Result;
            
        }

        //��������ȼ��Ľ�����
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
            ////�����ж��Ƿ�Ҫ���ɱ��������⡣
            /// ����ʱ�����ɵ������еȼ���(��������)��������
            /// �ж��Ƿ���Ҫ����������ĳ���,������µ�����,�����µ�����ͱ������Ӧ�Ŀ�λ��ͬ����ô�����ɱ�����ĳ���
            /// �����ʱ������һ��Ʒ��������
            /// �ж��Ƿ���Ҫ�����µ���������
            /// �µ���������ͱ�������ĳ����������ͬ,��������ڵĿ�λ��ͬ��

            Guid WorkOrderID = ID; //����
            Guid ProcessID; //��������
            Guid NextProcessID = CSystem.Sys.Svr.NullID; //��һ������
            string WorkOrderNo;
            bool IsRework = false; //�Ƿ��Ƿ��޹���

            bool IsProcessIn; //�Ƿ�Ҫ���ɱ��������⡣
            bool IsProcessOut; //�Ƿ�Ҫ���ɱ�����ĳ���
            bool IsNextProcessIn; //�Ƿ�Ҫ������һ����������
            Guid PositionID = CSystem.Sys.Svr.NullID; //�����Ӧ�Ŀ�λ
            Guid WarehouseID = CSystem.Sys.Svr.NullID; //������ڵĲֿ�



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

            //���´���,���޲����������
            //�ָ��ˣ�ֻ�б��������ⲻ���ɣ����������ɡ�at 2009.11.02
            //�ָĻ�ȥ��.at 2009.12.02
            if (!IsRework)
            {
                if (IsProcessIn == true)
                    //���ɱ���������,���������еȼ���(��������)������
                    ProcessIn(WarehouseID, ProcessID, PositionID, WorkOrderID, WorkOrderNo, WorkOrderDate, IsRework, conn, sqlTran);

                if (IsProcessOut == true)
                    //���ɱ�����ĳ���,�������һ��Ʒ���������Լ�����Ʒת��һ��Ʒ������
                    ProcessOut(WarehouseID, ProcessID, PositionID, NextProcessID, WorkOrderID, WorkOrderNo, WorkOrderDate, IsRework, conn, sqlTran);

                if (IsNextProcessIn == true)
                    //�����µ���������,�ͱ�������ĳ����������ͬ,��������ڵĿ�λ��ͬ��
                    NextProcessIn(WarehouseID, ProcessID, PositionID, NextProcessID, WorkOrderID, WorkOrderNo, WorkOrderDate, IsRework, conn, sqlTran);

                //����й����������˳�Ʒ��Ӧ�Ŀ�λ
                if (drProcess["FPositionID"] != DBNull.Value)
                    ProcessInFinish((Guid)drProcess["FWarehouseID"], ProcessID, (Guid)drProcess["FPositionID"], WorkOrderID, WorkOrderNo, WorkOrderDate, IsRework, conn, sqlTran);

            }

            return base.Check(ID, conn, sqlTran, mainRow);
        }
        public override bool UnCheck(Guid ID, SqlConnection conn, SqlTransaction sqlTran, DataRow mainRow)
        {
            DateTime currDate;
            DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select * from S_BaseInfo where ID='{0}'", @"ϵͳ\��ǰ�����"), conn, sqlTran);
            if (ds.Tables[0].Rows.Count != 1)
            {
                Msg.Error("������Ϣ�����ݶ�ʧ!");
                return false;
            }
            currDate = DateTime.Parse((string)ds.Tables[0].Rows[0]["Value"]);
            if (currDate > (DateTime)mainRow["Date"])
            {
                Msg.Error("�����ܷ�������ʷ��Ʊ��");
                return false;
            }

            //ɾ��������صĳ��ⵥ����ⵥ
            CSystem.Sys.Svr.cntMain.Excute(string.Format("Delete from D_WarehouseInBill where MainID in (Select ID from D_WarehouseIn where WorkOrderID='{0}');" +
                                                        "Delete from D_WarehouseIn where WorkOrderID='{0}';" +
                                                        "Delete from D_WarehouseOutBill where MainID in (Select ID from D_WarehouseOut where WorkOrderID='{0}');" +
                                                        "Delete from D_WarehouseOut where WorkOrderID='{0}';",
                                                        ID), conn, sqlTran);
            return true;
        }

        //���������Ӧ��λ�ĵĳ����
        //20090614 ������ʱ����Ҫ����������Ϣ
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
            
            int Status=0;  //Status: 0-�½�
            int Type=0;    //һ��  ���ڷ��޵Ĺ����� �����TypeΪ3�����Ϊ6
            Guid WarehouseInOutID;     //���ⵥ����ⵥ��ID
            int Qty1stClass; //һ��Ʒ����
            int Qty2ndClass; //����Ʒ����
            int SubTotalQty; // ������

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
            dr["CheckStatus"] = 1;//Ĭ��Ϊ�Ѹ���
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
                //�жϹ����Ƿ�Ϊ�����ɳ����
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

                    //������µ��������,����ȡWorkOrderDetail�мӹ�ͼ���еĲ��ʵ�ֵ
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

        //���ɳ�Ʒ������
        bool ProcessInFinish(Guid WarehouseID, Guid ProcessID, Guid PositionID, Guid WorkOrderID, String WorkOrderNo, DateTime WorkOrderDate, bool IsRework, SqlConnection conn, SqlTransaction sqlTran)
        {

            string StockHeadTable;
            string StockDetailTable;


            StockHeadTable = "D_WarehouseIn";
            StockDetailTable = "D_WarehouseInBill";

            string WhereClause = string.Format("(D_WorkOrderProduct.MainID = '{0}')", WorkOrderID);
            string SQL = CSystem.Sys.Svr.LDI.GetFields("D_WorkOrderProduct").QuerySQLWithClause(WhereClause);
            DataSet DSWorkOrderProduct = CSystem.Sys.Svr.cntMain.Select(SQL, conn, sqlTran);

            int Status = 0;  //Status: 0-�½�
            int Type = 0;    //һ��  ���ڷ��޵Ĺ����� �����TypeΪ3�����Ϊ6

            if (IsRework == true)
                Type = 6;

            Guid WarehouseInOutID;     //���ⵥ����ⵥ��ID
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
            dr["CheckStatus"] = 1;//Ĭ��Ϊ�Ѹ���
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
                //�жϹ����Ƿ�Ϊ�����ɳ����
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
                    //�ҳ���Ӧ�ĳ�Ʒ����
                    DataSet ds = CSystem.Sys.Svr.cntMain.Select("Select ID from P_Material where MaterialType=2 and WIPID='" + drWorkOrderProduct["MaterialID"] + "'");
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        Msg.Error(string.Format("{0}û�б���Ӧ�ĳ�Ʒ���ù�ϵ��", drWorkOrderProduct["MaterialName"]));
                        return false;
                    }
                    else
                        drDetail["MaterialID"] = ds.Tables[0].Rows[0]["ID"];

                    //�ӹ�ͼ��
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
        //����������,������������������D_WorkOrderProduct.
        //���ɱ���������ʱ,�ӹ�ͼ��(��ԭ���ļӹ���׼),��"��Դͼ��"Ϊ׼,��û����Դͼ��,�Ա���ļӹ�ͼ��Ϊ׼
        bool ProcessIn(Guid WarehouseID, Guid ProcessID, Guid PositionID, Guid WorkOrderID, String WorkOrderNo, DateTime WorkOrderDate, bool IsRework, SqlConnection conn, SqlTransaction sqlTran)
        {

            string StockHeadTable;
            string StockDetailTable;

  
            StockHeadTable = "D_WarehouseIn";
            StockDetailTable = "D_WarehouseInBill";
 
            string WhereClause = string.Format("(D_WorkOrderProduct.MainID = '{0}')", WorkOrderID);
            string SQL = CSystem.Sys.Svr.LDI.GetFields("D_WorkOrderProduct").QuerySQLWithClause(WhereClause);
            DataSet DSWorkOrderProduct = CSystem.Sys.Svr.cntMain.Select(SQL, conn, sqlTran);

            int Status = 0;  //Status: 0-�½�
            int Type = 0;    //һ��  ���ڷ��޵Ĺ����� �����TypeΪ3�����Ϊ6
            
            if (IsRework == true)
                Type = 6;

            Guid WarehouseInOutID;     //���ⵥ����ⵥ��ID
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
            dr["CheckStatus"] = 1;//Ĭ��Ϊ�Ѹ���
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

                //�жϹ����Ƿ�Ϊ�����ɳ����
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


        //���ﴫ����һ������������,��Ҫ�������ж��µ�����ͱ������Ӧ�Ŀ�λ�Ƿ���ͬ�������ͬ�����ɱ�����ĳ���,��֮�����λ��ͬ�Ͳ����ɱ�����ĳ���
        bool ProcessOut(Guid WarehouseID, Guid ProcessID, Guid PositionID, Guid NextProcessID, Guid WorkOrderID, String WorkOrderNo, DateTime WorkOrderDate, bool IsRework, SqlConnection conn, SqlTransaction sqlTran)
        {
            Guid NextPositionID=CSystem.Sys.Svr.NullID;
            if (NextProcessID!=CSystem.Sys.Svr.NullID)
                NextPositionID = GetPositionID(NextProcessID);

            if (PositionID != NextPositionID)
                ProcessInOut(WarehouseID,Enum_WarehouseMovetype.WarehouseOut, ProcessID, WorkOrderID, WorkOrderNo,WorkOrderDate,PositionID,false,IsRework, conn, sqlTran);

            return true;
 
        }

        //���ﴫ����һ������������,��Ҫ�������ж��µ�����ͱ������Ӧ�Ŀ�λ�Ƿ���ͬ�������ͬ��������һ����������,��֮�����λ��ͬ�Ͳ�������һ����������
        bool NextProcessIn(Guid WarehouseID, Guid ProcessID, Guid PositionID, Guid NextProcessID, Guid WorkOrderID, String WorkOrderNo, DateTime WorkOrderDate, bool IsRework, SqlConnection conn, SqlTransaction sqlTran)
        {
            Guid NextPositionID = CSystem.Sys.Svr.NullID;

            NextPositionID = GetPositionID(NextProcessID);
            if (PositionID != NextPositionID)
                ProcessInOut(WarehouseID, Enum_WarehouseMovetype.WarehouseIn, ProcessID, WorkOrderID, WorkOrderNo, WorkOrderDate, NextPositionID, true, IsRework, conn, sqlTran);


            return true;
 
        }

        //���ݹ���ȡ��������Ӧ�Ŀ�λ
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
            //����Ҫ���Ƶ�ֵ
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
