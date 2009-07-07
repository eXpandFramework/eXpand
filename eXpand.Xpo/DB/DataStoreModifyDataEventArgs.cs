using System;
using DevExpress.Xpo.DB;

namespace eXpand.Xpo.DB
{
    public class DataStoreModifyDataEventArgs : EventArgs
    {
        public DataStoreModifyDataEventArgs(ModificationStatement[] dmlStatements)
        {
            ModificationStatements = dmlStatements;
        }
        public ModificationStatement[] ModificationStatements { get; set; }
    }
}