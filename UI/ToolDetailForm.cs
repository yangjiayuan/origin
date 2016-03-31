using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Excel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Base;

namespace UI
{
    public class ToolDetailForm:IMenuAction
    {
        public enum enuInsertToolStripType : int {Detail = 1, Browser = 2, All = 3 };
        protected bool _OnlyDisplay = false;
        protected bool _AutoCode = false;
        protected int _LengthOfCode = 4;
        protected bool _DateInCode = true;
        protected string _AutoCodeFormat = "yyyyMM";
        protected SortedList<string, Control> _ControlMap;
        protected SortedList<string, UltraGrid> _GridMap;
        protected frmDetail _DetailForm;
        protected frmBrowser _BrowserForm;
        protected List<CCreateGrid> _ListCreateGrid = null;
        protected CCreateDetailForm _CreateDetailForm;
        protected string _MainTableName;
        protected COMFields _MainCOMFields;
        protected List<COMFields> _DetailCOMFields;
        protected CRightItem _Right;
        protected int _Caption_Width = 90;
        protected int _Data_Width = 110;


        public void Initialization()
        {
            _ListCreateGrid = null;
            _CreateDetailForm = null;
            _ControlMap = null;
            _GridMap = null;
            _DetailForm = null;
        }
        public virtual bool OnlyDisplay
        {
            get { return _OnlyDisplay; }
            set { _OnlyDisplay = value; }
        }

        public CRightItem Right
        {
            get { return _Right; }
            set { _Right = value; }
        }
        
        public virtual string MainTableName
        {
            get
            {
                if (_MainTableName == null && _Right != null)
                {
                    _MainTableName = _Right.Code;
                    return _MainTableName;
                }
                else
                    return _MainTableName;
            }
            set { _MainTableName = value; }
        }
        public virtual COMFields MainCOMFields
        {
            get
            {
                if (_MainCOMFields != null)
                    return _MainCOMFields;
                else if (_MainTableName != null)
                {
                    _MainCOMFields = CSystem.Sys.Svr.LDI.GetFields(_MainTableName);
                    return _MainCOMFields;
                }
                else
                    return null;
            }
            set
            {
                _MainCOMFields = value;
            }
        }
        public virtual List<COMFields> DetailCOMFields
        {
            get
            {
                if (_DetailCOMFields != null)
                    return _DetailCOMFields;
                else if (_MainTableName != null)
                {
                    _DetailCOMFields = CSystem.Sys.Svr.Properties.DetailTableDefineList(_MainTableName);
                    return _DetailCOMFields;
                }
                else
                    return null;
            }
            set
            {
                _DetailCOMFields = value;
            }
        }

        public virtual void CopyDetail(DataSet ds)
        {
        }
        public virtual void RefrashCheckStatus(UltraGridRow row)
        {
        }
        public virtual bool AutoCode
        {
            get
            {
                return _AutoCode;
            }
            set
            {
                _AutoCode = value;
            }
        }
        public virtual bool DateInCode
        {
            get
            {
                return _DateInCode;
            }
            set
            {
                _DateInCode = value;
            }
        }

        public virtual string AutoCodeFormat
        {

            get
            {
                return _AutoCodeFormat;
            }
            set
            {
                _AutoCodeFormat = value;
            }
        
        }
        public virtual int LengthOfCode
        {
            get
            {
                return _LengthOfCode;
            }
            set
            {
                _LengthOfCode = value;
            }
        }

        /// <summary>
        /// ����CreateDetailForm����
        /// </summary>
        /// <param name="createGrid"></param>
        public virtual void SetCreateDetail(CCreateDetailForm createDetailForm)
        {
            _CreateDetailForm = createDetailForm;
        }

        /// <summary>
        /// ����CreateGrid����
        /// </summary>
        /// <param name="createGrid"></param>
        public virtual void SetCreateGrid(CCreateGrid createGrid)
        {
            if (_ListCreateGrid == null)
                _ListCreateGrid = new List<CCreateGrid>();
            _ListCreateGrid.Add(createGrid);
        }

        /// <summary>
        /// ����DetailForm
        /// </summary>
        /// <param name="frm"></param>
        public virtual void SetDetailForm(frmDetail frm)
        {
            _DetailForm = frm;
        }

        public virtual void SetBrowerForm(frmBrowser frm)
        {
            _BrowserForm = frm;
        }

        /// <summary>
        /// ÿ���ؼ����������,��������Ӧ�¼�
        /// </summary>
        /// <param name="field"></param>
        /// <param name="control"></param>
        public virtual void NewControl(COMField f, Control ctl)
        {
        }

        public virtual void NewColumn(COMField f, UltraGridColumn col)
        {
        }
        /// <summary>
        /// ���ô�����еĿؼ��ļ���
        /// </summary>
        /// <param name="controlMap"></param>
        public virtual void SetControlMap(SortedList<string, Control> controlMap)
        {
            _ControlMap = controlMap;
        }

        /// <summary>
        /// ������ϸ���Grid
        /// </summary>
        /// <param name="table"></param>
        /// <param name="grid"></param>
        public virtual void SetGridMap(SortedList<string, UltraGrid> gridMap)
        {
            _GridMap = gridMap;
        }

        public virtual void AfterDataBind()
        {
        }

        public virtual void EditChanged(bool isEdit)
        {
        }

        /// <summary>
        /// ����Grid����
        /// </summary>
        /// <param name="gridMap"></param>
        public virtual void NewGrid(COMFields fields, UltraGrid grid)
        {

        }

        /// <summary>
        /// ��������ʱ���������ط���ִ�е���showDataʱ������Button
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="mainTableDefine"></param>
        /// <param name="detailTableDefine"></param>
        /// <returns></returns>
        public virtual bool NewData(DataSet ds, COMFields mainTableDefine, List<COMFields> detailTableDefine)
        {
            return true;
        }

        ///// <summary>
        ///// ������
        ///// </summary>
        ///// <param name="dataTable"></param>
        ///// <returns></returns>
        //public virtual bool InsertLines(DataTable dataTable)
        //{
        //    return true;
        //}

        /// <summary>
        /// ����ʱ
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public virtual string Saving(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            return null;
        }
        public virtual void InsertToolStrip(ToolStrip toolStrip,enuInsertToolStripType insertType)
        {
        }
        public virtual string GetPrefix()
        {
            return "";
        }
        public virtual bool AllowInsertRowInGrid(string TableName)
        {
             return true;
        }
        public virtual bool AllowUpdateInGrid(string TableName)
        {
             return true;
        }
        public virtual bool AllowCopyLine(string TableName)
        {
             return true;
        }
        public virtual bool AllowInsertRowInToolBar
        {
            get
            {
                return false ;
            }
        }
        public virtual bool AllowDeleteRowInToolBar
        {
            get
            {
                return true ;
            }
        }
        public virtual bool AllowNew
        {
            get
            {
                return true;
            }
        }

        public virtual bool AllowEdit
        {
            get
            {
                return true;
            }
        }
        public virtual bool AllowCheck
        {
            get
            {
                return false;
            }
        }
        public virtual bool ShowAddRowButton(string tableName)
        {
            return false;
        }
        public virtual bool Check(Guid ID, SqlConnection conn, SqlTransaction sqlTran,DataRow mainRow)
        {
            return true;
        }

        public virtual bool UnCheck(Guid ID, SqlConnection conn, SqlTransaction sqlTran, DataRow mainRow)
        {
            return true;
        }

        public virtual string BeforeSaving(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            return null;
        }
        public virtual DataSet InsertRowsInGrid(List<COMFields> detailTableDefine)
        {
            return null;
        }
        public virtual bool IsShow(string tableName)
        {
            return true;
            //return CSystem.Sys.Svr.Properties[tableName].Visible;
        }

        public virtual DataSet DeleteRowsInGrid(List<COMFields> DetailTableDefine)
        {
            try
            {
                if (_DetailForm.ActiveControl != null && _DetailForm.ActiveControl.GetType() == typeof(UltraGrid))
                {
                    UltraGrid grid = _DetailForm.ActiveControl as UltraGrid;
                    if (grid.Selected.Rows.Count == 0)
                        grid.ActiveRow.Selected = true;
                    grid.BeforeRowsDeleted += new BeforeRowsDeletedEventHandler(grid_BeforeRowsDeleted);
                    grid.PerformAction(UltraGridAction.DeleteRows);
                    grid.BeforeRowsDeleted -= new BeforeRowsDeletedEventHandler(grid_BeforeRowsDeleted);

                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        void grid_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            if (e.DisplayPromptMsg)
            {
                if (Msg.Question(string.Format("����ɾ��{0}������,�Ƿ����?", e.Rows.Length)) == DialogResult.Yes)
                    e.DisplayPromptMsg = false;
                else
                    e.Cancel = true;
            }
        }

        public virtual bool AllowEntryEditMode(string tableName)
        {
            return true;
        }
        public virtual bool AllowSort(string tableName)
        {
            return true;
        }
        public virtual bool BeforeExport(Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter exporter, UltraGrid grid, Worksheet sheet, ref int startRow, ref int startColumn)
        {
            return true;
        }
        public virtual void AfterExport(UltraGrid grid, Worksheet sheet)
        {

        }
        public virtual Form GetForm(CRightItem right, Form mdiForm)
        {
            return GetForm(right, mdiForm,null);
        }
        public virtual Form GetForm(CRightItem right, Form mdiForm,string where)
        {
            return GetForm(right, mdiForm, where, enuShowStatus.ShowCheck);
        }
        public virtual Form GetForm(CRightItem right, Form mdiForm,string where,enuShowStatus status)
        {
           frmBrowser frm;
            _Right = right;
            if (_MainTableName == null)
                MainTableName = right.Code;
            

            if (right.Name ==null)
                 frm = new frmBrowser(MainCOMFields, DetailCOMFields, where, status);
            else
                 frm = new frmBrowser(MainCOMFields, DetailCOMFields, where, status,0,right.Name);

            frm.toolDetailForm = this;
            frm.Tag = right.Code + "_" + right.Name;
            return frm;
        }

        public virtual bool Deleting(SqlConnection conn, SqlTransaction tran, Guid id)
        {
            return true;
        }

        public virtual void BeforeNewControl(object sender, UI.CCreateDetailForm.FieldControlEventArgs e)
        {
        }

        public virtual void AfterEditModeChanged(bool IsEdit)
        {
        }

        public virtual void AfterSetNewRowIDWhenCopy(DataSet ds)
        {
            
        }

        public virtual void BeforeSetRowNewIDWhenCopy(Guid newID, Guid oldID, string p)
        {
            
        }

        public virtual void BeforeCopyDetailTable(string tableName)
        {
            
        }
        public int CaptionWidth
        {
            set { _Caption_Width = value; }
            get { return _Caption_Width; }
        }
        public int DataWidth
        {
            set { _Data_Width = value; }
            get { return _Data_Width; }
        }

        public virtual void AfterSaved(DataSet ds, SqlConnection conn, SqlTransaction tran)
        {
            
        }

        public virtual bool CancelCodeRule(CCodeRule rule,bool isNew,object data)
        {
            return !isNew;
        }
    }
}
