using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;

namespace UI
{
    public partial class DictControl : UserControl
    {
        public DictControl()
        {
            InitializeComponent();
            EditorButton editButton = new EditorButton();
            this.ultraComboEditor1.ButtonsRight.Add(editButton);
            //ultraComboEditor1.KeyDown += new KeyEventHandler(ute_KeyDown);
            //ultraComboEditor1.EditorButtonClick += new EditorButtonEventHandler(Dict_EditorButtonClick);
            //ute.TextChanged += new EventHandler(Dict_TextChanged);
            //this.ultraComboEditor1.ButtonsRight
        }
        public Infragistics.Win.UltraWinEditors.UltraComboEditor CustomControl
        {
            get { return this.ultraComboEditor1; }
        }
        public String ValueBindName = "Text";

        private Keys keys;
        private void ultraComboEditor1_ValueChanged(object sender, EventArgs e)
        {
            if (keys != Keys.Up && keys != Keys.Down && keys != Keys.Enter)
            {
                DataTable dicTable = (DataTable)this.ultraComboEditor1.Tag;
                DataRow[] allrow = dicTable.Select("code like '%" + ultraComboEditor1.Text + "%'", "code");
                DataTable dtNew = dicTable.Clone();
                for (int i = 0; i < allrow.Length; i++)
                {
                    dtNew.ImportRow(allrow[i]);
                }
                this.ultraComboEditor1.CloseUp();
                ultraComboEditor1.DataSource = dtNew;
                this.ultraComboEditor1.DropDownListWidth = 500;
                this.ultraComboEditor1.DropDownListAlignment = Infragistics.Win.DropDownListAlignment.Left;
                this.ultraComboEditor1.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
                this.ultraComboEditor1.DropDown();
            }
        }

        private void ultraComboEditor1_KeyDown(object sender, KeyEventArgs e)
        {
            keys = e.KeyCode;
            //else if (e.KeyCode == Keys.Enter)
            //{
            //    this.Visible = false;
            //}
        }
    }
}
