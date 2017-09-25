using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    public interface IMenuAction
    {
        System.Windows.Forms.Form GetForm(CRightItem right,System.Windows.Forms.Form mdiForm);
    }

    public interface ITransaction
    {
        System.Windows.Forms.Form GetForm(string TransactionName,System.Windows.Forms.Form mdiForm);
    }
}
