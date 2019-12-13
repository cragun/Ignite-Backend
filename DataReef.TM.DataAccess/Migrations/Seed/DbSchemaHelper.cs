using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataReef.TM.DataAccess.Migrations.Seed
{
    internal static class DbSchemaHelper
    {
        /// <summary>
        /// Get CLR type based on SQL DB type
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static Type GetColumnType(string dbType)
        {
            switch (dbType)
            {
                //todo: to be added
                case "uniqueidentifier":
                    return typeof(Guid);
                case "nvarchar":
                    return typeof(String);
                case "bit":
                    return typeof(Boolean);
                case "datetime":
                    return typeof(DateTime);
                case "real":
                    return typeof(Double);
                case "int":
                    return typeof(Int16);
                case "bigint":
                    return typeof(Int32);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Builds a table based on existing DB column schema.
        /// No PK or FK or unique contraints.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="databaseColumnsSchema"></param>
        /// <returns></returns>
        public static DataTable BuildTableFromColumnSchema(string tableName, DataTable databaseColumnsSchema)
        {
            var table = new DataTable(tableName);

            foreach (DataRow c in databaseColumnsSchema.Rows)
            {
                if (c["TABLE_NAME"].ToString().Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
                {
                    var columnName = c["COLUMN_NAME"].ToString();
                    var columnType = DbSchemaHelper.GetColumnType(c["DATA_TYPE"].ToString());

                    DataColumn column = new DataColumn(columnName);
                    column.AllowDBNull = !c["IS_NULLABLE"].ToString().Equals("NO", StringComparison.InvariantCultureIgnoreCase);

                    if (columnType != null)
                    {
                        column.DataType = columnType;
                    }

                    table.Columns.Add(column);
                }
            }

            return table;
        }

        /// <summary>
        /// Get a list of tables in a DB.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<string> GetListOfTables(string connectionString)
        {
            DataTable tablesSchema;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Connect to the database then retrieve the schema information.
                connection.Open();
                tablesSchema = connection.GetSchema("Tables");
            }

            var tableNames = new List<string>();

            foreach (DataRow row in tablesSchema.Rows)
            {
                var tableName = row["TABLE_NAME"].ToString();
                if (!tableName.StartsWith("_"))
                    tableNames.Add(tableName);
            }

            return tableNames;
        }

        /// <summary>
        /// Retreive a table that contains the DB columns information
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>DataTable that contains all information about data columns</returns>
        public static DataTable GetColumnsSchema(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Connect to the database then retrieve the schema information.
                connection.Open();
                var columnsSchema = connection.GetSchema("Columns");

                return columnsSchema;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<ForeignKey> GetForeignKeysList(string connectionString)
        {

            var query_FK = @"SELECT
                            FK_Table = FK.TABLE_NAME,
                            FK_Column = CU.COLUMN_NAME,
                            PK_Table = PK.TABLE_NAME,
                            PK_Column = PT.COLUMN_NAME,
                            Constraint_Name = C.CONSTRAINT_NAME
                            FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
                            INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
                            INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
                            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
                            INNER JOIN (
                            SELECT i1.TABLE_NAME, i2.COLUMN_NAME
                            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
                            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
                            WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
                            ) PT ON PT.TABLE_NAME = PK.TABLE_NAME

                            ORDER BY FK_Table ASC";

            var foreignKeys = new List<ForeignKey>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Connect to the database then retrieve the schema information.
                connection.Open();

                SqlCommand command = new SqlCommand(query_FK, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var fk = new ForeignKey
                        {
                            TableName = reader["FK_Table"].ToString(),
                            ColumnName = reader["FK_Column"].ToString(),
                            PrimaryKeyTableName = reader["PK_Table"].ToString(),
                            PrimaryKeyColumnName = reader["PK_Column"].ToString(),

                        };
                        foreignKeys.Add(fk);
                    }
                }
            }
            return foreignKeys;

        }
    }

    internal class ForeignKey
    {
        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public string PrimaryKeyTableName { get; set; }

        public string PrimaryKeyColumnName { get; set; }
    }
}
