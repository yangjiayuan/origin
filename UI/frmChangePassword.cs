using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UI
{
    public partial class frmChangePassword : Form
    {
        public frmChangePassword()
        {
            InitializeComponent();
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            if (txtRetry.Text == txtNewPWD.Text)
            {
                int i = CSystem.Sys.Svr.cntMain.Excute(string.Format("Update P_User set PassWord='{0}' where ID='{1}' and Password='{2}'", txtNewPWD.Text, CSystem.Sys.Svr.User, txtOldPWD.Text));
                if (i != 1)
                    Base.Msg.Information("密码修改失败，可能密码已被其他用户修改！");
                else
                {
                    Base.Msg.Information("密码修改成功！");
                    this.DialogResult = DialogResult.Yes;
                    this.Close();
                }
            }
            else
                Base.Msg.Information("新密码与重新输入的密码不相同！");
        }
    }
}