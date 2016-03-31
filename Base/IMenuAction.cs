using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    public interface IMenuAction
    {
        System.Windows.Forms.Form GetForm(CRightItem right,System.Windows.Forms.Form mdiForm);
    }
}
