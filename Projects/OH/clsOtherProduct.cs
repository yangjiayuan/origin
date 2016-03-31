using System;
using System.Collections.Generic;
using System.Text;
using UI;

namespace OH
{
    public class clsOtherProduct:ToolDetailForm
    {
        public override void SetCreateGrid(CCreateGrid createGrid)
        {
            createGrid.BeforeSelectForm += new BeforeSelectFormEventHandler(createGrid_BeforeSelectForm);
            base.SetCreateGrid(createGrid);
        }

        void createGrid_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            switch (e.Field.FieldName)
            {
                case "MaterialName":
                    e.Where = "P_Material.MaterialType=1";
                    break;
                case "MachiningStandardName":
                    CCreateGrid createGrid = (CCreateGrid)sender;
                    if (createGrid.Grid.ActiveRow.Cells["MaterialID"].Value != DBNull.Value)
                    {
                        e.Where = "P_MachiningStandard.MaterialID='" + createGrid.Grid.ActiveRow.Cells["MaterialID"].Value + "'";
                    }
                    break;
            }
        }
    }
}
