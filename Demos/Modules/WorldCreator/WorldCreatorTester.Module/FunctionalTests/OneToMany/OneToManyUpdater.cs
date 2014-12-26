using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Xpo;

namespace WorldCreatorTester.Module.FunctionalTests.OneToMany {
    public class OneToManyUpdater:WorldCreatorUpdater {
        public OneToManyUpdater(Session session) : base(session){
        }

        public override void Update(){
            if (Session.FindObject<PersistentAssemblyInfo>(info => info.Name == "OneToMany") == null) {
                var persistentAssemblyInfo = new PersistentAssemblyInfo(Session){Name = "OneToMany"};

                var projectCassInfo = new PersistentClassInfo(Session);
                persistentAssemblyInfo.PersistentClassInfos.Add(projectCassInfo);
                projectCassInfo.Name = "Project";
                projectCassInfo.SetDefaultTemplate(TemplateType.Class);
                projectCassInfo.TypeAttributes.Add(new PersistentDefaultClassOptionsAttribute(Session));
                
                var persistentCoreTypeMemberInfo = new PersistentCoreTypeMemberInfo(Session);
                projectCassInfo.OwnMembers.Add(persistentCoreTypeMemberInfo);
                persistentCoreTypeMemberInfo.Name = "Name";
                persistentCoreTypeMemberInfo.DataType=DBColumnType.String;
                persistentCoreTypeMemberInfo.SetDefaultTemplate(TemplateType.ReadWriteMember);
                
                var contributorCassInfo = new PersistentClassInfo(Session);
                persistentAssemblyInfo.PersistentClassInfos.Add(contributorCassInfo);
                contributorCassInfo.Name = "Contributor";
                contributorCassInfo.SetDefaultTemplate(TemplateType.Class);

                persistentCoreTypeMemberInfo = new PersistentCoreTypeMemberInfo(Session);
                contributorCassInfo.OwnMembers.Add(persistentCoreTypeMemberInfo);
                persistentCoreTypeMemberInfo.Name = "Name";
                persistentCoreTypeMemberInfo.DataType = DBColumnType.String;
                persistentCoreTypeMemberInfo.SetDefaultTemplate(TemplateType.ReadWriteMember);

                var collectionMemberInfo = new PersistentCollectionMemberInfo(Session);
                projectCassInfo.OwnMembers.Add(collectionMemberInfo);
                collectionMemberInfo.CollectionClassInfo = contributorCassInfo;
                collectionMemberInfo.Name = "Contributors";
                var associationAttribute = new PersistentAssociationAttribute(Session);
                collectionMemberInfo.TypeAttributes.Add(associationAttribute);
                associationAttribute.AssociationName = "Project-Contributors";
                collectionMemberInfo.SetDefaultTemplate(TemplateType.XPCollectionMember);

                var referenceMemberInfo = new PersistentReferenceMemberInfo(Session);
                contributorCassInfo.OwnMembers.Add(referenceMemberInfo);
                referenceMemberInfo.Name = "Project";
                referenceMemberInfo.ReferenceClassInfo = projectCassInfo;
                associationAttribute = new PersistentAssociationAttribute(Session);
                referenceMemberInfo.TypeAttributes.Add(associationAttribute);
                associationAttribute.AssociationName = "Project-Contributors";
                referenceMemberInfo.SetDefaultTemplate(TemplateType.ReadWriteMember);

//                ObjectSpace.CommitChanges();
            }

        }
    }
}
