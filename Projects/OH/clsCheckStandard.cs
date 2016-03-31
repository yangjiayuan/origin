using System;
using System.Collections.Generic;
using System.Text;
using UI;

namespace OH
{
    public class clsCheckStandard:ToolDetailForm
    {
        public override void SetCreateGrid(CCreateGrid createGrid)
        {
            createGrid.BeforeSelectForm += new BeforeSelectFormEventHandler(createGrid_BeforeSelectForm);
            createGrid.AfterSelectForm += new AfterSelectFormEventHandler(createGrid_AfterSelectForm);
        }

        void createGrid_AfterSelectForm(object sender, AfterSelectFormEventArgs e)
        {
            if (e.Field.FieldName == "MaterialCheckObjectName")
            {
                if ((int)e.Row["IsCheckAmount"] == 0)
                {
                    e.GridRow.Cells["CheckObjectMeasureID"].Value = e.Row["MeasureID"];
                    e.GridRow.Cells["CheckObjectMeasureName"].Value = e.Row["MeasureName"];
                }
            }
            else if (e.Field.FieldName == "MachineName")
            {
                if (e.GridRow.Cells["ProcessID"].Value != e.Row["ProcessID"])
                {
                    e.GridRow.Cells["ProcessID"].Value = e.Row["ProcessID"];
                    e.GridRow.Cells["ProcessName"].Value = e.Row["ProcessName"];
                }
            }
        }

        void createGrid_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            if (e.Field.FieldName=="MachineName")
            {
                CCreateGrid createGrid = (CCreateGrid)sender;
                if (createGrid.Grid.ActiveRow.Cells["ProcessID"].Value != DBNull.Value)
                {
                    e.Where = "P_Machine.ProcessID='" + createGrid.Grid.ActiveRow.Cells["ProcessID"].Value + "'";
                }
            }
        }

    }
}
