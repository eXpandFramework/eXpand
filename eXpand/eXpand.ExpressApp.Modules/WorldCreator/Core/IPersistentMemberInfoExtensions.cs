using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public static class IPersistentMemberInfoExtensions {
        public static void CreateAssociation(this IPersistentMemberInfo persistentMemberInfo, string associationName) {
            ObjectSpace objectSpace = ObjectSpace.FindObjectSpace(persistentMemberInfo);
            var attribute =ObjectSpaceExtensions.CreateWCObject<IPersistentAssociationAttribute>(objectSpace);
            attribute.AssociationName = associationName;
            persistentMemberInfo.TypeAttributes.Add(attribute);
        }

        public static bool IsAssociation(this IPersistentMemberInfo persistentMemberInfo) {
            return PersistentAttributeInfoQuery.Find<AssociationAttribute>(persistentMemberInfo) != null;
        }
    }
}