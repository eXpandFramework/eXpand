using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.FilterDataStore.Core;
using Xpand.ExpressApp.FilterDataStore.Model;

namespace Xpand.ExpressApp.FilterDataStore.DomainLogic
{
    [DomainLogic(typeof(IModelDisabledDataStoreFilter))]
    public class ModelDisabledDataStoreFilterDomainLogic
    {
        public static List<string> Get_DataStoreFilters(IModelDisabledDataStoreFilter disabledDataStoreFilter)
        {
            return FilterProviderManager.Providers.OfType<FilterProviderBase>().Select(@base => @base.Name).ToList();
            
        }
    }
}
