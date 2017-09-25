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

        //Grid����ת������
        private void translate()
        {
            
            //Grid����ת������
            Infragistics.Win.UltraWinGrid.Resources.Customizer.SetCustomizedString("DataErrorCellUpdateUnableToUpdateValue", "����������ݸ�ʽ����ȷ�����顣\r\n�����Ϣ��{0}");
            Infragistics.Win.UltraWinGrid.Resources.Customizer.SetCustomizedString("MultiCellOperation_Error_ConversionError", "����������ݸ�ʽ����ȷ�����顣\r\n�����Ϣ������ת������ '{0}' ������:{1}");
            Infragistics.Win.UltraWinGrid.Resources.Customizer.SetCustomizedString("DeleteMultipleRowsPrompt", "ȷ��Ҫɾ��ָ���е�������?");
            Infragistics.Win.UltraWinGrid.Resources.Customizer.SetCustomizedString("DeleteRowsMessageTitle", ConfigurationManager.AppSettings["MainTitle"]);

            //ContextMenu�ĺ���
            Infragistics.Win.Resources.Customizer.SetCustomizedString("EditContextMenuCut", "����(&T)");
            Infragistics.Win.Resources.Customizer.SetCustomizedString("EditContextMenuCopy", "����(&C)");
            Infragistics.Win.Resources.Customizer.SetCustomizedString("EditContextMenuPaste", "ճ��(&P)");
            Infragistics.Win.Resources.Customizer.SetCustomizedString("EditContextMenuSelectAll", "ȫѡ(&A)");

            //��ӡԤ��
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_DialogCaption", "��ӡԤ��");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Print", "��ӡ(&P)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_ClosePreview", "�ر�(&C)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_ContextMenuPreviewZoom", "��ʾ����");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Current_Page", "��ǰҳ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Exit", "�˳�(&X)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_First_Page", "��һҳ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Go_To", "����");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Last_Page", "���һҳ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Next_Page", "��һҳ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Previous_Page", "ǰһҳ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Next_View", "��һ��ͼ(&N)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Previous_View", "ǰһ��ͼ(&P)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Hand_Tool", "���͹���(&H)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Page_Setup", "ҳ������(&U)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Snapshot_Tool", "���չ���(&S)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_View", "��ͼ(&V)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Whole_Page", "����ҳ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Zoom", "����(&Z)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Zoom_In", "�Ŵ�");

            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Zoom_Out", "��С");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolCategory_Context_Menus", "�����Ĳ˵�");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolCategory_File", "�ļ�");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolCategory_Menus", "�˵�");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolCategory_Tools", "������");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolCategory_View", "��ͼ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolCategory_Zoom_Mode", "����ģʽ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolTip_ClosePreview", "�ر�");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ToolTip_Zoom", "����");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_Page_X_OF_X", "ҳ:{0}/{1}");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("CustomizeImg_ToolBar_MenuBar", "�˵�");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("CustomizeImg_ToolBar_Standard", "��׼");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("CustomizeImg_ToolBar_View", "��ͼ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_File", "�ļ�(&F)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Tools", "����(&T)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Dynamic_Zoom_Tool", "��̬���Ź���(&D)");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Zoom_Out_Tool", "��С����");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Zoom_In_Tool", "�Ŵ󹤾�");
            // Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Page_Layout","�˵�"); 
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PreviewRowColSelection_Cancel", "ȡ��");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PreviewRowColSelection_SelectedPages", "{0} x {1} ҳ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PreviewRowColSelection_Cancel", "ȡ��");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Page_Width", "ҳ��");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ZoomListItem_MarginWidth", "���ֿ��");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ZoomListItem_PageWidth", "ҳ��");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_ZoomListItem_WholePage", "����ҳ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Page_Layout", "ҳ�沼��");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Margin_Width", "���ֿ��");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("ContextMenuPreviewHand", "������ͼ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Reduce_Page_Thumbnails", "��С");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Show_Page_Numbers", "��ʾҳ��");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_ContextMenuThumbnail", "����ͼ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Enlarge_Page_Thumbnails", "�Ŵ�");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Thumbnails", "����ͼ");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("PrintPreview_Tool_Continuous", "��������");
            //˵��
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_DynamicZoom_Instructions", "�������϶��������Ų���");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_Page_X_OF_X", "��ǰҳ: {0} / {1}");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_SnapShot_Instructions", "�������϶�,ϵͳ��ѡ�����������Ƶ�������");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_ZoomIn_Instructions", "�������϶�,ϵͳ���Ŵ�ѡ����������");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_ZoomOut_Instructions", "�������϶�,ϵͳ����Сѡ����������");
            Infragistics.Win.Printing.Resources.Customizer.SetCustomizedString("StatusBar_Hand_Instructions", "�������϶��Ա���ʾ��������");
            Infragistics.Win.UltraWinToolbars.Resources.Customizer.SetCustomizedString("AddRemoveButtons", "��ӻ�ɾ����ť(&A)");



            Infragistics.Win.UltraWinToolbars.Resources.Customizer.SetCustomizedString("Customize", "�Զ���...(&C)");
            Infragistics.Win.UltraWinToolbars.Resources.Customizer.SetCustomizedString("QuickCustomizeToolTipXP", "������ѡ��");
            Infragistics.Win.UltraWinToolbars.Resources.Customizer.SetCustomizedString("ResetToolbar", "���ù�����(&R)");
            //Infragistics.Win.UltraWinToolbars.Resources.Customizer.SetCustomizedString("CustomizeDialog_Commands", "��ť");
            Infragistics.Win.UltraWinToolbars.Resources.Customizer.SetCustomizedString("CustomizeCategoryAllCommands", "���а�ť");

        }
        
        //װ�ز˵�
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
                    //�ݹ���ҵ�����˵�
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

        //���²˵�Ȩ��
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
            e.Cancel = Msg.Question("��ȷ��Ҫ�˳�ϵͳ��?") != DialogResult.Yes;
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