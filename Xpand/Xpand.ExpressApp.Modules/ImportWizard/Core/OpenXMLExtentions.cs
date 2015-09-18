﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;

namespace Xpand.ExpressApp.ImportWizard.Core {

    public class SheetDimension {
        public int StartColumnIndex { get; set; }
        public int EndColumnIndex { get; set; }
        public int StartRowIndex { get; set; }
        public int EndRowIndex { get; set; }
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        public SheetDimension(string sheetDimension) {
            var dimms = sheetDimension.Split(':');
            if (dimms.Count() != 2)
                throw new InvalidDataException(@"Wrong sheet dimmension format");


            StartColumnIndex = ExcelExtentions.SplitAddress(dimms[0])[0].ColumnAddressToIndex();
            EndColumnIndex = ExcelExtentions.SplitAddress(dimms[1])[0].ColumnAddressToIndex();
            StartRowIndex = Convert.ToInt32(ExcelExtentions.SplitAddress(dimms[0])[1]) - 1;
            EndRowIndex = Convert.ToInt32(ExcelExtentions.SplitAddress(dimms[1])[1]) - 1;

            RowCount = EndRowIndex + 1;
            ColumnCount = EndColumnIndex + 1;
        }
    }

    public class Sheet {
        public WorksheetPart WorkSheetPart { get; set; }
        public WorkbookPart WorkbookPart { get; set; }
        public DocumentFormat.OpenXml.Spreadsheet.Sheet OXmlSheet { get; set; }
        public int? ColumnHeaderRow { get; set; }
        public StringValue Name { get { return OXmlSheet.Name; } }
        public string Dimension { get; set; }
        public int? PreviewRowCount { get; set; }
    }

    public class Column {
        public string Name { get; set; }
        public DocumentFormat.OpenXml.Spreadsheet.Column OXmlColumn { get; set; }
        public int ColumnIndex { get; set; }
    }

    public class Row {
        public DocumentFormat.OpenXml.Spreadsheet.Row OXmlRow { get; set; }
        public Sheet Sheet { get; set; }
        public Cell this[string column] {
            get {
                if (column == null)
                    throw new Exception(@"Invalid column name: " + null);

                var colIx = this.Columns().First(p => p.Name == column).ColumnIndex;
                var ret = this[colIx];
                return
                    ret;



            }
        }
        public Cell this[int columnIndex] {
            get {
                return this.Cells().ElementAt(columnIndex);
            }
        }


    }

    public class Cell {

        public DocumentFormat.OpenXml.Spreadsheet.Cell OXmlCell { get; set; }
        public Row Row { get; set; }
        public Column Column { get; set; }
        public string Value { get; set; }

    }

    public static class ExcelExtentions {
        #region Methods

        public static string IndexToColumnAddress(this int index) {
            if (index < 26) {
                var c = (char)('A' + index);
                var s = new string(c, 1);
                return s;
            }
            if (index < 702) {
                var i = index - 26;
                var i1 = i / 26;
                var i2 = i % 26;
                var s = new string((char)('A' + i1), 1) +
                    new string((char)('A' + i2), 1);
                return s;
            }
            if (index < 18278) {
                var i = index - 702;
                var i1 = i / 676;
                i = i - i1 * 676;
                var i2 = i / 26;
                var i3 = i % 26;
                var s = new string((char)('A' + i1), 1) +
                    new string((char)('A' + i2), 1) +
                    new string((char)('A' + i3), 1);
                return s;
            }
            throw new Exception(@"Invalid column address");
        }

        public static int ColumnAddressToIndex(this string columnAddress) {
            if (columnAddress.Length == 1) {
                var c = columnAddress[0];
                var i = c - 'A';
                return i;
            }
            if (columnAddress.Length == 2) {
                var c1 = columnAddress[0];
                var c2 = columnAddress[1];
                var i1 = c1 - 'A';
                var i2 = c2 - 'A';
                return (i1 + 1) * 26 + i2;
            }
            if (columnAddress.Length == 3) {
                var c1 = columnAddress[0];
                var c2 = columnAddress[1];
                var c3 = columnAddress[2];
                var i1 = c1 - 'A';
                var i2 = c2 - 'A';
                var i3 = c3 - 'A';
                return (i1 + 1) * 676 + (i2 + 1) * 26 + i3;
            }
            throw new Exception(@"Invalid column address");
        }

        public static string[] SplitAddress(string address) {
            int i;
            for (i = 0; i < address.Length; i++)
                if (address[i] >= '0' && address[i] <= '9')
                    break;
            if (i == address.Length)
                throw new Exception(@"Invalid cell address format");
            return new[] {
                address.Substring(0, i),
                address.Substring(i)
            };
        }

        #endregion

        #region OpenXml Extentions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spreadsheet"></param>
        /// <param name="test">Dummy parameter</param>
        /// <returns></returns>
        public static IEnumerable<DocumentFormat.OpenXml.Spreadsheet.Sheet> Sheets(this SpreadsheetDocument spreadsheet, object test) {
            var firstSheet = spreadsheet.WorkbookPart.Workbook.Elements().FirstOrDefault(p => p.LocalName == @"sheets");
            if (firstSheet != null) {
                return firstSheet.Elements().Where(e => e.LocalName == @"sheet")
                    .Select(s => (DocumentFormat.OpenXml.Spreadsheet.Sheet)s);
            }

            return Enumerable.Empty<DocumentFormat.OpenXml.Spreadsheet.Sheet>();
        }

        public static IEnumerable<DocumentFormat.OpenXml.Spreadsheet.Row> Rows(this WorksheetPart worksheet) {

            var firstSheetData = worksheet.RootElement.Elements().FirstOrDefault(p => p.LocalName == @"sheetData");
            if (firstSheetData != null) {
                return firstSheetData.Elements().Where(e => e.LocalName == @"row")
                    .Select(t => (DocumentFormat.OpenXml.Spreadsheet.Row)t);
            }

            return Enumerable.Empty<DocumentFormat.OpenXml.Spreadsheet.Row>();
        }

        public static WorksheetPart WorkSheet(this SpreadsheetDocument spreadsheet, DocumentFormat.OpenXml.Spreadsheet.Sheet oXmlSheet) {
            return spreadsheet.WorkbookPart.GetPartById(oXmlSheet.Id) as WorksheetPart;
        }

        public static IEnumerable<DocumentFormat.OpenXml.Spreadsheet.Cell> Cells(this DocumentFormat.OpenXml.Spreadsheet.Row row, SharedStringTable sharedStringTable) {
            foreach (var celll in row.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>()) {
                var c = (DocumentFormat.OpenXml.Spreadsheet.Cell)celll.Clone();
                if (celll.CellValue != null) {
                    var text = (celll.DataType != null
                               && celll.DataType.HasValue
                               && celll.CellValue.InnerText != null
                               && celll.DataType == CellValues.SharedString)
                                  ? sharedStringTable.ChildElements[int.Parse(celll.CellValue.InnerText)].InnerText
                                  : celll.CellValue.InnerText;
                    c.CellValue.Text = text;
                }

                yield return c;
            }
        }


        #endregion

        #region MyExtentions

        public static IEnumerable<Sheet> Sheets(this SpreadsheetDocument spreadsheet) {
            return spreadsheet.Sheets(null).Select(p => new Sheet {
                OXmlSheet = p,
                WorkbookPart = spreadsheet.WorkbookPart,
                WorkSheetPart = spreadsheet.WorkSheet(p),
                Dimension = spreadsheet.WorkSheet(p)
                    .RootElement.Descendants<DocumentFormat.OpenXml.Spreadsheet.SheetDimension>()
                    .First().Reference
            });
        }


        public static IEnumerable<Row> Rows(this Sheet sheet) {
            return sheet.WorkSheetPart.Rows().Select(p => new Row {
                OXmlRow = p,
                Sheet = sheet
            });
        }

        public static IEnumerable<Column> Columns(this Sheet sheet) {
            if (sheet == null)
                yield return null;
            else {
                for (var i = 0; i < sheet.Dimentions().ColumnCount; i++) {
                    var col = new Column { ColumnIndex = i };
                    int? columnHeaderRow = sheet.ColumnHeaderRow;
                    if (columnHeaderRow == null || columnHeaderRow == 0)
                        col.Name = i.IndexToColumnAddress();
                    else {
                        var cell = sheet.Rows().Select(t => t.OXmlRow)
                            .ElementAt((int)columnHeaderRow - 1).Cells(
                                GeSharedStringTable(sheet.WorkbookPart)).
                            ElementAtOrDefault(i);
                        col.Name = cell != null
                                       ? (cell.CellValue != null
                                              ? cell.CellValue.Text
                                              : i.IndexToColumnAddress())
                                       : i.IndexToColumnAddress();
                    }
                    yield return col;
                }
            }
        }

        public static IEnumerable<Column> Columns(this Row row) {
            return row.Sheet.Columns();
        }

        public static IEnumerable<Column> Columns(this Row row, int? columnHeaderRow) {
            return row.Sheet.Columns();
        }


        public static IEnumerable<Cell> Cells(this Row row){
            var columns = row.Sheet.Columns();
            var sharedStringTable = GeSharedStringTable(row.Sheet.WorkbookPart);
            if (sharedStringTable == null){
                return row.OXmlRow.Cells(null).Select(cell => GetCell(row, cell));
                
            }

            return columns.GroupJoin(row.OXmlRow.Cells(sharedStringTable)
                .Select(p => GetCell(row, p)), column => column.ColumnIndex,
                cl => cl != null ? cl.Column != null ? cl.Column.ColumnIndex : -1 : -1,
                (column, cls) => new{column, cls}).SelectMany(@t => @t.cls.DefaultIfEmpty(), (@t, cl) => new Cell{
                    Column = @t.column,
                    Row = row,
                    OXmlCell = cl != null ? cl.OXmlCell : null,
                    Value = cl != null? cl.OXmlCell != null? cl.OXmlCell.CellValue != null? cl.OXmlCell.CellValue.Text: "": "": ""
                });
        }

        private static SharedStringTable GeSharedStringTable(WorkbookPart workbookPart) {
            return workbookPart.SharedStringTablePart != null ? workbookPart.SharedStringTablePart.SharedStringTable : null;
        }

        private static Cell GetCell(Row row, DocumentFormat.OpenXml.Spreadsheet.Cell p){
            return new Cell {
                OXmlCell = p,
                Row = row,
                Column = row.Sheet.Columns().First(t 
                    => t.ColumnIndex ==ColumnAddressToIndex(SplitAddress(p.CellReference)[0])),
                Value = p.CellValue != null ? p.CellValue.Text : p.InnerText

            };
        }

        public static SheetDimension Dimentions(this Sheet sheet) {
            return new SheetDimension(sheet.Dimension);
        }

        public static DataTable DataPreviewTable(this Sheet sheet, int previewRowCount) {
            var dt = new DataTable("Preview Table");

            var dtcs = sheet.Columns().Select(p => new DataColumn(p.Name));
            //check for duplicates and throw error if found
            var bad = dtcs.GroupBy(d => d.ColumnName).Select(g => new { g.Key, count = g.Count() }).Where(g => g.count > 1);
            if (bad.Any()) {
                var s = bad.Aggregate(string.Empty, (current, enumerable) => current + (enumerable.Key + @"; "));
                throw new InvalidDataException(@"Duplicate column names found. " + s, new Exception(@"Bad columns : " + s.Trim()));
            }

            dt.Columns.AddRange(dtcs.ToArray());
            var rows = sheet.Rows().Take(previewRowCount).Skip(sheet.ColumnHeaderRow ?? 0);
            foreach (var rrow in rows) {
                dt.NewRow();

                var cells = rrow.Cells();

                var va = cells != null ?
                            cells.Select(p => p.Value).ToArray() :
                            new object[dt.Columns.Count];

                dt.Rows.Add(va);

            }
            return dt;
        }

        public static DataTable DataPreviewTable(this Sheet sheet) {
            return sheet.DataPreviewTable(sheet.PreviewRowCount ?? 10);
        }

        public static DataTable Transpose(this DataTable dt) {
            var dtNew = new DataTable();
            //adding columns    
            for (var i = 0; i <= dt.Rows.Count; i++)
                dtNew.Columns.Add(i.ToString(CultureInfo.InvariantCulture));
            //Changing Column Captions: 
            dtNew.Columns[0].ColumnName = " ";


            dtNew.Columns.Add("PropertyName", typeof(string));

            //Adding Row Data
            for (var k = 0; k < dt.Columns.Count; k++) {
                var r = dtNew.NewRow();
                r[0] = dt.Columns[k].ToString();
                for (var j = 1; j <= dt.Rows.Count; j++)
                    r[j] = dt.Rows[j - 1][k];
                r[dt.Rows.Count + 1] = null;
                dtNew.Rows.Add(r);
            }



            return dtNew;
        }

        #endregion
    }


}
