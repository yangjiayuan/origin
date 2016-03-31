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
            //�ҳ����ӱ�Ĺ�ϵ���������ӱ����ϵ���Ķ�Ӧ
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
                //�ҳ������ӱ������ǵ��Ӽ�¼���
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
        /// 1.����ǵ�һ��ִ�У�ȡ��ֽ�ŵĴ�С�����ҳü��ҳ�ŵĸ߶ȣ���Ϊ˽�б���������
        /// 2.��ӡҳü��
        /// 3.��ӡ���壬��Tableѭ�����ȴ�ӡ��ͷ���ٰ���ѭ������������ҳʱ����¼��ִ�е���Table��Index���е�Index���е�Index��
        ///   ������ɣ�e.HasMorePages��Ϊfalse,������true����ʾ���滹��ҳ��
        ///   ��ҳ�������У���С������ֽ�ŵĸ߼�ȥ��ҳü��ҳ�ŵĸ߶ȣ����ﵽָ����������ҳ�ֶβ�ͬ�����ϳ���ֽ�ŵĿ�ȡ����һ���������Ҫ�ظ�ҳüҳ�š�
        /// 4.��ӡҳ��
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
                //ֻ���м򵥵��ж�,�Ժ���Կ�������ҳ��ҳ�����
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

        //����true����ʾ���ڿռ�ԭ����Ҫ��ҳ������false����ʾ�пռ䣻������Body��������Ҫ�����ƶ���¼
        private bool clsPrintDocument_PrintPageBody(System.Drawing.Printing.PrintPageEventArgs e, int x,ref int y, SortedList<int,clsPrintColumn> listColumn, int allowHeight)
        {
            if (lastTableIndex == -1)
                lastTableIndex = 0;
            for (int i = lastTableIndex; _Template.MainBody != null && i < _Template.MainBody.Count; i++)
            {
                clsPrintTable table=_Template.MainBody[i];
                //��ӡ��ͷ
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

                //ֻ���м򵥵��ж�,�Ժ���Կ�������ҳ��ҳ�����
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
                //�ҳ����еĸ߶�
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

                    //ֻ���м򵥵��ж�,�Ժ���Կ�������ҳ��ҳ�����
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
                //ֻ���м򵥵��ж�,�Ժ���Կ�������ҳ��ҳ�����
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
        // �����л�����ʾ
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
            //System.Windows.Forms.MessageBox.Show("��ʼ");
            //��ʼ��ʱ
            //�ȸ������ұ߾ݺ͸��п�ȣ�������м�ҳ���Լ���ҳ�м���
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
                //System.Windows.Forms.MessageBox.Show("����߶�ǰ");
                //����ҳü�߶Ⱥ�ҳ�Ÿ߶�
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

            //System.Windows.Forms.MessageBox.Show("��ӡǰ");
            //��ӡ
            int x = e.MarginBounds.Left;
            int y = e.MarginBounds.Top;
            //��ѭ��ǰ��RowIndex���б��ݣ��Ա�֤��ÿ������ҳ��ͬ����
            SortedList<string, int> lastRowIndex=new SortedList<string,int>();
            foreach (KeyValuePair<string, int> kv in RowIndex)
                lastRowIndex.Add(kv.Key, kv.Value);
            //���ݺ����н���ѭ��
            if (indexPageColumn < listPageColumn.Count)
            {
                //System.Windows.Forms.MessageBox.Show("��ʼ������ѭ��");
                SortedList<int, clsPrintColumn> listColumn = listPageColumn[indexPageColumn];
                //�ȴ�ӡҳü
                clsPrintDocument_PrintPageHeaderOrFooter(e, x, y, listColumn, _Template.PageHeader);
                //�ٴ�ӡ��ҳ
                y = y + HeadHeight;
                bool bBody = clsPrintDocument_PrintPageBody(e, x,ref y, listColumn, BodyHeight);
                //����ӡҳ��
                clsPrintDocument_PrintPageHeaderOrFooter(e, x, y, listColumn, _Template.PageFooter);
                if (bBody)
                    e.HasMorePages = true;
                //���������һ��ʱ����Ҫ�ָ�һ��RowIndex
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
            //System.Windows.Forms.MessageBox.Show("����");
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
                //����ҳͷ
                clsPrintDocument_PrintPageheader(e, x, y);
                //������ͷ
                y = y + headHeight;
                clsPrintDocument_PrintTableheader(e, x, y);
                //����������
                y = y + TableHeaderHeight;
                clsPrintDocument_PrintTableRows(e, x, y, 1, TableBodyRowCount);
                //����ҳ��
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
                //����ҳͷ
                clsPrintDocument_PrintPageheader(e, x, y);
                //������ͷ
                y = y + headHeight;
                clsPrintDocument_PrintTableheader(e, x, y);
                //����������
                y = y + TableHeaderHeight;
                int endRow = 0;
                if ((j + 1) * pageRows > TableBodyRowCount)
                { endRow = TableBodyRowCount; }
                else { endRow = (j + 1) * pageRows; }
                clsPrintDocument_PrintTableRows(e, x, y, 1 + j * pageRows, endRow);
                //����ҳ��
                y = y + TableBodyRowHeight * (pageRows);
                clsPrintDocument_PrintPageFooter(e, x, y);

                //e.Graphics.DrawString("�� " + _PAGE.ToString() + "ҳ ", new Font("Tahoma", 20), new SolidBrush(Color.Black), 100, 900); 
                e.Graphics.DrawString("�� " + _PAGE.ToString() + "ҳ ", new Font("Tahoma", 10), new SolidBrush(Color.Black), e.MarginBounds.Width / 2 - 10, (e.MarginBounds.Height + e.MarginBounds.Bottom) / 2);
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
