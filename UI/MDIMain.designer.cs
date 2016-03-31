namespace UI
{
    partial class MDIMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MDIMain));
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.SystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.P_User = new System.Windows.Forms.ToolStripMenuItem();
            this.P_Role = new System.Windows.Forms.ToolStripMenuItem();
            this.toolChangePWD = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripPrintTemplate = new System.Windows.Forms.ToolStripMenuItem();
            this.ReportBuilderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.tabbedMdiManager = new Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager(this.components);
            this.toolStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStatusUser = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainStatus = new System.Windows.Forms.StatusStrip();
            this.toolStatusServer = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStatusClient = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStripMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabbedMdiManager)).BeginInit();
            this.MainStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.BackColor = System.Drawing.SystemColors.MenuBar;
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SystemToolStripMenuItem,
            this.HelpToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStripMain.Size = new System.Drawing.Size(1115, 24);
            this.menuStripMain.TabIndex = 13;
            this.menuStripMain.Text = "menuStripMain";
            // 
            // SystemToolStripMenuItem
            // 
            this.SystemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.P_User,
            this.P_Role,
            this.toolChangePWD,
            this.toolStripSeparator1,
            this.ExitToolStripMenuItem});
            this.SystemToolStripMenuItem.Name = "SystemToolStripMenuItem";
            this.SystemToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.SystemToolStripMenuItem.Tag = "1000";
            this.SystemToolStripMenuItem.Text = "系统";
            // 
            // P_User
            // 
            this.P_User.Name = "P_User";
            this.P_User.Size = new System.Drawing.Size(122, 22);
            this.P_User.Tag = "\"100001\"";
            this.P_User.Text = "操作员";
            this.P_User.Click += new System.EventHandler(this.OperatorToolStripMenuItem_Click);
            // 
            // P_Role
            // 
            this.P_Role.Name = "P_Role";
            this.P_Role.Size = new System.Drawing.Size(122, 22);
            this.P_Role.Tag = "100002";
            this.P_Role.Text = "权限设置";
            this.P_Role.Click += new System.EventHandler(this.AuthToolStripMenuItem_Click);
            // 
            // toolChangePWD
            // 
            this.toolChangePWD.Name = "toolChangePWD";
            this.toolChangePWD.Size = new System.Drawing.Size(122, 22);
            this.toolChangePWD.Tag = "100003";
            this.toolChangePWD.Text = "修改密码";
            this.toolChangePWD.Click += new System.EventHandler(this.toolChangePWD_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(119, 6);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.ExitToolStripMenuItem.Tag = "100004";
            this.ExitToolStripMenuItem.Text = "系统退出";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // HelpToolStripMenuItem
            // 
            this.HelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AboutToolStripMenuItem,
            this.toolStripPrintTemplate,
            this.ReportBuilderToolStripMenuItem,
            this.toolStripMenuItem1});
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            this.HelpToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.HelpToolStripMenuItem.Text = "帮助";
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.AboutToolStripMenuItem.Text = "关于...";
            this.AboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // toolStripPrintTemplate
            // 
            this.toolStripPrintTemplate.Name = "toolStripPrintTemplate";
            this.toolStripPrintTemplate.Size = new System.Drawing.Size(158, 22);
            this.toolStripPrintTemplate.Text = "打印模板设计器";
            this.toolStripPrintTemplate.Click += new System.EventHandler(this.toolStripPrintTemplate_Click);
            // 
            // ReportBuilderToolStripMenuItem
            // 
            this.ReportBuilderToolStripMenuItem.Name = "ReportBuilderToolStripMenuItem";
            this.ReportBuilderToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.ReportBuilderToolStripMenuItem.Text = "报表设计器";
            this.ReportBuilderToolStripMenuItem.Click += new System.EventHandler(this.ReportBuilderToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(158, 22);
            this.toolStripMenuItem1.Text = "打印测试";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // timerMain
            // 
            this.timerMain.Enabled = true;
            this.timerMain.Interval = 60000;
            this.timerMain.Tick += new System.EventHandler(this.timerMain_Tick);
            // 
            // tabbedMdiManager
            // 
            this.tabbedMdiManager.MdiParent = this;
            this.tabbedMdiManager.TabSettings.AllowClose = Infragistics.Win.DefaultableBoolean.True;
            this.tabbedMdiManager.TabSettings.AllowDrag = Infragistics.Win.UltraWinTabbedMdi.MdiTabDragStyle.WithinGroup;
            this.tabbedMdiManager.TabSettings.DisplayFormIcon = Infragistics.Win.DefaultableBoolean.False;
            this.tabbedMdiManager.TabSettings.HotTrack = Infragistics.Win.DefaultableBoolean.False;
            this.tabbedMdiManager.TabSettings.TabCloseAction = Infragistics.Win.UltraWinTabbedMdi.MdiTabCloseAction.Close;
            this.tabbedMdiManager.ViewStyle = Infragistics.Win.UltraWinTabbedMdi.ViewStyle.Office2007;
            // 
            // toolStatus
            // 
            this.toolStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.toolStatus.Name = "toolStatus";
            this.toolStatus.Size = new System.Drawing.Size(938, 19);
            this.toolStatus.Spring = true;
            this.toolStatus.Text = "就绪";
            this.toolStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStatusUser
            // 
            this.toolStatusUser.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStatusUser.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.toolStatusUser.Name = "toolStatusUser";
            this.toolStatusUser.Size = new System.Drawing.Size(35, 19);
            this.toolStatusUser.Text = "用户";
            this.toolStatusUser.ToolTipText = "用户";
            // 
            // MainStatus
            // 
            this.MainStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStatus,
            this.toolStatusServer,
            this.toolStatusClient,
            this.toolStatusUser});
            this.MainStatus.Location = new System.Drawing.Point(0, 546);
            this.MainStatus.Name = "MainStatus";
            this.MainStatus.Size = new System.Drawing.Size(1115, 24);
            this.MainStatus.TabIndex = 15;
            this.MainStatus.Text = "statusStrip1";
            this.MainStatus.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MainStatus_ItemClicked);
            // 
            // toolStatusServer
            // 
            this.toolStatusServer.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.toolStatusServer.Name = "toolStatusServer";
            this.toolStatusServer.Size = new System.Drawing.Size(43, 19);
            this.toolStatusServer.Text = "Server";
            // 
            // toolStatusClient
            // 
            this.toolStatusClient.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.toolStatusClient.Name = "toolStatusClient";
            this.toolStatusClient.Size = new System.Drawing.Size(53, 19);
            this.toolStatusClient.Text = "客户端: ";
            // 
            // MDIMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1115, 570);
            this.Controls.Add(this.MainStatus);
            this.Controls.Add(this.menuStripMain);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStripMain;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MDIMain";
            this.Text = "MDIMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MDIMain_FormClosing);
            this.Load += new System.EventHandler(this.MDIMain_Load);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabbedMdiManager)).EndInit();
            this.MainStatus.ResumeLayout(false);
            this.MainStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem SystemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem P_User;
        private System.Windows.Forms.ToolStripMenuItem P_Role;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
        private Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager tabbedMdiManager;
        private System.Windows.Forms.Timer timerMain;
        private System.Windows.Forms.ToolStripMenuItem toolChangePWD;
        private System.Windows.Forms.ToolStripMenuItem toolStripPrintTemplate;
        private System.Windows.Forms.StatusStrip MainStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStatusUser;
        private System.Windows.Forms.ToolStripMenuItem ReportBuilderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripStatusLabel toolStatusServer;
        private System.Windows.Forms.ToolStripStatusLabel toolStatusClient;

    }
}
