using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinEditors;
using System.Data;
using Base;

namespace UI
{
    public static class CommonTools
    {
        public static void SetVisble(COMFields table, string[] fields, COMField.Enum_Visible visible, bool enable)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                table[fields[i]].Visible = visible;
                table[fields[i]].Enable = enable;
                if (!enable && table[fields[i]].Mandatory)
                    table[fields[i]].Mandatory = false;
            }
        }
        public static void SetMandatory(COMFields table, string[] fields, bool mandatory)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                table[fields[i]].Mandatory = mandatory;
            }
        }
        public static void SetGroupName(COMFields table, string[] fields, string groupName)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                table[fields[i]].GroupName = groupName;
            }
        }
        public static DataRow SetDictValueWithRight(UltraTextEditor editor, string dictName, string field, string value, string other)
        {
            frmBrowser frm = (frmBrowser)CSystem.Sys.Svr.Right.GetForm(dictName);

            if (frm == null)
            {
                List<COMFields> detailTable = CSystem.Sys.Svr.Properties.DetailTableDefineList(dictName);
                COMFields mainTable = CSystem.Sys.Svr.LDI.GetFields(dictName);

                frm = new frmBrowser(mainTable, detailTable, null, enuShowStatus.None);
            }

            if (frm.MainTableDefine.FieldNameList(false).Contains("Disable"))
            {
                if (other == null || other.Length == 0)
                    other = dictName + ".Disable=0";
                else
                    other += " and " + dictName + ".Disable=0";
            }
            DataRow dr = frm.ShowSelect(null, null, other);
            if (dr != null)
                editor.Tag = dr["ID"];
            return dr;
        }
        public static DataRow SetDictValue(UltraTextEditor editor, string dictName, string field, string value, string other)
        {
            List<COMFields> detailTable = CSystem.Sys.Svr.Properties.DetailTableDefineList(dictName);
            COMFields mainTable = CSystem.Sys.Svr.LDI.GetFields(dictName);

            frmBrowser frm = new frmBrowser(mainTable, detailTable, null, enuShowStatus.None);

            if (frm.MainTableDefine.FieldNameList(false).Contains("Disable"))
            {
                if (other == null || other.Length == 0)
                    other = dictName + ".Disable=0";
                else
                    other += " and " + dictName + ".Disable=0";
            }
            DataRow dr = frm.ShowSelect(null, null, other);
            if (dr != null)
                editor.Tag = dr["ID"];
            return dr;
        }
        public static DataRow SetDataValue(UltraTextEditor editor, string dataName, string field, string value, string other)
        {
            frmBrowser frm = (frmBrowser)CSystem.Sys.Svr.Right.GetForm(dataName);

            if (frm == null)
            {
                List<COMFields> detailTable = CSystem.Sys.Svr.Properties.DetailTableDefineList(dataName);
                COMFields mainTable = CSystem.Sys.Svr.LDI.GetFields(dataName);

                frm = new frmBrowser(mainTable, detailTable, null, enuShowStatus.None);
            }
            DataRow dr = frm.ShowSelect(field, value, other);
            if (dr != null)
                editor.Tag = dr["ID"];
            return dr;
        }
    }
}
