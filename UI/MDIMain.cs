using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Base;
using System.Configuration;
using System.Diagnostics;
using UI.Print;
using UI.Print.DSStorageInTableAdapters;

namespace UI
{
    public partial class MDIMain : Form
    {
        public MDIMain()
        {
            InitializeComponent();
            translate();
            LoadCustomMenu();
        }

        //Grid数据转换错误
        private void translate()
        {
            
            //Grid数据转换错误
            Infragistics.Win.UltraWinGrid.Resources.Customizer.SetCustomizedString("DataErrorCellUpdateUnableToUpdateValue", "您输入的数据格式不正确，请检查。\r\n错号信息：{0}");
            Infragistics.Win.UltraWinGrid.Resources.Customizer.SetCustomizedString("MultiCellOperation_Error_ConversionError", "您输入的数据格式不正确，请检查。\r\n错号信息：不能转换数据 '{0}' 到类型:{1}");
            Infragistics.Win.UltraWinGrid.Resources.Customizer.SetCustomizedString("DeleteMultipleRowsPrompt", "确定要删除指定行的数据吗?");
            Infragistics.Win.UltraWinGrid.Resources.Customizer.SetCustomizedString("DeleteRowsMessageTitle", ConfigurationManager.AppSettings["MainTitle"]);

            //ContextMenu的汉化
            Infragistics.Win.Resources.Customizer.SetCustomizedString("EditContextMenuCut", "剪切(&T)");
            Infragistics.Win.Resources.Customizer.SetCustomizedString("EditContextMenuCopy", "复制(&C)");
            Infragistics.Win.Resources.Customizer.SetCustomizedString("EditContextMenuPaste", "粘贴(&P)");
            Infragistics.Win.Resources.Customizer.SetCustomizedString("EditContextMenuSelectAll", "全选(&A)");

            //打印预览
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_DialogCaption", "打印预览");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Print", "打印(&P)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_ClosePreview", "关闭(&C)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_ContextMenuPreviewZoom", "显示比例");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Current_Page", "当前页");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Exit", "退出(&X)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_First_Page", "第一页");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Go_To", "跳至");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Last_Page", "最后一页");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Next_Page", "下一页");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Previous_Page", "前一页");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Next_View", "下一视图(&N)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Previous_View", "前一视图(&P)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Hand_Tool", "手型工具(&H)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Page_Setup", "页面设置(&U)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Snapshot_Tool", "快照工具(&S)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_View", "视图(&V)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Whole_Page", "合适页");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Zoom", "缩放(&Z)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Zoom_In", "放大");

            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Zoom_Out", "缩小");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolCategory_Context_Menus", "上下文菜单");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolCategory_File", "文件");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolCategory_Menus", "菜单");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolCategory_Tools", "工具栏");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolCategory_View", "视图");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolCategory_Zoom_Mode", "缩放模式");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolTip_ClosePreview", "关闭");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolTip_Zoom", "缩放");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_Page_X_OF_X", "页:{0}/{1}");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("CustomizeImg_ToolBar_MenuBar", "菜单");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("CustomizeImg_ToolBar_Standard", "标准");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("CustomizeImg_ToolBar_View", "视图");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_File", "文件(&F)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Tools", "工具(&T)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Dynamic_Zoom_Tool", "动态缩放工具(&D)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Zoom_Out_Tool", "缩小工具");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Zoom_In_Tool", "放大工具");
            // Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Page_Layout","菜单"); 
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PreviewRowColSelection_Cancel", "取消");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PreviewRowColSelection_SelectedPages", "{0} x {1} 页");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PreviewRowColSelection_Cancel", "取消");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Page_Width", "页宽");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ZoomListItem_MarginWidth", "文字宽度");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ZoomListItem_PageWidth", "页宽");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ZoomListItem_WholePage", "合适页");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Page_Layout", "页面布局");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Margin_Width", "文字宽度");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("ContextMenuPreviewHand", "缩放视图");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Reduce_Page_Thumbnails", "缩小");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Show_Page_Numbers", "显示页号");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_ContextMenuThumbnail", "缩略图");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Enlarge_Page_Thumbnails", "放大");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Thumbnails", "缩略图");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Continuous", "连续排序");
            //说明
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_DynamicZoom_Instructions", "单击并拖动进行缩放操作");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_Page_X_OF_X", "当前页: {0} / {1}");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_SnapShot_Instructions", "单击并拖动,系统将选定矩型区域复制到剪帖板");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_ZoomIn_Instructions", "单击并拖动,系统将放大选定矩型区域");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_ZoomOut_Instructions", "单击并拖动,系统将缩小选定矩型区域");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_Hand_Instructions", "单击并拖动以便显示更多内容");
            Infragistics.Win.UltraWinToolbars.Resources.Customizer.SetCustomizedString("AddRemoveButtons", "添加或删除按钮(&A)");



            Infragistics.Win.UltraWinToolbars.Resources.Customizer.SetCustomizedString("Customize", "自定义...(&C)");
            Infragistics.Win.UltraWinToolbars.Resources.Customizer.SetCustomizedString("QuickCustomizeToolTipXP", "工具栏选项");
            Infragistics.Win.UltraWinToolbars.Resources.Customizer.SetCustomizedString("ResetToolbar", "重置工具栏(&R)");
            //Infragistics.Win.UltraWinToolbars.Resources.Customizer.SetCustomizedString("CustomizeDialog_Commands", "按钮");
            Infragistics.Win.UltraWinToolbars.Resources.Customizer.SetCustomizedString("CustomizeCategoryAllCommands", "所有按钮");

        }
        
        //装载菜单
        private void LoadCustomMenu()
        {
            SortedList<string, ToolStripMenuItem> customMenus = new SortedList<string, ToolStripMenuItem>();
            foreach (CRightItem rightitem in CSystem.Sys.Svr.Right.Rights)
            {
                if (rightitem.IsCustom)
                {
                    ToolStripMenuItem menu = new ToolStripMenuItem();
                    menu.Tag = rightitem;
                    menu.Text = rightitem.Name;
                    menu.Name = rightitem.ID;
                    if (customMenus.ContainsKey(rightitem.PID))
                    {
                        if (rightitem.NeedSeparator)
                            customMenus[rightitem.PID].DropDownItems.Add(new ToolStripSeparator());
                        customMenus[rightitem.PID].DropDownItems.Add(menu);
                    }
                    else
                    {
                        MainMenuStrip.Items.Insert(MainMenuStrip.Items.Count - 1, menu);
                    }
                    customMenus.Add(rightitem.ID, menu);
                    if (rightitem.HasAction)
                        menu.Click += new EventHandler(MenuItem_Click);
                    menu.Enabled = CSystem.Sys.Svr.Operator.RightValidate(rightitem);
                }
                else
                {
                    //递归查找到具体菜单
                    setRight(rightitem,MainMenuStrip.Items);
                }
            }
        }
        private void setRight(CRightItem right, ToolStripItemCollection items)
        {
            foreach(ToolStripItem item in items)
            {
                if (item.Name == right.Code)
                {
                    item.Tag = right;
                    switch (right.Code)
                    {
                        case "P_User":
                            right.GetFormHandler = this.Operator;
                            break;
                        case "P_Role":
                            right.GetFormHandler = this.Auth;
                            break;
                    }
                }
                else if (item.GetType()==typeof(ToolStripMenuItem) && ((ToolStripMenuItem)item).DropDownItems.Count > 0)
                {
                    setRight(right, ((ToolStripMenuItem)item).DropDownItems);
                }
            }
        }

        //更新菜单权限
        public void UpdateMenuItem()
        {
            UpdateToolStripMenuItem(SystemToolStripMenuItem, true);
            UpdateToolStripMenuItem(HelpToolStripMenuItem, true);
        } 

        private void UpdateToolStripMenuItem(ToolStripMenuItem TSMI)
        {
            UpdateToolStripMenuItem(TSMI, false);
        }
        private void UpdateToolStripMenuItem(ToolStripMenuItem TSMI,bool isSystem)
        {
            for (int i = 0; i < TSMI.DropDownItems.Count; i++)
            {
                CRightItem right = TSMI.DropDownItems[i].Tag as CRightItem;
                if (right == null)
                    continue;
                else
                    TSMI.DropDownItems[i].Enabled = CSystem.Sys.Svr.Operator.RightValidate(right);
            }
        }

        /*
        private void InitializeDictionaryMenuItems()
        {
            System.Windows.Forms.ToolStripMenuItem DictionaryToolStripMenuItem;
            int group = 1;
            foreach (CTableProperty CT in CSystem.Sys.Svr.Properties.Tables)
            {
                if (CT.AutoLoad)
                {
                    if (group != CT.GroupBy)
                    {
                        group = CT.GroupBy;
                        ToolStripSeparator tss = new ToolStripSeparator();
                        this.DictToolStripMenuItem.DropDownItems.Add(tss);
                    }
                    DictionaryToolStripMenuItem = new ToolStripMenuItem();
                    DictionaryToolStripMenuItem.Name = CT.TableName;
                    DictionaryToolStripMenuItem.Text = CT.Title;
                    DictionaryToolStripMenuItem.Click += new EventHandler(DictionaryToolStripMenuItem_Click);

                    this.DictToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { DictionaryToolStripMenuItem });
                }
            }

        }

        void DictionaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem DictMenuItem = (ToolStripMenuItem)sender;
            string MainTable = DictMenuItem.Name;
            COMFields MainTableDefine = CSystem.Sys.Svr.LDI.GetFields(DictMenuItem.Name);

            foreach (Form f in this.MdiChildren)
                if (f.Tag != null && (string)f.Tag == MainTable)
                {
                    f.Activate();
                    return;
                }
            List<COMFields> detailTable = CSystem.Sys.Svr.Properties.DetailTableDefineList(MainTable);
            frmBrowser frm = new frmBrowser(MainTableDefine, detailTable,"", enuShowStatus.None);
            frm.MdiParent = this;
            frm.Tag = MainTableDefine.OrinalTableName;
            if (MainTableDefine.OrinalTableName == "P_Operator")
            {
                frm.toolDetailForm = new Operator();
            }
            frm.Show();
        }
        private class Operator : ToolDetailForm
        {
            public override bool AutoCode
            {
                get
                {
                    return true;
                }
                set
                {
                    base.AutoCode = value;
                }
            }
            public override string GetPrefix()
            {
                return "S";
            }
            public override int LengthOfCode
            {
                get
                {
                    return 5;
                }
                set
                {
                    base.LengthOfCode = value;
                }
            }
            public override bool DateInCode
            {
                get
                {
                    return false;
                }
                set
                {
                    base.DateInCode = value;
                }
            }
        }*/



        #region MDIMain
        private void MDIMain_Load(object sender, EventArgs e)
        {
            this.Text = ConfigurationManager.AppSettings["MainTitle"];
            string bg = ConfigurationManager.AppSettings["Background"];
            if (bg != null && System.IO.File.Exists(bg))
            {
                this.BackgroundImage = Image.FromFile(bg);
                this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            }
            //this.toolStatusDepartment.Text = CSystem.Sys.Svr.DepartmentName;
            this.toolStatusServer.Text = CSystem.Sys.Svr.cntMain.DBServerName();
            this.toolStatusClient.Text += GetLocalHostName();
            this.toolStatusUser.Text = CSystem.Sys.Svr.UserName;
            //this.panelLeft.Visible = true;

        }

        private string  GetLocalHostName()
        {
            try 
            {
                // Get the local computer host name.
                String hostName = Dns.GetHostName();
                return hostName;
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
                return e.Message;
            }
        }
 
        private void MDIMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = Msg.Question("您确定要退出系统吗?") != DialogResult.Yes;
        }
        #endregion

        #region timer
        private void timerMain_Tick(object sender, EventArgs e)
        {
            CSystem.Sys.Svr.SystemTime = CSystem.Sys.Svr.SystemTime.AddMinutes(1);
        }
        #endregion

        #region menu
        private Form Operator(CRightItem right)
        {
            if (right != null)
            {
                ToolDetailForm tool = new ToolDetailForm();

                frmBrowser frm = (frmBrowser)tool.GetForm(right, this);
                frm.ShowStatus = enuShowStatus.None;
                return frm;
            }
            return null;
        }
        private void OperatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
             ToolStripMenuItem item = (ToolStripMenuItem)sender;
             if (item.Tag != null)
             {
                 CRightItem r = item.Tag as CRightItem;
                 if (r != null)
                 {
                     ToolDetailForm tool = new ToolDetailForm();

                     frmBrowser frm = (frmBrowser)tool.GetForm(r, this);
                     frm.ShowStatus = enuShowStatus.None;
                     if (CheckFormTag(frm.Tag))
                     {
                         frm.MdiParent = this;
                         frm.Show();
                     }
                 }
             }

            //string MainTable = "P_User";
            //COMFields MainTableDefine = CSystem.Sys.Svr.LDI.GetFields(MainTable);

            //if (!CheckFormTag(MainTable))
            //    return; 

            //List<COMFields> detailTable = CSystem.Sys.Svr.Properties.DetailTableDefineList(MainTable);
            //frmBrowser frm = new frmBrowser(MainTableDefine, detailTable, "", enuShowStatus.None);
            //frm.MdiParent = this;
            //frm.Tag = MainTable; ;
            //frm.Show();
        }

        private Form Auth(CRightItem right)
        {
            string MainTable = "P_Role";
            COMFields MainTableDefine = CSystem.Sys.Svr.LDI.GetFields(MainTable);

            List<COMFields> detailTable = CSystem.Sys.Svr.Properties.DetailTableDefineList(MainTable);
            frmRoles frm = new frmRoles(MainTableDefine, detailTable, "", enuShowStatus.None);
            return frm;
        }
        private void AuthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string MainTable = "P_Role";
            COMFields MainTableDefine = CSystem.Sys.Svr.LDI.GetFields(MainTable);

            if (!CheckFormTag(MainTable))
                return; 

            List<COMFields> detailTable = CSystem.Sys.Svr.Properties.DetailTableDefineList(MainTable);
            frmRoles frm = new frmRoles(MainTableDefine, detailTable, "", enuShowStatus.None);
            frm.MdiParent = this;
            frm.Tag = MainTable;
            frm.Show();
        }

        private void toolChangePWD_Click(object sender, EventArgs e)
        {
            frmChangePassword frm = new frmChangePassword();
            frm.ShowDialog();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void MenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            if (item.Tag != null)
            {
                CRightItem r = item.Tag as CRightItem;
                if (r != null)
                {
                    Form frm = r.GetForm(this);
                    if (frm != null)
                    {
                        if (CheckFormTag(frm.Tag))
                        {
                            frm.MdiParent = this;
                            frm.Show();
                        }
                    }
                }
            }
        }
        public bool CheckFormTag(object tag)
        {
            if (tag == null)
                return true;
            foreach (Form f in this.MdiChildren)
                if (f.Tag != null && f.Tag.Equals(tag))
                {
                    f.Activate();
                    return false; ;
                }
            return true;
        }
        #endregion

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAboutBox frm = new frmAboutBox();
            frm.ShowDialog();
        }

        private void toolStripPrintTemplate_Click(object sender, EventArgs e)
        {
            frmTemplate formPrintTemplate = new frmTemplate();
            formPrintTemplate.MdiParent = this;
            formPrintTemplate.Show();
        }

        private void ReportBuilderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process myProcess = new Process();

            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                myProcess.StartInfo.FileName = "C:\\Program Files (x86)\\Microsoft SQL Server\\Report Builder 3.0\\MSReportBuilder.exe";
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
                // This code assumes the process you are starting will terminate itself.  
                // Given that is is started without a window so you cannot terminate it  
                // on the desktop, it must terminate itself or you can do it programmatically 
                // from this application using the Kill method.
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DSStorageIn ReportData = new DSStorageIn();
        }

        private void MainStatus_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStatus_Click(object sender, EventArgs e)
        {

        }
    
    }
  }