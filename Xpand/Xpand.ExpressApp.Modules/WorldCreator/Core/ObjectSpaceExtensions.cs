using System;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public static class ObjectSpaceExtensions {
        public static object CreateWCObject(this IObjectSpace objectSpace, Type type) {
            var findBussinessObjectType = WCTypesInfo.Instance.FindBussinessObjectType(type);
            return Activator.CreateInstance(findBussinessObjectType, new object[] { ((ObjectSpace)objectSpace).Session });
        }
        public static T CreateWCObject<T>(this IObjectSpace objectSpace) {
            return (T)CreateWCObject(objectSpace, typeof(T));
        }

    }
}
