using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadSQLServerDB
{
    public class OpenDB
    {
        public static List<DBTable> OpenDataBase(string connectionString)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            DataTable dbTables = sqlConnection.GetSchema("Tables");
            sqlConnection.Close();
            List<DBTable> tables = new List<DBTable>();
            foreach (DataRow row in dbTables.Rows)
            {
                DBTable table = new DBTable();

                if ((row["TABLE_SCHEMA"].ToString() == "dbo") && (!row["TABLE_NAME"].ToString().StartsWith("__")) && (!row["TABLE_NAME"].ToString().StartsWith("AspNet")))
                {
                    table.TableName = row["TABLE_NAME"].ToString();
                    tables.Add(table);
                }
            }
            foreach (DBTable table in tables)
            {
                table.IDColumn = new DBColumn();

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey') = 1 AND TABLE_NAME = '" + table.TableName + "' AND TABLE_SCHEMA = 'dbo'", sqlConnection);
                sqlDataAdapter.SelectCommand.Connection.Open();
                DataSet dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet);
                sqlDataAdapter.SelectCommand.Connection.Close();
                foreach (DataRow column in dataSet.Tables[0].Rows)
                {
                    table.PKName = column["CONSTRAINT_NAME"].ToString();
                    table.IDColumn.ColumnName = column["COLUMN_NAME"].ToString();
                }
                sqlDataAdapter = new SqlDataAdapter("SELECT * FROM " + sqlConnection.Database + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + table.TableName + "'", sqlConnection);
                dataSet = new DataSet();
                sqlDataAdapter.SelectCommand.Connection.Open();
                sqlDataAdapter.Fill(dataSet);
                sqlDataAdapter.SelectCommand.Connection.Close();
                foreach (DataRow column in dataSet.Tables[0].Rows)
                {
                    if (column["COLUMN_NAME"].ToString() == table.IDColumn.ColumnName)
                    {
                        table.IDColumn.SqlDataType = column["DATA_TYPE"].ToString().ToUpper();
                        table.IDColumn.FlagNull = column["IS_NULLABLE"].ToString().ToUpper() == "YES";
                        if (column["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                        {
                            table.IDColumn.MaxLenth = (int)column["CHARACTER_MAXIMUM_LENGTH"];
                        }
                        SqlDataAdapter sqlDataAdapterFK = new SqlDataAdapter("select O.Name AS FK_NAME,lc.name as LOCAL_COLUMN_NAME,t.name as TABLE_NAME,c.name as COLUMN_NAME, c.is_nullable as COLUMN_NULL from sys.foreign_key_columns as fk inner join sys.objects as o on fk.constraint_object_id = o.object_id inner join sys.tables as t on fk.parent_object_id = t.object_id inner join sys.columns as c on fk.parent_object_id = c.object_id and fk.parent_column_id = c.column_id inner join sys.columns as lc on fk.referenced_object_id = lc.object_id and fk.referenced_column_id = lc.column_id where fk.referenced_object_id = (select object_id from sys.tables where name = '" + table.TableName + "') and lc.name = '" + column["COLUMN_NAME"].ToString() + "'", sqlConnection);
                        sqlDataAdapterFK.SelectCommand.Connection.Open();
                        DataSet dataSetFK = new DataSet();
                        sqlDataAdapterFK.Fill(dataSetFK);
                        sqlDataAdapterFK.SelectCommand.Connection.Close();
                        table.IDColumn.FKs = new List<DBFK>();
                        foreach (DataRow columnFK in dataSetFK.Tables[0].Rows)
                        {
                            DBFK tempFK = new DBFK();
                            tempFK.FKName = columnFK["FK_NAME"].ToString();
                            tempFK.ColumnName = columnFK["LOCAL_COLUMN_NAME"].ToString();
                            tempFK.TargetTabel = columnFK["TABLE_NAME"].ToString();
                            tempFK.TargetColumn = columnFK["COLUMN_NAME"].ToString();
                            tempFK.FlagNull = columnFK["COLUMN_NULL"].ToString() == "1";
                            table.IDColumn.FKs.Add(tempFK);
                        }
                    }
                    else
                    {
                        DBColumn tempTable = new DBColumn();
                        tempTable.ColumnName = column["COLUMN_NAME"].ToString();
                        tempTable.SqlDataType = column["DATA_TYPE"].ToString().ToUpper();
                        tempTable.FlagNull = column["IS_NULLABLE"].ToString().ToUpper() == "YES";
                        if (column["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                        {
                            tempTable.MaxLenth = (int)column["CHARACTER_MAXIMUM_LENGTH"];
                        }
                        SqlDataAdapter sqlDataAdapterFK = new SqlDataAdapter("select O.Name AS FK_NAME,lc.name as LOCAL_COLUMN_NAME,t.name as TABLE_NAME,c.name as COLUMN_NAME, c.is_nullable as COLUMN_NULL from sys.foreign_key_columns as fk inner join sys.objects as o on fk.constraint_object_id = o.object_id inner join sys.tables as t on fk.parent_object_id = t.object_id inner join sys.columns as c on fk.parent_object_id = c.object_id and fk.parent_column_id = c.column_id inner join sys.columns as lc on fk.referenced_object_id = lc.object_id and fk.referenced_column_id = lc.column_id where fk.referenced_object_id = (select object_id from sys.tables where name = '" + table.TableName + "') and lc.name = '" + column["COLUMN_NAME"].ToString() + "'", sqlConnection);
                        sqlDataAdapterFK.SelectCommand.Connection.Open();
                        DataSet dataSetFK = new DataSet();
                        sqlDataAdapterFK.Fill(dataSetFK);
                        sqlDataAdapterFK.SelectCommand.Connection.Close();
                        tempTable.FKs = new List<DBFK>();
                        foreach (DataRow columnFK in dataSetFK.Tables[0].Rows)
                        {
                            DBFK tempFK = new DBFK();
                            tempFK.FKName = columnFK["FK_NAME"].ToString();
                            tempFK.ColumnName = columnFK["LOCAL_COLUMN_NAME"].ToString();
                            tempFK.TargetTabel = columnFK["TABLE_NAME"].ToString();
                            tempFK.TargetColumn = columnFK["COLUMN_NAME"].ToString();
                            tempFK.FlagNull = columnFK["COLUMN_NULL"].ToString() == "1";
                            tempTable.FKs.Add(tempFK);
                        }
                        table.Columns.Add(tempTable);
                    }
                }
                sqlDataAdapter = new SqlDataAdapter("select O.Name AS FK_NAME,lc.name as LOCAL_COLUMN_NAME, t.name as TABLE_NAME, c.name as COLUMN_NAME, lc.is_nullable AS COLUMN_NULL from sys.foreign_key_columns as fk inner join sys.objects as o on fk.constraint_object_id = o.object_id inner join sys.tables as t on fk.referenced_object_id = t.object_id inner join sys.columns as c on fk.referenced_object_id = c.object_id and fk.referenced_column_id = c.column_id inner join sys.columns as lc on fk.parent_object_id = lc.object_id and fk.parent_column_id = lc.column_id where fk.parent_object_id = (select object_id from sys.tables where name = '" + table.TableName + "')", sqlConnection);
                sqlDataAdapter.SelectCommand.Connection.Open();
                dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet);
                sqlDataAdapter.SelectCommand.Connection.Close();
                foreach (DataRow column in dataSet.Tables[0].Rows)
                {
                    DBFK tempFK = new DBFK();
                    tempFK.FKName = column["FK_NAME"].ToString();
                    tempFK.ColumnName = column["LOCAL_COLUMN_NAME"].ToString();
                    tempFK.TargetTabel = column["TABLE_NAME"].ToString();
                    tempFK.TargetColumn = column["COLUMN_NAME"].ToString();
                    tempFK.FlagNull = (bool)column["COLUMN_NULL"];
                    table.FKs.Add(tempFK);
                }
            }
            return tables;
        }
    }
}
