using System;

namespace eXpand.Xpo.AOP.DataStore
{
    public interface IDataStoreEvents
    {
        event EventHandler<StatementArgs> SelectionData;
        event EventHandler<StatementArgs> ModificationData;
        void InvokeSelect(StatementArgs e);
        void InvokeModify(StatementArgs e);
    }
}