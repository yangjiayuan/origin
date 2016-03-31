using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UI
{
    public class ReportCondition
    {
        public string Name;
        public string Description;
        public string DataType;
        public string ConditionSQL;
        public string ConditionSQL2;
        public object Control = null;
        public int Width = 1;
        public bool NewLine = false;
        public string GroupName;
        public bool Mandatory = false;
        public ReportCondition(string name, string description, string dataType, string conditionSQL):this(name,description,dataType,conditionSQL,"")
        {
        }
        public ReportCondition(string name, string description, string dataType, string conditionSQL, string conditionSQL2)
        {
            Name = name;
            Description = description;
            DataType = dataType;
            ConditionSQL = conditionSQL;
            ConditionSQL2 = conditionSQL2;
        }
    }
    public class ReportColumn
    {
        public string Name;
        public string Description;
        public Infragistics.Win.HAlign Align;
        public string Format;
        public bool Visible;
        public string DataType;
        public RptSummaryType Summary = RptSummaryType.None;
        public bool Fixed = false;
        public bool MergeCells = false;
        public bool isLabel = false;
        public int[] OriginXY = null;
        public int[] SpanXY=new int[]{2,2};
        //public int[] OriginandSpan = new int[] { 2, 0, 2, 2 };
        public ReportColumn(string name, string description)
            : this(name, description, RptSummaryType.None)
        {
        }
        public ReportColumn(string name, string description, bool fix)
            : this(name, description)
        {
            Fixed = fix;
        }
        public ReportColumn(string name, string description, RptSummaryType summary)
        {
            Name = name;
            Description = description;
            Align = Infragistics.Win.HAlign.Default;
            Visible = true;
            Summary = summary;
        }
        public ReportColumn(string name, string description, string dataType, Infragistics.Win.HAlign align, string format, bool visible)
            : this(name, description, dataType, align, format, visible, RptSummaryType.None)
        {
        }
        public ReportColumn(string name, string description, string dataType, Infragistics.Win.HAlign align, string format, bool visible, RptSummaryType summary)
        {
            Name=name;
            Description=description;
            DataType = dataType;
            Align=align;
            Format=format;
            Visible = visible;
            Summary = summary;
        }
        public ReportColumn(string name, string description, string dataType, Infragistics.Win.HAlign align, string format, bool visible, RptSummaryType summary,bool fix):
            this(name, description, dataType, align, format, visible, summary)
        {
            Fixed = fix;
        }
    }
    public class ReportDetialButton
    {
        private string _Text;
        private string _Key;
        private List<ReportColumn> _Columns=new List<ReportColumn>();
        private string _SQL;
        private Button _Button;
        public ReportDetialButton(string text, string key, string sql)
        {
            _Text = text;
            _Key = key;
            _SQL = sql;
        }
        public string SQL
        {
            get { return _SQL; }
        }
        public string Text
        {
            get { return _Text; }
        }
        public string Key
        {
            get { return _Key; }
        }
        public List<ReportColumn> Columns
        {
            get { return _Columns; }
        }
        public Button Button
        {
            get { return _Button; }
            set { _Button = value; }
        }
    }
    // Summary:
    //     Used to specify the type of summary to calculate on a Infragistics.Win.UltraWinGrid.SummarySettings
    //     object.
    //
    // Remarks:
    //     Infragistics.Win.UltraWinGrid.SummarySettings.SummaryType Infragistics.Win.UltraWinGrid.UltraGridBand.Summaries
    public enum RptSummaryType
    {
        None=-1,
        // Summary:
        //     Average of values. Null values are taken as 0 and averaged.
        Average = 0,
        //
        // Summary:
        //     Sum of all values.
        Sum = 1,
        //
        // Summary:
        //     Minimum value. Null values are ignored and minimum of non-null values is
        //     calculated.
        Minimum = 2,
        //
        // Summary:
        //     Maximum value.
        Maximum = 3,
        //
        // Summary:
        //     Number of rows. Null values are taken into account as well.
        Count = 4,
        //
        // Summary:
        //     Custom summary. CustomSummaryCalculator must be set to a valid instance of
        //     ICustomSummaryCalculator.
        Custom = 5,
        //
        // Summary:
        //     Formula summary. Formula assigned to Infragistics.Win.UltraWinGrid.SummarySettings.Formula
        //     property will be evaluated and used as the summary value.
        Formula = 6,
    }
}
