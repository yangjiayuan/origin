using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    public class VarConverTo
    {
        static public Decimal ConvertToDecimal(Object var){
            if (var == null || Convert.IsDBNull(var))
            {
                return new decimal(0);
            }
            else
            {
                return Convert.ToDecimal(var);
            }
        }
        static public int ConvertToInt(Object var)
        {
            if (var == null || Convert.IsDBNull(var))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt16(var);
            }
        }
        static public String ConvertToString(Object var)
        {
            if (var == null || Convert.IsDBNull(var))
            {
                return "";
            }
            else
            {
                return Convert.ToString(var);
            }
        }
        static public bool ConvertToBoolean(Object var)
        {
            if (var == null || Convert.IsDBNull(var))
            {
                return false;
            }
            else
            {
                return Convert.ToBoolean(var);
            }
        }
    }
}
