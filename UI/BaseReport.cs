using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using System.Windows.Forms;
using Base;
using System.Data;

namespace UI
{
    public class BaseReport:IMenuAction
    {
        protected List<ReportColumn> _Columns = new List<ReportColumn>();
        protected List<ReportCondition> _Conditions = new List<ReportCondition>();
        protected List<ReportDetialButton> _DetailButtons = new List<ReportDetialButton>();
        protected string _Title;
        protected string _SQL;
        protected bool _IsWhere = true;
        protected bool _IsAppendSQL = false;
        protected bool _ShowConditionForm;
        public bool Inited = false;
        public bool Closed = false;
        public EventHandler InitControl;

        public string SQL
        {
            get
            {
                return _SQL;
            }
        }
        public bool IsAppendSQL
        {
            get { return _IsAppendSQL; }
        }

        public string Where
        {
            get
            {
                if (_IsWhere)
                    return " Where ";
                else
                    return " and ";
            }
        }
        public List<ReportDetialButton> DetailButtons
        {
            get
            {
                return _DetailButtons;
            }
        }
        public List<ReportColumn> Columns
        {
            get
            {
                if (!Inited)
                    Initialize();
                return _Columns;
            }
        }
        public List<ReportCondition> Conditions
        {
            get
            {
                if (!Inited)
                    Initialize();
                return _Conditions;
            }
        }
        public ReportCondition GetCondition(string Name)
        {
            foreach (ReportCondition rc in Conditions)
                if (rc.Name == Name)
                    return rc;
            return null;
        }
        protected ReportColumn AddColumn(string name, string description, string dataType, Infragistics.Win.HAlign align, string format, bool visible)
        {
            return AddColumn(name, description, dataType, align, format, visible, RptSummaryType.None);
        }
        protected ReportColumn AddColumn(string name, string description, string dataType, Infragistics.Win.HAlign align, string format, bool visible, RptSummaryType summary)
        {
            ReportColumn col = new ReportColumn(name, description, dataType, align, format, visible, summary);
            _Columns.Add(col);
            return col;
        }
        protected ReportColumn AddColumn(string name, string description, string dataType, Infragistics.Win.HAlign align, string format, bool visible, RptSummaryType summary,bool fix)
        {
            ReportColumn col = new ReportColumn(name, description, dataType, align, format, visible, summary, fix);
            _Columns.Add(col);
            return col;
        }
        protected ReportColumn AddColumn(string name, string description)
        {
            return AddColumn(name, description, RptSummaryType.None);
        }
        protected ReportColumn AddColumn(string name, string description,bool fix)
        {
            ReportColumn col = new ReportColumn(name, description, fix);
            _Columns.Add(col);
            return col;
        }
        protected ReportColumn AddColumn(string name, string description, RptSummaryType summary)
        {
            ReportColumn col = new ReportColumn(name, description, summary);
            _Columns.Add(col);
            return col;
        }
        protected ReportCondition AddCondition(string name, string description, string dataType, string conditionSQL)
        {
            return AddCondition(name, description, dataType, conditionSQL, "");
        }
        protected ReportCondition AddCondition(string name, string description, string dataType, string conditionSQL, string conditionSQL2)
        {
            ReportCondition rc = new ReportCondition(name, description, dataType, conditionSQL, conditionSQL2);
            _Conditions.Add(rc);
            return rc;
        }
        public bool ShowConditionForm
        {
            get
            {
                return _ShowConditionForm;
            }
        }
        public string Title
        {
            get { return _Title; }
        }
        public void Reshow()
        {
            Closed = false;
            if (InitControl != null)
                InitControl(this, EventArgs.Empty);
        }
        public virtual bool Initialize()
        {
            Inited = true;
            if (InitControl != null)
                InitControl(this, EventArgs.Empty);
            return true;
        }
        public virtual void InsertToolStrip(ToolStrip toolStrip)
        {
        }
        public virtual bool Strike(Form frm,UltraGridRow row)
        {
            return false;
        }
        public virtual string BeforeSelectForm(ReportCondition condition)
        {
            return "";
        }
        public virtual string BeforeSelectForm(frmReport.BeforeSelectFormEventArgs e)
        {
            return BeforeSelectForm(e.Condition);
        }

        public virtual void AfterSelectForm(ReportCondition rc,DataRow dr)
        {
        }
        public virtual void BeforeQuery(object sender, frmReport.BeforeQueryEventArgs e)
        {
        }
        public virtual void BeforeCondition(frmReport.BeforeConditionQueryEventArgs e)
        {
        }
        public virtual void BeforeNewControl(frmReport.BeforeConditionCreateEventArgs e)
        {
        }
        public virtual string DetailButtonClick(ReportDetialButton rdb,UltraGridRow Row)
        {
            return null;
        }

        #region IMenuAction Members

        public virtual Form GetForm(CRightItem right, Form mdiForm)
        {
            return new frmReport(this, mdiForm);
        }

        #endregion

        public virtual void AfterNewControl(frmReport.BeforeConditionCreateEventArgs e)
        {
        }
    }
}
