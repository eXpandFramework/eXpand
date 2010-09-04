using System;
using DevExpress.Xpo.DB;

namespace Xpand.Xpo.DB {
    public class DataStoreModifyDataEventArgs : EventArgs {
        public DataStoreModifyDataEventArgs(ModificationStatement[] dmlStatements) {
            ModificationStatements = dmlStatements;
        }

        public ModificationStatement[] ModificationStatements { get; set; }

        public ModificationResult ModificationResult { get; set; }

        public bool Handled { get; set; }
    }
}