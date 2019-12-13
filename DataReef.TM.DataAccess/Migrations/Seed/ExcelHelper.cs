using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataReef.TM.DataAccess.Migrations.Seed
{
    internal static class ExcelHelper
    {
        /// <summary>
        /// Get column name, up until two letters (A, B , AB, AC, etc. )
        /// </summary>
        /// <param name="cellReference"></param>
        /// <returns></returns>
        public static string GetColumnName(string cellReference)
        {
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);
            return match.Value;
        }

        public static object DefaultReferenceValues(Type t)
        {
            if (t.Equals(typeof(string)))
                return string.Empty;

            return null;
        }

        public static object ConvertFromStringValue(Type type, bool allowDbNull, string value)
        {
            try
            {
                if (type == typeof(Guid))
                {
                    if (!string.IsNullOrEmpty(value))
                        return new Guid(value);

                    return DBNull.Value;
                }
                if (type == typeof(Boolean))
                {
                    return value.Equals("1");
                }
                if (type == typeof(DateTime))
                {
                    DateTime date;
                    if (!DateTime.TryParse(value, out date))
                    {
                        //Excel maintains dates as number of days from 1 January 1900
                        Double numberOfDays;
                        if (Double.TryParse(value, out numberOfDays))
                        {
                            return DateTime.FromOADate(numberOfDays);
                        }

                        if (allowDbNull)
                            return DBNull.Value;
                    }
                    else
                    {
                        return date;
                    }
                }

                return Convert.ChangeType(value, type);
            }
            catch (Exception ex)
            {
                if (!allowDbNull)
                    SeedLogger.Error("Incorrect format. Expected " + type.Name + " ( not nullable ) but received " + value);

                return allowDbNull && (string.IsNullOrWhiteSpace(value) || value.Equals("NULL", StringComparison.CurrentCultureIgnoreCase)) ? DBNull.Value : null;
            }
        }


        public static string GetCellStringValue(Cell cell)
        {
            if (cell == null)
                return null;

            if (cell.DataType != null)
                return GetCellValueByDataType(cell);

            return GetCellValueByStyleIndex(cell);
        }

        private static string GetCellValueByStyleIndex(Cell cell)
        {
            if (cell.StyleIndex != null)
            {
                if (cell.StyleIndex == 1)
                {
                    double days;
                    if (Double.TryParse(cell.InnerText, out days))
                    {
                        return DateTime.FromOADate(days).ToString();
                    }
                }
                return cell.InnerText;
            }
            else
                return cell.InnerText;
        }

        //todo: maybe it`s worthwhile to refactor this method.
        private static string GetCellValueByDataType(Cell cell)
        {
            string value = cell.InnerText;

            switch (cell.DataType.Value)
            {
                case CellValues.Date:
                    if (value.StartsWith("TODAY", StringComparison.InvariantCultureIgnoreCase))
                        return DateTime.Now.ToString();
                    else
                        return value;

                case CellValues.SharedString:
                    // For shared strings, look up the value in the shared strings table.
                    // Get worksheet from cell
                    OpenXmlElement parent = cell.Parent;
                    while (parent.Parent != null && parent.Parent != parent
                            && string.Compare(parent.LocalName, "worksheet", true) != 0)
                    {
                        parent = parent.Parent;
                    }
                    if (string.Compare(parent.LocalName, "worksheet", true) != 0)
                    {
                        throw new Exception("Unable to find parent worksheet.");
                    }

                    Worksheet ws = parent as Worksheet;
                    SpreadsheetDocument ssDoc = ws.WorksheetPart.OpenXmlPackage as SpreadsheetDocument;
                    SharedStringTablePart sstPart = ssDoc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

                    // lookup value in shared string table
                    if (sstPart != null && sstPart.SharedStringTable != null)
                    {
                        value = sstPart.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                    }
                    break;

                //this case within a case is copied from msdn. 
                case CellValues.Boolean:
                    switch (value)
                    {
                        case "0":
                            value = "FALSE";
                            break;
                        default:
                            value = "TRUE";
                            break;
                    }
                    break;
            }

            return value;
        }

        public static Dictionary<string, string> GetExcelHeader(WorksheetPart workSheet)
        {
            Dictionary<string, string> excelHeaderMapping = new Dictionary<string, string>();

            var headerCells = workSheet.Worksheet.Descendants<Row>().FirstOrDefault();
            if (headerCells == null)
                return null; //sheet is empty   

            foreach (var cell in headerCells.Descendants<Cell>())
            {
                var excelColumnValue = ExcelHelper.GetCellStringValue(cell);
                var excellColumnReference = ExcelHelper.GetColumnName(cell.CellReference);

                excelHeaderMapping[excellColumnReference] = excelColumnValue;
            }
            return excelHeaderMapping;
        }

    }
}
