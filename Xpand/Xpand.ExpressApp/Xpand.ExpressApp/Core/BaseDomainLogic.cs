using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.Core {
    public abstract class BaseDomainLogic {
        protected static IEnumerable<Type> FindTypeDescenants(Type type) {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
            return ReflectionHelper.FindTypeDescendants(typeInfo).Where(info => !info.IsAbstract).Select(info => info.Type);
        }
    }
}
