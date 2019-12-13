using System.Reflection;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DataTable = System.Data.DataTable;

namespace DataReef.TM.DataAccess.Migrations.Seed
{
    public class DevelopmentSeed
    {
        private readonly string connectionString;
        private readonly string filePath;

        public DevelopmentSeed(string connectionString, string filePath)
        {
            this.connectionString = connectionString;
            this.filePath = filePath;
        }

        public void ImportSeedData()
        {
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            var tableNames = GetTablesForSeed();

            var dbColumnsSchema = DbSchemaHelper.GetColumnsSchema(connectionString);

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath);
            var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (var resource in resources)
            {
                SeedLogger.Info(resource);
            }

            using (SpreadsheetDocument myWorkbook = SpreadsheetDocument.Open(stream, false))
            {
                WorkbookPart workbookPart = myWorkbook.WorkbookPart;
                var sheets = workbookPart.Workbook.Descendants<Sheet>().ToList();

                foreach (var tableName in tableNames)
                {
                    var excelSheet = sheets.FirstOrDefault(s => s.Name == tableName);
                    if (excelSheet == null)
                        continue;

                    //It is required to build a identical table to be able to use the BulkCopy from ADO.
                    var table = DbSchemaHelper.BuildTableFromColumnSchema(tableName, dbColumnsSchema);

                    var wsPart = (WorksheetPart)(workbookPart.GetPartById(excelSheet.Id));

                    try
                    {
                        //SeedLogger.Info("Copy table " + table.TableName + " from excel");
                        CopyExcel(table, wsPart);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to copy table from excel, for: " + tableName + " | " + ex, ex);
                    }

                    try
                    {
                        //SeedLogger.Info("Write table " + table.TableName + " from excel");
                        Write(table);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to write table from excel, for: " + tableName + " | " + ex, ex);
                    }
                }
            }

        }

        public void BulkCopySeedData(DataTable table)
        {
            Write(table);
        }

        private void Write(DataTable datatable)
        {
            // connect to SQL
            using (var connection = new SqlConnection(connectionString))
            {
                var bulkCopy = MakeSqlBulkCopy(connection, datatable);
                bulkCopy.BatchSize = 10000;
                bulkCopy.BulkCopyTimeout = 60 * 2; // 2 minutes per batch size

                // set the destination table name
                connection.Open();

                using (var dataTableReader = new DataTableReader(datatable))
                    bulkCopy.WriteToServer(dataTableReader);

                connection.Close();
            }
        }

        private SqlBulkCopy MakeSqlBulkCopy(SqlConnection connection, DataTable table)
        {
            var bulkCopy =
                new SqlBulkCopy
                    (
                    connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null
                    )
                {
                    EnableStreaming = true
                };

            foreach (DataColumn column in table.Columns)
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);

            // TODO add missing multiple schema support. If a table is not in dbo it will fail the copy unless schema prefix is applied.
            var missingMultippleSchemaSupport = string.Empty;
            if (table.TableName.Contains("Schedule") && !table.TableName.Equals("ImmunizationSchedules"))
                missingMultippleSchemaSupport = "scheduling.";
            bulkCopy.DestinationTableName = missingMultippleSchemaSupport + table.TableName;

            return bulkCopy;
        }


        private void CopyExcel(DataTable table, WorksheetPart workSheet)
        {
            var headers = ExcelHelper.GetExcelHeader(workSheet);
            if (headers == null)
                return;
            
            //Iterate through every row                    
            var rows = workSheet.Worksheet.Descendants<Row>().Where(r => r.RowIndex > 1);
            foreach (var row in rows)
            {
                DataRow dataRow = table.NewRow();
                foreach (var cell in row.Descendants<Cell>())
                {
                    var excelReference = ExcelHelper.GetColumnName(cell.CellReference); //A, B, C, D ..
                    if (!headers.ContainsKey(excelReference))
                        continue;

                    var columnName = headers[excelReference];

                    var tableColumn = table.Columns[columnName];
                    if (tableColumn == null)
                        continue;

                    var value = ExcelHelper.GetCellStringValue(cell);

                    dataRow[columnName] = ExcelHelper.ConvertFromStringValue(tableColumn.DataType, tableColumn.AllowDBNull, value);
                }

                AutoFillNotNullable(dataRow);
                table.Rows.Add(dataRow);
            }

        }

        private static void AutoFillNotNullable(DataRow dr)
        {
            foreach (DataColumn column in dr.Table.Columns)
            {
                var columnDataType = column.DataType;
                if (!column.AllowDBNull)
                {
                    if (dr[column.ColumnName] == DBNull.Value)
                        dr[column.ColumnName] = columnDataType.IsValueType ? Activator.CreateInstance(columnDataType) : ExcelHelper.DefaultReferenceValues(columnDataType);
                }
            }
        }


        #region Determine table dependecies

        /// <summary>
        /// Retreive a list of table names from the Database, in dependency order.
        /// </summary>
        /// <returns></returns>
        private List<string> GetTablesForSeed()
        {
            var tablesList = DbSchemaHelper.GetListOfTables(connectionString);
            var foreignKeysList = DbSchemaHelper.GetForeignKeysList(connectionString);

            return OrderTablesByDependency(tablesList, foreignKeysList);
        }

        private static List<string> OrderTablesByDependency(List<string> tableNames, List<ForeignKey> foreignKeys)
        {
            var orderedTables = new List<string>();

            foreach (var tableName in tableNames)
            {
                if (!HasForeignKey(tableName, foreignKeys))
                {
                    if (!orderedTables.Contains(tableName))
                        orderedTables.Add(tableName);
                }
                else
                {
                    RecursiveForeignKeyLookup(tableName, orderedTables, foreignKeys);
                    if (!orderedTables.Contains(tableName))
                        orderedTables.Add(tableName);
                }

            }

            return orderedTables;
        }

        private static void RecursiveForeignKeyLookup(string tableName, List<string> orderedTables, List<ForeignKey> foreignKeys)
        {
            foreach (var fk in foreignKeys.Where(fk => fk.TableName.Equals(tableName, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (tableName.Equals(fk.PrimaryKeyTableName, StringComparison.InvariantCultureIgnoreCase))
                    continue;//ignore self referencing

                if (!HasForeignKey(fk.PrimaryKeyTableName, foreignKeys))
                {
                    if (!orderedTables.Contains(fk.PrimaryKeyTableName))
                        orderedTables.Add(fk.PrimaryKeyTableName);
                }
                else RecursiveForeignKeyLookup(fk.PrimaryKeyTableName, orderedTables, foreignKeys);
            }
        }

        private static bool HasForeignKey(string tableName, List<ForeignKey> foreignKeys)
        {
            return foreignKeys.Any(fk => fk.TableName.Equals(tableName, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }
}