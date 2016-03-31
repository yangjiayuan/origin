using System;
using System.Collections.Generic;
using System.Text;
using Base;
using UI;

namespace YUANYE
{
    public class clsDictionary:IMenuAction
    {
        #region IMenuAction Members

        public System.Windows.Forms.Form GetForm(CRightItem right,System.Windows.Forms.Form mdiForm)
        {

            string MainTable = right.Code;
            MDIMain mdi = (MDIMain)mdiForm;
            if (mdi.CheckFormTag(MainTable))
            {
                frmBrowser frm=null;

                if (MainTable == "P_Operator")
                    frm = (frmBrowser)( new Operator().GetForm(right,mdiForm));
                else
                    frm = (frmBrowser)(new ToolDetailForm().GetForm(right, mdiForm));
                frm.Tag = MainTable;
                frm.ShowStatus = enuShowStatus.None;
                return frm;
            }
            return null;
        }
        #endregion
        private class Operator : ToolDetailForm
        {
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
            public override string GetPrefix()
            {
                return "S";
            }
            public override int LengthOfCode
            {
                get
                {
                    return 5;
                }
                set
                {
                    base.LengthOfCode = value;
                }
            }
            public override bool DateInCode
            {
                get
                {
                    return false;
                }
                set
                {
                    base.DateInCode = value;
                }
            }
        }
    }
}
