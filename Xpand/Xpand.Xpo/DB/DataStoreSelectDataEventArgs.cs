using System;
using DevExpress.Xpo.DB;

namespace Xpand.Xpo.DB
{
    public class DataStoreSelectDataEventArgs : EventArgs
    {
        public DataStoreSelectDataEventArgs(SelectStatement[] selects)
        {
            SelectStatements = selects;
        }
        public SelectStatement[] SelectStatements { get; set; }

        public bool Handled { get; set; }

        public SelectedData SelectedData { get; set; }
    }
}