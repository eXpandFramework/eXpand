using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using Fasterflect;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public static class ObjectSpaceExtensions {
        public static object CreateWCObject(this IObjectSpace objectSpace, Type type) {
            var findBussinessObjectType = WCTypesInfo.Instance.FindBussinessObjectType(type);
            return findBussinessObjectType.CreateInstance(new object[] { ((XPObjectSpace)objectSpace).Session });
        }
        public static T CreateWCObject<T>(this IObjectSpace objectSpace) {
            return (T)CreateWCObject(objectSpace, typeof(T));
        }

    }
}
