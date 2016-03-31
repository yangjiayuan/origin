using System;
using System.Collections.Generic;
using System.Text;
using UI;

namespace OH
{
    public class clsPrice:ToolDetailForm
    {
        public override bool AllowCheck
        {
            get
            {
                return false;
            }
        }
        public override bool AllowDeleteRowInToolBar
        {
            get
            {
                return true;
            }
        }
        public override void SetCreateDetail(CCreateDetailForm createDetailForm)
        {
            createDetailForm.BeforeSelectForm += new BeforeSelectFormEventHandler(createDetailForm_BeforeSelectForm);
        }

        void createDetailForm_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            if (e.Field.FieldName == "PositionName")
            {
                string sql = "";
                if (_DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["WarehouseID"] != null && _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["WarehouseID"] != DBNull.Value)
                    sql = string.Format("P_Position.WarehouseID='{0}' and ", _DetailForm.MainDataSet.Tables[_DetailForm.MainTable].Rows[0]["WarehouseID"]);
                e.Where = string.Format("{0} P_Position.ID not in (Select PositionID from C_Price)", sql);
            }
            else if (e.Field.FieldName == "WarehouseName")
            {
                e.Where = "P_Warehouse.WarehouseType=4";//∏®¡œ≤÷ø‚
            }
        }

        public override void SetCreateGrid(CCreateGrid createGrid)
        {
            createGrid.BeforeSelectForm += new BeforeSelectFormEventHandler(createGrid_BeforeSelectForm);
        }

        void createGrid_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            if (e.Field.FieldName.StartsWith("Material"))
            {
                e.Where = "P_Material.MaterialType=4";
            }
        }
        public override bool AutoCode
        {
            get
            {
                return true;
            }
            set
            {
                base.AutoCode = value;
            }
        }
        
    }
}
