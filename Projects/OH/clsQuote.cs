using System;
using System.Collections.Generic;
using System.Text;
using UI;
using Base;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

namespace OH
{
    public class clsQuote:ToolDetailForm
    {
        private Guid _CustomerID;
        private string _CustomerName;
        private Guid _QuoteVersionID;
        private string _QuoteVersionNote;
        private bool _NewVerion = false;
        private DateTime _StartDate;
        private DateTime _EndDate;

        public Guid CustomerID
        {
            set { _CustomerID = value; }
        }
        public Guid QuoteVersionID
        {
            set { _QuoteVersionID = value; }
        }
        public string CustomerName
        {
            set { _CustomerName = value; }
        }
        public string QuoteVersionNote
        {
            set { _QuoteVersionNote = value; }
        }
        public bool NewVerion
        {
            set { _NewVerion = value; }
        }
        public DateTime StartDate
        {
            set { _StartDate = value; }
        }
        public DateTime EndDate
        {
            set { _EndDate = value; }
        }

        public override Form GetForm(CRightItem right, Form mdiForm)
        {

            frmBrowser frmCustomer = new frmBrowser(CSystem.Sys.Svr.LDI.GetFields("P_Customer"), null, "P_Customer.Disable=0", enuShowStatus.None);
            DataRow dr = frmCustomer.ShowSelect(null, null, null);
            if (dr != null)
            {                
                _CustomerID = (Guid)dr["ID"];
                _CustomerName = (string)dr["Name"];
                if (dr["QuoteVersionID"] is DBNull || dr["QuoteVersionID"] == null || dr["QuoteVersion"] is DBNull || dr["QuoteVersion"] ==null)
                {
                    Msg.Error("当前客户没有设置默认版本");
                    return null;
                }
                this.MainTableName = "P_Quote";
                _QuoteVersionID = (Guid)dr["QuoteVersionID"];
                _QuoteVersionNote = (string)dr["QuoteVersion"];
                SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
                SortedList<string, object> Value = new SortedList<string, object>();
                Value.Add("QuoteVersionNotes", _QuoteVersionNote);
                Value.Add("QuoteVersionID", _QuoteVersionID);
                defaultValue.Add(this.MainTableName, Value);
                frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
                frm.DefaultValue = defaultValue;
                if (_QuoteVersionID == Guid.Empty)
                    frm.Where = "0=1";
                else
                    frm.Where = string.Format("P_Quote.QuoteVersionID='{0}'", _QuoteVersionID);
                string text = string.Format("{0}[{1}]", "报价图号", _CustomerName);
                frm.Text = text;
                _MainCOMFields.Property.Title = text;
                return frm;
            }


            return null;
        }

        public override void SetCreateGrid(CCreateGrid createGrid)
        {
            base.SetCreateGrid(createGrid);

            createGrid.AfterSelectForm += new AfterSelectFormEventHandler(createGrid_AfterSelectForm);
            createGrid.BeforeSelectForm += new BeforeSelectFormEventHandler(createGrid_BeforeSelectForm);
        }

        public override void AfterDataBind()
        {
            UltraGrid grid = _GridMap["P_QuoteBill"];
            DataTable dt = _DetailForm.MainDataSet.Tables["P_QuoteBill"];
            Guid dtlId;
            foreach (UltraGridRow row in grid.Rows)
            {
                if ((int)row.Cells["CheckMachiningStandard"].Value == 1)
                {
                    row.Cells["MachiningStandard"].Activation = Activation.AllowEdit;
                }
                else
                {
                    row.Cells["MachiningStandard"].Activation = Activation.NoEdit;
                }
                if (VarConverTo.ConvertToBoolean(row.Cells["NeedLength"].Value))
                    row.Cells["Length"].Activation = Activation.AllowEdit;
                else
                    row.Cells["Length"].Activation = Activation.NoEdit;
                if (VarConverTo.ConvertToInt(row.Cells["PriceType"].Value) == 2)
                {
                    row.Cells["formula"].Activation = Activation.AllowEdit;
                }
                else
                {
                    row.Cells["formula"].Activation = Activation.NoEdit;
                }
                //dtlId = (Guid) row.Cells["Id"].Value;
                //DataRow[] rows = dt.Select("ID='" + dtlId + "'");
                //if (rows.Length > 0)
                //{
                //    if ((int)rows[0]["CheckMachiningStandard"] == 1)
                //    {
                //        row.Cells["MachiningStandard"].Activation = Activation.AllowEdit;
                //    }
                //    else
                //    {
                //        row.Cells["MachiningStandard"].Activation = Activation.NoEdit;
                //    }
                //    if (VarConverTo.ConvertToBoolean(rows[0]["NeedLength"]))
                //        row.Cells["Length"].Activation = Activation.AllowEdit;
                //    else
                //        row.Cells["Length"].Activation = Activation.NoEdit;
                //    if (VarConverTo.ConvertToInt(rows[0]["PriceType"]) == 2)
                //    {
                //        row.Cells["formula"].Activation = Activation.AllowEdit;
                //    }
                //    else
                //    {
                //        row.Cells["formula"].Activation = Activation.NoEdit;
                //    }
                //}
            }
        }

        void createGrid_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            if (e.Field.FieldName == "MaterialName")
            {
                e.Where = string.Format("P_Material.MaterialType=2 or P_Material.MaterialType=5 ");
            }
            else if (e.Field.FieldName == "MachiningStandard" && e.GridRow.Cells["MaterialID"].Value != DBNull.Value)
            {
                e.Where = string.Format("P_MachiningStandard.MaterialID in (select A.WIPID from p_Material A where A.ID='{0}') and (P_MachiningStandard.CustomerID  is null or P_MachiningStandard.CustomerID='{1}')", e.GridRow.Cells["MaterialID"].Value,_CustomerID );
            }
        }
        public override void NewGrid(COMFields fields, UltraGrid grid)
        {
            grid.AfterCellUpdate += new CellEventHandler(grid_AfterCellUpdate);
            grid.DisplayLayout.Bands[0].Columns["Price"].MaskInput = "{LOC}-nnnnnnnnnn.nnnn";
            grid.DisplayLayout.Bands[0].Columns["Price"].Format = "#,##0.0000";

        }
        private void grid_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            UltraGrid grid = _GridMap["P_QuoteBill"];
            if (e.Cell.Column.Key == "PriceType")
            {
                if (VarConverTo.ConvertToInt(e.Cell.Value) == 2)
                {
                    grid.ActiveRow.Cells["formula"].Activation = Activation.AllowEdit;              
                    grid.ActiveRow.Cells["formula"].IsInEditMode = true;
                }
                else
                {
                    grid.ActiveRow.Cells["formula"].Activation = Activation.NoEdit;
                }
            }
            //if (e.Cell.Column.Key == "MaterialID")
            //{
            //    if (VarConverTo.ConvertToBoolean(e.GridRow.Cells["NeedLenght"].Value))
            //    {
            //        grid.ActiveRow.Cells["Length"].Activation = Activation.AllowEdit;
            //        grid.ActiveRow.Cells["Length"].IsInEditMode = true;
            //    }
            //    else
            //    {
            //        grid.ActiveRow.Cells["Length"].Activation = Activation.NoEdit;
            //    }
            //}
        }
        void createGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {
            if ((int)e.GridRow.Cells["CheckMachiningStandard"].Value == 1)
            {
                e.GridRow.Cells["MachiningStandard"].Activation = Activation.AllowEdit;
            }
            else
            {
                e.GridRow.Cells["MachiningStandard"].Activation = Activation.NoEdit;
            }
            if (VarConverTo.ConvertToBoolean(e.GridRow.Cells["NeedLength"].Value))
                e.GridRow.Cells["Length"].Activation = Activation.AllowEdit;
            else
                e.GridRow.Cells["Length"].Activation = Activation.NoEdit;
            if (VarConverTo.ConvertToInt(e.GridRow.Cells["PriceType"].Value) == 2)
            {
                e.GridRow.Cells["formula"].Activation = Activation.AllowEdit;
            }
            else
            {
                e.GridRow.Cells["formula"].Activation = Activation.NoEdit;
            }
        }
        public override string BeforeSaving(DataSet ds, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction tran)
        {

            //if (_NewVerion)
            //{
            //    _QuoteVersionID = Guid.NewGuid();
            //    //ds.Tables["P_Quote"].Rows[0]["QuoteVersion"] = _QuoteVersionNote;
            //}
            ds.Tables["P_Quote"].Rows[0]["QuoteVersionID"] = _QuoteVersionID;
            ds.Tables["P_Quote"].Rows[0]["QuoteVersionNotes"] = _QuoteVersionNote;
            ds.Tables["P_Quote"].Rows[0]["Name"] = ds.Tables["P_Quote"].Rows[0]["Code"];
            int type = 0;
            String error = "";
            for (int i=0 ;i< ds.Tables["P_QuoteBill"].Rows.Count;i++)
            {
                int isInput =VarConverTo.ConvertToInt(ds.Tables["P_QuoteBill"].Rows[i]["CheckMachiningStandard"]);
                String MachiningStandard=VarConverTo.ConvertToString(ds.Tables["P_QuoteBill"].Rows[i]["MachiningStandard"]);
                type = VarConverTo.ConvertToInt(ds.Tables["P_QuoteBill"].Rows[i]["PriceType"]);
                if(type==1)
                    ds.Tables["P_Quote"].Rows[0]["IsLength"] = 1;
                if (isInput == 1&&MachiningStandard.Length==0)
                {                    
                    error += (i+1).ToString();
                    if (i < ds.Tables["P_QuoteBill"].Rows.Count - 1)
                    {
                        error += ",";
                    }
                }
                if (VarConverTo.ConvertToBoolean(ds.Tables["P_QuoteBill"].Rows[i]["NeedLength"]))
                {
                    if(VarConverTo.ConvertToInt(ds.Tables["P_QuoteBill"].Rows[i]["Length"])<=0)
                        return "第" + (i + 1).ToString() +"行的物料的长度是必填项，请检查";
                }
            }
            if (error.Length > 0)
            {
                return "第" + error + "行[加工图号]是必填项，请检查";
            }
            
            return base.BeforeSaving(ds, conn, tran);
        }
        public override void AfterSaved(DataSet ds, System.Data.SqlClient.SqlConnection conn, System.Data.SqlClient.SqlTransaction tran)
        {
            base.AfterSaved(ds, conn, tran);
            DataSet dsQuote = CSystem.Sys.Svr.cntMain.Select(string.Format("select * from P_QuoteVersion where MainID='{0}'", _CustomerID), "P_QuoteVersion", conn, tran);
            DataRow dr = dsQuote.Tables[0].NewRow();
            dr["ID"] = _QuoteVersionID;
            dr["MainID"] = _CustomerID;
            dr["LineNumber"] = dsQuote.Tables[0].Rows.Count + 1;
            dr["Notes"] = _QuoteVersionNote;
            dr["StartDate"] = _StartDate;
            dr["EndDate"] = _EndDate;
            dsQuote.Tables[0].Rows.Add(dr);
            CSystem.Sys.Svr.cntMain.Update(dsQuote.Tables[0], conn, tran);

            //保存版本
            //if (_NewVerion)
            //{
            //    DataSet dsQuote = CSystem.Sys.Svr.cntMain.Select(string.Format("select * from P_QuoteVersion where MainID='{0}'", _CustomerID),"P_QuoteVersion", conn, tran);
            //    DataRow dr = dsQuote.Tables[0].NewRow();
            //    dr["ID"] = _QuoteVersionID;
            //    dr["MainID"]=_CustomerID;
            //    dr["LineNumber"] = dsQuote.Tables[0].Rows.Count + 1;
            //    dr["Notes"] = _QuoteVersionNote;

            //    dsQuote.Tables[0].Rows.Add(dr);
            //    CSystem.Sys.Svr.cntMain.Update(dsQuote.Tables[0], conn, tran);
            //    _NewVerion = false;
            //}
        }
       
    }
}
