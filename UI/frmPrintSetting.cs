using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using Base;
using System.Collections;
using System.IO;

namespace UI
{
    public partial class frmPrintSetting : Form
    {
        private Hashtable PaperHash = new Hashtable();
        private decimal? height = null;
        private decimal? width = null;
        private string _Path;

        private clsPrintSetting _PrinterSettings;
        public frmPrintSetting()
        {
            InitializeComponent();
        }
        public frmPrintSetting(string path):this()
        {
            _Path = path;
        }
        private void frmPrintSetting_Load(object sender, EventArgs e)
        {
            Init();
        }
        public void Init()
        {
            int index = 0;
            PrintDocument fPrintDocument = new PrintDocument();
            if (PrinterSettings.InstalledPrinters.Count == 0)
            {
                MessageBox.Show("没有安装打印机");
                return;
            }
            this.cmbPrintName.Items.Add(fPrintDocument.PrinterSettings.PrinterName);
            foreach (string pdName in PrinterSettings.InstalledPrinters)
            {
                if (!cmbPrintName.Items.Contains(pdName))
                    this.cmbPrintName.Items.Add(pdName);
                //printNameHash.Add(pdName, index++);

            }
            this.cmbPrintName.SelectedIndex = 0;

            //获取打印模板
            if (Directory.Exists(this._Path))
            {
                string[] files = Directory.GetFiles(this._Path, "*.xml", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    int j = files[i].LastIndexOf('\\');
                    string f = files[i].Substring(j + 1, files[i].Length - j - 5);
                    cmbTemplate.Items.Add(f);
                }
                if (cmbTemplate.Items.Count > 0)
                    cmbTemplate.SelectedIndex = 0;
            }
            else
                grpTemplate.Enabled = false;
            try
            {
                string Print_Left = CSystem.Sys.Svr.Preferences.GetValue("Print_Left");
                if (Print_Left != null)
                    numLeft.Value = decimal.Parse(Print_Left);
                string Print_Right = CSystem.Sys.Svr.Preferences.GetValue("Print_Right");
                if (Print_Right != null)
                    numRight.Value = decimal.Parse(Print_Right);
                string Print_Top = CSystem.Sys.Svr.Preferences.GetValue("Print_Top");
                if (Print_Top != null)
                    numTop.Value = decimal.Parse(Print_Top);
                string Print_Bottom = CSystem.Sys.Svr.Preferences.GetValue("Print_Bottom");
                if (Print_Bottom != null)
                    numBottom.Value = decimal.Parse(Print_Bottom);
            }
            catch { }
        }
        public string TempateFile
        {
            get
            {
                if (cmbTemplate.SelectedIndex > -1)
                {
                    string f = cmbTemplate.Items[cmbTemplate.SelectedIndex] as string;
                    if (f != null && f.Length > 0)
                    {
                        if (_Path.EndsWith("\\"))
                            return _Path + f + ".xml";
                        else
                            return _Path + "\\" + f + ".xml";
                    }
                }
                return null;
            }
        }
        public clsPrintSetting PrintSetting
        {
            get { return _PrinterSettings; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cmbPrintName.Text.Length==0)
            {
                Msg.Information("请选择打印机！");
                return;
            }
            _PrinterSettings = new clsPrintSetting();
            _PrinterSettings.PrinterName = cmbPrintName.Text;
            _PrinterSettings.Copies = (short)numericPageRow.Value;
            _PrinterSettings.Landscape = IsLandscape;
            _PrinterSettings.IsOverPrint = chkOverprint.Checked;
            _PrinterSettings.Margins = new Margins((int)numLeft.Value, (int)numRight.Value, (int)numTop.Value, (int)numBottom.Value);
            if (txtPageNum.Text.Length>0)
                _PrinterSettings.PageRange = txtPageNum.Text;
            _PrinterSettings.PaperSize = new PaperSize(cmbPageSize.Text, (int)numPageWidth.Value, (int)numPageHeight.Value);

            CSystem.Sys.Svr.Preferences.SetValue("Print_Left", numLeft.Value.ToString());
            CSystem.Sys.Svr.Preferences.SetValue("Print_Right", numRight.Value.ToString());
            CSystem.Sys.Svr.Preferences.SetValue("Print_Top", numTop.Value.ToString());
            CSystem.Sys.Svr.Preferences.SetValue("Print_Bottom", numBottom.Value.ToString());
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private Hashtable paperIndexHash = new Hashtable();
        private PrinterSettings print = new PrinterSettings();
        private PaperSize customPaper = null;
        private void cmbPrintName_SelectedIndexChanged(object sender, EventArgs e)
        {
            customPaper = null;
            string printName = cmbPrintName.SelectedItem.ToString();
            this.height = this.numPageHeight.Value;
            this.width = this.numPageWidth.Value;
            if (printName != null)
            {
                if (cmbPageSize.Items.Count > 0) cmbPageSize.Items.Clear();
                PaperHash = new Hashtable();
                paperIndexHash = new Hashtable();
                print.PrinterName = printName;
                int index = 0;
                int selectedIndex = 0;
                foreach (PaperSize p in print.PaperSizes)
                {
                    cmbPageSize.Items.Add(p.PaperName);
                    PaperHash.Add(p.PaperName, p);
                    paperIndexHash.Add(p.PaperName, index++);
                    if (selectedIndex == 0 && p.Height == (int)this.height && p.Width == (int)this.width)
                        selectedIndex = index - 1;
                    else if ( p.Kind.Equals(print.DefaultPageSettings.PaperSize.Kind))
                    {
                        selectedIndex = index - 1;
                        numBottom.Value = print.DefaultPageSettings.Margins.Bottom;
                        numTop.Value = print.DefaultPageSettings.Margins.Top;
                        numLeft.Value = print.DefaultPageSettings.Margins.Left;
                        numRight.Value = print.DefaultPageSettings.Margins.Right;
                    }

                    if (p.Kind == PaperKind.Custom)
                        customPaper = p;
                }
                if (selectedIndex == 0 && customPaper != null && this.paperIndexHash.ContainsKey(this.customPaper.PaperName))
                {
                    selectedIndex = (int)this.paperIndexHash[this.customPaper.PaperName];
                }
                cmbPageSize.SelectedIndex = selectedIndex;
            }
        }
        private void cmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            PaperSize currPaper = PaperHash[cmbPageSize.SelectedItem.ToString()] as PaperSize;
            if (currPaper.Kind != PaperKind.Custom || (currPaper.Height > 0 && currPaper.Width > 0))
            {
                this.numPageWidth.Value = currPaper.Width;
                this.numPageHeight.Value = currPaper.Height;
            }
        }

        private void numPageWidth_ValueChanged(object sender, EventArgs e)
        {
            PaperSize currPaper = PaperHash[cmbPageSize.SelectedItem.ToString()] as PaperSize;

            if (currPaper.Kind != PaperKind.Custom && Convert.ToInt32(this.numPageWidth.Value) != currPaper.Width)
            {
                if (this.customPaper == null)
                {
                    MessageBox.Show("当前打印机不支持自定义纸张类型！", "提示");
                    numPageWidth.Value = currPaper.Width;
                }
                else
                {
                    this.cmbPageSize.SelectedIndex = (int)this.paperIndexHash[this.customPaper.PaperName];
                }
            }
        }

        private void numPageHeight_ValueChanged(object sender, EventArgs e)
        {
            PaperSize currPaper = PaperHash[cmbPageSize.SelectedItem.ToString()] as PaperSize;

            if (currPaper.Kind != PaperKind.Custom && Convert.ToInt32(this.numPageHeight.Value) != currPaper.Height)
            {
                if (this.customPaper == null)
                {
                    MessageBox.Show("当前打印机不支持自定义纸张类型！", "提示");
                    numPageHeight.Value = currPaper.Height;
                }
                else
                {
                    this.cmbPageSize.SelectedIndex = (int)this.paperIndexHash[this.customPaper.PaperName];
                }
            }
        }

        private bool IsLandscape;
        private void btnVertical_Click(object sender, EventArgs e)
        {
            IsLandscape = false;
            btnVertical.Image = imageList1.Images[0];
            btnHorizontal.Image = imageList1.Images[3];
        }

        private void btnHorizontal_Click(object sender, EventArgs e)
        {
            IsLandscape = true;
            btnVertical.Image = imageList1.Images[1];
            btnHorizontal.Image = imageList1.Images[2];
        }


    }
}