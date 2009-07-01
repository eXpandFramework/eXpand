using System;
using AopAlliance.Aop;

namespace eXpand.Xpo.AOP.DataStore
{
    public class DataStoreEventsMixin:IDataStoreEvents,IAdvice
    {
        public event EventHandler<StatementArgs> SelectionData;

        public void InvokeSelect(StatementArgs e)
        {
            EventHandler<StatementArgs> selectedDataHandler = SelectionData;
            if (selectedDataHandler != null) selectedDataHandler(this, e);
        }

        public void InvokeModify(StatementArgs e)
        {
            EventHandler<StatementArgs> selectedDataHandler = ModificationData;
            if (selectedDataHandler != null) selectedDataHandler(this, e);
        }

        public event EventHandler<StatementArgs> ModificationData;
    }
}