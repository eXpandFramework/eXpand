using System;
using System.Collections;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Configuration;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.FilterDataStore.Core {
    public static class FilterProviderManager {
        private const string FilterProvider = "FilterProvider";

        private static IValueManager<FilterProviderCollection> _valueManager;
        private static IValueManager<FilterProviderCollection> ValueManager {
            get {
                return _valueManager ??
                       (_valueManager =
                        DevExpress.Persistent.Base.ValueManager.GetValueManager<FilterProviderCollection>("FilterProviderManager"));
            }
        }

        public static FilterProviderCollection Providers {
            get {
                if (ValueManager.Value == null)
                    Initialize();
                return ValueManager.Value;
            }
        }

        public static bool IsRegistered {
            get{
                try{
                    return (ConfigurationManager.GetSection(FilterProvider) as FilterProviderConfiguration) != null&&!XpandModuleBase.IsLoadingExternalModel();
                }
                catch (Exception e){
                    Tracing.Tracer.LogError(e);
                    return false;
                }
            }

        }

        public static FilterProviderBase GetFilterProvider(string tableName, string filterMemberName, StatementContext modify) {
            FilterProviderBase provider = Providers.Cast<FilterProviderBase>().FirstOrDefault(probase => (probase.ObjectType == null || probase.ObjectType.Name == tableName) && probase.FilterMemberName == filterMemberName && (probase.StatementContext == modify || probase.StatementContext == StatementContext.Both));
            if (provider != null && HasFilterValue(provider) && !provider.UseFilterValueWhenNull)
                return null;
            return provider;
        }

        static bool HasFilterValue(FilterProviderBase provider) {
            return provider.FilterValue == null || (provider.FilterValue is ICollection && ((ICollection)provider.FilterValue).Count == 0);
        }

        internal static void Initialize() {
            try {
                var qc = ConfigurationManager.GetSection(FilterProvider) as FilterProviderConfiguration;

                if (qc == null)
                    throw new ConfigurationErrorsException(
                        "FilterProvider section not found in your application configuration file");
                if (qc.DefaultProvider == null)
                    throw new ProviderException("You must specify a valid default provider.");
                ValueManager.Value = new FilterProviderCollection();
                ProvidersHelper.InstantiateProviders(qc.Providers, ValueManager.Value, typeof(FilterProviderBase));
                ValueManager.Value.SetReadOnly();
            } catch (Exception ex) {
                Tracing.Tracer.LogError(ex);
                throw;
            }
        }




    }
}