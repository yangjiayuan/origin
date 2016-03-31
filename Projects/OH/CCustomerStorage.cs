using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Infragistics.Win.UltraWinGrid;
using Base;
using UI;
using System.Windows.Forms;
using Infragistics.Win.Printing;
using System.Collections;

namespace OH
{
    public class CCustomerStorage : ToolDetailForm
    {
        private enuDocumentIn DocumentInType;
        private ToolStripButton toolGRConfirm;

        private Form _mdiForm;

        public CCustomerStorage()
        {

        }
        public override bool AllowInsertRowInGrid(string TableName)
        {
            return false;
        }
        public override bool AllowDeleteRowInToolBar
        {
            get
            {
                return true;
            }
        }
        public override bool AllowUpdateInGrid(string TableName)
        {
            return true;
        }
        public override bool AllowNew
        {
            get
            {
                return false;
            }
        }

        public override bool AllowEdit
        {
            get
            {
                return false;
            }
        }

        public override bool AllowInsertRowInToolBar
        {
            get
            {
                return false;
            }
        }
        public override bool AllowCheck
        {
            get
            {
                return false;
            }
        }

        public override bool AutoCode
        {
            get
            {
                return false;
            }
        }
        public override void NewGrid(Base.COMFields fields, Infragistics.Win.UltraWinGrid.UltraGrid grid)
        {
            //grid.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(grid_AfterCellUpdate);
        }



        public override Form GetForm(CRightItem right, Form mdiForm)
        {
            try
            {
                string Filter;

                _mdiForm = mdiForm;
                this.MainTableName = "V_CustomerConsignStorageData";
                Filter = string.Format("V_CustomerConsignStorageData.Quantity >0 ");

                COMFields mainTableDefine = CSystem.Sys.Svr.LDI.GetFields(this.MainTableName).Clone();
                List<COMFields> detailTableDefines = CSystem.Sys.Svr.Properties.DetailTableDefineListClone(this.MainTableName);
                this._MainCOMFields = mainTableDefine;
                this._DetailCOMFields = detailTableDefines;

                frmBrowser frm = (frmBrowser)base.GetForm(right, mdiForm, Filter);
                return frm;
            }
            catch (Exception ex)
            {
                Msg.Warning(string.Format("客户寄售库存程序加载失败！%n {0}", ex.Message));
            }
            return null;

        }

        public override DataSet DeleteRowsInGrid(List<COMFields> DetailTableDefine)
        {
            if (_DetailForm.ActiveControl.GetType() == typeof(UltraGrid))
            {
                UltraGrid grid = _DetailForm.ActiveControl as UltraGrid;
                string TableName = null;
                foreach (string n in _DetailForm.GridMap.Keys)
                    if (_DetailForm.GridMap[n] == grid)
                        TableName = n;
            }
            return _DetailForm.MainDataSet;
        }

    }
}
