using System;
using System.Collections.Generic;
using System.Text;

namespace OH
{
    interface IDataImport
    {
        bool Import(string fileName, Guid customerID);
    }
}
