using System;
using System.Collections;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Configuration;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.FilterDataStore.Core {
    public static class FilterProviderManager {

        private static IValueManager<FilterProviderCollection> _valueManager;
        private static IValueManager<FilterProviderCollection> ValueManager {
            get {
                return _valueManager ??
                       (_valueManager =
                        DevExpress.Persistent.Base.ValueManager.CreateValueManager<FilterProviderCollection>());
            }
        }

        public static FilterProviderCollection Providers {
            get {
                if (ValueManager.Value == null)
                    Initialize();
                return ValueManager.Value;
            }
        }

        public static FilterProviderBase GetFilterProvider(string filterMemberName, StatementContext modify) {
            FilterProviderBase provider = Providers.Cast<FilterProviderBase>().Where(
                probase => probase.FilterMemberName == filterMemberName && (probase.StatementContext == modify || probase.StatementContext == StatementContext.Both)).FirstOrDefault();
            if (provider != null && HasFilterValue(provider) && !provider.UseFilterValueWhenNull)
                return null;
            return provider;
        }

        static bool HasFilterValue(FilterProviderBase provider) {
            return provider.FilterValue == null || (provider.FilterValue is ICollection && ((ICollection) provider.FilterValue).Count==0);
        }

        internal static void Initialize() {
            try {
                var qc =ConfigurationManager.GetSection("FilterProvider") as FilterProviderConfiguration;

                if (qc == null)
                    throw new ConfigurationErrorsException(
                        "FilterProvider section not found in your application configuration file");
                if (qc.DefaultProvider == null)
                    throw new ProviderException("You must specify a valid default provider.");
                ValueManager.Value = new FilterProviderCollection();
                ProvidersHelper.InstantiateProviders(qc.Providers, ValueManager.Value, typeof(FilterProviderBase));
                ValueManager.Value.SetReadOnly();
            }
            catch (Exception ex) {
                Tracing.Tracer.LogError(ex);
                throw;
            }
        }
    }
}