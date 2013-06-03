using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.EF;
using EFDemo.Module.Data;

namespace EFDemo.Module {
    public static class XafAppExtensions {
        public static void CreateCustomProvider(this XafApplication application, string connectionString, TypesInfo typesInfo, List<IObjectSpaceProvider> objectSpaceProviders, Action createBaseCustomObjectSpaceProvider) {
            var objectSpaceProviderCf = new EFObjectSpaceProviderCF(typeof(EFDemoDbContext), typesInfo, null, connectionString);
            objectSpaceProviderCf.UpdateSchema();
            createBaseCustomObjectSpaceProvider.Invoke();
            objectSpaceProviders.Add(objectSpaceProviderCf);
        }

    }
}