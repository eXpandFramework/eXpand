using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Configuration;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.FilterDataStore.Core {
    public static class FilterProviderManager {
        static FilterProviderBase defaultProvider;

        static bool isInitialized;

        static FilterProviderCollection providerCollection = new FilterProviderCollection();

        static FilterProviderManager() {
            Initialize();
        }

        public static FilterProviderBase Provider {
            get {
                if (!isInitialized) {
                    Initialize();
                }
                return defaultProvider;
            }
        }

        public static FilterProviderCollection Providers {
            get {
                if (!isInitialized) {
                    Initialize();
                }
                return providerCollection;
            }
        }

        public static FilterProviderBase GetFilterProvider(string filterMemberName) {
            FilterProviderBase provider = Providers.Cast<FilterProviderBase>().Where(
                probase => probase.FilterMemberName == filterMemberName).FirstOrDefault();
            if (provider != null && provider.FilterValue == null && !provider.UseFilterValueWhenNull)
                return null;
            return provider;
        }

        static void Initialize() {
            try {
                //Get the feature's configuration info
                var qc =
                    ConfigurationManager.GetSection("FilterProvider") as FilterProviderConfiguration;

                if (qc == null)
                    return;
                if (qc.DefaultProvider == null)
                    throw new ProviderException("You must specify a valid default provider.");

                //Instantiate the providers
                providerCollection = new FilterProviderCollection();
                ProvidersHelper.InstantiateProviders(qc.Providers, providerCollection, typeof (FilterProviderBase));
                providerCollection.SetReadOnly();
                defaultProvider = providerCollection[qc.DefaultProvider];
                if (defaultProvider == null) {
                    PropertyInformation information = qc.ElementInformation.Properties["defaultProvider"];
                    if (information != null)
                        throw new ConfigurationErrorsException(
                            "You must specify a default provider for the feature.",
                            information.Source,
                            information.LineNumber);
                }
            }
            catch (Exception ex) {
                isInitialized = true;

                Tracing.Tracer.LogError(ex);
                throw;
            }

            isInitialized = true; //error-free initialization
        }
    }
}