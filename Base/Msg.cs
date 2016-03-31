using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Base
{
    public class Msg
    {
        public const string Caption = "进销存管理系统";
        public static void Information(string text)
        {
            MessageBox.Show(text, Caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void Warning(string text)
        {
            MessageBox.Show(text, Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void Error(string text)
        {
            MessageBox.Show(text, Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static DialogResult Question(string text)
        {
            return MessageBox.Show(text, Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
        public static DialogResult Question(string text, MessageBoxButtons button)
        {
            return MessageBox.Show(text, Caption, button, MessageBoxIcon.Question);
        }
    }
}
