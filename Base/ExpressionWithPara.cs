using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    public class ExpressionWithPara
    {
        private CExpression cExp;
        private decimal[] p;
        public ExpressionWithPara(decimal[] p)
        {
            this.p = p;
            cExp = new CExpression();
            cExp.UserFunction += new CExpression.UserFunctionEventHandler(exp_UserFunction);
        }

        void exp_UserFunction(string Name, CStringStack Arguments, ref string Value)
        {
            if (string.Compare(Name, "p", true) == 0)
            {
                string s = "";
                Arguments.Pop(ref s);
                int i = int.Parse(s);
                i--;
                if (i > p.Length - 1)
                    Value = "0";
                else
                    Value = p[i].ToString();
            }
        }
        public decimal Evaluate(string exp)
        {
            string value = "";
            string msg = "";
            int err = 0;
            if (cExp.Evaluate(exp, ref value, ref msg, ref err) == 0)
            {
                try
                {
                    return decimal.Parse(value);
                }
                catch
                {
                    return 0;
                }
            }
            else
                return 0;
        }
        public string Check(string exp)
        {
            string value = "";
            string msg = "";
            int err = 0;
            if (cExp.Evaluate(exp, ref value, ref msg, ref err) == 0)
            {
                return "";
            }
            else
                return msg;
        }
    }
}
