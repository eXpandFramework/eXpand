
using System.Web;

namespace Xpand.ExpressApp.FilterDataStore.Web {
    public sealed partial class FilterDataStoreAspNetModule : FilterDataStoreModuleBase {
        bool _proxyEventsSubscribed;

        public FilterDataStoreAspNetModule() {
            InitializeComponent();
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