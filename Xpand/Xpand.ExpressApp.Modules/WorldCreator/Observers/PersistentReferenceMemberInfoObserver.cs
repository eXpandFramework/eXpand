using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using System.Linq;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Observers
{
    public class PersistentReferenceMemberInfoObserver:ObjectObserver<IPersistentReferenceMemberInfo>
    {
        public PersistentReferenceMemberInfoObserver(IObjectSpace objectSpace) : base(objectSpace) {
        }
        protected override void OnSaving(ObjectManipulatingEventArgs<IPersistentReferenceMemberInfo> e)
        {
            base.OnSaving(e);
            if (e.Object.RelationType == RelationType.OneToMany&&e.Object.IsAssociation())
                createTheManyPart(e.Object);
        }

        void createTheManyPart(IPersistentReferenceMemberInfo persistentReferenceMemberInfo) {
            IPersistentClassInfo classInfo = PersistentClassInfoQuery.Find(((ObjectSpace)ObjectSpace).Session, persistentReferenceMemberInfo.ReferenceTypeFullName);
            string collectionPropertyName = persistentReferenceMemberInfo.Name + "s";
            if (classInfo != null&&classInfo.OwnMembers.Where(info => info.Name==collectionPropertyName).FirstOrDefault()==null) {
                var associationAttribute =PersistentAttributeInfoQuery.Find<AssociationAttribute>(persistentReferenceMemberInfo);
                classInfo.CreateCollection(persistentReferenceMemberInfo.Owner.PersistentAssemblyInfo.Name ,
                                           persistentReferenceMemberInfo.Owner.Name).CreateAssociation(
                    associationAttribute.Name);
            }
        }
    }
}
