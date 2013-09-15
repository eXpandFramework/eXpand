using System;
using System.Linq;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Fasterflect;

namespace Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers {
    public static class PersistentAttributeInfoQuery {
        public static TAttribute Find<TAttribute>(IPersistentMemberInfo persistentMemberInfo) where TAttribute : Attribute{
            AttributeInfoAttribute firstOrDefault =
                persistentMemberInfo.TypeAttributes.Select(info => info.Create()).FirstOrDefault(attributeInfo => attributeInfo.Constructor.DeclaringType == typeof (TAttribute));
            if (firstOrDefault!= null)
                return (TAttribute)firstOrDefault.Constructor.DeclaringType.CreateInstance(firstOrDefault.InitializedArgumentValues);
            return null;
        }
    }
}