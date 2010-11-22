using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public static class IPersistentMemberInfoExtensions {
        public static void CreateAssociation(this IPersistentMemberInfo persistentMemberInfo, string associationName) {
            var objectSpace = ObjectSpace.FindObjectSpaceByObject(persistentMemberInfo);
            var attribute = ObjectSpaceExtensions.CreateWCObject<IPersistentAssociationAttribute>(objectSpace);
            attribute.AssociationName = associationName;
            persistentMemberInfo.TypeAttributes.Add(attribute);
        }

        public static bool IsAssociation(this IPersistentMemberInfo persistentMemberInfo) {
            return PersistentAttributeInfoQuery.Find<AssociationAttribute>(persistentMemberInfo) != null;
        }
    }
}