
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using DevExpress.Utils;

namespace Xpand.ExpressApp.FilterDataStore.Web {
    [ToolboxBitmap(typeof(FilterDataStoreAspNetModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class FilterDataStoreAspNetModule : FilterDataStoreModuleBase {
        bool _proxyEventsSubscribed;
        static FilterDataStoreAspNetModule() {
            TablesDictionary = new Dictionary<string, Type>();
        }
        public FilterDataStoreAspNetModule() {
            RequiredModuleTypes.Add(typeof(FilterDataStoreModule));
        }
        protected override bool? ProxyEventsSubscribed {
            get {
                bool result = false;
                if (HttpContext.Current != null)
                    bool.TryParse(HttpContext.Current.Application["ProxyEventsSubscribed"] + "", out result);
                _proxyEventsSubscribed = result;
                return _proxyEventsSubscribed;
            }
            set { HttpContext.Current.Application["ProxyEventsSubscribed"] = value; }
        }
    }
}