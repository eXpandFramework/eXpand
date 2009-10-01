using System;
using System.Configuration.Provider;

namespace eXpand.ExpressApp.FilterDataStore.Core
{
    public abstract class FilterProviderBase: ProviderBase
    {
        public abstract object FilterValue { get; }
        public abstract string FilterMemberName { get; }
        public abstract int FilterMemberSize { get; }
        public abstract bool FilterMemberIndexed { get; }
        public abstract bool UseFilterValueWhenNull { get; }
        public abstract Type FilterMemberType { get; }
        
    }
}