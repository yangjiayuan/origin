using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Xml;
namespace UI
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((!(ActiveControl is Button)) && (keyData == Keys.Enter || keyData == (Keys.Enter | Keys.Shift)))
            {
                if (keyData == Keys.Enter)
                    SendKeys.Send("{tab}");
                else
                    SendKeys.Send("+{tab}");
                return true;
            }
            else
                return base.ProcessCmdKey(ref msg, keyData); 
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            //DataSet ds = CSystem.Sys.Svr.cntMain.Select("Select A.*,B.Name DepartmentName,getdate() date from P_User A left join P_Department B on A.DepartmentID=B.ID where A.Disable=0 and A.Code = '" + txtUserCode.Text + "'");
            DataSet ds = CSystem.Sys.Svr.cntMain.Select("Select A.*,getdate() date from P_User A where A.Disable=0 and A.Code = '" + txtUserCode.Text + "'");
           
            if (ds == null) return;
            if (ds.Tables[0].Rows.Count == 1)
            {
                string p = ds.Tables[0].Rows[0]["Password"] as string;
                if (p == txtPassword.Text)
                {
                    CSystem.Sys.Svr.SystemTime = (DateTime)ds.Tables[0].Rows[0]["date"];
                    CSystem.Sys.Svr.User = (Guid)ds.Tables[0].Rows[0]["ID"];
                    CSystem.Sys.Svr.UserName = string.Format("{0}", ds.Tables[0].Rows[0]["Name"]);

                    //if (ds.Tables[0].Rows[0]["DepartmentID"]!=DBNull.Value)
                    //    CSystem.Sys.Svr.DepartmentID =(Guid) ds.Tables[0].Rows[0]["DepartmentID"];
                    //CSystem.Sys.Svr.DepartmentName = ds.Tables[0].Rows[0]["DepartmentName"] as string;

                    Guid roleID=CSystem.Sys.Svr.NullID;
                    if (ds.Tables[0].Rows[0]["RoleID"]!=DBNull.Value)
                        roleID=(Guid)ds.Tables[0].Rows[0]["RoleID"];
                    CSystem.Sys.Svr.Operator.Logon(CSystem.Sys.Svr.User,ds.Tables[0].Rows[0]["Code"] as string,CSystem.Sys.Svr.UserName,CSystem.Sys.Svr.DepartmentID,CSystem.Sys.Svr.DepartmentName,roleID);
                    this.DialogResult = DialogResult.OK;
                    this.Close();

                    //Add by Yang Jiayuan
                    if (CheckBoxRememberMe.Checked)
                    {
                        Properties.Settings.Default.UserName = txtUserCode.Text;
                        Properties.Settings.Default.Password = txtPassword.Text;
                        Properties.Settings.Default.Save();
                    }

                }
                else
                {
                    MessageBox.Show ("用户密码不正确！");
                }
            }
            else
                MessageBox.Show("用户代码不存在");
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
           
            string bg = ConfigurationManager.AppSettings["LoginBackground"];
            string color = ConfigurationManager.AppSettings["LoginColor"];
            if (color != null && color.Length > 0)
            {
                Color cl = Color.FromName(color);
                label1.ForeColor = cl;
                label2.ForeColor = cl;
            }
            if (bg != null && System.IO.File.Exists(bg))
            {
                this.BackgroundImage = Image.FromFile(bg);
                this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            }
            //Add by Yang Jiayuan
            if (CheckBoxRememberMe.Checked)
            {
                txtUserCode.Text = Properties.Settings.Default.UserName;
                txtPassword.Text = Properties.Settings.Default.Password;
            }
        }
    }
}