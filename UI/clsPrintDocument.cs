using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Data;
using System.Collections;
using System.IO;
using Base;

namespace UI
{
    public class clsPrintDocument : System.Drawing.Printing.PrintDocument
    {
        private clsPrintTemplate _Template;
        private DataSet _DataSet;
        private Hashtable _SystemValues;
        private int[] colWidth;
        private int  _PAGE;
        private clsPrintSetting _PrintSetting;
        private SortedList<string, int> RowIndex;
        private SortedList<string, DataRelation> ChildRelation;
        private SortedList<string, DataRow[]> ChildData;

        private int ColumnIndex = 0;
        private int ColumnCount = 0;
        private List<SortedList<int,clsPrintColumn>> listPageColumn;

        private int HeadHeight = 0;
        private int FooterHeight = 0;
        private int BodyHeight = 0;

        private int lastTableIndex = -1;
        private int lastTableHeaderIndex = -1;
        private int lastTableRowIndex = -1;

        public clsPrintDocument(clsPrintTemplate template, DataSet dataSet,clsPrintSetting printSetting)
        {
            _PrintSetting = printSetting;
            _Template=template;
            _DataSet=dataSet;
            _PAGE = 1;

            this.PrinterSettings = _PrintSetting.GetPrinterSettins();
            this.BeginPrint += new System.Drawing.Printing.PrintEventHandler(clsPrintDocument_BeginPrint);
            this.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(clsPrintDocument_PrintPage);

            RowIndex = new SortedList<string, int>();
            //找出主从表的关系，并建立从表与关系名的对应
            ChildRelation = new SortedList<string, DataRelation>();
            foreach (DataRelation dr in _DataSet.Relations)
            {
                ChildRelation.Add(dr.ChildTable.TableName, dr);
            }
            ChildData = new SortedList<string, DataRow[]>();
        }

        private void initSystemParameters()
		{
			_SystemValues = new Hashtable();
			_SystemValues.Add("PageNumber".ToLower(),0);
			_SystemValues.Add("TotalPages".ToLower(),0);
			_SystemValues.Add("PrintTime".ToLower(),DateTime.Now);

            RowIndex = new SortedList<string, int>();
		}
        private DataRow GetRow(string tableName)
        {
            if (!_DataSet.Tables.Contains(tableName))
                return null;
            int index = 0;
            if (RowIndex.ContainsKey(tableName))
                index = RowIndex[tableName];
            else
            {
                if (!NextRow(tableName))
                    return null;
            }

            if (ChildRelation.ContainsKey(tableName))
            {
                DataRelation dRel = ChildRelation[tableName];
                DataRow[] drs = null;
                if (!ChildData.ContainsKey(tableName))
                {
                    DataRow dr = GetRow(dRel.ParentTable.TableName);
                    drs = dr.GetChildRows(dRel);
                    ChildData.Add(tableName, drs);
                }
                else
                    drs = ChildData[tableName];
                
                return drs[index];
            }
            else
            {
                return _DataSet.Tables[tableName].Rows[index];
            }
        }
        private bool NextRow(string tableName)
        {
            if (!_DataSet.Tables.Contains(tableName))
                return false;

            int index = -1;
            if (RowIndex.ContainsKey(tableName))
                index = RowIndex[tableName];

            if (ChildRelation.ContainsKey(tableName))
            {
                DataRelation dRel = ChildRelation[tableName];
                DataRow[] drs = null;
                if (!ChildData.ContainsKey(tableName))
                {
                    DataRow dr = GetRow(dRel.ParentTable.TableName);
                    drs = dr.GetChildRows(dRel);
                    ChildData.Add(tableName, drs);
                }
                else
                    drs = ChildData[tableName];

                if (drs.Length <= index + 1)
                    return false;
            }
            else
            {
                if (_DataSet.Tables[tableName].Rows.Count <= index+1)
                    return false;
            }

            if (index>=0)
            {
                RowIndex[tableName] = index + 1;
                //找出所有子表，将他们的子记录清空
                foreach (DataRelation dr in _DataSet.Relations)
                {
                    if (dr.ParentTable.TableName == tableName)
                    {
                        if (ChildData.ContainsKey(dr.ChildTable.TableName))
                            ChildData.Remove(dr.ChildTable.TableName);
                        if (RowIndex.ContainsKey(dr.ChildTable.TableName))
                            RowIndex.Remove(dr.ChildTable.TableName);
                    }
                }
            }
            else
            {
                RowIndex.Add(tableName, 0);
            }
            return true;
        }

        void clsPrintDocument_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            initSystemParameters();
        }
        /*
        /// <summary>
        /// 1.如果是第一次执行，取得纸张的大小和算出页眉，页脚的高度，存为私有变量，备用
        /// 2.打印页眉，
        /// 3.打印主体，按Table循环，先打印表头，再按行循环，当遇到分页时，记录下执行到的Table的Index、列的Index、行的Index；
        ///   如是完成，e.HasMorePages设为false,否则是true，表示后面还有页。
        ///   分页的条件有，大小超过了纸张的高减去（页眉＋页脚的高度），达到指定行数，分页字段不同，列上超过纸张的宽度。最后一种情况，需要重复页眉页脚。
        /// 4.打印页脚
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clsPrintDocument_PrintPageheader(System.Drawing.Printing.PrintPageEventArgs e, int x, int y, SortedList<int, clsPrintColumn> listColumn)
        {
            StringFormat sf = new StringFormat();
            Pen myPen;
            myPen = new System.Drawing.Pen(System.Drawing.Color.Black);
           
            int startx = x;
            int starty = y;
            
            clsPrintRange PageHeader = _Template.PageHeader;
            clsPrintRows PageHeaderRows = PageHeader.Rows;

            DataRow pagerow = GetRow(PageHeader.Cells.DefaultTableName);

            int[] pagerowHeight = new int[PageHeaderRows.Count];
            //int pageHeight = 0;
            for (int ri = 0; ri < PageHeaderRows.Count; ri++)
            {
                pagerowHeight[ri] = PageHeaderRows[ri].Height;
                //pageHeight = pageHeight + pagerowHeight[ri];
            }

            for (int ri = 0; ri < PageHeader.Cells.Count; ri++)
            {
                clsPrintCell PageHeaderCell = PageHeader.Cells[ri];
                //只进行简单的判断,以后可以考虑增加页跨页的情况
                if (!listColumn.ContainsKey(printCell.StartColumn) && !listColumn.ContainsKey(printCell.EndColumn))
                    continue;

                int celLi = startx;
                foreach (clsPrintColumn col in listColumn)
                {
                    if (col.Index < printCell.StartColumn)
                        celLi = celLi + col.Width;
                }
                int celRi = startx;
                foreach (clsPrintColumn col in listColumn)
                {
                    if (col.Index <= printCell.EndColumn)
                        celRi = celRi + col.Width;
                }

                int celLj = starty;
                for (int nn = 0; nn < PageHeaderCell.StartRow; nn++)
                {
                    celLj = celLj + pagerowHeight[nn];
                }
                int celRj = starty;
                for (int nn = 0; nn <= PageHeaderCell.EndRow; nn++)
                {
                    celRj = celRj + pagerowHeight[nn];
                }


                Rectangle rect = new Rectangle(celLi, celLj, celRi - celLi, celRj - celLj);
                if (PageHeaderCell.BoderStyle.ToString()=="1,1,1,1")
                {
                    e.Graphics.DrawRectangle(myPen, rect);
                }

                //e.Graphics.DrawRectangle(myPen, rect);
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                string s = PageHeaderCell.Text;
                if (s == null || s == "")
                {
                    if (PageHeaderCell.Field == null || PageHeaderCell.Field == "")
                    {
                        s = PageHeaderCell.Format;
                    }
                    else
                    {
                        string[] fields = PageHeaderCell.Field.Split(',');
                        object[] p = new object[fields.Length];
                        for (int i = 0; i < fields.Length; i++)
                        {
                            if (pagerow.Table.Columns.Contains(fields[i]))
                                p[i] = pagerow[fields[i]];
                        }
                        string f = PageHeaderCell.Format;
                        if (f == null || f == "")
                            for (int i = 0; i < fields.Length; i++)
                                f += "{" + i + "}";
                        s = string.Format(f, p);
                    }
                }
                e.Graphics.DrawString(s, PageHeaderCell.Font, new SolidBrush(Color.Black), rect, sf);
            }
        }*/

        //返回true，表示由于空间原因，需要翻页；返回false，表示有空间；但对于Body而主，需要主表移动记录
        private bool clsPrintDocument_PrintPageBody(System.Drawing.Printing.PrintPageEventArgs e, int x,ref int y, SortedList<int,clsPrintColumn> listColumn, int allowHeight)
        {
            if (lastTableIndex == -1)
                lastTableIndex = 0;
            for (int i = lastTableIndex; _Template.MainBody != null && i < _Template.MainBody.Count; i++)
            {
                clsPrintTable table=_Template.MainBody[i];
                //打印表头
                if (lastTableRowIndex == -1)
                {
                    if (clsPrintDocument_PrintTableHeader(e, x, ref y, listColumn, ref allowHeight, table.TableHeader, table.TableName))
                        return true;
                    if (clsPrintDocument_PrintTableRows(e, x,ref y, listColumn, ref allowHeight, table.TableBody, table.TableName))
                        return true;
                }
                else
                {
                    if (table.ShowHeaderInPerPage)
                        if (clsPrintDocument_PrintTableHeader(e, x,ref  y, listColumn, ref allowHeight, table.TableHeader, table.TableName))
                            return true;
                    if (clsPrintDocument_PrintTableRows(e, x,ref y, listColumn, ref allowHeight, table.TableBody, table.TableName))
                        return true;
                }
            }
            lastTableIndex = -1;
            return false;
        }

        private bool clsPrintDocument_PrintTableHeader(System.Drawing.Printing.PrintPageEventArgs e, int x, ref int y, SortedList<int,clsPrintColumn> listColumn, ref int allowHeight, clsPrintRange Page,string tableName)
        {
            if (lastTableHeaderIndex == -1)
                lastTableHeaderIndex = 0;
            StringFormat sf = new StringFormat();
            Pen myPen;
            myPen = new System.Drawing.Pen(System.Drawing.Color.Black);

            int startx = x;
            int starty = y;
            
            clsPrintRows PrintRows = Page.Rows;

            if (tableName == null || tableName.Length == 0)
                tableName = Page.DefaultTableName;
            DataRow headrow = GetRow(tableName);
            if (headrow==null)
                return false;
            
            SortedList<int, clsPrintRow> printRow = new SortedList<int, clsPrintRow>();
            int t=0;
            int ri = lastTableHeaderIndex;
            for (; ri < PrintRows.Count; ri++)
            {
                if (t + PrintRows[ri].Height > allowHeight)
                    break;
                else
                {
                    t += PrintRows[ri].Height;
                    printRow.Add(ri, PrintRows[ri]);
                }
            }
            if (printRow.Count == 0)
                return true;
            allowHeight-=t;
            y += t;
            int startIndex = lastTableHeaderIndex;
            if (ri == PrintRows.Count)
                lastTableHeaderIndex = -1;

            for (ri = 0; ri < Page.Cells.Count; ri++)
            {
                clsPrintCell printCell = Page.Cells[ri];

                //只进行简单的判断,以后可以考虑增加页跨页的情况
                if (!printRow.ContainsKey(printCell.StartRow) && !printRow.ContainsKey(printCell.EndRow))
                    continue;
                if (!listColumn.ContainsKey(printCell.StartColumn) && !listColumn.ContainsKey(printCell.EndColumn))
                    continue;

                int celLi = startx;
                foreach(clsPrintColumn col in listColumn.Values)
                {
                    if (col.Index < printCell.StartColumn)
                        celLi = celLi + col.Width;
                }
                int celRi = startx;
                foreach (clsPrintColumn col in listColumn.Values)
                {
                    if (col.Index <= printCell.EndColumn)
                        celRi = celRi + col.Width;
                }

                int celLj = starty;
                for (int nn = startIndex; nn < printCell.StartRow; nn++)
                {
                    celLj = celLj + printRow[nn].Height;
                }
                int celRj = starty;
                for (int nn = startIndex; nn <= printCell.EndRow; nn++)
                {
                    celRj = celRj + printRow[nn].Height;
                }

                if (printRow.ContainsKey(printCell.StartRow))
                    for (int i = 0; i < printCell.BoderStyle.Top; i++)
                    {
                        e.Graphics.DrawLine(myPen, celLi, celLj + i, celRi, celLj + i);
                    }
                if (printRow.ContainsKey(printCell.EndRow))
                    for (int i = 0; i < printCell.BoderStyle.Bottom; i++)
                    {
                        e.Graphics.DrawLine(myPen, celLi, celRj + i, celRi, celRj + i);
                    }
                if (listColumn.ContainsKey(printCell.StartColumn))
                    for (int i = 0; i < printCell.BoderStyle.Left; i++)
                    {
                        e.Graphics.DrawLine(myPen, celLi + i, celLj, celLi + i, celRj);
                    }
                if (listColumn.ContainsKey(printCell.EndColumn))
                    for (int i = 0; i < printCell.BoderStyle.Right; i++)
                    {
                        e.Graphics.DrawLine(myPen, celRi + i, celLj, celRi + i, celRj);
                    }

                Rectangle rect = new Rectangle(celLi, celLj, celRi - celLi, celRj - celLj);
                //if (printCell.BoderStyle.ToString() == "1,1,1,1")
                //{
                //    e.Graphics.DrawRectangle(myPen, rect);
                //}

                switch (printCell.Alignment)
                {
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.TopCenter:
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.TopLeft:
                        sf.Alignment = StringAlignment.Near;
                        break;
                    case ContentAlignment.BottomRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.TopRight:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }
                sf.LineAlignment = StringAlignment.Center;
                string s = printCell.Text;
                if (s == null || s == "")
                {
                    if (printCell.Field == null || printCell.Field == "")
                    {
                        s = printCell.Format;
                    }
                    else
                    {
                        string[] fields = printCell.Field.Split(',');
                        object[] p = new object[fields.Length];
                        for (int i = 0; i < fields.Length; i++)
                            p[i] = headrow[fields[i]];
                        string f = printCell.Format;
                        if (f == null || f == "")
                            for (int i = 0; i < fields.Length; i++)
                                f += "{" + i + "}";
                        s = string.Format(f, p);
                    }
                }
                if (printCell.IsImage)
                {
                    e.Graphics.DrawImage(unSerializationImg(printCell.Image), rect);
                }
                else
                    e.Graphics.DrawString(s, printCell.Font, new SolidBrush(Color.Black), rect, sf);
            }
            return (lastTableHeaderIndex != -1);
        }

        private bool clsPrintDocument_PrintTableRows(System.Drawing.Printing.PrintPageEventArgs e, int x, ref int y, SortedList<int, clsPrintColumn> listColumn, ref int allowHeight, clsPrintRange Page, string tableName)
        {
            if (lastTableRowIndex == -1)
                lastTableRowIndex = 0;
            StringFormat sf = new StringFormat();
            Pen myPen;
            myPen = new System.Drawing.Pen(System.Drawing.Color.Black);

            int startx = x;
            int starty = y;

            clsPrintRange TableBody = Page;
            clsPrintRows PrintRows = Page.Rows;
            if (tableName == null || tableName.Length == 0)
                tableName = Page.DefaultTableName;

            //int j = 0;
            while (true)
            {
                DataRow r = GetRow(tableName);
                if (r == null)
                    return false;
                //找出各行的高度
                SortedList<int, clsPrintRow> printRow = new SortedList<int, clsPrintRow>();
                if (lastTableRowIndex == -1)
                    lastTableRowIndex = 0;
                int t = 0;
                int ri = lastTableRowIndex;
                for (; ri < PrintRows.Count; ri++)
                {
                    if (t + PrintRows[ri].Height > allowHeight)
                        return true;
                    else
                    {
                        t += PrintRows[ri].Height;
                        printRow.Add(ri, PrintRows[ri]);
                    }
                }
                if (printRow.Count == 0)
                    return true;
                allowHeight -= t;
                y += t;
                int startIndex = lastTableRowIndex;
                if (ri == PrintRows.Count)
                    lastTableRowIndex = -1;

                for (ri = 0; ri < TableBody.Cells.Count; ri++)
                {
                    clsPrintCell printCell = TableBody.Cells[ri];

                    //只进行简单的判断,以后可以考虑增加页跨页的情况
                    if (!printRow.ContainsKey(printCell.StartRow) && !printRow.ContainsKey(printCell.EndRow))
                        continue;
                    if (!listColumn.ContainsKey(printCell.StartColumn) && !listColumn.ContainsKey(printCell.EndColumn))
                        continue;

                    int celLi = startx;
                    foreach (clsPrintColumn col in listColumn.Values)
                    {
                        if (col.Index < printCell.StartColumn)
                            celLi = celLi + col.Width;
                    }
                    int celRi = startx;
                    foreach (clsPrintColumn col in listColumn.Values)
                    {
                        if (col.Index <= printCell.EndColumn)
                            celRi = celRi + col.Width;
                    }

                    int celLj = starty;
                    for (int nn = startIndex; nn < printCell.StartRow; nn++)
                    {
                        celLj = celLj + printRow[nn].Height;
                    }
                    int celRj = starty;
                    for (int nn = startIndex; nn <= printCell.EndRow; nn++)
                    {
                        celRj = celRj + printRow[nn].Height;
                    }

                    myPen = new System.Drawing.Pen(System.Drawing.Color.Black);

                    if (printRow.ContainsKey(printCell.StartRow))
                        for (int i = 0; i < printCell.BoderStyle.Top; i++)
                        {
                            e.Graphics.DrawLine(myPen, celLi, celLj + i, celRi, celLj + i);
                        }
                    if (printRow.ContainsKey(printCell.EndRow))
                        for (int i = 0; i < printCell.BoderStyle.Bottom; i++)
                        {
                            e.Graphics.DrawLine(myPen, celLi, celRj + i, celRi, celRj + i);
                        }
                    if (listColumn.ContainsKey(printCell.StartColumn))
                        for (int i = 0; i < printCell.BoderStyle.Left; i++)
                        {
                            e.Graphics.DrawLine(myPen, celLi + i, celLj, celLi + i, celRj);
                        }
                    if (listColumn.ContainsKey(printCell.EndColumn))
                        for (int i = 0; i < printCell.BoderStyle.Right; i++)
                        {
                            e.Graphics.DrawLine(myPen, celRi + i, celLj, celRi + i, celRj);
                        }

                    Rectangle rect = new Rectangle(celLi, celLj, celRi - celLi, celRj - celLj);

                    switch (printCell.Alignment)
                    {
                        case ContentAlignment.BottomCenter:
                        case ContentAlignment.MiddleCenter:
                        case ContentAlignment.TopCenter:
                            sf.Alignment = StringAlignment.Center;
                            break;
                        case ContentAlignment.BottomLeft:
                        case ContentAlignment.MiddleLeft:
                        case ContentAlignment.TopLeft:
                            sf.Alignment = StringAlignment.Near;
                            break;
                        case ContentAlignment.BottomRight:
                        case ContentAlignment.MiddleRight:
                        case ContentAlignment.TopRight:
                            sf.Alignment = StringAlignment.Far;
                            break;
                    }
                    sf.LineAlignment = StringAlignment.Center;

                    string s = printCell.Text;
                    if (s == null || s == "")
                    {
                        if (printCell.Field == null || printCell.Field == "")
                        {
                            s = printCell.Format;
                        }
                        else
                        {
                            string[] fields = printCell.Field.Split(',');
                            object[] p = new object[fields.Length];
                            for (int iField = 0; iField < fields.Length; iField++)
                                p[iField] = r[fields[iField]];
                            string f = printCell.Format;
                            if (f == null || f == "")
                                for (int iField = 0; iField < fields.Length; iField++)
                                    f += "{" + iField + "}";
                            s = string.Format(f, p);
                        }
                    }
                    if (printCell.IsImage)
                    {
                        e.Graphics.DrawImage(unSerializationImg(printCell.Image), rect);
                    }
                    else
                        e.Graphics.DrawString(s, printCell.Font, new SolidBrush(Color.Black), rect, sf);
                    //e.Graphics.DrawString(s, printCell.Font, new SolidBrush(Color.Black), rect, sf);
                }
                //j++;
                starty += t;
                if (!NextRow(tableName))
                    return false;
            }
        }

        void clsPrintDocument_PrintPageHeaderOrFooter(System.Drawing.Printing.PrintPageEventArgs e, int x, int y, SortedList<int, clsPrintColumn> listColumn, clsPrintRange Page)
        {
            if (Page == null)
                return;
            StringFormat sf = new StringFormat();
            Pen myPen;
            myPen = new System.Drawing.Pen(System.Drawing.Color.Black);

            int startx = x;
            int starty = y;

            //clsPrintRange Page = _Template.PageFooter;
            clsPrintRows PrintRows = Page.Rows;
            DataRow pageRow = GetRow(Page.Cells.DefaultTableName);

            SortedList<int, clsPrintRow> printRow = new SortedList<int, clsPrintRow>();
            //int[] rowHeight = new int[PrintRows.Count];
            for (int ri = 0; ri < PrintRows.Count; ri++)
            {
                printRow.Add(ri, PrintRows[ri]);
            }

            for (int ri = 0; ri < Page.Cells.Count; ri++)
            {
                clsPrintCell printCell = Page.Cells[ri];
                //只进行简单的判断,以后可以考虑增加页跨页的情况
                if (!listColumn.ContainsKey(printCell.StartColumn) && !listColumn.ContainsKey(printCell.EndColumn))
                    continue;

                int celLi = startx;
                foreach (clsPrintColumn col in listColumn.Values)
                {
                    if (col.Index < printCell.StartColumn)
                        celLi = celLi + col.Width;
                }
                int celRi = startx;
                foreach (clsPrintColumn col in listColumn.Values)
                {
                    if (col.Index <= printCell.EndColumn)
                        celRi = celRi + col.Width;
                }

                int celLj = starty;
                for (int nn = 0; nn < printCell.StartRow; nn++)
                {
                    celLj = celLj + printRow[nn].Height;
                }
                int celRj = starty;
                for (int nn = 0; nn <= printCell.EndRow; nn++)
                {
                    celRj = celRj + printRow[nn].Height;
                }

                if (printRow.ContainsKey(printCell.StartRow))
                    for (int i = 0; i < printCell.BoderStyle.Top;i++ )
                    {
                        e.Graphics.DrawLine(myPen, celLi, celLj + i, celRi, celLj + i);
                    }
                if (printRow.ContainsKey(printCell.EndRow))
                    for (int i = 0; i < printCell.BoderStyle.Bottom;i++ )
                    {
                        e.Graphics.DrawLine(myPen, celLi, celRj + i, celRi, celRj + i);
                    }
                if (listColumn.ContainsKey(printCell.StartColumn))
                    for (int i = 0; i < printCell.BoderStyle.Left;i++ )
                    {
                        e.Graphics.DrawLine(myPen, celLi + i, celLj, celLi + i, celRj);
                    }
                if (listColumn.ContainsKey(printCell.EndColumn))
                    for (int i=0;i<printCell.BoderStyle.Right;i++)
                    {
                        e.Graphics.DrawLine(myPen, celRi + i, celLj, celRi + i, celRj);
                    }

                Rectangle rect = new Rectangle(celLi, celLj, celRi - celLi, celRj - celLj);
                //if (printCell.BoderStyle.ToString() == "1,1,1,1")
                //{
                //    e.Graphics.DrawRectangle(myPen, rect);
                //}

                //e.Graphics.DrawRectangle(myPen, rect);
                if (!listColumn.ContainsKey(printCell.StartColumn) || !printRow.ContainsKey(printCell.StartRow))
                    continue;
                switch (printCell.Alignment)
                {
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.TopCenter:
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.TopLeft:
                        sf.Alignment = StringAlignment.Near;
                        break;
                    case ContentAlignment.BottomRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.TopRight:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }
                sf.LineAlignment = StringAlignment.Center;
                string s = printCell.Text;
                if (s == null || s == "")
                {
                    if (printCell.Field == null || printCell.Field == "")
                    {
                        s = printCell.Format;
                    }
                    else
                    {
                        string[] fields = printCell.Field.Split(',');
                        object[] p = new object[fields.Length];
                        for (int i = 0; i < fields.Length; i++)
                        {
                            if (pageRow != null && pageRow.Table.Columns.Contains(fields[i]))
                                p[i] = pageRow[fields[i]];
                        }
                        string f = printCell.Format;
                        if (f == null || f == "")
                            for (int i = 0; i < fields.Length; i++)
                                f += "{" + i + "}";
                        s = string.Format(f, p);
                    }
                }
                else if(s.Contains("${"))
                {
                    string reps = s.Substring(2, s.Length -3);
                    if (this._SystemValues.ContainsKey(reps.ToLower()))
                    {
                        string format = printCell.Format;
                        if (format.Length > 0)
                        {
                            s = string.Format(format, _SystemValues[reps.ToLower()]);
                        }else
                            s = VarConverTo.ConvertToString(_SystemValues[reps.ToLower()]);
                    }
                }
                if (printCell.IsImage)
                {
                    e.Graphics.DrawImage(unSerializationImg(printCell.Image), rect);
                }
                else
                    e.Graphics.DrawString(s, printCell.Font, new SolidBrush(Color.Black), rect, sf);
            }
        }
        // 反序列化并显示
        private Image unSerializationImg(string strImg)
        {
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(strImg));
            System.Runtime.Serialization.IFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Image img = (Image)formater.Deserialize(stream);
            stream.Close();
            return img;
        }
        private int indexPageColumn = 0;
        void clsPrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show("开始");
            //初始化时
            //先根据左右边据和各列宽度，算出横有几页，以及各页有几列
            if (listPageColumn == null)
            {
                listPageColumn = new List<SortedList<int,clsPrintColumn>>();
                int maxWidth = e.MarginBounds.Width;// -e.MarginBounds.Left - e.MarginBounds.Right;

                clsPrintColumns PrintColumns = _Template.Columns;
                //colWidth = new int[PrintColumns.Count];
                int lastWidth = 0;
                SortedList<int,clsPrintColumn> lastPage = new SortedList<int,clsPrintColumn>();
                listPageColumn.Add(lastPage);
                for (int ci = 0; ci < PrintColumns.Count; ci++)
                {
                    //colWidth[ci] = PrintColumns[ci].Width;
                    if (lastWidth + PrintColumns[ci].Width <= maxWidth)
                    {
                        lastWidth += PrintColumns[ci].Width;
                        lastPage.Add(ci, PrintColumns[ci]);
                    }
                    else
                    {
                        lastPage = new SortedList<int,clsPrintColumn>();
                        listPageColumn.Add(lastPage);
                        lastPage.Add(ci, PrintColumns[ci]);
                        lastWidth = PrintColumns[ci].Width;
                    }
                }
                //System.Windows.Forms.MessageBox.Show("计算高度前");
                //计算页眉高度和页脚高度
                clsPrintRange PageHeader = _Template.PageHeader;
                for (int i = 0;PageHeader!=null && i < PageHeader.Rows.Count; i++)
                {
                    HeadHeight += PageHeader.Rows[i].Height;
                }
                clsPrintRange PageFooter = _Template.PageFooter;
                int FooterHeight = 0;
                for (int i = 0; PageFooter!=null && i < PageFooter.Rows.Count; i++)
                {
                    FooterHeight += PageFooter.Rows[i].Height;
                }
                BodyHeight = e.MarginBounds.Height - HeadHeight - FooterHeight;//- e.MarginBounds.Top - e.MarginBounds.Bottom 
            }

            //System.Windows.Forms.MessageBox.Show("打印前");
            //打印
            int x = e.MarginBounds.Left;
            int y = e.MarginBounds.Top;
            //在循环前对RowIndex进行备份，以保证，每个横向页是同步的
            SortedList<string, int> lastRowIndex=new SortedList<string,int>();
            foreach (KeyValuePair<string, int> kv in RowIndex)
                lastRowIndex.Add(kv.Key, kv.Value);
            //根据横向列进行循环
            if (indexPageColumn < listPageColumn.Count)
            {
                //System.Windows.Forms.MessageBox.Show("开始横向列循环");
                SortedList<int, clsPrintColumn> listColumn = listPageColumn[indexPageColumn];
                //先打印页眉
                clsPrintDocument_PrintPageHeaderOrFooter(e, x, y, listColumn, _Template.PageHeader);
                //再打印主页
                y = y + HeadHeight;
                bool bBody = clsPrintDocument_PrintPageBody(e, x,ref y, listColumn, BodyHeight);
                //最后打印页脚
                clsPrintDocument_PrintPageHeaderOrFooter(e, x, y, listColumn, _Template.PageFooter);
                if (bBody)
                    e.HasMorePages = true;
                //当不是最后一个时，需要恢复一下RowIndex
                if (indexPageColumn < listPageColumn.Count - 1)
                {
                    RowIndex = new SortedList<string, int>();
                    foreach (KeyValuePair<string, int> kv in lastRowIndex)
                        RowIndex.Add(kv.Key, kv.Value);
                    indexPageColumn++;
                    e.HasMorePages = true;
                }
                else
                {
                    indexPageColumn = 0;

                    if (!e.HasMorePages)
                    {
                        if (_Template.PageHeader!=null && _Template.PageHeader.DefaultTableName.Length > 0)
                            e.HasMorePages = NextRow(_Template.PageHeader.DefaultTableName);
                        else if (_Template.PageFooter!=null && _Template.PageFooter.DefaultTableName.Length > 0)
                            e.HasMorePages = NextRow(_Template.PageHeader.DefaultTableName);
                    }
                }
            }
            //System.Windows.Forms.MessageBox.Show("结束");
            /*clsPrintRange PageHeader = _Template.PageHeader;
            int headHeight = 0;
            for (int i = 0; i < PageHeader.Rows.Count; i++)
            {
                headHeight = headHeight + PageHeader.Rows[i].Height;
            }
            clsPrintRange PageFooter = _Template.PageFooter;
            int FooterHeight = 0;
            for (int i = 0; i < PageFooter.Rows.Count; i++)
            {
                FooterHeight = FooterHeight + PageFooter.Rows[i].Height;
            }

            clsPrintRange TableHeader = _Template.MainBody[0].TableHeader;
            int TableHeaderHeight = 0;
            for (int i = 0; i < TableHeader.Rows.Count; i++)
            {
                TableHeaderHeight = TableHeaderHeight + TableHeader.Rows[i].Height;
            }
            clsPrintRange TableBody = _Template.MainBody[0].TableBody;
            int TableBodyRowHeight = 0;
            for (int i = 0; i < TableBody.Rows.Count; i++)
            {
                TableBodyRowHeight = TableBodyRowHeight + TableBody.Rows[i].Height;
            }
            DataTable dt = _DataSet.Tables[_Template.MainBody[0].TableName];
            int  TableBodyRowCount=0;
            if (dt!=null)
                TableBodyRowCount=dt.Rows.Count;


            int pageHeight = e.MarginBounds.Height;
            int x = e.MarginBounds.Left;
            int y = e.MarginBounds.Top;
            if (pageHeight > headHeight + FooterHeight + TableHeaderHeight + TableBodyRowHeight * TableBodyRowCount)
            {
                //int y = e.MarginBounds.Top;
                //处理页头
                clsPrintDocument_PrintPageheader(e, x, y);
                //处理表格头
                y = y + headHeight;
                clsPrintDocument_PrintTableheader(e, x, y);
                //处理表格主体
                y = y + TableHeaderHeight;
                clsPrintDocument_PrintTableRows(e, x, y, 1, TableBodyRowCount);
                //处理页脚
                y = y + TableBodyRowHeight * (TableBodyRowCount);
                clsPrintDocument_PrintPageFooter(e, x, y);

                e.HasMorePages = false;
            }
            else
            {
                int pageRows = (int)((pageHeight - headHeight - FooterHeight - TableHeaderHeight) / TableBodyRowHeight);
                int pageTotle = (int)Math.Ceiling((double)TableBodyRowCount / (double)pageRows);
                int j = _PAGE - 1;
                //int y = e.MarginBounds.Top;
                //处理页头
                clsPrintDocument_PrintPageheader(e, x, y);
                //处理表格头
                y = y + headHeight;
                clsPrintDocument_PrintTableheader(e, x, y);
                //处理表格主体
                y = y + TableHeaderHeight;
                int endRow = 0;
                if ((j + 1) * pageRows > TableBodyRowCount)
                { endRow = TableBodyRowCount; }
                else { endRow = (j + 1) * pageRows; }
                clsPrintDocument_PrintTableRows(e, x, y, 1 + j * pageRows, endRow);
                //处理页脚
                y = y + TableBodyRowHeight * (pageRows);
                clsPrintDocument_PrintPageFooter(e, x, y);

                //e.Graphics.DrawString("第 " + _PAGE.ToString() + "页 ", new Font("Tahoma", 20), new SolidBrush(Color.Black), 100, 900); 
                e.Graphics.DrawString("第 " + _PAGE.ToString() + "页 ", new Font("Tahoma", 10), new SolidBrush(Color.Black), e.MarginBounds.Width / 2 - 10, (e.MarginBounds.Height + e.MarginBounds.Bottom) / 2);
                string sss = "";
                sss = sss + "||" + (1 + j * pageRows).ToString();
                sss = sss + "||" + endRow.ToString();
                sss = sss + "||" + pageRows.ToString();
                sss = sss + "||" + pageTotle.ToString();
                sss = sss + "|*|" + pageHeight.ToString();
                sss = sss + "|*|" + _PAGE.ToString();
                e.Graphics.DrawString(sss, new Font("Tahoma", 10), new SolidBrush(Color.Black), new Rectangle(0, j * 100, 400, 100), new StringFormat());
                _PAGE = _PAGE + 1;

                if (_PAGE <= pageTotle)
                {
                    e.HasMorePages = true;
                }
                else
                {
                    e.HasMorePages = false;
                }
            }*/
        }
    }
}
