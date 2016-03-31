using System;
using System.Collections.Generic;
using System.Text;

namespace YUANYE
{
    interface IDataImport
    {
        bool Import(string fileName, Guid customerID);
    }
}
