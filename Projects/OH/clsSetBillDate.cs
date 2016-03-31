using System;
using System.Collections.Generic;
using System.Text;
using Base;
using UI;

namespace OH
{
    public class clsSetBillDate : ToolDetailForm
    {
        #region IMenuAction Members

        public override System.Windows.Forms.Form  GetForm(CRightItem right, System.Windows.Forms.Form mdiForm)
        {
            frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm);
            this.AutoCode = true;
            SortedList<string, SortedList<string, object>> defaultValue = new SortedList<string, SortedList<string, object>>();
            SortedList<string, object> Value = new SortedList<string, object>();
            Value.Add("BillDate", CSystem.Sys.Svr.SystemTime.Date);
            defaultValue.Add(this.MainTableName, Value);
            frm.DefaultValue = defaultValue;
            return frm;
        }

        #endregion
    }
}
