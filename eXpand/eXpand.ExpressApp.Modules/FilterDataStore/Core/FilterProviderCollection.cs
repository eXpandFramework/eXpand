using System;
using System.Configuration.Provider;

namespace eXpand.ExpressApp.FilterDataStore.Core
{
    public class FilterProviderCollection : ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (!(provider is FilterProviderBase))
                throw new ArgumentException("The provider parameter must be of type MyProviderProvider.");

            base.Add(provider);
        }

        new public FilterProviderBase this[string name]
        {
            get { return (FilterProviderBase)base[name]; }
        }

        public void CopyTo(FilterProviderBase[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}