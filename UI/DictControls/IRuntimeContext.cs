using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace UI.DictControls
{
    interface IRuntimeContext
    {
        Type GetStyleType(Type t);
        DataTable GetTableByName(string TableName);
        void SetControlStyle(Control c, string typename);
    }
}
