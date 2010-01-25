using System;
using DevExpress.Xpo.DB;

namespace eXpand.Xpo.DB
{
    public class DataStoreSelectDataEventArgs : EventArgs
    {
        public DataStoreSelectDataEventArgs(SelectStatement[] selects)
        {
            SelectStatements = selects;
        }
        public SelectStatement[] SelectStatements { get; set; }

        public bool Handled { get; set; }

        public SelectedData SelectData { get; set; }
    }
}