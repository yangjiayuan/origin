using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Cells = SourceGrid.Cells;
using System.Diagnostics;
using Base;
using System.Runtime.Serialization;
using System.IO;

namespace UI
{
    public partial class frmTemplate : Form
    {
        private clsPrintTemplate template;
        private CellView cellView;
        private int x;
        private int y;
        public frmTemplate()
        {
            InitializeComponent();
        }

        /* 列头,行头都以Button形式体现
         * 其中行头,以多层的形式体现,所有行头,占四列
         * 页眉区分页眉,行头,前三个列合一个单元格,后一列为行头
         * Body,Table,TableHeader,Row四列
         * Body,Table,TableBody,Row四列
         * 页脚区分页脚,行头,前三个列合一个单元格,后一列为行头
         * Property对象有:页眉,页脚,Body,Table,TableHeader,TableBody,行,单元格，列
         */
        private void toolStripNew_Click(object sender, EventArgs e)
        {
            gridTemplate.Rows.Clear();
            gridTemplate.Columns.Clear();
            template = new clsPrintTemplate();
            gridTemplate.RowsCount = 1;
            gridTemplate.ColumnsCount = 4 + 5;
            //左上角
            Cells.RowHeader rh = new SourceGrid.Cells.RowHeader();
            rh.Tag = template;
            rh.ColumnSpan = 4;
            rh.Value = "左上角";
            gridTemplate.SetCell(0, 0, rh);


            //默认先建五列
            for (int i = 0; i < 5; i++)
            {
                clsPrintColumn col = template.Columns.NewChild();
                col.Index = i;
                col.Width = 80;

                PrintColumnHeader ch = new PrintColumnHeader();
                ch.Tag = col;
                ch.Value = i+1;
                gridTemplate.SetCell(0, 4 + i, ch);
                gridTemplate.Columns[4 + i].Width = 80;
            }
        }


        private void toolStripInsertPageHeader_Click(object sender, EventArgs e)
        {
            if (template == null)
                return;
            if (template.PageHeader == null)
            {
                clsPrintRange header = template.NewPageHeader();
                clsPrintRow row = header.Rows.NewChild();
                Cells.RowHeader phh = new SourceGrid.Cells.RowHeader();
                phh.Tag = header;
                phh.ColumnSpan = 3;


                phh.Value = "页眉";
                gridTemplate.Rows.Insert(1);
                gridTemplate.Rows[1].Height = row.Height;
                gridTemplate.SetCell(1, 0, phh);

                Cells.RowHeader phr = new SourceGrid.Cells.RowHeader();
                phr.Tag = row;
                phr.Value = 1;
                gridTemplate.SetCell(1, 3, phr);

                NewCellByRow(0, header, 1);
            }
        }

        private void toolStripInsertLine_Click(object sender, EventArgs e)
        {
            if (template == null)
                return;
            //取第三列
            Cells.ICell cell = gridTemplate[y, 2];
            object o = cell.Tag;
            if (o.GetType() == typeof(clsPrintRange))
            {
                clsPrintRange range = (clsPrintRange)o;
                clsPrintRow row = range.Rows.InsertChild(y - cell.Row.Index);

                //将后续的行改名
                for (int j = y; j < gridTemplate.Rows.Count; j++)
                {
                    if (cell == gridTemplate[j, 2])
                    {
                        gridTemplate[j, 3].Value = j - cell.Row.Index + 2;
                    }
                    else
                        break;
                }

                gridTemplate.Rows.Insert(y);
                NewCellByEmptyRow(y - cell.Row.Index + 1, range, y);

                //将前面的列，增加行高
                for (int j = 0; j < 3; j++)
                {
                    Cells.ICell sourceCell = gridTemplate[y + 1, j];
                    if (sourceCell != null)
                    {
                        if (sourceCell.Row.Index == y + 1 && sourceCell.Column.Index == j)
                        {
                            Cells.RowHeader newCell = new SourceGrid.Cells.RowHeader();
                            newCell.RowSpan = sourceCell.RowSpan + 1;
                            newCell.ColumnSpan = sourceCell.ColumnSpan;
                            newCell.Value = sourceCell.Value;
                            newCell.Tag = sourceCell.Tag;
                            gridTemplate[y + 1, j] = null;
                            gridTemplate.SetCell(y, j, newCell);
                        }
                        else if (sourceCell.Column.Index == j)
                            sourceCell.RowSpan++;
                    }
                    else
                    {
                        gridTemplate[y, j].RowSpan++;
                    }
                }

                Cells.RowHeader rowH = new SourceGrid.Cells.RowHeader();
                rowH.Value = y - gridTemplate[y, 2].Row.Index + 1;
                gridTemplate.SetCell(y, 3, rowH);
                gridTemplate.Rows[y].Height = row.Height;

            }
        }

        private void NewCellByEmptyRow(int i, clsPrintRange range, int y)
        {
            int col = 0;
            for (int j = 4; j < gridTemplate.Columns.Count; j++)
            {
                Cells.Cell cell = new SourceGrid.Cells.Cell();
                cell.View = cellView;
                clsPrintCell pc = range.Cells.NewChild();
                pc.StartColumn = col;
                pc.EndColumn = col;
                pc.StartRow = y - 1;
                pc.EndRow = y - 1;
                cell.Tag = pc;
                cell.RowSpan = 1;
                cell.ColumnSpan = 1;
                gridTemplate.SetCell(y, col + 4, cell);
                col += 1;
            }
        }
        private void toolStripInsertTable_Click(object sender, EventArgs e)
        {
            if (template == null)
                return;
            int y = 1;
            if (template.PageHeader != null)
                y += template.PageHeader.RowCount;

            if (template.MainBody == null)
            {
                Cells.RowHeader rowMainBody = new SourceGrid.Cells.RowHeader();
                rowMainBody.Tag = template.NewMainBody();
                rowMainBody.RowSpan = 2;
                rowMainBody.Value = "主页";

                gridTemplate.Rows.Insert(y);
                gridTemplate.Rows.Insert(y);
                gridTemplate.SetCell(y, 0, rowMainBody);
            }
            else
            {
                for (int i = 0; i < template.MainBody.Count; i++)
                    y += template.MainBody[i].RowCount;

                //y++;
                gridTemplate.Rows.Insert(y);
                gridTemplate.Rows.Insert(y);
                gridTemplate[y - 1, 0].RowSpan = gridTemplate[y - 1, 0].RowSpan + 2;
            }


            clsPrintTable table = template.MainBody.NewChild();

            Cells.RowHeader rowTable = new SourceGrid.Cells.RowHeader();
            rowTable.Tag = table;
            rowTable.RowSpan = 2;
            rowTable.Value = "表" + template.MainBody.Count;
            gridTemplate.SetCell(y, 1, rowTable);

            Cells.RowHeader rowTableHeader = new SourceGrid.Cells.RowHeader();
            rowTableHeader.Tag = table.TableHeader;
            rowTableHeader.Value = "表头";
            gridTemplate.SetCell(y, 2, rowTableHeader);

            Cells.RowHeader rowTableRow = new SourceGrid.Cells.RowHeader();
            rowTableRow.Tag = table.TableHeader.Rows.NewChild();
            rowTableRow.Value = 1;
            gridTemplate.SetCell(y, 3, rowTableRow);
            gridTemplate.Rows[y].Height = table.TableHeader.Rows[0].Height;
            NewCellByRow(0, table.TableHeader, y);

            y++;
            Cells.RowHeader rowTableBody = new SourceGrid.Cells.RowHeader();
            rowTableBody.Tag = table.TableBody;
            rowTableBody.Value = "表体";
            gridTemplate.SetCell(y, 2, rowTableBody);

            Cells.RowHeader rowTableBodyRow = new SourceGrid.Cells.RowHeader();
            rowTableBodyRow.Tag = table.TableBody.Rows.NewChild();
            rowTableBodyRow.Value = 1;
            gridTemplate.SetCell(y, 3, rowTableBodyRow);
            gridTemplate.Rows[y].Height = table.TableBody.Rows[0].Height;
            NewCellByRow(0, table.TableBody, y);
        }

        private void toolStripInsertFooter_Click(object sender, EventArgs e)
        {
            if (template == null)
                return;
            if (template.PageFooter == null)
            {
                int y = 1;
                if (template.PageHeader != null)
                    y += template.PageHeader.RowCount;

                if (template.MainBody != null)
                {
                    for (int i = 0; i < template.MainBody.Count; i++)
                        y += template.MainBody[i].RowCount;
                }

                clsPrintRange footer = template.NewPageFooter();
                clsPrintRow row = footer.Rows.NewChild();
                Cells.RowHeader pfr = new SourceGrid.Cells.RowHeader();
                pfr.Tag = footer;
                pfr.ColumnSpan = 3;


                pfr.Value = "页脚";
                gridTemplate.Rows.Insert(y);

                gridTemplate.Rows[y].Height = row.Height;
                gridTemplate.SetCell(y, 0, pfr);

                Cells.RowHeader phr = new SourceGrid.Cells.RowHeader();
                phr.Tag = row;
                phr.Value = 1;
                gridTemplate.SetCell(y, 3, phr);

                for (int i = 0; i < template.Columns.Count; i++)
                {
                    clsPrintCell cell = footer.Cells.NewChild();
                    cell.StartColumn = i;
                    cell.StartRow = 0;
                    cell.EndColumn = i;
                    cell.EndRow = 0;

                    Cells.Cell phc = new SourceGrid.Cells.Cell();
                    phc.Tag = cell;
                    gridTemplate.SetCell(y, i + 4, phc);
                }
            }
        }

        private void toolStripClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripSave_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter="打印模板定义文件(*.xml)|*.xml";   

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                template.Save(sfd.FileName);
            }
        }

        private void toolStripInsertColumn_Click(object sender, EventArgs e)
        {
            if (x > 3 && template!=null)
            {
                clsPrintColumn col = template.Columns.InsertChild(x - 4);
                col.Width = 80;

                //将后续的列改名
                for (int j = x; j < gridTemplate.Columns.Count; j++)
                {
                    gridTemplate[0, j].Value = j - 2;
                }
                
                gridTemplate.Columns.Insert(x);
                Cells.ColumnHeader colH = new SourceGrid.Cells.ColumnHeader();
                colH.Value = x - 3;
                gridTemplate.SetCell(0, x, colH);
                gridTemplate.Columns[x].Width = 80;

                //找出所有列为x-4的单格，如果找到后，新增这些单格到GRid
                int y = 1;
                int i = x - 4;
                if (template.PageHeader != null)
                {
                    NewCellByColumn(i, template.PageHeader, y);
                    y += template.PageHeader.RowCount;
                }
                if (template.MainBody != null)
                    foreach (clsPrintTable table in template.MainBody)
                    {
                        NewCellByColumn(i, table.TableHeader, y);
                        y += table.TableHeader.RowCount;
                        NewCellByColumn(i, table.TableBody, y);
                        y += table.TableBody.RowCount;
                    }
                if (template.PageFooter != null)
                    NewCellByColumn(i, template.PageFooter, y);
            }
        }
        private void NewCellByColumn(int i, clsPrintRange range,int y)
        {
            foreach (clsPrintCell c in range.Cells)
            {
                if (c.StartColumn == i)
                {
                    Cells.Cell cell = new SourceGrid.Cells.Cell();
                    cell.Tag = c;
                    gridTemplate.SetCell(y + c.StartRow, c.StartColumn + 4, cell);
                }
            }
        }
        private void NewCellByRow(int i, clsPrintRange range, int y)
        {
            foreach (clsPrintCell c in range.Cells)
            {
                if (c.StartRow == i)
                {
                    if (c.IsImage)
                    {
                        Cells.Cell cell = new SourceGrid.Cells.Image();
                        cell.Value = this.unSerializationImg(c.Image);
                        cell.View = cellView;
                        cell.Tag = c;
                        cell.RowSpan = c.EndRow - c.StartRow + 1;
                        cell.ColumnSpan = c.EndColumn - c.StartColumn + 1;
                        gridTemplate.SetCell(y, c.StartColumn + 4, cell);
                    }
                    else
                    {
                        Cells.Cell cell = new SourceGrid.Cells.Cell();
                        cell.View = cellView;
                        cell.Tag = c;
                        cell.RowSpan = c.EndRow - c.StartRow + 1;
                        cell.ColumnSpan = c.EndColumn - c.StartColumn + 1;
                        gridTemplate.SetCell(y, c.StartColumn + 4, cell);
                    }
                }
            }
        }

        private void toolScriptDeleteColumn_Click(object sender, EventArgs e)
        {
            if (x > 3 && gridTemplate!=null)
            {
                gridTemplate.Columns.Remove(x);
                //删除配置
                template.Columns.Remove(x - 4);

                for (int j = x; j < gridTemplate.Columns.Count; j++)
                {
                    gridTemplate[0, j].Value = j - 3;
                }
            }
        }
        
        private void toolStripDeleteRow_Click(object sender, EventArgs e)
        {
            if (y > 0 && gridTemplate!=null)
            {
                SortedList<int, Cells.ICell> list = new SortedList<int, SourceGrid.Cells.ICell>();
                for (int i = 0; i < 3; i++)
                {
                    list.Add(i,gridTemplate[y, i]);
                }

                //取第三列
                Cells.ICell cell = gridTemplate[y, 2];
                object o = cell.Tag;
                if (o.GetType() != typeof(clsPrintRange))
                    return;
                clsPrintRange range = (clsPrintRange)o;
                range.Rows.Remove(y - cell.Row.Index);
                if (range.Cells.Count == 0)
                {
                }
                //将后续的行改名
                for (int j = y; j < gridTemplate.Rows.Count; j++)
                {
                    if (cell == gridTemplate[j, 2])
                    {
                        gridTemplate[j, 3].Value = j - cell.Row.Index;
                    }
                    else
                        break;
                }

                gridTemplate.Rows.Remove(y);
                //当遇到最后一行时，需要将Y设到上一行
                if (gridTemplate.RowsCount == y)
                    y--;
                foreach (KeyValuePair<int, Cells.ICell> v in list)
                {
                    Cells.ICell rowH = v.Value;
                    int i=v.Key;
                    if (rowH.Row != null)
                    {
                        if (rowH.Row.Index > 0)
                        {
                            if (rowH.RowSpan > 1)
                            {
                                rowH.RowSpan--;
                                //Cells.RowHeader newRow = new SourceGrid.Cells.RowHeader();
                                //newRow.Value = rowH.Value;
                                //newRow.RowSpan = rowH.RowSpan - 1;
                                //newRow.ColumnSpan = rowH.ColumnSpan;
                                //newRow.Tag = rowH.Tag;
                                //gridTemplate.SetCell(y, i, newRow);
                            }
                        }
                        else
                        {
                            Cells.RowHeader newRow = new SourceGrid.Cells.RowHeader();
                            newRow.Value = rowH.Value;
                            newRow.RowSpan = rowH.RowSpan - 1;
                            newRow.ColumnSpan = rowH.ColumnSpan;
                            newRow.Tag = rowH.Tag;
                            gridTemplate.SetCell(y, i, newRow);
                        }
                    }
                }
            }
        }

        private void toolStripMerge_Click(object sender, EventArgs e)
        {
            SourceGrid.RangeRegion rr = gridTemplate.Selection.GetSelectionRegion();
            if (rr.Count != 1)
                return;
            SourceGrid.Range r = rr[0];
            if (r.Start.Equals(r.End))
                return;
            if (r.Start.Row < 1 || r.Start.Column < 4)
                return;

            clsPrintRange lastRange = null;
            for (int i = r.Start.Row; i <= r.End.Row; i++)
            {
                clsPrintRange range = (clsPrintRange)gridTemplate[i, 2].Tag;
                if (lastRange != null && lastRange != range)
                    return;
                else
                    lastRange = range;
            }
            //进行合并，先找出首单元格
            Cells.ICell gridCell = gridTemplate[r.Start.Row, r.Start.Column];
            clsPrintCell cell = (clsPrintCell)gridCell.Tag;

            for (int i = r.Start.Column; i <= r.End.Column;i++ )
                for (int j = r.Start.Row; j <= r.End.Row; j++)
                {
                    //考虑到如果第一个格子是合并的情况
                    if ((i >= gridCell.Column.Index && i < gridCell.Column.Index + gridCell.ColumnSpan) == false || (j >= gridCell.Row.Index && j < gridCell.Row.Index+gridCell.RowSpan) == false)
                    {
                        if (gridTemplate[j, i] != null)
                        {
                            lastRange.Cells.Remove((clsPrintCell)gridTemplate[j, i].Tag);
                            gridTemplate[j, i] = null;
                        }
                    }
                    //if (i != r.Start.Column || j != r.Start.Row)
                    //{

                    //    if (gridTemplate[j, i] != null)
                    //    {
                    //        lastRange.Cells.Remove((clsPrintCell)gridTemplate[j, i].Tag);
                    //        gridTemplate[j, i] = null;
                    //    }
                    //}
                }
            gridCell.RowSpan = r.End.Row - r.Start.Row + 1;
            gridCell.ColumnSpan = r.End.Column - r.Start.Column + 1;
            cell.EndColumn = cell.StartColumn + r.End.Column - r.Start.Column;
            cell.EndRow = cell.StartRow + r.End.Row - r.Start.Row;
        }

        private void toolStripSplit_Click(object sender, EventArgs e)
        {
            SourceGrid.RangeRegion rr = gridTemplate.Selection.GetSelectionRegion();
            if (rr.Count != 1)
                return;
            SourceGrid.Range r = rr[0];
            if (r.Start.Equals(r.End))
                return;
            if (r.Start.Row < 1 || r.Start.Column < 4)
                return;

            clsPrintRange lastRange = null;
            Cells.ICell lastCell=null;
            for (int i = r.Start.Row; i <= r.End.Row; i++)
            {
                lastCell = gridTemplate[i, 2];
                clsPrintRange range = (clsPrintRange)lastCell.Tag;
                if (lastRange != null && lastRange != range)
                    return;
                else
                    lastRange = range;
            }

            for (int i = r.Start.Column; i <= r.End.Column; i++)
                for (int j = r.Start.Row; j <= r.End.Row; j++)
                {
                    Cells.ICell gridCell = gridTemplate[j, i];
                    if (gridCell!=null && i == gridCell.Column.Index && j == gridCell.Row.Index)
                    {
                        gridCell.RowSpan = 1;
                        gridCell.ColumnSpan = 1;
                        clsPrintCell cell = (clsPrintCell)gridCell.Tag;
                        cell.EndRow = cell.StartRow;
                        cell.EndColumn = cell.StartColumn;
                    }
                    else
                    {
                        Cells.Cell c = new SourceGrid.Cells.Cell();
                        gridTemplate.SetCell(j, i, c);
                        clsPrintCell pc = lastRange.Cells.NewChild();
                        pc.StartColumn = i - lastCell.ColumnSpan-1;
                        pc.EndColumn = i - lastCell.ColumnSpan-1;
                        pc.StartRow = j - lastCell.Row.Index;
                        pc.EndRow = j - lastCell.Row.Index;
                        c.Tag = pc;
                    }
                }
        }

        private void toolStripOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "打印模板定义文件(*.xml)|*.xml";   
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                template = new clsPrintTemplate(ofd.FileName);
                //先增加列头
                gridTemplate.Rows.Clear();
                gridTemplate.Columns.Clear();

                gridTemplate.RowsCount = 1;
                gridTemplate.ColumnsCount = 4 + template.Columns.Count;
                //左上角
                Cells.RowHeader rh = new SourceGrid.Cells.RowHeader();
                rh.Tag = template;
                rh.ColumnSpan = 4;
                rh.Value = "左上角";
                gridTemplate.SetCell(0, 0, rh);

                //先建列
                for (int i = 0; i < template.Columns.Count; i++)
                {
                    clsPrintColumn col = template.Columns[i];

                    PrintColumnHeader ch = new PrintColumnHeader();
                    ch.Tag = col;
                    ch.Value = col.Index + 1;
                    gridTemplate.SetCell(0, 4 + col.Index, ch);
                    gridTemplate.Columns[4 + col.Index].Width = col.Width;
                }
                int y = 1;
                //页眉
                if (template.PageHeader != null)
                {
                    clsPrintRange header = template.PageHeader;
                    if (header.Rows.Count > 0)
                    {
                        Cells.RowHeader phh = new SourceGrid.Cells.RowHeader();
                        phh.Tag = header;
                        phh.ColumnSpan = 3;
                        phh.RowSpan = header.Rows.Count;

                        phh.Value = "页眉";
                        gridTemplate.RowsCount = gridTemplate.RowsCount + header.Rows.Count;
                        gridTemplate.SetCell(y, 0, phh);

                        foreach (clsPrintRow row in header.Rows)
                        {
                            Cells.RowHeader phr = new SourceGrid.Cells.RowHeader();
                            phr.Tag = row;
                            phr.Value = row.Index + 1;
                            gridTemplate.SetCell(row.Index + y, 3, phr);
                            gridTemplate.Rows[y+row.Index].Height = row.Height;

                            NewCellByRow(row.Index, header, y + row.Index);
                        }
                        y += header.Rows.Count;
                    }
                }
                //主页
                if (template.MainBody != null)
                {
                    //先增加表，再增加主页头
                    int i = 1;
                    int h = 0;
                    int lastY = y;
                    foreach (clsPrintTable table in template.MainBody)
                    {
                        gridTemplate.RowsCount += table.TableHeader.RowCount;

                        Cells.RowHeader rowTableHeader = new SourceGrid.Cells.RowHeader();
                        rowTableHeader.Tag = table.TableHeader;
                        rowTableHeader.RowSpan = table.TableHeader.RowCount;
                        rowTableHeader.Value = "表头";
                        gridTemplate.SetCell(y, 2, rowTableHeader);

                        foreach (clsPrintRow r in table.TableHeader.Rows)
                        {
                            Cells.RowHeader rowTableRow = new SourceGrid.Cells.RowHeader();
                            rowTableRow.Tag = r;
                            rowTableRow.Value = r.Index + 1;
                            gridTemplate.SetCell(y + r.Index, 3, rowTableRow);
                            gridTemplate.Rows[y+r.Index].Height = r.Height;
                            NewCellByRow(r.Index, table.TableHeader, y + r.Index);
                        }

                        gridTemplate.RowsCount += table.TableBody.RowCount;

                        Cells.RowHeader rowTable = new SourceGrid.Cells.RowHeader();
                        rowTable.Tag = table;
                        rowTable.RowSpan = table.TableBody.Rows.Count + table.TableHeader.Rows.Count;
                        rowTable.Value = "表" + i;
                        gridTemplate.SetCell(y, 1, rowTable);
                        i++;

                        y += table.TableHeader.RowCount;
                        Cells.RowHeader rowTableBody = new SourceGrid.Cells.RowHeader();
                        rowTableBody.Tag = table.TableBody;
                        rowTableBody.RowSpan = table.TableBody.RowCount;
                        rowTableBody.Value = "表体";
                        gridTemplate.SetCell(y, 2, rowTableBody);

                        foreach (clsPrintRow r in table.TableBody.Rows)
                        {
                            Cells.RowHeader rowTableBodyRow = new SourceGrid.Cells.RowHeader();
                            rowTableBodyRow.Tag = r;
                            rowTableBodyRow.Value = r.Index + 1;
                            gridTemplate.SetCell(y + r.Index, 3, rowTableBodyRow);
                            gridTemplate.Rows[y+r.Index].Height = r.Height;
                            NewCellByRow(r.Index, table.TableBody, y + r.Index);
                        }
                        h += table.TableBody.Rows.Count + table.TableHeader.Rows.Count;
                        y += table.TableBody.RowCount;
                    }
                    Cells.RowHeader rowMainBody = new SourceGrid.Cells.RowHeader();
                    rowMainBody.Tag = template.NewMainBody();
                    rowMainBody.Value = "主页";
                    gridTemplate.SetCell(lastY, 0, rowMainBody);
                    rowMainBody.RowSpan = h;
                }

                //页脚
                if (template.PageFooter != null)
                {
                    clsPrintRange Footer = template.PageFooter;
                    if (Footer.Rows.Count > 0)
                    {
                        Cells.RowHeader phh = new SourceGrid.Cells.RowHeader();
                        phh.Tag = Footer;
                        phh.ColumnSpan = 3;
                        phh.RowSpan = Footer.Rows.Count;

                        phh.Value = "页脚";
                        gridTemplate.RowsCount = gridTemplate.RowsCount + Footer.Rows.Count;
                        gridTemplate.SetCell(y, 0, phh);

                        foreach (clsPrintRow row in Footer.Rows)
                        {
                            Cells.RowHeader phr = new SourceGrid.Cells.RowHeader();
                            phr.Tag = row;
                            phr.Value = row.Index + 1;
                            gridTemplate.SetCell(row.Index + y, 3, phr);
                            gridTemplate.Rows[y + row.Index].Height = row.Height;

                            NewCellByRow(row.Index, Footer, y + row.Index);
                        }
                        y += Footer.Rows.Count;
                    }
                }
            }
        }

        private void frmTemplate_Load(object sender, EventArgs e)
        {
            gridTemplate.Controller.AddController(new ClickController(propertyGrid, this));
            gridTemplate.Columns.ColumnWidthChanged += new SourceGrid.ColumnInfoEventHandler(Columns_ColumnWidthChanged);
            gridTemplate.Rows.RowHeightChanged += new SourceGrid.RowInfoEventHandler(Rows_RowHeightChanged);
            cellView = new CellView();
            SourceGrid.Grid.MaxSpan = 100;
        }

        void Rows_RowHeightChanged(object sender, SourceGrid.RowInfoEventArgs e)
        {
            try{
                if (gridTemplate[e.Row.Index, 3] != null && gridTemplate[e.Row.Index, 3].Tag!=null)
                    ((clsPrintRow)gridTemplate[e.Row.Index, 3].Tag).Height = e.Row.Height;
            }
            catch { }
        }

        void Columns_ColumnWidthChanged(object sender, SourceGrid.ColumnInfoEventArgs e)
        {
            try
            {
                if (gridTemplate[0, e.Column.Index] != null && gridTemplate[0, e.Column.Index].Tag!=null)
                    ((clsPrintColumn)gridTemplate[0, e.Column.Index].Tag).Width = e.Column.Width;
            }
            catch { }
        }
        private class PrintColumnHeader : Cells.Header
        {

        }
        public class MyController : SourceGrid.Cells.Controllers.ControllerBase
        {
            private SourceGrid.Cells.Views.Cell MouseEnterView = new SourceGrid.Cells.Views.Cell();
            private SourceGrid.Cells.Views.Cell MouseLeaveView = new SourceGrid.Cells.Views.Cell();
            public MyController()
            {
                MouseEnterView.BackColor = Color.Green;
            }

            public override void OnMouseEnter(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnMouseEnter(sender, e);

                sender.Cell.View = MouseEnterView;

                sender.Grid.InvalidateCell(sender.Position);
            }
            public override void OnMouseLeave(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnMouseLeave(sender, e);

                sender.Cell.View = MouseLeaveView;

                sender.Grid.InvalidateCell(sender.Position);
            }
        }

        class ClickController : SourceGrid.Cells.Controllers.ControllerBase
        {
            private PropertyGrid _PropertyGrid;
            private frmTemplate _Template;
            public ClickController(PropertyGrid property, frmTemplate template)
            {
                _PropertyGrid = property;
                _Template = template;
            }
            public override void OnFocusEntered(SourceGrid.CellContext sender, EventArgs e)
            {
                Cells.ICell cell = (Cells.ICell)sender.Cell;
                _PropertyGrid.SelectedObject = cell.Tag;
                _Template.x = sender.CellRange.Start.Column;
                _Template.y = sender.CellRange.Start.Row;
                _PropertyGrid.Refresh();

                base.OnFocusEntered(sender, e);
            }

            public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
            {
                Cells.ICell cell = (Cells.ICell)sender.Cell;
                _PropertyGrid.SelectedObject = cell.Tag;
                _Template.x = sender.CellRange.Start.Column;
                _Template.y = sender.CellRange.Start.Row;
                _PropertyGrid.Refresh();

                base.OnClick(sender, e);
            }
        }
        private class CellView : Cells.Views.Cell
        {
            protected override void OnDrawContent(DevAge.Drawing.GraphicsCache graphics, RectangleF area)
            {
                base.OnDrawContent(graphics, area);
            }
            protected override void PrepareView(SourceGrid.CellContext context)
            {
                base.PrepareView(context);
            }
            private SortedList<string, DevAge.Drawing.RectangleBorder> listBorder = new SortedList<string, DevAge.Drawing.RectangleBorder>();
            protected override void PrepareVisualElementText(SourceGrid.CellContext context)
            {
                base.PrepareVisualElementText(context);

                Cells.ICell cell = (Cells.ICell)context.Cell;
                clsPrintCell c = cell.Tag as clsPrintCell;
                if (c != null)
                {
                    string s = c.Text;
                    if (s == null || s == "")
                    {
                        if (c.Field == null || c.Field == "")
                        {
                            s = c.Format;
                        }
                        else
                        {
                            string[] fields = c.Field.Split(',');
                            object[] p = new object[fields.Length];
                            for (int i = 0; i < fields.Length; i++)
                            {
                                p[i] = fields[i];
                            }
                            string f = c.Format;
                            if (f == null || f == "")
                                for (int i = 0; i < fields.Length; i++)
                                    f += "{" + i + "}";
                            s = string.Format(f, p);
                        }
                    }

                    ElementText.Value = s;
                    ElementText.Font = c.Font;
                    ((DevAge.Drawing.VisualElements.TextGDI)ElementText).Alignment = (DevAge.Drawing.ContentAlignment)c.Alignment;
                    string bs = c.BoderStyle.ToString();

                    DevAge.Drawing.RectangleBorder cellBorder;
                    if (listBorder.ContainsKey(bs))
                        cellBorder = listBorder[bs];
                    else
                    {
                        if (bs == "0,0,0,0")
                        {
                            DevAge.Drawing.BorderLine borderTop = new DevAge.Drawing.BorderLine(Color.White, (float)0, System.Drawing.Drawing2D.DashStyle.Solid);
                            DevAge.Drawing.BorderLine borderBottom = new DevAge.Drawing.BorderLine(Color.LightGray, (float)1, System.Drawing.Drawing2D.DashStyle.Solid);
                            DevAge.Drawing.BorderLine borderLeft = new DevAge.Drawing.BorderLine(Color.White, (float)0, System.Drawing.Drawing2D.DashStyle.Solid);
                            DevAge.Drawing.BorderLine borderRight = new DevAge.Drawing.BorderLine(Color.LightGray, (float)1, System.Drawing.Drawing2D.DashStyle.Solid);
                            cellBorder = new DevAge.Drawing.RectangleBorder(borderTop, borderBottom, borderLeft, borderRight);
                        }
                        else
                        {
                            DevAge.Drawing.BorderLine borderTop = new DevAge.Drawing.BorderLine(Color.Black, (float)c.BoderStyle.Top);
                            DevAge.Drawing.BorderLine borderBottom = new DevAge.Drawing.BorderLine(Color.Black, (float)c.BoderStyle.Bottom);
                            DevAge.Drawing.BorderLine borderLeft = new DevAge.Drawing.BorderLine(Color.Black, (float)c.BoderStyle.Left);
                            DevAge.Drawing.BorderLine borderRight = new DevAge.Drawing.BorderLine(Color.Black, (float)c.BoderStyle.Right);
                            cellBorder = new DevAge.Drawing.RectangleBorder(borderTop, borderBottom, borderLeft, borderRight);
                        }
                        listBorder.Add(bs, cellBorder);
                    }
                    this.Border = cellBorder;
                }
            }
        }

        private void toolStripInsertImage_Click(object sender, EventArgs e)
        {
            SourceGrid.RangeRegion rr = gridTemplate.Selection.GetSelectionRegion();
            if (rr.Count != 1)
                return;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "位图文件|*.bmp|JPEG|*.jpeg|GIF|*.GIF";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(ofd.FileName);
                SourceGrid.Range r = rr[0];
                Cells.ICell lastCell = gridTemplate[r.Start.Row, r.Start.Column];
                Cells.ICell imageCell = new SourceGrid.Cells.Image();//gridTemplate[r.Start.Row,r.Start.Column];
                imageCell.Value = img;
                imageCell.RowSpan = lastCell.RowSpan;
                imageCell.ColumnSpan = lastCell.ColumnSpan;
                //imageCell.Editor = im;
                gridTemplate.SetCell(r.Start.Row, r.Start.Column, imageCell);
                clsPrintCell range = (clsPrintCell)lastCell.Tag;                
                range.IsImage=true;
                range.Image = serializationImg(img);
                imageCell.Tag = range;
            }
            //Image img = Clipboard.GetImage();
            ////if (img == null)
            ////{
            ////    return;
            ////}
            //SourceGrid.Range r = rr[0];
            //Cells.ICell lastCell = gridTemplate[r.Start.Row, r.Start.Column];
            //Cells.ICell imageCell = new SourceGrid.Cells.Image();//gridTemplate[r.Start.Row,r.Start.Column];
            ////imageCell.Value = img;
            //imageCell.RowSpan = lastCell.RowSpan;
            //imageCell.ColumnSpan = lastCell.ColumnSpan;
            //SourceGrid.Cells.Editors.ImagePicker im = new SourceGrid.Cells.Editors.ImagePicker();
            //imageCell.Editor = im;
            //gridTemplate.SetCell(r.Start.Row, r.Start.Column, imageCell);

            //clsPrintCell range = (clsPrintCell)lastCell.Tag;
            //imageCell.Tag = range;

            //imageCell.Value  = img;
            //System.Runtime.Serialization.IFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //Stream stream = new FileStream("C:\\xi.dat", FileMode.Create);
            //formater.Serialize(stream, img);
            //stream.Close();
            //SourceGrid.Cells.Editors.ImagePicker



        }

        private string serializationImg(Image img)
        {

            string ser = "";
            System.Runtime.Serialization.IFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            formater.Serialize(mem, img);
            string textstring = System.Convert.ToBase64String(mem.ToArray());
            return textstring;

        }
        // 反序列化并显示
        private Image unSerializationImg(string strImg)
        {
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(strImg));
            System.Runtime.Serialization.IFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Image img = (Image)formater.Deserialize(stream);
            stream.Close();
            return  img;
        }


    }
}