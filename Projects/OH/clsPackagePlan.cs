using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using System.Windows.Forms;
using Base;
using UI;
using System.Data.SqlClient;
using System.Data;

namespace OH
{
    public class clsPackagePlan : ToolDetailForm
    {
        private ToolStripButton toolFinish;
        public override bool AllowDeleteRowInToolBar
        {
            get
            {
                return true;
            }
        }

        public override bool AllowInsertRowInToolBar
        {
            get
            {
                return false;
            }
        }

        public override bool AutoCode
        {
            get
            {
                return true;
            }
        }
        public override void InsertToolStrip(System.Windows.Forms.ToolStrip toolStrip,ToolDetailForm.enuInsertToolStripType insertType)
        {
            //base.InsertToolStrip(toolStrip,insertType);
            int i = toolStrip.Items.Count;

            if (insertType == enuInsertToolStripType.Detail)
            {
                toolFinish = new ToolStripButton();
                toolFinish.Font = new System.Drawing.Font("SimSun", 10.5F);
                toolFinish.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolFinish.Name = "toolSplit";

                toolFinish.Text = "完成";
                toolFinish.Image = clsResources.Finish;
                //toolFinish.Image = clsResources.Finish;
                toolFinish.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
                toolFinish.Click += new EventHandler(toolFinish_Click);

                toolStrip.Items.Insert(i - 2, toolFinish);
            }
        }
        void toolFinish_Click(object sender, EventArgs e)
        {
            if (_DetailForm.IsChecked)
            {
                using (SqlConnection conn = new SqlConnection(CSystem.Sys.Svr.cntMain.ConnectionString))
                {
                    SqlTransaction sqlTran = null;
                    try
                    {
                        conn.Open();
                        sqlTran = conn.BeginTransaction();
                        DataRow dr = _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0];
                        Guid id = (Guid)dr["ID"];
                        DataSet ds = CSystem.Sys.Svr.cntMain.Select(string.Format("Select Finished from {0} where ID='{1}'",_DetailForm.MainTable, id));
                        if (ds.Tables[0].Rows.Count == 1 && ds.Tables[0].Rows[0]["Finished"].Equals(dr["Finished"]))
                        {
                            if ((int)dr["Finished"] == 0)
                            {
                                if (Msg.Question("是否将当前包装计划手工指定为完成状态?") != DialogResult.Yes)
                                    return;
                                dr["Finished"] = 1;
                                CSystem.Sys.Svr.cntMain.Update(_DetailForm.MainDataSet.Tables[_DetailForm.MainTable]);
                                sqlTran.Commit();
                                _DetailForm.ChangeData();
                            }
                            else
                            {
                                Msg.Information("当前包装计划已完成！");
                            }
                        }
                        else
                            Msg.Warning("当前单据已被修改,请重新打开，再试！");
                        //RefreshCheck();
                    }
                    catch (Exception ex)
                    {
                        Msg.Warning(ex.ToString());
                        if (sqlTran != null)
                            sqlTran.Rollback();
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        public override void SetCreateGrid(CCreateGrid createGrid)
        {
            base.SetCreateGrid(createGrid);
            createGrid.AfterSelectForm += new AfterSelectFormEventHandler(_CreateGrid_AfterSelectForm);
            createGrid.BeforeSelectForm += new BeforeSelectFormEventHandler(_CreateGrid_BeforeSelectForm);
            createGrid.Grid.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(Grid_AfterCellUpdate);
        }

        void Grid_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "Quantity1":
                    if ((int)e.Cell.Row.Cells["QuantityCheckMethod"].Value == 1)
                    {
                        decimal length = 1;
                        if (e.Cell.Row.Cells["Length"].Activation == Activation.AllowEdit && e.Cell.Row.Cells["Length"].Value != DBNull.Value)
                            length = (decimal)e.Cell.Row.Cells["Length"].Value;
                        decimal conv = (decimal)e.Cell.Row.Cells["ConvertQuotiety"].Value;
                        if (conv == 0)
                            conv = 1;
                        e.Cell.Row.Cells["Quantity2"].Value = Math.Round((decimal)e.Cell.Value * length * conv, 4);
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
                        e.Cell.Row.Cells["Quantity1"].Value = Math.Round((decimal)e.Cell.Value * length * conv, 4);
                    }
                    break;
            }
        }
        void _CreateGrid_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            CCreateGrid createdGrid = sender as CCreateGrid;
            string[] s = e.Field.ValueType.Split(':');
            switch (s[1])
            {
                case "P_MachiningStandard":
                    e.Where = "P_MachiningStandard.MaterialID in (Select WIPID from P_Material where ID='" + createdGrid.Grid.ActiveRow.Cells["MaterialID"].Value + "')";
                    break;
                case "P_Material":
                    e.Where = "P_Material.MaterialType=" + 2;
                    break;
            }
        }
        void _CreateGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {
            CCreateGrid createGrid = (CCreateGrid)sender;
            if (e.Field.FieldName == "MaterialCode" || e.Field.FieldName == "MaterialName")
            {
                if ((int)e.Row["NeedLength"] == 1)
                    createGrid.Grid.ActiveRow.Cells["Length"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
                else
                    createGrid.Grid.ActiveRow.Cells["Length"].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            }
        }
        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
            SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
            SortedList<string, object> Value = new SortedList<string, object>();
            Value.Add("BillDate", CSystem.Sys.Svr.SystemTime.Date);
            Value.Add("ConsignDate", CSystem.Sys.Svr.SystemTime.Date);
            defaultValue.Add("D_PackagePlan", Value);
            frm.DefaultValue = defaultValue;
            return frm;
        }

    }
}
