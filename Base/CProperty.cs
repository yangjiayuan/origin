using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    public class CTableProperty
    {
        public enum enuTableType : int { MainTable = 0, ChildTable = 1, View = 2 };
        public string ID;
        public string TableName;
        public string Title;
        public bool AutoLoad;
        public bool Visible;
        public string ParentTableName;
        public enuTableType TableType;
        public int GroupBy;
        public int OrderBy;
        public bool HistoryTable;
        public override string ToString()
        {
            return Title;
        }
        public CTableProperty Clone()
        {
            CTableProperty pty = new CTableProperty();
            pty.ID = this.ID;
            pty.TableName = this.TableName;
            pty.Title = this.Title;
            pty.Visible = this.Visible;
            pty.ParentTableName = this.ParentTableName;
            pty.TableType = this.TableType;
            pty.GroupBy = this.GroupBy;
            pty.OrderBy = this.OrderBy;
            pty.HistoryTable = this.HistoryTable;
            return pty;
        }
    }
}
