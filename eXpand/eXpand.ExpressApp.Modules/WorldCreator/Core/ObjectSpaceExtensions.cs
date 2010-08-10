using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.WorldCreator.Core
{
    public static class ObjectSpaceExtensions
    {
        public static T CreateWCObject<T>(this ObjectSpace objectSpace)
        {
            var findBussinessObjectType = WCTypesInfo.Instance.FindBussinessObjectType<T>();
            return (T)objectSpace.CreateObject(findBussinessObjectType);
        }

    }
}
