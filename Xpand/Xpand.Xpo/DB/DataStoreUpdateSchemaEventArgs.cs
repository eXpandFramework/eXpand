using System;
using DevExpress.Xpo.DB;

namespace Xpand.Xpo.DB
{
    public class DataStoreUpdateSchemaEventArgs : EventArgs
    {
        public DataStoreUpdateSchemaEventArgs(bool dontCreateIfFirstTableNotExist, params DBTable[] tables)
        {
            DontCreateIfFirstTableNotExist = dontCreateIfFirstTableNotExist;
            Tables = tables;
        }
        public DBTable[] Tables { get; set; }
        public bool DontCreateIfFirstTableNotExist { get; set; }

        public UpdateSchemaResult UpdateSchemaResult { get; set; }

        

        public bool Updated { get; set; }
    }
}