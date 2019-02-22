using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadSQLServerDB
{
    public class DBTable
    {
        public string TableName { get; set; }
        public string PKName { get; set; }
        public DBColumn IDColumn { get; set; }
        public List<DBColumn> Columns { get; set; }
        public List<DBFK> FKs { get; set; }

        public DBTable()
        {
            Columns = new List<DBColumn>();
            FKs = new List<DBFK>();
        }
    }

    public class DBColumn
    {
        public string ColumnName { get; set; }
        public string SqlDataType { get; set; }
        public int? MaxLenth { get; set; }
        public bool FlagNull { get; set; }
        public List<DBFK> FKs { get; set; }
    }

    public class DBFK
    {
        public string FKName { get; set; }
        public string ColumnName { get; set; }
        public string TargetTabel { get; set; }
        public string TargetColumn { get; set; }
        public bool FlagNull { get; set; }
    }
}
