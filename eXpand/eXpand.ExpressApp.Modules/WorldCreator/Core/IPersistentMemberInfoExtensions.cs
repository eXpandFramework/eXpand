using System;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public static class IPersistentMemberInfoExtensions {
        public static void CreateAssociation(this IPersistentMemberInfo persistentMemberInfo, string associationName) {
            var attribute =
                (IPersistentAssociationAttribute)
                Activator.CreateInstance(TypesInfo.Instance.PersistentAssociationAttributeType,
                                         persistentMemberInfo.Session);
            attribute.AssociationName = associationName;
            persistentMemberInfo.TypeAttributes.Add(attribute);
        }

        public static bool IsAssociation(this IPersistentMemberInfo persistentMemberInfo) {
            return PersistentAttributeInfoQuery.Find<AssociationAttribute>(persistentMemberInfo) != null;
        }
    }
}