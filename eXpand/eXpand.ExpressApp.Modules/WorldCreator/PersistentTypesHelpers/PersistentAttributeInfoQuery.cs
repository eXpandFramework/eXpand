using System;
using System.Linq;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers {
    public static class PersistentAttributeInfoQuery {
        public static TAttribute Find<TAttribute>(IPersistentMemberInfo persistentMemberInfo) where TAttribute : Attribute{
            AttributeInfo firstOrDefault =
                persistentMemberInfo.TypeAttributes.Select(info => info.Create()).Where(
                    attributeInfo => attributeInfo.Constructor.DeclaringType == typeof (TAttribute)).FirstOrDefault();
            if (firstOrDefault!= null)
                return (TAttribute)Activator.CreateInstance(firstOrDefault.Constructor.DeclaringType, firstOrDefault.InitializedArgumentValues);
            return null;
        }
    }
}