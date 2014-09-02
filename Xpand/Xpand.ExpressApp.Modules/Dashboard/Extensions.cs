﻿using System;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Dashboard {
    public static class Extensions {
        public static object CreateDashboardDataSource(this XafApplication application, Type objectType) {
            var space = application.CreateObjectSpace(objectType);
            return new ProxyCollection(space, space.TypesInfo.FindTypeInfo(objectType), space.GetObjects(objectType));
        }
    }
}
