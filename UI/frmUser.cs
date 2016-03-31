using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

namespace UI
{
    public partial class frmUser : Form
    {
        
        private string table;
        private string description;
        public frmUser()
        {
            InitializeComponent();
        }
        public frmUser(string table, string description)
        {
            InitializeComponent();
            this.description = description;
            this.table = table;
            this.Text = this.description;
        }            
    }
}