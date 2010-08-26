using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.WorldCreator.Core
{
    public static class ObjectSpaceExtensions
    {
        public static object CreateWCObject(this ObjectSpace objectSpace,Type type) {
            var findBussinessObjectType = WCTypesInfo.Instance.FindBussinessObjectType(type);
            return objectSpace.CreateObject(findBussinessObjectType);
        }
        public static T CreateWCObject<T>(this ObjectSpace objectSpace)
        {
            return (T) CreateWCObject(objectSpace,typeof(T));
        }

    }
}
