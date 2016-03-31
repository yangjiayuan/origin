using System;
using System.Collections.Generic;
using System.Text;
using Base;
using UI;

namespace OH
{
    public class clsDataList:IMenuAction
    {
        #region IMenuAction Members

        public System.Windows.Forms.Form GetForm(CRightItem right, System.Windows.Forms.Form mdiForm)
        {
            string tableName = right.Code;
            frmDataList frm = new frmDataList(CSystem.Sys.Svr.LDI.GetFields(tableName));
            frm.BeforeSelectForm += new BeforeSelectFormEventHandler(frm_BeforeSelectForm);
            frm.Text = right.Name;
            return frm;
        }

        void frm_BeforeSelectForm(object sender, BeforeSelectFormEventArgs e)
        {
            if (e.Field.FieldName == "MaterialName")
            {
                e.Where = "P_Material.MaterialType=1";
            }
            //else if (e.Field.FieldName == "OperatorName")
            //{
            //    CCreateGrid createGrid = (CCreateGrid)sender;
            //    if (createGrid.Grid.ActiveRow != null && createGrid.Grid.ActiveRow.Cells["MachineID"].Value != DBNull.Value)
            //    {
            //        e.Where = "P_Operator.MachineID='" + createGrid.Grid.ActiveRow.Cells["MachineID"].Value + "'";
            //    }
            //}
        }

        #endregion
    }
}
