using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;
using Xpand.ExpressApp.FilterDataStore.Core;
using Xpand.ExpressApp.FilterDataStore.Win.Providers;

namespace Xpand.ExpressApp.FilterDataStore.Win {
    [ToolboxBitmap(typeof(FilterDataStoreWindowsFormsModule))]
    [ToolboxItem(true)]
    public sealed partial class FilterDataStoreWindowsFormsModule : FilterDataStoreModuleBase {
        public FilterDataStoreWindowsFormsModule() {
            InitializeComponent();
            _tablesDictionary = new Dictionary<string, Type>();
        }
        
        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (FilterProviderManager.IsRegistered)
                application.SetupComplete += ApplicationOnSetupComplete;
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            SkinFilterProvider skinFilterProvider = FilterProviderManager.Providers.OfType<SkinFilterProvider>().FirstOrDefault();
            if (skinFilterProvider != null)
                skinFilterProvider.FilterValue = ((IModelApplicationOptionsSkin)Application.Model.Options).Skin;
        }

        protected override bool? ProxyEventsSubscribed { get; set; }
    }
}