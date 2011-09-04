using System;
using System.Linq;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers {
    public static class PersistentAttributeInfoQuery {
        public static TAttribute Find<TAttribute>(IPersistentMemberInfo persistentMemberInfo) where TAttribute : Attribute{
            AttributeInfoAttribute firstOrDefault =
                persistentMemberInfo.TypeAttributes.Select(info => info.Create()).Where(
                    attributeInfo => attributeInfo.Constructor.DeclaringType == typeof (TAttribute)).FirstOrDefault();
            if (firstOrDefault!= null)
                return (TAttribute)ReflectionHelper.CreateObject(firstOrDefault.Constructor.DeclaringType, firstOrDefault.InitializedArgumentValues);
            return null;
        }
    }
}